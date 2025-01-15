using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NosAssistant2.Dtos;
using NosAssistant2.Dtos.Input;
using NosAssistant2.Dtos.Output;
using NosAssistant2.GameObjects;

namespace NosAssistant2.Helpers;

public static class NAHttpClient
{
	private static HttpClient _httpClient = new HttpClient();

	private static string _baseUrl = "https://nosassistant.pl/api";

	public static void SetHeader(string key, string value)
	{
		string value2 = Uri.EscapeDataString(value);
		if (_httpClient.DefaultRequestHeaders.Contains(key))
		{
			_httpClient.DefaultRequestHeaders.Remove(key);
		}
		_httpClient.DefaultRequestHeaders.Add(key, value2);
	}

	public static void SetHttpClient()
	{
		string official_cert = "3082010A028201010093A5A18BDA76C9300E980D1093551F0C0BE3283E1BE69B3878626ECAE38C6903ABADEF20877CA3FB59972149CEB7090E10C8FF6FE5197BF85B74195E30E9360B1D1D5A45D08BCCCF8F23FC2E3EDE7608BDC8F4EFB88A30FDF1700426F6551E6EA78EB79A539605870404A1EA0970FB34F087B248F347B5AE64838A435A1D222B1E4F9A7CFBA8F74D85A2C97EA85CC22116E5298988A2F582D4D08EF19B63285DC2B3D20D4E0E43E4BAD47848E91CCA9D72C48A8FD3057B87DB1E8FDE6A945DDAEC0B059439568FD89EC9D7A642916A52851630E972CEA74154D6884498CA69463628BCF44F2A30810C39C23143CE9E519F5D6BC00DD010E6E7D633265B161D050203010001";
		string thumbprint = "8F5D4893F6DDA40E6AC011F75D288F1CD5113DDF";
		_httpClient = new HttpClient(new HttpClientHandler
		{
			SslProtocols = (SslProtocols.Tls12 | SslProtocols.Tls13),
			ServerCertificateCustomValidationCallback = delegate(HttpRequestMessage message, X509Certificate2? cert, X509Chain? chain, SslPolicyErrors errors)
			{
				if (cert == null)
				{
					return false;
				}
				return cert.GetPublicKeyString() == official_cert && cert.Thumbprint == thumbprint;
			}
		})
		{
			Timeout = TimeSpan.FromSeconds(5.0)
		};
	}

	public static async Task<LicenseResponse> CheckLicense(LicenseNickname dto)
	{
		try
		{
			var temp = _httpClient.PostAsJsonAsync(_baseUrl + "/auth/check/license", dto);

            HttpResponseMessage result = temp.Result;
            result.EnsureSuccessStatusCode();
			return new()
			{
				valid = true,
				valid_until = DateTime.MaxValue
			};
        }
		catch (Exception)
		{
			return new();
        }
	}

	public static string CheckRole(Dtos.Input.LicenseNickname dto)
	{
		try
		{
			HttpResponseMessage result = _httpClient.PostAsJsonAsync(_baseUrl + "/auth/check/role", dto).Result;
			result.EnsureSuccessStatusCode();
			return result.Content.ReadAsStringAsync().Result;
		}
		catch (Exception)
		{
			return "";
		}
	}

	public static string UserProve(Dtos.Input.UserProve dto)
	{
		try
		{
			HttpResponseMessage result = _httpClient.PostAsJsonAsync(_baseUrl + "/users/prove", dto).Result;
			result.EnsureSuccessStatusCode();
			return result.Content.ReadAsStringAsync().Result;
		}
		catch (Exception)
		{
			return "";
		}
	}

	public static async Task<int> GetBestCandidate(Dtos.Input.BestCandidateDto dto)
	{
		if (dto.server_id == 0)
		{
			return 0;
		}
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/get_candidate", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<BestCandidateResponse>(await obj.Content.ReadAsStringAsync()).character_id;
		}
		catch (Exception)
		{
			return 0;
		}
	}

	public static async Task<bool> BossKilled(TempRaidInfoDto dto)
	{
		if (dto.server_id == 0)
		{
			return false;
		}
		try
		{
			(await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/boss_killed", dto)).EnsureSuccessStatusCode();
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static async Task<bool> CreateRaid(RaidData dto)
	{
		if (dto.server_id == 0)
		{
			return false;
		}
		try
		{
			(await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/create", dto)).EnsureSuccessStatusCode();
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static async Task<List<MarathonData>> FetchMarathons(PlayerDataDto dto)
	{
		if (dto.server_id == 0)
		{
			return new List<MarathonData>();
		}
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/user/marathons", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<MarathonData>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			GUI.ShowPopUp("Failed to fetch data from the server.");
			return new List<MarathonData>();
		}
	}

	public static async Task<List<RaidData>> FetchRaidsInMarathon(RaidsInMarathonDto dto)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/user/raids", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<RaidData>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			GUI.ShowPopUp("Failed to fetch data from the server.");
			return new List<RaidData>();
		}
	}

	public static async Task<List<MaxHitData>> FetchFamilyAllRaidsMaxHits(FamilyDataDto dto)
	{
		if (dto.server_id == 0)
		{
			return new List<MaxHitData>();
		}
		dto.family = ((dto.family == "-") ? "--" : dto.family);
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/family/tophits", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<MaxHitData>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			GUI.ShowPopUp("Failed to fetch data from the server.");
			return new List<MaxHitData>();
		}
	}

	public static async Task<List<MaxHitData>> FetchFamilyTop10MaxHits(FamilyDataSpecificRaidDto dto)
	{
		if (dto.server_id == 0)
		{
			return new List<MaxHitData>();
		}
		dto.family = ((dto.family == "-") ? "--" : dto.family);
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/family/top10hits ", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<MaxHitData>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			GUI.ShowPopUp("Failed to fetch data from the server.");
			return new List<MaxHitData>();
		}
	}

	public static async Task<List<RaidDurationData>> FetchFamilyTop10RaidTimes(FamilyDataSpecificRaidDto dto)
	{
		if (dto.server_id == 0)
		{
			return new List<RaidDurationData>();
		}
		dto.family = ((dto.family == "-") ? "--" : dto.family);
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/family/top10lowesttimes", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<RaidDurationData>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			GUI.ShowPopUp("Failed to fetch data from the server.");
			return new List<RaidDurationData>();
		}
	}

	public static async Task<RaidData?> FetchRaidData(int raid_id)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.GetAsync($"{_baseUrl}/raid/raid_id/{raid_id}");
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<RaidData>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			GUI.ShowPopUp("Failed to fetch data from the server.");
			return new();
        }
	}

	public static async Task<bool> SendPlayersInfoData(PlayerFullInfoOnMapDto dto)
	{
		if (dto.server_id == 0)
		{
			return false;
		}
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/player/update", dto);
			obj.EnsureSuccessStatusCode();
			await obj.Content.ReadAsStringAsync();
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static async Task<PlayerInfoData> FetchPlayerData(ServerNicknameDto dto)
	{
		if (dto.server_id == 0)
		{
			return new();
        }
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/player/get", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<PlayerInfoData>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
        }
	}

	public static async Task<PlayerInfoData> SendItemDetails(GameEquipementItem dto)
	{
		if (dto.server_id == 0)
		{
			return new();
        }
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/item/set", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<PlayerInfoData>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
        }
	}

	public static async Task<RaidRankingInfo> GetRaidRankingInfo(GetRaidRankingInfoDto dto)
	{
		if (dto.server_id == 0)
		{
			return new();
        }
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/ranking", dto);
			obj.EnsureSuccessStatusCode();
			RaidRankingInfo raidRankingInfo = JsonConvert.DeserializeObject<RaidRankingInfo>(await obj.Content.ReadAsStringAsync());
			if (raidRankingInfo == null)
			{
				raidRankingInfo = new RaidRankingInfo
				{
					sp_id = dto.sp_id,
					boss_id = dto.boss_id,
					server_id = dto.server_id,
					mean_damage = 0,
					stddev_damage = 0,
					player_count = 0
				};
			}
			return raidRankingInfo;
		}
		catch (Exception)
		{
			return new();
        }
	}

	public static async Task<List<RaidRankingInfo>> GetRaidRankingsInfo(List<GetRaidRankingInfoDto> dto)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/rankings", dto);
			obj.EnsureSuccessStatusCode();
			List<RaidRankingInfo> list = JsonConvert.DeserializeObject<List<RaidRankingInfo>>(await obj.Content.ReadAsStringAsync());
			if (list == null)
			{
				list = new List<RaidRankingInfo>();
			}
			return list;
		}
		catch (Exception)
		{
			return new();
        }
	}

	public static async Task<PlayerRaidRankingInfo> GetPlayerRaidRankingInfo(GetPlayerRaidRankingInfoDto dto)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/player", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<PlayerRaidRankingInfo>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
        }
	}

	public static async Task<List<PlayerRaidRankingInfo>> GetPlayersRaidRankingInfo(List<GetPlayerRaidRankingInfoDto> dto)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/players", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<PlayerRaidRankingInfo>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
        }
	}

	public static async Task<List<PlayerFullRankingInfo>> GetFullRanking(GetFullRankingDto dto)
	{
		if (dto.server_id == 0)
		{
			return new();
        }
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/full_ranking", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<PlayerFullRankingInfo>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
        }
	}

	public static async Task<StatusMessage> RemindLicense(RemindLicenseDto dto)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/users/remind/license", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<StatusMessage>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
        }
	}

	public static async Task<StatusMessage> ChangeNickname(ChangeNicknameDto dto)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/users/change/nickname", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<StatusMessage>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
        }
	}

	public static async Task<StatusMessage> ConfirmChangeNickname(ConfirmNicknameChangeDto dto)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/users/change/nickname/confirm", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<StatusMessage>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
        }
	}

	public static async Task<List<BarStatusDto>> GetRaidBarsStatus(int server_id)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.GetAsync($"{_baseUrl}/raid/status/bar/{server_id}");
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<BarStatusDto>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
        }
	}

	public static async Task<List<RankingMaxHit>> GetMaxHitsRanking(RankingInfoDto dto)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/max_hits_ranking", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<RankingMaxHit>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
        }
	}

	public static async Task<List<RankingBestTime>> GetBestTimesRanking(RankingInfoDto dto)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/best_times_ranking", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<RankingBestTime>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
        }
	}

	public static async Task<List<RankingRaidsDone>> GetRaidsDoneRanking(RankingInfoDto dto)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.PostAsJsonAsync(_baseUrl + "/raid/raids_done_ranking", dto);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<RankingRaidsDone>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
		}
	}

	public static async Task<QuestDto?> GetQuestData(string quest_id)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.GetAsync(_baseUrl + "/quests/" + quest_id);
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<QuestDto>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static async Task<List<GameMapDto>?> GetGameWorld()
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.GetAsync(_baseUrl + "/maps/info/gameworld");
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<GameMapDto>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static async Task<List<NPCDto>?> GetNPCData(int id)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.GetAsync($"{_baseUrl}/npcs/{id}");
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<NPCDto>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static async Task<List<MobDto>?> GetMobData(int id)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.GetAsync($"{_baseUrl}/monsters/{id}");
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<MobDto>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
		}
	}

	public static async Task<List<MobDto>> GetAllMobsData()
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.GetAsync(_baseUrl + "/monsters");
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<MobDto>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
		}
	}

	public static async Task<List<NPCDto>> GetAllNPCsData()
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.GetAsync(_baseUrl + "/npcs");
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<Dtos.Output.NPCDto>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
		}
	}

	public static async Task<List<Dtos.Output.MapDto>> GetAllMapsData()
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.GetAsync(_baseUrl + "/maps");
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<Dtos.Output.MapDto>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
		}
	}

	public static async Task<List<Dtos.Output.TimeSpaceDto>> GetAllTSData()
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.GetAsync(_baseUrl + "/timespaces");
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<List<Dtos.Output.TimeSpaceDto>>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
		}
	}

	public static async Task<GameObjects.TimeSpaceData?> GetTimeSpaceData(int id)
	{
		_ = 1;
		try
		{
			HttpResponseMessage obj = await _httpClient.GetAsync($"{_baseUrl}/timespaces/data/{id}");
			obj.EnsureSuccessStatusCode();
			return JsonConvert.DeserializeObject<GameObjects.TimeSpaceData>(await obj.Content.ReadAsStringAsync());
		}
		catch (Exception)
		{
			return new();
		}
	}
}
