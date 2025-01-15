using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NosAssistant2.Helpers;

namespace NosAssistant2.GameObjects;

public class GamePlayer
{
	public static Dictionary<int, string> id_to_class = new Dictionary<int, string>
	{
		{ 0, "Adventurer" },
		{ 1, "Warrior" },
		{ 2, "Archer" },
		{ 3, "Mage" },
		{ 4, "Martial" }
	};

	public static Dictionary<int?, string> id_to_fam_role = new Dictionary<int?, string>
	{
		{ 915, "Head" },
		{ 916, "Deputy" },
		{ 917, "Keeper" },
		{ 918, "Member" }
	};

	public static readonly Dictionary<int?, string> title_to_name = new Dictionary<int?, string>
	{
		{ 13008, "AI Destroyer" },
		{ 13007, "Is in Love" },
		{ 13006, "Toxic" },
		{ 13004, "Is in Love" },
		{ 13003, "Giant Arma Destroyer" },
		{ 13002, "Pollutus Hunter" },
		{ 13001, "Giant Arma" },
		{ 13000, "Moss Giant Pollutus" },
		{ 9496, "Snowman Head" },
		{ 9488, "Jellyfish" },
		{ 9487, "Slick" },
		{ 9484, "5-Star Collector" },
		{ 9483, "4-Star Collector" },
		{ 9482, "Lord of the Fleece" },
		{ 9481, "Mothzilla" },
		{ 9480, "Bubble Butt" },
		{ 9479, "Master Trainer" },
		{ 9478, "Evolution Champion" },
		{ 9477, "Evolution Master" },
		{ 9476, "Evolution Expert" },
		{ 9475, "Evolution Amateur" },
		{ 9472, "Expert Trainer" },
		{ 9471, "Trainer" },
		{ 9470, "Amateur Trainer" },
		{ 9468, "Eternal" },
		{ 9462, "Mushroom Head" },
		{ 9452, "Ate All the Cake" },
		{ 9444, "Mystical" },
		{ 9442, "Snow Fairy" },
		{ 9440, "Dragon Hunter" },
		{ 9439, "Busy Bee" },
		{ 9438, "Fabulous" },
		{ 9437, "Stir Fry Masterchef" },
		{ 9436, "Professional Stir Fry Chef" },
		{ 9435, "Amateur Stir Fry Chef" },
		{ 9434, "Stew Masterchef" },
		{ 9433, "Professional Stew Chef" },
		{ 9432, "Amateur Stew Chef" },
		{ 9431, "Rotisserie Masterchef" },
		{ 9430, "Professional Rotisserie Chef" },
		{ 9429, "Amateur Rotisserie Chef" },
		{ 9428, "Pun Master" },
		{ 9427, "Gourmet" },
		{ 9426, "Royal Chef of Honour" },
		{ 9425, "Royal Chef" },
		{ 9424, "Royal Apprentice Chef" },
		{ 9423, "Ninja" },
		{ 9422, "Grinch" },
		{ 9421, "Bigfoot" },
		{ 9420, "Caught the Biggest" },
		{ 9419, "Big Fish to Fry" },
		{ 9418, "Mermaid" },
		{ 9417, "Something Fishy" },
		{ 9416, "Alligator" },
		{ 9415, "Octo-Catcher" },
		{ 9414, "Fish Out of Water" },
		{ 9413, "Jaws" },
		{ 9412, "Blue-Water Fisher" },
		{ 9411, "Bass Kicker" },
		{ 9410, "The Price of Fish" },
		{ 9409, "Pondlife" },
		{ 9408, "Champion Angler" },
		{ 9407, "Monstrous" },
		{ 9406, "Time Traveller" },
		{ 9405, "Highscore Hunter" },
		{ 9404, "Real Hero" },
		{ 9403, "Celestial Spire Conqueror" },
		{ 9402, "Celestial Spire Hero" },
		{ 9401, "Paimon's Slayer" },
		{ 9400, "Saviour of the Orcs" },
		{ 9399, "Warm Heart" },
		{ 9398, "in Boots" },
		{ 9397, "Survivor!" },
		{ 9396, "Wordsmith" },
		{ 9395, "Film Star" },
		{ 9394, "Photo Model" },
		{ 9393, "Quizmaster" },
		{ 9392, "The Biggest Fan" },
		{ 9391, "Prodigy" },
		{ 9390, "Cookie Monster" },
		{ 9389, "Artiste" },
		{ 9388, "Minion" },
		{ 9387, "Lightbringer" },
		{ 9386, "Watercharmer" },
		{ 9385, "Shadowheart" },
		{ 9384, "Firebreather" },
		{ 9383, "Mad Hatter" },
		{ 9382, "Twisted" },
		{ 9381, "Treasure Hunter" },
		{ 9380, "Ancelloan's Herald" },
		{ 9379, "Little Devil" },
		{ 9378, "Little Angel" },
		{ 9377, "Lopears" },
		{ 9376, "Evil Twin" },
		{ 9375, "Little Witch" },
		{ 9374, "Doppelgänger" },
		{ 9373, "Ice Cold" },
		{ 9372, "Dragonslayer" },
		{ 9371, "Phoenix" },
		{ 9370, "Pyromaniac" },
		{ 9369, "Fire Hound" },
		{ 9368, "Arrrrr!" },
		{ 9367, "BBQ King" },
		{ 9366, "Master Thief" },
		{ 9365, "Sleepy Head" },
		{ 9364, "Gorged" },
		{ 9363, "Sakura's Hero" },
		{ 9362, "Nugget" },
		{ 9361, "Rock Solid" },
		{ 9360, "Arachnophobe" },
		{ 9359, "Ritualist" },
		{ 9358, "Gardener" },
		{ 9357, "Sweet Tooth" },
		{ 9356, "Punter" },
		{ 9355, "Bushtail" },
		{ 9354, "Fire Devil" },
		{ 9353, "Croesus" },
		{ 9352, "Unicorn" },
		{ 9351, "Duellist" },
		{ 9350, "Test Subject" },
		{ 9349, "Sly Dog" },
		{ 9348, "Horror" },
		{ 9347, "Godlike" },
		{ 9346, "Legendary Hero" },
		{ 9345, "Sixth Sense" },
		{ 9344, "Bastion" },
		{ 9343, "Eagle Eyes" },
		{ 9342, "Fountain of Fortune" },
		{ 9341, "Liberator" },
		{ 9340, "Onyx Dragon" },
		{ 9339, "PvP Titan" },
		{ 9338, "PvP Legend" },
		{ 9337, "PvP Champion" },
		{ 9336, "PvP Angel of Death" },
		{ 9335, "Hercules" },
		{ 9334, "Force of Nature" },
		{ 9333, "Augustus" },
		{ 9332, "Fist of the Gods" },
		{ 9331, "Sight Beyond Sight" },
		{ 9330, "The Chosen One" },
		{ 9329, "Puppet Master" },
		{ 9328, "Astrologer" },
		{ 9327, "Pure Soul" },
		{ 9326, "Living Legend" },
		{ 9325, "Illuminated" },
		{ 9324, "Influencer" },
		{ 9323, "Necromancer" },
		{ 9322, "Gravedigger" },
		{ 9321, "Enter the Dragon" },
		{ 9320, "Sensei" },
		{ 9319, "Black Belt" },
		{ 9318, "Blademaster" },
		{ 9317, "Templar" },
		{ 9316, "Mercenary" },
		{ 9315, "Master Archer" },
		{ 9314, "Robin Hood" },
		{ 9313, "Bull's Eye" },
		{ 9312, "Warlock" },
		{ 9311, "Arch Mage" },
		{ 9310, "Mystic" },
		{ 9309, "Zenith" },
		{ 9308, "Indefatigable" },
		{ 9307, "Saviour of NosVille" },
		{ 9306, "Weedkiller" },
		{ 9305, "Diplomat" },
		{ 9304, "Peacemaker" },
		{ 9303, "Idealist" },
		{ 9302, "Rebel" },
		{ 9301, "Animal Lover" },
		{ 9300, "Adventurer" }
	};

	public int type { get; set; } = 1;


	public string nickname { get; set; } = "";


	public string family { get; set; } = "";


	public int character_id { get; set; }

	public int x { get; set; }

	public int y { get; set; }

	public int z { get; set; }

	public int lvl { get; set; }

	public int clvl { get; set; }

	public int spID { get; set; } = -1;


	public int sp_upgrade { get; set; } = -1;


	public int sp_wings_id { get; set; } = -1;


	public int classID { get; set; } = -1;


	public int sex { get; set; } = -1;


	public string fraction { get; set; } = "netural";


	public string items { get; set; } = "";


	public string weapon_upgrade { get; set; } = "";


	public string armor_upgrade { get; set; } = "";


	public int family_role_id { get; set; }

	public int family_lvl { get; set; }

	public int family_id { get; set; }

	public int reputation { get; set; }

	public int fairy_id { get; set; }

	public int fairy_element_id { get; set; }

	public int title { get; set; }

	public int? real_title { get; set; }

	public Image? icon { get; set; }

	public Color color { get; set; } = NAStyles.PlayersColor;


	public void UpdateIcon()
	{
		if (spID < 1800 && spID != 1564)
		{
			SPCard sPCard = new SPCard();
			sPCard.UpdateSPCard(spID);
			string value = ((sex == 0) ? "male" : "female");
			string text = $"images\\portraits\\{spID}_{value}{((sPCard != null && !sPCard.Shared) ? ("_" + classID) : "")}.png";
			icon = (File.Exists(text) ? Image.FromFile(text) : (File.Exists("images/npcs/empty.png") ? Image.FromFile("images/npcs/empty.png") : null));
		}
		else if (spID < 1000000)
		{
			string text2 = $"images\\portraits\\{spID}.png";
			icon = (File.Exists(text2) ? Image.FromFile(text2) : (File.Exists("images/npcs/empty.png") ? Image.FromFile("images/npcs/empty.png") : null));
		}
		else
		{
			string text3 = $"images\\npcs\\{spID - 1000000}.png";
			icon = (File.Exists(text3) ? Image.FromFile(text3) : (File.Exists("images/npcs/empty.png") ? Image.FromFile("images/npcs/empty.png") : null));
		}
	}

	public static Image? GetIcon(int spID, int sex, int classID)
	{
		SPCard sPCard = new SPCard();
		sPCard.UpdateSPCard(spID);
		string value = ((sex == 0) ? "male" : "female");
		string text = $"images\\portraits\\{spID}_{value}{((sPCard != null && !sPCard.Shared) ? ("_" + classID) : "")}.png";
		if (!File.Exists(text))
		{
			if (!File.Exists("images/npcs/empty.png"))
			{
				return null;
			}
			return Image.FromFile("images/npcs/empty.png");
		}
		return Image.FromFile(text);
	}

	public static Image? GetReputationIcon(int id)
	{
		if (!File.Exists($"images/reputation/{id}.png"))
		{
			if (!File.Exists("images/reputation/0.png"))
			{
				return null;
			}
			return Image.FromFile("images/reputation/0.png");
		}
		return Image.FromFile($"images/reputation/{id}.png");
	}
}
