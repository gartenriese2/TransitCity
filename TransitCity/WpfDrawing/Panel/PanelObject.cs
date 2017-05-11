using System;
using System.ComponentModel;
using System.Windows.Media;

namespace WpfDrawing.Panel
{
    public abstract class PanelObject : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private double _angle;

        public event PropertyChangedEventHandler PropertyChanged;

        public double X
        {
            get => _x;
            set
            {
                if (Math.Abs(_x - value) > double.Epsilon)
                {
                    _x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (Math.Abs(_y - value) > double.Epsilon)
                {
                    _y = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }

        public double Angle
        {
            get => _angle;
            set
            {
                if (!Equals(value, _angle))
                {
                    _angle = value;
                    OnPropertyChanged(nameof(Angle));
                }
            }
        }

        public abstract Drawing GetDrawing();

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
