using System.Collections.Generic;

namespace NosAssistant2.Dtos.Output;

public class GameMapDto
{
	public int id { get; set; }

	public int mapId { get; set; }

	public List<int> adjacents { get; set; } = new List<int>();


	public List<GameMapMonsterDto> monsters { get; set; } = new List<GameMapMonsterDto>();


	public List<GameMapNPCsDto> npcs { get; set; }

	public List<GameMapTimeSpacesDto> timeSpaces { get; set; } = new List<GameMapTimeSpacesDto>();

}
