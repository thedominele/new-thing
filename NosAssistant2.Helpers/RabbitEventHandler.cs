using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using NosAssistant2.Dtos;
using NosAssistant2.Dtos.Input;
using NosAssistant2.Dtos.Output;
using NosAssistant2.Dtos.RabbitQueue;
using NosAssistant2.GameObjects;
using RabbitMQ.Client;

namespace NosAssistant2.Helpers;

public static class RabbitEventHandler
{
	private static IConnection? _connection;

	private static IModel? _channel;

	public static void Connect()
	{
		_connection = new ConnectionFactory
		{
			HostName = "nosassistant.pl",
			UserName = GUI.Main?.nickname,
			Password = CheckAccess.license_key
		}.CreateConnection();
		_channel = _connection.CreateModel();
	}

	public static void Disconnect()
	{
		if (_connection != null && _connection.IsOpen)
		{
			_connection.Close();
		}
		if (_channel != null && _channel.IsOpen)
		{
			_channel.Close();
		}
	}

	public static void SendEvent(BaseEvent baseEvent, byte priority)
	{
		if (_connection == null || _channel == null || !_connection.IsOpen || !_channel.IsOpen)
		{
			Disconnect();
			Connect();
		}
		if (_connection != null && _channel != null)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(baseEvent));
			IBasicProperties basicProperties = _channel.CreateBasicProperties();
			basicProperties.Priority = priority;
			basicProperties.UserId = GUI.Main?.nickname ?? "";
			_channel.BasicPublish("", "priority_events", basicProperties, bytes);
		}
	}

	public static bool SendPlayersOnMapData(PlayerFullInfoOnMapDto dto)
	{
		if (dto.server_id == 0)
		{
			return false;
		}
		try
		{
			SendPlayersOnMapEvent baseEvent = new SendPlayersOnMapEvent
			{
				EventType = "SendPlayersOnMapEvent",
				Data = dto
			};
			byte priority = 1;
			SendEvent(baseEvent, priority);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool SendPlayerData(PlayerFullInfoDto dto)
	{
		if (dto.server_id == 0)
		{
			return false;
		}
		try
		{
			SendPlayerEvent baseEvent = new SendPlayerEvent
			{
				EventType = "SendPlayerEvent",
				Data = dto
			};
			byte priority = 1;
			SendEvent(baseEvent, priority);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool SendEquipementItem(GameEquipementItem dto)
	{
		if (dto.server_id == 0)
		{
			return false;
		}
		try
		{
			SendEquipementItemEvent baseEvent = new SendEquipementItemEvent
			{
				EventType = "SendEquipementItemEvent",
				Data = dto
			};
			byte priority = 2;
			SendEvent(baseEvent, priority);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool SendSPDetails(UpdatePlayerSPDto dto)
	{
		if (dto.server_id == 0)
		{
			return false;
		}
		try
		{
			UpdatePlayerSPEvent baseEvent = new UpdatePlayerSPEvent
			{
				EventType = "UpdatePlayerSPEvent",
				Data = dto
			};
			byte priority = 2;
			SendEvent(baseEvent, priority);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool UpdateSPDetails(PlayerSPDetails dto)
	{
		if (dto.server_id == 0)
		{
			return false;
		}
		try
		{
			UpdatePlayerSPDetailsEvent baseEvent = new UpdatePlayerSPDetailsEvent
			{
				EventType = "UpdatePlayerSPDetailsEvent",
				Data = dto
			};
			byte priority = 2;
			SendEvent(baseEvent, priority);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool SendTattoos(PlayerTattooDto dto)
	{
		if (dto.server_id == 0)
		{
			return false;
		}
		try
		{
			SendTattoosEvent baseEvent = new SendTattoosEvent
			{
				EventType = "SendTattoosEvent",
				Data = dto
			};
			byte priority = 2;
			SendEvent(baseEvent, priority);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool SendFairyDetails(PlayerFairyDto dto)
	{
		if (dto.server_id == 0)
		{
			return false;
		}
		try
		{
			SendFairyDetailsEvent baseEvent = new SendFairyDetailsEvent
			{
				EventType = "SendFairyDetailsEvent",
				Data = dto
			};
			byte priority = 2;
			SendEvent(baseEvent, priority);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool SendRaidDataEvent(RaidData dto)
	{
		if (dto.server_id == 0)
		{
			return false;
		}
		try
		{
			SendRaidDataEvent baseEvent = new SendRaidDataEvent
			{
				EventType = "SendRaidDataEvent",
				Data = dto
			};
			byte priority = 3;
			SendEvent(baseEvent, priority);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool SendBarStatusEvent(BarStatusDto dto)
	{
		if (dto.server_id == 0)
		{
			return false;
		}
		try
		{
			SendBarStatusEvent baseEvent = new SendBarStatusEvent
			{
				EventType = "SendBarStatusEvent",
				Data = dto
			};
			byte priority = 1;
			SendEvent(baseEvent, priority);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static void UploadRaidsManually()
	{
		Disconnect();
		_connection = new ConnectionFactory
		{
			HostName = "nosassistant.pl",
			UserName = "x",
			Password = "x"
		}.CreateConnection();
		_channel = _connection.CreateModel();
		GUI.Main.nickname = "x";
		string text = "raids_upload";
		if (Directory.Exists(text))
		{
			string[] files = Directory.GetFiles(text, "*.json");
			foreach (string path in files)
			{
				try
				{
					SendRaidDataEvent(JsonConvert.DeserializeObject<RaidData>(File.ReadAllText(path)));
				}
				catch (Exception)
				{
				}
			}
		}
		else
		{
			Console.WriteLine("The directory '" + text + "' does not exist.");
		}
	}
}
