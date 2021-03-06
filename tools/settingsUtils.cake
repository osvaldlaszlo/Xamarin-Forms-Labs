#addin "Cake.Json"

public class SettingsUtils
{
	public static Settings LoadSettings(ICakeContext context)
	{
		var settingsFile = context.Argument<string>("settingsFile", ".\\settings.json");
		
		context.Information("Loading Settings: {0}", settingsFile);

		if (!context.FileExists(settingsFile))
		{
			context.Error("Settings File Does Not Exist");
			return null;
		}
		
		var obj = context.DeserializeJsonFromFile<Settings>(settingsFile);

		obj.SettingsFile = settingsFile;
		
		// Allow for any overrides
		obj.Target = context.Argument<string>("target", obj.Target);
		obj.Configuration = context.Argument<string>("configuration", obj.Configuration);
		obj.VersionFile = context.Argument<string>("versionFile", obj.VersionFile);
				
		obj.ExecuteBuild 		= context.Argument<string>("build", obj.ExecuteBuild.ToString()).ToLower() == "true" 		|| context.Argument<string>("build", obj.ExecuteBuild.ToString()) == "1";
		obj.ExecutePackage 		= context.Argument<string>("package", obj.ExecutePackage.ToString()).ToLower() == "true" 	|| context.Argument<string>("package", obj.ExecutePackage.ToString()) == "1";
		obj.ExecuteUnitTest 	= context.Argument<string>("unitTest", obj.ExecuteUnitTest.ToString()).ToLower() == "true" 	|| context.Argument<string>("unitTest", obj.ExecuteUnitTest.ToString()) == "1";
		obj.ExecuteClean 		= context.Argument<string>("clean", obj.ExecuteClean.ToString()).ToLower() == "true" 		|| context.Argument<string>("clean", obj.ExecuteClean.ToString()) == "1";

		if (obj.NuGet == null) obj.NuGet = new NuGetSettings();
		
		obj.NuGet.FeedUrl		= context.Argument<string>("nugetFeed", obj.NuGet.FeedUrl);
		obj.NuGet.FeedApiKey	= context.Argument<string>("nugetApiKey", obj.NuGet.FeedApiKey);
		
		obj.NuGet.LibraryMinVersionDependency 		= (context.Argument<string>("dependencyVersion", obj.NuGet.LibraryMinVersionDependency)).Replace(":",".");
		obj.NuGet.VersionDependencyTypeForLibrary 	= context.Argument<VersionDependencyTypes>("dependencyType", obj.NuGet.VersionDependencyTypeForLibrary);
		
		return obj;
	}
	
	public static void DisplayHelp(ICakeContext context)
	{
		var defaultValues = new Settings();
		
		context.Information("Command Line Help/Syntax:");
		context.Information("\t.\\build.ps1 <Target>\t\t\t\t(Default: {0})", defaultValues.Target);
		context.Information("\t\t-Configuration=<Configuration>\t\t(Default: {0})", defaultValues.Configuration);
		context.Information("\t\t-settingsFile=<Settings File>\t\t(Default: {0})", defaultValues.SettingsFile);
		context.Information("\t\t-versionFile=<Version File>\t\t(Default: {0})", defaultValues.VersionFile);
		context.Information("\t\t-build=<0|1>\t\t\t\t(Default: {0})", defaultValues.ExecuteBuild);
		context.Information("\t\t-package=<0|1>\t\t\t\t(Default: {0})", defaultValues.ExecutePackage);
		context.Information("\t\t-unitTest=<0|1>\t\t\t\t(Default: {0})", defaultValues.ExecuteUnitTest);
		context.Information("\t\t-clean=<0|1>\t\t\t\t(Default: {0})", defaultValues.ExecuteClean);
		context.Information("\t\t-nugetFeed=<Nuget Feed URL>\t\t(Default: {0})", defaultValues.NuGet.FeedUrl);
		context.Information("\t\t-nugetApiKey=<Nuget Feed API Key>\t(Default: {0})", defaultValues.NuGet.FeedApiKey);
		context.Information("\t\t-dependencyVersion=<Version Number>\t(Default: {0})", defaultValues.NuGet.LibraryMinVersionDependency);
		context.Information("\t\t-dependencyType=<none|exact|greaterthan|greaterthanorequal|lessthan>\t\t(Default: {0})", defaultValues.NuGet.VersionDependencyTypeForLibrary);
		context.Information("");
		context.Information("Examples:");
		context.Information("\t.\\build Build -Configuration=Release");
		context.Information("\t.\\build UnitTest -build=0");
	}
}

public class Settings
{
	public Settings()
	{
		ExecuteBuild = true;
		ExecutePackage = true;
		ExecuteUnitTest = true;
		ExecuteClean = true;
		
		Target = "DisplayHelp";
		Configuration = "Release";
		SettingsFile = ".\\settings.json";
		VersionFile = ".\\version.json";
		
		Version = new VersionSettings();
		Build = new BuildSettings();
		Test = new TestSettings();
		NuGet = new NuGetSettings();
	}

	public string Target {get;set;}
	public string Configuration {get;set;}
	public string SettingsFile {get;set;}
	public string VersionFile {get;set;}
	
	public bool ExecuteBuild {get;set;}
	public bool ExecutePackage {get;set;}
	public bool ExecuteUnitTest {get;set;}
	public bool ExecuteClean {get;set;}
	
	public VersionSettings Version {get;set;}
	public BuildSettings Build {get;set;}
	public TestSettings Test {get;set;}
	public NuGetSettings NuGet {get;set;}
	
	public void Display(ICakeContext context)
	{
		context.Information("Settings:");

		context.Information("\tTarget: {0}", Target);
		context.Information("\tConfiguration: {0}", Configuration);
		context.Information("\tSettings File: {0}", SettingsFile);
		context.Information("\tVersion File: {0}", VersionFile);
		
		context.Information("\tExecute Build: {0}", ExecuteBuild);
		context.Information("\tExecute Package: {0}", ExecutePackage);
		context.Information("\tExecute UnitTests: {0}", ExecuteUnitTest);
		context.Information("\tExecute Clean: {0}", ExecuteClean);
		
		
		Version.Display(context);
		Build.Display(context);
		Test.Display(context);
		NuGet.Display(context);
	}
}

public class VersionSettings
{
	public VersionSettings()
	{
		LoadFrom = VersionSourceTypes.versionfile;
	}
	
	public string VersionFile {get;set;}
	public string AssemblyInfoFile {get;set;}
	public VersionSourceTypes LoadFrom {get;set;}
	public bool AutoIncrementVersion {get;set;}
	
	public void Display(ICakeContext context)
	{
		context.Information("Version Settings:");
		context.Information("\tVersion File: {0}", VersionFile);
		context.Information("\tAssemblyInfo File: {0}", AssemblyInfoFile);
		context.Information("\tLoad From: {0}", LoadFrom);
		context.Information("\tAutoIncrement Version: {0}", AutoIncrementVersion);
	}
}

public class BuildSettings
{
	public BuildSettings()
	{
		SourcePath = "./source";
		SolutionFileSpec = "*.sln";
		TreatWarningsAsErrors = false;
		EnableXamarinIOS = false;
		MaxCpuCount = 0;
	}
	
	public string SourcePath {get;set;}
	public string SolutionFileSpec {get;set;}
	public bool TreatWarningsAsErrors {get;set;}
	public int MaxCpuCount {get;set;}
	
	public bool EnableXamarinIOS {get;set;}
	public string MacAgentIPAddress {get;set;}
	public string MacAgentUserName {get;set;}
	public string MacAgentUserPassword {get;set;}
	
	public string SolutionFilePath {
		get {
			if (SolutionFileSpec.Contains("/")) return SolutionFileSpec;
			
			return string.Format("{0}{1}{2}", SourcePath, SolutionFileSpec.Contains("*") ? "/**/" : "", SolutionFileSpec);
		}
	}
	
	public void Display(ICakeContext context)
	{
		context.Information("Build Settings:");
		context.Information("\tSource Path: {0}", SourcePath);
		context.Information("\tSolution File Spec: {0}", SolutionFileSpec);
		context.Information("\tSolution File Path: {0}", SolutionFilePath);
		context.Information("\tTreat Warnings As Errors: {0}", TreatWarningsAsErrors);
		context.Information("\tMax Cpu Count: {0}", MaxCpuCount);
		
		context.Information("\tEnable Xamarin IOS: {0}", EnableXamarinIOS);
		context.Information("\tMac Agent IP Address: {0}", MacAgentIPAddress);
		context.Information("\tMac Agent User Name: {0}", MacAgentUserName);
		//context.Information("\tMac Agent User Password: {0}", MacAgentUserPassword);
	}
}

public class TestSettings
{
	public TestSettings()
	{
		SourcePath = "./tests";
		ResultsPath = "./tests";
		AssemblyFileSpec = "*.UnitTests.dll";
		Framework = TestFrameworkTypes.NUnit3;
	}
	
	public string SourcePath {get;set;}
	public string ResultsPath {get;set;}
	public string AssemblyFileSpec {get;set;}
	public TestFrameworkTypes Framework {get;set;}
			
	public void Display(ICakeContext context)
	{
		context.Information("Test Settings:");
		context.Information("\tSource Path: {0}", SourcePath);
		context.Information("\tResults Path: {0}", ResultsPath);
		context.Information("\tTest Assemploes File Spec: {0}", AssemblyFileSpec);
	}
}

public class NuGetSettings
{
	public NuGetSettings()
	{
		NuSpecPath = "./nuspec";
		NuGetConfig = "./.nuget/NuGet.Config";
		ArtifactsPath = "artifacts/packages";
		UpdateVersion = false;
		VersionDependencyTypeForLibrary = VersionDependencyTypes.none;
		UpdateLibraryDependencies = false;
		LibraryNamespaceBase = null;
		LibraryMinVersionDependency = null;
	}

	public string NuGetConfig {get;set;}
	public string FeedUrl {get;set;}
	public string FeedApiKey {get;set;}
	public string NuSpecPath {get;set;}
	public string ArtifactsPath {get;set;}
	public bool UpdateVersion {get;set;}
	public VersionDependencyTypes VersionDependencyTypeForLibrary {get;set;}
	public bool UpdateLibraryDependencies {get;set;}
	public string LibraryNamespaceBase {get;set;}
	public string LibraryMinVersionDependency {get;set;}
	
	public string NuSpecFileSpec {
		get {
			return string.Format("{0}/**/*.nuspec", NuSpecPath);
		}
	}

	public string NuGetPackagesSpec {
		get {
			return string.Format("{0}/*.nupkg", ArtifactsPath);
		}
	}
	
	public void Display(ICakeContext context)
	{
		context.Information("NuGet Settings:");
		context.Information("\tNuGet Config: {0}", NuGetConfig);
		context.Information("\tFeed Url: {0}", FeedUrl);
		//context.Information("\tFeed API Key: {0}", FeedApiKey);
		context.Information("\tNuSpec Path: {0}", NuSpecPath);
		context.Information("\tNuSpec File Spec: {0}", NuSpecFileSpec);
		context.Information("\tArtifacts Path: {0}", ArtifactsPath);
		context.Information("\tNuGet Packages Spec: {0}", NuGetPackagesSpec);
		context.Information("\tUpdate Version: {0}", UpdateVersion);
		context.Information("\tUpdate Library Dependencies: {0}", UpdateLibraryDependencies);
		context.Information("\tForce Version Match: {0}", VersionDependencyTypeForLibrary);
		context.Information("\tLibrary Namespace Base: {0}", LibraryNamespaceBase);
		context.Information("\tLibrary Min Version Dependency: {0}", LibraryMinVersionDependency);
	}
}

public enum VersionDependencyTypes {
	none,
	exact,
	greaterthan,
	greaterthanorequal,
	lessthan
}

public enum VersionSourceTypes {
	none,
	versionfile,
	assemblyinfo,
	git,
	tfs
}

public enum TestFrameworkTypes {
	none,
	NUnit2,
	NUnit3,
	XUnit,
	XUnit2
}