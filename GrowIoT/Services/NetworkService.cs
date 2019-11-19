using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GrowIoT.Models.Enums;
using GrowIoT.Models.Shared;
using Newtonsoft.Json;

namespace GrowIoT.Services
{
    public class NetworkService
    {
        public delegate void NetworkPack(GrowPackage package);
        public event NetworkPack PackReceive;

        private readonly int _port;

        public NetworkService(int port)
        {
            _port = port;

        }

        //public void StartNetwork()
        //{
        //    StartTcp(GetIpAddress(), _port);
        //}

        //public IPAddress GetIpAddress(IHttpContextAccessor accessor)
        //{
        //    var ipAddress = new List<string>();
        //    var hosts = Windows.Networking.Connectivity.NetworkInformation.GetHostNames().ToList();
        //    foreach (var host in hosts)
        //    {
        //        var ip = host.DisplayName;
        //        ipAddress.Add(ip);
        //    }
        //    var address = IPAddress.Parse(ipAddress.Last());
        //    return address;
        //}

        private void StartTcp(IPAddress ipAddress, int port)
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
                                        response = new GrowPackage<bool>(package.Command,true);
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
