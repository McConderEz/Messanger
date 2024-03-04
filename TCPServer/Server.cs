using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{

    internal class Server
    {
        
        public string Ip { get; private set; }
        public int Port { get; private set; }
        static Socket socket;
        public Server(string Name)
        {
            Ip = "127.0.0.1";
            Port = 8081;
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, Port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp); 
            socket.Bind(ipPoint);
            socket.Listen(10);
        }

        public void Process()
        {
            Console.WriteLine("Ожидание подключений...");

            while (true)
            {
                Socket client = socket.Accept();
                ClientObject clientObject = new ClientObject(client);


                Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                clientThread.Start();
            }
        }        
    }
}
