using System.Collections.Generic;

namespace NosAssistant2.Dtos.Input;

public class RankingInfoDto
{
	public int boss_id { get; set; }

	public List<int> sp_ids { get; set; } = new List<int>();

}
