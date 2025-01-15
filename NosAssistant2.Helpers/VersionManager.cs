using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using NosAssistant2.Configs;
using NosAssistant2.Dtos.Output;
using NosAssistant2.GUIElements;

namespace NosAssistant2.Helpers;

public static class VersionManager
{
	private static HttpClient client = new HttpClient();

	private static string apiServerIP = "https://nosassistant.pl/api";

	private static string serverIP = "https://nosassistant.pl/na2";

	private static BackgroundWorker worker;

	public static bool CheckForUpdates()
	{
		bool? version = GetVersion(close: true);
		if (!version.HasValue)
		{
			return false;
		}
		if (!Directory.Exists("tools"))
		{
			Directory.CreateDirectory("tools");
		}
		//Download("tools/NAUpdater.exe");
		if (!File.Exists("tools/DllInjector.exe"))
		{
			Download("tools/DllInjector.exe");
		}
		if (version == false)
		{
			Settings.config.checkFiles = true;
			Settings.SaveSettings();
			//new NAMessageBox("Updating process will start.", "New Update", error: true).ShowDialog();
			try
			{
				//Process.Start("tools/NAUpdater.exe");
			}
			catch (Exception ex)
			{
				new NAMessageBox(ex.ToString(), "Error").ShowDialog();
			}
			//Application.Exit();
		}
		GUI.last_version_check = DateTime.UtcNow;
		return true;
	}

	public static bool? GetVersion(bool close)
	{
		try
		{
			HttpResponseMessage result = client.GetAsync(apiServerIP + "/version").Result;
			result.EnsureSuccessStatusCode();
			string result2 = result.Content.ReadAsStringAsync().Result;
			if (result2.StartsWith("maintenance"))
			{
				new NAMessageBox("Server is ongoing maintenance. Try again later", "Maintenance", error: true).ShowDialog();
				Application.Exit();
				return null;
			}
			return GUI.version == result2.Trim();
		}
		catch
		{
			if (close)
			{
				new NAMessageBox("Could not connect to the server", "Server offline", error: true).ShowDialog();
				Application.Exit();
			}
			return null;
		}
	}

	private static void Worker_DoWork(object sender, DoWorkEventArgs e)
	{
		DownloadMissingFilesInternal((BackgroundWorker)sender, e);
	}

	private static void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		Tuple<int, int> tuple = (Tuple<int, int>)e.UserState;
		GUI.UpdateLoadingBar(tuple.Item1, tuple.Item2);
	}

	private static void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		GUI.StartRoutine();
	}

	private static void DownloadMissingFilesInternal(BackgroundWorker worker, DoWorkEventArgs e)
	{
		GUI.setLoadingLabelText("Downloading Missing Files...");
		List<string> list = new List<string>();
		CollectionsMarshal.SetCount(list, 12);
		Span<string> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = "images/spells";
		num++;
		span[num] = "images/portraits";
		num++;
		span[num] = "images/maps";
		num++;
		span[num] = "images/effects";
		num++;
		span[num] = "images/npcs";
		num++;
		span[num] = "images/items";
		num++;
		span[num] = "images/sp_wings";
		num++;
		span[num] = "images/ranks";
		num++;
		span[num] = "images/pets";
		num++;
		span[num] = "images/partners";
		num++;
		span[num] = "images/time_spaces";
		num++;
		span[num] = "sounds";
		num++;
		List<string> list2 = new List<string>();
		foreach (string item in list)
		{
			if (!Directory.Exists(item))
			{
				Directory.CreateDirectory(item);
			}
			foreach (FileHashDto item2 in GetFilesListInDirectory(item))
			{
				string text = Path.Combine(item, item2.file);
				if (!File.Exists(text))
				{
					list2.Add(text);
				}
				else if (Settings.config.checkFiles && item != "sounds" && ComputeFileHash(text) != item2.hash)
				{
					list2.Add(text);
				}
			}
		}
		Settings.config.checkFiles = false;
		Settings.SaveSettings();
		int totalFiles = list2.Count;
		int downloadedFiles = 0;
		List<Task> list3 = new List<Task>();
		foreach (string missing_file in list2)
		{
			list3.Add(Task.Run(delegate
			{
				if (Download(missing_file))
				{
					Interlocked.Increment(ref downloadedFiles);
					worker.ReportProgress((int)((double)downloadedFiles / (double)totalFiles * 100.0), Tuple.Create(downloadedFiles, totalFiles));
				}
			}));
		}
		Task.WhenAll(list3).GetAwaiter().GetResult();
		GUI.setLoadingLabelText("Loading Characters...");
	}

	public static void DownloadMissingFiles()
	{
		worker = new BackgroundWorker
		{
			WorkerReportsProgress = true,
			WorkerSupportsCancellation = true
		};
		worker.DoWork += Worker_DoWork;
		worker.ProgressChanged += Worker_ProgressChanged;
		worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
		worker.RunWorkerAsync();
	}

	public static List<FileHashDto> GetFilesListInDirectory(string path)
	{
		try
		{
			StringContent content = new StringContent(JsonConvert.SerializeObject(new { path }), Encoding.UTF8, "application/json");
			HttpResponseMessage result = client.PostAsync(apiServerIP + "/files", content).Result;
			if (result.IsSuccessStatusCode)
			{
				return JsonConvert.DeserializeObject<List<FileHashDto>>(result.Content.ReadAsStringAsync().Result);
			}
			ShowErrorMessageAndExit("Could not connect to the server", "Server offline");
		}
		catch (Exception)
		{
			ShowErrorMessageAndExit("Could not connect to the server", "Server offline");
		}
		return new List<FileHashDto>();
	}

	private static void ShowErrorMessageAndExit(string message, string title)
	{
		new NAMessageBox(message, title, error: true).ShowDialog();
		Application.Exit();
	}

	public static bool Download(string file)
	{
		try
		{
			HttpResponseMessage result = client.GetAsync(serverIP + "/" + file).Result;
			result.EnsureSuccessStatusCode();
			Stream result2 = result.Content.ReadAsStreamAsync().Result;
			FileStream fileStream = File.Create(file);
			result2.Seek(0L, SeekOrigin.Begin);
			result2.CopyTo(fileStream);
			if (fileStream.Length == 0L)
			{
				fileStream.Close();
				File.Delete(file);
				return false;
			}
			fileStream.Close();
			return true;
		}
		catch
		{
			if (File.Exists(file))
			{
				File.Delete(file);
			}
			GUI.ShowPopUp("Failed while downloading files:" + file);
			return false;
		}
	}

	public static bool CheckIfNpcapIsInstalled()
	{
		using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Npcap");
		return registryKey != null;
	}

	private static string ComputeFileHash(string filePath)
	{
		using SHA256 sHA = SHA256.Create();
		using FileStream inputStream = File.OpenRead(filePath);
		return BitConverter.ToString(sHA.ComputeHash(inputStream)).Replace("-", "").ToLowerInvariant();
	}
}
