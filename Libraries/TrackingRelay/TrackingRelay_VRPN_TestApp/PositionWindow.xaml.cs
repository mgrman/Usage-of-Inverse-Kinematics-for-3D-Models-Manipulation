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
using System.Windows.Shapes;
using TrackingRelay_Utils;

namespace TrackingRelay_VRPN_TestApp
{
    /// <summary>
    /// Interaction logic for PositionWindow.xaml
    /// </summary>
    public partial class PositionWindow : Window
    {

        Ellipse _movingObject;

        Point _mouseDownOffset;

        public PositionWindow()
        {
            InitializeComponent();

            _rightHand.MouseDown += EllipseMouseDown;
            _leftHand.MouseDown += EllipseMouseDown;
            _chest.MouseDown += EllipseMouseDown;
            _backOfNeck.MouseDown += EllipseMouseDown;


            this.MouseMove += (o, e) =>
            {
                if (_movingObject == null)
                    return;

                var pos = e.GetPosition(_parentGrid);

                _movingObject.Margin = new Thickness(pos.X + _mouseDownOffset.X, pos.Y + _mouseDownOffset.Y, 0, 0);

            };

            this.MouseUp += (o, e) => _movingObject = null;
        }

        private void EllipseMouseDown(object sender, MouseButtonEventArgs e)
        {
            _movingObject = sender as Ellipse;
            var pos = e.GetPosition(_parentGrid);

            _mouseDownOffset = new Point(_movingObject.Margin.Left - pos.X, _movingObject.Margin.Top - pos.Y);
        }


        public Vector3 RightHandPos { get { return new Vector3(_rightHand.Margin.Left, 0, _rightHand.Margin.Top); } }
        public Vector3 LeftHandPos { get { return new Vector3(_leftHand.Margin.Left, 0, _leftHand.Margin.Top); } }
        public Vector3 ChestPos { get { return new Vector3(_chest.Margin.Left, 0, _chest.Margin.Top); } }
        public Vector3 BackOfNeckPos { get { return new Vector3(_backOfNeck.Margin.Left, 0, _backOfNeck.Margin.Top); } }

        public Vector3 Scale
        {
            get
            {
                Vector3 pars;
                return Vector3.TryParse(_scale.Text, out pars) ? pars : new Vector3();
            }
        }
        public Vector3 Offset
        {
            get
            {
                Vector3 vec;
                return Vector3.TryParse(_offset.Text, out vec)?vec:new Vector3();
            }
        }
    }
}
