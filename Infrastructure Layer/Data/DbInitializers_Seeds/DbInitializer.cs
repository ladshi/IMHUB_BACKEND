using IMHub.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using System.Data;

namespace IMHub.Infrastructure.Data.DbInitializers_Seeds
{
    public class DbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DbInitializer> _logger;
        private readonly IEnumerable<ICustomSeeder> _seeders;

        public DbInitializer(
            ApplicationDbContext context,
            ILogger<DbInitializer> logger,
            IEnumerable<ICustomSeeder> seeders)
        {
            _context = context;
            _logger = logger;
            _seeders = seeders;
        }

        public async Task RunAsync()
        {
            bool canConnect = false;

            try
            {
                if (_context.Database.IsSqlServer())
                {
                    // Check if we can connect to database
                    canConnect = await _context.Database.CanConnectAsync();
                    
                    if (!canConnect)
                    {
                        _logger.LogInformation("Database does not exist. It will be created by migrations.");
                    }
                    else
                    {
                        _logger.LogInformation("Database connection successful. Checking migrations...");
                    }
                    
                    // Check if key tables exist (indicating database is already set up)
                    var keyTablesExist = await CheckKeyTablesExistAsync();
                    
                    if (keyTablesExist)
                    {
                        _logger.LogInformation("Key tables already exist. Checking migration history...");
                        
                        // Try to sync migration history if needed
                        await SyncMigrationHistoryIfNeededAsync();
                        
                        // Try to apply any pending migrations
                        try
                        {
                            await _context.Database.MigrateAsync();
                            _logger.LogInformation("Database migrations applied successfully");
                        }
                        catch (SqlException sqlEx) when (sqlEx.Number == 2714) // Object already exists
                        {
                            _logger.LogInformation("All migrations appear to be applied. Database is up to date.");
                        }
                        catch (Exception migrationEx)
                        {
                            _logger.LogWarning(migrationEx, "Migration check completed with warnings, but database appears functional.");
                        }
                    }
                    else
                    {
                        // No tables exist, apply migrations normally
                        try
                        {
                            await _context.Database.MigrateAsync();
                            _logger.LogInformation("Database migrations applied successfully");
                        }
                        catch (Exception migrationEx)
                        {
                            _logger.LogError(migrationEx, "Failed to apply migrations to empty database.");
                            throw; // Re-throw for empty database - this is a real error
                        }
                    }
                }

                // Always try to run seeders if we can connect to database
                if (canConnect || await _context.Database.CanConnectAsync())
                {
                    _logger.LogInformation("Starting database seeding...");
                    foreach (var seeder in _seeders)
                    {
                        try
                        {
                            await seeder.InitializeAsync();
                        }
                        catch (Exception seederEx)
                        {
                            _logger.LogError(seederEx, "Error in seeder: {SeederType}", seeder.GetType().Name);
                            // Continue with other seeders even if one fails
                        }
                    }
                    _logger.LogInformation("Database seeding completed");
                }
                else
                {
                    _logger.LogError("Cannot connect to database. Skipping seeding.");
                    throw new InvalidOperationException("Cannot connect to database");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                try
                {
                    var connectionString = _context.Database.GetDbConnection().ConnectionString;
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        // Mask password in connection string for security
                        var maskedConnectionString = connectionString.Contains("Password", StringComparison.OrdinalIgnoreCase)
                            ? connectionString.Substring(0, Math.Min(50, connectionString.Length)) + "..."
                            : connectionString;
                        _logger.LogError("Connection String: {ConnectionString}", maskedConnectionString);
                    }
                    else
                    {
                        _logger.LogError("Connection String: Not available");
                    }
                }
                catch
                {
                    _logger.LogError("Could not retrieve connection string");
                }
                _logger.LogError("Please ensure:");
                _logger.LogError("1. SQL Server is running");
                _logger.LogError("2. Database 'IMHubDB' exists or can be created");
                _logger.LogError("3. Windows user has SQL Server permissions");
                _logger.LogError("4. Connection string in appsettings.Development.json is correct");
                
                // Only throw if we couldn't connect at all
                if (!canConnect && !await _context.Database.CanConnectAsync())
                {
                    throw;
                }
                // Otherwise, log error but don't throw - seeders might have run
            }
        }

        private async Task<bool> CheckKeyTablesExistAsync()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME IN ('Roles', 'Users', 'Organizations', 'PlatformAdmins')";
                
                var result = await command.ExecuteScalarAsync();
                var tableCount = Convert.ToInt32(result);
                
                return tableCount >= 2; // At least 2 key tables exist
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not check if tables exist. Assuming they don't.");
                return false;
            }
        }

        private async Task SyncMigrationHistoryIfNeededAsync()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                // Check if migration history table exists
                using var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = '__EFMigrationsHistory'";
                
                var historyTableExists = Convert.ToInt32(await checkCommand.ExecuteScalarAsync()) > 0;

                if (!historyTableExists)
                {
                    _logger.LogInformation("Migration history table does not exist. Creating it...");
                    // Create migration history table
                    using var createCommand = connection.CreateCommand();
                    createCommand.CommandText = @"
                        CREATE TABLE [__EFMigrationsHistory] (
                            [MigrationId] nvarchar(150) NOT NULL,
                            [ProductVersion] nvarchar(32) NOT NULL,
                            CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                        )";
                    await createCommand.ExecuteNonQueryAsync();
                    _logger.LogInformation("Migration history table created.");
                }

                // Check if initial migration is recorded
                using var checkMigrationCommand = connection.CreateCommand();
                checkMigrationCommand.CommandText = @"
                    SELECT COUNT(*) 
                    FROM [__EFMigrationsHistory] 
                    WHERE [MigrationId] = '20251130205959_IntialCreate'";
                
                var migrationExists = Convert.ToInt32(await checkMigrationCommand.ExecuteScalarAsync()) > 0;

                if (!migrationExists && historyTableExists)
                {
                    _logger.LogInformation("Initial migration not found in history. Marking as applied since tables exist...");
                    // Mark initial migration as applied
                    using var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = @"
                        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                        VALUES ('20251130205959_IntialCreate', '9.0.8')";
                    await insertCommand.ExecuteNonQueryAsync();
                    _logger.LogInformation("Initial migration marked as applied in history.");
                }

                // Check for FixPendingModelChanges migration
                using var checkFixCommand = connection.CreateCommand();
                checkFixCommand.CommandText = @"
                    SELECT COUNT(*) 
                    FROM [__EFMigrationsHistory] 
                    WHERE [MigrationId] = '20251204184327_FixPendingModelChanges'";
                
                var fixMigrationExists = Convert.ToInt32(await checkFixCommand.ExecuteScalarAsync()) > 0;

                if (!fixMigrationExists && historyTableExists)
                {
                    _logger.LogInformation("FixPendingModelChanges migration not found. Marking as applied...");
                    using var insertFixCommand = connection.CreateCommand();
                    insertFixCommand.CommandText = @"
                        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                        VALUES ('20251204184327_FixPendingModelChanges', '9.0.8')";
                    await insertFixCommand.ExecuteNonQueryAsync();
                    _logger.LogInformation("FixPendingModelChanges migration marked as applied.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not sync migration history. This is usually safe to ignore.");
            }
        }
    }
}
