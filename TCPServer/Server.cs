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
        static TcpListener listener;
        public Server(string Name)
        {
            Ip = "127.0.0.1";
            Port = 8081;
            listener = new TcpListener(IPAddress.Parse(Ip), Port);         
        }

        public void Process()
        {
            listener.Start();
            Console.WriteLine("Ожидание подключений...");
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ClientObject clientObject = new ClientObject(client);
                

                Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                clientThread.Start();
            }
        }        
    }
}
