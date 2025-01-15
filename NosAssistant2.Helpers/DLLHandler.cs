using System;
using System.Diagnostics;
using System.IO;
using NosAssistant2.Configs;

namespace NosAssistant2.Helpers;

public static class DLLHandler
{
	public static void InjectDLL(uint process_id)
	{
		string text = Directory.GetCurrentDirectory() + "\\tools\\DllInjector.exe";
		if (!File.Exists(text))
		{
			GUI.ShowPopUp("DLLInjector.exe was not found");
			return;
		}
		if (!File.Exists(Settings.config.dllPath))
		{
			GUI.ShowPopUp("Chosen File does not exist");
			return;
		}
		int value = new Random().Next(0, 1000000);
		string text2 = Settings.config.dllPath.Replace(".dll", "") + $"_{value}.dll";
		File.Copy(Settings.config.dllPath, text2);
		string arguments = $"{process_id} \"{text2}\"";
		Process process = new Process();
		process.StartInfo.FileName = text;
		process.StartInfo.UseShellExecute = true;
		process.StartInfo.Verb = "runas";
		process.StartInfo.Arguments = arguments;
		process.Start();
		string searchPattern = Settings.config.dllPath.Substring(Settings.config.dllPath.LastIndexOf("\\") + 1).Replace(".dll", "") + "_*.dll";
		string[] files = Directory.GetFiles(Settings.config.dllPath.Substring(0, Settings.config.dllPath.LastIndexOf("\\")) ?? "", searchPattern);
		foreach (string text3 in files)
		{
			try
			{
				if (text3 != text2)
				{
					File.Delete(text3);
				}
			}
			catch
			{
			}
		}
	}
}
