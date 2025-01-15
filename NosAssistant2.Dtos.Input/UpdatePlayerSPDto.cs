using NosAssistant2.Dtos.Output;

namespace NosAssistant2.Dtos.Input;

public class UpdatePlayerSPDto
{
	public string unique_id { get; set; } = "";


	public int server_id { get; set; }

	public int character_id { get; set; }

	public PlayerSPDto sp_details { get; set; }
}
