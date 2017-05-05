// ScatterPlotRender.cs by Charles Petzold, December 2008

using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfTestApp
{
    public class ScatterPlotRender : FrameworkElement
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource",
                typeof(ObservableNotifiableCollection<DataPoint>),
                typeof(ScatterPlotRender),
                new PropertyMetadata(OnItemsSourceChanged));

        public static readonly DependencyProperty BrushesProperty =
            DependencyProperty.Register("Brushes",
                typeof(Brush[]),
                typeof(ScatterPlotRender),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BackgroundProperty =
            Panel.BackgroundProperty.AddOwner(typeof(ScatterPlotRender));

        public ObservableNotifiableCollection<DataPoint> ItemsSource
        {
            set => SetValue(ItemsSourceProperty, value);
            get => (ObservableNotifiableCollection<DataPoint>)GetValue(ItemsSourceProperty);
        }

        public Brush[] Brushes
        {
            set => SetValue(BrushesProperty, value);
            get => (Brush[])GetValue(BrushesProperty);
        }

        public Brush Background
        {
            set => SetValue(BackgroundProperty, value);
            get => (Brush)GetValue(BackgroundProperty);
        }

        static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as ScatterPlotRender).OnItemsSourceChanged(args);
        }

        void OnItemsSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != null)
            {
                var coll = args.OldValue as ObservableNotifiableCollection<DataPoint>;
                coll.CollectionChanged -= OnCollectionChanged;
                coll.ItemPropertyChanged -= OnItemPropertyChanged;
            }

            if (args.NewValue != null)
            {
                var coll = args.NewValue as ObservableNotifiableCollection<DataPoint>;
                coll.CollectionChanged += OnCollectionChanged;
                coll.ItemPropertyChanged += OnItemPropertyChanged;
            }

            InvalidateVisual();
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            InvalidateVisual();
        }

        void OnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs args)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Background, null, new Rect(RenderSize));

            if (ItemsSource == null || Brushes == null)
                return;

            foreach (var dataPoint in ItemsSource)
            {
                dc.DrawEllipse(Brushes[dataPoint.Type], null,
                    new Point(RenderSize.Width * dataPoint.VariableX,
                        RenderSize.Height * dataPoint.VariableY), 1, 1);
            }
        }
    }
}
