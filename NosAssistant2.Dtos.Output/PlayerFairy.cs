using System;

namespace NosAssistant2.Dtos.Output;

public class PlayerFairy
{
	public int? fairy_id { get; set; }

	public int? element { get; set; }

	public int? upgrade { get; set; }

	public int? percent { get; set; }

	public string? effects { get; set; }

	public DateTime updatedAt { get; set; }
}
