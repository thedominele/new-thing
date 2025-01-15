using System.Collections.Generic;

namespace NosAssistant2.GameObjects;

public class NosPacket
{
	public string date { get; set; } = "";


	public string type { get; set; } = "";


	public string packet_type { get; set; } = "";


	public List<string> packet_splitted { get; set; } = new List<string>();


	public uint process_id { get; set; }

	public nint hwnd { get; set; }

	public string content { get; set; } = "";

}
