using System.Collections.Generic;

namespace NosAssistant2.Dtos.Output;

public class GameMapNPCsDto
{
	public int npcId { get; set; }

	public List<int> destinationMaps { get; set; } = new List<int>();

}
