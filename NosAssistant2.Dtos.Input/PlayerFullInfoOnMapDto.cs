using System.Collections.Generic;

namespace NosAssistant2.Dtos.Input;

public class PlayerFullInfoOnMapDto
{
	public string unique_id { get; set; }

	public int map_id { get; set; }

	public int server_id { get; set; }

	public List<PlayerFullInfoDto> players { get; set; }
}
