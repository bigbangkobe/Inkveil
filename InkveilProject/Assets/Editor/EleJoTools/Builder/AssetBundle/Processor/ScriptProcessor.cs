using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;

/// <summary>
/// 脚本处理器
/// </summary>
public sealed class ScriptProcessor : BaseProcessor
{
	private const string PATH_RAS = "key_ras";
	private const string EXT_SCRIPT = "*.lua";
	private const string GUI_TITLE = "Sign Script [{0}/{1}]";
	private const string GUI_INFO = "Signing...{0}";

	public override void OnPreProcess(Dictionary<string, AssetBundleConfig> configMap)
	{
		Signature();
		AssetDatabase.Refresh();

		base.OnPreProcess(configMap);
	}

	public override void OnPostProcess(string path, string buildPath)
	{
		base.OnPostProcess(path, buildPath);

		FileInfo[] fileInfos = new DirectoryInfo(setting.path).GetFiles(setting.searchPattern, setting.searchOption);
		for (int i = 0; i < fileInfos.Length; ++i)
		{
			FileInfo fileInfo = fileInfos[i];
			File.Delete(fileInfo.FullName);
		}
		AssetDatabase.Refresh();
	}

	/// <summary>
	/// 签名脚本
	/// </summary>
	private void Signature()
	{
		SHA1 sha = new SHA1CryptoServiceProvider();
		RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
		rsa.FromXmlString(File.ReadAllText(PATH_RAS));

		string ext = setting.searchPattern.Substring(1);
		FileInfo[] fileInfos = new DirectoryInfo(setting.path).GetFiles(EXT_SCRIPT, setting.searchOption);
		for (int i = 0; i < fileInfos.Length; ++i)
		{
			FileInfo fileInfo = fileInfos[i];

			string title = string.Format(GUI_TITLE, i + 1, fileInfos.Length);
			string info = string.Format(GUI_INFO, fileInfo.Name);
			EditorUtility.DisplayProgressBar(title, info, (float)(i + 1) / fileInfos.Length);

			byte[] filecontent = File.ReadAllBytes(fileInfo.FullName);
			byte[] sig = rsa.SignData(filecontent, sha);

			string path = fileInfo.FullName.Replace('\\', '/');
			path = path.Substring(path.LastIndexOf(setting.path) + setting.path.Length);
			path = setting.path + path;
			path = path.Remove(path.LastIndexOf('.')) + ext;

			fileInfo = new FileInfo(path);
			if (!fileInfo.Directory.Exists)
			{
				fileInfo.Directory.Create();
			}

			FileStream fs = new FileStream(path, FileMode.Create);
			fs.Write(sig, 0, sig.Length);
			fs.Write(filecontent, 0, filecontent.Length);
			fs.Close();
		}
		EditorUtility.ClearProgressBar();
	}
}