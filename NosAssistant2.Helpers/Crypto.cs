using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using NosAssistant2.GameObjects;

namespace NosAssistant2.Helpers;

public static class Crypto
{
	public static string ReadNickFromMemory(int processId)
	{
		List<int> list = new List<int>();
		CollectionsMarshal.SetCount(list, 2);
		Span<int> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = 5210584;
		num++;
		span[num] = 268;
		num++;
		string text = ReadStringFromMemory(processId, 14, list).Trim();
		if (!(text == "Entwell") && !(text == ""))
		{
			return text;
		}
		return "undefined";
	}

	public static string ReadFamilyFromMemory(int processId)
	{
		List<int> list = new List<int>();
		CollectionsMarshal.SetCount(list, 2);
		Span<int> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = 3554988;
		num++;
		span[num] = 504;
		num++;
		string text = ReadStringFromMemory(processId, 30, list).Trim();
		if (!(text == ""))
		{
			if (!text.Contains("("))
			{
				return "-";
			}
			return text.Substring(0, text.LastIndexOf("("));
		}
		return "undefined";
	}

	public static List<HotBarElement> ReadCurrentHotbarFromMemory(int processId)
	{
		List<HotBarElement> list = new List<HotBarElement>();
		for (int i = 0; i < 30; i++)
		{
			var (slotValue, isItem) = ReadHotbarSlotFromMemory(processId, i);
			list.Add(new HotBarElement
			{
				HotBarSlotID = i,
				SlotValue = slotValue,
				isItem = isItem
			});
		}
		return list;
	}

	public static (int, bool) ReadHotbarSlotFromMemory(int processId, int slotID)
	{
		List<int> list = new List<int>();
		CollectionsMarshal.SetCount(list, 5);
		Span<int> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = 5209892;
		num++;
		span[num] = 196 + slotID * 4;
		num++;
		span[num] = 168;
		num++;
		span[num] = 8;
		num++;
		span[num] = 0;
		num++;
		List<int> list2 = list;
		List<int> list3 = list2.ToList();
		list3[list3.Count - 1] += 4;
		return (ReadIntFromMemory(processId, list2), ReadIntFromMemory(processId, list3) == 0);
	}

	public static int ReadInRaidFromMemory(int processId)
	{
		List<int> list = new List<int>();
		CollectionsMarshal.SetCount(list, 4);
		Span<int> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = 5197412;
		num++;
		span[num] = 472;
		num++;
		span[num] = 452;
		num++;
		span[num] = 24;
		num++;
		int num2 = ReadIntFromMemory(processId, list);
		List<int> list2 = new List<int>();
		CollectionsMarshal.SetCount(list2, 4);
		span = CollectionsMarshal.AsSpan(list2);
		num = 0;
		span[num] = 5197628;
		num++;
		span[num] = 196;
		num++;
		span[num] = 136;
		num++;
		span[num] = 916;
		num++;
		return num2 ^ ReadIntFromMemory(processId, list2);
	}

	public static int ReadSPFromMemory(int processId)
	{
		List<int> list = new List<int>();
		CollectionsMarshal.SetCount(list, 2);
		Span<int> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = 3554988;
		num++;
		span[num] = 432;
		num++;
		return ReadIntFromMemory(processId, list);
	}

	public static int ReadMapIdFromMemory(int processId)
	{
		List<int> list = new List<int>();
		CollectionsMarshal.SetCount(list, 1);
		Span<int> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = 3559432;
		num++;
		return ReadIntFromMemory(processId, list);
	}

	public static int ReadEncryptionKeyFromMemory(int processId)
	{
		List<int> list = new List<int>();
		CollectionsMarshal.SetCount(list, 2);
		Span<int> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = 5210092;
		num++;
		span[num] = 72;
		num++;
		return ReadIntFromMemory(processId, list);
	}

	public static int ReadCharacterIdFromMemory(int processId)
	{
		List<int> list = new List<int>();
		CollectionsMarshal.SetCount(list, 2);
		Span<int> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = 5208464;
		num++;
		span[num] = 36;
		num++;
		return ReadIntFromMemory(processId, list);
	}

	public static int ReadClassIdFromMemory(int processId)
	{
		List<int> list = new List<int>();
		CollectionsMarshal.SetCount(list, 2);
		Span<int> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = 5209276;
		num++;
		span[num] = 304;
		num++;
		return ReadIntFromMemory(processId, list);
	}

	public static int ReadSexIdFromMemory(int processId)
	{
		List<int> list = new List<int>();
		CollectionsMarshal.SetCount(list, 2);
		Span<int> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = 5209140;
		num++;
		span[num] = 260;
		num++;
		return ReadIntFromMemory(processId, list);
	}

	public static int ReadTSStoneCountFromMemory(int processId)
	{
		List<int> list = new List<int>();
		CollectionsMarshal.SetCount(list, 2);
		Span<int> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = 5209272;
		num++;
		span[num] = 72;
		num++;
		return ReadIntFromMemory(processId, list) % 256;
	}

	private static int ReadIntFromMemory(int processId, List<int> offsets)
	{
		try
		{
			Process processById = Process.GetProcessById(processId);
			nint num = processById.MainModule?.BaseAddress ?? IntPtr.Zero;
			foreach (int offset in offsets)
			{
				byte[] array = new byte[IntPtr.Size];
				nint lpBaseAddress = IntPtr.Add(num, offset);
				DllImports.ReadProcessMemory(processById.Handle, lpBaseAddress, array, array.Length, out var _);
				num = BitConverter.ToInt32(array, 0);
			}
			return Convert.ToInt32(num);
		}
		catch (Exception exception)
		{
			NALogger.LogExceptionToFile(exception);
			return 0;
		}
	}

	private static string ReadStringFromMemory(int processId, int stringSize, List<int> offsets)
	{
		Process processById = Process.GetProcessById(processId);
		nint num = processById.MainModule?.BaseAddress ?? IntPtr.Zero;
		int lpNumberOfBytesRead;
		foreach (int offset in offsets)
		{
			byte[] array = new byte[IntPtr.Size];
			nint lpBaseAddress = IntPtr.Add(num, offset);
			DllImports.ReadProcessMemory(processById.Handle, lpBaseAddress, array, array.Length, out lpNumberOfBytesRead);
			num = BitConverter.ToInt32(array, 0);
		}
		nint lpBaseAddress2 = num;
		byte[] array2 = new byte[stringSize];
		DllImports.ReadProcessMemory(processById.Handle, lpBaseAddress2, array2, array2.Length, out lpNumberOfBytesRead);
		string text = FormatGameString(Encoding.Latin1.GetString(array2));
		if (text.Contains('\0'))
		{
			text = text.Remove(text.IndexOf('\0'));
		}
		return text;
	}

	public static void GetPatternAddress(int processId)
	{
		(byte[] pattern, string mask) tuple = GeneratePatternAndMask("a1 ? ? ? ? 3b c1 8b 35");
		byte[] item = tuple.pattern;
		string item2 = tuple.mask;
		Process processById = Process.GetProcessById(processId);
		nint baseAddress = processById.MainModule?.BaseAddress ?? IntPtr.Zero;
		int regionSize = processById.MainModule?.ModuleMemorySize ?? 0;
		nint num = FindPattern(processById.Handle, baseAddress, regionSize, item, item2);
		byte[] array = new byte[4];
		if (num != IntPtr.Zero)
		{
			DllImports.ReadProcessMemory(processById.Handle, num + 1, array, array.Length, out var _);
		}
	}

	private static (byte[] pattern, string mask) GeneratePatternAndMask(string hexString)
	{
		string[] array = hexString.Split(' ');
		List<byte> list = new List<byte>();
		List<char> list2 = new List<char>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text == "?")
			{
				list.Add(0);
				list2.Add('?');
			}
			else
			{
				list.Add(Convert.ToByte(text, 16));
				list2.Add('x');
			}
		}
		return (pattern: list.ToArray(), mask: new string(list2.ToArray()));
	}

	public static List<int> ReadInventoryFromMemory(int processId)
	{
		Process.GetProcessById(processId);
		List<int> list = new List<int>();
		CollectionsMarshal.SetCount(list, 5);
		Span<int> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = 3507756;
		num++;
		span[num] = 232;
		num++;
		span[num] = 168;
		num++;
		span[num] = 8;
		num++;
		span[num] = 48;
		num++;
		List<int> list2 = list;
		for (int i = 0; i < 60; i++)
		{
			List<int> list3 = new List<int>();
			CollectionsMarshal.SetCount(list3, 5);
			span = CollectionsMarshal.AsSpan(list3);
			num = 0;
			span[num] = 3507756;
			num++;
			span[num] = 232 + i * 4;
			num++;
			span[num] = 168;
			num++;
			span[num] = 8;
			num++;
			span[num] = 48;
			num++;
			list2 = list3;
			ReadIntFromMemory(processId, list2);
		}
		return new List<int>();
	}

	private static nint FindPattern(nint processHandle, nint baseAddress, int regionSize, byte[] pattern, string mask)
	{
		uint length = (uint)mask.Length;
		byte[] array = new byte[length];
		for (uint num = 0u; num < regionSize; num++)
		{
			DllImports.ReadProcessMemory(processHandle, (nint)(baseAddress + num), array, (int)length, out var _);
			bool flag = true;
			for (uint num2 = 0u; num2 < length; num2++)
			{
				if (mask[(int)num2] != '?' && array[num2] != pattern[num2])
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return (nint)(baseAddress + num);
			}
		}
		return IntPtr.Zero;
	}

	public static int FormatNostaleSpId(int nostale_sp_id)
	{
		switch (nostale_sp_id)
		{
		case 65535:
			return 0;
		case 564:
			return 1564;
		default:
			if (nostale_sp_id % 256 == 0)
			{
				return nostale_sp_id / 256;
			}
			if ((nostale_sp_id - 128) % 256 == 0)
			{
				return (nostale_sp_id - 128) / 256;
			}
			if (nostale_sp_id >= 2767)
			{
				return nostale_sp_id + 957;
			}
			if (nostale_sp_id >= 2748)
			{
				return nostale_sp_id + 969;
			}
			if (nostale_sp_id > 800)
			{
				return nostale_sp_id + 1000;
			}
			return 1000000 + nostale_sp_id;
		}
	}

	public static string FormatGameString(string s)
	{
		return s.Replace('¹', 'ą').Replace('æ', 'ć').Replace('ê', 'ę')
			.Replace('³', 'ł')
			.Replace('ñ', 'ń')
			.Replace('ó', 'ó')
			.Replace('\u009c', 'ś')
			.Replace('\u009f', 'ź')
			.Replace('¿', 'ż')
			.Replace('¥', 'Ą')
			.Replace('Æ', 'Ć')
			.Replace('Ê', 'Ę')
			.Replace('£', 'Ł')
			.Replace('Ñ', 'Ń')
			.Replace('Ó', 'Ó')
			.Replace('\u008c', 'Ś')
			.Replace('\u008f', 'Ź')
			.Replace('\u00af', 'Ż')
			.Replace('\u0086', '†')
			.Replace('\u0092', '’')
			.Replace('\u0094', '″')
			.Replace('\u0095', '•')
			.Replace('\u0099', '™');
	}

	public static string NormalizeRecvPacket(byte[] packet)
	{
		return FormatGameString(Encoding.Latin1.GetString(packet)).Trim();
	}

	public static byte[] DecryptRecvPacket(byte[] packet)
	{
		byte[] array = new byte[16]
		{
			0, 32, 45, 46, 48, 49, 50, 51, 52, 53,
			54, 55, 56, 57, 10, 0
		};
		List<byte> list = new List<byte>();
		int num = 0;
		while (packet.Length > num)
		{
			int num2 = packet[num] & 0x7F;
			int num3 = packet[num] & 0x80;
			num++;
			if (num3 != 0)
			{
				for (int i = 0; (double)i < Math.Ceiling((double)num2 / 2.0); i++)
				{
					if (num >= packet.Length)
					{
						break;
					}
					byte num4 = packet[num];
					num++;
					byte b = (byte)(num4 >> 4);
					list.Add(array[b]);
					byte b2 = (byte)(num4 & 0xFu);
					if (b2 == 0)
					{
						break;
					}
					list.Add(array[b2]);
				}
				continue;
			}
			for (int j = 0; j < num2; j++)
			{
				if (num >= packet.Length)
				{
					break;
				}
				list.Add((byte)(packet[num] ^ 0xFFu));
				num++;
			}
		}
		return list.ToArray();
	}

	public static string DecryptSentPacket(in ReadOnlySpan<byte> str, int encryptionKey)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = encryptionKey & 0xFF;
		byte b = (byte)(encryptionKey >> 6);
		b = (byte)(b & 0xFFu);
		switch ((byte)(b & 3))
		{
		case 0:
		{
			ReadOnlySpan<byte> readOnlySpan = str;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				byte num5 = readOnlySpan[i];
				byte b5 = (byte)(num + 64);
				byte value4 = (byte)(num5 - b5);
				stringBuilder.Append((char)value4);
			}
			break;
		}
		case 1:
		{
			ReadOnlySpan<byte> readOnlySpan = str;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				byte num3 = readOnlySpan[i];
				byte b3 = (byte)(num + 64);
				byte value2 = (byte)(num3 + b3);
				stringBuilder.Append((char)value2);
			}
			break;
		}
		case 2:
		{
			ReadOnlySpan<byte> readOnlySpan = str;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				byte num4 = readOnlySpan[i];
				byte b4 = (byte)(num + 64);
				byte value3 = (byte)((uint)(num4 - b4) ^ 0xC3u);
				stringBuilder.Append((char)value3);
			}
			break;
		}
		case 3:
		{
			ReadOnlySpan<byte> readOnlySpan = str;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				byte num2 = readOnlySpan[i];
				byte b2 = (byte)(num + 64);
				byte value = (byte)((uint)(num2 + b2) ^ 0xC3u);
				stringBuilder.Append((char)value);
			}
			break;
		}
		default:
			stringBuilder.Append('\u000f');
			break;
		}
		string[] array = stringBuilder.ToString().Split('ÿ');
		StringBuilder stringBuilder2 = new StringBuilder();
		for (int j = 0; j < array.Length; j++)
		{
			ReadOnlySpan<char> str2 = array[j].AsSpan();
			stringBuilder2.Append(DecryptSentPrivate(in str2));
			if (j < array.Length - 2)
			{
				stringBuilder2.Append('\n');
			}
		}
		return FormatGameString(stringBuilder2.ToString());
	}

	public static List<NosPacket> DecryptPayload(NosPayload nosPayload)
	{
		List<NosPacket> list = new List<NosPacket>();
		if (nosPayload.type == "RECV")
		{
			byte[] array = DecryptRecvPacket(nosPayload.payload);
			string text = NormalizeRecvPacket(array);
			if (array.Length != 0 && text != "")
			{
				string[] array2 = text.Split(" ");
				List<string> list2 = new List<string>();
				CollectionsMarshal.SetCount(list2, array2.Length);
				Span<string> span = CollectionsMarshal.AsSpan(list2);
				int num = 0;
				Span<string> span2 = new Span<string>(array2);
				span2.CopyTo(span.Slice(num, span2.Length));
				num += span2.Length;
				List<string> list3 = list2;
				if (list3.Count > 0)
				{
					NosPacket item = new NosPacket
					{
						type = "RECV",
						date = nosPayload.arrival_date,
						process_id = nosPayload.process_id,
						packet_type = list3.ElementAt(0),
						packet_splitted = list3,
						content = text,
						hwnd = nosPayload.hwnd
					};
					list.Add(item);
				}
			}
		}
		else if (nosPayload.type == "SENT")
		{
			ReadOnlySpan<byte> str = nosPayload.payload;
			string[] array2 = DecryptSentPacket(in str, nosPayload.encryption_key).Split("\n");
			foreach (string obj in array2)
			{
				string text2 = "";
				string[] array3 = obj.Split(" ");
				if (array3.Length != 0)
				{
					string[] value = array3.Skip(1).ToArray();
					text2 = string.Join(" ", value);
				}
				if (text2 != "")
				{
					string[] array4 = text2.Split(" ");
					List<string> list4 = new List<string>();
					CollectionsMarshal.SetCount(list4, array4.Length);
					Span<string> span2 = CollectionsMarshal.AsSpan(list4);
					int num2 = 0;
					Span<string> span = new Span<string>(array4);
					span.CopyTo(span2.Slice(num2, span.Length));
					num2 += span.Length;
					List<string> list5 = list4;
					if (list5.Count > 0)
					{
						NosPacket item2 = new NosPacket
						{
							type = "SENT",
							date = nosPayload.arrival_date,
							process_id = nosPayload.process_id,
							packet_type = list5.ElementAt(0),
							packet_splitted = list5,
							content = text2,
							hwnd = nosPayload.hwnd
						};
						list.Add(item2);
					}
				}
			}
		}
		return list;
	}

	private static string DecryptSentPrivate(in ReadOnlySpan<char> str)
	{
		using MemoryStream memoryStream = new MemoryStream();
		char[] array = new char[14]
		{
			' ', '-', '.', '0', '1', '2', '3', '4', '5', '6',
			'7', '8', '9', '\n'
		};
		for (int i = 0; i < str.Length; i++)
		{
			if (str[i] <= 'z')
			{
				int num = str[i];
				for (int j = 0; j < num; j++)
				{
					i++;
					if (i < str.Length)
					{
						memoryStream.WriteByte((byte)(str[i] ^ 0xFFu));
					}
					else
					{
						memoryStream.WriteByte(byte.MaxValue);
					}
				}
				continue;
			}
			int num2 = str[i];
			num2 &= 0x7F;
			for (int k = 0; k < num2; k++)
			{
				i++;
				int num3 = ((i < str.Length) ? str[i] : '\0');
				num3 &= 0xF0;
				num3 >>= 4;
				int num4 = ((i < str.Length) ? str[i] : '\0');
				num4 &= 0xF;
				if (num3 != 0 && num3 != 15)
				{
					memoryStream.WriteByte((byte)array[num3 - 1]);
					k++;
				}
				if (num4 != 0 && num4 != 15)
				{
					memoryStream.WriteByte((byte)array[num4 - 1]);
				}
			}
		}
		return Encoding.Latin1.GetString(memoryStream.ToArray());
	}

	public static void ReadFromMemoryTest(int pid)
	{
		using StreamWriter streamWriter = new StreamWriter("test.txt", append: true);
		streamWriter.WriteLine($"Decrypt key: {ReadEncryptionKeyFromMemory(pid)}");
		streamWriter.WriteLine("Nickname: " + ReadNickFromMemory(pid));
		streamWriter.WriteLine($"Class: {ReadClassIdFromMemory(pid)}");
		streamWriter.WriteLine($"Sex: {ReadSexIdFromMemory(pid)}");
		streamWriter.WriteLine("Family: " + ReadFamilyFromMemory(pid));
		streamWriter.WriteLine($"SP: {ReadSPFromMemory(pid)}");
		streamWriter.WriteLine($"Char ID: {ReadCharacterIdFromMemory(pid)}");
		streamWriter.WriteLine($"Map: {ReadMapIdFromMemory(pid)}");
		streamWriter.WriteLine($"TSStones: {ReadTSStoneCountFromMemory(pid)}");
		streamWriter.WriteLine("Hotbar: " + JsonConvert.SerializeObject(ReadCurrentHotbarFromMemory(pid)));
		streamWriter.WriteLine("\n\n");
	}
}
