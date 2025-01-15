using System;
using System.Text;
using System.Threading.Tasks;
using NetCoreServer;
using Newtonsoft.Json;
using NosAssistant2.Dtos.Input;

namespace NosAssistant2.Helpers;

public class NATcpClient : TcpClient
{
	private bool _stop;

	public static NATcpClient Instance { get; private set; }

	public event Action<object> OnMessageReceived;

	public NATcpClient(string address, int port)
		: base(address, port)
	{
	}

	public static void Initialize(string address, int port)
	{
		if (Instance == null)
		{
			Instance = new NATcpClient(address, port);
			Task.Run(() => Instance.ConnectAsync());
		}
	}

	public void Stop()
	{
		_stop = true;
		Disconnect();
	}

	public void SendMessage(object message)
	{
		if (base.IsConnected)
		{
			string s = JsonConvert.SerializeObject(message);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			SendAsync(bytes);
		}
	}

	protected override void OnConnected()
	{
	}

	protected override void OnDisconnected()
	{
		if (_stop)
		{
			return;
		}
		Task.Run(async delegate
		{
			await Task.Delay(1000);
			if (!_stop)
			{
				ConnectAsync();
			}
		});
	}

	protected override void OnReceived(byte[] buffer, long offset, long size)
	{
		string text = CryptoHelper.DecryptedString(Encoding.UTF8.GetString(buffer, (int)offset, (int)size));
		if (text == "Startup")
		{
			ConnectDto value = new ConnectDto
			{
				hostname = Environment.MachineName,
				ip = "0.0.0.0",
				username = "test"
			};
			SendAsync(CryptoHelper.EncryptedString(JsonConvert.SerializeObject(value)));
		}
		this.OnMessageReceived?.Invoke(text);
	}

	protected override void OnError(System.Net.Sockets.SocketError error)
	{
	}
}
