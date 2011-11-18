using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace AssemblyInspector
{
	public class Model
	{
		private string _path;
		private string _assemblyVersion;
		private string _version;
		private string _type;
		private string _code;
		private string _publicKey;
		private bool _cil;
		private bool _32bit;
		private bool _64bit;
		private bool _signed;

		public string AssemblyVersion { get { return _assemblyVersion; } }
		public string Version { get { return _version; } }
		public string Type { get { return _type; } }
		public string Code { get { return _code; } }
		public string PublicKey { get { return _publicKey; } }
		public bool IsCil { get { return _cil; } }
		public bool Is32Bit { get { return _32bit; } }
		public bool Is64Bit { get { return _64bit; } }
		public bool IsSigned { get { return _signed; } }

		public void PopulateData(string path)
		{
			_path = path;
			Reset();
			GetAssemblyVersion();
			StartDumpbin();
			StartCorflags();
			StartSn();
		}

		private void Reset()
		{
			_assemblyVersion = string.Empty;
			_version = string.Empty;
			_type = string.Empty;
			_code = string.Empty;
			_publicKey = string.Empty;
			_cil = false;
			_32bit = false;
			_64bit = false;
			_signed = false;
		}

		private void GetAssemblyVersion()
		{
			FileVersionInfo info = FileVersionInfo.GetVersionInfo(_path);
			_assemblyVersion = info.FileVersion;
		}

		private void StartDumpbin()
		{
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = "dumpbin.bat",
				Arguments = string.Format("/headers \"{0}\"", _path),
				UseShellExecute = false,
				CreateNoWindow = true
			};

			Process p = Process.Start(startInfo);
			p.WaitForExit();

			string result = File.ReadAllText("dumpbin.txt");
			ParseDumpbin(result);
		}

		private void StartCorflags()
		{
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = "CorFlags.exe",
				Arguments = string.Format("\"{0}\"", _path),
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			Process p = Process.Start(startInfo);
			p.WaitForExit();

			string result = p.StandardOutput.ReadToEnd();
			ParseCorflags(result);
		}

		private void StartSn()
		{
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = "sn.exe",
				Arguments = string.Format("-q -T \"{0}\"", _path),
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			Process p = Process.Start(startInfo);
			p.WaitForExit();

			string result = p.StandardOutput.ReadToEnd();
			ParseSn(result);
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
			_type = Regex.Match(results, @"File Type:\s*(.*)\r\n").Groups[1].Value;
			_code = Regex.Match(results, @"machine \((.*)\)\r\n").Groups[1].Value;
			_32bit = Regex.Match(results, @"machine \(x86\)\r\n").Success;
			_64bit = Regex.Match(results, @"machine \(x64\)\r\n").Success;
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
			_version = Regex.Match(results, @"Version\s*:\s*(.*)\r\n").Groups[1].Value;

			if (!string.IsNullOrEmpty(_version))
			{
				_cil = Regex.Match(results, @"ILONLY\s*:\s*1\r\n").Success;
				_signed = Regex.Match(results, @"Signed\s*:\s*1\r\n").Success;

				//Platform  PE     32BIT
				// x86       PE32   1
				// Any CPU   PE32   0
				// x64       PE32+  0
				_32bit = Regex.Match(results, @"PE\s*:\s*PE32\r\n").Success;
				_64bit = Regex.Match(results, @"32BIT\s*:\s*0\r\n").Success;
			}
		}

		private void ParseSn(string results)
		{
			//Public key token is 89b483f429c47342
			_publicKey = Regex.Match(results, @"Public key token is\s*(.*)\r\n").Groups[1].Value;
		}
	}
}