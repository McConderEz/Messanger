using System.Net.Sockets;
using System.Net;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System;
using TCPServer;

namespace TCPClient
{
    public partial class Form1 : Form
    {
        private Socket clientSocket;
        private List<Socket> clients;
        private NetworkStream stream;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private async void SendMessage_Click(object sender, EventArgs e)
        {
            string message = $" [{DateTime.Now}]{textBox3.Text}: {textBox4.Text}";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
            textBox4.Text = "";
        }

        public async Task GetListOfUsersAsync()
        {
            await Task.Run(() => GetListOfUsers());
        }

        private void GetListOfUsers()
        {
            while (clientSocket.Connected)
            {
                if (treeUsers.Nodes[treeUsers.Nodes.Count - 1].Text !=
                    clients[clients.Count - 1].RemoteEndPoint.ToString())
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        treeUsers.Nodes.Add(clients[clients.Count - 1].RemoteEndPoint.ToString());
                    }));
                }
            }
        }

        public async Task GetMessageAsync()
        {
            await Task.Run(() => GetMessage());
        }

        public void GetMessage()
        {
            while (clientSocket.Connected)
            {
                byte[] buffer = new byte[4096];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Invoke((MethodInvoker)(() =>
                    {
                        MessageWindow.AppendText(data + "\r\n");
                    }));
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) // Подключение
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));
                stream = new NetworkStream(clientSocket);
                button1.Enabled = false;
                button2.Enabled = true;
                GetMessageAsync();

                // TODO: Сделать список пользователей
                // TODO: Вынести в отдельный класс и сделать атрибут UserName для отображения его в списке подключенных пользователей
                // TODO: Передача файлов
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (clientSocket.Connected)
            {
                stream.Close();
                clientSocket.Close();
                button1.Enabled = true;
                button2.Enabled = false;
            }
        }

        private void SendFile_Click(object sender, EventArgs e)
        {
            if (clientSocket.Connected)
            {
                OpenFileDialogSend.ShowDialog();
                Task.Run(() => SendFileAsync(OpenFileDialogSend.FileName));
            }
        }

        private async Task SendFileAsync(string fileName)
        {
            using (var fileStream = File.OpenRead(fileName))
            {
                long length = fileStream.Length;
                byte[] lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));
                await stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);
                await fileStream.CopyToAsync(stream);
            }
        }
    }
}