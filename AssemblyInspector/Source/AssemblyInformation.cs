using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace AssemblyInspector
{
	public class AssemblyInformation
	{
		private readonly string _path;

		public string AssemblyVersion { get; private set; }
		public string Version { get; private set; }
		public string PublicKey { get; private set; }
		public string Type { get; private set; }
		public string Code { get; private set; }
		public bool IsCil { get; private set; }
		public bool Is32Bit { get; private set; }
		public bool Is64Bit { get; private set; }
		public bool IsSigned { get; private set; }

		public AssemblyInformation(string path)
		{
			_path = path;

			GetAssemblyVersion();
			StartDumpbin();
			StartCorflags();
			StartSn();
		}

		private void GetAssemblyVersion()
		{
			var info = FileVersionInfo.GetVersionInfo(_path);
			AssemblyVersion = info.FileVersion;
		}

		private void StartDumpbin()
		{
			var startInfo = new ProcessStartInfo
			{
				FileName = "dumpbin.bat",
				Arguments = string.Format("/headers \"{0}\"", _path),
				UseShellExecute = false,
				CreateNoWindow = true
			};

			Process.Start(startInfo).WaitForExit();

			string result = File.ReadAllText("dumpbin.txt");
			ParseDumpbin(result);
		}

		private void StartCorflags()
		{
			const string file = "CorFlags.exe";
			string args = string.Format("\"{0}\"", _path);
			string result = Start(file, args);
			ParseCorflags(result);
		}

		private void StartSn()
		{
			const string file = "sn.exe";
			string args = string.Format("-q -T \"{0}\"", _path);
			string result = Start(file, args);
			ParseSn(result);
		}

		private string Start(string file, string args)
		{
			string result;

			var startInfo = new ProcessStartInfo
			{
				FileName = file,
				Arguments = args,
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using (var p = Process.Start(startInfo))
			{
				p.WaitForExit();
				result = p.StandardOutput.ReadToEnd();
				ParseSn(result);
			}

			return result;
		}

		private void ParseDumpbin(string results)
		{
			//File Type: EXECUTABLE IMAGE
			//FILE HEADER VALUES
			//             14C machine (x86)
			//            8664 machine (x64)
			//             1C0 machine (ARM)
			//OPTIONAL HEADER VALUES
			//             10B magic # (PE32)
			//             20B magic # (PE32+)
			Type = Regex.Match(results, @"File Type:\s*(.*)\r\n").Groups[1].Value;
			Code = Regex.Match(results, @"machine \((.*)\)\r\n").Groups[1].Value;
			Is32Bit = Regex.Match(results, @"machine \(x86\)\r\n").Success;
			Is64Bit = Regex.Match(results, @"machine \(x64\)\r\n").Success;
		}

		private void ParseCorflags(string results)
		{
			//Version   : v2.0.50727
			//CLR Header: 2.5
			//PE        : PE32
			//CorFlags  : 3
			//ILONLY    : 1
			//32BIT     : 1
			//Signed    : 0
			Version = Regex.Match(results, @"Version\s*:\s*(.*)\r\n").Groups[1].Value;

			if (!string.IsNullOrEmpty(Version))
			{
				IsCil = Regex.Match(results, @"ILONLY\s*:\s*1\r\n").Success;
				IsSigned = Regex.Match(results, @"Signed\s*:\s*1\r\n").Success;

				//Platform  PE     32BIT
				// x86       PE32   1
				// Any CPU   PE32   0
				// x64       PE32+  0
				Is32Bit = Regex.Match(results, @"PE\s*:\s*PE32\r\n").Success;
				Is64Bit = Regex.Match(results, @"32BIT\s*:\s*0\r\n").Success;
			}
		}

		private void ParseSn(string results)
		{
			//Public key token is 89b483f429c47342
			PublicKey = Regex.Match(results, @"Public key token is\s*(.*)\r\n").Groups[1].Value;
		}
	}
}