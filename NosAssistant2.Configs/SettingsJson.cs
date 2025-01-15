using System.Collections.Generic;
using System.Linq;
using NosAssistant2.GameData;
using NosAssistant2.GameObjects;

namespace NosAssistant2.Configs;

public class SettingsJson
{
	public List<int> raidsNotifications = new List<int>();

	public List<string> licenseKeys { get; set; } = new List<string>();


	public string dllPath { get; set; } = "";


	public string raidHostName { get; set; } = "";


	public bool autoconfirm { get; set; }

	public bool autoJoinList { get; set; }

	public int autoFullThreshold { get; set; } = 50;


	public int randomizeCordsRange { get; set; } = 3;


	public bool renameClients { get; set; } = true;


	public bool enableHotkeys { get; set; } = true;


	public bool playSounds { get; set; } = true;


	public bool showTooltips { get; set; } = true;


	public int volume { get; set; } = 25;


	public int window_size { get; set; }

	public bool low_spec { get; set; }

	public string defaultNetwordDeviceID { get; set; } = "";


	public List<Buffset> buffsets { get; set; } = new List<Buffset>();


	public List<InviteItem> inviteList { get; set; } = new List<InviteItem>();


	public PacketLoggerSettings PacketLoggerSettings { get; set; } = new PacketLoggerSettings();


	public RaidModeSettings RaidModeSettings { get; set; } = new RaidModeSettings();


	public DelaySettings DelaySettings { get; set; } = new DelaySettings();


	public MapSettings MapSettings { get; set; } = new MapSettings();


	public ControlsSettings ControlsSettings { get; set; } = new ControlsSettings();


	public CounterSettings CounterSettings { get; set; } = new CounterSettings();


	public WaypointsConfig WaypointsConfig { get; set; } = new WaypointsConfig();


	public List<string> ColumnsOrder { get; set; } = new List<string>();


	public List<CharConfig> charsConfigs { get; set; } = new List<CharConfig>();


	public List<SoundSettings> sounds { get; set; } = new List<SoundSettings>();


	public bool checkFiles { get; set; } = true;


	public SettingsJson()
	{
		List<string> source = new List<string>
		{
			"Alzanor Wind", "Belial Reflect", "Carno Grab", "Coiling Vines", "Cooking", "Erenia Cry", "Error Sound", "Fernon Reflect", "Impostor", "FFD Spawn",
			"Asgobas Spawn (lol)", "Arma Bomb", "Normal Fish", "Notification Sound", "Paimon Miniboss", "Polluted Water Prison", "Rare Fish", "Run", "Raid Notification", "Trainer Finished",
			"Ultimate Arma Color"
		};
		List<string> source2 = new List<string>
		{
			"sounds/alzanor_wind_warning.wav", "sounds/belial_reflect_warning.wav", "sounds/carno_grab_warning.wav", "sounds/coiling_vines_warning.wav", "sounds/cooking.wav", "sounds/erenia_crying_warning.wav", "sounds/fernon_reflect_warning.wav", "sounds/impostor.wav", "sounds/ffd_spawn_warning.wav", "sounds/asgobas_spawn_warning.wav",
			"sounds/error.wav", "sounds/arma_bomb_warning.wav", "sounds/normal_fish.wav", "sounds/notification.wav", "sounds/paimon_miniboss_warning.wav", "sounds/polluted_water_prison_warning.wav", "sounds/rare_fish.wav", "sounds/run.wav", "sounds/raid_notification.wav", "sounds/trainer_finished.wav",
			"sounds/ultimate_arma_red.wav"
		};
		source = source.Order().ToList();
		source2 = source2.Order().ToList();
		for (int i = 0; i < source.Count; i++)
		{
			sounds.Add(new SoundSettings
			{
				sound_name = source.ElementAt(i),
				path = source2.ElementAt(i),
				volume = 25,
				state = true
			});
		}
	}
}
