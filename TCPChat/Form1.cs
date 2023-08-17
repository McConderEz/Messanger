using static System.Net.Mime.MediaTypeNames;
using System.Text;

namespace TCPChat
{
    public partial class Form1 : Form
    {
        Server server;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            server = new Server("127.0.0.1", 8083);
            server.Start();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            server.flag = true;
        }

        #region Пустота
        

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        #endregion

        public void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}