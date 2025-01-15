using System.Collections.Generic;

namespace NosAssistant2.Configs;

public class PacketLoggerSettings
{
	public List<string> Filters { get; set; } = new List<string>();


	public bool LoggerMode { get; set; } = true;


	public bool LoggerRule { get; set; } = true;

}
