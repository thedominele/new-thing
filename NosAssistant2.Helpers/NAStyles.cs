using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NosAssistant2.Configs;
using NosAssistant2.Dtos.Output;
using NosAssistant2.EnumsID;
using NosAssistant2.GameData;
using NosAssistant2.GameObjects;

namespace NosAssistant2.Helpers;

public static class NAStyles
{
	public static readonly Color MainThemeDarker = Color.FromArgb(20, 9, 67);

	public static readonly Color MainThemeLighter = Color.FromArgb(32, 15, 85);

	public static readonly Color MenuButtonPressedColor = Color.FromArgb(26, 12, 90);

	public static readonly Color ButtonTrueColor = Color.FromArgb(72, 149, 239);

	public static readonly Color ButtonFalseColor = Color.FromArgb(56, 49, 180);

	public static readonly Color HoveredRaidItemColor = Color.FromArgb(50, 43, 165);

	public static readonly Color ShowMoreColor = Color.FromArgb(72, 149, 239);

	public static readonly Color NAButtonHoverColor = Color.FromArgb(181, 23, 158);

	public static readonly Color NotSelectedTabColor = Color.FromArgb(20, 9, 90);

	public static readonly Color ActiveCharColor = Color.FromArgb(63, 55, 201);

	public static readonly Color NotActiveCharColor = Color.FromArgb(181, 23, 158);

	public static readonly Color SelectedPanelColor = Color.FromArgb(100, 97, 200);

	public static readonly Color CounterForeColor = Color.FromArgb(233, 138, 232);

	public static readonly Color RankingFamMemberColor = Color.BlueViolet;

	public static readonly Color RankingFamMemberHoverColor = Color.FromArgb(165, 93, 232);

	public static readonly Color RankingSelfHoverColor = Color.FromArgb(196, 49, 175);

	public static Color MapperColor = NotActiveCharColor;

	public static readonly Color AltColor = CounterForeColor;

	public static readonly Color PetsColor = Color.FromArgb(157, 149, 45);

	public static readonly Color PlayersColor = Color.SandyBrown;

	public static readonly Color EntitiesColor = Color.SeaGreen;

	public static readonly Color MonstersColor = Color.BlueViolet;

	public static readonly Color SemiExclusiveMonstersColor = Color.Yellow;

	public static readonly Color SpecialMonstersColor = Color.Red;

	public static readonly Color BossMonsterColor = Color.DarkRed;

	public static readonly Color LeversPressedColor = Color.ForestGreen;

	public static readonly Color LeversNotPressedColor = Color.Yellow;

	public static readonly Color NotRequiredLeverColor = Color.SlateGray;

	public static readonly Color PortalColor = Color.DeepPink;

	public static readonly Color IceFlowerColor = Color.Green;

	public static readonly Color QuestArrowColor = Color.White;

	public static readonly Color TimeSpaceColorDefault = Color.Black;

	public static readonly Color TimeSpaceColorCompleted = Color.Green;

	public static readonly Color TimeSpaceColorCurrentQuest = Color.Yellow;

	public static readonly Color TimeSpaceColorNotCompleted = Color.Gray;

	public static readonly Color TimeSpaceColorHeroic = Color.Orange;

	public static Image? TimeSpaceIcon = null;

	public static List<GameMonster> mob_filters = new List<GameMonster>();

	public static Dictionary<int, List<GameMonster>> map_to_filters = new Dictionary<int, List<GameMonster>>();

	public static Bitmap? ts_map = null;

	public static int map_width = -1;

	public static int map_height = -1;

	public static bool force_live_map_draw = false;

	public static Bitmap? BitmapLarge { get; set; } = null;


	public static Bitmap? BitmapOriginal { get; set; } = null;


	public static DateTime? MapBossDespawnTime { get; set; } = null;


	public static DateTime LastWarningSound { get; set; } = DateTime.UtcNow;


	public static DateTime? FirstBossSpawn { get; set; } = null;


	public static int CurrentTSStonesCount { get; set; } = 0;


	public static async void SetTSStonesData()
	{
		if (GUI.Mapper != null)
		{
			await Task.Delay(100);
			int num = ((GUI.Mapper.real_map_id == 150) ? 140 : 60);
			CurrentTSStonesCount = Crypto.ReadTSStoneCountFromMemory((int)GUI.Mapper.process_id);
			int minute = DateTime.Now.Minute;
			int second = DateTime.Now.Second;
			if (CurrentTSStonesCount < num)
			{
				FirstBossSpawn = DateTime.UtcNow.AddMinutes((60 - minute) % 4).AddSeconds(-second);
			}
			else
			{
				FirstBossSpawn = DateTime.UtcNow.AddMinutes(60 - minute).AddSeconds(-second);
			}
		}
	}

	public static Color ColorFromHPRatio(double hpPercetnage)
	{
		if (hpPercetnage < 0.15)
		{
			return Color.Red;
		}
		if (hpPercetnage < 0.3)
		{
			return Color.Orange;
		}
		if (hpPercetnage < 0.5)
		{
			return Color.Yellow;
		}
		if (hpPercetnage < 0.75)
		{
			return Color.GreenYellow;
		}
		return Color.Green;
	}

	public static Bitmap DrawSpawnTimersOnMap(Bitmap bitmap, double ratioX, double ratioY)
	{
		if (GUI.Mapper == null)
		{
			return bitmap;
		}
		using (Graphics graphics = Graphics.FromImage(bitmap))
		{
			List<SpawnTimedMob> spawn_timed_mobs = GUI.spawn_timed_mobs;
			List<SpawnTimedMob> list = new List<SpawnTimedMob>();
			CollectionsMarshal.SetCount(list, spawn_timed_mobs.Count);
			Span<SpawnTimedMob> span = CollectionsMarshal.AsSpan(list);
			int num = 0;
			Span<SpawnTimedMob> span2 = CollectionsMarshal.AsSpan(spawn_timed_mobs);
			span2.CopyTo(span.Slice(num, span2.Length));
			num += span2.Length;
			List<SpawnTimedMob> list2 = list;
			foreach (SpawnTimedMob item in list2)
			{
				if (!item.spawn_coordinates.HasValue)
				{
					continue;
				}
				int num2 = (int)Math.Round((double)item.spawn_coordinates.Value.X * ratioX);
				int num3 = (int)Math.Round((double)item.spawn_coordinates.Value.Y * ratioY);
				string s = (item.spawn_time - DateTime.UtcNow).ToString("mm\\:ss");
				Font font = new Font("Microsoft Sans Serif", 11.25f, FontStyle.Bold, GraphicsUnit.Point, 238);
				using SolidBrush solidBrush = new SolidBrush(NotActiveCharColor);
				if (item.isMapBoss)
				{
					font = new Font("Microsoft Sans Serif", 25f, FontStyle.Bold, GraphicsUnit.Point, 238);
					solidBrush.Color = NotActiveCharColor;
					string text = ((GUI.Mapper.real_map_id == 150) ? "FFD" : "Asgobas");
					TimeSpan timeSpan = item.spawn_time - DateTime.UtcNow;
					s = text + ": " + timeSpan.ToString("mm\\:ss");
					graphics.DrawString(s, font, solidBrush, num2, num3);
					if (Math.Round(timeSpan.TotalSeconds) == 15.0 && (DateTime.UtcNow - LastWarningSound).TotalSeconds > 5.0)
					{
						if (text == "FFD")
						{
							Controller.PlaySound("FFD Spawn");
						}
						else
						{
							Controller.PlaySound("Asgobas Spawn (lol)");
						}
						LastWarningSound = DateTime.UtcNow;
					}
				}
				else
				{
					graphics.DrawString(s, font, solidBrush, num2, num3);
					if (!Settings.config.low_spec && item.icon != null)
					{
						int num4 = 24;
						int num5 = 6;
						int num6 = num2 - num4 + 4;
						int num7 = num3 + (num4 - num5) / 2;
						Brush gray = Brushes.Gray;
						graphics.FillEllipse(gray, new Rectangle(num6 - (num4 + num5) / 2, num7 - (num4 + num5) / 2, num4 + num5, num4 + num5));
						graphics.DrawImage(item.icon, new Rectangle(num6 - num4 / 2, num7 - num4 / 2, num4, num4));
					}
				}
			}
			if (!list2.Exists((SpawnTimedMob x) => x.isMapBoss))
			{
				using SolidBrush brush = new SolidBrush(NotActiveCharColor);
				Font font2 = new Font("Microsoft Sans Serif", 25f, FontStyle.Bold, GraphicsUnit.Point, 238);
				string text2 = ((GUI.Mapper.real_map_id == 150) ? "FFD" : "Asgobas");
				TimeSpan? timeSpan2 = MapBossDespawnTime - DateTime.UtcNow;
				string s2;
				if (timeSpan2.HasValue)
				{
					s2 = text2 + ": Alive : " + timeSpan2.Value.ToString("mm\\:ss");
				}
				else
				{
					timeSpan2 = FirstBossSpawn - DateTime.UtcNow;
					if (timeSpan2.HasValue && FirstBossSpawn > DateTime.UtcNow)
					{
						s2 = text2 + ":" + timeSpan2.Value.ToString("mm\\:ss");
					}
					else
					{
						if (!timeSpan2.HasValue || !(FirstBossSpawn <= DateTime.UtcNow))
						{
							return bitmap;
						}
						s2 = text2 + ":Soon";
					}
					if (timeSpan2.HasValue && Math.Round(timeSpan2.Value.TotalSeconds) == 15.0 && (DateTime.UtcNow - LastWarningSound).TotalSeconds > 5.0)
					{
						if (text2 == "FFD")
						{
							Controller.PlaySound("FFD Spawn");
						}
						else
						{
							Controller.PlaySound("Asgobas Spawn (lol)");
						}
						LastWarningSound = DateTime.UtcNow;
					}
				}
				graphics.DrawString(s2, font2, brush, 0f, 0f);
			}
		}
		return bitmap;
	}

	public static Bitmap DrawEntitiesOnBitmap(Bitmap bitmap, double ratioX, double ratioY)
	{
		using Graphics graphics = Graphics.FromImage(bitmap);
		List<GameEntity> entities = GUI.entities;
		List<GameEntity> list = new List<GameEntity>();
		CollectionsMarshal.SetCount(list, entities.Count);
		Span<GameEntity> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		Span<GameEntity> span2 = CollectionsMarshal.AsSpan(entities);
		span2.CopyTo(span.Slice(num, span2.Length));
		num += span2.Length;
		foreach (GameEntity item in list)
		{
			int num2 = 20;
			if (item.type_name == "Item" || item.type_name == "Lever" || (!Settings.config.MapSettings.Entities && item.id != 2004 && item.type_name == "NPC" && item.id != QuestManager.quest_target_id && item.id != QuestManager.teleporting_npc_id) || (!Settings.config.MapSettings.Pets && (item.type_name == "Pet" || item.type_name == "Partner")))
			{
				continue;
			}
			int num3 = (int)Math.Round((double)item.x * ratioX);
			int num4 = (int)Math.Round((double)item.y * ratioY);
			int num5 = 4;
			using SolidBrush solidBrush = new SolidBrush(Color.White);
			Color color = EntitiesColor;
			if (item.type_name == "portal")
			{
				color = PortalColor;
				num2 = 28;
			}
			if (RaidManager.IsInUltimateArmaBossroom)
			{
				num2 /= 2;
			}
			if ((item.id == QuestManager.quest_target_id || item.id == QuestManager.teleporting_npc_id) && item.type_name != "portal")
			{
				num2 *= 2;
				num5 *= 2;
				int num6 = (int)((float)num2 * 1.5f);
				graphics.FillEllipse(solidBrush, new Rectangle(num3 - (num6 + num5) / 2, num4 - (num6 + num5) / 2, num6 + num5, num6 + num5));
			}
			new Rectangle(num3 - num2, num4 - num2, 2 * num2, 2 * num2);
			if (item.id == 2004)
			{
				color = IceFlowerColor;
			}
			if (item.type_name == "Pet" || item.type_name == "Partner")
			{
				color = PetsColor;
			}
			solidBrush.Color = color;
			if (Settings.config.low_spec && item.type_name != "portal")
			{
				num2 /= 2;
			}
			graphics.FillEllipse(solidBrush, new Rectangle(num3 - (num2 + num5) / 2, num4 - (num2 + num5) / 2, num2 + num5, num2 + num5));
			if (!Settings.config.low_spec && item.icon != null)
			{
				graphics.DrawImage(item.icon, new Rectangle(num3 - num2 / 2, num4 - num2 / 2, num2, num2));
			}
		}
		return bitmap;
	}

	public static Bitmap DrawLeversOnBitmap(Bitmap bitmap, double ratioX, double ratioY)
	{
		using Graphics graphics = Graphics.FromImage(bitmap);
		int num = 18;
		foreach (GameEntity item in (from x in GUI.entities
			where x.type_name == "Lever"
			orderby x.is_required_lever descending
			select x).ToList())
		{
			int num2 = (int)Math.Round((double)item.x * ratioX);
			int num3 = (int)Math.Round((double)item.y * ratioY);
			Color color = NotRequiredLeverColor;
			if (item.id == 1135 || item.id == 1000)
			{
				color = LeversNotPressedColor;
			}
			if (item.id == 1136 || item.id == 1045)
			{
				color = LeversPressedColor;
			}
			if (!item.is_required_lever)
			{
				color = NotRequiredLeverColor;
			}
			using SolidBrush brush = new SolidBrush(color);
			graphics.FillEllipse(brush, new Rectangle(num2 - num / 2, num3 - num / 2, num, num));
		}
		return bitmap;
	}

	public static Bitmap DrawTimeSpacesOnBitmap(Bitmap bitmap, double ratioX, double ratioY)
	{
		if (TimeSpaceIcon == null && File.Exists("images\\npcs\\time_space.png"))
		{
			TimeSpaceIcon = Image.FromFile("images\\npcs\\time_space.png");
		}
		using Graphics graphics = Graphics.FromImage(bitmap);
		int num = 30;
		int num2 = 8;
		List<GameTimeSpace> time_spaces = GUI.time_spaces;
		List<GameTimeSpace> list = new List<GameTimeSpace>();
		CollectionsMarshal.SetCount(list, time_spaces.Count);
		Span<GameTimeSpace> span = CollectionsMarshal.AsSpan(list);
		int num3 = 0;
		Span<GameTimeSpace> span2 = CollectionsMarshal.AsSpan(time_spaces);
		span2.CopyTo(span.Slice(num3, span2.Length));
		num3 += span2.Length;
		foreach (GameTimeSpace item in list)
		{
			int num4 = (int)Math.Round((double)item.x * ratioX);
			int num5 = (int)Math.Round((double)item.y * ratioY);
			new Rectangle(num4 - num, num5 - num, 2 * num, 2 * num);
			Color color = TimeSpaceColorDefault;
			if (item.state == 12)
			{
				color = TimeSpaceColorHeroic;
			}
			else if (item.state == 4)
			{
				color = TimeSpaceColorCompleted;
			}
			else if (item.state == 1)
			{
				color = TimeSpaceColorCurrentQuest;
			}
			else if (item.state == 0)
			{
				color = TimeSpaceColorNotCompleted;
			}
			using SolidBrush brush = new SolidBrush(color);
			graphics.FillEllipse(brush, new Rectangle(num4 - (num + num2) / 2, num5 - (num + num2) / 2, num + num2, num + num2));
			if (TimeSpaceIcon != null)
			{
				graphics.DrawImage(TimeSpaceIcon, new Rectangle(num4 - num / 2, num5 - num / 2, num, num));
			}
		}
		return bitmap;
	}

	public static Bitmap DrawMonstersOnBitmap(Bitmap bitmap, double ratioX, double ratioY)
	{
		using Graphics graphics = Graphics.FromImage(bitmap);
		List<GameMonster> list = new List<GameMonster>();
		list.AddRange(GUI.monsters.Where((GameMonster x) => !x.is_boss && !x.is_special && !x.is_semi_exclusive));
		foreach (GameMonster monster in list)
		{
			if (!Settings.config.MapSettings.Mobs && !monster.is_special && !monster.is_boss && !monster.is_semi_exclusive && !RaidManager.IsInUltimateArmaBossroom)
			{
				NostaleCharacterInfo? mapper = GUI.Mapper;
				if ((mapper == null || mapper.real_map_id != 2752) && !QuestManager.mobs_to_hunt.Contains(monster.id))
				{
					continue;
				}
			}
			if (mob_filters.Any((GameMonster x) => x.id == monster.id))
			{
				continue;
			}
			int num = (int)Math.Round((double)monster.x * ratioX);
			int num2 = (int)Math.Round((double)monster.y * ratioY);
			int num3 = 16;
			int num4 = 6;
			bool flag = false;
			Color color = MonstersColor;
			flag = QuestManager.mobs_to_hunt.Contains(monster.id);
			if (monster.id == 2376 || monster.id == 2377)
			{
				num3 = 32;
				num4 = 10;
				color = ((monster.id == 2376) ? Color.Red : Color.DarkBlue);
				flag = monster.hp_percent == 100;
			}
			else if (monster.id == 1563 || monster.id == 1564)
			{
				num3 = 32;
				num4 = 10;
				color = ((monster.id == 1563) ? Color.Red : Color.DarkBlue);
			}
			if (RaidManager.IsInUltimateArmaBossroom)
			{
				num3 /= 2;
			}
			using SolidBrush solidBrush = new SolidBrush(color);
			if (Settings.config.low_spec)
			{
				NostaleCharacterInfo? mapper2 = GUI.Mapper;
				if (mapper2 == null || mapper2.map_id != 2759)
				{
					num3 /= 2;
				}
			}
			if (flag)
			{
				int num5 = (int)((float)num3 * 1.5f);
				solidBrush.Color = Color.White;
				graphics.FillEllipse(solidBrush, new Rectangle(num - (num5 + num4) / 2, num2 - (num5 + num4) / 2, num5 + num4, num5 + num4));
				solidBrush.Color = color;
			}
			graphics.FillEllipse(solidBrush, new Rectangle(num - (num3 + num4) / 2, num2 - (num3 + num4) / 2, num3 + num4, num3 + num4));
			NostaleCharacterInfo? mapper3 = GUI.Mapper;
			if (((mapper3 == null || mapper3.map_id != 2759) && Settings.config.low_spec) || monster.icon == null)
			{
				continue;
			}
			graphics.DrawImage(monster.icon, new Rectangle(num - num3 / 2, num2 - num3 / 2, num3, num3));
			if (monster.id == 2376 || monster.id == 2377)
			{
				int num6 = (int)((double)num3 * 1.25);
				int num7 = 8;
				int x2 = num - num6 / 2;
				int y = num2 - (num3 / 2 + num7 + 2);
				int num8 = num6 * monster.hp_percent / 100;
				graphics.FillRectangle(Brushes.Black, new Rectangle(x2, y, num6, num7));
				using SolidBrush brush = new SolidBrush(ColorFromHPRatio((double)monster.hp_percent / 100.0));
				if (num8 <= 3 && monster.hp_percent > 0)
				{
					num8 = 3;
				}
				graphics.FillRectangle(brush, new Rectangle(x2, y, num8, num7));
			}
			else if (monster.id == 1563 || monster.id == 1564)
			{
				int num9 = (int)((double)num3 * 0.75);
				int x3 = num - num9 / 2;
				int y2 = num2 - num9 / 2;
				using SolidBrush brush2 = new SolidBrush(Color.Yellow);
				graphics.FillEllipse(brush2, new Rectangle(x3, y2, num9, num9));
			}
		}
		return bitmap;
	}

	public static Bitmap DrawDangerousCirclesOnBitmap(Bitmap bitmap, double ratioX, double ratioY)
	{
		using Graphics graphics = Graphics.FromImage(bitmap);
		List<GameMonster> list = new List<GameMonster>();
		list.AddRange(GUI.monsters.Where((GameMonster x) => x.is_circle.Item1));
		foreach (GameMonster item in list)
		{
			int num = (int)Math.Round((double)item.x * ratioX);
			int num2 = (int)Math.Round((double)item.y * ratioY);
			int num3 = (int)((double)item.is_circle.Item2 * ratioX);
			int num4 = (int)((double)item.is_circle.Item2 * ratioY);
			using SolidBrush brush = new SolidBrush(MonstersColor);
			Rectangle rect = new Rectangle(num - num3, num2 - num4, 2 * num3, 2 * num4);
			graphics.FillRectangle(brush, rect);
			if (GUI.Main != null && Utils.IsInSquare(new Point(GUI.Main.x_pos, GUI.Main.y_pos), new Rectangle(item.x - item.is_circle.Item2 - 1, item.y - item.is_circle.Item2 - 1, item.is_circle.Item2 * 2 + 1, item.is_circle.Item2 * 2 + 1)))
			{
				Controller.PlaySound("Run");
			}
		}
		return bitmap;
	}

	public static Bitmap DrawSpecialMonstersOnBitmap(Bitmap bitmap, double ratioX, double ratioY)
	{
		using Graphics graphics = Graphics.FromImage(bitmap);
		int num = 16;
		int num2 = 6;
		List<GameMonster> list = new List<GameMonster>();
		list.AddRange(GUI.monsters.Where((GameMonster x) => x.is_boss || x.is_special || x.is_semi_exclusive));
		foreach (GameMonster item in list)
		{
			int num3 = (int)Math.Round((double)item.x * ratioX);
			int num4 = (int)Math.Round((double)item.y * ratioY);
			num = (item.is_semi_exclusive ? 24 : 16);
			num = (item.is_special ? 32 : num);
			num = (item.is_boss ? 40 : num);
			Color color = (item.is_semi_exclusive ? SemiExclusiveMonstersColor : MonstersColor);
			color = (item.is_special ? SpecialMonstersColor : color);
			color = (item.is_boss ? BossMonsterColor : color);
			if (RaidManager.IsInUltimateArmaBossroom)
			{
				num /= 2;
			}
			using SolidBrush brush = new SolidBrush(color);
			if (Settings.config.low_spec)
			{
				num /= 2;
			}
			graphics.FillEllipse(brush, new Rectangle(num3 - (num + num2) / 2, num4 - (num + num2) / 2, num + num2, num + num2));
			if (!Settings.config.low_spec && item.icon != null)
			{
				graphics.DrawImage(item.icon, new Rectangle(num3 - num / 2, num4 - num / 2, num, num));
			}
		}
		return bitmap;
	}

	public static Bitmap DrawPlayersOnBitmap(Bitmap bitmap, double ratioX, double ratioY)
	{
		if (!Settings.config.MapSettings.Players && !RaidManager.raidStarted)
		{
			return bitmap;
		}
		using Graphics graphics = Graphics.FromImage(bitmap);
		List<GamePlayer> players = GUI.players;
		List<GamePlayer> list = new List<GamePlayer>();
		CollectionsMarshal.SetCount(list, players.Count);
		Span<GamePlayer> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		Span<GamePlayer> span2 = CollectionsMarshal.AsSpan(players);
		span2.CopyTo(span.Slice(num, span2.Length));
		num += span2.Length;
		foreach (GamePlayer item in list)
		{
			int num2 = (int)Math.Round((double)item.x * ratioX);
			int num3 = (int)Math.Round((double)item.y * ratioY);
			int num4 = 24;
			int num5 = 6;
			Color color = item.color;
			if (color != PlayersColor && !RaidManager.IsInUltimateArmaBossroom)
			{
				num4 *= 2;
			}
			if (num2 == 0 && num3 == 0)
			{
				continue;
			}
			new Rectangle(num2 - num4, num3 - num4, 2 * num4, 2 * num4);
			if (GUI.Mapper != null && MapID.isGlacernonMap(GUI.Mapper.real_map_id))
			{
				color = ((item.fraction == "demon") ? Color.FromArgb(125, 89, 178) : Color.FromArgb(239, 225, 237));
				num4 = 30;
				num5 = 10;
			}
			if (RaidManager.IsInUltimateArmaBossroom)
			{
				num4 /= 2;
			}
			using SolidBrush brush = new SolidBrush(color);
			if (Settings.config.low_spec)
			{
				num4 /= 2;
			}
			graphics.FillEllipse(brush, new Rectangle(num2 - (num4 + num5) / 2, num3 - (num4 + num5) / 2, num4 + num5, num4 + num5));
			if (!Settings.config.low_spec && item.icon != null)
			{
				graphics.DrawImage(item.icon, new Rectangle(num2 - num4 / 2, num3 - num4 / 2, num4, num4));
			}
		}
		return bitmap;
	}

	public static Bitmap DrawSelfOnBitmap(Bitmap bitmap, double ratioX, double ratioY)
	{
		using Graphics graphics = Graphics.FromImage(bitmap);
		List<NostaleCharacterInfo> list = new List<NostaleCharacterInfo>();
		list.AddRange(GUI._nostaleCharacterInfoList.OrderBy((NostaleCharacterInfo x) => x.hwnd == GUI.Mapper.hwnd));
		foreach (NostaleCharacterInfo item in list)
		{
			if ((item == GUI.Mapper && !Settings.config.MapSettings.Mapper) || (item != GUI.Mapper && !Settings.config.MapSettings.Alts))
			{
				continue;
			}
			int num = (int)Math.Round((double)item.x_pos * ratioX);
			int num2 = (int)Math.Round((double)item.y_pos * ratioY);
			int num3 = ((item.hwnd == GUI.Mapper.hwnd) ? 40 : 32);
			int num4 = 8;
			Color color = ((item.hwnd == GUI.Mapper.hwnd) ? MapperColor : AltColor);
			color = ((!item.special_color.HasValue) ? color : item.special_color.Value);
			if ((num == 0 && num2 == 0) || item.map_id != GUI.Mapper.map_id || item.server != GUI.Mapper.server || item.channel != GUI.Mapper.channel)
			{
				continue;
			}
			using SolidBrush brush = new SolidBrush(color);
			if (RaidManager.IsInUltimateArmaBossroom)
			{
				num3 /= 2;
			}
			if (Settings.config.low_spec)
			{
				num3 /= 2;
			}
			graphics.FillEllipse(brush, new Rectangle(num - (num3 + num4) / 2, num2 - (num3 + num4) / 2, num3 + num4, num3 + num4));
			if (!Settings.config.low_spec && item.icon != null)
			{
				graphics.DrawImage(item.icon, new Rectangle(num - num3 / 2, num2 - num3 / 2, num3, num3));
			}
		}
		return bitmap;
	}

	public static Bitmap DrawFieldsOnBitmap(Bitmap bitmap, double ratioX, double ratioY)
	{
		using Graphics graphics = Graphics.FromImage(bitmap);
		List<GameField> fields = GUI.fields;
		List<GameField> list = new List<GameField>();
		CollectionsMarshal.SetCount(list, fields.Count);
		Span<GameField> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		Span<GameField> span2 = CollectionsMarshal.AsSpan(fields);
		span2.CopyTo(span.Slice(num, span2.Length));
		num += span2.Length;
		foreach (GameField item in list)
		{
			int num2 = (int)Math.Round((double)item.x * ratioX);
			int num3 = (int)Math.Round((double)item.y * ratioY);
			int num4 = (int)(1.0 * ratioX);
			int num5 = (int)(5.0 * ratioX);
			int num6 = (int)(5.0 * ratioY);
			Rectangle rect = new Rectangle(num2 - num5, num3 - num6, 2 * num5, 2 * num6);
			using Pen pen = new Pen(item.Color, num4);
			graphics.DrawRectangle(pen, rect);
		}
		return bitmap;
	}

	public static Bitmap DrawQuestNavigatorOnBitmap(Bitmap bitmap, double ratioX, double ratioY)
	{
		if (GUI.Mapper == null)
		{
			return bitmap;
		}
		using (Graphics.FromImage(bitmap))
		{
			QuestManager.teleporting_npc_id = -1;
			if (QuestManager.path_to_quest_target.Count > 1)
			{
				GameEntity gameEntity = (from x in GUI.entities.Where((GameEntity x) => x.type_name == "portal" && x.portal_target_map_id == QuestManager.path_to_quest_target.ElementAt(1)).ToList()
					orderby Utils.CalculateDistance(new Point(GUI.Mapper.x_pos, GUI.Mapper.y_pos), new Point(x.x, x.y))
					select x).FirstOrDefault();
				if (gameEntity != null)
				{
					bitmap = DrawArrow(bitmap, GUI.Mapper.x_pos, GUI.Mapper.y_pos, gameEntity.x, gameEntity.y, ratioX, ratioY);
					return bitmap;
				}
				if (!NAvigator.game_world.ContainsKey(GUI.Mapper.real_map_id))
				{
					return bitmap;
				}
				GameMap gameMap = NAvigator.game_world[GUI.Mapper.real_map_id];
				if (gameMap == null)
				{
					return bitmap;
				}
				MapNPC teleporting_npc = gameMap.NPCsList.Find((MapNPC x) => x.destination_maps.Any((int y) => y == QuestManager.path_to_quest_target.ElementAt(1)));
				if (teleporting_npc == null)
				{
					return bitmap;
				}
				GameEntity gameEntity2 = (from x in GUI.entities
					where x.id == teleporting_npc.ID
					orderby Utils.CalculateDistance(new Point(GUI.Mapper.x_pos, GUI.Mapper.y_pos), new Point(x.x, x.y))
					select x).FirstOrDefault();
				QuestManager.teleporting_npc_id = gameEntity2?.id ?? (-1);
				if (gameEntity2 == null)
				{
					return bitmap;
				}
				bitmap = DrawArrow(bitmap, GUI.Mapper.x_pos, GUI.Mapper.y_pos, gameEntity2.x, gameEntity2.y, ratioX, ratioY);
				return bitmap;
			}
			if (QuestManager.path_to_quest_target.Count == 1)
			{
				if (QuestManager.quest_target_type == "NPC")
				{
					GameEntity gameEntity3 = (from x in GUI.entities
						where x.id == QuestManager.quest_target_id && x.type_name == "NPC"
						orderby Utils.CalculateDistance(new Point(GUI.Mapper.x_pos, GUI.Mapper.y_pos), new Point(x.x, x.y))
						select x).FirstOrDefault();
					if (gameEntity3 != null)
					{
						bitmap = DrawArrow(bitmap, GUI.Mapper.x_pos, GUI.Mapper.y_pos, gameEntity3.x, gameEntity3.y, ratioX, ratioY);
						return bitmap;
					}
				}
				else if (QuestManager.quest_target_type == "TimeSpace")
				{
					GameTimeSpace gameTimeSpace = GUI.time_spaces.Find((GameTimeSpace x) => x.ID == QuestManager.quest_target_id);
					if (gameTimeSpace != null)
					{
						bitmap = DrawArrow(bitmap, GUI.Mapper.x_pos, GUI.Mapper.y_pos, gameTimeSpace.x, gameTimeSpace.y, ratioX, ratioY);
						return bitmap;
					}
				}
				else if (QuestManager.quest_target_type == "Map")
				{
					bitmap = DrawArrow(bitmap, GUI.Mapper.x_pos, GUI.Mapper.y_pos, QuestManager.target_location.X, QuestManager.target_location.Y, ratioX, ratioY);
					return bitmap;
				}
			}
		}
		return bitmap;
	}

	public static Bitmap ZoomUltArmaBossRoom(Bitmap bitmap, double ratioX, double ratioY)
	{
		if (!RaidManager.IsInUltimateArmaBossroom)
		{
			return bitmap;
		}
		int num = (int)(83.0 * ratioX / 2.0);
		int num2 = (int)(83.0 * ratioX / 2.0);
		int num3 = (int)(386.0 * ratioY / 2.0);
		int num4 = bitmap.Width - num - num2;
		int num5 = bitmap.Height - num3;
		if (num4 > 0 && num5 > 0)
		{
			Bitmap bitmap2 = new Bitmap(num4, num5);
			using (Graphics graphics = Graphics.FromImage(bitmap2))
			{
				Rectangle srcRect = new Rectangle(num, num3, num4, num5);
				Rectangle destRect = new Rectangle(0, 0, num4, num5);
				graphics.DrawImage(bitmap, destRect, srcRect, GraphicsUnit.Pixel);
			}
			bitmap.Dispose();
			bitmap = bitmap2;
		}
		return bitmap;
	}

	public static Bitmap DrawArrow(Bitmap bitmap, int x_src, int y_src, int x_dest, int y_dest, double ratioX, double ratioY)
	{
		if (x_dest == -1 && y_dest == -1)
		{
			return bitmap;
		}
		using Graphics graphics = Graphics.FromImage(bitmap);
		float num = (float)((double)x_src * ratioX);
		float num2 = (float)((double)y_src * ratioY);
		float num3 = (float)((double)x_dest * ratioX);
		float num4 = (float)((double)y_dest * ratioY);
		using (Pen pen = new Pen(QuestArrowColor, 5f))
		{
			graphics.DrawLine(pen, num, num2, num3, num4);
		}
		float num5 = (float)Math.Atan2(num4 - num2, num3 - num);
		PointF[] points = new PointF[3]
		{
			new PointF(num3, num4),
			new PointF(num3 - 20f * (float)Math.Cos((double)num5 - Math.PI / 6.0), num4 - 20f * (float)Math.Sin((double)num5 - Math.PI / 6.0)),
			new PointF(num3 - 20f * (float)Math.Cos((double)num5 + Math.PI / 6.0), num4 - 20f * (float)Math.Sin((double)num5 + Math.PI / 6.0))
		};
		using Brush brush = new SolidBrush(QuestArrowColor);
		graphics.FillPolygon(brush, points);
		return bitmap;
	}

	public static async Task<Bitmap> DrawTSDataMap(Bitmap bitmap, int time_space_id)
	{
		TimeSpaceData timeSpaceData;
		if (QuestManager.current_time_space_data.ID != time_space_id)
		{
			timeSpaceData = await NAHttpClient.GetTimeSpaceData(time_space_id);
			if (timeSpaceData != null)
			{
				QuestManager.current_time_space_data = timeSpaceData;
			}
		}
		else
		{
			timeSpaceData = QuestManager.current_time_space_data;
		}
		if (timeSpaceData == null || timeSpaceData.rooms.Count == 0)
		{
			return bitmap;
		}
		QuestManager.current_time_space_data = timeSpaceData;
		int num = 600;
		int num2 = 25;
		int num3 = 10;
		Color.FromArgb(58, 12, 163);
		Color.FromArgb(76, 201, 240);
		int x2 = timeSpaceData.rooms.OrderBy((TimeSpaceRoom x) => x.x).First().x;
		int x3 = timeSpaceData.rooms.OrderBy((TimeSpaceRoom x) => x.x).Last().x;
		int y = timeSpaceData.rooms.OrderBy((TimeSpaceRoom x) => x.y).First().y;
		int y2 = timeSpaceData.rooms.OrderBy((TimeSpaceRoom x) => x.y).Last().y;
		int num4 = Math.Abs(x2 - x3) + 1;
		int num5 = Math.Abs(y - y2) + 1;
		int num6 = Math.Max(num5, num4);
		int num7 = (num - 2 * num2 - (num6 - 1) * num3) / num6;
		int num8 = (900 - (2 * num2 + (num4 - 1) * num3 + num4 * num7)) / 2;
		int num9 = (num - (2 * num2 + (num5 - 1) * num3 + num5 * num7)) / 2;
		int num10 = num8 + num2 + (TimeSpaceManager.current_player_position.X - x2) * (num7 + num3) + num7 / 3;
		int num11 = num9 + num2 + (TimeSpaceManager.current_player_position.Y - y) * (num7 + num3) + num7 / 3;
		using SolidBrush solidBrush = new SolidBrush(Color.White);
		using (Graphics graphics = Graphics.FromImage(bitmap))
		{
			if (TimeSpaceManager.ts_started)
			{
				int num12 = num7 / 3;
				int num13 = 8;
				int num14 = 4;
				graphics.FillEllipse(solidBrush, new Rectangle(num10 - (num13 + num14) / 2, num11 - (num13 + num14) / 2, num12 + (num13 + num14), num12 + (num13 + num14)));
				solidBrush.Color = MapperColor;
				graphics.FillEllipse(solidBrush, new Rectangle(num10 - num13 / 2, num11 - num13 / 2, num12 + num13, num12 + num13));
				if (GUI.Mapper?.icon != null)
				{
					graphics.DrawImage(GUI.Mapper.icon, new Rectangle(num10, num11, num12, num12));
				}
			}
			if (QuestManager.highlighted_time_space_room.X == -1 || QuestManager.highlighted_time_space_room.Y == -1)
			{
				return bitmap;
			}
			int x4 = num8 + num2 + (QuestManager.highlighted_time_space_room.X - x2) * (num7 + num3);
			int y3 = num9 + num2 + (QuestManager.highlighted_time_space_room.Y - y) * (num7 + num3);
			solidBrush.Color = Color.FromArgb(180, 255, 255, 0);
			graphics.FillRectangle(solidBrush, x4, y3, num7, num7);
			TimeSpaceRoom timeSpaceRoom = QuestManager.current_time_space_data.rooms.Find((TimeSpaceRoom r) => r.x == QuestManager.highlighted_time_space_room.X && r.y == QuestManager.highlighted_time_space_room.Y);
			if (timeSpaceRoom == null || timeSpaceRoom.mobs.Count == 0)
			{
				return bitmap;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (TimeSpaceMonster item in timeSpaceRoom.mobs.OrderBy((TimeSpaceMonster x) => x.ID))
			{
				if (!dictionary.ContainsKey(item.ID))
				{
					dictionary[item.ID] = 0;
				}
				dictionary[item.ID]++;
			}
			int num15 = 30;
			int num16 = 10;
			int num17 = num16;
			int height = (num15 + num16) * dictionary.Count + num16;
			int width = 100;
			solidBrush.Color = RankingFamMemberColor;
			graphics.FillRectangle(solidBrush, num16, num16, width, height);
			Color white = Color.White;
			solidBrush.Color = white;
			foreach (int key in dictionary.Keys)
			{
				Image icon = GameMonster.GetIcon(key);
				if (icon != null)
				{
					graphics.DrawImage(icon, new Rectangle(num16 * 2, num17 + num16, num15, num15));
				}
				graphics.DrawString($"x{dictionary[key]}", new Font("Microsoft Sans Serif", 14f, FontStyle.Bold), solidBrush, new PointF(num16 * 2 + num15 + num16, num16 + num17 + num15 / 4));
				num17 += num15 + num16;
			}
		}
		return bitmap;
	}

	public static Bitmap? GenerateSearchQuestInstanceMap(int instance_map, string instance_type, int instance_x, int instance_y, Image? instance_icon, int mob_count)
	{
		MapDto mapDto = NAvigator.world_maps.Find((MapDto x) => x.mapId == instance_map);
		if (mapDto == null)
		{
			return null;
		}
		int fileMapId = mapDto.fileMapId;
		string text = $"images\\maps\\{fileMapId}.png";
		string text2 = $"images\\maps\\{fileMapId}_org.png";
		if (!Path.Exists(text) || !Path.Exists(text2))
		{
			return null;
		}
		Bitmap bitmap = new Bitmap(text);
		Bitmap bitmap2 = new Bitmap(text2);
		switch (instance_type)
		{
		case "Map":
			return bitmap;
		case "NPC":
		case "TimeSpace":
		{
			double num5 = (double)bitmap.Width / (double)bitmap2.Width * 2.0;
			double num6 = (double)bitmap.Height / (double)bitmap2.Height * 2.0;
			int num7 = (int)Math.Round((double)instance_x * num5);
			int num8 = (int)Math.Round((double)instance_y * num6);
			int num9 = 40;
			int num10 = 8;
			Color color = Color.White;
			if (instance_type == "NPC")
			{
				color = EntitiesColor;
			}
			else if (instance_type == "TimeSpace")
			{
				color = Color.Yellow;
			}
			using (SolidBrush brush2 = new SolidBrush(color))
			{
				using Graphics graphics2 = Graphics.FromImage(bitmap);
				graphics2.FillEllipse(brush2, new Rectangle(num7 - (num9 + num10) / 2, num8 - (num9 + num10) / 2, num9 + num10, num9 + num10));
				if (instance_icon == null)
				{
					return bitmap;
				}
				graphics2.DrawImage(instance_icon, new Rectangle(num7 - num9 / 2, num8 - num9 / 2, num9, num9));
			}
			break;
		}
		case "Mob":
		{
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				int num = 80;
				int num2 = 20;
				int num3 = bitmap.Width - num - num2;
				int num4 = num2;
				if (instance_icon != null)
				{
					graphics.DrawImage(instance_icon, new Rectangle(num3, num4, num, num));
				}
				string text3 = $"{mob_count}x";
				Font font = new Font("Microsoft Sans Serif", 40f, FontStyle.Bold);
				using SolidBrush brush = new SolidBrush(CounterForeColor);
				PointF point = new PointF((float)num3 - graphics.MeasureString(text3, font).Width - 5f, (float)num4 + ((float)num - graphics.MeasureString(text3, font).Height) / 2f);
				graphics.DrawString(text3, font, brush, point);
			}
			break;
		}
		}
		return bitmap;
	}

	public static Point? GetRoomCoordinatesFromMousePosition(Point mousePosition, int width, int height, TimeSpaceData current_ts)
	{
		if (current_ts == null || current_ts.rooms.Count == 0)
		{
			return null;
		}
		int num = 900;
		int num2 = 600;
		int num3 = 10;
		float num4 = Math.Min((float)width / (float)num, (float)height / (float)num2);
		int num5 = (int)(25f * num4);
		int num6 = (int)((float)num3 * num4);
		int x2 = current_ts.rooms.OrderBy((TimeSpaceRoom x) => x.x).First().x;
		int x3 = current_ts.rooms.OrderBy((TimeSpaceRoom x) => x.x).Last().x;
		int y = current_ts.rooms.OrderBy((TimeSpaceRoom x) => x.y).First().y;
		int y2 = current_ts.rooms.OrderBy((TimeSpaceRoom x) => x.y).Last().y;
		int num7 = Math.Abs(x2 - x3) + 1;
		int num8 = Math.Abs(y - y2) + 1;
		int num9 = Math.Max(num8, num7);
		int num10 = (height - 2 * num5 - (num9 - 1) * num6) / num9;
		int num11 = (width - (2 * num5 + (num7 - 1) * num6 + num7 * num10)) / 2;
		int num12 = (height - (2 * num5 + (num8 - 1) * num6 + num8 * num10)) / 2;
		int num13 = mousePosition.X - num11 - num5;
		int num14 = mousePosition.Y - num12 - num5;
		if (num13 < 0 || num14 < 0)
		{
			return null;
		}
		int roomX = num13 / (num10 + num6);
		int roomY = num14 / (num10 + num6);
		if (roomX >= num7 || roomY >= num8)
		{
			return null;
		}
		int num15 = num13 % (num10 + num6);
		int num16 = num14 % (num10 + num6);
		if (num15 >= num10 || num16 >= num10)
		{
			return null;
		}
		roomX += x2;
		roomY += y;
		if (!current_ts.rooms.Any((TimeSpaceRoom x) => x.x == roomX && x.y == roomY))
		{
			return null;
		}
		return new Point(roomX, roomY);
	}

	public static Bitmap OverlayMinimap(Bitmap bigMap, Bitmap miniMap)
	{
		int width = (int)((double)bigMap.Width * 0.25);
		int num = (int)((double)bigMap.Height * 0.25);
		Bitmap image = new Bitmap(miniMap, new Size(width, num));
		using Graphics graphics = Graphics.FromImage(bigMap);
		int x = 2;
		int y = bigMap.Height - num - 2;
		graphics.DrawImage(image, x, y, width, num);
		return bigMap;
	}
}
