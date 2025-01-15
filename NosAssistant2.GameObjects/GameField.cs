using System.Collections.Generic;
using System.Drawing;

namespace NosAssistant2.GameObjects;

public class GameField
{
	private static Color blueFieldColor = Color.Blue;

	private static Color redFieldColor = Color.Red;

	private static Color yellowFieldColor = Color.Yellow;

	private static Color blackFieldColor = Color.Black;

	public static readonly Dictionary<int, GameField> dict = new Dictionary<int, GameField>
	{
		{
			4280,
			new GameField
			{
				ID = 4280,
				Name = "Glacerus Field 1",
				Color = blueFieldColor
			}
		},
		{
			4281,
			new GameField
			{
				ID = 4281,
				Name = "Glacerus Field 2",
				Color = blueFieldColor
			}
		},
		{
			4282,
			new GameField
			{
				ID = 4282,
				Name = "Glacerus Field 3",
				Color = blueFieldColor
			}
		},
		{
			8130,
			new GameField
			{
				ID = 8130,
				Name = "Azgobas Field 1",
				Color = blueFieldColor
			}
		},
		{
			8131,
			new GameField
			{
				ID = 8131,
				Name = "Azgobas Field 2",
				Color = redFieldColor
			}
		},
		{
			8132,
			new GameField
			{
				ID = 8132,
				Name = "Azgobas Field 3",
				Color = yellowFieldColor
			}
		},
		{
			8133,
			new GameField
			{
				ID = 8133,
				Name = "Azgobas Field 4",
				Color = blackFieldColor
			}
		},
		{
			821,
			new GameField
			{
				ID = 821,
				Name = "Hidden TS Field",
				Color = blueFieldColor
			}
		}
	};

	public int ID { get; set; } = -1;


	public string Name { get; set; } = "";


	public int x { get; set; } = -1;


	public int y { get; set; } = -1;


	public Color Color { get; set; } = Color.Transparent;


	public GameField()
	{
	}

	public static bool isGlacerusField(int id)
	{
		return new List<int> { 4280, 4281, 4282 }.Contains(id);
	}

	public GameField(int id)
	{
		if (dict.ContainsKey(id))
		{
			GameField gameField = dict[id];
			ID = gameField.ID;
			Name = gameField.Name;
			Color = gameField.Color;
		}
	}

	public static bool isField(int id)
	{
		return dict.ContainsKey(id);
	}
}
