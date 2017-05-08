using System;
using System.ComponentModel;
using System.Windows.Media;

namespace WpfDrawing.Panel
{
    public abstract class PanelObject : INotifyPropertyChanged
    {
        private double _variableX;
        private double _variableY;

        public event PropertyChangedEventHandler PropertyChanged;

        public double VariableX
        {
            get => _variableX;
            set
            {
                if (Math.Abs(_variableX - value) > double.Epsilon)
                {
                    _variableX = value;
                    OnPropertyChanged("VariableX");
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
                    OnPropertyChanged("VariableY");
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
