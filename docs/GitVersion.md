# GitVersion Setup and Usage

This project uses GitVersion for automated semantic versioning based on Git history and branch conventions.

## Configuration

The GitVersion configuration is defined in `GitVersion.yml` at the repository root.

### Branching Strategy

- **main/master**: Production releases (e.g., `1.0.0`)
- **develop**: Alpha builds with minor version increments (e.g., `1.1.0-alpha.1`)
- **feature/***: Feature branch builds with branch name in version (e.g., `1.0.0-my-feature.1`)
- **release/***: Beta releases (e.g., `1.1.0-beta.1`)
- **hotfix/***: Hotfix releases (e.g., `1.0.1-beta.1`)
- **pull-request**: PR builds (e.g., `1.0.0-PullRequest.123`)

## Version Increments

- **main**: Patch increment (1.0.0 → 1.0.1)
- **develop**: Minor increment (1.0.0 → 1.1.0)
- **release**: No increment (uses version from branch name)
- **feature**: Inherits from parent branch
- **hotfix**: Patch increment

## Usage

### Creating Releases

1. **Regular Release**:
   ```bash
   # Create a release branch
   git checkout -b release/1.1.0
   
   # When ready to release, merge to main and tag
   git checkout main
   git merge release/1.1.0
   git tag v1.1.0
   git push origin main --tags
   ```

2. **Hotfix Release**:
   ```bash
   # Create hotfix from main
   git checkout main
   git checkout -b hotfix/critical-fix
   
   # When ready, merge back to main and tag
   git checkout main
   git merge hotfix/critical-fix
   git tag v1.0.1
   git push origin main --tags
   ```

3. **Feature Development**:
   ```bash
   # Feature branches get automatic versioning
   git checkout -b feature/new-functionality
   # Each commit will create versions like 1.0.0-new-functionality.1
   ```

### Manual Version Override

If you need to set a specific version, you can create a tag:

```bash
# Set version to 2.0.0
git tag v2.0.0
git push origin v2.0.0
```

### CI/CD Integration

The GitHub Actions workflow automatically:

1. Calculates version using GitVersion
2. Applies version to all packable projects
3. Creates NuGet packages with correct version numbers
4. Publishes to GitHub Packages

### Local Development

To see what version GitVersion would calculate locally:

```bash
# Install GitVersion tool
dotnet tool install --global GitVersion.Tool

# Check current version
dotnet-gitversion

# Check version for specific configuration
dotnet-gitversion /config GitVersion.yml
```

### Version Outputs

GitVersion provides several version formats:

- **NuGetVersion**: Used for NuGet package versions (e.g., `1.0.0`, `1.1.0-alpha.1`)
- **AssemblySemVer**: Used for assembly version (e.g., `1.0.0.0`)
- **InformationalVersion**: Full version with metadata (e.g., `1.0.0+Branch.main.Sha.abc123`)
- **SemVer**: Semantic version (e.g., `1.0.0-alpha.1`)

## Troubleshooting

### No version tags found

If GitVersion can't find any version tags, it starts from 0.1.0. To set an initial version:

```bash
git tag v1.0.0
git push origin v1.0.0
```

### Unexpected version numbers

Check your Git history and ensure:
- Proper merge commits exist
- Branch names follow the expected patterns
- No duplicate tags exist

Use `dotnet-gitversion /showvariable NuGetVersion` to debug version calculation.