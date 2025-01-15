namespace NosAssistant2.Dtos.Input;

public class TempRaidInfoDto
{
	public int server_id { get; set; }

	public int boss_id { get; set; }

	public long server_boss_id { get; set; }

	public int character_id { get; set; }

	public int packets_corrupted_count { get; set; }
}
