using System.Collections.Generic;

namespace NosAssistant2.Helpers;

public class MapNPC
{
	public int ID { get; set; } = -1;


	public List<int> destination_maps { get; set; } = new List<int>();

}
