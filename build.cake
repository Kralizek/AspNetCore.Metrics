#tool "nuget:?package=OpenCover"

var target = Argument("Target", "Build");
var configuration = Argument("Configuration", "Release");
var artifactFolder = Argument<DirectoryPath>("Output", "artifacts");
var includeCoverage = HasArgument("IncludeCoverage");
var cleanBuildOutput = HasArgument("Clean");

var testFolder = artifactFolder.Combine("tests");

Task("Information")
    .Does(() =>
{
    Information($"Target: {target}");
    Information($"Configuration: {configuration}");
    Information($"Output: {artifactFolder}");
    Information($"Test: {testFolder}");
    Information($"IncludeCoverage: {includeCoverage}");
});

Task("Reset")
    .Does(() =>
{
    CleanDirectory(artifactFolder);
    CleanDirectory(testFolder);

    EnsureDirectoryExists(artifactFolder);
    EnsureDirectoryExists(testFolder);
});

Task("Clean")
    .WithCriteria(cleanBuildOutput)
    .Does(() =>
{
    DotNetCoreClean(".");    
});

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore();
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
        Configuration = configuration
    };

    DotNetCoreBuild(".", settings);
});

Task("Run-Tests-With-Coverage")
    .WithCriteria(includeCoverage)
    .Does(() =>
{
    var testProjectFiles = GetFiles("tests/Metrics.*/*.csproj");

    foreach (var file in testProjectFiles)
    {
        Information($"Testing {file.FullPath}");

        var resultFileName = MakeAbsolute(testFolder).CombineWithFilePath(file.GetFilenameWithoutExtension().AppendExtension(".trx"));

        var testSettings = new DotNetCoreTestSettings
        {
            Logger = $"trx;LogFileName={resultFileName.FullPath}"
        };

        var openCoverSettings = new OpenCoverSettings{ MergeOutput = true, Register = "user", ReturnTargetCodeOffset = 1000 }
            .WithFilter("+[*]Kralizek.AspNetCore.Metrics.*")
            .WithFilter("-[*]Tests.*")
            .WithFilter("-[TestBase]*");

        var coverageOutput = MakeAbsolute(artifactFolder).CombineWithFilePath("coverage.xml");

        OpenCover(tool => 
            {
                tool.DotNetCoreTest(file.FullPath, testSettings);
            },
            coverageOutput,
            openCoverSettings
        );
    }
});

Task("Run-Tests-Without-Coverage")
    .WithCriteria(!includeCoverage)
    .Does(() =>
{
    var testProjectFiles = GetFiles("tests/Metrics.*/*.csproj");

    foreach (var file in testProjectFiles)
    {
        Information($"Testing {file.FullPath}");

        var resultFileName = MakeAbsolute(testFolder).CombineWithFilePath(file.GetFilenameWithoutExtension().AppendExtension(".trx"));

        var settings = new DotNetCoreTestSettings
        {
            Logger = $"trx;LogFileName={resultFileName.FullPath}"
        };

        DotNetCoreTest(file.FullPath, settings);
    }
});

Task("Run-Tests")
    .IsDependentOn("Reset")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Run-Tests-Without-Coverage")
    .IsDependentOn("Run-Tests-With-Coverage")
    .Does(() =>
{

});

Task("Test-Local")
    .WithCriteria(BuildSystem.IsLocalBuild)
    .Does(() =>
{
    
});


Task("Test-AppVeyor")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .Does(() =>
{
    
});

Task("Test")
    .IsDependentOn("Run-Tests")
    .IsDependentOn("Test-Local")
    .IsDependentOn("Test-AppVeyor");

Task("Pack");

RunTarget(target);