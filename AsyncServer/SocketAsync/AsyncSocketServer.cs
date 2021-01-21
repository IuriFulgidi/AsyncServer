using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fulgidi_SocketAsync
{
    public class AsyncSocketServer
    {
        IPAddress mIP;
        int mPort;
        TcpListener mServer;
        bool continua;

        List<TcpClient> mClients;

        public AsyncSocketServer()
        {
            mClients = new List<TcpClient>();
        }

        // Mette in ascolto il server
        public async void InizioAscolto(IPAddress ipaddr = null, int port = 23000)
        {
            //faccio dei controlli su IPAddress e sulle porte
            if (ipaddr == null)
            {
                //mIP = IPAddress.Any;
                ipaddr = IPAddress.Any;
            }
            if (port < 0 || port > 65535)
            {
                //mPort = 23000;
                port = 23000;
            }

            mIP = ipaddr;//aggiunte
            mPort = port;//aggiunte

            Debug.WriteLine($"Avvio il server. IP: {mIP.ToString()} - Porta: {mPort.ToString()}");
            //creare l'oggetto server
            mServer = new TcpListener(mIP, mPort);

            //avviare il server
            mServer.Start();
            continua = true;
            while (continua)
            {
                //mi metto in ascolto
                TcpClient client = await mServer.AcceptTcpClientAsync();
                mClients.Add(client);
                Debug.WriteLine($"Client connessi: {mClients.Count()}, Client appena connesso:{ client.Client.RemoteEndPoint}");

                RiceviMessaggi(client);
            }
        }
        public async void RiceviMessaggi(TcpClient client)
        {
            NetworkStream stream = null;
            StreamReader reader = null;
            try
            {
                stream = client.GetStream();
                reader = new StreamReader(stream);
                char[] buff = new char[512];

                //ricezione effettiva
                while (continua)
                {
                    Debug.WriteLine("Pronto ad ascoltare...");
                    int nBytes = await reader.ReadAsync(buff, 0, buff.Length);
                    if (nBytes == 0)
                    {
                        RemoveClient(client);
                        Debug.WriteLine("Client disconnesso.");
                        break;
                    }
                    string recvMessage = new string(buff);
                    Debug.WriteLine($"Returned bytes: {nBytes}. Messaggio: {recvMessage}");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void RemoveClient(TcpClient client)
        {
            if (mClients.Contains(client))
            {
                mClients.Remove(client);
            }
        }

        public void SendToAll(string messaggio)
        {
            try
            {
                if (string.IsNullOrEmpty(messaggio))
                    return;

                byte[] buff = Encoding.ASCII.GetBytes(messaggio);

                foreach (TcpClient client in mClients)
                {
                    client.GetStream().WriteAsync(buff, 0, buff.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore:" + ex.Message);
            }
        }

        public void CloseConnection()
        {
            try
            {
                foreach (TcpClient client in mClients)
                {
                    client.Close();
                    RemoveClient(client);
                }

                mServer.Stop();
                mServer = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Errore:" + ex.Message);
            }
        }
    }
}
