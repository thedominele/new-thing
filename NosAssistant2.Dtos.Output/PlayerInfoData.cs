using System;
using System.Collections.Generic;

namespace NosAssistant2.Dtos.Output;

public class PlayerInfoData
{
	public string nickname { get; set; }

	public string family { get; set; }

	public int lvl { get; set; }

	public int clvl { get; set; }

	public int class_id { get; set; }

	public int sex { get; set; }

	public int? title { get; set; }

	public string vanity { get; set; }

	public int? family_lvl { get; set; }

	public int? family_role_id { get; set; }

	public int? reputation { get; set; }

	public DateTime updatedAt { get; set; }

	public List<PlayerSP> sp_cards { get; set; }

	public List<PlayerItem> items { get; set; }

	public List<PlayerFairy> fairies { get; set; }

	public List<PlayerTattoo> tattoos { get; set; }
}
