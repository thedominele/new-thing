namespace NosAssistant2.Dtos.Output;

public class PlayerFairyDto
{
	public string unique_id { get; set; }

	public int server_id { get; set; }

	public int owner_id { get; set; }

	public int fairy_id { get; set; }

	public int element { get; set; }

	public int? upgrade { get; set; }

	public string? effects { get; set; }

	public int? percent { get; set; }
}
