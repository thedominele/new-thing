using System;
using System.Drawing;

namespace NosAssistant2.GameObjects;

public class PetTrainerMob
{
	public int mob_id { get; set; }

	public int mob_server_id { get; set; }

	public int duration { get; set; }

	public DateTime spawn_time { get; set; }

	public DateTime despawn_time { get; set; }

	public Image? icon { get; set; }
}
