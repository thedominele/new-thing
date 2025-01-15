namespace NosAssistant2.Dtos.Input;

public class PlayerFullInfoDto
{
	public string unique_id { get; set; } = "";


	public int server_id { get; set; }

	public int character_id { get; set; }

	public string nickname { get; set; }

	public string family { get; set; }

	public int lvl { get; set; }

	public int clvl { get; set; }

	public int class_id { get; set; }

	public int sex { get; set; }

	public int? sp_id { get; set; }

	public int? sp_upgrade { get; set; }

	public int? sp_wings_id { get; set; }

	public string vanity { get; set; }

	public int weapon_type { get; set; }

	public int? weapon_upgrade { get; set; }

	public int? weapon_rarity { get; set; }

	public int? armor_upgrade { get; set; }

	public int? armor_rarity { get; set; }

	public int? family_id { get; set; }

	public int? family_role_id { get; set; }

	public int? family_lvl { get; set; }

	public int reputation { get; set; }

	public int? fairy_element_id { get; set; }

	public int? fairy_id { get; set; }

	public int title { get; set; }

	public int? real_title { get; set; }
}
