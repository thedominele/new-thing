using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NosAssistant2.GameObjects;

public class GameRune
{
	public static readonly Dictionary<string, (string, int, bool)> RuneEffectData = new Dictionary<string, (string, int, bool)>
	{
		{
			"3.0",
			("All attacks are incresed by", 0, false)
		},
		{
			"4.0",
			("Hit rate of all attacks is increased by", 0, false)
		},
		{
			"4.3",
			("Concentration during the magic attack is increased by", 0, false)
		},
		{
			"4.4",
			("Damage to monsters is increased by", 0, true)
		},
		{
			"5.0",
			("Chance of inflicting critical hits is increased by", 0, true)
		},
		{
			"5.1",
			("Increases damage from critical hits by", 0, true)
		},
		{
			"10.4",
			("Damage to low-level monsters in the Celestial Lair and Land of Life is increased by", 0, true)
		},
		{
			"44.1",
			("All attacks are increased by", 0, true)
		},
		{
			"50.3",
			("Damage to high-level dragons is increased by", 0, true)
		},
		{
			"96.0",
			("Increases the equipped fairy's element by", 0, false)
		},
		{
			"102.0",
			("Your specialist's attack skill points are increased by", 0, false)
		},
		{
			"102.2",
			("Your specialist's elemental skill points are increased by", 0, false)
		},
		{
			"102.4",
			("Hit rate is increased by", 0, true)
		},
		{
			"103.4",
			("Concentration is increased by", 0, true)
		},
		{
			"104.3",
			("Increases the attack power of your NosMate by", 0, true)
		},
		{
			"105.0",
			("Apocalypse Power", 7600, false)
		},
		{
			"105.1",
			("Reflection Power", 7620, false)
		},
		{
			"105.2",
			("Wolf Power", 7644, false)
		},
		{
			"105.3",
			("Kickback Power", 7660, false)
		},
		{
			"105.4",
			("Explosion Power", 7680, false)
		},
		{
			"106.0",
			("Agility Power", 7700, false)
		},
		{
			"106.1",
			("Lightning Power", 7720, false)
		},
		{
			"106.2",
			("Curse Power", 7740, false)
		},
		{
			"106.3",
			("Bear Power", 7760, false)
		},
		{
			"106.4",
			("Frost Power", 7780, false)
		},
		{
			"5.3",
			("Probability to receive critical hits is decreased by", 0, true)
		},
		{
			"5.4",
			("Damage from critical hits is reduced by", 0, true)
		},
		{
			"9.0",
			("All defence powers are increased by", 0, false)
		},
		{
			"10.0",
			("Dodge is increased by", 0, false)
		},
		{
			"10.3",
			("Defence is increased by", 0, true)
		},
		{
			"13.0",
			("All elemental resistance is increased by", 0, false)
		},
		{
			"39.3",
			("Increases maximum HP by", 0, true)
		},
		{
			"39.4",
			("Increases maximum MP by", 0, true)
		},
		{
			"45.0",
			("Up to level 4 there is chance of never getting a bad effect", 0, true)
		},
		{
			"98.0",
			("With a *x* probability all elemental damage is reduced by", 0, true)
		},
		{
			"102.1",
			("Your specialist's defence skill points are increased by", 0, false)
		},
		{
			"102.3",
			("Your specialist's HP/MP skill points are increased by", 0, false)
		},
		{
			"107.4",
			("Dodge is increased by", 0, true)
		},
		{
			"116.0",
			("Power of Cleansing Armour", 7840, false)
		},
		{
			"116.1",
			("Power of Regeneration", 7864, false)
		},
		{
			"116.2",
			("Power of Flame", 7888, false)
		},
		{
			"116.3",
			("Power of Purity", 7912, false)
		},
		{
			"116.4",
			("Power of Resistance", 7940, false)
		},
		{
			"117.0",
			("Power of Blood", 7964, false)
		},
		{
			"117.1",
			("Power of Conversion", 7984, false)
		},
		{
			"117.2",
			("Power of Unyielding", 5608, false)
		},
		{
			"117.3",
			("Power of Instinct", 5632, false)
		},
		{
			"117.4",
			("Power of Healing", 5656, false)
		}
	};

	public List<RuneEffect> effects { get; set; } = new List<RuneEffect>();


	public GameRune()
	{
	}

	public GameRune(string rune_string)
	{
		foreach (string item3 in rune_string.Split(" ").ToList())
		{
			List<string> source = item3.Split(".").ToList();
			(string, int, bool) tuple = RuneEffectData[source.ElementAt(0) + "." + source.ElementAt(1)];
			string item = tuple.Item1;
			bool item2 = tuple.Item3;
			item = item.Replace("*x*", $"{Math.Abs((int)Convert.ToDouble(source.ElementAt(3)) * 25 / 100)}%");
			effects.Add(new RuneEffect
			{
				effect = (item ?? ""),
				value = Math.Abs((int)Convert.ToDouble(source.ElementAt(2)) * 25 / 100),
				isPercent = item2,
				level = Math.Abs(Convert.ToInt32(source.ElementAt(4)))
			});
		}
	}

	public List<Label> toLabels(Control parent)
	{
		List<Label> list = new List<Label>();
		foreach (RuneEffect effect in effects)
		{
			string value = (effect.isPercent ? "%" : "");
			Label label = new Label();
			label.Text = $"{effect.effect}: {effect.value}{value}";
			label.ForeColor = Color.Orange;
			label.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold, GraphicsUnit.Point, 238);
			label.Name = $"rune_label_{effects.IndexOf(effect)}";
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
