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
        public MainWindow()
        {
            InitializeComponent();
            mserver = new AsyncSocketServer();
        }

        private void btnAvvia_Click(object sender, RoutedEventArgs e)
        {
            mserver.InizioAscolto();

            Thread invioInf = new Thread(() => InviaInf());
            invioInf.Start();
        }

        public void InviaInf()
        {
            while(true)
            {
                mserver.SendToAll(DateTime.Now.ToString()+"\n");
                Thread.Sleep(10000);
            }
        }
    }
}
