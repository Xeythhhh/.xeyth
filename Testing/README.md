# Testing

Centralized test organization for the .xeyth framework.

## Structure

```
Testing/
  Automation/           # Tests for Automation slice projects
    Automation.Cli.Common.Tests/
    Automation.Framework.Tests/
    Automation.Planning.Tests/
    Automation.Verify.Tests/
  Contracts/            # Tests for Contracts slice projects
    Contracts.Core.Tests/
  Git/                  # Tests for Git slice projects
    GitTool.Tests/
  Shared/               # Shared test infrastructure
    TestUtilities/      # Reusable test utilities (builders, helpers)
    TestFixtures/       # xUnit fixtures for test setup/teardown
```

## Organization Principles

- **Slice-based**: Tests organized by framework slice (Automation, Contracts, Git)
- **Naming**: Test projects follow `{SourceProject}.Tests` pattern
- **Consistency**: All test projects use xUnit with standard packages
- **Shared Infrastructure**: Reusable test utilities in `Shared/`

## Test Categories

Tests are categorized using xUnit traits:

```csharp
[Trait("Category", "Unit")]        // Fast, isolated, no external dependencies
[Trait("Category", "Integration")] // Multiple components, may use filesystem/temp DBs
[Trait("Category", "Architecture")]// Architectural rules validation (NetArchTest)
[Trait("Category", "E2E")]         // End-to-end workflows, full system integration
```

## Running Tests

```bash
# All tests
dotnet test

# By category
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
dotnet test --filter "Category=Architecture"
dotnet test --filter "Category=E2E"

# Specific project
dotnet test Testing/Automation/Automation.Planning.Tests

# With code coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Conventions

See [TestOrganization.convention](TestOrganization.convention) for comprehensive guidelines on:

- Directory structure requirements
- Test project naming patterns
- Test file and class naming
- Test method naming (MethodName_Scenario_ExpectedBehavior)
- Project file structure
- Test categorization strategy
- Shared infrastructure organization

## Test Frameworks

- **xUnit**: Primary test framework (v2.9.3+)
- **Verify**: Snapshot testing for complex outputs
- **NetArchTest**: Architecture validation
- **Spectre.Console.Testing**: CLI output testing
- **Playwright**: Browser/E2E testing (when needed)

## Adding New Tests

1. Create test project in appropriate slice directory:
   ```bash
   cd Testing/{Slice}/
   dotnet new xunit -n {ProjectName}.Tests
   ```

2. Add project reference to source project:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\..\..\src\{ProjectName}\{ProjectName}.csproj" />
   </ItemGroup>
   ```

3. Add to solution:
   ```bash
   dotnet sln add Testing/{Slice}/{ProjectName}.Tests
   ```

4. Follow naming conventions from [TestOrganization.convention](TestOrganization.convention)

## Migration History

- **2025-12-27**: Reorganized from scattered locations
  - Moved `Git/Tests/` → `Testing/Git/`
  - Moved `src/tests/` → `Testing/Automation/` and `Testing/Contracts/`
  - Established consistent structure and conventions
  - Created TestOrganization.convention

## See Also

- [TestOrganization.convention](TestOrganization.convention) - Detailed naming and structure rules
- [SnapshotTestImprovements.task](SnapshotTestImprovements.task) - Snapshot testing enhancements
- [IntegrationTestFramework.task](IntegrationTestFramework.task) - Integration test infrastructure
