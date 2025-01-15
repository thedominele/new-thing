namespace NosAssistant2.Dtos.Output;

public class TimeSpaceDto : EntityDto
{
	public int timeSpaceId { get; set; }

	public int x { get; set; }

	public int y { get; set; }

	public int minLvl { get; set; }

	public int maxLvl { get; set; }
}
