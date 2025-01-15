using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NosAssistant2.GameObjects;

public class GameEntity
{
	public string type_name { get; set; } = "";


	public int type { get; set; } = 2;


	public int id { get; set; }

	public long server_id { get; set; }

	public int x { get; set; }

	public int y { get; set; }

	public int z { get; set; }

	public int quantity { get; set; }

	public int hp_percent { get; set; }

	public int max_hp_percent { get; set; }

	public int pet_owner_id { get; set; } = -1;


	public int sppID { get; set; } = -1;


	public int item_owner_id { get; set; } = -1;


	public bool is_required_lever { get; set; }

	public Image? icon { get; set; }

	public int portal_target_map_id { get; set; } = -1;


	public static bool IsLever(int lever_id)
	{
		return new List<int>
		{
			1135, 1136, 1137, 1138, 1000, 1045, 1051, 1052, 1048, 1049,
			1057, 1055, 1056
		}.Contains(lever_id);
	}

	public void UpdateIcon()
	{
		int num = -1;
		if (type_name == "NPC")
		{
			num = id;
			icon = (File.Exists($"images/npcs/{num}.png") ? Image.FromFile($"images/npcs/{num}.png") : (File.Exists("images/npcs/empty.png") ? Image.FromFile("images/npcs/empty.png") : null));
		}
		else if (type_name == "Pet")
		{
			num = id;
			icon = (File.Exists($"images/pets/{num}.png") ? Image.FromFile($"images/pets/{num}.png") : (File.Exists("images/npcs/empty.png") ? Image.FromFile("images/npcs/empty.png") : null));
		}
		else if (type_name == "Partner")
		{
			num = ((sppID == -1) ? id : sppID);
			icon = (File.Exists($"images/partners/{num}.png") ? Image.FromFile($"images/partners/{num}.png") : (File.Exists("images/npcs/empty.png") ? Image.FromFile("images/npcs/empty.png") : null));
		}
	}
}
