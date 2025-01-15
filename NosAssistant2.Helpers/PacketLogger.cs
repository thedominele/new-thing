using System.Collections.Generic;
using System.Linq;
using NosAssistant2.Configs;
using NosAssistant2.GameData;
using NosAssistant2.GameObjects;

namespace NosAssistant2.Helpers;

public static class PacketLogger
{
	public static Dictionary<nint, bool> activeChars = new Dictionary<nint, bool>();

	public static void Print(string text, NosPacket packet)
	{
		if ((Settings.config.PacketLoggerSettings.LoggerMode || Settings.config.PacketLoggerSettings.LoggerRule || Settings.config.PacketLoggerSettings.Filters.Any((string filter) => packet.content.Contains(filter))) && (!Settings.config.PacketLoggerSettings.LoggerMode || Settings.config.PacketLoggerSettings.LoggerRule || !Settings.config.PacketLoggerSettings.Filters.Any((string filter) => packet.content.Contains(filter))) && (Settings.config.PacketLoggerSettings.LoggerMode || !Settings.config.PacketLoggerSettings.LoggerRule || Settings.config.PacketLoggerSettings.Filters.Any((string filter) => packet.content.StartsWith(filter))) && (!Settings.config.PacketLoggerSettings.LoggerMode || !Settings.config.PacketLoggerSettings.LoggerRule || !Settings.config.PacketLoggerSettings.Filters.Any((string filter) => packet.content.StartsWith(filter))) && activeChars[packet.hwnd])
		{
			GUI.PacketsConsolePrint(text);
		}
	}

	public static void updatePacketsLoggerDict()
	{
		foreach (NostaleCharacterInfo nostaleCharacterInfo in GUI._nostaleCharacterInfoList)
		{
			activeChars[nostaleCharacterInfo.hwnd] = nostaleCharacterInfo.config.packetLoggerFilterState;
		}
	}
}
