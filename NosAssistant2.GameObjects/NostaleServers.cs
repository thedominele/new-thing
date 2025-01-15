using System.Collections.Generic;
using System.Linq;

namespace NosAssistant2.GameObjects;

public static class NostaleServers
{
	private static Dictionary<string, string> ipToServerName = new Dictionary<string, string>
	{
		{ "79.110.84.250", "Cosmos" },
		{ "79.110.84.175", "Dragonveil" },
		{ "79.110.84.77", "Alzanor" },
		{ "79.110.84.25", "Valehir" }
	};

	private static Dictionary<string, int> serverNameToId = new Dictionary<string, int>
	{
		{ "Cosmos", 1 },
		{ "Dragonveil", 2 },
		{ "Alzanor", 4 },
		{ "Valehir", 5 }
	};

	private static Dictionary<int, int> portToServerNumber = new Dictionary<int, int>
	{
		{ 4003, 1 },
		{ 4004, 2 },
		{ 4006, 3 },
		{ 4007, 4 },
		{ 4008, 5 },
		{ 4009, 6 },
		{ 4010, 7 },
		{ 4011, 51 }
	};

	public static string GetServerNameFromIp(string ip)
	{
		if (!ipToServerName.TryGetValue(ip, out string value))
		{
			return "Unknown";
		}
		return value;
	}

	public static int GetServerChannelFromPort(int port)
	{
		if (!portToServerNumber.TryGetValue(port, out var value))
		{
			return 0;
		}
		return value;
	}

	public static int GetServerIdFromName(string name)
	{
		if (!serverNameToId.TryGetValue(name, out var value))
		{
			return 0;
		}
		return value;
	}

	public static string GetServerNameFromId(int id)
	{
		return serverNameToId.FirstOrDefault<KeyValuePair<string, int>>((KeyValuePair<string, int> x) => x.Value == id).Key;
	}

	public static List<string> GetAllServersNames()
	{
		return serverNameToId.Keys.ToList();
	}
}
