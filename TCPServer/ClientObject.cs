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
        private Socket clientSocket;
        private static List<Socket> clients = new List<Socket>();
        private static List<NetworkStream> streams = new List<NetworkStream>();

        public ClientObject(Socket socket)
        {
            clientSocket = socket;
            clients.Add(clientSocket);
        }

        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = new NetworkStream(clientSocket);
                streams.Add(stream);
                GetFile(stream);
                byte[] buffer = new byte[4096];
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine(data);

                    foreach (var clientStream in streams)
                    {
                        clientStream.Write(Encoding.UTF8.GetBytes(data), 0, bytesRead);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                stream?.Close();
                clientSocket?.Close();
                streams.Remove(stream);
                clients.Remove(clientSocket);
            }
        }

        public async Task GetFile(NetworkStream stream)
        {
            byte[] buf = new byte[sizeof(long)];
            await ReadBytes(buf.Length, stream, buf);
            long remainingLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buf, 0));

            using var file = File.Create("test.txt");
            while (remainingLength > 0)
            {
                int lengthToRead = (int)Math.Min(remainingLength, buf.Length);
                await ReadBytes(lengthToRead, stream, buf);
                await file.WriteAsync(buf, 0, lengthToRead);
                remainingLength -= lengthToRead;
            }
        }

        private async Task ReadBytes(int howmuch, NetworkStream stream, byte[] buf)
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
