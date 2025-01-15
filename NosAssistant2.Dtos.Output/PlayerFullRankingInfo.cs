namespace NosAssistant2.Dtos.Output;

public class PlayerFullRankingInfo
{
	public int player_avg_damage { get; set; }

	public int sp_id { get; set; }

	public int boss_id { get; set; }

	public int sex { get; set; }

	public int server_id { get; set; }

	public string nickname { get; set; } = "";


	public string family { get; set; } = "-";

}
