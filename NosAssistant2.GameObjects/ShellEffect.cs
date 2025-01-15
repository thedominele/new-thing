using System.Drawing;

namespace NosAssistant2.GameObjects;

public class ShellEffect
{
	public char prefix { get; set; }

	public Color color { get; set; }

	public string effect { get; set; }

	public int value { get; set; }

	public bool isPercent { get; set; }
}
