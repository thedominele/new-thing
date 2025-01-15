using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using NosAssistant2.Configs;
using NosAssistant2.Dtos.Input;
using NosAssistant2.Dtos.Output;
using NosAssistant2.EnumsID;
using NosAssistant2.GameData;
using NosAssistant2.GameObjects;
using NosAssistant2.GUIElements;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace NosAssistant2.Helpers;

public static class PacketsManager
{
	private static readonly ConcurrentQueue<NosPayload> _payloadsQueue = new ConcurrentQueue<NosPayload>();

	public static LibPcapLiveDevice? _device;

	private static Dictionary<nint, List<byte>> lastsDict = new Dictionary<nint, List<byte>>();

	private static List<NosPacket> new_packets = new List<NosPacket>();

	private static bool is_last_packet_rdlstf = false;

	private static NosPacket? last_rdlstf_packet = null;

	public static bool start_collecting = false;

	public static List<string> processed_packets = new List<string>();

	public static Dictionary<nint, byte[]> payloadsDict = new Dictionary<nint, byte[]>();

	public static int packets_corrupted_count = 0;

	public static DateTime last_status_bar_event_set = DateTime.UtcNow;

	public static bool UpdateNostaleCharacterInfosList()
	{
		List<NostaleCharacterInfo> nostaleCharacterInfoList = GUI._nostaleCharacterInfoList;
		List<NostaleCharacterInfo> list = new List<NostaleCharacterInfo>();
		CollectionsMarshal.SetCount(list, nostaleCharacterInfoList.Count);
		Span<NostaleCharacterInfo> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		Span<NostaleCharacterInfo> span2 = CollectionsMarshal.AsSpan(nostaleCharacterInfoList);
		span2.CopyTo(span.Slice(num, span2.Length));
		num += span2.Length;
		List<NostaleCharacterInfo> list2 = list;
		List<NostaleCharacterInfo> nostaleNetworkInfoList = GetNostaleNetworkInfoList();
		bool result = false;
		bool flag = false;
		foreach (NostaleCharacterInfo character in GUI._nostaleCharacterInfoList)
		{
			if (!nostaleNetworkInfoList.Any((NostaleCharacterInfo x) => x.hwnd == character.hwnd))
			{
				if (character == GUI.SelectedClient)
				{
					GUI.SelectedClient = null;
					GUI.SelectedPanel = null;
				}
				list2.Remove(character);
				result = true;
				flag = true;
				GUI._nostaleCharacterInfoList = list2;
				GUI.RemoveCharacterPanel(character);
				GUI.RemoveRaiderPanel(character);
				if (GUI.Mapper != null && character == GUI.Mapper)
				{
					RaidManager.RaidFinished();
				}
				if (GUI.Inviter?.hwnd == character.hwnd)
				{
					GUI.Inviter = null;
					GUI.miniland_state.Clear();
					GUI.updateInviterLabel();
				}
			}
		}
		foreach (NostaleCharacterInfo character in nostaleNetworkInfoList)
		{
			if (!list2.Any((NostaleCharacterInfo x) => x.hwnd == character.hwnd))
			{
				list2.Add(character);
				character.nickname = Crypto.ReadNickFromMemory((int)character.process_id);
				character.family_name = Crypto.ReadFamilyFromMemory((int)character.process_id);
				character.map_id = Crypto.ReadMapIdFromMemory((int)character.process_id);
				character.real_map_id = character.map_id;
				character.character_id = Crypto.ReadCharacterIdFromMemory((int)character.process_id);
				int sp_id = Crypto.FormatNostaleSpId(Crypto.ReadSPFromMemory((int)character.process_id));
				character.SPCard.UpdateSPCard(sp_id);
				character.class_id = Crypto.ReadClassIdFromMemory((int)character.process_id);
				character.sex = ((Crypto.ReadSexIdFromMemory((int)character.process_id) == 0) ? "male" : "female");
				character.hotBar = Crypto.ReadCurrentHotbarFromMemory((int)character.process_id);
				character.server = NostaleServers.GetServerNameFromIp(character.remote_address ?? "");
				character.channel = NostaleServers.GetServerChannelFromPort((int)character.remote_port.GetValueOrDefault());
				character.UpdateIcon();
				GUI._nostaleCharacterInfoList = list2;
				Settings.LoadCharConfig(character);
				GUI.AddCharacterToControlPanel(character);
				result = true;
				GUI.UpdateLoadingBar(list2.Count, nostaleNetworkInfoList.Count);
			}
		}
		foreach (NostaleCharacterInfo character in nostaleNetworkInfoList)
		{
			NostaleCharacterInfo nostaleCharacterInfo = list2.Find((NostaleCharacterInfo x) => x.hwnd == character.hwnd);
			if (nostaleCharacterInfo == null)
			{
				continue;
			}
			nostaleCharacterInfo.local_port = character.local_port;
			if (!nostaleCharacterInfo.local_port.HasValue && nostaleCharacterInfo.status != "offline" && GUI.Mapper != null && nostaleCharacterInfo == GUI.Mapper && RaidManager.raidStarted)
			{
				RaidManager.RaidFinished();
			}
			nostaleCharacterInfo.remote_port = character.remote_port;
			nostaleCharacterInfo.local_address = character.local_address;
			nostaleCharacterInfo.remote_address = character.remote_address;
			nostaleCharacterInfo.process_id = character.process_id;
			nostaleCharacterInfo.server = NostaleServers.GetServerNameFromIp(character.remote_address ?? "");
			nostaleCharacterInfo.channel = NostaleServers.GetServerChannelFromPort((int)character.remote_port.GetValueOrDefault());
			if (!nostaleCharacterInfo.local_port.HasValue && nostaleCharacterInfo.status != "offline")
			{
				nostaleCharacterInfo.status = "offline";
				nostaleCharacterInfo.nickname = "undefined";
				result = true;
				GUI._nostaleCharacterInfoList = list2;
				GUI.UpdateCharacterPanel(character);
				GUI.RemoveRaiderPanel(character);
				if (GUI.Mapper != null && nostaleCharacterInfo == GUI.Mapper)
				{
					ResetMapperData();
				}
			}
			else if (nostaleCharacterInfo.local_port.HasValue && nostaleCharacterInfo.status != "connected")
			{
				nostaleCharacterInfo.status = "connected";
				nostaleCharacterInfo.encryption_key = Crypto.ReadEncryptionKeyFromMemory((int)nostaleCharacterInfo.process_id);
				result = true;
				GUI.UpdateCharacterPanel(character);
				GUI.AddCharacterToRaidersPanel(character);
			}
		}
		GUI._nostaleCharacterInfoList = list2;
		if (flag)
		{
			CheckAccess.ExtendAccess();
		}
		PacketLogger.updatePacketsLoggerDict();
		return result;
	}

	private static bool IsForbiddenProcessRunning()
	{
		List<string> source = new List<string> { "Wireshark", "Fiddler", "NinjaOne", "ThreatLocker", "Atera", "Freshservice", "Miradore", "Site24x7" };
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			if (source.Contains<string>(process.ProcessName, StringComparer.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public static List<NostaleCharacterInfo> GetNostaleNetworkInfoList()
	{
		List<NostaleCharacterInfo> windowsList = Controller.getWindowsList();
		try
		{
			int dwOutBufLen = 0;
			DllImports.GetExtendedTcpTable(IntPtr.Zero, ref dwOutBufLen, sort: true, 2, TcpTableClass.TcpTableOwnerPidAll, 0);
			nint num = Marshal.AllocHGlobal(dwOutBufLen);
			if (DllImports.GetExtendedTcpTable(num, ref dwOutBufLen, sort: true, 2, TcpTableClass.TcpTableOwnerPidAll, 0) != 0)
			{
				throw new Exception("Failed to retrieve TCP table.");
			}
			MIB_TCPTABLE_OWNER_PID mIB_TCPTABLE_OWNER_PID = Marshal.PtrToStructure<MIB_TCPTABLE_OWNER_PID>(num);
			nint num2 = (nint)((long)num + (long)Marshal.SizeOf(typeof(uint)));
			for (int i = 0; i < mIB_TCPTABLE_OWNER_PID.dwNumEntries; i++)
			{
				MIB_TCPROW_OWNER_PID row = Marshal.PtrToStructure<MIB_TCPROW_OWNER_PID>(num2);
				if (windowsList.Any((NostaleCharacterInfo x) => x.process_id == row.dwOwningPid))
				{
					NostaleCharacterInfo nostaleCharacterInfo = windowsList.Find((NostaleCharacterInfo x) => x.process_id == row.dwOwningPid);
					if (nostaleCharacterInfo == null)
					{
						continue;
					}
					nostaleCharacterInfo.local_address = new IPAddress(BitConverter.GetBytes(row.dwLocalAddr)).ToString();
					nostaleCharacterInfo.remote_address = new IPAddress(BitConverter.GetBytes(row.dwRemoteAddr)).ToString();
					nostaleCharacterInfo.local_port = (ushort)IPAddress.NetworkToHostOrder((short)row.dwLocalPort);
					nostaleCharacterInfo.remote_port = (ushort)IPAddress.NetworkToHostOrder((short)row.dwRemotePort);
					nostaleCharacterInfo.process_id = row.dwOwningPid;
				}
				num2 = (nint)((long)num2 + (long)Marshal.SizeOf(typeof(MIB_TCPROW_OWNER_PID)));
			}
			Marshal.FreeHGlobal(num);
		}
		catch (Exception)
		{
		}
		return windowsList;
	}

	public static List<byte[]> SplitPackets(byte[] payload)
	{
		List<byte[]> list = new List<byte[]>();
		List<byte> list2 = new List<byte>();
		foreach (byte b in payload)
		{
			if (b == byte.MaxValue)
			{
				list.Add(list2.ToArray());
				list2.Clear();
			}
			else
			{
				list2.Add(b);
			}
		}
		if (list2.Count > 0)
		{
			list.Add(list2.ToArray());
		}
		return list;
	}

	public static string GetMainNetworkCardID()
	{
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		NetworkInterface networkInterface = allNetworkInterfaces.ToList().Find((NetworkInterface x) => x.Id == Settings.config.defaultNetwordDeviceID);
		if (networkInterface != null)
		{
			GUI.SetNetworkDeviceState(networkInterface.Name);
			return networkInterface.Id;
		}
		foreach (NetworkInterface item in allNetworkInterfaces.Where((NetworkInterface x) => x.OperationalStatus == OperationalStatus.Up))
		{
			if (item.GetIPv4Statistics().BytesSent > 0)
			{
				GUI.SetNetworkDeviceState(item.Name);
				return item.Id;
			}
		}
		return "";
	}

	public static void Listen(object sender, DoWorkEventArgs e)
	{
		string mainNetworkDeviceID = GetMainNetworkCardID();
		_device = LibPcapLiveDeviceList.Instance.Where((LibPcapLiveDevice x) => x.ToString().Contains(mainNetworkDeviceID)).FirstOrDefault();
		if (_device == null)
		{
			throw new Exception("Failed while getting main network device");
		}
		_device.Open(new DeviceConfiguration
		{
			Mode = DeviceModes.MaxResponsiveness
		});
		string text = "";
		foreach (NostaleCharacterInfo nostaleCharacterInfo in GUI._nostaleCharacterInfoList)
		{
			if (nostaleCharacterInfo.remote_address != null && nostaleCharacterInfo.local_port.HasValue)
			{
				text += $"(host {nostaleCharacterInfo.remote_address} and port {nostaleCharacterInfo.local_port}) or ";
			}
		}
		if (text.Length > 4)
		{
			string text2 = text;
			text = text2.Substring(0, text2.Length - 4);
			_device.Filter = text;
		}
		_device.OnPacketArrival += OnPacketArrival;
		_device.StartCapture();
	}

	public static async void UpdateConnectionsData(object sender, DoWorkEventArgs e)
	{
		while (true)
		{
			await Task.Delay(1000);
			if (IsForbiddenProcessRunning())
			{
				Utils.InvokeIfRequired(GUI.form, delegate
				{
					new NAMessageBox("Forbidden process detected! The program will close.", "Forbidden Process!", error: true).Show();
				});
				await Task.Delay(3000);
				Application.Exit();
			}
			if (!UpdateNostaleCharacterInfosList())
			{
				continue;
			}
			string text = "";
			foreach (NostaleCharacterInfo nostaleCharacterInfo in GUI._nostaleCharacterInfoList)
			{
				if (nostaleCharacterInfo.remote_address != null && nostaleCharacterInfo.local_port.HasValue)
				{
					text += $"(host {nostaleCharacterInfo.remote_address} and port {nostaleCharacterInfo.local_port}) or ";
				}
			}
			if (_device == null)
			{
				break;
			}
			if (text.Length > 4)
			{
				string text2 = text;
				text = text2.Substring(0, text2.Length - 4);
				_device.Filter = text;
			}
		}
		throw new Exception("Failed while getting main network device");
	}

	private static void OnPacketArrival(object s, PacketCapture e)
	{
		if (!(Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data).PayloadPacket is IPv4Packet pv4Packet))
		{
			return;
		}
		Packet payloadPacket = pv4Packet.PayloadPacket;
		TcpPacket tcpPacket = payloadPacket as TcpPacket;
		if (tcpPacket == null)
		{
			return;
		}
		byte[] array = tcpPacket.PayloadData;
		if (array.Length == 0)
		{
			return;
		}
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.local_port == tcpPacket.DestinationPort || x.local_port == tcpPacket.SourcePort);
		if (nostaleCharacterInfo == null)
		{
			return;
		}
		if (nostaleCharacterInfo.local_port == tcpPacket.DestinationPort)
		{
			if (start_collecting)
			{
				if (payloadsDict.TryGetValue(nostaleCharacterInfo.hwnd, out byte[] value))
				{
					Dictionary<nint, byte[]> dictionary = payloadsDict;
					nint hwnd = nostaleCharacterInfo.hwnd;
					byte[] array2 = value;
					byte[] array3 = array;
					int num = 0;
					byte[] array4 = new byte[array2.Length + array3.Length];
					Span<byte> span = new Span<byte>(array2);
					span.CopyTo(new Span<byte>(array4).Slice(num, span.Length));
					num += span.Length;
					Span<byte> span2 = new Span<byte>(array3);
					span2.CopyTo(new Span<byte>(array4).Slice(num, span2.Length));
					num += span2.Length;
					dictionary[hwnd] = array4;
				}
				else
				{
					payloadsDict[nostaleCharacterInfo.hwnd] = array;
				}
			}
			if (!lastsDict.ContainsKey(nostaleCharacterInfo.hwnd))
			{
				lastsDict[nostaleCharacterInfo.hwnd] = new List<byte>();
			}
			if (lastsDict[nostaleCharacterInfo.hwnd] != null && lastsDict[nostaleCharacterInfo.hwnd].Count != 0)
			{
				List<byte> list = lastsDict[nostaleCharacterInfo.hwnd];
				byte[] array4 = array;
				int num = 0;
				byte[] array3 = new byte[list.Count + array4.Length];
				Span<byte> span2 = CollectionsMarshal.AsSpan(list);
				span2.CopyTo(new Span<byte>(array3).Slice(num, span2.Length));
				num += span2.Length;
				Span<byte> span = new Span<byte>(array4);
				span.CopyTo(new Span<byte>(array3).Slice(num, span.Length));
				num += span.Length;
				array = array3;
			}
			List<byte[]> list2 = SplitPackets(array);
			if (list2.Count != 0)
			{
				if (array.Last() == byte.MaxValue)
				{
					lastsDict[nostaleCharacterInfo.hwnd] = new List<byte>();
				}
				else
				{
					lastsDict[nostaleCharacterInfo.hwnd] = list2.Last().ToList();
					list2.RemoveAt(list2.Count - 1);
				}
			}
			{
				foreach (byte[] item3 in list2)
				{
					NosPayload item = new NosPayload
					{
						payload = item3,
						type = "RECV",
						hwnd = nostaleCharacterInfo.hwnd,
						process_id = nostaleCharacterInfo.process_id,
						encryption_key = nostaleCharacterInfo.encryption_key,
						arrival_date = DateTime.Now.ToString("HH:mm:ss")
					};
					_payloadsQueue.Enqueue(item);
				}
				return;
			}
		}
		if (array.Length != 0)
		{
			NosPayload item2 = new NosPayload
			{
				payload = array,
				type = "SENT",
				hwnd = nostaleCharacterInfo.hwnd,
				process_id = nostaleCharacterInfo.process_id,
				encryption_key = nostaleCharacterInfo.encryption_key,
				arrival_date = DateTime.Now.ToString("HH:mm:ss")
			};
			_payloadsQueue.Enqueue(item2);
		}
	}

	public static async void HandlePackets(object sender, DoWorkEventArgs e)
	{
		while (true)
		{
			if (_payloadsQueue.IsEmpty)
			{
				await Task.Delay(100);
				continue;
			}
			_payloadsQueue.TryDequeue(out NosPayload result);
			if (result == null)
			{
				continue;
			}
			new_packets = Crypto.DecryptPayload(result);
			foreach (NosPacket currentPacket in new_packets)
			{
				if (start_collecting && currentPacket.type == "RECV" && GUI.Mapper != null && currentPacket.hwnd == GUI.Mapper.hwnd)
				{
					processed_packets.Add(currentPacket.content);
				}
				try
				{
					if (currentPacket.packet_type == "c_info")
					{
						HandleCharInfoPacket(currentPacket);
					}
					if (GUI.Main != null)
					{
						if (GUI.PacketLoggerPrintRecv && currentPacket.type == "RECV")
						{
							PacketLogger.Print(BuildPacketStringMessage(currentPacket), currentPacket);
						}
						if (GUI.PacketLoggerPrintSent && currentPacket.type == "SENT")
						{
							PacketLogger.Print(BuildPacketStringMessage(currentPacket), currentPacket);
						}
						if (currentPacket.packet_type == "c_mode" && currentPacket.type == "RECV")
						{
							HandleSPInfoPacket(currentPacket);
						}
						if (currentPacket.packet_type == "at")
						{
							HandleMapChangedPacket(currentPacket);
						}
						if (currentPacket.packet_type == "c_map")
						{
							HandleCMapPacket(currentPacket);
						}
						if (currentPacket.packet_type == "walk")
						{
							HandleWalkPacket(currentPacket);
						}
						if (currentPacket.packet_type == "stat")
						{
							HandleStatPacket(currentPacket);
						}
						if (!(currentPacket.packet_type == "qnamli2") || !(currentPacket.type == "RECV"))
						{
							goto IL_03cc;
						}
						if (!(currentPacket.packet_splitted.ElementAt(1) == "0"))
						{
							await HandleNewRaidInList(currentPacket);
							goto IL_03cc;
						}
						HandleMinilandInvitePacket(currentPacket);
					}
					goto end_IL_013c;
					IL_06f9:
					if (!currentPacket.content.StartsWith("rd 2"))
					{
						goto IL_07d4;
					}
					NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == currentPacket.hwnd);
					if (nostaleCharacterInfo == null)
					{
						continue;
					}
					nostaleCharacterInfo.inRaid = false;
					if (GUI.Mapper != null && GUI.Mapper.hwnd == currentPacket.hwnd && RaidManager.raidStarted)
					{
						RaidManager.RaidFinished();
					}
					if (GUI.Mapper != null && GUI.Mapper.hwnd == currentPacket.hwnd && RaidManager.firstRaid)
					{
						RaidManager.ClearAll();
					}
					if (GUI.Mapper != null && GUI.Mapper.hwnd == currentPacket.hwnd && !RaidManager.firstRaid)
					{
						RaidManager.RestoreCounter(paint: false);
					}
					goto IL_07d4;
					IL_07d4:
					if (!currentPacket.content.StartsWith("raidbf 0 4"))
					{
						goto IL_08af;
					}
					NostaleCharacterInfo nostaleCharacterInfo2 = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == currentPacket.hwnd);
					if (nostaleCharacterInfo2 == null)
					{
						continue;
					}
					nostaleCharacterInfo2.inRaid = false;
					if (GUI.Mapper != null && GUI.Mapper.hwnd == currentPacket.hwnd && RaidManager.raidStarted)
					{
						RaidManager.RaidFinished();
					}
					if (GUI.Mapper != null && GUI.Mapper.hwnd == currentPacket.hwnd && RaidManager.firstRaid)
					{
						RaidManager.ClearAll();
					}
					if (GUI.Mapper != null && GUI.Mapper.hwnd == currentPacket.hwnd && !RaidManager.firstRaid)
					{
						RaidManager.RestoreCounter(paint: false);
					}
					goto IL_08af;
					IL_0602:
					if (!currentPacket.content.StartsWith("raid 2 -1") && !currentPacket.content.StartsWith("raidf 2 -1"))
					{
						goto IL_06f9;
					}
					NostaleCharacterInfo nostaleCharacterInfo3 = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == currentPacket.hwnd);
					if (nostaleCharacterInfo3 == null)
					{
						continue;
					}
					nostaleCharacterInfo3.inRaid = false;
					if (GUI.Mapper != null && GUI.Mapper.hwnd == currentPacket.hwnd && RaidManager.raidStarted)
					{
						RaidManager.RaidFinished();
					}
					if (GUI.Mapper != null && GUI.Mapper.hwnd == currentPacket.hwnd && RaidManager.firstRaid)
					{
						RaidManager.ClearAll();
					}
					if (GUI.Mapper != null && GUI.Mapper.hwnd == currentPacket.hwnd && !RaidManager.firstRaid)
					{
						RaidManager.RestoreCounter(paint: false);
					}
					goto IL_06f9;
					IL_03cc:
					if (currentPacket.packet_type == "rl" && currentPacket.type == "RECV")
					{
						HandleOpenListPacket(currentPacket);
					}
					if (currentPacket.packet_type == "rdlst")
					{
						HandleRaidListPacket(currentPacket);
					}
					if (currentPacket.packet_type == "rdlstf" && !is_last_packet_rdlstf)
					{
						HandleRaidListFPacket(currentPacket);
					}
					else if (is_last_packet_rdlstf && currentPacket.hwnd == GUI.Mapper?.hwnd)
					{
						if (currentPacket.packet_type == "rdlstf")
						{
							concatRaidListFPackets(currentPacket);
							HandleRaidListPacket(currentPacket);
						}
						else if (last_rdlstf_packet != null)
						{
							HandleRaidListPacket(last_rdlstf_packet);
							is_last_packet_rdlstf = false;
							last_rdlstf_packet = null;
						}
					}
					if ((currentPacket.content.StartsWith("raid 4") || currentPacket.content.StartsWith("raidf 4")) && GUI.Mapper != null && GUI.Mapper.hwnd == currentPacket.hwnd)
					{
						RaidManager.RaidStarted();
					}
					if ((!currentPacket.content.StartsWith("raid 2") && !currentPacket.content.StartsWith("raidf 2")) || currentPacket.content.Contains("-1"))
					{
						goto IL_0602;
					}
					NostaleCharacterInfo nostaleCharacterInfo4 = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == currentPacket.hwnd);
					if (nostaleCharacterInfo4 == null)
					{
						continue;
					}
					nostaleCharacterInfo4.inRaid = true;
					goto IL_0602;
					IL_08af:
					if (currentPacket.packet_type == "in" && currentPacket.type == "RECV")
					{
						HandleInPacket(currentPacket);
					}
					if (currentPacket.packet_type == "out" && currentPacket.type == "RECV")
					{
						HandleOutPacket(currentPacket);
					}
					if (currentPacket.packet_type == "throw" && currentPacket.type == "RECV")
					{
						HandleItemThrowPacket(currentPacket);
					}
					if (currentPacket.packet_type == "drop" && currentPacket.type == "RECV")
					{
						HandleItemDropPacket(currentPacket);
					}
					if (currentPacket.packet_type == "get" && currentPacket.type == "RECV")
					{
						HandleItemPickedPacket(currentPacket);
					}
					if (currentPacket.packet_type == "su" && currentPacket.type == "RECV")
					{
						HandleSkillUsePacket(currentPacket);
					}
					if (currentPacket.packet_type == "eff" && currentPacket.type == "RECV")
					{
						HandleEffPacket(currentPacket);
					}
					if (currentPacket.packet_type == "eff_g" && currentPacket.type == "RECV")
					{
						HandleFieldSpawnedPacket(currentPacket);
					}
					if (currentPacket.packet_type == "eff_s" && currentPacket.type == "RECV")
					{
						HandleFieldDespawnedPacket(currentPacket);
					}
					if (currentPacket.packet_type == "mv" && currentPacket.type == "RECV")
					{
						HandleMvPacket(currentPacket);
					}
					if (currentPacket.packet_type == "ptctl" && currentPacket.type == "SENT")
					{
						HandlePtctlPacket(currentPacket);
					}
					if (currentPacket.packet_type == "st" && currentPacket.type == "RECV")
					{
						HandleStPacket(currentPacket);
					}
					if (currentPacket.packet_type == "pairy" && currentPacket.type == "RECV")
					{
						HandlePairyPacket(currentPacket);
					}
					if (currentPacket.packet_type == "lev" && currentPacket.type == "RECV")
					{
						HandleLevPacket(currentPacket);
					}
					if (currentPacket.packet_type == "bf" && currentPacket.type == "RECV")
					{
						HandleBfPacket(currentPacket);
					}
					if (currentPacket.packet_type == "ct" && currentPacket.type == "RECV")
					{
						HandleCtPacket(currentPacket);
					}
					if (currentPacket.packet_type == "gp" && currentPacket.type == "RECV")
					{
						HandleGPPacket(currentPacket);
					}
					if (currentPacket.packet_type == "tp" && currentPacket.type == "RECV")
					{
						HandleTPPacket(currentPacket);
					}
					if (currentPacket.packet_type == "guri" && currentPacket.type == "RECV")
					{
						HandleGuriPacket(currentPacket);
					}
					if (currentPacket.packet_type == "die" && currentPacket.type == "RECV")
					{
						HandleDiePacket(currentPacket);
					}
					if (currentPacket.packet_type == "msgi" && currentPacket.type == "RECV")
					{
						HandleMsgiPacket(currentPacket);
					}
					if (currentPacket.packet_type == "dlgi" && currentPacket.type == "RECV")
					{
						HandleDlgiPacket(currentPacket);
					}
					if (currentPacket.packet_type == "rboss" && currentPacket.type == "RECV")
					{
						HandleRbossPacket(currentPacket);
					}
					if (currentPacket.packet_type == "sayi" && currentPacket.type == "RECV")
					{
						HandleSayiPacket(currentPacket);
					}
					if (currentPacket.packet_type == "infoi" && currentPacket.type == "RECV")
					{
						HandleInfoiPacket(currentPacket);
					}
					if (currentPacket.packet_type == "eq" && currentPacket.type == "RECV")
					{
						HandleEqPacket(currentPacket);
					}
					if (currentPacket.packet_type == "e_info" && currentPacket.type == "RECV")
					{
						HandleE_InfoPacket(currentPacket);
					}
					if (currentPacket.packet_type == "sayitemt" && currentPacket.type == "RECV")
					{
						HandleSayitemtPacket(currentPacket);
					}
					if (currentPacket.packet_type == "slinfo" && currentPacket.type == "RECV")
					{
						HandleSlinfoPacket(currentPacket);
					}
					if (currentPacket.packet_type == "titinfo" && currentPacket.type == "RECV")
					{
						HandleTitInfoPacket(currentPacket);
					}
					if (currentPacket.packet_type == "gidx" && currentPacket.type == "RECV")
					{
						HandleGidxPacket(currentPacket);
					}
					if (currentPacket.packet_type == "act6" && currentPacket.type == "RECV")
					{
						HandleAct6Packet(currentPacket);
					}
					if (currentPacket.packet_type == "sc_p" && currentPacket.type == "RECV")
					{
						HandleSc_pPacket(currentPacket);
					}
					if (currentPacket.packet_type == "mlinfo" && currentPacket.type == "RECV")
					{
						HandleMlInfoPacket(currentPacket);
					}
					if (currentPacket.packet_type == "fc" && currentPacket.type == "RECV")
					{
						HandleFCPacket(currentPacket);
					}
					if (currentPacket.packet_type == "evnt" && currentPacket.type == "RECV")
					{
						HandleEvntPacket(currentPacket);
					}
					if (currentPacket.packet_type == "qstlist" && currentPacket.type == "RECV")
					{
						HandleQstlistPacket(currentPacket);
					}
					if (currentPacket.packet_type == "qsti" && currentPacket.type == "RECV")
					{
						HandleQstiPacket(currentPacket);
					}
					if (currentPacket.packet_type == "wp" && currentPacket.type == "RECV")
					{
						HandleWPPacket(currentPacket);
					}
					if (currentPacket.packet_type == "rbr" && currentPacket.type == "RECV")
					{
						HandleRbrPacket(currentPacket);
					}
					if (currentPacket.packet_type == "rsfn" && currentPacket.type == "RECV")
					{
						HandleRsfnPacket(currentPacket);
					}
					if (currentPacket.packet_type == "rsfp" && currentPacket.type == "RECV")
					{
						HandleRsfpPacket(currentPacket);
					}
					if (currentPacket.packet_type == "rsfi" && currentPacket.type == "RECV")
					{
						HandleRsfiPacket(currentPacket);
					}
					if (currentPacket.packet_type == "rsfm" && currentPacket.type == "RECV")
					{
						HandleRsfmPacket(currentPacket);
					}
					end_IL_013c:;
				}
				catch (Exception ex)
				{
					if (currentPacket.hwnd == GUI.Mapper?.hwnd)
					{
						packets_corrupted_count++;
					}
					if (ex is FormatException || ex is ArgumentOutOfRangeException)
					{
						continue;
					}
					NostaleCharacterInfo nostaleCharacterInfo5 = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == currentPacket.hwnd);
					if (nostaleCharacterInfo5 == null)
					{
						continue;
					}
					string errorlog = $"Packet:{currentPacket.content} Error message: {ex}";
					try
					{
						using HttpClient client = new HttpClient();
						client.PostAsJsonAsync("https://nosassistant.pl/api/errorlog", new ErrorLog
						{
							errorlog = errorlog,
							mapper = GUI.Mapper?.nickname,
							map_id = nostaleCharacterInfo5.real_map_id.ToString()
						}).Wait();
					}
					catch (Exception exception)
					{
						NALogger.LogExceptionToFile(exception);
					}
				}
			}
		}
	}

	private static string BuildPacketStringMessage(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null)
		{
			return "";
		}
		return string.Concat($"[{packet.date:HH:mm:ss}][{nostaleCharacterInfo.nickname}]" + "[" + packet.type + "] ", packet.content);
	}

	private static void HandleCharInfoPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo character = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (character == null)
		{
			return;
		}
		string nickname = character.nickname;
		string text = packet.packet_splitted[1];
		string text2 = packet.packet_splitted[4];
		int family_id = 0;
		int family_role_id = 0;
		if (text2 != "-1" && text2.Contains("."))
		{
			List<string> list = text2.Split(".").ToList();
			if (list.Count == 2)
			{
				family_id = Convert.ToInt32(list.ElementAt(0));
				family_role_id = Convert.ToInt32(list.ElementAt(1));
			}
		}
		string text3 = packet.packet_splitted[5];
		int character_id = Convert.ToInt32(packet.packet_splitted[6]);
		int num = Convert.ToInt32(packet.packet_splitted[8]);
		string sex = ((num == 1) ? "female" : "male");
		int num2 = Convert.ToInt32(packet.packet_splitted[11]);
		int reputation = Convert.ToInt32(packet.packet_splitted[12]);
		int sp_id = Convert.ToInt32(packet.packet_splitted[14]);
		int sp_upgrade = Convert.ToInt32(packet.packet_splitted[16]);
		int sp_wings = Convert.ToInt32(packet.packet_splitted[17]);
		if (text3 != "-")
		{
			RaidManager.UpdateFam(character_id, text3);
		}
		character.nickname = text;
		character.family_name = text3;
		character.character_id = character_id;
		character.sex = sex;
		character.class_id = num2;
		character.SPCard = new SPCard();
		character.SPCard.UpdateSPCard(sp_id, sp_upgrade, sp_wings);
		if (nickname != text)
		{
			Settings.LoadCharsConfig();
			character.SPCard.UpdateSPCard(sp_id, sp_upgrade, sp_wings);
			character.UpdateIcon();
			GUI.UpdateCharacterPanel(character);
			GUI.AddCharacterToRaidersPanel(character);
			Task.Run(async delegate
			{
				await Task.Delay(200);
				character.hotBar = Crypto.ReadCurrentHotbarFromMemory((int)character.process_id);
			});
		}
		CheckAccess.ExtendAccess();
		if (GUI.Mapper != null && GUI.Mapper.hwnd == packet.hwnd && GUI.Mapper.lvl != -1 && GUI.Mapper.clvl != -1)
		{
			Analytics.self.character_id = character_id;
			Analytics.self.nickname = text;
			Analytics.self.lvl = GUI.Mapper.lvl;
			Analytics.self.clvl = GUI.Mapper.clvl;
			Analytics.self.family = text3;
			Analytics.self.sex = num;
			Analytics.self.classID = num2;
			Analytics.self.family_id = family_id;
			Analytics.self.family_role_id = family_role_id;
			Analytics.self.reputation = reputation;
		}
	}

	private static async void HandleSPInfoPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo character = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (character == null)
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int source_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		int sp_id = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
		int sp_upgrade = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		int sp_wings_id = Convert.ToInt32(packet.packet_splitted.ElementAt(5));
		switch (num)
		{
		case 1:
		{
			GamePlayer gamePlayer = GUI.players.Find((GamePlayer x) => x.character_id == source_id);
			if (gamePlayer != null)
			{
				gamePlayer.spID = sp_id;
				gamePlayer.sp_upgrade = sp_upgrade;
				gamePlayer.sp_wings_id = sp_wings_id;
				gamePlayer.UpdateIcon();
			}
			RaidManager.UpdateSP(source_id, sp_id);
			if (character.character_id == source_id)
			{
				character.SPCard.UpdateSPCard(sp_id, sp_upgrade, sp_wings_id);
				character.UpdateIcon();
				GUI.UpdateCharacterPanel(character);
				GUI.UpdateRaiderPanel(character);
				await Task.Delay(100);
				character.hotBar = Crypto.ReadCurrentHotbarFromMemory((int)character.process_id);
				if (GUI.Mapper != null && character == GUI.Mapper)
				{
					Analytics.self.spID = sp_id;
					Analytics.self.sp_upgrade = sp_upgrade;
					Analytics.self.sp_wings_id = sp_wings_id;
				}
			}
			break;
		}
		case 2:
		{
			GameEntity gameEntity = GUI.entities.Find((GameEntity x) => x.server_id == source_id);
			if (gameEntity != null)
			{
				gameEntity.id = sp_id;
				gameEntity.UpdateIcon();
			}
			break;
		}
		}
	}

	private static void HandleMapChangedPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		int x_pos = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
		int y_pos = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd)
		{
			if (GUI.players.Count != 0)
			{
				Analytics.SendMapPlayersInfo();
			}
			NAStyles.map_to_filters[GUI.Mapper.map_id] = NAStyles.mob_filters;
			if (NAStyles.map_to_filters.ContainsKey(num))
			{
				NAStyles.mob_filters = NAStyles.map_to_filters[num];
			}
			else
			{
				NAStyles.mob_filters = new List<GameMonster>();
			}
			if (GUI.mob_filters_window != null)
			{
				Task.Run(async delegate
				{
					await Task.Delay(1500);
					if (GUI.mob_filters_window != null)
					{
						Utils.InvokeIfRequired(GUI.mob_filters_window, delegate
						{
							GUI.mob_filters_window.RefreshItems();
						});
					}
				});
			}
			if (num != 20001)
			{
				Miniland.isInOwnMiniland = false;
			}
		}
		if (nostaleCharacterInfo != null)
		{
			nostaleCharacterInfo.map_id = num;
			nostaleCharacterInfo.x_pos = x_pos;
			nostaleCharacterInfo.y_pos = y_pos;
		}
		if (GUI.Inviter != null && GUI.Inviter.hwnd == packet.hwnd)
		{
			GUI.miniland_state.Clear();
		}
		if (GUI.Mapper == null || packet.hwnd != GUI.Mapper.hwnd)
		{
			return;
		}
		GUI.updateMapInfo(num);
		ResetMapperData();
		if (GUI.dmgContributionForm != null && GUI.dmgContributionForm.Visible)
		{
			DmgContributionCounterWindow.clear();
		}
		RaidManager.glacernonRaidStarted = MapID.isGlacernonRaidMap(GUI.Mapper.map_id);
		if (Settings.config.RaidModeSettings.AutoResizeMap && MapID.isRaidMap(GUI.Mapper.map_id) && !MapID.isBossRoom(GUI.Mapper.map_id))
		{
			Utils.InvokeIfRequired(GUI.RaidModeForm, delegate
			{
				GUI.RaidModeForm?.EnlargeMap();
			});
		}
		if (Settings.config.RaidModeSettings.AutoResizeMap && MapID.isBossRoom(GUI.Mapper.map_id))
		{
			Utils.InvokeIfRequired(GUI.RaidModeForm, delegate
			{
				GUI.RaidModeForm?.restoreMapLocation();
			});
		}
		if (ICManager.ICStarted != 0 && GUI.Mapper.map_id != 2004 && GUI.Mapper.map_id != 2717)
		{
			ICManager.InstantCombatFinished();
		}
		ICManager.ICStarted = ((GUI.Mapper.map_id == 2004) ? 1 : ((GUI.Mapper.map_id == 2717) ? 2 : 0));
		if (RaidManager.glacernonRaidStarted || ICManager.ICStarted != 0)
		{
			if (RaidManager.glacernonRaidStarted)
			{
				RaidManager.GlacernonRaidStarted();
			}
			else if (ICManager.ICStarted != 0)
			{
				ICManager.InstantCombatStarted();
			}
			RaidManager.AddCharacterToGlacernonRaidOrIC(new GamePlayer
			{
				nickname = GUI.Mapper.nickname,
				character_id = GUI.Mapper.character_id,
				lvl = GUI.Mapper.lvl,
				clvl = GUI.Mapper.clvl,
				spID = GUI.Mapper.SPCard.ID,
				classID = GUI.Mapper.class_id,
				sex = ((!(GUI.Mapper.sex == "male")) ? 1 : 0),
				family = GUI.Mapper.family_name,
				icon = GUI.Mapper.icon
			});
		}
		RaidMapData.UpdateRaidData(num);
		RaidManager.mobsKilled = 0;
		RaidManager.lastThresholdCount = 0;
		RaidManager.currentMobsThreshold = RaidMapData.MobsToKill(num);
		GUI.RaidModeForm?.UpdateMobKilled();
		RaidForm.SortByTotal();
	}

	private static void HandleWalkPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo != null)
		{
			string value = packet.packet_splitted.ElementAt(1);
			string value2 = packet.packet_splitted.ElementAt(2);
			nostaleCharacterInfo.x_pos = Convert.ToInt32(value);
			nostaleCharacterInfo.y_pos = Convert.ToInt32(value2);
		}
	}

	private static void HandleStatPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null)
		{
			return;
		}
		nostaleCharacterInfo.current_hp = Convert.ToInt32(packet.packet_splitted[1]);
		nostaleCharacterInfo.max_hp = Convert.ToInt32(packet.packet_splitted[2]);
		nostaleCharacterInfo.current_mana = Convert.ToInt32(packet.packet_splitted[3]);
		nostaleCharacterInfo.max_mana = Convert.ToInt32(packet.packet_splitted[4]);
		if ((nostaleCharacterInfo.config.isRaider || nostaleCharacterInfo.config.isAttacker) && !nostaleCharacterInfo.config.isDisabled)
		{
			GUI.UpdateRaiderPanel(nostaleCharacterInfo);
			if (GUI.AutoFull && (double)nostaleCharacterInfo.current_hp / (double)nostaleCharacterInfo.max_hp * 100.0 < (double)Settings.config.autoFullThreshold && nostaleCharacterInfo.config.isAutoFull && !nostaleCharacterInfo.config.isDisabled)
			{
				Raids.useFullHPPotion(nostaleCharacterInfo);
			}
			if (nostaleCharacterInfo.config.isAutoFullMana && !nostaleCharacterInfo.config.isDisabled && nostaleCharacterInfo.current_mana <= nostaleCharacterInfo.config.AutoFullManaThreshold && nostaleCharacterInfo.config.AutoFullManaThreshold < nostaleCharacterInfo.max_mana)
			{
				Raids.useItem(nostaleCharacterInfo.hwnd, ItemID.FullManaPotions, "Full Mana Potion", isItem: true, restrict: true, 1000);
			}
		}
	}

	private static void HandleRaidListPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null)
		{
			return;
		}
		nostaleCharacterInfo.inRaid = true;
		if (GUI.Mapper == null || GUI.Mapper.hwnd != nostaleCharacterInfo.hwnd)
		{
			return;
		}
		if ((RaidManager.currentRaid = Convert.ToInt32(packet.packet_splitted.ElementAt((packet.packet_type == "rdlst") ? 3 : 4))) == 32)
		{
			CounterColumnsSettings counterColumnsSettings = (Settings.config.RaidModeSettings.MinimalisticGrid ? Settings.config.CounterSettings.small : ((!Settings.config.RaidModeSettings.GridExpanded) ? Settings.config.CounterSettings.normal : Settings.config.CounterSettings.large));
			RaidForm.isInBellial = true;
			if (counterColumnsSettings.RaidSpec)
			{
				RaidForm.showRaidSpecificColumns();
			}
		}
		else
		{
			RaidForm.isInBellial = false;
			RaidForm.hideRaidSpecificColumns();
		}
		if (!RaidManager.raidStarted)
		{
			RaidManager.UpdateCharactersInRaidList(packet);
		}
		else
		{
			RaidManager.UpdateCharactersInfo(packet);
		}
	}

	private static async void HandleOpenListPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null || nostaleCharacterInfo.config.isDisabled || nostaleCharacterInfo.inRaid || !nostaleCharacterInfo.config.isRaider || nostaleCharacterInfo.nickname == GUI.raidHost || !Raids.allowJoining)
		{
			return;
		}
		switch (Convert.ToInt32(packet.packet_splitted.ElementAt(1)))
		{
		case 0:
		{
			List<string> list = packet.packet_splitted.Skip(2).ToList();
			List<RaidListProperties> list2 = new List<RaidListProperties>();
			foreach (string item in list)
			{
				string[] array = item.Split(".");
				List<string> list3 = new List<string>(array.Length);
				list3.AddRange(array);
				List<string> list4 = list3;
				list2.Add(new RaidListProperties
				{
					raidID = Convert.ToInt32(list4[0]),
					minRaidLvl = Convert.ToInt32(list4[1]),
					maxRaidLvl = Convert.ToInt32(list4[2]),
					host = list4[3],
					hostLvl = Convert.ToInt32(list4[4]),
					hostSPID = Convert.ToInt32(list4[5]),
					hostClassID = Convert.ToInt32(list4[6]),
					funnyNumber = Convert.ToInt32(list4[7]),
					membersCount = Convert.ToInt32(list4[8]),
					hostAWLvl = Convert.ToInt32(list4[9])
				});
			}
			List<RaidListProperties> list5 = new List<RaidListProperties>();
			list5.AddRange(list2.OrderBy((RaidListProperties x) => x.raidID));
			list2 = list5;
			int num = list2.FindIndex((RaidListProperties x) => x.host == GUI.raidHost);
			List<NostaleCharacterInfo> list6 = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isRaider && !x.config.isDisabled && !x.inRaid && x.nickname != GUI.raidHost).ToList();
			if (num == -1)
			{
				AbortRaidJoining(nostaleCharacterInfo.nickname + " did not find the raid in the list. Joining cancelled.", list6);
				break;
			}
			Raids.readyRaiders.Add(new KeyValuePair<int, NostaleCharacterInfo>(num + 1, nostaleCharacterInfo));
			if (Raids.readyRaiders.Count == list6.Count)
			{
				Raids.joined_raid_id = list2.ElementAt(num).raidID;
				Raids.allJoinList(list2.ElementAt(num).membersCount + 1);
			}
			break;
		}
		case 1:
			nostaleCharacterInfo.inRaid = true;
			break;
		case 3:
			nostaleCharacterInfo.inRaid = true;
			break;
		case 2:
			break;
		}
	}

	private static async Task HandleNewRaidInList(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null)
		{
			return;
		}
		if (packet.packet_splitted.ElementAt(2) == "#rl")
		{
			int bossID = RaidID.GetBossID(RaidID.GetRaidIDFromRaidListID(Convert.ToInt32(packet.packet_splitted.ElementAt(5))));
			if (Settings.config.raidsNotifications.Contains(bossID) && Math.Abs((GUI.last_raid_notifiaction_shown - DateTime.UtcNow).TotalSeconds) > 5.0)
			{
				GUI.ShowRaidNotification($"Raid {GameBoss.getBossName(bossID)} has appeared in the raid list (CH{nostaleCharacterInfo.channel})");
				GUI.last_raid_notifiaction_shown = DateTime.UtcNow;
			}
		}
		if (nostaleCharacterInfo.config.isDisabled || !nostaleCharacterInfo.config.isRaider || nostaleCharacterInfo.nickname == GUI.raidHost || !Settings.config.autoJoinList || !(packet.packet_splitted.Last() == GUI.raidHost))
		{
			return;
		}
		if (Raids.joining)
		{
			return;
		}
		List<NostaleCharacterInfo> expected_raiders = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isRaider && !x.config.isDisabled && !x.inRaid && x.nickname != GUI.raidHost).ToList();
		if (expected_raiders.Count == 0)
		{
			return;
		}
		if (!expected_raiders.All((NostaleCharacterInfo x) => x.server == expected_raiders[0].server && x.channel == expected_raiders[0].channel))
		{
			GUI.ShowPopUp("Not all raiders are on the same server or channel!");
			return;
		}
		Raids.readyForOpenList.Add(nostaleCharacterInfo);
		if (Raids.readyForOpenList.Count == expected_raiders.Count)
		{
			Raids.allowJoining = true;
			Raids.OpenList();
		}
		else if (!Raids.aborting_task_running)
		{
			Task.Run(async delegate
			{
				Raids.aborting_task_running = true;
				await Task.Delay(30000);
				Raids.allowJoining = false;
				Raids.joining = false;
				Raids.readyRaiders.Clear();
				Raids.aborting_task_running = false;
			});
		}
	}

	private static async void HandleHotBarChangePacket(NosPacket packet)
	{
		
		NostaleCharacterInfo character = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (character != null)
		{
			int hotBarSlotID = Convert.ToInt32(packet.packet_splitted[2]);
			await Task.Delay(100);
			(character.hotBar.ElementAt(hotBarSlotID).SlotValue, character.hotBar.ElementAt(hotBarSlotID).isItem) = Crypto.ReadHotbarSlotFromMemory((int)packet.process_id, hotBarSlotID);
		}
	}

	private static void HandleMvPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd)
		{
			Convert.ToInt32(packet.packet_splitted.ElementAt(1));
			int server_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			int x2 = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
			int y = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
			Convert.ToInt32(packet.packet_splitted.ElementAt(5));
			GameMonster gameMonster = GUI.monsters.Find((GameMonster x) => x.server_id == server_id);
			if (gameMonster != null)
			{
				gameMonster.x = x2;
				gameMonster.y = y;
			}
			GameEntity gameEntity = GUI.entities.Find((GameEntity x) => x.server_id == server_id);
			if (gameEntity != null)
			{
				gameEntity.x = x2;
				gameEntity.y = y;
			}
			GamePlayer gamePlayer = GUI.players.Find((GamePlayer x) => x.character_id == server_id);
			if (gamePlayer != null)
			{
				gamePlayer.x = x2;
				gamePlayer.y = y;
			}
		}
	}

	private static void HandlePtctlPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd)
		{
			long server_id = Convert.ToInt64(packet.packet_splitted.ElementAt(3));
			int x2 = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
			int y = Convert.ToInt32(packet.packet_splitted.ElementAt(5));
			GameEntity gameEntity = GUI.entities.Find((GameEntity x) => x.server_id == server_id);
			if (gameEntity != null)
			{
				gameEntity.x = x2;
				gameEntity.y = y;
			}
		}
	}

	private static void HandleEffPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null || nostaleCharacterInfo != GUI.Mapper)
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int server_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
		int num3 = 15;
		if (num == 1)
		{
			if (GameTarot.IsTarot(num2))
			{
				RaidManager.UpdateTarot(server_id, num2);
			}
			if (num2 == num3)
			{
				RaidManager.AddBon(server_id);
			}
		}
		if (num == 3)
		{
			GameMonster gameMonster = GUI.monsters.Find((GameMonster x) => x.server_id == server_id);
			if (gameMonster != null && num2 == 824)
			{
				gameMonster.is_special = true;
			}
		}
	}

	private static void HandleInPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null || (nostaleCharacterInfo != GUI.Mapper && nostaleCharacterInfo != GUI.Inviter))
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		if (num == 1)
		{
			HandlePlayerInPacket(packet);
		}
		else if (nostaleCharacterInfo == GUI.Mapper)
		{
			switch (num)
			{
			case 2:
			case 9:
				HandleEntityInPacket(packet);
				break;
			case 3:
				HandleMobInPacket(packet);
				break;
			}
		}
	}

	private static void HandlePlayerInPacket(NosPacket packet)
	{
		
		string nickname = Crypto.FormatGameString(packet.packet_splitted.ElementAt(2));
		int character_id = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		int x2 = Convert.ToInt32(packet.packet_splitted.ElementAt(5));
		int y2 = Convert.ToInt32(packet.packet_splitted.ElementAt(6));
		Convert.ToInt32(packet.packet_splitted.ElementAt(7));
		int sex = Convert.ToInt32(packet.packet_splitted.ElementAt(9));
		int classID = Convert.ToInt32(packet.packet_splitted.ElementAt(12));
		string items = packet.packet_splitted.ElementAt(13);
		int fairy_element_id = Convert.ToInt32(packet.packet_splitted.ElementAt(19));
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(21));
		int spID = Convert.ToInt32(packet.packet_splitted.ElementAt(23));
		string weapon_upgrade = packet.packet_splitted.ElementAt(24);
		string armor_upgrade = packet.packet_splitted.ElementAt(25);
		string text = packet.packet_splitted.ElementAt(26);
		int family_id = 0;
		int family_role_id = 0;
		if (text != "-1" && text.Contains("."))
		{
			List<string> list = text.Split(".").ToList();
			if (list.Count == 2)
			{
				family_id = Convert.ToInt32(list.ElementAt(0));
				family_role_id = Convert.ToInt32(list.ElementAt(1));
			}
		}
		string text2 = packet.packet_splitted.ElementAt(27);
		int reputation = Convert.ToInt32(packet.packet_splitted.ElementAt(28));
		int sp_upgrade = Convert.ToInt32(packet.packet_splitted.ElementAt(30));
		int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(31));
		int sp_wings_id = Convert.ToInt32(packet.packet_splitted.ElementAt(32));
		int lvl = Convert.ToInt32(packet.packet_splitted.ElementAtOrDefault(33) ?? "-1");
		int family_lvl = Convert.ToInt32(packet.packet_splitted.ElementAtOrDefault(34) ?? "-1");
		int clvl = Convert.ToInt32(packet.packet_splitted.ElementAtOrDefault(39) ?? "-1");
		int title = Convert.ToInt32(packet.packet_splitted.ElementAtOrDefault(40) ?? "-1");
		GamePlayer gamePlayer = new GamePlayer
		{
			nickname = nickname,
			character_id = character_id,
			family = text2,
			x = x2,
			y = y2,
			classID = classID,
			clvl = clvl,
			lvl = lvl,
			sex = sex,
			spID = spID,
			sp_upgrade = sp_upgrade,
			sp_wings_id = sp_wings_id,
			fraction = num2 switch
			{
				3 => "angel", 
				4 => "demon", 
				_ => "neutral", 
			},
			items = items,
			armor_upgrade = armor_upgrade,
			weapon_upgrade = weapon_upgrade,
			reputation = reputation,
			family_lvl = family_lvl,
			family_id = family_id,
			family_role_id = family_role_id,
			fairy_element_id = fairy_element_id,
			fairy_id = num,
			title = title
		};
		if (GUI.Inviter != null && packet.hwnd == GUI.Inviter.hwnd)
		{
			GUI.miniland_state.Add(gamePlayer);
			if (Settings.config.inviteList.All((InviteItem x) => GUI.miniland_state.Any((GamePlayer y) => y.nickname == x.nickname)) && GUI.Inviter.map_id == 20001)
			{
				GUI.ShowPopUp("All players are ready!", isNotification: true);
			}
			if (packet.hwnd != GUI.Mapper?.hwnd)
			{
				return;
			}
		}
		if (!GUI.players.Exists((GamePlayer x) => x.character_id == character_id) && !GUI._nostaleCharacterInfoList.Any((NostaleCharacterInfo x) => x.character_id == character_id))
		{
			gamePlayer.UpdateIcon();
			GUI.players.Add(gamePlayer);
		}
		if (RaidManager.glacernonRaidStarted || ICManager.ICStarted != 0)
		{
			NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
			if (nostaleCharacterInfo == null || GUI.Mapper == null || GUI.Mapper.hwnd != nostaleCharacterInfo.hwnd)
			{
				return;
			}
			gamePlayer.UpdateIcon();
			RaidManager.AddCharacterToGlacernonRaidOrIC(gamePlayer);
		}
		if (text2 != "-")
		{
			RaidManager.UpdateFam(character_id, text2);
		}
		RaidManager.UpdateFairy(character_id, num);
	}

	private static void HandleEntityInPacket(NosPacket packet)
	{
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		int entity_server_id = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
		int x = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		int y = Convert.ToInt32(packet.packet_splitted.ElementAt(5));
		GameEntity gameEntity;
		switch (num)
		{
		case 2:
		{
			int z = Convert.ToInt32(packet.packet_splitted.ElementAt(6));
			int num3 = Convert.ToInt32(packet.packet_splitted.ElementAt(7));
			Convert.ToInt32(packet.packet_splitted.ElementAt(8));
			int num4 = Convert.ToInt32(packet.packet_splitted.ElementAt(12));
			int sppID = Convert.ToInt32(packet.packet_splitted.ElementAt(15));
			int num5 = Convert.ToInt32(packet.packet_splitted.ElementAt(17));
			string type_name = "";
			if (num4 == -1)
			{
				type_name = "NPC";
			}
			else
			{
				switch (num5)
				{
				case 1:
					type_name = "Partner";
					break;
				case 2:
					type_name = "Pet";
					break;
				}
			}
			gameEntity = new GameEntity
			{
				type = num,
				type_name = type_name,
				id = num2,
				server_id = entity_server_id,
				x = x,
				y = y,
				z = z,
				hp_percent = num3,
				max_hp_percent = num3,
				pet_owner_id = num4,
				sppID = sppID
			};
			break;
		}
		case 9:
		{
			Convert.ToInt32(packet.packet_splitted.ElementAt(6));
			int item_owner_id = Convert.ToInt32(packet.packet_splitted.ElementAt(9));
			gameEntity = new GameEntity
			{
				type = num,
				type_name = (GameEntity.IsLever(num2) ? "Lever" : "Item"),
				id = num2,
				server_id = entity_server_id,
				x = x,
				y = y,
				item_owner_id = item_owner_id,
				is_required_lever = (GameEntity.IsLever(num2) && RaidMapData.ID != -1 && !RaidMapData.IgnoreButtons.Any(((int x, int y) a) => a.x == x && a.y == y))
			};
			NostaleCharacterInfo? mapper = GUI.Mapper;
			if (mapper == null || mapper.real_map_id != 2754)
			{
				NostaleCharacterInfo? mapper2 = GUI.Mapper;
				if (mapper2 == null || mapper2.real_map_id != 2755)
				{
					goto IL_027c;
				}
			}
			RaidManager.handleArmaButtons(gameEntity);
			goto IL_027c;
		}
		default:
			return;
			IL_027c:
			if (TimeSpaceManager.ts_started && gameEntity.type_name == "Lever")
			{
				TimeSpaceManager.AddLever(gameEntity);
			}
			break;
		}
		if (!GUI.entities.Exists((GameEntity x) => x.server_id == entity_server_id))
		{
			if (gameEntity.type != 9)
			{
				gameEntity.UpdateIcon();
			}
			GUI.entities.Add(gameEntity);
		}
	}

	private static void HandleMobInPacket(NosPacket packet)
	{
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		int mob_server_id = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
		int x2 = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		int y = Convert.ToInt32(packet.packet_splitted.ElementAt(5));
		Convert.ToInt32(packet.packet_splitted.ElementAt(6));
		int hp_percent = Convert.ToInt32(packet.packet_splitted.ElementAt(7));
		GameMonster mob = new GameMonster
		{
			id = num,
			name = GameMonster.GetNameById(num),
			server_id = mob_server_id,
			x = x2,
			y = y,
			hp_percent = hp_percent,
			is_boss = GameBoss.isBoss(num),
			is_special = GameMonster.isMapSpecial(num),
			is_semi_exclusive = GameMonster.isSemiExclusive(num),
			is_spawn_timed = GameMonster.isSpawnTimed(num),
			is_spawn_timed_boss = GameMonster.isSpawnTimedBoss(num),
			is_circle = GameMonster.isCircle(num)
		};
		if (TimeSpaceManager.ts_started)
		{
			TimeSpaceManager.AddMob(mob);
		}
		if (!GUI.monsters.Exists((GameMonster x) => x.server_id == mob_server_id))
		{
			mob.UpdateIcon();
			GUI.monsters.Add(mob);
			if (mob.is_spawn_timed && !mob.is_spawn_timed_boss)
			{
				mob.spawn_point = GameMonster.FindClosestSpawnPoint(x2, y, mob.id);
			}
		}
		if (RaidManager.raidStarted && GameSummon.isSummon(num))
		{
			RaidManager.AddSummon(num, mob_server_id);
		}
		if (GUI.Mapper != null && MapID.isFamMobbingMap(GUI.Mapper.real_map_id))
		{
			if (mob.is_spawn_timed && !mob.is_spawn_timed_boss)
			{
				GUI.spawn_timed_mobs.RemoveAll((SpawnTimedMob x) => x.server_id == mob.server_id);
			}
			else if (mob.is_spawn_timed && mob.is_spawn_timed_boss)
			{
				GUI.spawn_timed_mobs.RemoveAll((SpawnTimedMob x) => x.id == mob.id);
				NAStyles.MapBossDespawnTime = DateTime.UtcNow.AddSeconds(60.0);
			}
		}
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd && Miniland.isInOwnMiniland && (GUI.Mapper.SPCard.ID == 49 || GUI.Mapper.SPCard.ID == 50) && !Miniland.pet_trainer_mobs_list.Any((PetTrainerMob x) => x.mob_server_id == mob_server_id) && GameMonster.PetTrainerMobsDurations.ContainsKey(num))
		{
			PetTrainerMob pt_mob = new PetTrainerMob
			{
				mob_id = num,
				mob_server_id = mob_server_id,
				duration = GameMonster.PetTrainerMobsDurations[num],
				spawn_time = DateTime.UtcNow,
				despawn_time = DateTime.UtcNow.AddSeconds(GameMonster.PetTrainerMobsDurations[num]),
				icon = GameMonster.GetIcon(num)
			};
			PetTrainerMob petTrainerMob = Miniland.pet_trainer_mobs_list.Find((PetTrainerMob x) => x.duration == pt_mob.duration);
			if (petTrainerMob != null)
			{
				Miniland.pet_trainer_mobs_list.Remove(petTrainerMob);
			}
			Miniland.pet_trainer_mobs_list.Add(pt_mob);
			Miniland.pet_trainer_mobs_list = Miniland.pet_trainer_mobs_list.OrderBy((PetTrainerMob x) => x.duration).ToList();
		}
	}

	private static void HandleOutPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null || (nostaleCharacterInfo != GUI.Mapper && nostaleCharacterInfo != GUI.Inviter))
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		if (num == 1)
		{
			HandlePlayerOutPacket(packet);
		}
		else if (nostaleCharacterInfo == GUI.Mapper)
		{
			switch (num)
			{
			case 2:
				HandleEntityOutPacket(packet);
				break;
			case 3:
				HandleMobOutPacket(packet);
				break;
			case 9:
				HandleLeverOutPacket(packet);
				HandleItemOutPacket(packet);
				break;
			}
		}
	}

	private static void HandlePlayerOutPacket(NosPacket packet)
	{
		Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int character_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		if (GUI.Inviter != null && packet.hwnd == GUI.Inviter.hwnd)
		{
			GamePlayer gamePlayer = GUI.miniland_state.Find((GamePlayer x) => x.character_id == character_id);
			if (gamePlayer != null)
			{
				GUI.miniland_state.Remove(gamePlayer);
			}
			if (packet.hwnd != GUI.Mapper?.hwnd)
			{
				return;
			}
		}
		GamePlayer gamePlayer2 = GUI.players.Find((GamePlayer x) => x.character_id == character_id);
		if (gamePlayer2 != null)
		{
			GUI.players.Remove(gamePlayer2);
			DmgContributionCounterWindow.removePlayer(gamePlayer2.character_id);
			Analytics.SendSinglePlayerData(gamePlayer2);
		}
	}

	private static void HandleEntityOutPacket(NosPacket packet)
	{
		Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int server_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		GameEntity gameEntity = GUI.entities.Find((GameEntity x) => x.server_id == server_id);
		if (gameEntity != null)
		{
			GUI.entities.Remove(gameEntity);
		}
	}

	private static void HandleMobOutPacket(NosPacket packet)
	{
		Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int server_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		GameMonster mob = GUI.monsters.Find((GameMonster x) => x.server_id == server_id);
		if (mob != null)
		{
			GUI.monsters.Remove(mob);
		}
		if (mob != null && GUI.dmgContributionForm != null && GUI.dmgContributionForm.Visible)
		{
			DmgContributionCounterWindow.removeMob(mob.server_id);
		}
		if (mob != null && mob.is_spawn_timed)
		{
			SpawnTimedMob spawnTimedMob = (mob.is_spawn_timed_boss ? GameMonster.SpawnTimedBosses[mob.id] : new SpawnTimedMob(GameMonster.SpawnTimedMobsFromID[mob.server_id]));
			spawnTimedMob.server_id = mob.server_id;
			spawnTimedMob.spawn_time = DateTime.UtcNow.AddSeconds(spawnTimedMob.respawn_duration);
			if (!mob.is_spawn_timed_boss)
			{
				spawnTimedMob.spawn_coordinates = mob.spawn_point;
			}
			if (!GUI.spawn_timed_mobs.Exists((SpawnTimedMob x) => x.server_id == mob.server_id))
			{
				GUI.spawn_timed_mobs.Add(spawnTimedMob);
			}
		}
	}

	private static void HandleLeverOutPacket(NosPacket packet)
	{
		Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int server_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		GameEntity gameEntity = GUI.entities.Find((GameEntity x) => x.server_id == server_id);
		if (gameEntity != null)
		{
			GUI.entities.Remove(gameEntity);
		}
	}

	private static void HandleItemOutPacket(NosPacket packet)
	{
		Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int server_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		GameEntity gameEntity = GUI.entities.Find((GameEntity x) => x.server_id == server_id);
		if (gameEntity != null)
		{
			GUI.entities.Remove(gameEntity);
		}
	}

	private static void HandleItemPickedPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd)
		{
			int character_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			int server_id = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
			GameEntity gameEntity = GUI.entities.Find((GameEntity x) => x.server_id == server_id);
			if (gameEntity != null && gameEntity.id == 1046)
			{
				RaidManager.AddGold(character_id, gameEntity.quantity);
			}
			if (gameEntity != null)
			{
				GUI.entities.Remove(gameEntity);
			}
		}
	}

	private static void HandleItemThrowPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd)
		{
			int id = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
			int item_server_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			int x2 = Convert.ToInt32(packet.packet_splitted.ElementAt(5));
			int y = Convert.ToInt32(packet.packet_splitted.ElementAt(6));
			int quantity = Convert.ToInt32(packet.packet_splitted.ElementAt(7));
			GameEntity item = new GameEntity
			{
				type_name = "Item",
				type = 9,
				id = id,
				server_id = item_server_id,
				x = x2,
				y = y,
				quantity = quantity
			};
			if (!GUI.entities.Exists((GameEntity x) => x.server_id == item_server_id))
			{
				GUI.entities.Add(item);
			}
		}
	}

	private static void HandleItemDropPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd)
		{
			int id = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
			int item_server_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			int x2 = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
			int y = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
			int quantity = Convert.ToInt32(packet.packet_splitted.ElementAt(5));
			int item_owner_id = Convert.ToInt32(packet.packet_splitted.ElementAt(7));
			GameEntity item = new GameEntity
			{
				type_name = "Item",
				type = 9,
				id = id,
				server_id = item_server_id,
				x = x2,
				y = y,
				quantity = quantity,
				item_owner_id = item_owner_id
			};
			if (!GUI.entities.Exists((GameEntity x) => x.server_id == item_server_id))
			{
				GUI.entities.Add(item);
			}
		}
	}

	private static void HandleSkillUsePacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null || nostaleCharacterInfo != GUI.Mapper)
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int source_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
		int dest_id = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		int skill_id = Convert.ToInt32(packet.packet_splitted.ElementAt(5));
		Convert.ToInt32(packet.packet_splitted.ElementAt(6));
		Convert.ToInt32(packet.packet_splitted.ElementAt(7));
		Convert.ToInt32(packet.packet_splitted.ElementAt(8));
		Convert.ToInt32(packet.packet_splitted.ElementAt(9));
		Convert.ToInt32(packet.packet_splitted.ElementAt(10));
		int num3 = Convert.ToInt32(packet.packet_splitted.ElementAt(11));
		int hp_percent = Convert.ToInt32(packet.packet_splitted.ElementAt(12));
		int num4 = Convert.ToInt32(packet.packet_splitted.ElementAt(13));
		int type = Convert.ToInt32(packet.packet_splitted.ElementAt(14));
		Convert.ToInt32(packet.packet_splitted.ElementAt(15));
		int mob_current_hp = -1;
		int num5 = -1;
		if (packet.packet_splitted.Count == 18)
		{
			mob_current_hp = Convert.ToInt32(packet.packet_splitted.ElementAt(16));
			num5 = Convert.ToInt32(packet.packet_splitted.ElementAt(17));
		}
		if (packet.packet_splitted.Count == 17)
		{
			Convert.ToInt32(packet.packet_splitted.ElementAt(7));
			Convert.ToInt32(packet.packet_splitted.ElementAt(8));
			Convert.ToInt32(packet.packet_splitted.ElementAt(9));
			num3 = Convert.ToInt32(packet.packet_splitted.ElementAt(10));
			hp_percent = Convert.ToInt32(packet.packet_splitted.ElementAt(11));
			num4 = Convert.ToInt32(packet.packet_splitted.ElementAt(12));
			type = Convert.ToInt32(packet.packet_splitted.ElementAt(13));
			Convert.ToInt32(packet.packet_splitted.ElementAt(14));
			mob_current_hp = Convert.ToInt32(packet.packet_splitted.ElementAt(15));
			num5 = Convert.ToInt32(packet.packet_splitted.ElementAt(16));
		}
		if ((num == 1 || num == 2 || num == 3) && num2 == 3)
		{
			GameMonster mob = null;
			if (num == 1)
			{
				RaidManager.last_hit_character_id = source_id;
				RaidManager.AddDamage(source_id, dest_id, num4, type, skill_id, pet: false);
			}
			if (num == 2)
			{
				RaidManager.AddPetSummon(source_id);
				RaidManager.AddDamage(source_id, dest_id, num4, type, skill_id, pet: true);
			}
			if (num == 3 && num2 == 3)
			{
				mob = GUI.monsters.Find((GameMonster x) => x.server_id == dest_id);
				if (mob == null)
				{
					return;
				}
				RaidManager.handleSummon(source_id, num4, dest_id);
			}
			if (GUI.dmgContributionForm != null && GUI.dmgContributionForm.Visible && (num == 1 || num == 2) && num2 == 3 && num5 != -1)
			{
				if (num == 2)
				{
					int num6 = GUI.entities.Find((GameEntity x) => x.server_id == source_id)?.pet_owner_id ?? (-1);
					if (num6 == -1)
					{
						return;
					}
					source_id = num6;
				}
				DmgContributionCounterWindow.addPlayer(source_id);
				DmgContributionCounterWindow.addMob(dest_id, num5);
				DmgContributionCounterWindow.addDamage(source_id, dest_id, num4, num5, mob_current_hp);
				if (num3 == 0)
				{
					DmgContributionCounterWindow.removeMob(dest_id);
				}
			}
			if (num2 == 3 && num3 == 0)
			{
				mob = GUI.monsters.Find((GameMonster x) => x.server_id == dest_id);
				if (mob == null)
				{
					return;
				}
				if (mob.is_boss && GUI.Mapper?.hwnd == packet.hwnd)
				{
					RaidManager.BossKilled(mob);
				}
				RaidManager.MobKilled(mob);
				if (GUI.Mapper != null && MapID.isFamMobbingMap(GUI.Mapper.real_map_id) && (mob.is_spawn_timed || mob.is_spawn_timed_boss))
				{
					SpawnTimedMob spawnTimedMob = (mob.is_spawn_timed_boss ? GameMonster.SpawnTimedBosses[mob.id] : new SpawnTimedMob(GameMonster.SpawnTimedMobsFromID[mob.id]));
					spawnTimedMob.spawn_time = DateTime.UtcNow.AddSeconds(spawnTimedMob.respawn_duration);
					spawnTimedMob.server_id = mob.server_id;
					if (!mob.is_spawn_timed_boss)
					{
						spawnTimedMob.spawn_coordinates = mob.spawn_point;
					}
					if (!GUI.spawn_timed_mobs.Exists((SpawnTimedMob x) => x.server_id == mob.server_id))
					{
						GUI.spawn_timed_mobs.Add(spawnTimedMob);
					}
				}
			}
			mob = GUI.monsters.Find((GameMonster x) => x.server_id == dest_id);
			if (mob != null)
			{
				mob.hp_percent = hp_percent;
			}
		}
		if (num2 == 1 && num3 == 0)
		{
			RaidManager.AddDeath(dest_id);
			if (GUI.dmgContributionForm != null && GUI.dmgContributionForm.Visible)
			{
				DmgContributionCounterWindow.playerDead(dest_id);
			}
		}
		if (num == 1 && num2 == 1 && source_id != dest_id && num4 != 0)
		{
			RaidManager.MarkPlayerHittingPlayers(source_id, num3);
		}
	}

	private static void HandleFieldSpawnedPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null || nostaleCharacterInfo != GUI.Mapper)
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		int x2 = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		int y = Convert.ToInt32(packet.packet_splitted.ElementAt(5));
		if (!GameField.isField(num) && num != -1)
		{
			return;
		}
		if (num != -1)
		{
			GameField gameField = new GameField(num);
			gameField.x = x2;
			gameField.y = y;
			GUI.fields.Add(gameField);
			if (!Raids.isMovingToField && GameField.isGlacerusField(gameField.ID))
			{
				Raids.isMovingToField = true;
				Raids.MoveToGlacerusField();
			}
		}
		else
		{
			GUI.fields.Clear();
		}
	}

	private static void HandleFieldDespawnedPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo != null && nostaleCharacterInfo == GUI.Mapper)
		{
			int item = 4293;
			List<int> obj = new List<int> { item };
			int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
			int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			int num3 = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
			if (obj.Contains(num3))
			{
				GUI.fields.Clear();
			}
			if (!nostaleCharacterInfo.config.isDisabled && num == 1 && num2 == nostaleCharacterInfo.character_id && num3 == 7790)
			{
				Controller.PlaySound("Cooking");
			}
		}
	}

	public static void HandlePairyPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null || nostaleCharacterInfo != GUI.Mapper)
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		int fairy_element_id = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		Convert.ToInt32(packet.packet_splitted.ElementAt(5));
		int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(6));
		RaidManager.UpdateFairy(num, num2);
		if (GUI.Mapper.character_id != num || GUI.Mapper.hwnd != packet.hwnd)
		{
			return;
		}
		Analytics.self.fairy_id = num2;
		Analytics.self.fairy_element_id = fairy_element_id;
		int serverIdFromName = NostaleServers.GetServerIdFromName(GUI.Mapper.server);
		if (serverIdFromName != 0)
		{
			string item = $"SendPlayersOnMapEvent_{serverIdFromName}_{Analytics.self.character_id}_{Analytics.self.spID}";
			if (!Analytics.sent_rabbit_unique_ids.Contains(item) && !(Analytics.self.items == "") && Analytics.self.sp_upgrade > 0)
			{
				Analytics.SendSinglePlayerData(Analytics.self);
				Analytics.sent_rabbit_unique_ids.Add(item);
			}
		}
	}

	public static void HandleLevPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo != null && nostaleCharacterInfo == GUI.Mapper)
		{
			int lvl = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
			int clvl = Convert.ToInt32(packet.packet_splitted.ElementAt(10));
			GUI.Mapper.lvl = lvl;
			GUI.Mapper.clvl = clvl;
		}
	}

	public static void HandleBfPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null || Convert.ToInt32(packet.packet_splitted.ElementAt(1)) != 1)
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		List<string> list = packet.packet_splitted.ElementAt(3).Split(".").ToList();
		Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		int num2 = Convert.ToInt32(list.ElementAt(1));
		int num3 = Convert.ToInt32(list.ElementAt(2));
		int num4 = 1000;
		bool flag = false;
		if (num != nostaleCharacterInfo.character_id)
		{
			return;
		}
		new Random();
		if (nostaleCharacterInfo.config.potionValehir && num2 == 963 && num3 != 0)
		{
			if ((double)RaidManager.current_boss_current_hp / (double)RaidManager.current_boss_max_hp < 0.05 || (DateTime.UtcNow - nostaleCharacterInfo.lastRaidPotionUsed).TotalMilliseconds < (double)num4)
			{
				return;
			}
			nostaleCharacterInfo.lastRaidPotionUsed = DateTime.UtcNow;
			Raids.performActionQueue.Enqueue((nostaleCharacterInfo, "ValehirDebuff"));
			Raids.performAction();
		}
		if (nostaleCharacterInfo.config.potionAlzanor && num2 == 971 && num3 != 0)
		{
			if ((double)RaidManager.current_boss_current_hp / (double)RaidManager.current_boss_max_hp < 0.05 || (DateTime.UtcNow - nostaleCharacterInfo.lastRaidPotionUsed).TotalMilliseconds < (double)num4)
			{
				return;
			}
			nostaleCharacterInfo.lastRaidPotionUsed = DateTime.UtcNow;
			Raids.performActionQueue.Enqueue((nostaleCharacterInfo, "AlzanorDebuff"));
			Raids.performAction();
		}
		if (num2 == 4104 && num3 != 0)
		{
			Controller.PlaySound("Arma Bomb");
			return;
		}
		if (num2 == 4086 || (num2 == 4107 && num3 != 0))
		{
			if (nostaleCharacterInfo != null && nostaleCharacterInfo.character_id == GUI.Mapper?.character_id && num2 == 4107)
			{
				NAStyles.MapperColor = Color.Black;
			}
			if (nostaleCharacterInfo != null && nostaleCharacterInfo.character_id == GUI.Mapper?.character_id && num2 == 4086)
			{
				NAStyles.MapperColor = Color.White;
			}
			flag = true;
			Controller.PlaySound("Coiling Vines");
			return;
		}
		if (num2 == 4087 && num3 != 0)
		{
			if (nostaleCharacterInfo != null && nostaleCharacterInfo.character_id == GUI.Mapper?.character_id)
			{
				NAStyles.MapperColor = Color.GreenYellow;
			}
			flag = true;
			Controller.PlaySound("Polluted Water Prison");
			return;
		}
		if (num2 == 763 && num3 != 0)
		{
			Controller.PlaySound("Belial Reflect");
			return;
		}
		if (PlayerSP.BuffToWings.ContainsKey(Convert.ToInt32(num2)))
		{
			Analytics.UpdateSelfSPWings(nostaleCharacterInfo, PlayerSP.BuffToWings[Convert.ToInt32(num2)]);
			return;
		}
		if (num2 == 4010 && num3 != 0)
		{
			Raids.MoveToAsgobasField(8130, nostaleCharacterInfo.hwnd);
		}
		if (num2 == 4011 && num3 != 0)
		{
			Raids.MoveToAsgobasField(8131, nostaleCharacterInfo.hwnd);
		}
		if (num2 == 4012 && num3 != 0)
		{
			Raids.MoveToAsgobasField(8132, nostaleCharacterInfo.hwnd);
		}
		if (num2 == 4013 && num3 != 0)
		{
			Raids.MoveToAsgobasField(8133, nostaleCharacterInfo.hwnd);
		}
		if (num2 == 4138 && num3 != 0)
		{
			if (nostaleCharacterInfo != null && nostaleCharacterInfo.character_id == GUI.Mapper?.character_id)
			{
				NAStyles.MapperColor = Color.DarkBlue;
			}
			flag = true;
			Controller.PlaySound("Ultimate Arma Color - Blue");
			return;
		}
		if (num2 == 4137 && num3 != 0)
		{
			if (nostaleCharacterInfo != null && nostaleCharacterInfo.character_id == GUI.Mapper?.character_id)
			{
				NAStyles.MapperColor = Color.Red;
			}
			flag = true;
			Controller.PlaySound("Ultimate Arma Color - Red");
			return;
		}
		if (nostaleCharacterInfo != null && nostaleCharacterInfo.character_id == GUI.Mapper?.character_id && !flag)
		{
			NAStyles.MapperColor = NAStyles.NotActiveCharColor;
		}
		if (nostaleCharacterInfo != null)
		{
			Analytics.HandleTattoosBuff(list, nostaleCharacterInfo.character_id);
		}
	}

	public static void HandleCtPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo character = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (character == null)
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		Convert.ToInt32(packet.packet_splitted.ElementAt(3));
		Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(7));
		if (num != 3)
		{
			return;
		}
		if (num2 == 1432 && character.config.useFullBeforeEreniaManuk)
		{
			Task.Run(async delegate
			{
				await Task.Delay(new Random().Next(400, 700));
				if (character.current_hp < character.max_hp)
				{
					Raids.useFullHPPotion(character);
				}
			});
		}
		if (character == GUI.Mapper)
		{
			switch (num2)
			{
			case 1432:
				Controller.PlaySound("Erenia Cry");
				break;
			case 1460:
				Controller.PlaySound("Fernon Reflect");
				break;
			case 669:
				Controller.PlaySound("Carno Grab");
				break;
			case 1005:
				Controller.PlaySound("Paimon Miniboss");
				break;
			case 1683:
				Controller.PlaySound("Alzanor Wind");
				Raids.MoveToClosestPillar();
				break;
			}
		}
	}

	private static void HandleStPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null || nostaleCharacterInfo != GUI.Mapper)
		{
			return;
		}
		switch (Convert.ToInt32(packet.packet_splitted.ElementAt(1)))
		{
		case 3:
		{
			int mob_server_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			int num = Convert.ToInt32(packet.packet_splitted.ElementAt(8));
			int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(10));
			PetTrainerMob petTrainerMob = Miniland.pet_trainer_mobs_list.Find((PetTrainerMob x) => x.mob_server_id == mob_server_id);
			if (petTrainerMob != null)
			{
				petTrainerMob.spawn_time = DateTime.UtcNow.AddSeconds(-(num2 - num));
				petTrainerMob.despawn_time = DateTime.UtcNow.AddSeconds(num);
			}
			break;
		}
		case 1:
		{
			int character_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			Convert.ToInt32(packet.packet_splitted.ElementAt(3));
			Convert.ToInt32(packet.packet_splitted.ElementAt(4));
			Convert.ToInt32(packet.packet_splitted.ElementAt(5));
			Convert.ToInt32(packet.packet_splitted.ElementAt(6));
			Convert.ToInt32(packet.packet_splitted.ElementAt(7));
			Convert.ToInt32(packet.packet_splitted.ElementAt(8));
			Convert.ToInt32(packet.packet_splitted.ElementAt(9));
			Convert.ToInt32(packet.packet_splitted.ElementAt(10));
			bool flag = false;
			bool valehirDebuff = false;
			bool bellialDebuff = false;
			bool flag2 = false;
			NostaleCharacterInfo nostaleCharacterInfo2 = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.character_id == character_id);
			GamePlayer gamePlayer = GUI.players.Find((GamePlayer x) => x.character_id == character_id);
			foreach (string item in packet.packet_splitted.Skip(11).ToList())
			{
				if (item.StartsWith(116.ToString()))
				{
					flag = true;
				}
				else if (item.StartsWith(4107.ToString()))
				{
					if (gamePlayer != null)
					{
						gamePlayer.color = Color.Black;
					}
					if (nostaleCharacterInfo2 != null && nostaleCharacterInfo2.character_id != GUI.Mapper.character_id)
					{
						nostaleCharacterInfo2.special_color = Color.Black;
					}
					flag2 = true;
				}
				else if (item.StartsWith(4086.ToString()))
				{
					if (gamePlayer != null)
					{
						gamePlayer.color = Color.White;
					}
					if (nostaleCharacterInfo2 != null && nostaleCharacterInfo2.character_id != GUI.Mapper.character_id)
					{
						nostaleCharacterInfo2.special_color = Color.White;
					}
					flag2 = true;
				}
				else if (item.StartsWith(4087.ToString()))
				{
					if (gamePlayer != null)
					{
						gamePlayer.color = Color.GreenYellow;
					}
					if (nostaleCharacterInfo2 != null && nostaleCharacterInfo2.character_id != GUI.Mapper.character_id)
					{
						nostaleCharacterInfo2.special_color = Color.GreenYellow;
					}
					flag2 = true;
				}
				else if (item.StartsWith(963.ToString()))
				{
					valehirDebuff = true;
				}
				else if (item.StartsWith(763.ToString()))
				{
					bellialDebuff = true;
				}
				else if (item.StartsWith(4138.ToString()))
				{
					if (gamePlayer != null)
					{
						gamePlayer.color = Color.DarkBlue;
					}
					if (nostaleCharacterInfo2 != null && nostaleCharacterInfo2.character_id != GUI.Mapper.character_id)
					{
						nostaleCharacterInfo2.special_color = Color.DarkBlue;
					}
					flag2 = true;
				}
				else if (item.StartsWith(4137.ToString()))
				{
					if (gamePlayer != null)
					{
						gamePlayer.color = Color.Red;
					}
					if (nostaleCharacterInfo2 != null && nostaleCharacterInfo2.character_id != GUI.Mapper.character_id)
					{
						nostaleCharacterInfo2.special_color = Color.Red;
					}
					flag2 = true;
				}
				else
				{
					List<string> list = item.Split(".").ToList();
					if (PlayerSP.BuffToWings.ContainsKey(Convert.ToInt32(list.ElementAt(0))))
					{
						Analytics.UpdatePlayersSPWings(character_id, PlayerSP.BuffToWings[Convert.ToInt32(list.ElementAt(0))]);
					}
					else
					{
						Analytics.HandleTattoosBuff(list, character_id);
					}
				}
			}
			if (gamePlayer != null && !flag2)
			{
				gamePlayer.color = NAStyles.PlayersColor;
			}
			if (nostaleCharacterInfo2 != null && nostaleCharacterInfo2.character_id != GUI.Mapper.character_id && !flag2)
			{
				nostaleCharacterInfo2.special_color = null;
			}
			RaidManager.UpdateEffects(character_id, flag, valehirDebuff, bellialDebuff);
			if (!flag && nostaleCharacterInfo2 != null && nostaleCharacterInfo2.inRaid && nostaleCharacterInfo2.config.attackPotion && RaidManager.raidStarted && nostaleCharacterInfo2.SPCard.ID <= 54)
			{
				Raids.useItem(nostaleCharacterInfo2.hwnd, new List<int> { 8020, 246 }, "Attack Potion", isItem: true, restrict: true, Utils.randomizeDelay(Settings.config.DelaySettings.Items));
			}
			break;
		}
		}
	}

	public static void HandleCMapPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null)
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		if (Convert.ToInt32(packet.packet_splitted.ElementAt(3)) == 0)
		{
			return;
		}
		nostaleCharacterInfo.real_map_id = num;
		if (nostaleCharacterInfo == GUI.Mapper)
		{
			if (QuestManager.navigating_instance_map_id == -1 && GUI.Mapper.real_map_id != -1)
			{
				QuestManager.UpdateQuestTarget();
			}
			else if (GUI.Mapper.real_map_id != -1)
			{
				QuestManager.NavigateToNonQuest();
			}
			GUI.UpdateMapBottomPanel();
		}
		if (RaidManager.currentRaid == 30 && num == 2649 && RaidManager.raidStarted && Settings.config.WaypointsConfig.KirolasStart)
		{
			Raids.performActionQueue.Enqueue((nostaleCharacterInfo, "MoveKirolas"));
			Raids.performAction();
		}
		if (nostaleCharacterInfo == GUI.Mapper && MapID.isFamMobbingMap(num))
		{
			NAStyles.SetTSStonesData();
		}
	}

	public static void HandleGPPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo != null && nostaleCharacterInfo == GUI.Mapper)
		{
			int x = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
			int y = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			int portal_target_map_id = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
			int state = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
			GameEntity gameEntity = new GameEntity
			{
				x = x,
				y = y,
				type_name = "portal",
				portal_target_map_id = portal_target_map_id
			};
			if (GUI.entities.Find((GameEntity p) => p.x == x && p.y == y && p.type_name == "portal") == null)
			{
				GUI.entities.Add(gameEntity);
			}
			if (TimeSpaceManager.ts_started)
			{
				TimeSpaceManager.AddPortal(gameEntity, state);
			}
		}
	}

	public static void HandleTPPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null || nostaleCharacterInfo != GUI.Mapper)
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int server_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
		int num3 = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		switch (num)
		{
		case 1:
		{
			NostaleCharacterInfo nostaleCharacterInfo2 = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.character_id == server_id);
			if (nostaleCharacterInfo2 != null)
			{
				nostaleCharacterInfo2.x_pos = num2;
				nostaleCharacterInfo2.y_pos = num3;
				Raids.handleTeleport(nostaleCharacterInfo2, num2, num3);
			}
			GamePlayer gamePlayer = GUI.players.Find((GamePlayer x) => x.character_id == server_id);
			if (gamePlayer != null)
			{
				gamePlayer.x = num2;
				gamePlayer.y = num3;
			}
			break;
		}
		case 3:
		{
			GameMonster gameMonster = GUI.monsters.Find((GameMonster x) => x.server_id == server_id);
			if (gameMonster != null)
			{
				gameMonster.x = num2;
				gameMonster.y = num3;
			}
			break;
		}
		}
	}

	public static void HandleGuriPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null || packet.packet_splitted.Count < 3 || (nostaleCharacterInfo.SPCard.ID != 35 && nostaleCharacterInfo.SPCard.ID != 36))
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		int num3 = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
		int num4 = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		if (num3 == nostaleCharacterInfo.character_id && !nostaleCharacterInfo.config.isDisabled && num == 6 && num2 == 1)
		{
			switch (num4)
			{
			case 30:
				Controller.PlaySound("Normal Fish");
				break;
			case 31:
				Controller.PlaySound("Rare Fish");
				break;
			}
		}
	}

	public static void concatRaidListFPackets(NosPacket packet)
	{
		if (last_rdlstf_packet == null)
		{
			return;
		}
		foreach (string item in packet.packet_splitted.Skip(5).ToList())
		{
			NosPacket? nosPacket = last_rdlstf_packet;
			nosPacket.content = nosPacket.content + " " + item;
		}
		packet.content = last_rdlstf_packet.content;
		packet.packet_splitted = packet.content.Split(" ").ToList();
		is_last_packet_rdlstf = false;
		last_rdlstf_packet = null;
	}

	public static void HandleRaidListFPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd && (is_last_packet_rdlstf || Convert.ToInt32(packet.packet_splitted.ElementAt(3)) == 0))
		{
			last_rdlstf_packet = packet;
			is_last_packet_rdlstf = true;
		}
	}

	public static void HandleDiePacket(NosPacket packet)
	{
		if (GUI.Mapper == null || packet.hwnd != GUI.Mapper.hwnd)
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int source_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
		Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		if (num == 3 && num2 == 3)
		{
			GameMonster gameMonster = GUI.monsters.Find((GameMonster x) => x.server_id == source_id);
			if (gameMonster != null)
			{
				GUI.monsters.Remove(gameMonster);
			}
		}
		if (num == 1)
		{
			RaidManager.AddDeath(source_id);
		}
	}

	public static async void AbortRaidJoining(string error_msg, List<NostaleCharacterInfo> expected_raiders)
	{
		Raids.allowJoining = false;
		Raids.joining = false;
		Raids.readyRaiders.Clear();
		await Task.Delay(300);
		Controller.MultiButtonPress(expected_raiders, Keys.Escape);
		GUI.ShowPopUp(error_msg);
	}

	public static void HandleMsgiPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd)
		{
			if (packet.packet_splitted.Count >= 3 && (packet.packet_splitted.ElementAt(2) == "1287" || packet.packet_splitted.ElementAt(2) == "2559"))
			{
				ICManager.IncreaseRound();
			}
			if (packet.packet_splitted.Count >= 3 && packet.packet_splitted.ElementAt(2) == "383" && ICManager.ICStarted != 0)
			{
				ICManager.InstantCombatFinished();
			}
		}
	}

	public static void HandleDlgiPacket(NosPacket packet)
	{
		
		if (GUI.AutoRespawn)
		{
			NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
			if (nostaleCharacterInfo != null && !nostaleCharacterInfo.config.isAttacker && !nostaleCharacterInfo.config.isDisabled && packet.packet_splitted.Count >= 3 && packet.packet_splitted.ElementAt(1) == "#revival^8" && packet.packet_splitted.ElementAt(2) == "#revival^9")
			{
				Miniland.Respawn(nostaleCharacterInfo);
			}
		}
	}

	public static void HandleRbossPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo != null && nostaleCharacterInfo == GUI.Mapper)
		{
			Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			int current_boss_current_hp = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
			int current_boss_max_hp = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
			Convert.ToInt32(packet.packet_splitted.ElementAt(5));
			RaidManager.current_boss_current_hp = current_boss_current_hp;
			RaidManager.current_boss_max_hp = current_boss_max_hp;
		}
	}

	public static void HandleE_InfoPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null)
		{
			return;
		}
		int serverIdFromName = NostaleServers.GetServerIdFromName(nostaleCharacterInfo.server);
		if (serverIdFromName == 0)
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		if (GameEquipementItem.isCostume(packet.content))
		{
			return;
		}
		switch (num)
		{
		case 0:
		case 1:
		case 2:
		case 5:
		{
			GameEquipementItem gameEquipementItem = new GameEquipementItem(packet.content, serverIdFromName, nostaleCharacterInfo.character_id);
			RabbitEventHandler.SendEquipementItem(gameEquipementItem);
			if (gameEquipementItem.rune_string != null && (gameEquipementItem.rarity >= 5 || gameEquipementItem.type != 2))
			{
				new GameRune(gameEquipementItem.rune_string);
			}
			break;
		}
		case 4:
			Analytics.HandleFairyDetails(packet.content, nostaleCharacterInfo, nostaleCharacterInfo.character_id);
			break;
		}
	}

	public static void HandleSayiPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo != null)
		{
			List<string> list = new List<string>();
			CollectionsMarshal.SetCount(list, 2);
			Span<string> span = CollectionsMarshal.AsSpan(list);
			int num = 0;
			span[num] = "2037";
			num++;
			span[num] = "2044";
			num++;
			List<string> list2 = list;
			if (packet.packet_splitted.ElementAt(1) == "1" && packet.packet_splitted.ElementAt(3) == "10" && list2.Contains(packet.packet_splitted.ElementAt(4)))
			{
				GUI.ShowPopUp(nostaleCharacterInfo.nickname + " could not join the raid!");
			}
			_ = packet.packet_splitted.ElementAt(1) == "1";
		}
	}

	public static async void HandleInfoiPacket(NosPacket packet)
	{
		if (GUI.Inviter != null && packet.hwnd == GUI.Inviter.hwnd && packet.content == "infoi 310 0 0 0")
		{
			await Task.Delay(Utils.randomizeDelay(Settings.config.DelaySettings.Invite / 2));
			Controller.SingleButtonPress(packet.hwnd, Keys.Escape);
			await Task.Delay(Utils.randomizeDelay(Settings.config.DelaySettings.Invite / 4));
			Controller.SingleButtonPress(packet.hwnd, Keys.Return);
		}
	}

	public static async void HandleMinilandInvitePacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo != null && Settings.config.autoconfirm && !nostaleCharacterInfo.config.isDisabled)
		{
			await Task.Delay(Utils.randomizeDelay(Settings.config.DelaySettings.Invite));
			Controller.SingleButtonPress(packet.hwnd, Keys.Return);
		}
	}

	public static void HandleEqPacket(NosPacket packet)
	{
		
		GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (GUI.Mapper == null || GUI.Mapper.hwnd != packet.hwnd)
		{
			return;
		}
		int character_id = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		string text = packet.packet_splitted.ElementAt(7);
		string weapon_upgrade = packet.packet_splitted.ElementAt(8);
		string armor_upgrade = packet.packet_splitted.ElementAt(9);
		List<string> source = text.Split(".").ToList();
		if (GUI.Mapper != null && GUI.Mapper.character_id == character_id)
		{
			List<string> list = Analytics.self.items.Split(".").ToList();
			if (list.Count != 11 || (!(list.ElementAt(2) != source.ElementAt(2)) && !(list.ElementAt(7) != source.ElementAt(7))))
			{
				Analytics.self.items = text;
				Analytics.self.weapon_upgrade = weapon_upgrade;
				Analytics.self.armor_upgrade = armor_upgrade;
			}
			return;
		}
		GamePlayer gamePlayer = GUI.players.Find((GamePlayer x) => x.character_id == character_id);
		if (gamePlayer != null)
		{
			List<string> source2 = gamePlayer.items.Split(".").ToList();
			if (!(source2.ElementAt(2) != source.ElementAt(2)) && !(source2.ElementAt(7) != source.ElementAt(7)))
			{
				gamePlayer.items = text;
				gamePlayer.weapon_upgrade = weapon_upgrade;
				gamePlayer.armor_upgrade = armor_upgrade;
			}
		}
	}

	public static void HandleSayitemtPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null)
		{
			return;
		}
		int serverIdFromName = NostaleServers.GetServerIdFromName(nostaleCharacterInfo.server);
		if (serverIdFromName == 0)
		{
			return;
		}
		if (packet.content.Contains("e_info"))
		{
			int num = packet.content.IndexOf("e_info");
			string text = packet.content.Substring(num, packet.content.Length - num);
			int num2 = Convert.ToInt32(text.Split(" ").ElementAt(1));
			if (GameEquipementItem.isCostume(text))
			{
				return;
			}
			switch (num2)
			{
			case 0:
			case 1:
			case 2:
			case 5:
			{
				int sender_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
				GameEquipementItem gameEquipementItem = new GameEquipementItem(text, serverIdFromName, sender_id);
				if ((gameEquipementItem.type != 2 || gameEquipementItem.rarity >= 5) && !Analytics.sent_rabbit_unique_ids.Contains(gameEquipementItem.unique_id))
				{
					Analytics.sent_rabbit_unique_ids.Add(gameEquipementItem.unique_id);
					RabbitEventHandler.SendEquipementItem(gameEquipementItem);
				}
				break;
			}
			case 4:
			{
				int owner_id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
				Analytics.HandleFairyDetails(text, nostaleCharacterInfo, owner_id);
				break;
			}
			}
		}
		else if (packet.content.Contains("slinfo"))
		{
			int owner_id2 = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			int num3 = packet.content.IndexOf("slinfo");
			Analytics.SendSPCard(packet.content.Substring(num3, packet.content.Length - num3), nostaleCharacterInfo, owner_id2);
		}
	}

	public static void HandleSlinfoPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo != null)
		{
			Analytics.SendSPCard(packet.content, nostaleCharacterInfo, nostaleCharacterInfo.character_id);
		}
	}

	public static void HandleTitInfoPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd)
		{
			Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			int title = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
			int value = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
			Analytics.self.title = title;
			Analytics.self.real_title = value;
		}
	}

	public static void HandleGidxPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd && Convert.ToInt32(packet.packet_splitted.ElementAt(1)) == 1 && Convert.ToInt32(packet.packet_splitted.ElementAt(2)) == GUI.Mapper.character_id)
		{
			Analytics.self.family_lvl = Convert.ToInt32(packet.packet_splitted.ElementAt(5));
		}
	}

	public static void HandleAct6Packet(NosPacket packet)
	{
		
		if (last_status_bar_event_set.AddSeconds(10.0) > DateTime.UtcNow)
		{
			return;
		}
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null || packet.packet_splitted.Count < 11)
		{
			return;
		}
		int value = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
		int value2 = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		int value3 = Convert.ToInt32(packet.packet_splitted.ElementAt(5));
		int value4 = Convert.ToInt32(packet.packet_splitted.ElementAt(6));
		int value5 = Convert.ToInt32(packet.packet_splitted.ElementAt(7));
		int value6 = Convert.ToInt32(packet.packet_splitted.ElementAt(8));
		int value7 = Convert.ToInt32(packet.packet_splitted.ElementAt(9));
		int value8 = Convert.ToInt32(packet.packet_splitted.ElementAt(10));
		string data = $"{value} {value2} {value3} {value4} {value5} {value6} {value7} {value8}";
		int serverIdFromName = NostaleServers.GetServerIdFromName(nostaleCharacterInfo.server);
		if (serverIdFromName != 0)
		{
			int? channel = nostaleCharacterInfo.channel;
			if (channel.HasValue && channel != 0)
			{
				RabbitEventHandler.SendBarStatusEvent(new BarStatusDto
				{
					data = data,
					server_id = serverIdFromName,
					channel = channel.Value,
					type = "act6",
					unique_id = $"SendBarStatusEvent_{serverIdFromName}_{channel}_act6"
				});
				last_status_bar_event_set = DateTime.UtcNow;
			}
		}
	}

	public static void HandleSc_pPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd)
		{
			int pet_index = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
			int id = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			int pet_server_id = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
			int lvl = Convert.ToInt32(packet.packet_splitted.ElementAt(35));
			int num = Convert.ToInt32(packet.packet_splitted.ElementAt(36));
			int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(39));
			int num3 = Convert.ToInt32(packet.packet_splitted.ElementAt(40));
			MinilandPet minilandPet = Miniland.pets_list.Find((MinilandPet x) => x.pet_server_id == pet_server_id);
			if (minilandPet == null)
			{
				Miniland.pets_list.Add(new MinilandPet
				{
					pet_index = pet_index,
					pet_server_id = pet_server_id,
					lvl = lvl,
					clvl = num,
					current_xp = num2,
					max_xp = num3,
					last_update = DateTime.UtcNow,
					xp_changed = false,
					icon = GameMonster.GetIcon(id)
				});
			}
			else if (minilandPet.clvl != num || minilandPet.current_xp != num2 || minilandPet.max_xp != num3)
			{
				minilandPet.xp_changed = true;
				minilandPet.last_update = DateTime.UtcNow;
				minilandPet.lvl = lvl;
				minilandPet.clvl = num;
				minilandPet.current_xp = num2;
				minilandPet.max_xp = num3;
			}
		}
	}

	public static void HandleMlInfoPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd)
		{
			Miniland.isInOwnMiniland = true;
		}
	}

	private static void HandleFCPacket(NosPacket packet)
	{
		
		if (last_status_bar_event_set.AddSeconds(10.0) > DateTime.UtcNow)
		{
			return;
		}
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null || packet.packet_splitted.Count != 21)
		{
			return;
		}
		int value = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
		int value2 = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		int value3 = Convert.ToInt32(packet.packet_splitted.ElementAt(5));
		int value4 = Convert.ToInt32(packet.packet_splitted.ElementAt(6));
		int value5 = Convert.ToInt32(packet.packet_splitted.ElementAt(12));
		int value6 = Convert.ToInt32(packet.packet_splitted.ElementAt(13));
		int value7 = Convert.ToInt32(packet.packet_splitted.ElementAt(14));
		int value8 = Convert.ToInt32(packet.packet_splitted.ElementAt(15));
		string data = $"{value} {value2} {value3} {value4} {value5} {value6} {value7} {value8}";
		int serverIdFromName = NostaleServers.GetServerIdFromName(nostaleCharacterInfo.server);
		if (serverIdFromName != 0)
		{
			int? channel = nostaleCharacterInfo.channel;
			if (channel.HasValue && channel != 0)
			{
				RabbitEventHandler.SendBarStatusEvent(new BarStatusDto
				{
					data = data,
					server_id = serverIdFromName,
					channel = channel.Value,
					type = "fc",
					unique_id = $"SendBarStatusEvent_{serverIdFromName}_{channel}_fc"
				});
				last_status_bar_event_set = DateTime.UtcNow;
			}
		}
	}

	private static void HandleEvntPacket(NosPacket packet)
	{
		if (GUI.Mapper == null || packet.hwnd != GUI.Mapper.hwnd)
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		int num3 = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
		int num4 = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
		if (RaidManager.IsInUltimateArmaBossroom)
		{
			if (num == 3 && num2 == 0 && num3 == 600 && num4 == 600 && !RaidManager.UltArmaBoxesSpawn)
			{
				RaidManager.UltArmaBoxesSpawn = true;
			}
			else if (num == 3 && num2 == 1 && num3 == -1 && num4 == -1 && RaidManager.UltArmaBoxesSpawn)
			{
				RaidManager.UltArmaBoxesSpawn = false;
			}
		}
		if (TimeSpaceManager.ts_started)
		{
			if (num == 3 && num3 > 0 && num3 == num4)
			{
				TimeSpaceManager.MarkTimeRoom();
			}
			if (num == 1 && num2 != -1 && num3 == num4 && num3 > 0)
			{
				TimeSpaceManager.MarkBonusTimeRoom();
			}
		}
	}

	private static void HandleQstlistPacket(NosPacket packet)
	{
		
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == packet.hwnd);
		if (nostaleCharacterInfo == null)
		{
			return;
		}
		if (packet.packet_splitted.Count == 1)
		{
			List<GameQuest> list = new List<GameQuest>();
			nostaleCharacterInfo.config.quests = list;
			Settings.SaveSettings();
			if (nostaleCharacterInfo == GUI.Mapper)
			{
				QuestManager.UpdateQuestList(list);
			}
		}
		else
		{
			if (packet.packet_splitted.Count < 2)
			{
				return;
			}
			List<GameQuest> list2 = new List<GameQuest>();
			for (int i = 1; i < packet.packet_splitted.Count; i++)
			{
				List<string> list3 = packet.packet_splitted.ElementAt(i).Split(".").ToList();
				int questPosition = Convert.ToInt32(list3.ElementAt(0));
				int questID = Convert.ToInt32(list3.ElementAt(1));
				int idk = Convert.ToInt32(list3.ElementAt(2));
				int questType = Convert.ToInt32(list3.ElementAt(3));
				int item = Convert.ToInt32(list3.ElementAt(4));
				int item2 = Convert.ToInt32(list3.ElementAt(5));
				int completed = Convert.ToInt32(list3.ElementAt(6));
				GameQuest gameQuest = new GameQuest
				{
					questPosition = questPosition,
					questID = questID,
					questType = questType,
					idk1 = idk,
					completed = completed
				};
				gameQuest.QuestProgress.Add((item, item2));
				if (list3.Count > 8)
				{
					int item3 = Convert.ToInt32(list3.ElementAt(7));
					int num = Convert.ToInt32(list3.ElementAt(8));
					if (num != 0)
					{
						gameQuest.QuestProgress.Add((item3, num));
					}
				}
				if (list3.Count > 10)
				{
					int item4 = Convert.ToInt32(list3.ElementAt(9));
					int num2 = Convert.ToInt32(list3.ElementAt(10));
					if (num2 != 0)
					{
						gameQuest.QuestProgress.Add((item4, num2));
					}
				}
				if (list3.Count > 12)
				{
					int item5 = Convert.ToInt32(list3.ElementAt(11));
					int num3 = Convert.ToInt32(list3.ElementAt(12));
					if (num3 != 0)
					{
						gameQuest.QuestProgress.Add((item5, num3));
					}
				}
				list2.Add(gameQuest);
			}
			nostaleCharacterInfo.config.quests = list2;
			Settings.SaveSettings();
			if (nostaleCharacterInfo == GUI.Mapper)
			{
				QuestManager.UpdateQuestList(list2);
				if (QuestManager.questline_position != -1)
				{
					QuestManager.UpdateCurrentQuest(QuestManager.questline_position);
				}
			}
		}
	}

	private static void HandleQstiPacket(NosPacket packet)
	{
		if (GUI.Mapper == null || packet.hwnd != GUI.Mapper.hwnd)
		{
			return;
		}
		List<string> source = packet.packet_splitted.ElementAt(1).Split(".").ToList();
		Convert.ToInt32(source.ElementAt(0));
		int quest_ID = Convert.ToInt32(source.ElementAt(1));
		Convert.ToInt32(source.ElementAt(2));
		GameQuest gameQuest = QuestManager.quests.Find((GameQuest x) => x.questID == quest_ID);
		if (gameQuest != null)
		{
			if (gameQuest.QuestProgress.Count > 0)
			{
				int item = Convert.ToInt32(source.ElementAt(4));
				int item2 = Convert.ToInt32(source.ElementAt(5));
				gameQuest.QuestProgress[0] = (item, item2);
			}
			if (gameQuest.QuestProgress.Count > 1)
			{
				int item3 = Convert.ToInt32(source.ElementAt(7));
				int item4 = Convert.ToInt32(source.ElementAt(8));
				gameQuest.QuestProgress[1] = (item3, item4);
			}
			if (gameQuest.QuestProgress.Count > 2)
			{
				int item5 = Convert.ToInt32(source.ElementAt(9));
				int item6 = Convert.ToInt32(source.ElementAt(10));
				gameQuest.QuestProgress[2] = (item5, item6);
			}
			if (gameQuest.QuestProgress.Count > 3)
			{
				int item7 = Convert.ToInt32(source.ElementAt(11));
				int item8 = Convert.ToInt32(source.ElementAt(12));
				gameQuest.QuestProgress[3] = (item7, item8);
			}
			if (QuestManager.followed_quest != null && gameQuest.questID == QuestManager.followed_quest.questID && (gameQuest.questType == 1 || gameQuest.questType == 17))
			{
				QuestManager.UpdateQuestTarget();
			}
		}
	}

	private static void HandleWPPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd)
		{
			int x2 = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
			int y = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			int id = Convert.ToInt32(packet.packet_splitted.ElementAt(3));
			int state = Convert.ToInt32(packet.packet_splitted.ElementAt(4));
			int min_lvl = Convert.ToInt32(packet.packet_splitted.ElementAt(5));
			int max_lvl = Convert.ToInt32(packet.packet_splitted.ElementAt(6));
			if (GUI.time_spaces.Find((GameTimeSpace x) => x.ID == id) == null)
			{
				GUI.time_spaces.Add(new GameTimeSpace
				{
					ID = id,
					x = x2,
					y = y,
					state = state,
					min_lvl = min_lvl,
					max_lvl = max_lvl
				});
			}
		}
	}

	private static void HandleRbrPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd)
		{
			int iD = Convert.ToInt32(packet.packet_splitted.ElementAt(1).Split(".").ElementAt(0));
			int min_lvl = Convert.ToInt32(packet.packet_splitted.ElementAt(4).Split(".").ElementAt(0));
			int max_lvl = Convert.ToInt32(packet.packet_splitted.ElementAt(4).Split(".").ElementAt(1));
			TimeSpaceManager.current_ts = new TimeSpaceData
			{
				ID = iD,
				min_lvl = min_lvl,
				max_lvl = max_lvl
			};
			TimeSpaceManager.starting_ts = true;
		}
	}

	private static void HandleRsfnPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd)
		{
			int room_x = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
			int room_y = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			TimeSpaceManager.AddRoom(room_x, room_y);
		}
	}

	private static void HandleRsfpPacket(NosPacket packet)
	{
		if (GUI.Mapper == null || packet.hwnd != GUI.Mapper.hwnd)
		{
			return;
		}
		int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
		int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
		if (num == 0 && num2 == -1)
		{
			TimeSpaceManager.FinishTimeSpace(save: false);
			return;
		}
		if (TimeSpaceManager.starting_ts)
		{
			TimeSpaceManager.StartTimeSpace(num, num2);
		}
		TimeSpaceManager.UpdatePlayersPostion(num, num2);
	}

	private static void HandleRsfiPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd && TimeSpaceManager.ts_started)
		{
			TimeSpaceManager.FinishTimeSpace(save: false);
		}
	}

	private static void HandleRsfmPacket(NosPacket packet)
	{
		if (GUI.Mapper != null && packet.hwnd == GUI.Mapper.hwnd && TimeSpaceManager.ts_started)
		{
			int num = Convert.ToInt32(packet.packet_splitted.ElementAt(1));
			int num2 = Convert.ToInt32(packet.packet_splitted.ElementAt(2));
			if (TimeSpaceManager.ts_started)
			{
				TimeSpaceManager.current_ts.ID = num;
			}
			if (num == TimeSpaceManager.current_ts.ID && num2 == 2)
			{
				TimeSpaceManager.MarkKillMobsRoom();
			}
		}
	}

	public static void ResetMapperData()
	{
		GUI.entities.Clear();
		GUI.monsters.Clear();
		GUI.players.Clear();
		GUI.fields.Clear();
		GUI.spawn_timed_mobs.Clear();
		GUI.time_spaces.Clear();
		Analytics.self = new GamePlayer();
		RaidManager.IsInUltimateArmaBossroom = false;
		RaidManager.UltArmaBoxesSpawn = false;
	}
}
