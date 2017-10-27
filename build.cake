#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=JetBrains.dotCover.CommandLineTools"

var target = Argument("Target", "Build");
var configuration = Argument("Configuration", "Release");
var outputPath = Argument<DirectoryPath>("Output", "output");
var overrideEnvironment = Argument<bool>("OverrideChecks", false);

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore();
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild(".", new DotNetCoreBuildSettings
    {
        Configuration = configuration
    });
});

Task("Test-WithCoverage")
    //.WithCriteria(() => BuildSystem.IsRunningOnAppVeyor || overrideEnvironment)
    .IsDependentOn("Restore")
    .Does(() =>
{
    var testProjectFiles = GetFiles("tests/Metrics.*/*.csproj");

    var testOutputFolder = outputPath.Combine("Tests");

    foreach (var file in testProjectFiles)
    {
        Information($"Processing {file.FullPath}");

        var outputFilePath = MakeAbsolute(testOutputFolder).CombineWithFilePath(file.GetFilenameWithoutExtension());

        DotCoverCover(tool => {
            tool.DotNetCoreTool(file, "xunit", new ProcessArgumentBuilder()
                    .AppendSwitchQuoted("-xml", outputFilePath.AppendExtension(".xml").ToString()));
        },
        MakeAbsolute(testOutputFolder).CombineWithFilePath(file.GetFilenameWithoutExtension()).AppendExtension(".dcvr"),
        new DotCoverCoverSettings{ TargetWorkingDir = file.GetDirectory() }
            .WithFilter("+:Kralizek")
            .WithFilter("-:Tests")
        );
    }
});

Task("Test-Local")
    .WithCriteria(() => BuildSystem.IsLocalBuild)
    .IsDependentOn("Restore")
    .Does(() =>
{
    var testProjectFiles = GetFiles("tests/Metrics.*/*.csproj");

    foreach (var file in testProjectFiles)
    {
        Information($"Processing {file.FullPath}");

        DotNetCoreTool(file, "xunit");
    }
});

Task("Test")
    .IsDependentOn("Test-Local")
    .IsDependentOn("Test-WithCoverage")
;

// Task("Test")
//     .IsDependentOn("Restore")
//     .Does(() =>
// {
//     var projectFiles = GetFiles("tests/Metrics.*/*.csproj");
//     foreach (var file in projectFiles)
//     {
//         DotNetCoreTest(file.FullPath);
//     }
// });

// Task("CleanOutputFolder")
//     .Does(() =>
// {
//     CleanDirectory(outputPath);
// });

// Task("Pack")
//     .IsDependentOn("Test")
//     .IsDependentOn("CleanOutputFolder")
//     .Does(() =>
// {
//     EnsureDirectoryExists(outputPath);

//     DotNetCorePack("Metrics.sln", new DotNetCorePackSettings
//     {
//         Configuration = configuration,
//         OutputDirectory = outputPath
//     });
// });

RunTarget(target);