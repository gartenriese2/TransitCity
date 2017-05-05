// DataPoint.cs by Charles Petzold, December 2008

using System;
using System.ComponentModel;
using System.Windows.Media;

namespace WpfTestApp
{
    public abstract class DataPoint : INotifyPropertyChanged
    {
        private int _type;
        private double _variableX, _variableY;
        private string _id;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Type
        {
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
            get => _type;
        }

        public double VariableX
        {
            set
            {
                if (Math.Abs(_variableX - value) > double.Epsilon)
                {
                    _variableX = value;
                    OnPropertyChanged("VariableX");
                }
            }
            get => _variableX;
        }

        public double VariableY
        {
            set
            {
                if (Math.Abs(_variableY - value) > double.Epsilon)
                {
                    _variableY = value;
                    OnPropertyChanged("VariableY");
                }
            }
            get => _variableY;
        }

        public string Id
        {
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
            get => _id;
        }

        public abstract Drawing GetDrawing();

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
