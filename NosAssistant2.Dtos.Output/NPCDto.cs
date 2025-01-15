using System.Collections.Generic;

namespace NosAssistant2.Dtos.Output;

public class NPCDto : EntityDto
{
	public int npcId { get; set; }

	public List<int> destinationMaps { get; set; } = new List<int>();


	public int x { get; set; }

	public int y { get; set; }
}
