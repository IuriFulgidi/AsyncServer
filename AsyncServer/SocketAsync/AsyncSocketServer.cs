using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketAsync
{
    public class AsyncSocketServer
    {
        IPAddress mIP;
        TcpClient client;
        TcpListener mserver;


        bool continua;
        public async void InizioAscolto()
        {
            mIP = ipAddr;
            mPort = port;

            Debug.WriteLine($"avvio il server. IP:{mIP} - Porta: {nPort}");

            //creare l'oggetto server
            mserver = new TcpListener(mIP, mPort);

            //avviare il server
            mserver.Start();
            while(true)
            {
                //mi metto in ascolto
                TcpClient client = await mserver.AcceptTcpClientAsync();
                Debug.WriteLine("Client connesso:" + client.Client.RemoteEndPoint);

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
                        Debug.WriteLine("client disconnesso.");
                        break;
                    }
                    string recvMessage = new string(buff);
                    Debug.WriteLine($"Returned bytes: {nBytes}, Messaggio {recvMessage}");
                }
            }
            catch
            {

            }
        }
    }
}
