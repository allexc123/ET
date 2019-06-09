using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using ETModel;
using UnityEditor;

namespace ETEditor
{
	internal class OpcodeInfo
	{
		public string Name;
		public int Opcode;
	}

	public class Proto2CSEditor: EditorWindow
	{
		[MenuItem("Tools/Proto2CS")]
		public static void AllProto2CS()
		{
			//Process process = ProcessHelper.Run("dotnet", "Proto2CS.dll", "../Proto/", true);
			//Log.Info(process.StandardOutput.ReadToEnd());
			//AssetDatabase.Refresh();

            string protoc = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                protoc = "protoc.exe";
            }
            else
            {
                protoc = "protoc";
            }
            string rootPath = Path.GetFullPath("../Proto/");
            ProcessHelper.Run($"{rootPath}{protoc}", $"--csharp_out={rootPath}../Unity/Assets/Hotfix/Module/PB/ --proto_path= {rootPath} Login.proto","../Proto/", waitExit: true);
        }
	}
}
