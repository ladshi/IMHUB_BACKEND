# PowerShell script to verify user activation after approval
param(
    [string]$Email = "ladhu123@gmail.com",
    [string]$ConnectionString = ""
)

Write-Host "=== User Activation Verification ===" -ForegroundColor Cyan
Write-Host ""

# Try different connection strings if not provided
if ([string]::IsNullOrEmpty($ConnectionString)) {
    $connectionStrings = @(
        "Server=localhost;Database=IMHubDB;Trusted_Connection=true;TrustServerCertificate=true;",
        "Server=(localdb)\mssqllocaldb;Database=IMHubDB;Trusted_Connection=True;TrustServerCertificate=True;",
        "Server=.;Database=IMHubDB;Trusted_Connection=true;TrustServerCertificate=true;"
    )
} else {
    $connectionStrings = @($ConnectionString)
}

$connected = $false
$connection = $null

foreach ($connStr in $connectionStrings) {
    try {
        Write-Host "Trying connection: $($connStr.Split(';')[0])..." -ForegroundColor Yellow
        $connection = New-Object System.Data.SqlClient.SqlConnection($connStr)
        $connection.Open()
        $connected = $true
        Write-Host "Connected successfully!" -ForegroundColor Green
        Write-Host ""
        break
    } catch {
        Write-Host "  Failed: $($_.Exception.Message)" -ForegroundColor Red
        if ($connection -ne $null) {
            $connection.Dispose()
            $connection = $null
        }
    }
}

if (-not $connected) {
    Write-Host ""
    Write-Host "ERROR: Could not connect to database with any connection string." -ForegroundColor Red
    Write-Host ""
    Write-Host "Please try one of these:" -ForegroundColor Yellow
    Write-Host "  1. Make sure SQL Server is running"
    Write-Host "  2. Check if database 'IMHubDB' exists"
    Write-Host "  3. Run with custom connection string:"
    Write-Host "     .\verify-user-activation.ps1 -Email 'your@email.com' -ConnectionString 'Server=YOUR_SERVER;Database=IMHubDB;...'"
    exit 1
}

try {
    # Check user details
    $userQuery = "SELECT u.Id, u.Name, u.Email, u.IsActive, u.IsDeleted, u.OrganizationId, o.Name AS OrganizationName, o.IsActive AS OrganizationIsActive FROM Users u LEFT JOIN Organizations o ON u.OrganizationId = o.Id WHERE u.Email = @EmailParam"
    
    $userCommand = New-Object System.Data.SqlClient.SqlCommand($userQuery, $connection)
    $param = $userCommand.Parameters.Add("@EmailParam", [System.Data.SqlDbType]::NVarChar)
    $param.Value = $Email
    
    $userReader = $userCommand.ExecuteReader()
    
    if ($userReader.HasRows) {
        $userReader.Read()
        
        Write-Host "User Details:" -ForegroundColor Yellow
        Write-Host "  ID: $($userReader['Id'])"
        Write-Host "  Name: $($userReader['Name'])"
        Write-Host "  Email: $($userReader['Email'])"
        Write-Host "  IsActive: $($userReader['IsActive'])"
        Write-Host "  IsDeleted: $($userReader['IsDeleted'])"
        Write-Host "  OrganizationId: $($userReader['OrganizationId'])"
        Write-Host "  OrganizationName: $($userReader['OrganizationName'])"
        Write-Host "  OrganizationIsActive: $($userReader['OrganizationIsActive'])"
        Write-Host ""
        
        $userIsActive = [bool]$userReader['IsActive']
        $orgIsActive = [bool]$userReader['OrganizationIsActive']
        $isDeleted = [bool]$userReader['IsDeleted']
        
        $userReader.Close()
        
        if ($isDeleted) {
            Write-Host "ERROR: User is deleted (IsDeleted = true)" -ForegroundColor Red
            exit 1
        } elseif (-not $userIsActive) {
            Write-Host "ERROR: User is inactive (IsActive = false)" -ForegroundColor Red
            Write-Host ""
            Write-Host "To fix this:" -ForegroundColor Yellow
            Write-Host "  1. Make sure the organization is approved via admin dashboard"
            Write-Host "  2. Run the approval API again: POST /api/superadmin/organizations/{id}/approve"
            Write-Host "  3. Or manually activate in SQL: UPDATE Users SET IsActive = 1 WHERE Email = '$Email'"
            exit 1
        } elseif (-not $orgIsActive) {
            Write-Host "ERROR: Organization is inactive (OrganizationIsActive = false)" -ForegroundColor Red
            Write-Host ""
            Write-Host "To fix this:" -ForegroundColor Yellow
            Write-Host "  1. Approve the organization via admin dashboard"
            Write-Host "  2. This should automatically activate all users"
            exit 1
        } else {
            Write-Host "SUCCESS: User is active and organization is active" -ForegroundColor Green
            Write-Host "User should be able to login successfully!" -ForegroundColor Green
            exit 0
        }
    } else {
        Write-Host "ERROR: User not found with email: $Email" -ForegroundColor Red
        Write-Host ""
        Write-Host "Please check:" -ForegroundColor Yellow
        Write-Host "  1. Email address is correct"
        Write-Host "  2. User exists in the database"
        exit 1
    }
    
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} finally {
    if ($connection -ne $null) {
        $connection.Close()
        $connection.Dispose()
    }
}
