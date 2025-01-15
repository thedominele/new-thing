using System;
using System.Collections.Generic;
using System.Drawing;

namespace NosAssistant2.GameObjects;

public class SpawnTimedMob
{
	public int id { get; set; } = -1;


	public int server_id { get; set; } = -1;


	public string name { get; set; } = "";


	public Point? spawn_coordinates { get; set; } = default(Point);


	public List<Point> possible_spawn_coordinates { get; set; } = new List<Point>
	{
		new Point(0, 0)
	};


	public DateTime spawn_time { get; set; }

	public int respawn_duration { get; set; }

	public bool isMapBoss { get; set; }

	public Image? icon { get; set; }

	public SpawnTimedMob()
	{
	}

	public SpawnTimedMob(SpawnTimedMob other)
	{
		id = other.id;
		server_id = other.server_id;
		name = other.name;
		spawn_coordinates = other.spawn_coordinates;
		possible_spawn_coordinates = new List<Point>(other.possible_spawn_coordinates);
		spawn_time = other.spawn_time;
		respawn_duration = other.respawn_duration;
		isMapBoss = other.isMapBoss;
		icon = other.icon;
	}
}
