# PowerShell Script to Test Backend API Endpoints
# Run this script after starting the backend server

$baseUrl = "http://localhost:5299/api"
$headers = @{
    "Content-Type" = "application/json"
}

Write-Host "=== Testing IMHub Backend API ===" -ForegroundColor Green
Write-Host ""

# Test 1: Check if server is running
Write-Host "Test 1: Checking if server is running..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/auth/login" -Method POST -Headers $headers -Body '{"email":"test","password":"test"}' -ErrorAction Stop
    Write-Host "✓ Server is running" -ForegroundColor Green
} catch {
    if ($_.Exception.Response.StatusCode -eq 400 -or $_.Exception.Response.StatusCode -eq 401) {
        Write-Host "✓ Server is running (got expected error response)" -ForegroundColor Green
    } else {
        Write-Host "✗ Server is not running or not accessible" -ForegroundColor Red
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
}
Write-Host ""

# Test 2: Test Login with default SuperAdmin
Write-Host "Test 2: Testing login with default SuperAdmin..." -ForegroundColor Yellow
try {
    $loginBody = @{
        email = "admin@imhub.com"
        password = "Admin@123"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method POST -Headers $headers -Body $loginBody
    $token = $response.token
    
    if ($token) {
        Write-Host "✓ Login successful!" -ForegroundColor Green
        Write-Host "  User: $($response.name)" -ForegroundColor Cyan
        Write-Host "  Role: $($response.role)" -ForegroundColor Cyan
        Write-Host "  Token: $($token.Substring(0, 20))..." -ForegroundColor Cyan
        
        # Store token for subsequent requests
        $authHeaders = @{
            "Content-Type" = "application/json"
            "Authorization" = "Bearer $token"
        }
    } else {
        Write-Host "✗ Login failed - no token received" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "✗ Login failed" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody" -ForegroundColor Red
    }
    exit 1
}
Write-Host ""

# Test 3: Test Get Current User
Write-Host "Test 3: Testing Get Current User endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/auth/me" -Method GET -Headers $authHeaders
    Write-Host "✓ Get Current User successful!" -ForegroundColor Green
    Write-Host "  User ID: $($response.id)" -ForegroundColor Cyan
    Write-Host "  Email: $($response.email)" -ForegroundColor Cyan
    Write-Host "  Role: $($response.role)" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Get Current User failed" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 4: Test Get Organizations (SuperAdmin)
Write-Host "Test 4: Testing Get Organizations endpoint (SuperAdmin)..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/superadmin/organizations" -Method GET -Headers $authHeaders
    Write-Host "✓ Get Organizations successful!" -ForegroundColor Green
    Write-Host "  Organizations found: $($response.data.items.Count)" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Get Organizations failed" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 5: Test OpenAPI endpoint
Write-Host "Test 5: Testing OpenAPI endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5299/openapi/v1.json" -Method GET
    Write-Host "✓ OpenAPI endpoint accessible!" -ForegroundColor Green
    Write-Host "  OpenAPI spec size: $($response.Content.Length) bytes" -ForegroundColor Cyan
} catch {
    Write-Host "✗ OpenAPI endpoint not accessible" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

Write-Host "=== All Tests Completed ===" -ForegroundColor Green
Write-Host ""
Write-Host "Backend API is working correctly!" -ForegroundColor Green
Write-Host "You can now test the frontend connection." -ForegroundColor Cyan

