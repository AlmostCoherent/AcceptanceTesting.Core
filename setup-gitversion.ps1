# Initial GitVersion setup script for Windows

Write-Host "Setting up GitVersion for AcceptanceTesting.Core repository..." -ForegroundColor Green

# Check if we already have version tags
$existingTags = git tag -l "v*"
if ($existingTags) {
    Write-Host "Version tags already exist:" -ForegroundColor Yellow
    $existingTags | Sort-Object
    Write-Host ""
    Write-Host "Current GitVersion calculation:" -ForegroundColor Cyan
    
    if (Get-Command dotnet-gitversion -ErrorAction SilentlyContinue) {
        $currentVersion = dotnet-gitversion /showvariable NuGetVersion
        Write-Host $currentVersion -ForegroundColor Green
    } else {
        Write-Host "GitVersion tool not installed. Install with: dotnet tool install --global GitVersion.Tool" -ForegroundColor Red
    }
} else {
    Write-Host "No version tags found. Creating initial version tag v1.0.0..." -ForegroundColor Yellow
    
    # Create initial tag
    git tag v1.0.0
    Write-Host "Created tag v1.0.0" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "To push the tag to remote repository, run:" -ForegroundColor Cyan
    Write-Host "git push origin v1.0.0" -ForegroundColor White
    Write-Host ""
    Write-Host "Current GitVersion calculation:" -ForegroundColor Cyan
    
    if (Get-Command dotnet-gitversion -ErrorAction SilentlyContinue) {
        $currentVersion = dotnet-gitversion /showvariable NuGetVersion
        Write-Host $currentVersion -ForegroundColor Green
    } else {
        Write-Host "GitVersion tool not installed. Install with: dotnet tool install --global GitVersion.Tool" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "GitVersion setup complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Push any new tags: git push origin --tags" -ForegroundColor White
Write-Host "2. Create branches following the pattern: feature/*, release/*, hotfix/*" -ForegroundColor White
Write-Host "3. The CI/CD pipeline will automatically version packages based on your Git history" -ForegroundColor White