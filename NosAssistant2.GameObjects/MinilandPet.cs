using System;
using System.Drawing;

namespace NosAssistant2.GameObjects;

public class MinilandPet
{
	public int pet_index { get; set; }

	public int pet_server_id { get; set; }

	public int lvl { get; set; }

	public int clvl { get; set; }

	public int current_xp { get; set; }

	public int max_xp { get; set; }

	public DateTime last_update { get; set; }

	public bool xp_changed { get; set; }

	public Image? icon { get; set; }
}
