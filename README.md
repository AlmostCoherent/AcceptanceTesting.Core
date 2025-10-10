# AcceptanceTesting.Core

Core acceptance testing framework

[![Publish NuGet Packages](https://github.com/AlmostCoherent/AcceptanceTesting.Core/actions/workflows/nuget-publish.yml/badge.svg)](https://github.com/AlmostCoherent/AcceptanceTesting.Core/actions/workflows/nuget-publish.yml)

## Overview

This repository contains the AcceptanceTesting.Core framework, a core library for acceptance testing in .NET applications.

## Installation

### From NuGet.org

```bash
dotnet add package AlmostCoherent.AcceptanceTesting.Core
```

### From GitHub Packages

First, add the GitHub Packages source (requires authentication with a GitHub Personal Access Token):

```bash
# Note: This stores credentials in plain text. For better security, use a credential provider.
dotnet nuget add source --username USERNAME --password GITHUB_PAT --store-password-in-clear-text --name github "https://nuget.pkg.github.com/AlmostCoherent/index.json"

# Alternative: Use environment variables to avoid storing in plain text
export NUGET_AUTH_TOKEN=your_github_pat
dotnet nuget add source --username USERNAME --password "%NUGET_AUTH_TOKEN%" --name github "https://nuget.pkg.github.com/AlmostCoherent/index.json"
```

> **Security Note**: Storing credentials in plain text is convenient but not recommended for production environments. Consider using [Azure Artifacts Credential Provider](https://github.com/microsoft/artifacts-credprovider) or other secure credential management solutions.

Then install the package:

```bash
dotnet add package AlmostCoherent.AcceptanceTesting.Core --source github
```

## Building from Source

### Prerequisites

- .NET 8.0 SDK or later

### Build Commands

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Create NuGet packages
dotnet pack --configuration Release --output ./nupkgs
```

## CI/CD Pipeline

This repository includes a GitHub Actions workflow that automatically:

- Builds the solution on every push and pull request
- Runs all tests
- Creates NuGet packages
- Publishes packages to NuGet.org and GitHub Packages (on pushes to main or version tags)

See [.github/workflows/README.md](.github/workflows/README.md) for more details on the CI/CD pipeline.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.