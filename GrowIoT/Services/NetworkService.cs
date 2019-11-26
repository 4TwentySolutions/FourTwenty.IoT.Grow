using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Connect.Models;
using Newtonsoft.Json;


namespace GrowIoT.Services
{
   public class IoTNetworkService : INetworkService
    {
        public delegate void NetworkPack(GrowPackage package);
        public event NetworkPack PackReceive;

        private readonly int _port;

        public IoTNetworkService(int port)
        {
            _port = port;

        }

        public void StartNetwork(string ip, int? port = 0)
        {
            if (IPAddress.TryParse(ip, out var address))
                StartTcp(address, port.GetValueOrDefault() == 0 ? _port : port.GetValueOrDefault());
        }

        public void StartTcp(IPAddress ipAddress, int port)
        {
            TcpListener server = null;
            try
            {
                server = new TcpListener(ipAddress, port);
                server.Start();
                Debug.WriteLine($"--- Tcp: The server is running at: {ipAddress}:{_port} ---");
                var bytes = new byte[256];

                while (true)
                {

                    TcpClient client = server.AcceptTcpClient();
                    Debug.WriteLine("\n--- Tcp: Connected! ---");
                    NetworkStream stream = client.GetStream();
                    int i;
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var data = Encoding.ASCII.GetString(bytes, 0, i);
                        Debug.WriteLine("Received: {0}", data);
                        GrowPackage response = null;
                        if (!string.IsNullOrEmpty(data))
                        {
                            var package = JsonConvert.DeserializeObject<GrowPackage>(data);
                            if (package != null)
                            {
                                PackReceive?.Invoke(package);

                                switch (package.Command)
                                {
                                    case IoTCommand.Ping:
                                        response = new GrowPackage<bool>(package.Command, true);
                                        break;
                                    case IoTCommand.None:
                                        response = new GrowPackage<bool>(package.Command, true);
                                        break;
                                }
                            }
                        }

                        if (response == null)
                        {
                            response = new GrowPackage(IoTCommand.None);
                        }
                        data = JsonConvert.SerializeObject(response);

                        var msg = Encoding.ASCII.GetBytes(data);

                        stream.Write(msg, 0, msg.Length);
                        Debug.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                    Debug.WriteLine("--- Tcp: Close! ---\n");
                }
            }
            catch (SocketException e)
            {
                Debug.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server?.Stop();
            }
        }

    }
}
