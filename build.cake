#tool "nuget:?package=OpenCover"
#tool "nuget:?package=JetBrains.dotCover.CommandLineTools"

DirectoryPath artifactFolder = "artifacts";
DirectoryPath tempFolder = "temp";

var target = Argument("Target", "Full");
var configuration = Argument("Configuration", "Release");

var cleanBuildOutput = HasArgument("Clean");
var useDotCover = HasArgument("DotCover");
var useOpenCover = HasArgument("OpenCover");

var includeCoverage = useDotCover || useOpenCover;

var testFolder = artifactFolder.Combine("tests");
var coverageFolder = artifactFolder.Combine("coverage");
var packageFolder = artifactFolder.Combine("packages");

Task("Information")
    .Does(() =>
{
    Information($"Target: {target}");
    Information($"Clean: {cleanBuildOutput}");
    Information($"Configuration: {configuration}");
    Information($"Output: {artifactFolder}");
    Information($"IncludeCoverage: {includeCoverage}");
    Information($"DotCover: {useDotCover}");
    Information($"OpenCover: {useOpenCover}");
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

Task("Run-Tests-OpenCover")
    .WithCriteria(includeCoverage)
    .WithCriteria(useOpenCover)
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

        var coverageOutput = MakeAbsolute(coverageFolder).CombineWithFilePath("openCover.xml");

        OpenCover(tool => 
            {
                tool.DotNetCoreTest(file.FullPath, testSettings);
            },
            coverageOutput,
            openCoverSettings
        );
    }
});

Task("Run-Tests-DotCover")
    .WithCriteria(includeCoverage)
    .WithCriteria(useDotCover)
    .Does(() =>
{
    var dotCoverFolder = tempFolder.Combine("dotCover");

    var testProjectFiles = GetFiles("tests/Metrics.*/*.csproj");

    foreach (var file in testProjectFiles)
    {
        Information($"Testing {file.FullPath}");

        var resultFileName = MakeAbsolute(testFolder).CombineWithFilePath(file.GetFilenameWithoutExtension().AppendExtension(".trx"));

        var testSettings = new DotNetCoreTestSettings
        {
            Logger = $"trx;LogFileName={resultFileName.FullPath}"
        };

        var dotCoverCoverSettings = new DotCoverCoverSettings { TargetWorkingDir = file.GetDirectory() }
            .WithFilter("+:Kralizek.*")
            .WithFilter("-:Metrics.*")
            .WithFilter("-:TestBase");

        var localCoverageOutput = MakeAbsolute(dotCoverFolder).CombineWithFilePath(file.GetFilenameWithoutExtension().AppendExtension(".dvcr"));

        DotCoverCover(tool => 
            {
                tool.DotNetCoreTest(file.FullPath, testSettings);
            },
            localCoverageOutput,
            dotCoverCoverSettings
        );
    }

    var dvcrFiles = GetFiles("**/*.dvcr");

    var mergedFile = MakeAbsolute(dotCoverFolder).CombineWithFilePath("merged.dvcr");

    Information($"Merging {dvcrFiles.Count()} into {mergedFile.FullPath}");
    DotCoverMerge(dvcrFiles, mergedFile);

    Information($"Generating reports");
    DotCoverReport(mergedFile, MakeAbsolute(coverageFolder).CombineWithFilePath("dotCover.detailed.xml"), 
        new DotCoverReportSettings{ ReportType = DotCoverReportType.DetailedXML });

    DotCoverReport(mergedFile, MakeAbsolute(coverageFolder).CombineWithFilePath("dotCover.xml"), 
        new DotCoverReportSettings{ ReportType = DotCoverReportType.XML });

    DotCoverReport(mergedFile, MakeAbsolute(coverageFolder).CombineWithFilePath("dotCover.html"), 
        new DotCoverReportSettings{ ReportType = DotCoverReportType.HTML });
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

Task("Test")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Run-Tests-Without-Coverage")
    .IsDependentOn("Run-Tests-DotCover")
    .IsDependentOn("Run-Tests-OpenCover");

Task("Pack")
    .IsDependentOn("Test")
    .Does(()=>
{
    var packSettings = new DotNetCorePackSettings 
    {
        Configuration = configuration,
        OutputDirectory = packageFolder
    };

    DotNetCorePack("Metrics.sln", packSettings);
});

Task("Upload-Coverage-Results-DotCover")
    .IsDependentOn("Run-Tests-DotCover")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .WithCriteria(useDotCover)
    .Does(() => 
{
    AppVeyor.UploadArtifact(MakeAbsolute(coverageFolder).CombineWithFilePath("dotCover.xml"));
    
    AppVeyor.UploadArtifact(MakeAbsolute(coverageFolder).CombineWithFilePath("dotCover.detailed.xml"));   
});

Task("Upload-Coverage-Results-OpenCover")
    .IsDependentOn("Run-Tests-OpenCover")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .WithCriteria(useOpenCover)
    .Does(() => 
{
    AppVeyor.UploadArtifact(MakeAbsolute(coverageFolder).CombineWithFilePath("dotCover.xml"));
    
    AppVeyor.UploadArtifact(MakeAbsolute(coverageFolder).CombineWithFilePath("dotCover.detailed.xml"));   
});

Task("Upload-Coverage-Results")
    .IsDependentOn("Upload-Coverage-Results-DotCover")
    .IsDependentOn("Upload-Coverage-Results-OpenCover")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor);

Task("Upload-Test-Results")
    .IsDependentOn("Test")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .Does(() => 
{
    var files = GetFiles("**/*.trx");

    foreach (var file in files)
    {
        AppVeyor.UploadTestResults(file, AppVeyorTestResultsType.MSTest);
    }
});

Task("Upload-Packages")
    .IsDependentOn("Pack")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .Does(() =>
{
    var files = GetFiles("artifacts/packages/*.nupkg");

    foreach (var file in files)
    {
        AppVeyor.UploadArtifact(file,
            new AppVeyorUploadArtifactsSettings 
            {
                ArtifactType = AppVeyorUploadArtifactType.NuGetPackage
            });
    }
});

Task("Upload-Artifacts")
    .IsDependentOn("Upload-Test-Results")
    .IsDependentOn("Upload-Coverage-Results")
    .IsDependentOn("Upload-Packages")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor);

Task("Full")
    .IsDependentOn("Upload-Artifacts");

Setup(context =>
{
    CleanDirectory(artifactFolder);
    CleanDirectory(tempFolder);

    EnsureDirectoryExists(artifactFolder);
    EnsureDirectoryExists(tempFolder);
});

Teardown(context => 
{
    DeleteDirectory(tempFolder, new DeleteDirectorySettings {
        Recursive = true,
        Force = true
    });
});

RunTarget(target);