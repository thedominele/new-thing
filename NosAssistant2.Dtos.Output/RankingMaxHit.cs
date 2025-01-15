namespace NosAssistant2.Dtos.Output;

public class RankingMaxHit
{
	public int character_id { get; set; }

	public int max_hit { get; set; }

	public int max_hit_skill_id { get; set; }

	public int sp_id { get; set; }

	public int boss_id { get; set; }

	public string nickname { get; set; } = "";


	public string family { get; set; } = "";


	public int sex { get; set; }

	public int server_id { get; set; }
}
