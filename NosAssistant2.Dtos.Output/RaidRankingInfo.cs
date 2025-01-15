namespace NosAssistant2.Dtos.Output;

public class RaidRankingInfo
{
	public int sp_id { get; set; }

	public int boss_id { get; set; }

	public int server_id { get; set; }

	public int mean_damage { get; set; }

	public int stddev_damage { get; set; }

	public int player_count { get; set; }
}
