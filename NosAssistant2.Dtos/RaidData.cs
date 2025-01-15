using System;
using System.Collections.Generic;

namespace NosAssistant2.Dtos;

public class RaidData
{
	public string version { get; set; } = "";


	public int server_id { get; set; }

	public int channel_id { get; set; }

	public string marathon_id { get; set; } = "";


	public int sent_by_character_id { get; set; }

	public string sent_by_character_nickname { get; set; } = "";


	public long server_boss_id { get; set; }

	public int boss_id { get; set; }

	public double finished_in { get; set; } = -1.0;


	public DateTime? boss_killed_at { get; set; } = default(DateTime);


	public List<RaidPlayer> players { get; set; } = new List<RaidPlayer>();

}
