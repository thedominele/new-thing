namespace NosAssistant2.Dtos.Output;

public class PlayerSPDetails
{
	public string unique_id { get; set; }

	public int sp_id { get; set; }

	public int sp_upgrade { get; set; }

	public int? sp_wings { get; set; }

	public int? real_sp_wings { get; set; }

	public int job { get; set; }

	public int perfection { get; set; }

	public string build { get; set; }

	public string pp { get; set; }

	public string sl { get; set; }

	public int owner_id { get; set; }

	public int server_id { get; set; }

	public long server_sp_id { get; set; }

	public int card_class_id { get; set; }
}
