using Fulgidi_SocketAsync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AsyncServer
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AsyncSocketServer mserver;
        bool serverOn = false;
        public MainWindow()
        {
            InitializeComponent();
            mserver = new AsyncSocketServer();
        }

        private void btnAvvia_Click(object sender, RoutedEventArgs e)
        {
            if (!serverOn)
            {
                serverOn = true;
                mserver.InizioAscolto();

                //avvio thread
                Thread sendDateTime = new Thread(() => SendDateTime());
                sendDateTime.Start();
            }
            else
                MessageBox.Show("Il server è stato già avviato","Attenzione!",MessageBoxButton.OK,MessageBoxImage.Warning);
        }

        /// <summary>
        /// inva un messaggio con data e tempo ogni dieci secondi
        /// </summary>
        public void SendDateTime()
        {
            while(true)
            {
                mserver.SendToAll(DateTime.Now.ToString()+"\n");
                Thread.Sleep(10000);
            }
        }

        private void btnDisconnetti_Click(object sender, RoutedEventArgs e)
        {
            var risp = MessageBox.Show("Il server verrà disconnesso dal primo client connesso client,procedere?", "Attenzione!", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (risp == MessageBoxResult.Yes)
            {
                mserver.CloseConnection();
                serverOn = false;
            }   
            else
                MessageBox.Show("Operazione annullata", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnBroadcast_Click(object sender, RoutedEventArgs e)
        {
            string msg = txtBroadcast.Text;
            if (!string.IsNullOrEmpty(msg))
                mserver.SendToAll(msg);
            else
                MessageBox.Show("Scrivere il messaggio da inviare", "Attenzione", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
