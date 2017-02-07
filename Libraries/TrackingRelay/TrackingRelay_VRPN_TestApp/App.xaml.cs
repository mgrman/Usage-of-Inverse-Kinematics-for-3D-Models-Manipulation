using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TrackingRelay_Utils;

namespace TrackingRelay_VRPN_TestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Select(o => o.ToLower()).Contains("-planePositionTest".ToLower()))
            {
                MainWindow rightHand = new MainWindow();

                //rightHand._broadcastPosNoise.Text = string.Format("{0};{1};{2}", 0 * scale, 0.1 * scale, 0 * scale);
                rightHand._serverAdressTextBox.Text = "localhost";
                rightHand._serverNameTextBox.Text = "RightHand";
                rightHand._serverPort.Text = "";
                rightHand.Show();


                MainWindow leftHand = new MainWindow();

                //leftHand._broadcastPosNoise.Text = string.Format("{0};{1};{2}", 0.1 * scale, 0 * scale, 0 * scale);
                leftHand._serverAdressTextBox.Text = "localhost";
                leftHand._serverNameTextBox.Text = "LeftHand";
                leftHand._serverPort.Text = "";
                leftHand.Show();

                MainWindow backOfNeck = new MainWindow();

                backOfNeck._serverAdressTextBox.Text = "localhost";
                backOfNeck._serverNameTextBox.Text = "BackOfNeck";
                backOfNeck._serverPort.Text = "";
                backOfNeck.Show();

                MainWindow chest = new MainWindow();

                //chest._broadcastPosNoise.Text = string.Format("{0};{1};{2}", 0 * scale, 0 * scale, 0.1 * scale);
                chest._serverAdressTextBox.Text = "localhost";
                chest._serverNameTextBox.Text = "Chest";
                chest._serverPort.Text = "";
                chest.Show();

                rightHand.StartStopServer();
                leftHand.StartStopServer();
                chest.StartStopServer();
                backOfNeck.StartStopServer();


                Vector3 scale;
                Vector3 offset;
                Vector3 leftHandPos;
                Vector3 rightHandPos;
                Vector3 chestPos;
                Vector3 backOfNeckPos;

                PositionWindow posWindow = new PositionWindow();
                posWindow.Show();

                while (rightHand.IsLoaded || leftHand.IsLoaded || chest.IsLoaded)
                {
                    if (!posWindow.IsLoaded)
                    {
                        leftHand.Close();
                        rightHand.Close();
                        chest.Close();
                        backOfNeck.Close();
                        break;
                    }

                    await Task.Delay(10);

                    scale = posWindow.Scale;
                    offset = posWindow.Offset;
                    leftHandPos = posWindow.LeftHandPos * scale + offset;
                    rightHandPos = posWindow.RightHandPos * scale + offset;
                    chestPos = posWindow.ChestPos * scale + offset;
                    backOfNeckPos = posWindow.BackOfNeckPos * scale + offset;


                    leftHand._broadcastPos.Text = string.Format("{0};{1};{2}", leftHandPos.X, leftHandPos.Y, leftHandPos.Z);
                    rightHand._broadcastPos.Text = string.Format("{0};{1};{2}", rightHandPos.X, rightHandPos.Y, rightHandPos.Z);
                    chest._broadcastPos.Text = string.Format("{0};{1};{2}", chestPos.X, chestPos.Y, chestPos.Z);
                    backOfNeck._broadcastPos.Text = string.Format("{0};{1};{2}", backOfNeckPos.X, backOfNeckPos.Y, backOfNeckPos.Z);
                }


                return;
            }


            MainWindow mw = new MainWindow();
            mw.Show();

        }

    }
}
