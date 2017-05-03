﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Shapes;
using WpfDrawing.Annotations;

namespace WpfDrawing.Panel
{
    /// <summary>
    /// Interaction logic for PanelControl.xaml
    /// </summary>
    public partial class PanelControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register(
            "Elements",
            typeof(ObservableCollection<Shape>),
            typeof(PanelControl),
            new PropertyMetadata(new ObservableCollection<Shape>()));

        public PanelControl()
        {
            InitializeComponent();

            Elements.CollectionChanged += OnElementsCollectionChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Shape> Elements
        {
            get => (ObservableCollection<Shape>)GetValue(ElementsProperty);
            set => SetValue(ElementsProperty, value);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnElementsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("Collection changed");
            OnPropertyChanged(nameof(Elements));
        }
    }
}
