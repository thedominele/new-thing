// Decompiled with JetBrains decompiler
// Type: NosAssistant2.Helpers.Analytics
// Assembly: NosAssistant2, Version=1.106.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5A569BAC-7221-4ADB-8020-B7EF6440456E
// Assembly location: E:\Neuer Ordner (9)\NosAssistant2.dll

using NosAssistant2.Dtos;
using NosAssistant2.Dtos.Input;
using NosAssistant2.Dtos.Output;
using NosAssistant2.EnumsID;
using NosAssistant2.GameData;
using NosAssistant2.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

#nullable enable
namespace NosAssistant2.Helpers
{
    public static class Analytics
    {
        public static List<MarathonData> marathons = new List<MarathonData>();
        public static List<MarathonData> marathons_org = new List<MarathonData>();
        public static string marathons_filter = "";
        public static List<RaidData> current_marathon = new List<RaidData>();
        public static List<PlayerFullRankingInfo> ranking_data_average_damage = new List<PlayerFullRankingInfo>();
        public static List<RankingMaxHit> ranking_data_max_hits = new List<RankingMaxHit>();
        public static List<RankingBestTime> ranking_data_best_times = new List<RankingBestTime>();
        public static List<RankingRaidsDone> ranking_data_raids_done = new List<RankingRaidsDone>();
        public static Dictionary<int, List<MaxHitData>> family_max_hits_all_raids = new Dictionary<int, List<MaxHitData>>();
        public static string SearchServer = "";
        public static string SearchNickname = "";
        public static List<PlayerItem> searcherd_player_items_with_shells = new List<PlayerItem>();
        public static int shown_shell_type = 1;
        public static bool show_item_rune = false;
        public static string ranking_mode = "Global";
        public static string real_ranking_mode = "Global";
        public static string ranking_data_mode = "average_damage";
        public static int ranking_raid_type_filter = 0;
        public static PictureBox? ranking_raid_type_filter_icon = (PictureBox)null;
        public static List<int> ranking_sps_filter = new List<int>();
        public static List<PictureBox> ranking_sps_filter_icons = new List<PictureBox>();
        public static List<RaidRankingInfo> ranking_avg_stddev_values = new List<RaidRankingInfo>();
        public static Dictionary<(int, int, int), (int, DateTime)> player_ranks_assigment = new Dictionary<(int, int, int), (int, DateTime)>();
        public static List<BarStatusDto> raids_bar_status_data = new List<BarStatusDto>();
        public static GamePlayer self = new GamePlayer();
        public static List<string> sent_rabbit_unique_ids = new List<string>();
        public static int players_raid_statistics_raid_type = -1;
        public static PictureBox? players_raid_statistics_raid_type_picture_box = (PictureBox)null;
        public static PlayerInfoData? current_player_data = (PlayerInfoData)null;

        public static void Clear()
        {
            Analytics.marathons_org.Clear();
            Analytics.marathons.Clear();
            Analytics.current_marathon.Clear();
            Analytics.marathons_filter = "";
            Analytics.family_max_hits_all_raids.Clear();
            Analytics.shown_shell_type = 1;
            Analytics.show_item_rune = false;
            Analytics.searcherd_player_items_with_shells.Clear();
            GUI.current_marathon_page = 1;
            GUI.family_records_page = 1;
            GUI.raids_history_page = 1;
            GUI.ranking_page = 1;
            Analytics.ClearRankingFilters();
            Analytics.ranking_mode = "Global";
            Analytics.ranking_data_mode = "average_damage";
            Analytics.ranking_raid_type_filter = 0;
            Analytics.ranking_sps_filter_icons.Clear();
            Analytics.ranking_avg_stddev_values.Clear();
            Analytics.ranking_data_average_damage.Clear();
            Analytics.ranking_data_max_hits.Clear();
            Analytics.ranking_raid_type_filter_icon = (PictureBox)null;
            GUI.ClearRankingItems();
            Analytics.SearchNickname = GUI.Mapper?.nickname ?? "";
            Analytics.SearchServer = GUI.Mapper?.server ?? "";
            Analytics.players_raid_statistics_raid_type_picture_box = (PictureBox)null;
            Analytics.players_raid_statistics_raid_type = -1;
        }

        public static async Task GetMarathons()
        {
            PlayerDataDto dto = new PlayerDataDto();
            dto.server_id = NostaleServers.GetServerIdFromName(GUI.Mapper?.server ?? "");
            NostaleCharacterInfo mapper = GUI.Mapper;
            dto.character_id = mapper != null ? mapper.character_id : 0;
            Analytics.marathons_org = await NAHttpClient.FetchMarathons(dto);
            Analytics.marathons = Analytics.marathons_org;
        }

        public static async Task GetRaidsInMarathons(string m_id)
        {
            RaidsInMarathonDto dto = new RaidsInMarathonDto();
            dto.marathon_id = m_id;
            NostaleCharacterInfo mapper = GUI.Mapper;
            dto.character_id = mapper != null ? mapper.character_id : 0;
            Analytics.current_marathon = await NAHttpClient.FetchRaidsInMarathon(dto);
        }

        public static async Task GetFamilyMaxHitsAllRaids()
        {
            List<MaxHitData> maxHitDataList = await NAHttpClient.FetchFamilyAllRaidsMaxHits(new FamilyDataDto()
            {
                server_id = NostaleServers.GetServerIdFromName(GUI.Mapper?.server ?? ""),
                family = GUI.Mapper?.family_name ?? "--"
            });
            Analytics.family_max_hits_all_raids.Clear();
            foreach (MaxHitData maxHitData in maxHitDataList)
            {
                if (!Analytics.family_max_hits_all_raids.ContainsKey(maxHitData.boss_id))
                    Analytics.family_max_hits_all_raids[maxHitData.boss_id] = new List<MaxHitData>();
                Analytics.family_max_hits_all_raids[maxHitData.boss_id].Add(maxHitData);
            }
        }

        public static async Task<List<MaxHitData>> GetFamilyTop10MaxHits(int b_id)
        {
            return await NAHttpClient.FetchFamilyTop10MaxHits(new FamilyDataSpecificRaidDto()
            {
                boss_id = b_id,
                server_id = NostaleServers.GetServerIdFromName(GUI.Mapper?.server ?? ""),
                family = GUI.Mapper?.family_name ?? "--"
            });
        }

        public static async Task<List<RaidDurationData>> GetFamilyTop10RaidsDurations(int b_id)
        {
            return await NAHttpClient.FetchFamilyTop10RaidTimes(new FamilyDataSpecificRaidDto()
            {
                boss_id = b_id,
                server_id = NostaleServers.GetServerIdFromName(GUI.Mapper?.server ?? ""),
                family = GUI.Mapper?.family_name ?? "--"
            });
        }

        public static void Inpersonate(int character_id, string family, int server)
        {
            GUI.Mapper.family_name = family;
            GUI.Mapper.character_id = character_id;
            GUI.Mapper.server = NostaleServers.GetServerNameFromId(server);
        }

        public static void SendMapPlayersInfo()
        {
            List<PlayerFullInfoDto> playerFullInfoDtoList = new List<PlayerFullInfoDto>();
            int serverIdFromName = NostaleServers.GetServerIdFromName(GUI.Mapper?.server ?? "");
            if (serverIdFromName == 0)
                return;
            foreach (GamePlayer player in GUI.players)
            {
                PlayerFullInfoDto playerFullInfoDto = Analytics.PackPlayerData(player, serverIdFromName);
                if (playerFullInfoDto != null)
                    playerFullInfoDtoList.Add(playerFullInfoDto);
            }
            if (GUI.Mapper == null)
                return;
            PlayerFullInfoOnMapDto dto = new PlayerFullInfoOnMapDto();
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 3);
            interpolatedStringHandler.AppendLiteral("SendPlayersOnMapEvent_");
            interpolatedStringHandler.AppendFormatted<int>(serverIdFromName);
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted<int?>(GUI.Mapper.channel);
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted<int>(GUI.Mapper.map_id);
            dto.unique_id = interpolatedStringHandler.ToStringAndClear();
            dto.map_id = GUI.Mapper.map_id;
            dto.server_id = serverIdFromName;
            dto.players = playerFullInfoDtoList;
            RabbitEventHandler.SendPlayersOnMapData(dto);
        }

        public static void SendSinglePlayerData(GamePlayer player)
        {
            int serverIdFromName = NostaleServers.GetServerIdFromName(GUI.Mapper?.server ?? "");
            if (serverIdFromName == 0 || GUI.Mapper == null)
                return;
            PlayerFullInfoDto dto = Analytics.PackPlayerData(player, serverIdFromName);
            if (dto == null)
                return;
            RabbitEventHandler.SendPlayerData(dto);
        }

        public static PlayerFullInfoDto? PackPlayerData(GamePlayer player, int serv_id)
        {
            if (player.lvl <= 85 || player.clvl == 0)
                return (PlayerFullInfoDto)null;
            List<string> list = ((IEnumerable<string>)player.items.Split(".")).ToList<string>();
            string str1 = list.ElementAt<string>(1);
            string str2 = list.ElementAt<string>(2);
            list.ElementAt<string>(3);
            PlayerFullInfoDto playerFullInfoDto1 = new PlayerFullInfoDto();
            playerFullInfoDto1.server_id = serv_id;
            playerFullInfoDto1.character_id = player.character_id;
            playerFullInfoDto1.nickname = player.nickname;
            playerFullInfoDto1.family = player.family;
            playerFullInfoDto1.lvl = player.lvl;
            playerFullInfoDto1.clvl = player.clvl;
            playerFullInfoDto1.class_id = player.classID;
            playerFullInfoDto1.sex = player.sex;
            playerFullInfoDto1.vanity = player.items;
            playerFullInfoDto1.weapon_type = -1;
            playerFullInfoDto1.reputation = player.reputation;
            playerFullInfoDto1.title = player.title;
            int? nullable1 = player.real_title;
            int? nullable2;
            if (nullable1.HasValue)
            {
                nullable2 = player.real_title;
            }
            else
            {
                nullable1 = new int?();
                nullable2 = nullable1;
            }
            playerFullInfoDto1.real_title = nullable2;
            PlayerFullInfoDto playerFullInfoDto2 = playerFullInfoDto1;
            if (SPID.IsCombatSP(player.spID))
            {
                if (player.spID == 30)
                    player.spID = 29;
                if (player.spID == 43)
                    player.spID = 42;
                playerFullInfoDto2.sp_id = new int?(player.spID);
                playerFullInfoDto2.sp_upgrade = new int?(player.sp_upgrade);
                playerFullInfoDto2.sp_wings_id = new int?(player.sp_wings_id);
            }
            if (str2 != "-1")
            {
                playerFullInfoDto2.weapon_type = 1;
                if (player.weapon_upgrade.Length == 1)
                {
                    playerFullInfoDto2.weapon_upgrade = new int?(0);
                    playerFullInfoDto2.weapon_rarity = new int?(Convert.ToInt32(player.weapon_upgrade));
                }
                else if ((player.weapon_upgrade.Contains("-1") || player.weapon_upgrade.Contains("-2")) && player.weapon_upgrade.Length == 2)
                {
                    playerFullInfoDto2.weapon_upgrade = new int?(0);
                    playerFullInfoDto2.weapon_rarity = new int?(Convert.ToInt32(player.weapon_upgrade));
                }
                else if ((player.weapon_upgrade.Contains("-1") || player.weapon_upgrade.Contains("-2")) && player.weapon_upgrade.Length == 3)
                {
                    playerFullInfoDto2.weapon_upgrade = new int?(Convert.ToInt32(player.weapon_upgrade.Substring(0, 1)));
                    playerFullInfoDto2.weapon_rarity = new int?(Convert.ToInt32(player.weapon_upgrade.Substring(1, 2)));
                }
                else if (!player.weapon_upgrade.Contains("-") && player.weapon_upgrade.Length >= 3)
                {
                    playerFullInfoDto2.weapon_upgrade = new int?(Convert.ToInt32(player.weapon_upgrade.Substring(0, 2)));
                    playerFullInfoDto2.weapon_rarity = new int?(Convert.ToInt32(player.weapon_upgrade.Substring(2, player.weapon_upgrade.Length - 2)));
                }
                else
                {
                    playerFullInfoDto2.weapon_upgrade = new int?(Convert.ToInt32(player.weapon_upgrade.Substring(0, 1)));
                    playerFullInfoDto2.weapon_rarity = new int?(Convert.ToInt32(player.weapon_upgrade.Substring(1, 1)));
                }
            }
            if (str1 != "-1")
            {
                if (player.armor_upgrade.Length == 1)
                {
                    playerFullInfoDto2.armor_upgrade = new int?(0);
                    playerFullInfoDto2.armor_rarity = new int?(Convert.ToInt32(player.armor_upgrade));
                }
                else if ((player.armor_upgrade.Contains("-1") || player.armor_upgrade.Contains("-2")) && player.armor_upgrade.Length == 2)
                {
                    playerFullInfoDto2.armor_upgrade = new int?(0);
                    playerFullInfoDto2.armor_rarity = new int?(Convert.ToInt32(player.armor_upgrade));
                }
                else if ((player.armor_upgrade.Contains("-1") || player.armor_upgrade.Contains("-2")) && player.armor_upgrade.Length == 3)
                {
                    playerFullInfoDto2.armor_upgrade = new int?(Convert.ToInt32(player.armor_upgrade.Substring(0, 1)));
                    playerFullInfoDto2.armor_rarity = new int?(Convert.ToInt32(player.armor_upgrade.Substring(1, 2)));
                }
                else if (!player.armor_upgrade.Contains("-") && player.armor_upgrade.Length >= 3)
                {
                    playerFullInfoDto2.armor_upgrade = new int?(Convert.ToInt32(player.armor_upgrade.Substring(0, 2)));
                    playerFullInfoDto2.armor_rarity = new int?(Convert.ToInt32(player.armor_upgrade.Substring(2, player.armor_upgrade.Length - 2)));
                }
                else
                {
                    playerFullInfoDto2.armor_upgrade = new int?(Convert.ToInt32(player.armor_upgrade.Substring(0, 1)));
                    playerFullInfoDto2.armor_rarity = new int?(Convert.ToInt32(player.armor_upgrade.Substring(1, 1)));
                }
            }
            if (player.family != "-")
            {
                playerFullInfoDto2.family = player.family;
                playerFullInfoDto2.family_id = new int?(player.family_id);
                playerFullInfoDto2.family_lvl = new int?(player.family_lvl);
                playerFullInfoDto2.family_role_id = new int?(player.family_role_id);
            }
            if (player.fairy_element_id != 0)
            {
                playerFullInfoDto2.fairy_element_id = new int?(player.fairy_element_id);
                playerFullInfoDto2.fairy_id = new int?(GameFairy.IsFairyBoosted(player.fairy_id) ? player.fairy_id - 5 : player.fairy_id);
            }
            PlayerFullInfoDto playerFullInfoDto3 = playerFullInfoDto2;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 3);
            interpolatedStringHandler.AppendLiteral("SendPlayerEvent_");
            interpolatedStringHandler.AppendFormatted<int>(playerFullInfoDto2.server_id);
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted<int>(playerFullInfoDto2.character_id);
            interpolatedStringHandler.AppendLiteral("_");
            ref DefaultInterpolatedStringHandler local = ref interpolatedStringHandler;
            nullable1 = playerFullInfoDto2.sp_id;
            int valueOrDefault = nullable1.GetValueOrDefault();
            local.AppendFormatted<int>(valueOrDefault);
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            playerFullInfoDto3.unique_id = stringAndClear;
            return playerFullInfoDto2;
        }

        public static RaidData CalculateTotal()
        {
            RaidData total = new RaidData();
            foreach (RaidData raidData in Analytics.current_marathon)
            {
                foreach (RaidPlayer player1 in raidData.players)
                {
                    RaidPlayer player = player1;
                    RaidPlayer raidPlayer = total.players.Find((Predicate<RaidPlayer>)(x => x.character_id == player.character_id));
                    if (raidPlayer == null)
                    {
                        total.players.Add(new RaidPlayer(player));
                    }
                    else
                    {
                        raidPlayer.damage += player.damage;
                        raidPlayer.damage_miniboss += player.damage_miniboss;
                        raidPlayer.damage_onyx += player.damage_onyx;
                        raidPlayer.pets += player.pets;
                        raidPlayer.gold += player.gold;
                        raidPlayer.hit += player.hit;
                        raidPlayer.miss += player.miss;
                        raidPlayer.crit += player.crit;
                        raidPlayer.bon += player.bon;
                        raidPlayer.boncrit += player.boncrit;
                        raidPlayer.debuffs += player.debuffs;
                        raidPlayer.dead += player.dead;
                        raidPlayer.player_hits += player.player_hits;
                        raidPlayer.player_kills += player.player_kills;
                        raidPlayer.all_damage += player.all_damage;
                        raidPlayer.mob_damage += player.mob_damage;
                        raidPlayer.all_hits += player.all_hits;
                        raidPlayer.all_miss += player.all_miss;
                        if (player.max_hit > raidPlayer.max_hit)
                        {
                            raidPlayer.max_hit = player.max_hit;
                            raidPlayer.max_hit_skill_id = player.max_hit_skill_id;
                        }
                    }
                }
            }
            foreach (RaidPlayer player in total.players)
                player.average = player.hit == 0 ? 0L : player.damage / (long)player.hit;
            total.players = total.players.OrderBy<RaidPlayer, long>((Func<RaidPlayer, long>)(x => x.damage)).ToList<RaidPlayer>();
            return total;
        }

        public static void UpdatePlayersSPWings(int _character_id, int real_wings_id)
        {
            GamePlayer gamePlayer = GUI.players.Find((Predicate<GamePlayer>)(x => x.character_id == _character_id));
            if (gamePlayer == null || GUI.Mapper == null)
                return;
            int serverIdFromName = NostaleServers.GetServerIdFromName(GUI.Mapper.server);
            if (gamePlayer.spID == 43)
                gamePlayer.spID = 42;
            if (gamePlayer.spID == 30)
                gamePlayer.spID = 29;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 3);
            interpolatedStringHandler.AppendLiteral("UpdatePlayerSPEvent_");
            interpolatedStringHandler.AppendFormatted<int>(serverIdFromName);
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted<int>(_character_id);
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted<int>(gamePlayer.spID);
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            if (Analytics.sent_rabbit_unique_ids.Contains(stringAndClear) || !SPID.IsCombatSP(gamePlayer.spID))
                return;
            PlayerSPDto playerSpDto = new PlayerSPDto()
            {
                sp_id = gamePlayer.spID,
                sp_upgrade = gamePlayer.sp_upgrade,
                sp_wings = gamePlayer.sp_wings_id,
                real_sp_wings = real_wings_id
            };
            RabbitEventHandler.SendSPDetails(new UpdatePlayerSPDto()
            {
                sp_details = playerSpDto,
                unique_id = stringAndClear,
                server_id = serverIdFromName,
                character_id = _character_id
            });
            Analytics.sent_rabbit_unique_ids.Add(stringAndClear);
        }

        public static void UpdateSelfSPWings(NostaleCharacterInfo character, int real_wings_id)
        {
            int serverIdFromName = NostaleServers.GetServerIdFromName(character.server);
            int id = character.SPCard.ID;
            if (id == 43)
                id = 42;
            if (id == 30)
                id = 29;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 3);
            interpolatedStringHandler.AppendLiteral("UpdatePlayerSPEvent_");
            interpolatedStringHandler.AppendFormatted<int>(serverIdFromName);
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted<int>(character.character_id);
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted<int>(character.SPCard.ID);
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            if (Analytics.sent_rabbit_unique_ids.Contains(stringAndClear) || !SPID.IsCombatSP(id) || character.SPCard.SPUpgrade <= 0)
                return;
            PlayerSPDto playerSpDto = new PlayerSPDto()
            {
                sp_id = id,
                sp_upgrade = character.SPCard.SPUpgrade,
                sp_wings = character.SPCard.WingsID,
                real_sp_wings = real_wings_id
            };
            RabbitEventHandler.SendSPDetails(new UpdatePlayerSPDto()
            {
                sp_details = playerSpDto,
                unique_id = stringAndClear,
                server_id = serverIdFromName,
                character_id = character.character_id
            });
            Analytics.sent_rabbit_unique_ids.Add(stringAndClear);
        }

        public static void SendSPCard(
          string packet_content,
          NostaleCharacterInfo character,
          int owner_id)
        {
            List<string> list = ((IEnumerable<string>)packet_content.Split(" ")).ToList<string>();
            if (list.Count < 48)
                return;
            int num1 = Convert.ToInt32(list.ElementAt<string>(3));
            if (!SPID.IsCombatSP(num1))
                return;
            if (num1 == 43)
                num1 = 42;
            if (num1 == 30)
                num1 = 29;
            int int32_1 = Convert.ToInt32(list.ElementAt<string>(4));
            Convert.ToInt32(list.ElementAt<string>(5));
            Convert.ToInt32(list.ElementAt<string>(15));
            Convert.ToInt32(list.ElementAt<string>(16));
            Convert.ToInt32(list.ElementAt<string>(17));
            Convert.ToInt32(list.ElementAt<string>(18));
            list.ElementAt<string>(21);
            long int64 = Convert.ToInt64(list.ElementAt<string>(22));
            Convert.ToInt32(list.ElementAt<string>(23));
            int int32_2 = Convert.ToInt32(list.ElementAt<string>(24));
            int int32_3 = Convert.ToInt32(list.ElementAt<string>(25));
            int int32_4 = Convert.ToInt32(list.ElementAt<string>(26));
            int int32_5 = Convert.ToInt32(list.ElementAt<string>(27));
            int int32_6 = Convert.ToInt32(list.ElementAt<string>(28));
            int int32_7 = Convert.ToInt32(list.ElementAt<string>(32));
            int int32_8 = Convert.ToInt32(list.ElementAt<string>(33));
            int int32_9 = Convert.ToInt32(list.ElementAt<string>(34));
            int int32_10 = Convert.ToInt32(list.ElementAt<string>(35));
            int int32_11 = Convert.ToInt32(list.ElementAt<string>(36));
            int int32_12 = Convert.ToInt32(list.ElementAt<string>(37));
            int int32_13 = Convert.ToInt32(list.ElementAt<string>(38));
            int int32_14 = Convert.ToInt32(list.ElementAt<string>(39));
            int int32_15 = Convert.ToInt32(list.ElementAt<string>(40));
            int int32_16 = Convert.ToInt32(list.ElementAt<string>(41));
            int int32_17 = Convert.ToInt32(list.ElementAt<string>(42));
            int int32_18 = Convert.ToInt32(list.ElementAt<string>(43));
            int int32_19 = Convert.ToInt32(list.ElementAt<string>(44));
            int int32_20 = Convert.ToInt32(list.ElementAt<string>(46));
            int int32_21 = Convert.ToInt32(list.ElementAt<string>(47));
            int key = int32_21 == 0 ? int32_20 : int32_21;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 4);
            interpolatedStringHandler.AppendFormatted<int>(int32_2);
            interpolatedStringHandler.AppendLiteral(".");
            interpolatedStringHandler.AppendFormatted<int>(int32_3);
            interpolatedStringHandler.AppendLiteral(".");
            interpolatedStringHandler.AppendFormatted<int>(int32_4);
            interpolatedStringHandler.AppendLiteral(".");
            interpolatedStringHandler.AppendFormatted<int>(int32_5);
            string stringAndClear1 = interpolatedStringHandler.ToStringAndClear();
            interpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 4);
            interpolatedStringHandler.AppendFormatted<int>(int32_7);
            interpolatedStringHandler.AppendLiteral(".");
            interpolatedStringHandler.AppendFormatted<int>(int32_8);
            interpolatedStringHandler.AppendLiteral(".");
            interpolatedStringHandler.AppendFormatted<int>(int32_9);
            interpolatedStringHandler.AppendLiteral(".");
            interpolatedStringHandler.AppendFormatted<int>(int32_10);
            string stringAndClear2 = interpolatedStringHandler.ToStringAndClear();
            interpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 8);
            interpolatedStringHandler.AppendFormatted<int>(int32_12);
            interpolatedStringHandler.AppendLiteral(".");
            interpolatedStringHandler.AppendFormatted<int>(int32_13);
            interpolatedStringHandler.AppendLiteral(".");
            interpolatedStringHandler.AppendFormatted<int>(int32_14);
            interpolatedStringHandler.AppendLiteral(".");
            interpolatedStringHandler.AppendFormatted<int>(int32_15);
            interpolatedStringHandler.AppendLiteral(".");
            interpolatedStringHandler.AppendFormatted<int>(int32_16);
            interpolatedStringHandler.AppendLiteral(".");
            interpolatedStringHandler.AppendFormatted<int>(int32_17);
            interpolatedStringHandler.AppendLiteral(".");
            interpolatedStringHandler.AppendFormatted<int>(int32_18);
            interpolatedStringHandler.AppendLiteral(".");
            interpolatedStringHandler.AppendFormatted<int>(int32_19);
            string stringAndClear3 = interpolatedStringHandler.ToStringAndClear();
            int serverIdFromName = NostaleServers.GetServerIdFromName(character.server);
            int num2 = SPID.SPIDToClass[num1];
            if (owner_id == character.character_id && num2 != character.class_id)
                return;
            interpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 3);
            interpolatedStringHandler.AppendLiteral("UpdatePlayerSPDetailsEvent_");
            interpolatedStringHandler.AppendFormatted<int>(serverIdFromName);
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted<int>(owner_id);
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted<long>(int64);
            string stringAndClear4 = interpolatedStringHandler.ToStringAndClear();
            if (Analytics.sent_rabbit_unique_ids.Contains(stringAndClear4))
                return;
            Analytics.sent_rabbit_unique_ids.Add(stringAndClear4);
            if (serverIdFromName == 0 || int64 == 0L)
                return;
            RabbitEventHandler.UpdateSPDetails(new PlayerSPDetails()
            {
                sp_id = num1,
                perfection = int32_11,
                job = int32_1,
                sp_upgrade = int32_6,
                build = stringAndClear1,
                sl = stringAndClear2,
                pp = stringAndClear3,
                real_sp_wings = new int?(SPID.WingsItemIDToWingsID[int32_20]),
                sp_wings = new int?(SPID.WingsItemIDToWingsID[key]),
                owner_id = owner_id,
                server_id = serverIdFromName,
                server_sp_id = int64,
                unique_id = stringAndClear4,
                card_class_id = num2
            });
        }

        public static void HandleTattoosBuff(List<string> buff_splitted, int character_id)
        {
            NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((Predicate<NostaleCharacterInfo>)(x => x.character_id == character_id));
            if (nostaleCharacterInfo == null)
                return;
            if (buff_splitted.Count == 3)
                buff_splitted = buff_splitted.Skip<string>(1).ToList<string>();
            if (buff_splitted.Count != 2 || buff_splitted.ElementAt<string>(0).Length != 4)
                return;
            string key = buff_splitted.ElementAt<string>(0).Substring(0, 3);
            int int32 = Convert.ToInt32(buff_splitted.ElementAt<string>(0).Substring(3, 1));
            if (!EffectsID.BuffToTattoo.ContainsKey(key) || GUI.Mapper == null)
                return;
            int serverIdFromName = NostaleServers.GetServerIdFromName(nostaleCharacterInfo.server);
            if (serverIdFromName == 0)
                return;
            int num = EffectsID.BuffToTattoo[key];
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 3);
            interpolatedStringHandler.AppendLiteral("SendTattoosEvent_");
            interpolatedStringHandler.AppendFormatted<int>(serverIdFromName);
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted<int>(character_id);
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted<int>(num);
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            if (Analytics.sent_rabbit_unique_ids.Contains(stringAndClear))
                return;
            RabbitEventHandler.SendTattoos(new PlayerTattooDto()
            {
                unique_id = stringAndClear,
                tattoo_id = num,
                upgrade = int32,
                character_id = character_id,
                server_id = serverIdFromName
            });
            Analytics.sent_rabbit_unique_ids.Add(stringAndClear);
        }

        public static void HandleFairyDetails(
          string fairy_data_string,
          NostaleCharacterInfo character,
          int owner_id)
        {
            List<string> list = ((IEnumerable<string>)fairy_data_string.Split(" ")).ToList<string>();
            int int32_1 = Convert.ToInt32(list.ElementAt<string>(2));
            if (!GameFairy.ItemIDtoFairyID.Keys.Contains(int32_1))
                return;
            int int32_2 = Convert.ToInt32(list.ElementAt<string>(3));
            int int32_3 = Convert.ToInt32(list.ElementAt<string>(4));
            int int32_4 = Convert.ToInt32(list.ElementAt<string>(11));
            if (int32_4 != 0)
                owner_id = int32_4;
            if (GameFairy.isDrone(int32_1) && int32_4 == 0)
                return;
            int int32_5 = Convert.ToInt32(list.ElementAt<string>(14));
            string str = (string)null;
            if (int32_5 != 0)
                str = string.Join(" ", list.TakeLast<string>(int32_5)).Trim();
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 3);
            interpolatedStringHandler.AppendLiteral("SendFairyDetailsEvent_");
            interpolatedStringHandler.AppendFormatted<int>(NostaleServers.GetServerIdFromName(character.server));
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted<int>(owner_id);
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted<int>(int32_1);
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            if (Analytics.sent_rabbit_unique_ids.Contains(stringAndClear))
                return;
            RabbitEventHandler.SendFairyDetails(new PlayerFairyDto()
            {
                unique_id = stringAndClear,
                fairy_id = GameFairy.ItemIDtoFairyID[int32_1],
                upgrade = new int?(int32_5),
                percent = new int?(int32_3),
                element = int32_2,
                owner_id = owner_id,
                effects = str,
                server_id = NostaleServers.GetServerIdFromName(character.server)
            });
            Analytics.sent_rabbit_unique_ids.Add(stringAndClear);
        }

        public static int AssignPlayersRank(int player_average, int server_average, int stddev)
        {
            double num1 = 0.25;
            double num2 = 1.0;
            double num3 = 1.5;
            if ((double)player_average < (double)server_average - num2 * (double)stddev)
                return 1;
            if ((double)player_average < (double)server_average - num1 * (double)stddev)
                return 2;
            if ((double)player_average < (double)server_average + num1 * (double)stddev)
                return 3;
            if ((double)player_average < (double)server_average + num3 * (double)stddev)
                return 4;
            return (double)player_average >= (double)server_average + num3 * (double)stddev ? 5 : 0;
        }

        public static async Task<int> GetRank(int character_id, int sp_id, int boss_id)
        {
            int server_id = NostaleServers.GetServerIdFromName(GUI.Mapper?.server ?? "");
            if (server_id == 0)
                return 0;
            RaidRankingInfo ranking_info = await NAHttpClient.GetRaidRankingInfo(new GetRaidRankingInfoDto()
            {
                server_id = server_id,
                boss_id = boss_id,
                sp_id = sp_id
            });
            if (ranking_info == null)
                return 0;
            PlayerRaidRankingInfo playerRaidRankingInfo = await NAHttpClient.GetPlayerRaidRankingInfo(new GetPlayerRaidRankingInfoDto()
            {
                server_id = server_id,
                boss_id = boss_id,
                sp_id = sp_id,
                character_id = character_id
            });
            return playerRaidRankingInfo != null ? Analytics.AssignPlayersRank(playerRaidRankingInfo.player_avg_damage, ranking_info.mean_damage, ranking_info.stddev_damage) : 0;
        }

        public static Image? GetRankIcon(int rank)
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler.AppendLiteral("images/ranks/");
            interpolatedStringHandler.AppendFormatted<int>(rank);
            interpolatedStringHandler.AppendLiteral(".png");
            if (!File.Exists(interpolatedStringHandler.ToStringAndClear()))
                return !File.Exists("images/ranks/0.png") ? (Image)null : Image.FromFile("images/ranks/0.png");
            interpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler.AppendLiteral("images/ranks/");
            interpolatedStringHandler.AppendFormatted<int>(rank);
            interpolatedStringHandler.AppendLiteral(".png");
            return Image.FromFile(interpolatedStringHandler.ToStringAndClear());
        }

        public static void UpdateRankingRaidTypeFilter(int boss_id, PictureBox boss_filter_icon)
        {
            if (Analytics.ranking_raid_type_filter_icon != null && Analytics.ranking_raid_type_filter_icon == boss_filter_icon)
            {
                Analytics.ranking_raid_type_filter_icon.BackColor = NAStyles.MainThemeLighter;
                Analytics.ranking_raid_type_filter_icon = (PictureBox)null;
                Analytics.ranking_raid_type_filter = 0;
                GUI.UpdateRankingModeLabel();
            }
            else
            {
                if (Analytics.ranking_raid_type_filter_icon != null)
                    Analytics.ranking_raid_type_filter_icon.BackColor = NAStyles.MainThemeLighter;
                Analytics.ranking_raid_type_filter = boss_id;
                Analytics.ranking_raid_type_filter_icon = boss_filter_icon;
                Analytics.ranking_raid_type_filter_icon.BackColor = NAStyles.NotActiveCharColor;
                GUI.UpdateRankingModeLabel();
            }
        }

        public static void UpdateRankingSPFilter(int sp_id, PictureBox sp_filter_icon)
        {
            sp_filter_icon.BackColor = Analytics.ranking_sps_filter.Contains(sp_id) ? NAStyles.MainThemeLighter : NAStyles.NotActiveCharColor;
            if (Analytics.ranking_sps_filter.Contains(sp_id))
            {
                Analytics.ranking_sps_filter.Remove(sp_id);
                Analytics.ranking_sps_filter_icons.Remove(sp_filter_icon);
            }
            else
            {
                Analytics.ranking_sps_filter.Add(sp_id);
                Analytics.ranking_sps_filter_icons.Add(sp_filter_icon);
            }
        }

        public static void ClearRankingFilters()
        {
            if (Analytics.ranking_raid_type_filter_icon != null)
                Analytics.ranking_raid_type_filter_icon.BackColor = NAStyles.MainThemeLighter;
            Analytics.ranking_raid_type_filter_icon = (PictureBox)null;
            Analytics.ranking_raid_type_filter = 0;
            foreach (Control rankingSpsFilterIcon in Analytics.ranking_sps_filter_icons)
                rankingSpsFilterIcon.BackColor = NAStyles.MainThemeLighter;
            Analytics.ranking_sps_filter.Clear();
            GUI.UpdateRankingModeLabel();
        }

        public static async Task<PlayerInfoData?> SearchForPlayer()
        {
            int server_id = NostaleServers.GetServerIdFromName(Analytics.SearchServer);
            string str = Analytics.SearchNickname;
            if (Analytics.SearchNickname.StartsWith("/") && Analytics.SearchNickname.Length >= 1)
                str = Analytics.SearchNickname.Substring(1);
            if (server_id == 0)
                return (PlayerInfoData)null;
            PlayerInfoData playerInfoData = await NAHttpClient.FetchPlayerData(new ServerNicknameDto()
            {
                server_id = server_id,
                nickname = str
            });
            if (playerInfoData == null && !Analytics.SearchNickname.StartsWith("/"))
            {
                GUI.ShowPopUp("No data about this player", true);
                return (PlayerInfoData)null;
            }
            if (playerInfoData == null)
            {
                playerInfoData = await NAHttpClient.FetchPlayerData(new ServerNicknameDto()
                {
                    server_id = server_id,
                    nickname = Analytics.SearchNickname
                });
                if (playerInfoData == null)
                {
                    GUI.ShowPopUp("No data about this player", true);
                    return (PlayerInfoData)null;
                }
            }
            return playerInfoData;
        }

        public static async Task RefreshBarStatusData()
        {
            int serverIdFromName = NostaleServers.GetServerIdFromName(GUI.Mapper?.server ?? "");
            if (serverIdFromName == 0)
                return;
            Analytics.raids_bar_status_data = await NAHttpClient.GetRaidBarsStatus(serverIdFromName);
            GUI.last_raids_bars_refresh = DateTime.UtcNow;
        }

        public static void ChangePlayerRaidStatisticsRaidType(int boss_id, PictureBox boss_icon)
        {
            if (boss_icon == Analytics.players_raid_statistics_raid_type_picture_box)
                return;
            if (Analytics.players_raid_statistics_raid_type_picture_box != null)
                Analytics.players_raid_statistics_raid_type_picture_box.BackColor = NAStyles.MainThemeDarker;
            Analytics.players_raid_statistics_raid_type_picture_box = boss_icon;
            boss_icon.BackColor = NAStyles.NotActiveCharColor;
            Analytics.players_raid_statistics_raid_type = boss_id;
            if (Analytics.current_player_data == null)
                return;
            GUI.form.SetPlayersRaidsStatisticsData(Analytics.current_player_data);
        }
    }
}
