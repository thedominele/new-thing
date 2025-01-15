using System;

namespace NosAssistant2.Dtos.Output;

public class PlayerItem
{
	public int? type { get; set; }

	public int? item_id { get; set; }

	public int? item_upgrade { get; set; }

	public int? item_rarity { get; set; }

	public string shell { get; set; }

	public string rune { get; set; }

	public DateTime? shell_updated_at { get; set; }

	public DateTime updatedAt { get; set; }
}
