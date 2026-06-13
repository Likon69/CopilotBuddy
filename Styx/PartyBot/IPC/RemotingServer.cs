using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using Styx.Helpers;
using Styx.RemotableObjects;

namespace PartyBot.IPC
{
	/// <summary>
	/// TCP server on port 1337 — port of ns18.Class23 (RemotingServer).
	/// Accepts connections from RemotingClient(s) and streams the latest BotMessage as JSON.
	/// Replaces System.Runtime.Remoting.Channels.TcpChannel which is .NET Framework only.
	/// </summary>
	public class RemotingServer : IObserver
	{
		public RemotingServer()
		{
			_listener = new TcpListener(IPAddress.Loopback, 1337);
			try
			{
				_listener.Start();
			}
			catch (SocketException ex)
			{
				Logging.Write("Channel is busy! Remoting Server already started.");
				Logging.WriteException(ex);
				return;
			}

			Cache.Instance.SetObserver(this);

			_thread = new Thread(AcceptLoop)
			{
				IsBackground = true,
				Name = "RemotingServerAcceptLoop"
			};
			_thread.Start();
			Logging.Write("Remoting server started");
		}

		/// <summary>
		/// Called by LeaderPlugin each Pulse to push the current BotMessage.
		/// Equivalent to Class23.method_0 → botMessage_0.SetMessage(msg).
		/// </summary>
		public void SetMessage(BotMessage message)
		{
			lock (_lock)
				_current = message;
		}

		/// <summary>IObserver.Notify — called by Cache when message changes.</summary>
		public void Notify(BotMessage message)
		{
			lock (_lock)
				_current = message;
		}

		private void AcceptLoop()
		{
			while (true)
			{
				try
				{
					TcpClient client = _listener.AcceptTcpClient();
					Thread clientThread = new Thread(() => ServeClient(client))
					{
						IsBackground = true,
						Name = "RemotingServerClient"
					};
					clientThread.Start();
				}
				catch (Exception ex)
				{
					Logging.WriteException(ex);
					Thread.Sleep(500);
				}
			}
		}

		private void ServeClient(TcpClient client)
		{
			try
			{
				using (client)
				using (NetworkStream stream = client.GetStream())
				using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
				{
					while (client.Connected)
					{
						BotMessage? msg;
						lock (_lock)
							msg = _current;

						if (msg != null)
							writer.WriteLine(JsonSerializer.Serialize(msg, _jsonOptions));

						Thread.Sleep(76);
					}
				}
			}
			catch (Exception)
			{
				// client disconnected — normal
			}
		}

		private readonly TcpListener _listener;
		private readonly Thread? _thread;
		private readonly object _lock = new object();
		private BotMessage? _current;
		// BotMessage exposes its data as public fields (no properties), which matches the
		// HB 3.3.5a Remoting/MarshalByRefObject contract. System.Text.Json only serializes
		// properties by default — we MUST opt in to fields or the wire payload is "{}"
		// and the member sees an empty BotMessage (LeaderName='', LeaderGuid=0, ...).
		private static readonly JsonSerializerOptions _jsonOptions = new() { IncludeFields = true };
	}
}
