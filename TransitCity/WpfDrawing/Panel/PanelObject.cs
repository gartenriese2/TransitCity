using System;
using System.ComponentModel;
using System.Windows.Media;

namespace WpfDrawing.Panel
{
    public abstract class PanelObject : INotifyPropertyChanged
    {
        private double _variableX;
        private double _variableY;
        private double _angle;

        public event PropertyChangedEventHandler PropertyChanged;

        public double VariableX
        {
            get => _variableX;
            set
            {
                if (Math.Abs(_variableX - value) > double.Epsilon)
                {
                    _variableX = value;
                    OnPropertyChanged(nameof(VariableX));
                }
            }
        }

        public double VariableY
        {
            get => _variableY;
            set
            {
                if (Math.Abs(_variableY - value) > double.Epsilon)
                {
                    _variableY = value;
                    OnPropertyChanged(nameof(VariableY));
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
