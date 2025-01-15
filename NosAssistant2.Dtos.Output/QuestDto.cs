using System;
using System.Collections.Generic;

namespace NosAssistant2.Dtos.Output;

public class QuestDto
{
	public int vnum { get; set; }

	public int questType { get; set; }

	public List<int> vnumArray { get; set; }

	public List<int> lvl { get; set; }

	public string title { get; set; }

	public string desc { get; set; }

	public List<int> talk { get; set; }

	public List<int> target { get; set; }

	public List<List<int>> data { get; set; }

	public int link { get; set; }

	public DateTime createdAt { get; set; }

	public DateTime updatedAt { get; set; }

	public List<int> prizes { get; set; }
}
