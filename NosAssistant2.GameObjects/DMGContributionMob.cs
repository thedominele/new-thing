using System.Collections.Generic;
using System.Drawing;

namespace NosAssistant2.GameObjects;

public class DMGContributionMob
{
	public string name { get; set; } = "-";


	public int mob_id { get; set; } = -1;


	public int server_id { get; set; } = -1;


	public int mob_current_hp { get; set; } = -1;


	public int mob_max_hp { get; set; } = -1;


	public bool is_special { get; set; }

	public Image? icon { get; set; }

	public Dictionary<int, int> playersDMG { get; set; } = new Dictionary<int, int>();


	public DMGContributionMob()
	{
	}

	public DMGContributionMob(DMGContributionMob other)
	{
		name = other.name;
		mob_id = other.mob_id;
		server_id = other.server_id;
		mob_current_hp = other.mob_current_hp;
		mob_max_hp = other.mob_max_hp;
		is_special = other.is_special;
		icon = other.icon;
		playersDMG = new Dictionary<int, int>(other.playersDMG);
	}
}
