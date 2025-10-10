# NuGet Publishing Workflow

This workflow automatically builds and publishes NuGet packages for the Testing.Acceptance solution.

## Features

- **Automatic Build**: Builds the solution on every push and pull request
- **Testing**: Runs all tests before packaging
- **Package Creation**: Creates NuGet packages with symbols
- **Multi-Feed Publishing**: Publishes to both NuGet.org and GitHub Packages
- **Version Control**: Supports tag-based versioning (e.g., v1.0.0)

## Setup Instructions

### 1. NuGet.org Publishing (Optional)

To publish to NuGet.org, you need to set up an API key:

1. Create an account at [NuGet.org](https://www.nuget.org/)
2. Generate an API key from your account settings
3. Add the API key as a GitHub secret named `NUGET_API_KEY`:
   - Go to your repository settings
   - Navigate to "Secrets and variables" → "Actions"
   - Click "New repository secret"
   - Name: `NUGET_API_KEY`
   - Value: Your NuGet.org API key

### 2. GitHub Packages Publishing (Automatic)

Publishing to GitHub Packages uses the built-in `GITHUB_TOKEN` and requires no additional setup. However, to install packages from GitHub Packages, users will need to authenticate with a GitHub Personal Access Token (PAT):

```bash
# Option 1: Store credentials (not recommended for production)
dotnet nuget add source --username USERNAME --password GITHUB_PAT --store-password-in-clear-text --name github "https://nuget.pkg.github.com/AlmostCoherent/index.json"

# Option 2: Use environment variables (recommended)
export NUGET_AUTH_TOKEN=your_github_pat
dotnet nuget add source --username USERNAME --password "%NUGET_AUTH_TOKEN%" --name github "https://nuget.pkg.github.com/AlmostCoherent/index.json"
```

> **Security Best Practice**: For production environments, use secure credential providers like [Azure Artifacts Credential Provider](https://github.com/microsoft/artifacts-credprovider) instead of storing credentials in plain text.

## Workflow Triggers

- **Push to main**: Builds, tests, packs, and publishes packages
- **Push tags (v*)**: Builds, tests, packs, and publishes packages (for versioned releases)
- **Pull requests**: Builds, tests, and packs (does not publish)
- **Manual dispatch**: Can be triggered manually from the Actions tab

## Versioning

The package version is defined in the project file (`Testing.Acceptance.csproj`). To create a new release:

1. Update the `<Version>` property in the project file
2. Commit and push to main, or create a tag:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

## Output

Successful builds produce:
- NuGet package (.nupkg)
- Symbol package (.snupkg)
- Artifacts uploaded to GitHub Actions (retained for 30 days)
