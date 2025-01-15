using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NosAssistant2.GameData;

namespace NosAssistant2.Configs;

public static class Settings
{
	private static int sounds_count = 19;

	public static SettingsJson config { get; set; } = new SettingsJson();


	public static void LoadSettings()
	{
		if (!File.Exists(Directory.GetCurrentDirectory() + "/settings.json"))
		{
			File.Create("settings.json").Close();
			SaveSettings();
		}
		string value = File.ReadAllText("settings.json");
		try
		{
			SettingsJson settingsJson = JsonConvert.DeserializeObject<SettingsJson>(value);
			if (settingsJson != null)
			{
				config.licenseKeys = settingsJson.licenseKeys;
				config.dllPath = settingsJson.dllPath;
				config.raidHostName = settingsJson.raidHostName;
				config.autoFullThreshold = settingsJson.autoFullThreshold;
				config.autoconfirm = settingsJson.autoconfirm;
				config.autoJoinList = settingsJson.autoJoinList;
				config.renameClients = settingsJson.renameClients;
				config.enableHotkeys = settingsJson.enableHotkeys;
				config.playSounds = settingsJson.playSounds;
				config.showTooltips = settingsJson.showTooltips;
				config.low_spec = settingsJson.low_spec;
				config.buffsets = settingsJson.buffsets;
				if (config.buffsets.Count != 3)
				{
					config.buffsets = new List<Buffset>
					{
						new Buffset(),
						new Buffset(),
						new Buffset()
					};
				}
				config.defaultNetwordDeviceID = settingsJson.defaultNetwordDeviceID;
				config.inviteList = settingsJson.inviteList;
				config.window_size = settingsJson.window_size;
				config.charsConfigs = settingsJson.charsConfigs;
				config.randomizeCordsRange = settingsJson.randomizeCordsRange;
				config.PacketLoggerSettings = settingsJson.PacketLoggerSettings;
				config.DelaySettings = settingsJson.DelaySettings;
				config.RaidModeSettings = settingsJson.RaidModeSettings;
				config.MapSettings = settingsJson.MapSettings;
				config.ControlsSettings = settingsJson.ControlsSettings;
				config.CounterSettings = settingsJson.CounterSettings;
				config.WaypointsConfig = settingsJson.WaypointsConfig;
				config.ColumnsOrder = settingsJson.ColumnsOrder;
				config.checkFiles = settingsJson.checkFiles;
				config.sounds = settingsJson.sounds;
				if (config.sounds.Count >= sounds_count)
				{
					config.sounds = config.sounds.GetRange(config.sounds.Count - sounds_count, sounds_count);
				}
				else
				{
					config.sounds = config.sounds.GetRange(0, config.sounds.Count);
				}
				FixSounds();
				config.raidsNotifications = settingsJson.raidsNotifications;
			}
		}
		catch
		{
		}
	}

	public static void LoadCharsConfig()
	{
		CharConfig charConfig = null;
		foreach (NostaleCharacterInfo character in GUI._nostaleCharacterInfoList)
		{
			if (character.nickname == "undefined")
			{
				continue;
			}
			CharConfig charConfig2 = config.charsConfigs.Find((CharConfig x) => x.nickname == character.nickname && x.character_id == character.character_id);
			if (charConfig2 != null)
			{
				if (charConfig2.isGoldPicker)
				{
					if (charConfig != null)
					{
						charConfig2.isGoldPicker = false;
						charConfig = charConfig2;
					}
					charConfig = charConfig2;
				}
				character.config = charConfig2;
			}
			else
			{
				CharConfig item = new CharConfig
				{
					nickname = character.nickname,
					character_id = character.character_id
				};
				character.config = item;
				config.charsConfigs.Add(item);
			}
		}
	}

	public static void LoadCharConfig(NostaleCharacterInfo character)
	{
		if (!(character.nickname == "undefined"))
		{
			CharConfig charConfig = config.charsConfigs.Find((CharConfig x) => x.nickname == character.nickname && x.character_id == character.character_id);
			if (charConfig != null)
			{
				character.config = charConfig;
				return;
			}
			CharConfig item = new CharConfig
			{
				nickname = character.nickname,
				character_id = character.character_id
			};
			character.config = item;
			config.charsConfigs.Add(item);
		}
	}

	public static void SaveSettings()
	{
		File.WriteAllText("settings.json", JsonConvert.SerializeObject(config, Formatting.Indented));
	}

	public static void LoadDefaults()
	{
		config = new SettingsJson
		{
			licenseKeys = config.licenseKeys,
			dllPath = config.dllPath,
			raidHostName = config.raidHostName,
			autoFullThreshold = config.autoFullThreshold,
			autoconfirm = config.autoconfirm,
			autoJoinList = config.autoJoinList,
			inviteList = config.inviteList,
			charsConfigs = config.charsConfigs,
			randomizeCordsRange = config.randomizeCordsRange,
			PacketLoggerSettings = config.PacketLoggerSettings,
			RaidModeSettings = config.RaidModeSettings,
			MapSettings = config.MapSettings
		};
		SaveSettings();
	}

	public static void FixSounds()
	{
		foreach (SoundSettings sound in new SettingsJson().sounds)
		{
			SoundSettings soundSettings = config.sounds.Find((SoundSettings x) => x.sound_name == sound.sound_name);
			if (soundSettings == null)
			{
				config.sounds.Add(sound);
			}
			else if (soundSettings.path == null)
			{
				soundSettings = sound;
			}
		}
	}
}
