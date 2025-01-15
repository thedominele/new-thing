namespace NosAssistant2.Dtos.Output;

public class RankingRaidsDone
{
	public int character_id { get; set; }

	public int total_raids { get; set; }

	public int sp_id { get; set; }

	public int boss_id { get; set; }

	public string nickname { get; set; } = "";


	public string family { get; set; } = "";


	public int sex { get; set; }

	public int server_id { get; set; }
}
