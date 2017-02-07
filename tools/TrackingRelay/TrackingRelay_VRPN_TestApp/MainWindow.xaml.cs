using TrackingRelay_Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;
using TrackingRelay_VRPN;

namespace TrackingRelay_VRPN_TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VrpnManager _manager = new VrpnManager(true);

        public MainWindow()
        {
            InitializeComponent();

            _manager.ConnectionChanged += (o, e) => UpdateGUI();

            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += (o, e) =>
            {
                _manager.UpdateMessages();

            };
            timer.Start();
            _console.ItemsSource = _manager.Messages;

            _broadcastPos_TextChanged(null, null);
            _broadcastPosNoise_TextChanged(null, null);
        }


        private void StartStopServer_Click(object sender, RoutedEventArgs e)
        {
            StartStopServer();
        }

        public void StartStopServer()
        {
            if (_manager.ServerConnected)
            {
                _manager.KillServer();
            }
            else
            {
                if (_serverPort.Text.Trim().ToLower() == "any")
                    _serverPort.Text = TcpUtils.FreeTcpPort().ToString();

                int pars;
                int? port = Int32.TryParse(_serverPort.Text, out pars) ? pars : (int?)null;

                _manager.RunServer(_serverNameTextBox.Text, port);
            }

        }


        private void StartStopClient_Click(object sender, RoutedEventArgs e)
        {
            StartStopClient();
        }
        public void StartStopClient()
        {

            if (_manager.ClientConnected)
            {
                _manager.KillClient();
            }
            else
            {
                _manager.RunClient(_serverNameTextBox.Text, _serverAdressTextBox.Text, 0);
            }

        }

        private void UpdateGUI()
        {
            
            if (_manager.ClientConnected)
            {
                _startStopClientButton.Content = "Stop client";
            }
            else
            {
                _startStopClientButton.Content = "Start client";
            }

            if (_manager.ServerConnected)
            {
                _startStopServerButton.Content = "Stop server";
            }
            else
            {
                _startStopServerButton.Content = "Start server";
            }

            _serverAdressTextBox.IsEnabled = !_manager.ClientConnected;
            _serverNameTextBox.IsEnabled = !_manager.ClientConnected && !_manager.ServerConnected;



        }

        private void _broadcastPos_TextChanged(object sender, TextChangedEventArgs e)
        {
            double pars;
            var items = _broadcastPos.Text.Split(';').Select(o => double.TryParse(o, out pars) ? pars : double.NaN).Where(o=>!double.IsNaN(o)).ToArray(); 

            if (items.Length < 3)
                return;

            _manager.BroadcastPosition = new Vector3(items[0], items[1], items[2]);

        }

        private void _broadcastPosNoise_TextChanged(object sender, TextChangedEventArgs e)
        {

            double pars;
            var items = _broadcastPosNoise.Text.Split(';').Select(o => double.TryParse(o, out pars) ? pars : double.NaN).Where(o => !double.IsNaN(o)).ToArray();

            if (items.Length < 3)
                return;

            _manager.BroadcastPosition_Noise = new Vector3(items[0], items[1], items[2]);
        }

        private void _startAnotherserver_Click(object sender, RoutedEventArgs e)
        {
            var newW = new MainWindow();
            newW.Show();
        }

    }

}
