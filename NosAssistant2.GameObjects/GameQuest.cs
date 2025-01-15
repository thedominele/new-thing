using System.Collections.Generic;

namespace NosAssistant2.GameObjects;

public class GameQuest
{
	public int questPosition { get; set; }

	public int questID { get; set; }

	public int idk1 { get; set; }

	public int questType { get; set; }

	public int completed { get; set; }

	public List<(int actualCount, int maxCount)> QuestProgress { get; set; } = new List<(int, int)>();

}
