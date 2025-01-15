using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NosAssistant2.Dtos.Output;
using NosAssistant2.EnumsID;
using NosAssistant2.GameData;
using NosAssistant2.GameObjects;

namespace NosAssistant2.Helpers;

public static class QuestManager
{
	public static int quest_target_id = -1;

	public static List<int> mobs_to_hunt = new List<int>();

	public static string quest_target_type = "";

	public static Point target_location = new Point(-1, -1);

	public static string target_location_map = "";

	public static string time_space_name = "";

	public static string quest_search_type = "NPC";

	public static string quest_search_selected_item = "";

	public static int teleporting_npc_id = -1;

	public static int questline_position = -1;

	public static Bitmap? QuestSearchInstanceMap = null;

	public static bool ShowQuestSearchInstanceMap = false;

	public static int navigating_instance_map_id = -1;

	public static int navigating_instance_id = -1;

	public static string navigating_instance_type = "";

	public static Point highlighted_time_space_room = new Point(-1, -1);

	public static TimeSpaceData current_time_space_data = new TimeSpaceData();

	public static List<GameQuest> quests { get; set; } = new List<GameQuest>();


	public static GameQuest? followed_quest { get; set; } = null;


	public static QuestDto? followed_quest_data { get; set; } = null;


	public static List<int> path_to_quest_target { get; set; } = new List<int>();


	public static void UnselectQuest()
	{
		followed_quest = null;
		followed_quest_data = null;
		path_to_quest_target = new List<int>();
		mobs_to_hunt = new List<int>();
		quest_target_type = "NPC";
		target_location = new Point(-1, -1);
		target_location_map = "";
		time_space_name = "";
		teleporting_npc_id = -1;
		questline_position = -1;
		GUI.form.UpdateQuestsPanel();
	}

	public static void StopNavigating()
	{
		UnselectQuest();
		navigating_instance_map_id = -1;
		navigating_instance_id = -1;
		navigating_instance_type = "";
		ShowQuestSearchInstanceMap = false;
		quest_search_selected_item = "";
	}

	public static async void UpdateCurrentQuest(int _questline_position)
	{
		questline_position = _questline_position;
		GameQuest gameQuest = quests.Find((GameQuest x) => x.questPosition == questline_position);
		if (gameQuest == null)
		{
			followed_quest = null;
			path_to_quest_target.Clear();
			followed_quest_data = null;
			GUI.form.UpdateQuestsPanel();
			return;
		}
		followed_quest = gameQuest;
		QuestDto questDto = await NAHttpClient.GetQuestData(gameQuest.questID.ToString());
		if (questDto != null)
		{
			followed_quest_data = questDto;
			UpdateQuestTarget();
		}
	}

	public static void UpdateCurrentQuestClick(string quest_label)
	{
		if (quest_label.Contains("Main"))
		{
			UpdateCurrentQuest(5);
			return;
		}
		int num = Convert.ToInt32(quest_label.Split("#").ElementAt(1));
		if (quest_label.Contains("Side"))
		{
			UpdateCurrentQuest(quests.Where((GameQuest x) => x.questPosition > 5).ToList().ElementAt(num - 1)
				.questPosition);
			}
			else if (quest_label.Contains("Flower"))
			{
				UpdateCurrentQuest(quests.Where((GameQuest x) => x.questPosition < 5).ToList().ElementAt(num - 1)
					.questPosition);
				}
			}

			public static void UpdateQuestList(List<GameQuest> gameQuests)
			{
				quests = gameQuests.ToList();
				quests = (from q in quests
					orderby q.questPosition != 5, (q.questPosition <= 5) ? 1 : 0, q.questPosition
					select q).ToList();
				followed_quest = gameQuests.Find((GameQuest x) => x.questPosition == questline_position);
				if (followed_quest == null)
				{
					GUI.form.UpdateQuestsPanel();
				}
				else
				{
					UpdateQuestTarget();
				}
			}

			public static async void UpdateQuestTarget()
			{
				if (followed_quest_data == null)
				{
					GUI.form.UpdateQuestsPanel();
					return;
				}
				path_to_quest_target = GetPathToQuestTarget(followed_quest_data);
				if (path_to_quest_target.Count != 0)
				{
					target_location_map = MapID.GetMapName(path_to_quest_target.Last());
				}
				GUI.form.UpdateQuestsPanel();
			}

			public static void PrintQuestList()
			{
			}

			public static List<int> GetPathToQuestTarget(QuestDto quest)
			{
				if (quest == null)
				{
					return new List<int>();
				}
				int num = 0;
				quest_target_type = "";
				quest_target_id = -1;
				mobs_to_hunt.Clear();
				TimeSpaceDto timeSpaceDto = null;
				switch (quest.questType)
				{
				case 7:
					quest_target_type = "TimeSpace";
					quest_target_id = quest.data.ElementAt(0).ElementAt(0);
					timeSpaceDto = NAvigator.world_time_spaces.Find((TimeSpaceDto x) => x.timeSpaceId == quest_target_id);
					if (timeSpaceDto == null)
					{
						return new List<int>();
					}
					time_space_name = timeSpaceDto.name;
					break;
				case 13:
					quest_target_type = "TimeSpace";
					quest_target_id = quest.data.ElementAt(0).ElementAt(0);
					timeSpaceDto = NAvigator.world_time_spaces.Find((TimeSpaceDto x) => x.timeSpaceId == quest_target_id);
					if (timeSpaceDto == null)
					{
						return new List<int>();
					}
					time_space_name = timeSpaceDto.name;
					break;
				case 12:
				case 14:
				case 15:
					quest_target_type = "NPC";
					quest_target_id = quest.data.ElementAt(0).ElementAt(0);
					break;
				case 2:
					if (quest.target.ElementAt(2) == 0)
					{
						quest_target_type = "NPC";
						quest_target_id = quest.talk.ElementAt(2);
						break;
					}
					quest_target_type = "Map";
					target_location.X = quest.target.ElementAt(0);
					target_location.Y = quest.target.ElementAt(1);
					quest_target_id = quest.target.ElementAt(2);
					break;
				case 4:
					if (quest.target.Count >= 3 && quest.target.ElementAt(2) != 0 && quest.target.ElementAt(2) != -1 && followed_quest != null && followed_quest.QuestProgress.Any(((int actualCount, int maxCount) x) => x.actualCount != x.maxCount))
					{
						quest_target_type = "Map";
						target_location.X = quest.target.ElementAt(0);
						target_location.Y = quest.target.ElementAt(1);
						quest_target_id = quest.target.ElementAt(2);
					}
					else
					{
						quest_target_type = "NPC";
						quest_target_id = quest.data.ElementAt(0).ElementAt(1);
					}
					break;
				case 1:
				case 3:
				case 5:
				case 6:
				case 17:
					if (quest.data.Count == 1 && GameBoss.isBoss(quest.data.ElementAt(0).ElementAt(0)))
					{
						mobs_to_hunt.Add(quest.data.ElementAt(0).ElementAt(0));
						quest_target_type = "Raid";
						quest_target_id = mobs_to_hunt.First();
						break;
					}
					quest_target_type = "Mob";
					foreach (List<int> datum in quest.data)
					{
						if (followed_quest != null && num <= followed_quest.QuestProgress.Count - 1)
						{
							if (followed_quest.QuestProgress[num].actualCount == followed_quest.QuestProgress[num].maxCount)
							{
								num++;
								continue;
							}
							mobs_to_hunt.Add(datum.ElementAt(0));
							num++;
						}
					}
					quest_target_id = ((mobs_to_hunt.Count > 0) ? mobs_to_hunt.First() : (-1));
					break;
				case 19:
					quest_target_type = "Map";
					target_location.X = quest.data.ElementAt(0).ElementAt(1);
					target_location.Y = quest.data.ElementAt(0).ElementAt(2);
					quest_target_id = quest.data.ElementAt(0).ElementAt(0);
					break;
				case 26:
				case 31:
					return new List<int>();
				case 25:
					if (followed_quest != null && followed_quest.QuestProgress.All(((int actualCount, int maxCount) x) => x.actualCount == x.maxCount))
					{
						quest_target_type = "NPC";
						quest_target_id = quest.data.ElementAt(0).ElementAt(2);
					}
					else
					{
						quest_target_type = "Raid";
						mobs_to_hunt.Add(RaidID.GetBossID(quest.data.ElementAt(0).ElementAt(0)));
						quest_target_id = mobs_to_hunt.First();
					}
					break;
				}
				if (quest_target_id != -1 && !(quest_target_type == ""))
				{
					NostaleCharacterInfo? mapper = GUI.Mapper;
					if (mapper != null)
					{
						_ = mapper.real_map_id;
						if (0 == 0)
						{
							List<int> list = NAvigator.FindShortestPath(GUI.Mapper.real_map_id, quest_target_id, quest_target_type);
							if (list.Count == 0 && quest.questType == 1)
							{
								quest_target_type = "Map";
								target_location.X = quest.target.ElementAt(0);
								target_location.Y = quest.target.ElementAt(1);
								quest_target_id = quest.target.ElementAt(2);
								list = NAvigator.FindShortestPath(GUI.Mapper.real_map_id, quest_target_id, quest_target_type);
							}
							return list;
						}
					}
				}
				return new List<int>();
			}

			public static void NavigateToNonQuest()
			{
				ShowQuestSearchInstanceMap = false;
				UnselectQuest();
				quest_target_id = navigating_instance_map_id;
				quest_target_type = "Map";
				if (GUI.Mapper == null)
				{
					return;
				}
				path_to_quest_target = NAvigator.FindShortestPath(GUI.Mapper.real_map_id, quest_target_id, quest_target_type);
				if (path_to_quest_target.Count == 1)
				{
					quest_target_id = navigating_instance_id;
					quest_target_type = navigating_instance_type;
					if (quest_search_type == "Mob")
					{
						mobs_to_hunt.Clear();
						mobs_to_hunt.Add(quest_target_id);
					}
				}
				GUI.form.UpdateQuestPath(path_to_quest_target);
			}
		}
