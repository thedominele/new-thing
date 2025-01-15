using System;

namespace NosAssistant2.Dtos.Output;

public class LicenseResponse
{
	public bool valid { get; set; }

	public DateTime valid_until { get; set; }
}
