using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using NosAssistant2.GameObjects;

namespace NosAssistant2.GameData;

public class NostaleCharacterInfo
{
	public string? local_address { get; set; }

	public string? remote_address { get; set; }

	public uint? local_port { get; set; }

	public uint? remote_port { get; set; }

	public int? channel { get; set; } = -1;


	public string server { get; set; } = "";


	public uint process_id { get; set; }

	public DateTime start_time { get; set; }

	public string nickname { get; set; } = "undefined";


	public nint hwnd { get; set; }

	public int character_id { get; set; }

	public int lvl { get; set; } = -1;


	public int clvl { get; set; } = -1;


	public string family_name { get; set; } = "";


	public string sex { get; set; } = "male";


	public int class_id { get; set; } = -1;


	public string status { get; set; } = "offline";


	public int encryption_key { get; set; }

	public CharConfig config { get; set; } = new CharConfig();


	public int map_id { get; set; } = -1;


	public int real_map_id { get; set; } = -1;


	public int x_pos { get; set; }

	public int y_pos { get; set; }

	public int current_hp { get; set; }

	public int current_mana { get; set; }

	public int max_hp { get; set; }

	public int max_mana { get; set; }

	public bool inRaid { get; set; }

	public SPCard SPCard { get; set; } = new SPCard();


	public Image? icon { get; set; }

	public DateTime lastFullUsed { get; set; }

	public DateTime lastItemUsed { get; set; }

	public DateTime lastRaidPotionUsed { get; set; }

	public Color? special_color { get; set; }

	[JsonIgnore]
	public List<HotBarElement> hotBar { get; set; } = new List<HotBarElement>();


	public void UpdateIcon()
	{
		if (SPCard.ID < 1800 && SPCard.ID != 1564)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 3);
			defaultInterpolatedStringHandler.AppendLiteral("images\\portraits\\");
			defaultInterpolatedStringHandler.AppendFormatted(SPCard.ID);
			defaultInterpolatedStringHandler.AppendLiteral("_");
			defaultInterpolatedStringHandler.AppendFormatted(sex);
			SPCard sPCard = SPCard;
			defaultInterpolatedStringHandler.AppendFormatted((sPCard != null && !sPCard.Shared) ? ("_" + class_id) : "");
			defaultInterpolatedStringHandler.AppendLiteral(".png");
			string text = defaultInterpolatedStringHandler.ToStringAndClear();
			icon = (File.Exists(text) ? Image.FromFile(text) : (File.Exists("images/npcs/empty.png") ? Image.FromFile("images/npcs/empty.png") : null));
		}
		else if (SPCard.ID < 1000000)
		{
			string text2 = $"images\\portraits\\{SPCard.ID}.png";
			icon = (File.Exists(text2) ? Image.FromFile(text2) : (File.Exists("images/npcs/empty.png") ? Image.FromFile("images/npcs/empty.png") : null));
		}
		else
		{
			string text3 = $"images\\npcs\\{SPCard.ID - 1000000}.png";
			icon = (File.Exists(text3) ? Image.FromFile(text3) : (File.Exists("images/npcs/empty.png") ? Image.FromFile("images/npcs/empty.png") : null));
		}
	}
}
