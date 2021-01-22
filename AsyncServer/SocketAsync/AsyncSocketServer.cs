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
        public async void InizioAscolto()
        {
            mIP = IPAddress.Any;
            mPort = 23000;

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
                SendToOne(client, "­scrivere 'time' per ricevere l'ora e 'date' per la data\n"); 
                RiceviMessaggi(client);
            }
        }
        private async void RiceviMessaggi(TcpClient client)
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
                    string recvMessage = new string(buff,0,nBytes).ToLower();
                    Debug.WriteLine($"Returned bytes: {nBytes}. Messaggio: {recvMessage}");

                    Rispondi(client, recvMessage);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Rispondi(TcpClient client, string msg)
        {
            string risposta;

            if (msg == "time")
                risposta = DateTime.Now.ToShortTimeString();
            else if (msg == "date")
                risposta = DateTime.Today.ToShortDateString();
            else
                risposta = "non ho capito";

            risposta += "\n";
            SendToOne(client, risposta);
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
        public void SendToOne(TcpClient client, string messaggio)
        {
            try
            {
                if (string.IsNullOrEmpty(messaggio))
                    return;

                byte[] buff = Encoding.ASCII.GetBytes(messaggio);
                client.GetStream().WriteAsync(buff, 0, buff.Length);
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
