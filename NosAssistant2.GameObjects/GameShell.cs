using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NosAssistant2.GameObjects;

public class GameShell
{
	public static readonly Dictionary<int, (char, Color)> OrderToColor = new Dictionary<int, (char, Color)>
	{
		{
			1,
			('C', Color.Orange)
		},
		{
			2,
			('B', Color.Yellow)
		},
		{
			3,
			('A', Color.Green)
		},
		{
			4,
			('S', Color.Lime)
		},
		{
			6,
			('B', Color.Purple)
		},
		{
			7,
			('A', Color.Purple)
		},
		{
			10,
			('B', Color.Pink)
		},
		{
			11,
			('A', Color.Pink)
		},
		{
			12,
			('S', Color.Pink)
		},
		{
			13,
			('C', Color.Orange)
		},
		{
			14,
			('B', Color.Yellow)
		},
		{
			15,
			('A', Color.Green)
		},
		{
			16,
			('S', Color.Lime)
		},
		{
			18,
			('B', Color.Purple)
		},
		{
			19,
			('A', Color.Purple)
		},
		{
			22,
			('B', Color.Pink)
		},
		{
			23,
			('A', Color.Pink)
		},
		{
			24,
			('S', Color.Pink)
		}
	};

	public static readonly Dictionary<int, (string, bool)> WeaponEffectToString = new Dictionary<int, (string, bool)>
	{
		{
			1,
			("Enchanced Damage", false)
		},
		{
			2,
			("% to Damage", true)
		},
		{
			3,
			("Minor Bleeding", true)
		},
		{
			4,
			("Open Wound", true)
		},
		{
			5,
			("Heavy Bleeding", true)
		},
		{
			6,
			("Stun", true)
		},
		{
			7,
			("Freeze", true)
		},
		{
			8,
			("Stun Badly", true)
		},
		{
			9,
			("Increased Damage to Plant", true)
		},
		{
			10,
			("Increased Damage to Animal", true)
		},
		{
			11,
			("Increased Damage to monsters", true)
		},
		{
			12,
			("Increased Damage to Undead", true)
		},
		{
			13,
			("Incresed Damage to Lower Society Monsters", true)
		},
		{
			14,
			("Incresed damage to map bosses", true)
		},
		{
			15,
			("(Except sticks) Incresed Chance of Critical Hit", true)
		},
		{
			16,
			("(Except sticks) Incresed Critical Damage", true)
		},
		{
			17,
			("(Sticks only) Undisturbed When Casting Spells", false)
		},
		{
			18,
			("Increase Fire Property", false)
		},
		{
			19,
			("Increase Water Property", false)
		},
		{
			20,
			("Increased Light Property", false)
		},
		{
			21,
			("Increased Shadow Property", false)
		},
		{
			22,
			("Increased Elemental Properies", false)
		},
		{
			23,
			("Reduced MP Conumption", false)
		},
		{
			24,
			("HP Recovery per Kill", false)
		},
		{
			25,
			("MP Recovery per Kill", false)
		},
		{
			26,
			("Increased SL Attack Stat", false)
		},
		{
			27,
			("Increased SL Defence Stat", false)
		},
		{
			28,
			("Increased SL Property Stat", false)
		},
		{
			29,
			("Increased SL Energy Stat", false)
		},
		{
			30,
			("Increased Overall SL Stat", false)
		},
		{
			31,
			("(Main weapon only) Gain More Gold", true)
		},
		{
			32,
			("(Main weapon only) increased combat EXP", true)
		},
		{
			33,
			("(Main weapon only) Gain More CXP", true)
		},
		{
			34,
			("% to Damage in PVP", true)
		},
		{
			35,
			("Reduces opponent's defence power in PvP by", true)
		},
		{
			36,
			("Reduce Opponent's Fire Resistance in PVP", true)
		},
		{
			37,
			("Reduces Opponent's water Resistance in PVP", true)
		},
		{
			38,
			("Reduces Opponent's light Resistance in PVP", true)
		},
		{
			39,
			("Reduce Opponent's Shadow Resistance in PVP", true)
		},
		{
			40,
			("Reduce Opponent's All Resistance in PVP", true)
		},
		{
			42,
			("% to Damage at 15% in PVP", true)
		},
		{
			43,
			("Drain Oppnent's Mana in PVP", false)
		}
	};

	public static readonly Dictionary<int, (string, bool)> ArmorEffectToString = new Dictionary<int, (string, bool)>
	{
		{
			1,
			("Enchanced Melee Defense", false)
		},
		{
			2,
			("Enchanced Long Range Defense", false)
		},
		{
			3,
			("Enchanced Magic Defense", false)
		},
		{
			4,
			("% to Overall Defense", true)
		},
		{
			5,
			("Reduced Chance of Small Open Wound", true)
		},
		{
			6,
			("Reduced Chance of Open Wound and Small Open Wound", true)
		},
		{
			7,
			("Reduced Chance of All Open Wounds", true)
		},
		{
			8,
			("Reduces the chance of a Blackout", true)
		},
		{
			9,
			("Reduced Chance of All Stun", true)
		},
		{
			10,
			("Reduced Chance of Dedman's Hand", true)
		},
		{
			11,
			("Reduced Chance of Being Frozen", true)
		},
		{
			12,
			("Reduced Chance of Being Blinded", true)
		},
		{
			13,
			("Reduced Chance of Spasm", true)
		},
		{
			14,
			("Reduced Chance of Weak Armor", true)
		},
		{
			15,
			("Reduced Chance of Shock", true)
		},
		{
			16,
			("Reduced Chance of Paralysis Poison", true)
		},
		{
			17,
			("Reduces the chance of all negative effects", true)
		},
		{
			18,
			("Increased HP Recovery Rate While Resting", true)
		},
		{
			19,
			("Increased Natural HP Recovery", true)
		},
		{
			20,
			("Increased MP Recovery Rate While Resting", true)
		},
		{
			21,
			("Increased Natural MP Recovery Rate", true)
		},
		{
			22,
			("HP recovery while defending", true)
		},
		{
			23,
			("Reduces chance of receiving a critical hit", true)
		},
		{
			24,
			("Increases fire resistance", true)
		},
		{
			25,
			("Increases water resistance", true)
		},
		{
			26,
			("Increases light resistance", true)
		},
		{
			27,
			("Increases shadow resistance", true)
		},
		{
			28,
			("Increases all resistances", true)
		},
		{
			29,
			("Decreased Pride Reduction", true)
		},
		{
			30,
			("Decreased Consumption of Production Point", true)
		},
		{
			31,
			("Increased Production Chance", true)
		},
		{
			32,
			("Increased Item Recovery", true)
		},
		{
			33,
			("% to Overall Defence in PVP", true)
		},
		{
			34,
			("Dodge Melee Attacks in PVP", true)
		},
		{
			35,
			("Dodge Ranged Attacks in PVP", true)
		},
		{
			36,
			("Dodge Magic Attacks in PVP", true)
		},
		{
			37,
			("Dodge All Attacks in PVP", true)
		},
		{
			38,
			("Protect Mana in PVP", false)
		}
	};

	public List<ShellEffect> effects { get; set; } = new List<ShellEffect>();


	public int item_type { get; set; }

	public GameShell()
	{
	}

	public GameShell(List<string> bonuses_strings, int i_type)
	{
		item_type = i_type;
		foreach (string bonuses_string in bonuses_strings)
		{
			List<string> source = bonuses_string.Split(".").ToList();
			var (prefix, color) = OrderToColor[Convert.ToInt32(source.ElementAt(0))];
			string text;
			bool isPercent;
			if (item_type != 1)
			{
				(text, isPercent) = ArmorEffectToString[Convert.ToInt32(source.ElementAt(1))];
			}
			else
			{
				(text, isPercent) = WeaponEffectToString[Convert.ToInt32(source.ElementAt(1))];
			}
			int num = Convert.ToInt32(source.ElementAt(2));
			effects.Add(new ShellEffect
			{
				color = color,
				prefix = prefix,
				effect = text,
				value = ((text == "(Sticks only) Undisturbed When Casting Spells" || text == "Protect Mana in PVP") ? (-1) : num),
				isPercent = isPercent
			});
		}
	}

	public List<Label> toLabels(Control parent)
	{
		List<Label> list = new List<Label>();
		foreach (ShellEffect effect in effects)
		{
			Label label = new Label();
			string value = ((effect.value == -1) ? "" : (effect.isPercent ? $": {effect.value}%" : $": {effect.value}"));
			label.Text = $"{effect.prefix}-{effect.effect}{value}";
			label.ForeColor = effect.color;
			label.Name = $"shell_label_{effects.IndexOf(effect)}";
			label.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold, GraphicsUnit.Point, 238);
			label.Width = parent.Width;
			GUI.ScaleControl(label);
			using (Graphics graphics = label.CreateGraphics())
			{
				int num = ((GUI.MainMonitorScalingFactor == 75f) ? 50 : 40);
				SizeF sizeF = graphics.MeasureString(label.Text, label.Font, label.Width - num);
				int num2 = 4;
				label.Height = (int)Math.Ceiling(sizeF.Height) + num2;
			}
			list.Add(label);
		}
		return list;
	}
}
