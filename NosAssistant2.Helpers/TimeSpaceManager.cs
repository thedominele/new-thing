using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using NosAssistant2.GameObjects;

namespace NosAssistant2.Helpers;

public static class TimeSpaceManager
{
	public static TimeSpaceData? current_ts = null;

	public static bool starting_ts = false;

	public static bool ts_started = false;

	public static bool first_time_packet = true;

	public static Point current_player_position = default(Point);

	public static void UpdatePlayersPostion(int x_pos, int y_pos)
	{
		if (current_ts != null)
		{
			current_player_position.X = x_pos;
			current_player_position.Y = y_pos;
			TimeSpaceRoom timeSpaceRoom = current_ts.rooms.Find((TimeSpaceRoom x) => x.x == current_player_position.X && x.y == current_player_position.Y);
			if (timeSpaceRoom != null && GUI.Mapper != null)
			{
				timeSpaceRoom.map_id = GUI.Mapper.real_map_id;
			}
		}
	}

	public static void AddRoom(int room_x, int room_y)
	{
		if (current_ts == null)
		{
			current_ts = new TimeSpaceData();
			starting_ts = true;
		}
		if (!current_ts.rooms.Any((TimeSpaceRoom x) => x.x == room_x && x.y == room_y))
		{
			current_ts.rooms.Add(new TimeSpaceRoom
			{
				x = room_x,
				y = room_y
			});
		}
	}

	public static void AddPortal(GameEntity portal, int state)
	{
		if (current_ts == null)
		{
			return;
		}
		TimeSpaceRoom timeSpaceRoom = current_ts.rooms.Find((TimeSpaceRoom x) => x.x == current_player_position.X && x.y == current_player_position.Y);
		if (timeSpaceRoom != null && NAStyles.BitmapOriginal != null && !timeSpaceRoom.portals.Any((TimeSpacePortal x) => x.x == portal.x && x.y == portal.y && x.state == state))
		{
			int num = NAStyles.map_width / 2;
			int num2 = NAStyles.map_height / 2;
			string text = "";
			if ((double)portal.x < (double)num * 0.25)
			{
				text = "W";
			}
			else if ((double)portal.x > (double)num * 0.75)
			{
				text = "E";
			}
			else if ((double)portal.y < (double)num2 * 0.25)
			{
				text = "N";
			}
			else if ((double)portal.y > (double)num2 * 0.75)
			{
				text = "S";
			}
			if (!(text == ""))
			{
				timeSpaceRoom.portals.Add(new TimeSpacePortal
				{
					x = portal.x,
					y = portal.y,
					state = state,
					orientation = text
				});
			}
		}
	}

	public static void AddLever(GameEntity lever)
	{
		if (current_ts != null)
		{
			TimeSpaceRoom timeSpaceRoom = current_ts.rooms.Find((TimeSpaceRoom x) => x.x == current_player_position.X && x.y == current_player_position.Y);
			if (timeSpaceRoom != null && !timeSpaceRoom.levers.Any((TimeSpaceLever x) => x.server_id == lever.server_id))
			{
				timeSpaceRoom.levers.Add(new TimeSpaceLever
				{
					ID = lever.id,
					server_id = (int)lever.server_id,
					x = lever.x,
					y = lever.y
				});
			}
		}
	}

	public static void AddMob(GameMonster mob)
	{
		if (current_ts != null)
		{
			TimeSpaceRoom timeSpaceRoom = current_ts.rooms.Find((TimeSpaceRoom x) => x.x == current_player_position.X && x.y == current_player_position.Y);
			if (timeSpaceRoom != null && !timeSpaceRoom.mobs.Any((TimeSpaceMonster x) => x.server_id == mob.server_id))
			{
				timeSpaceRoom.mobs.Add(new TimeSpaceMonster
				{
					ID = mob.id,
					server_id = mob.server_id,
					name = mob.name,
					x = mob.x,
					y = mob.y
				});
				timeSpaceRoom.contains_mobs = true;
			}
		}
	}

	public static void StartTimeSpace(int x_start, int y_start)
	{
		starting_ts = false;
		ts_started = true;
		first_time_packet = true;
		NAStyles.force_live_map_draw = false;
		MarkStartingRoom(x_start, y_start);
		GUI.UpdateSwitchMapModeButtonVisibility();
	}

	public static void FinishTimeSpace(bool save)
	{
		if (current_ts == null)
		{
			return;
		}
		if (save)
		{
			if (!Directory.Exists("TimeSpaceData"))
			{
				Directory.CreateDirectory("TimeSpaceData");
			}
			string contents = JsonConvert.SerializeObject(current_ts, Formatting.Indented);
			File.WriteAllText($"TimeSpaceData\\{current_ts.ID}.json", contents);
		}
		ts_started = false;
		starting_ts = false;
		current_ts = null;
		NAStyles.force_live_map_draw = false;
		GUI.UpdateSwitchMapModeButtonVisibility();
	}

	public static void MarkKillMobsRoom()
	{
		if (current_ts != null)
		{
			TimeSpaceRoom timeSpaceRoom = current_ts.rooms.Find((TimeSpaceRoom x) => x.x == current_player_position.X && x.y == current_player_position.Y);
			if (timeSpaceRoom != null)
			{
				timeSpaceRoom.kill_mobs = true;
			}
		}
	}

	public static void MarkTimeRoom()
	{
		if (current_ts != null)
		{
			TimeSpaceRoom timeSpaceRoom = current_ts.rooms.Find((TimeSpaceRoom x) => x.x == current_player_position.X && x.y == current_player_position.Y);
			if (timeSpaceRoom != null)
			{
				timeSpaceRoom.time_room = true;
			}
		}
	}

	public static void MarkBonusTimeRoom()
	{
		if (current_ts == null)
		{
			return;
		}
		TimeSpaceRoom timeSpaceRoom = current_ts.rooms.Find((TimeSpaceRoom x) => x.x == current_player_position.X && x.y == current_player_position.Y);
		if (timeSpaceRoom != null)
		{
			if (first_time_packet)
			{
				first_time_packet = false;
			}
			else
			{
				timeSpaceRoom.bonus_time = true;
			}
		}
	}

	public static void MarkStartingRoom(int room_x, int room_y)
	{
		if (current_ts != null)
		{
			TimeSpaceRoom timeSpaceRoom = current_ts.rooms.Find((TimeSpaceRoom x) => x.x == room_x && x.y == room_y);
			if (timeSpaceRoom != null)
			{
				timeSpaceRoom.is_starting_room = true;
				NAStyles.ts_map = (File.Exists($"images\\time_spaces\\{current_ts.ID}.png") ? ((Bitmap)Image.FromFile($"images\\time_spaces\\{current_ts.ID}.png")) : null);
			}
		}
	}

	public static void HighlightTimeSpace(int x, int y, PictureBox map)
	{
		if (ts_started || (NAvigator.show_time_space_map && map.Name.Contains("Quest")))
		{
			Point? roomCoordinatesFromMousePosition = NAStyles.GetRoomCoordinatesFromMousePosition(new Point(x, y), map.Width, map.Height, QuestManager.current_time_space_data);
			QuestManager.highlighted_time_space_room = ((!roomCoordinatesFromMousePosition.HasValue) ? new Point(-1, -1) : roomCoordinatesFromMousePosition.Value);
			if (roomCoordinatesFromMousePosition.HasValue)
			{
				GUI.ForceMapRefresh();
			}
		}
	}
}
