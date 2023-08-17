using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    public class ClientObject
    {
        public TcpClient client;
        public static List<TcpClient> clients = new List<TcpClient>();
        private static List<NetworkStream> streams = new List<NetworkStream>();
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
            clients.Add(client);
        }

        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                streams.Add(stream);
                GetFile(stream);
                byte[] buffer = new byte[4096];
                while (true)
                {
                    

                    int size = 0;
                    StringBuilder data = new StringBuilder();

                    do
                    {
                        size = stream.Read(buffer,0,4096);
                        data.Append(Encoding.UTF8.GetString(buffer,0,size));

                    } while (client.Available > 0);
                                          
                    Console.WriteLine(data);

                    foreach (var _stream in streams)
                    {
                        _stream.Write(Encoding.UTF8.GetBytes(data.ToString()));       
                    }

                }
               
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }

        public async Task GetFile(NetworkStream stream)//TODO:Разобрать работы отправки и приёма файла, доработать
        {
            byte[] buf = new byte[65536];
            await ReadBytes(sizeof(long),stream,buf);
            long remainingLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buf, 0));
            using var file = File.Create("test.txt");
            while (remainingLength > 0)
            {
                int lengthToRead = (int)Math.Min(remainingLength, buf.Length);
                await ReadBytes(lengthToRead,stream,buf);
                await file.WriteAsync(buf, 0, lengthToRead);
                remainingLength -= lengthToRead;
            }
        }

        private async Task ReadBytes(int howmuch,NetworkStream stream, byte[] buf)
        {
            int readPos = 0;
            while (readPos < howmuch)
            {
                var actuallyRead = await stream.ReadAsync(buf, readPos, howmuch - readPos);
                if (actuallyRead == 0)
                    throw new EndOfStreamException();
                readPos += actuallyRead;
            }
        }
    }
}
