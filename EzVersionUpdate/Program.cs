using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;
namespace EzVersionUpdate.Cli
{

	class Program
    {
		
		public class Options
        {
			[Option('t', "test", Required = false, HelpText = "Run this as test with no file change.", Default= false)]
			public bool test { get; set; }

			[Option('v', "version", Required = false, HelpText = "Will cause the application to mark all project files with this version number")]
            public string versionNumberForce { get; set; }

            [Option('p', "path", Required = false, HelpText = "Path to file to affect or a directory to recursively search for project files to affect")]
            public string path { get; set; }

            [Option('i', "increment", Required = false, HelpText = "increment type [0.1.2.3]- no value is 2 or 'REVISION' 0.0.X.0, it will bump that version by 1", Default = 2)]
            public int increment { get; set; }

			[Option('f', "filemasks", Required = false, HelpText = "Comma seperate file masks to search for, default will be *.csproj", Default ="*.csproj")]
			public string fileMasks { get; set; }

		}

		static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
					   int MAJOR = 0; int MINOR = 1; int REVISION = 2; int BUILD = 3; //Version Segments
					   Trace.Listeners.Add(new ConsoleTraceListener(true));
					   System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
					   System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
					   string version = fvi.FileVersion;
					   var path = Environment.CurrentDirectory;
					   if (!Path.EndsInDirectorySeparator(path)) path += Path.DirectorySeparatorChar;
					   if (!string.IsNullOrEmpty(o.path)) path = o.path;


					   Trace.WriteLine($"EzVersionUpdate Utility - Version: {version} - https://github.com/rvegajr/EzVersionUpdate");
					   Trace.WriteLine($"Searching for project files...");
					   var masks = o.fileMasks.Split(",");
					   var list = FindFiles(path, masks);
					   foreach( var file in list)
                       {
						   Trace.WriteLine($"Project File {file}");
						   string filever = GetVersionInProjectFile(file);
						   if (!string.IsNullOrEmpty(filever))
						   {
							   var Version = VersionStringIncrement(filever, o.increment);
							   if (!string.IsNullOrEmpty(o.versionNumberForce)) Version = o.versionNumberForce;
							   Match match1 = Regex.Match(Version, @"\d{1,3}\.\d{1,3}\.\d{1,3}");
							   Match match2 = Regex.Match(Version, @"\d{1,3}\.\d{1,3}\.\d{1,5}\.\d{1,5}");
							   if (!((match1.Success) || (match2.Success))) throw new Exception("Nuget version number should be ##9.##9.####9.####9 - thus 1.1.1, 1.1.1.0, or 999.999.9999 are valid");
							   Trace.WriteLine($"   File Version in file is {filever}.. will change to {Version}" + (o.test ? ".. remove -t to make it happen" : ""));
							   if (!o.test)
							   {
								   var VersionAttribute = VersionStringParts(Version, MAJOR, MINOR, REVISION);
								   var AssemblyVersionAttribute = VersionStringParts(Version, MAJOR, MINOR, REVISION, BUILD);
								   UpdateVersionInProjectFile(file, VersionAttribute);
								   UpdateVersionSettingInProjectFile(file, AssemblyVersionAttribute, "AssemblyVersion");
								   UpdateVersionSettingInProjectFile(file, AssemblyVersionAttribute, "FileVersion");
							   }
						   }
                           else
                           {
							   Trace.WriteLine($"   No File version in file.. skipping");
						   }
					   }
					   Trace.WriteLine($"Finished!");
				   });
        }

		public static List<string> FindFiles(string path, string[] masks)
		{
			SearchOption sopt = SearchOption.AllDirectories;
			List<String> listFiles = new List<string>();
			List<DirectoryInfo> dirs2scan = new List<DirectoryInfo>();
			var bNoHidden = true;
			dirs2scan.Add(new DirectoryInfo(path));

			for (; dirs2scan.Count != 0;)
			{
				int scanIndex = dirs2scan.Count - 1;        // Try to preserve somehow alphabetic order which GetFiles returns 
															// by scanning though last directory.
				foreach(var mask in masks)
                {
					FileInfo[] filesInfo = dirs2scan[scanIndex].GetFiles(mask, SearchOption.TopDirectoryOnly);

					foreach (FileInfo fi in filesInfo)
					{
						if (bNoHidden && fi.Attributes.HasFlag(FileAttributes.Hidden)) continue;
						listFiles.Add(fi.FullName);
					}
				}

				if (sopt != SearchOption.AllDirectories)
					break;

				foreach (DirectoryInfo dir in dirs2scan[scanIndex].GetDirectories("*", SearchOption.TopDirectoryOnly))
				{
					if (bNoHidden && dir.Attributes.HasFlag(FileAttributes.Hidden))
						continue;

					dirs2scan.Add(dir);
				}
				dirs2scan.RemoveAt(scanIndex);
			}
			return listFiles;
		}

		//versionSegments can equal Major, Minor, Revision, Build in format Major.Minor.Revision.Build
		public static string VersionStringParts(string versionString, params int[] versionSegments)
		{
			var vArr = versionString.Split('.');
			if (vArr.Length == 3)
			{
				var lstArr = vArr.ToList();
				lstArr.Add("0");
				vArr = lstArr.ToArray();
			}
			string newVersion = "";
			foreach (var versionSegment in versionSegments)
			{
				newVersion += (newVersion.Length > 0 ? "." : "") + vArr[versionSegment].ToString();
			}
			return newVersion;
		}

		//segmentToIncrement can equal Major, Minor, Revision, Build in format Major.Minor.Revision.Build
		public static string VersionStringIncrement(string versionString, int segmentToIncrement)
		{
			var vArr = versionString.Split('.');
			var valAsStr = vArr[segmentToIncrement];
			int valAsInt = 0;
			int.TryParse(valAsStr, out valAsInt);
			vArr[segmentToIncrement] = (valAsInt + 1).ToString();
			return String.Join(".", vArr);
		}


		public static string Pluck(string str, string leftString, string rightString)
		{
			try
			{
				var lpos = str.LastIndexOf(leftString);
				var rpos = str.IndexOf(rightString, lpos + 1);
				if (rpos > 0)
				{
					lpos = str.LastIndexOf(leftString, rpos);
					if ((lpos > 0) && (rpos > lpos))
					{
						return str.Substring(lpos + leftString.Length, (rpos - lpos) - leftString.Length);
					}
				}
			}
			catch (Exception)
			{
				return "";
			}
			return "";
		}


		public static string GetVersionInProjectFile(string projectFileName)
		{
			var _VersionInfoText = System.IO.File.ReadAllText(projectFileName);
			var _AssemblyFileVersionAttribute = Pluck(_VersionInfoText, "<Version>", "</Version>");
			return _AssemblyFileVersionAttribute;
		}

		public static bool UpdateVersionInProjectFile(string projectFileName, string NewVersion)
		{
			var _VersionInfoText = System.IO.File.ReadAllText(projectFileName);
			var _AssemblyFileVersionAttribute = Pluck(_VersionInfoText, "<Version>", "</Version>");
			var VersionPattern = "<Version>{0}</Version>";
			var _AssemblyFileVersionAttributeTextOld = string.Format(VersionPattern, _AssemblyFileVersionAttribute);
			var _AssemblyFileVersionAttributeTextNew = string.Format(VersionPattern, NewVersion);
			var newText = _VersionInfoText.Replace(_AssemblyFileVersionAttributeTextOld, _AssemblyFileVersionAttributeTextNew);

			System.IO.File.WriteAllText(projectFileName, newText);
			return true;
		}

		public static string GetVersionSettingInProjectFile(string projectFileName, string Name)
		{
			var _VersionInfoText = System.IO.File.ReadAllText(projectFileName);
			var _AssemblyFileVersionAttribute = Pluck(_VersionInfoText, "<" + Name + ">", "</" + Name + ">");
			return _AssemblyFileVersionAttribute;
		}

		public static bool UpdateVersionSettingInProjectFile(string projectFileName, string NewVersion, string Name)
		{
			var _VersionInfoText = System.IO.File.ReadAllText(projectFileName);
			var _AssemblyFileVersionAttribute = Pluck(_VersionInfoText, "<" + Name + ">", "</" + Name + ">");
			//Trace.WriteLine("_AssemblyFileVersionAttribute : {0}", _AssemblyFileVersionAttribute);
			var VersionPattern = "<" + Name + ">{0}</" + Name + ">";
			//Trace.WriteLine("VersionPattern : {0}", VersionPattern);
			var _AssemblyFileVersionAttributeTextOld = string.Format(VersionPattern, _AssemblyFileVersionAttribute);
			//Trace.WriteLine("_AssemblyFileVersionAttributeTextOld : {0}", _AssemblyFileVersionAttributeTextOld);
			var _AssemblyFileVersionAttributeTextNew = string.Format(VersionPattern, NewVersion);
			//Trace.WriteLine("_AssemblyFileVersionAttributeTextNew : {0}", _AssemblyFileVersionAttributeTextNew);
			var newText = _VersionInfoText.Replace(_AssemblyFileVersionAttributeTextOld, _AssemblyFileVersionAttributeTextNew);
			//Trace.WriteLine("newText : {0}", newText);

			System.IO.File.WriteAllText(projectFileName, newText);
			return true;
		}

	}
}
