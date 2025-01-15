using System.Collections.Generic;
using System.Drawing;

namespace NosAssistant2.GameObjects;

public class TimeSpaceData
{
	public int ID { get; set; } = -1;


	public int min_lvl { get; set; }

	public int max_lvl { get; set; }

	public bool has_arrow { get; set; } = true;


	public List<Point> rooms_order { get; set; } = new List<Point>();


	public string directions { get; set; } = "";


	public List<TimeSpaceRoom> rooms { get; set; } = new List<TimeSpaceRoom>();

}
