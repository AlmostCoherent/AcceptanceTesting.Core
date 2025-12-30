#!/bin/bash
# Initial GitVersion setup script

echo "Setting up GitVersion for AcceptanceTesting.Core repository..."

# Check if we already have version tags
if git tag -l "v*" | grep -q "v"; then
    echo "Version tags already exist:"
    git tag -l "v*" | sort -V
    echo ""
    echo "Current GitVersion calculation:"
    if command -v dotnet-gitversion &> /dev/null; then
        dotnet-gitversion /showvariable NuGetVersion
    else
        echo "GitVersion tool not installed. Install with: dotnet tool install --global GitVersion.Tool"
    fi
else
    echo "No version tags found. Creating initial version tag v1.0.0..."
    
    # Create initial tag
    git tag v1.0.0
    echo "Created tag v1.0.0"
    
    echo ""
    echo "To push the tag to remote repository, run:"
    echo "git push origin v1.0.0"
    echo ""
    echo "Current GitVersion calculation:"
    if command -v dotnet-gitversion &> /dev/null; then
        dotnet-gitversion /showvariable NuGetVersion
    else
        echo "GitVersion tool not installed. Install with: dotnet tool install --global GitVersion.Tool"
    fi
fi

echo ""
echo "GitVersion setup complete!"
echo ""
echo "Next steps:"
echo "1. Push any new tags: git push origin --tags"
echo "2. Create branches following the pattern: feature/*, release/*, hotfix/*"
echo "3. The CI/CD pipeline will automatically version packages based on your Git history"