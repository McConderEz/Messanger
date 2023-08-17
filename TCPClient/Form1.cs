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
        TcpClient client;
        List<TcpClient> clients;
        NetworkStream stream;
        public Form1()
        {
            InitializeComponent();

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void SendMessage_Click(object sender, EventArgs e)
        {
            string message = $"[{DateTime.Now}]{textBox3.Text}:"+textBox4.Text;
            stream.Write(Encoding.UTF8.GetBytes(message));
            textBox4.Text = "";
        }

        public async Task GetListOfUsersAsync()
        {
            await Task.Run(() => GetListOfUsers());
        }

        private void GetListOfUsers()
        {            
            while (client.Connected)
            {
                if (treeUsers.Nodes[treeUsers.Nodes.Count - 1].Text != ClientObject.clients.LastOrDefault().ToString()) 
                {
                    treeUsers.Nodes.Add(ClientObject.clients.LastOrDefault().ToString());
                }
            }
        }

        public async Task GetMessageAsync()
        {
            await Task.Run(() => GetMessage());
        }

        public void GetMessage()
        {
            while (client.Connected)
            {
                byte[] buffer = new byte[4096];
                int size = 0;
                StringBuilder data = new StringBuilder();
                do
                {
                    size = stream.Read(buffer, 0, 4096);
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));

                } while (client.Available > 0);
                if (data != null)
                    MessageWindow.AppendText(data + "\r\n");
                data = null;
            }
        }

        private void MessageWindow_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)//Подключение
        {
            try
            {
                client = new TcpClient();
                client.Connect(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));
                stream = client.GetStream();
                button1.Enabled = false;
                button2.Enabled = true;
                GetMessageAsync();
                
                //TODO: Сделать список пользователей
                //TODO:Вынести в отдельный класс и сделать атрибут UserName для отображении его в списке подключенных пользователей
                //TODO: Передача файлов

            }
            catch(Exception ex) 
            {

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            { 
                stream.Close();
                client.Close();
                button1.Enabled = true;
                button2.Enabled = false;
            }
        }

        private void SendFile_Click(object sender, EventArgs e)
        {
            if(client.Connected)
            {
                OpenFileDialogSend.ShowDialog();
                Task.Run(() => SendFileAsync(OpenFileDialogSend.FileName));
            }
        }

        private async Task SendFileAsync(string fileName)
        {
            var stream = client.GetStream();
            using var file = File.OpenRead(fileName);
            var length = file.Length;
            byte[] lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));
            await stream.WriteAsync(lengthBytes);
            await file.CopyToAsync(stream);
        }
    }
}