using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NosAssistant2.GameObjects;

public class GameTarot
{
	private static int NoElement = 0;

	private static int Fire = 1;

	private static int Water = 2;

	private static int Light = 3;

	private static int Shadow = 4;

	private static int All = 5;

	public static readonly Dictionary<int, GameTarot> dict = new Dictionary<int, GameTarot>
	{
		{
			863,
			new GameTarot
			{
				ID = 863,
				Name = "Sun",
				Path = "images\\effects\\588.png",
				icon = (File.Exists("images\\effects\\588.png") ? Image.FromFile("images\\effects\\588.png") : null)
			}
		},
		{
			864,
			new GameTarot
			{
				ID = 864,
				Name = "Fool",
				Path = "images\\effects\\656.png",
				icon = (File.Exists("images\\effects\\656.png") ? Image.FromFile("images\\effects\\656.png") : null)
			}
		},
		{
			865,
			new GameTarot
			{
				ID = 865,
				Name = "Magician",
				Path = "images\\effects\\657.png",
				icon = (File.Exists("images\\effects\\657.png") ? Image.FromFile("images\\effects\\657.png") : null)
			}
		},
		{
			866,
			new GameTarot
			{
				ID = 866,
				Name = "Lovers",
				Path = "images\\effects\\658.png",
				icon = (File.Exists("images\\effects\\658.png") ? Image.FromFile("images\\effects\\658.png") : null)
			}
		},
		{
			867,
			new GameTarot
			{
				ID = 867,
				Name = "Hermit",
				Path = "images\\effects\\659.png",
				icon = (File.Exists("images\\effects\\659.png") ? Image.FromFile("images\\effects\\659.png") : null)
			}
		},
		{
			868,
			new GameTarot
			{
				ID = 868,
				Name = "Death",
				Path = "images\\effects\\660.png",
				icon = (File.Exists("images\\effects\\660.png") ? Image.FromFile("images\\effects\\660.png") : null)
			}
		},
		{
			869,
			new GameTarot
			{
				ID = 869,
				Name = "Devil",
				Path = "images\\effects\\661.png",
				icon = (File.Exists("images\\effects\\661.png") ? Image.FromFile("images\\effects\\661.png") : null)
			}
		},
		{
			870,
			new GameTarot
			{
				ID = 870,
				Name = "Tower",
				Path = "images\\effects\\662.png",
				icon = (File.Exists("images\\effects\\662.png") ? Image.FromFile("images\\effects\\662.png") : null)
			}
		},
		{
			871,
			new GameTarot
			{
				ID = 871,
				Name = "Star",
				Path = "images\\effects\\663.png",
				icon = (File.Exists("images\\effects\\663.png") ? Image.FromFile("images\\effects\\663.png") : null)
			}
		},
		{
			872,
			new GameTarot
			{
				ID = 872,
				Name = "Moon",
				Path = "images\\effects\\664.png",
				icon = (File.Exists("images\\effects\\664.png") ? Image.FromFile("images\\effects\\664.png") : null)
			}
		}
	};

	public int ID { get; set; } = -1;


	public string Name { get; set; } = "";


	public string Path { get; set; } = "";


	public Image? icon { get; set; }

	public static bool IsTarot(int effect_id)
	{
		GameTarot value;
		return dict.TryGetValue(effect_id, out value);
	}

	public static Image GetImage(int effect_id)
	{
		dict.TryGetValue(effect_id, out GameTarot value);
		if (value == null)
		{
			if (!File.Exists("images\\effects\\empty.png"))
			{
				return null;
			}
			return Image.FromFile("images\\effects\\empty.png");
		}
		return value.icon;
	}
}
