using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPChat
{
    public class Server:Form1
    {
        public string Ip { get; private set; }
        public int Port { get; private set; }

        public static IPEndPoint tcpEndPoint;
        public static Socket socket;
        public static Socket? listener;
        public bool flag = false;
        public Server(string Ip, int Port)
        {
            this.Ip = Ip;
            this.Port = Port;

            tcpEndPoint = new IPEndPoint(IPAddress.Parse(this.Ip), this.Port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            try
            {
                socket.Bind(tcpEndPoint);
                socket.Listen(5);
                MessageWindow.AppendText("Сервер запущен...\n");

                while (true)
                {
                    listener = socket.Accept();                    
                    RecieveMessageAsync(listener);   
                    if(flag == true)
                        SendMessageAsync(listener);

                    listener.Shutdown(SocketShutdown.Both);
                    listener.Close();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public async Task<bool> SendMessageAsync(Socket listener)
        {
            await Task.Run(() => SendMessage(listener));
            return true;
        }

        public async Task<bool> RecieveMessageAsync(Socket listener)
        {
            await Task.Run(() => RecieveMessage(listener));
            return true;
        }

        public void SendMessage(Socket listener)
        {
            string text;

            text = MessagePanel.Text+"\n";
            listener.Send(Encoding.UTF8.GetBytes(text));
            MessageWindow.AppendText("Здаров"+"\n");
            flag = false;
        }

        public void RecieveMessage(Socket listener)
        {
            StringBuilder data = new StringBuilder();
            int size = 0;
            byte[] buffer = new byte[1024];

            do
            {
                size = listener.Receive(buffer);
                data.Append(Encoding.UTF8.GetString(buffer, 0, size));

            } while (listener.Available > 0);
            MessageWindow.AppendText($"[{DateTime.Now}]" + data + "\n");
        }
    }
}
