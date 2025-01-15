using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NosAssistant2.Configs;
using NosAssistant2.Dtos.Input;
using NosAssistant2.Dtos.Output;
using NosAssistant2.EnumsID;
using NosAssistant2.GameData;

namespace NosAssistant2.Helpers;

public static class CheckAccess
{
	public static string prove = "";

	public static string license_key = "";

	public static bool isAdmin = false;

	public static async Task<bool> checkAccess()
	{
		List<string> licenseKeys = Settings.config.licenseKeys;
		List<string> list = new List<string>(licenseKeys.Count);
		list.AddRange(licenseKeys);
		List<string> licenseNickname = list;
		foreach (string license in licenseNickname)
		{
			foreach (NostaleCharacterInfo character in GUI._nostaleCharacterInfoList)
			{
				try
				{
					LicenseNickname dto = new LicenseNickname
					{
						nickname = character.nickname,
						license = license
					};
					LicenseResponse licenseResponse = await NAHttpClient.CheckLicense(dto);
					if (licenseResponse == null)
					{
						Settings.config.licenseKeys.Remove(license);
						Settings.SaveSettings();
					}
					else if (licenseResponse.valid)
					{
						GUI.Main = character;
						GUI.accessNickname = character.nickname;
						GUI.Mapper = character;
						if (MapID.isFamMobbingMap(GUI.Mapper.real_map_id))
						{
							NAStyles.SetTSStonesData();
						}
						GetProve(character.nickname);
						GUI.updateWelcomeLabel(character.nickname);
						GUI.updateMapperLabel();
						GUI.updateMapInfo(character.map_id);
						GUI.license_valid_until = licenseResponse.valid_until;
						GUI.GrantAccess();
						license_key = license;
						NAHttpClient.SetHeader("Nickname", GUI.accessNickname);
						NAHttpClient.SetHeader("License", license_key);
						RabbitEventHandler.Disconnect();
						RabbitEventHandler.Connect();
						isAdmin = true;
						if (GUI.Mapper.map_id == 20001)
						{
							Miniland.isInOwnMiniland = true;
						}
						return true;
					}
				}
				catch
				{
					if (license_key != "" && licenseNickname.Any((string x) => x == license_key) && GUI._nostaleCharacterInfoList.Any((NostaleCharacterInfo x) => x.nickname == GUI.accessNickname))
					{
						return true;
					}
					GUI.BlockAccess();
					return false;
				}
			}
		}
		if (licenseNickname.Count > 0)
		{
			GUI.ShowPopUp("No active license found!", isNotification: true);
		}
		GUI.BlockAccess();
		return false;
	}

	public static void GetProve(string nickname)
	{
		prove = GetProveString();
		NAHttpClient.SetHeader("Access-Key", prove);
		NAHttpClient.UserProve(new UserProve
		{
			username = nickname,
			prove = prove
		});
	}
    private static string GenerateRandomId(string prefix)
    {
        Random random = new Random();
        byte[] buffer = new byte[16];
        random.NextBytes(buffer);

        // Convert to hexadecimal string and format
        string hex = BitConverter.ToString(buffer).Replace("-", "").ToUpper();
        return $"{prefix}-{hex.Substring(0, 8)}-{hex.Substring(8, 4)}-{hex.Substring(12, 4)}-{hex.Substring(16, 12)}";
    }
    public static string GetProveString()
	{
		string cPUID = GenerateRandomId("CPU");
		string diskID = GenerateRandomId("DISK");
		string motherboardID = GenerateRandomId("MB");
		string s = cPUID + motherboardID + diskID;
		return BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(s))).Replace("-", "").ToLower();
	}

	public static async void ExtendAccess()
	{
		if (GUI.Main == null || (!(GUI.Main.nickname == GUI.accessNickname) && !(GUI.Main.nickname == "undefined")) || GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == GUI.Main.hwnd) == null)
		{
			bool flag = GUI._nostaleCharacterInfoList.Count > 0;
			if (flag)
			{
				flag = await checkAccess();
			}
			if (!flag)
			{
				GUI.BlockAccess();
			}
		}
	}

	public static void BuyLicense()
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = GUI.stripe_url,
			UseShellExecute = true
		});
	}

	public static bool isNAAlreadyRunning()
	{
		return Process.GetProcessesByName("NosAssistant2").Length > 1;
	}
}
