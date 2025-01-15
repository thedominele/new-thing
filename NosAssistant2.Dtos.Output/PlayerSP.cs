using System;
using System.Collections.Generic;

namespace NosAssistant2.Dtos.Output;

public class PlayerSP
{
	public const int AngelWings = 1;

	public const int DevilWings = 2;

	public const int FireWings = 4;

	public const int IceWings = 3;

	public const int GoldenEagleWings = 5;

	public const int TitanWings = 6;

	public const int ArchangelWings = 7;

	public const int ArchdaemonWings = 8;

	public const int BlazingFireWings = 9;

	public const int FrostyIceWings = 10;

	public const int GoldenWings = 11;

	public const int OnyxWings = 12;

	public const int FairyWings = 13;

	public const int MegaTitanWings = 14;

	public const int ZephyrWings = 15;

	public const int LightningWings = 16;

	public const int BladeWings = 17;

	public const int CrystalWings = 18;

	public const int PetalWings = 19;

	public const int LunarWings = 20;

	public const int GreenRetroWings = 21;

	public const int PinkRetroWings = 22;

	public const int YellowRetroWings = 23;

	public const int PurpleRetroWings = 24;

	public const int RedRetroWings = 25;

	public const int MagentaRetroWings = 26;

	public const int CyanRetroWings = 27;

	public const int TreeWings = 28;

	public const int SteampunkWings = 29;

	public const int PurpleMechaFlameWings = 30;

	public const int BlackMechaFlameWings = 31;

	public const int TurquoiseMechaFlameWings = 32;

	public const int BlueMechaFlameWings = 33;

	public const int RedMechaFlameWings = 34;

	public const int GreenMechaFlameWings = 35;

	public const int YellowMechaFlameWings = 36;

	public static readonly Dictionary<int, int> BuffToWings = new Dictionary<int, int>
	{
		{ 4002, 5 },
		{ 387, 6 },
		{ 395, 7 },
		{ 396, 8 },
		{ 397, 9 },
		{ 398, 10 },
		{ 410, 11 },
		{ 411, 12 },
		{ 444, 13 },
		{ 663, 14 },
		{ 686, 15 },
		{ 755, 16 },
		{ 838, 17 },
		{ 851, 18 },
		{ 926, 19 },
		{ 985, 20 },
		{ 1444, 21 },
		{ 4062, 28 },
		{ 4121, 29 },
		{ 4183, 30 }
	};

	public int sp_id { get; set; }

	public int sp_upgrade { get; set; }

	public int sp_wings { get; set; }

	public int? real_sp_wings { get; set; }

	public int? job { get; set; }

	public int? perfection { get; set; }

	public string? build { get; set; }

	public string? pp { get; set; }

	public string? sl { get; set; }

	public DateTime updatedAt { get; set; }

	public static bool isRetro(int id)
	{
		if (id >= 21)
		{
			return id <= 27;
		}
		return false;
	}

	public static bool isMechaFlame(int id)
	{
		if (id >= 30)
		{
			return id <= 36;
		}
		return false;
	}
}
