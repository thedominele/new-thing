using System.Collections.Generic;

namespace NosAssistant2.GameObjects;

public class TimeSpaceRoom
{
	public int map_id { get; set; }

	public int x { get; set; }

	public int y { get; set; }

	public List<TimeSpacePortal> portals { get; set; } = new List<TimeSpacePortal>();


	public List<TimeSpaceLever> levers { get; set; } = new List<TimeSpaceLever>();


	public List<TimeSpaceMonster> mobs { get; set; } = new List<TimeSpaceMonster>();


	public bool bonus_time { get; set; }

	public bool time_room { get; set; }

	public bool kill_mobs { get; set; }

	public bool contains_mobs { get; set; }

	public bool is_starting_room { get; set; }

	public bool is_waiting_room { get; set; }
}
