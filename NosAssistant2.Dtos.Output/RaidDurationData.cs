using System;

namespace NosAssistant2.Dtos.Output;

public class RaidDurationData
{
	public int boss_id { get; set; }

	public int raid_id { get; set; }

	public DateTime createdAt { get; set; }

	public double finished_in { get; set; }
}
