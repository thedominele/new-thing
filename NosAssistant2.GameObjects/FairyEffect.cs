using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NosAssistant2.GameObjects;

public class FairyEffect
{
	public string effect { get; set; }

	public string effect_class { get; set; }

	public Color effect_color { get; set; }

	public int value { get; set; }

	public bool isPercent { get; set; }

	public static List<Label> toLabels(Control parent, List<FairyEffect> effects)
	{
		List<Label> list = new List<Label>();
		foreach (FairyEffect effect in effects)
		{
			Label label = new Label();
			string text = ((effect.value == -1) ? "" : (effect.isPercent ? $": {effect.value}%" : $": {effect.value}"));
			if (effect.effect.Contains("*x*"))
			{
				label.Text = effect.effect_class + "-" + effect.effect.Replace("*x*", text.Substring(2, text.Length - 2));
			}
			else
			{
				label.Text = effect.effect_class + "-" + effect.effect + text;
			}
			label.ForeColor = effect.effect_color;
			label.Name = $"fairy_effect_label_{effects.IndexOf(effect)}";
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
