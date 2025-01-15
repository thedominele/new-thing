using System;

namespace NosAssistant2.GameObjects;

public class NosPayload
{
	public int encryption_key;

	public string arrival_date = "null";

	public string type { get; set; } = "";


	public nint hwnd { get; set; } = IntPtr.Zero;


	public uint process_id { get; set; }

	public byte[] payload { get; set; } = Array.Empty<byte>();

}
