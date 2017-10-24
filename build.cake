var target = Argument("Target", "Build");
var configuration = Argument("Configuration", "Release");
var outputPath = Argument<DirectoryPath>("Output", "output");

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore();
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild("Metrics.sln", new DotNetCoreBuildSettings
    {
        Configuration = configuration
    });
});

Task("Test")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var projectFiles = GetFiles("tests/Metrics.*/*.csproj");
    foreach (var file in projectFiles)
    {
        DotNetCoreTest(file.FullPath);
    }
});

Task("CleanOutputFolder")
    .Does(() =>
{
    CleanDirectory(outputPath);
});

Task("Pack")
    .IsDependentOn("Test")
    .IsDependentOn("CleanOutputFolder")
    .Does(() =>
{
    EnsureDirectoryExists(outputPath);

    DotNetCorePack("Metrics.sln", new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = outputPath
    });
});

RunTarget(target);