using System.Collections.Generic;
using System.Drawing;
using NosAssistant2.GameObjects;

namespace NosAssistant2.GameData;

public class CharConfig
{
	public bool useFullBeforeEreniaManuk;

	public List<GameQuest> quests = new List<GameQuest>();

	public string nickname { get; set; } = "";


	public int character_id { get; set; }

	public bool isAttacker { get; set; }

	public bool isRaider { get; set; } = true;


	public bool isMover { get; set; }

	public bool isBuffer { get; set; } = true;


	public bool isAutoFull { get; set; }

	public bool isDisabled { get; set; }

	public string charBuffs { get; set; } = "";


	public string partnerBuffs { get; set; } = "";


	public bool petBuff { get; set; }

	public bool potionValehir { get; set; }

	public bool potionAlzanor { get; set; }

	public bool attackPotion { get; set; }

	public bool sideZenas { get; set; }

	public bool walkToFields { get; set; }

	public bool isAutoFullMana { get; set; }

	public int AutoFullManaThreshold { get; set; }

	public bool isDebuffer { get; set; }

	public bool isGoldPicker { get; set; }

	public bool packetLoggerFilterState { get; set; }

	public Point window_position { get; set; } = new Point(0, 0);

}
