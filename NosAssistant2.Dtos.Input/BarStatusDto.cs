using System;

namespace NosAssistant2.Dtos.Input;

public class BarStatusDto
{
	public string unique_id { get; set; }

	public string type { get; set; }

	public string data { get; set; }

	public int server_id { get; set; }

	public int channel { get; set; }

	public DateTime? updatedAt { get; set; }
}
