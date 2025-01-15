using System;
using System.Collections.Generic;
using System.Linq;

namespace NosAssistant2.GameObjects;

public class GameEquipementItem
{
	private Dictionary<int, (string, int)> item_types = new Dictionary<int, (string, int)>
	{
		{
			32,
			("bow", 1)
		},
		{
			33,
			("bow", 1)
		},
		{
			142,
			("bow", 1)
		},
		{
			34,
			("bow", 1)
		},
		{
			35,
			("bow", 1)
		},
		{
			143,
			("bow", 1)
		},
		{
			36,
			("bow", 1)
		},
		{
			37,
			("bow", 1)
		},
		{
			144,
			("bow", 1)
		},
		{
			38,
			("bow", 1)
		},
		{
			39,
			("bow", 1)
		},
		{
			145,
			("bow", 1)
		},
		{
			265,
			("bow", 1)
		},
		{
			40,
			("bow", 1)
		},
		{
			41,
			("bow", 1)
		},
		{
			146,
			("bow", 1)
		},
		{
			42,
			("bow", 1)
		},
		{
			43,
			("bow", 1)
		},
		{
			147,
			("bow", 1)
		},
		{
			44,
			("bow", 1)
		},
		{
			45,
			("bow", 1)
		},
		{
			148,
			("bow", 1)
		},
		{
			266,
			("bow", 1)
		},
		{
			756,
			("bow", 1)
		},
		{
			300,
			("bow", 1)
		},
		{
			403,
			("bow", 1)
		},
		{
			757,
			("bow", 1)
		},
		{
			267,
			("bow", 1)
		},
		{
			404,
			("bow", 1)
		},
		{
			4862,
			("bow", 1)
		},
		{
			4002,
			("bow", 1)
		},
		{
			353,
			("bow", 1)
		},
		{
			4905,
			("bow", 1)
		},
		{
			4003,
			("bow", 1)
		},
		{
			4903,
			("bow", 1)
		},
		{
			940,
			("bow", 1)
		},
		{
			4868,
			("bow", 1)
		},
		{
			354,
			("bow", 1)
		},
		{
			945,
			("bow", 1)
		},
		{
			4904,
			("bow", 1)
		},
		{
			4450,
			("bow", 1)
		},
		{
			4966,
			("bow", 1)
		},
		{
			4960,
			("bow", 1)
		},
		{
			4983,
			("bow", 1)
		},
		{
			4451,
			("bow", 1)
		},
		{
			4452,
			("bow", 1)
		},
		{
			4619,
			("bow", 1)
		},
		{
			4620,
			("bow", 1)
		},
		{
			8629,
			("bow", 1)
		},
		{
			8630,
			("bow", 1)
		},
		{
			1,
			("bat", 1)
		},
		{
			2,
			("bat", 1)
		},
		{
			3,
			("bat", 1)
		},
		{
			4,
			("bat", 1)
		},
		{
			5,
			("bat", 1)
		},
		{
			6,
			("bat", 1)
		},
		{
			7,
			("bat", 1)
		},
		{
			18,
			("sword", 1)
		},
		{
			19,
			("sword", 1)
		},
		{
			135,
			("sword", 1)
		},
		{
			20,
			("sword", 1)
		},
		{
			21,
			("sword", 1)
		},
		{
			136,
			("sword", 1)
		},
		{
			22,
			("sword", 1)
		},
		{
			23,
			("sword", 1)
		},
		{
			137,
			("sword", 1)
		},
		{
			24,
			("sword", 1)
		},
		{
			25,
			("sword", 1)
		},
		{
			138,
			("sword", 1)
		},
		{
			262,
			("sword", 1)
		},
		{
			26,
			("sword", 1)
		},
		{
			27,
			("sword", 1)
		},
		{
			139,
			("sword", 1)
		},
		{
			28,
			("sword", 1)
		},
		{
			29,
			("sword", 1)
		},
		{
			140,
			("sword", 1)
		},
		{
			30,
			("sword", 1)
		},
		{
			31,
			("sword", 1)
		},
		{
			141,
			("sword", 1)
		},
		{
			263,
			("sword", 1)
		},
		{
			754,
			("sword", 1)
		},
		{
			299,
			("sword", 1)
		},
		{
			400,
			("sword", 1)
		},
		{
			755,
			("sword", 1)
		},
		{
			264,
			("sword", 1)
		},
		{
			401,
			("sword", 1)
		},
		{
			4860,
			("sword", 1)
		},
		{
			4000,
			("sword", 1)
		},
		{
			4001,
			("sword", 1)
		},
		{
			4902,
			("sword", 1)
		},
		{
			349,
			("sword", 1)
		},
		{
			944,
			("sword", 1)
		},
		{
			199,
			("sword", 1)
		},
		{
			4900,
			("sword", 1)
		},
		{
			350,
			("sword", 1)
		},
		{
			4866,
			("sword", 1)
		},
		{
			4901,
			("sword", 1)
		},
		{
			4447,
			("sword", 1)
		},
		{
			4958,
			("sword", 1)
		},
		{
			4964,
			("sword", 1)
		},
		{
			4981,
			("sword", 1)
		},
		{
			4448,
			("sword", 1)
		},
		{
			4449,
			("sword", 1)
		},
		{
			4617,
			("sword", 1)
		},
		{
			4618,
			("sword", 1)
		},
		{
			8627,
			("sword", 1)
		},
		{
			8628,
			("sword", 1)
		},
		{
			46,
			("wand", 1)
		},
		{
			47,
			("wand", 1)
		},
		{
			149,
			("wand", 1)
		},
		{
			48,
			("wand", 1)
		},
		{
			49,
			("wand", 1)
		},
		{
			150,
			("wand", 1)
		},
		{
			50,
			("wand", 1)
		},
		{
			51,
			("wand", 1)
		},
		{
			151,
			("wand", 1)
		},
		{
			52,
			("wand", 1)
		},
		{
			53,
			("wand", 1)
		},
		{
			152,
			("wand", 1)
		},
		{
			268,
			("wand", 1)
		},
		{
			54,
			("wand", 1)
		},
		{
			55,
			("wand", 1)
		},
		{
			153,
			("wand", 1)
		},
		{
			56,
			("wand", 1)
		},
		{
			57,
			("wand", 1)
		},
		{
			154,
			("wand", 1)
		},
		{
			58,
			("wand", 1)
		},
		{
			59,
			("wand", 1)
		},
		{
			155,
			("wand", 1)
		},
		{
			269,
			("wand", 1)
		},
		{
			758,
			("wand", 1)
		},
		{
			301,
			("wand", 1)
		},
		{
			406,
			("wand", 1)
		},
		{
			759,
			("wand", 1)
		},
		{
			270,
			("wand", 1)
		},
		{
			407,
			("wand", 1)
		},
		{
			4858,
			("wand", 1)
		},
		{
			4004,
			("wand", 1)
		},
		{
			4328,
			("wand", 1)
		},
		{
			356,
			("wand", 1)
		},
		{
			4908,
			("wand", 1)
		},
		{
			4327,
			("wand", 1)
		},
		{
			4005,
			("wand", 1)
		},
		{
			4906,
			("wand", 1)
		},
		{
			941,
			("wand", 1)
		},
		{
			357,
			("wand", 1)
		},
		{
			4867,
			("wand", 1)
		},
		{
			946,
			("wand", 1)
		},
		{
			4907,
			("wand", 1)
		},
		{
			4453,
			("wand", 1)
		},
		{
			4965,
			("wand", 1)
		},
		{
			4959,
			("wand", 1)
		},
		{
			4982,
			("wand", 1)
		},
		{
			4454,
			("wand", 1)
		},
		{
			4455,
			("wand", 1)
		},
		{
			4621,
			("wand", 1)
		},
		{
			4622,
			("wand", 1)
		},
		{
			8631,
			("wand", 1)
		},
		{
			8632,
			("wand", 1)
		},
		{
			4720,
			("gauntlet", 1)
		},
		{
			4719,
			("gauntlet", 1)
		},
		{
			4722,
			("gauntlet", 1)
		},
		{
			4721,
			("gauntlet", 1)
		},
		{
			4724,
			("gauntlet", 1)
		},
		{
			4723,
			("gauntlet", 1)
		},
		{
			4729,
			("gauntlet", 1)
		},
		{
			4730,
			("gauntlet", 1)
		},
		{
			4726,
			("gauntlet", 1)
		},
		{
			4725,
			("gauntlet", 1)
		},
		{
			4727,
			("gauntlet", 1)
		},
		{
			4731,
			("gauntlet", 1)
		},
		{
			4728,
			("gauntlet", 1)
		},
		{
			4732,
			("gauntlet", 1)
		},
		{
			4733,
			("gauntlet", 1)
		},
		{
			4734,
			("gauntlet", 1)
		},
		{
			4456,
			("gauntlet", 1)
		},
		{
			4735,
			("gauntlet", 1)
		},
		{
			4736,
			("gauntlet", 1)
		},
		{
			4457,
			("gauntlet", 1)
		},
		{
			4458,
			("gauntlet", 1)
		},
		{
			4623,
			("gauntlet", 1)
		},
		{
			4624,
			("gauntlet", 1)
		},
		{
			8633,
			("gauntlet", 1)
		},
		{
			8634,
			("gauntlet", 1)
		},
		{
			8,
			("catapult", 2)
		},
		{
			9,
			("catapult", 2)
		},
		{
			10,
			("catapult", 2)
		},
		{
			11,
			("catapult", 2)
		},
		{
			68,
			("crossbow", 2)
		},
		{
			69,
			("crossbow", 2)
		},
		{
			156,
			("crossbow", 2)
		},
		{
			70,
			("crossbow", 2)
		},
		{
			73,
			("crossbow", 2)
		},
		{
			74,
			("crossbow", 2)
		},
		{
			291,
			("crossbow", 2)
		},
		{
			75,
			("crossbow", 2)
		},
		{
			157,
			("crossbow", 2)
		},
		{
			76,
			("crossbow", 2)
		},
		{
			77,
			("crossbow", 2)
		},
		{
			760,
			("crossbow", 2)
		},
		{
			292,
			("crossbow", 2)
		},
		{
			761,
			("crossbow", 2)
		},
		{
			402,
			("crossbow", 2)
		},
		{
			4006,
			("crossbow", 2)
		},
		{
			4861,
			("crossbow", 2)
		},
		{
			352,
			("crossbow", 2)
		},
		{
			4007,
			("crossbow", 2)
		},
		{
			4911,
			("crossbow", 2)
		},
		{
			4863,
			("crossbow", 2)
		},
		{
			947,
			("crossbow", 2)
		},
		{
			4909,
			("crossbow", 2)
		},
		{
			4910,
			("crossbow", 2)
		},
		{
			4459,
			("crossbow", 2)
		},
		{
			4961,
			("crossbow", 2)
		},
		{
			4955,
			("crossbow", 2)
		},
		{
			4978,
			("crossbow", 2)
		},
		{
			4460,
			("crossbow", 2)
		},
		{
			4465,
			("crossbow", 2)
		},
		{
			4625,
			("crossbow", 2)
		},
		{
			4626,
			("crossbow", 2)
		},
		{
			8635,
			("crossbow", 2)
		},
		{
			8636,
			("crossbow", 2)
		},
		{
			78,
			("dagger", 2)
		},
		{
			79,
			("dagger", 2)
		},
		{
			158,
			("dagger", 2)
		},
		{
			80,
			("dagger", 2)
		},
		{
			81,
			("dagger", 2)
		},
		{
			82,
			("dagger", 2)
		},
		{
			289,
			("dagger", 2)
		},
		{
			83,
			("dagger", 2)
		},
		{
			159,
			("dagger", 2)
		},
		{
			84,
			("dagger", 2)
		},
		{
			85,
			("dagger", 2)
		},
		{
			762,
			("dagger", 2)
		},
		{
			290,
			("dagger", 2)
		},
		{
			763,
			("dagger", 2)
		},
		{
			405,
			("dagger", 2)
		},
		{
			4008,
			("dagger", 2)
		},
		{
			4857,
			("dagger", 2)
		},
		{
			4914,
			("dagger", 2)
		},
		{
			351,
			("dagger", 2)
		},
		{
			4009,
			("dagger", 2)
		},
		{
			4912,
			("dagger", 2)
		},
		{
			4865,
			("dagger", 2)
		},
		{
			948,
			("dagger", 2)
		},
		{
			4913,
			("dagger", 2)
		},
		{
			4466,
			("dagger", 2)
		},
		{
			4963,
			("dagger", 2)
		},
		{
			4957,
			("dagger", 2)
		},
		{
			4980,
			("dagger", 2)
		},
		{
			4467,
			("dagger", 2)
		},
		{
			4468,
			("dagger", 2)
		},
		{
			4627,
			("dagger", 2)
		},
		{
			4628,
			("dagger", 2)
		},
		{
			8637,
			("dagger", 2)
		},
		{
			8638,
			("dagger", 2)
		},
		{
			86,
			("spellgun", 2)
		},
		{
			87,
			("spellgun", 2)
		},
		{
			160,
			("spellgun", 2)
		},
		{
			88,
			("spellgun", 2)
		},
		{
			89,
			("spellgun", 2)
		},
		{
			90,
			("spellgun", 2)
		},
		{
			293,
			("spellgun", 2)
		},
		{
			91,
			("spellgun", 2)
		},
		{
			161,
			("spellgun", 2)
		},
		{
			92,
			("spellgun", 2)
		},
		{
			93,
			("spellgun", 2)
		},
		{
			764,
			("spellgun", 2)
		},
		{
			294,
			("spellgun", 2)
		},
		{
			765,
			("spellgun", 2)
		},
		{
			408,
			("spellgun", 2)
		},
		{
			4010,
			("spellgun", 2)
		},
		{
			4859,
			("spellgun", 2)
		},
		{
			4917,
			("spellgun", 2)
		},
		{
			355,
			("spellgun", 2)
		},
		{
			4011,
			("spellgun", 2)
		},
		{
			4864,
			("spellgun", 2)
		},
		{
			949,
			("spellgun", 2)
		},
		{
			4915,
			("spellgun", 2)
		},
		{
			4916,
			("spellgun", 2)
		},
		{
			4469,
			("spellgun", 2)
		},
		{
			4962,
			("spellgun", 2)
		},
		{
			4956,
			("spellgun", 2)
		},
		{
			4979,
			("spellgun", 2)
		},
		{
			4470,
			("spellgun", 2)
		},
		{
			4471,
			("spellgun", 2)
		},
		{
			4629,
			("spellgun", 2)
		},
		{
			4630,
			("spellgun", 2)
		},
		{
			8639,
			("spellgun", 2)
		},
		{
			8640,
			("spellgun", 2)
		},
		{
			4759,
			("token", 2)
		},
		{
			4999,
			("token", 2)
		},
		{
			4758,
			("token", 2)
		},
		{
			4760,
			("token", 2)
		},
		{
			4761,
			("token", 2)
		},
		{
			4764,
			("token", 2)
		},
		{
			4762,
			("token", 2)
		},
		{
			4765,
			("token", 2)
		},
		{
			4763,
			("token", 2)
		},
		{
			4766,
			("token", 2)
		},
		{
			4472,
			("token", 2)
		},
		{
			4767,
			("token", 2)
		},
		{
			4769,
			("token", 2)
		},
		{
			4768,
			("token", 2)
		},
		{
			4770,
			("token", 2)
		},
		{
			4473,
			("token", 2)
		},
		{
			4474,
			("token", 2)
		},
		{
			4631,
			("token", 2)
		},
		{
			4632,
			("token", 2)
		},
		{
			8641,
			("token", 2)
		},
		{
			8642,
			("token", 2)
		},
		{
			12,
			("cloth", 0)
		},
		{
			13,
			("cloth", 0)
		},
		{
			14,
			("cloth", 0)
		},
		{
			15,
			("cloth", 0)
		},
		{
			16,
			("cloth", 0)
		},
		{
			17,
			("cloth", 0)
		},
		{
			94,
			("armor", 0)
		},
		{
			95,
			("armor", 0)
		},
		{
			162,
			("armor", 0)
		},
		{
			96,
			("armor", 0)
		},
		{
			97,
			("armor", 0)
		},
		{
			163,
			("armor", 0)
		},
		{
			98,
			("armor", 0)
		},
		{
			99,
			("armor", 0)
		},
		{
			164,
			("armor", 0)
		},
		{
			100,
			("armor", 0)
		},
		{
			101,
			("armor", 0)
		},
		{
			165,
			("armor", 0)
		},
		{
			102,
			("armor", 0)
		},
		{
			103,
			("armor", 0)
		},
		{
			166,
			("armor", 0)
		},
		{
			297,
			("armor", 0)
		},
		{
			104,
			("armor", 0)
		},
		{
			105,
			("armor", 0)
		},
		{
			167,
			("armor", 0)
		},
		{
			106,
			("armor", 0)
		},
		{
			766,
			("armor", 0)
		},
		{
			298,
			("armor", 0)
		},
		{
			994,
			("armor", 0)
		},
		{
			767,
			("armor", 0)
		},
		{
			409,
			("armor", 0)
		},
		{
			4851,
			("armor", 0)
		},
		{
			4012,
			("armor", 0)
		},
		{
			4920,
			("armor", 0)
		},
		{
			4013,
			("armor", 0)
		},
		{
			4854,
			("armor", 0)
		},
		{
			4014,
			("armor", 0)
		},
		{
			4918,
			("armor", 0)
		},
		{
			4919,
			("armor", 0)
		},
		{
			4475,
			("armor", 0)
		},
		{
			4949,
			("armor", 0)
		},
		{
			4952,
			("armor", 0)
		},
		{
			4984,
			("armor", 0)
		},
		{
			4476,
			("armor", 0)
		},
		{
			4477,
			("armor", 0)
		},
		{
			4633,
			("armor", 0)
		},
		{
			4634,
			("armor", 0)
		},
		{
			8643,
			("armor", 0)
		},
		{
			8644,
			("armor", 0)
		},
		{
			107,
			("tunic", 0)
		},
		{
			108,
			("tunic", 0)
		},
		{
			168,
			("tunic", 0)
		},
		{
			109,
			("tunic", 0)
		},
		{
			110,
			("tunic", 0)
		},
		{
			169,
			("tunic", 0)
		},
		{
			111,
			("tunic", 0)
		},
		{
			112,
			("tunic", 0)
		},
		{
			170,
			("tunic", 0)
		},
		{
			113,
			("tunic", 0)
		},
		{
			114,
			("tunic", 0)
		},
		{
			171,
			("tunic", 0)
		},
		{
			115,
			("tunic", 0)
		},
		{
			116,
			("tunic", 0)
		},
		{
			172,
			("tunic", 0)
		},
		{
			295,
			("tunic", 0)
		},
		{
			117,
			("tunic", 0)
		},
		{
			118,
			("tunic", 0)
		},
		{
			173,
			("tunic", 0)
		},
		{
			119,
			("tunic", 0)
		},
		{
			768,
			("tunic", 0)
		},
		{
			296,
			("tunic", 0)
		},
		{
			993,
			("tunic", 0)
		},
		{
			769,
			("tunic", 0)
		},
		{
			410,
			("tunic", 0)
		},
		{
			4852,
			("tunic", 0)
		},
		{
			4016,
			("tunic", 0)
		},
		{
			4015,
			("tunic", 0)
		},
		{
			4923,
			("tunic", 0)
		},
		{
			4856,
			("tunic", 0)
		},
		{
			4017,
			("tunic", 0)
		},
		{
			4921,
			("tunic", 0)
		},
		{
			4922,
			("tunic", 0)
		},
		{
			4478,
			("tunic", 0)
		},
		{
			4951,
			("tunic", 0)
		},
		{
			4954,
			("tunic", 0)
		},
		{
			4986,
			("tunic", 0)
		},
		{
			4479,
			("tunic", 0)
		},
		{
			4480,
			("tunic", 0)
		},
		{
			4635,
			("tunic", 0)
		},
		{
			4636,
			("tunic", 0)
		},
		{
			8645,
			("tunic", 0)
		},
		{
			8646,
			("tunic", 0)
		},
		{
			120,
			("robe", 0)
		},
		{
			121,
			("robe", 0)
		},
		{
			174,
			("robe", 0)
		},
		{
			122,
			("robe", 0)
		},
		{
			123,
			("robe", 0)
		},
		{
			175,
			("robe", 0)
		},
		{
			124,
			("robe", 0)
		},
		{
			125,
			("robe", 0)
		},
		{
			176,
			("robe", 0)
		},
		{
			126,
			("robe", 0)
		},
		{
			127,
			("robe", 0)
		},
		{
			177,
			("robe", 0)
		},
		{
			128,
			("robe", 0)
		},
		{
			129,
			("robe", 0)
		},
		{
			178,
			("robe", 0)
		},
		{
			271,
			("robe", 0)
		},
		{
			130,
			("robe", 0)
		},
		{
			131,
			("robe", 0)
		},
		{
			179,
			("robe", 0)
		},
		{
			132,
			("robe", 0)
		},
		{
			770,
			("robe", 0)
		},
		{
			272,
			("robe", 0)
		},
		{
			989,
			("robe", 0)
		},
		{
			771,
			("robe", 0)
		},
		{
			411,
			("robe", 0)
		},
		{
			4853,
			("robe", 0)
		},
		{
			4019,
			("robe", 0)
		},
		{
			4018,
			("robe", 0)
		},
		{
			4926,
			("robe", 0)
		},
		{
			4855,
			("robe", 0)
		},
		{
			4020,
			("robe", 0)
		},
		{
			4924,
			("robe", 0)
		},
		{
			4925,
			("robe", 0)
		},
		{
			4481,
			("robe", 0)
		},
		{
			4950,
			("robe", 0)
		},
		{
			4953,
			("robe", 0)
		},
		{
			4985,
			("robe", 0)
		},
		{
			4482,
			("robe", 0)
		},
		{
			4483,
			("robe", 0)
		},
		{
			4637,
			("robe", 0)
		},
		{
			4638,
			("robe", 0)
		},
		{
			8647,
			("robe", 0)
		},
		{
			8648,
			("robe", 0)
		},
		{
			4737,
			("martial_armor", 0)
		},
		{
			4738,
			("martial_armor", 0)
		},
		{
			4740,
			("martial_armor", 0)
		},
		{
			4739,
			("martial_armor", 0)
		},
		{
			4747,
			("martial_armor", 0)
		},
		{
			4742,
			("martial_armor", 0)
		},
		{
			4748,
			("martial_armor", 0)
		},
		{
			4741,
			("martial_armor", 0)
		},
		{
			4744,
			("martial_armor", 0)
		},
		{
			4743,
			("martial_armor", 0)
		},
		{
			4749,
			("martial_armor", 0)
		},
		{
			4750,
			("martial_armor", 0)
		},
		{
			4746,
			("martial_armor", 0)
		},
		{
			4745,
			("martial_armor", 0)
		},
		{
			4751,
			("martial_armor", 0)
		},
		{
			4752,
			("martial_armor", 0)
		},
		{
			4484,
			("martial_armor", 0)
		},
		{
			4753,
			("martial_armor", 0)
		},
		{
			4754,
			("martial_armor", 0)
		},
		{
			4505,
			("martial_armor", 0)
		},
		{
			4506,
			("martial_armor", 0)
		},
		{
			4639,
			("martial_armor", 0)
		},
		{
			4640,
			("martial_armor", 0)
		},
		{
			8649,
			("martial_armor", 0)
		},
		{
			8650,
			("martial_armor", 0)
		}
	};

	public string unique_id { get; set; } = "";


	public int server_id { get; set; }

	public int item_id { get; set; }

	public int type { get; set; }

	public int owner_id { get; set; }

	public int upgrade { get; set; }

	public int rarity { get; set; }

	public string? shell_string { get; set; }

	public string? rune_string { get; set; }

	public int value1 { get; set; }

	public int value2 { get; set; }

	public int? value3 { get; set; }

	public int utility_value { get; set; }

	public GameEquipementItem()
	{
	}

	public GameEquipementItem(string item_string, int server, int sender_id)
	{
		List<string> list = item_string.Split(" ").ToList();
		int num = Convert.ToInt32(list.ElementAt(1));
		if (num != 1 && num != 2 && num != 5 && num != 0)
		{
			throw new Exception("Not implemented item type parse in GameEqupementItem");
		}
		if (isCostume(item_string))
		{
			throw new Exception("GameEqupementItem received a costume");
		}
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int? num5 = null;
		int num6 = 0;
		int num7 = Convert.ToInt32(list.ElementAt(2));
		if (!item_types.ContainsKey(num7))
		{
			return;
		}
		int num8 = Convert.ToInt32(list.ElementAt(3));
		int num9 = Convert.ToInt32(list.ElementAt(4));
		Convert.ToInt32(list.ElementAt(6));
		if (num == 1 || num == 5 || num == 0)
		{
			num3 = Convert.ToInt32(list.ElementAt(7));
			num4 = Convert.ToInt32(list.ElementAt(8));
			num6 = Convert.ToInt32(list.ElementAt(9));
		}
		if (num == 2)
		{
			num3 = Convert.ToInt32(list.ElementAt(7));
			num4 = Convert.ToInt32(list.ElementAt(8));
			num5 = Convert.ToInt32(list.ElementAt(9));
			num6 = Convert.ToInt32(list.ElementAt(10));
			num2 = 3;
		}
		int num10 = Convert.ToInt32(list.ElementAt(17 - num2));
		if (num10 == 0)
		{
			num10 = sender_id;
		}
		string value = null;
		string text = null;
		int num11 = Convert.ToInt32(list.ElementAt(18 - num2));
		if (num11 != 0)
		{
			value = "";
			for (int i = 19 - num2; i < 19 - num2 + num11; i++)
			{
				value = value + list.ElementAt(i) + " ";
			}
			value = value.TrimEnd();
		}
		int num12 = 19 - num2 + num11;
		if (list.Count > num12)
		{
			int num13 = Convert.ToInt32(list.ElementAt(num12 + 2));
			if (num13 == 0)
			{
				text = null;
			}
			else
			{
				for (int j = num12 + 3; j < num12 + 3 + num13; j++)
				{
					text = text + list.ElementAt(j) + " ";
				}
				text = text.TrimEnd();
			}
		}
		unique_id = $"SendEquipementItemEvent_{num7}_{num10}_{value}";
		item_id = num7;
		type = item_types[num7].Item2;
		rarity = num8;
		upgrade = num9;
		value1 = num3;
		value2 = num4;
		value3 = num5;
		utility_value = num6;
		owner_id = num10;
		shell_string = value;
		rune_string = text;
		server_id = server;
	}

	public static bool isCostume(string item_string)
	{
		List<string> source = item_string.Split(" ").ToList();
		if (Convert.ToInt32(source.ElementAt(1)) != 2)
		{
			return false;
		}
		int num = Convert.ToInt32(source.ElementAt(7));
		int num2 = Convert.ToInt32(source.ElementAt(8));
		int num3 = Convert.ToInt32(source.ElementAt(9));
		if (num == 0 && num2 == 0 && num3 == 0)
		{
			return true;
		}
		return false;
	}
}
