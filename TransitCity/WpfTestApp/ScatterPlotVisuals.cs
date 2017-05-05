// ScatterPlotVisuals.cs by Charles Petzold, December 2008

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfTestApp
{
    public class ScatterPlotVisuals : FrameworkElement
    {
        private readonly VisualCollection _visualChildren;

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource",
                typeof(ObservableNotifiableCollection<DataPoint>),
                typeof(ScatterPlotVisuals),
                new PropertyMetadata(OnItemsSourceChanged));

        public static readonly DependencyProperty BackgroundProperty =
            Panel.BackgroundProperty.AddOwner(typeof(ScatterPlotRender));

        public ScatterPlotVisuals()
        {
            _visualChildren = new VisualCollection(this);
            ToolTip = "";
        }

        public ObservableNotifiableCollection<DataPoint> ItemsSource
        {
            set => SetValue(ItemsSourceProperty, value);
            get => (ObservableNotifiableCollection<DataPoint>)GetValue(ItemsSourceProperty);
        }

        public Brush Background
        {
            set => SetValue(BackgroundProperty, value);
            get => (Brush)GetValue(BackgroundProperty);
        }

        static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as ScatterPlotVisuals).OnItemsSourceChanged(args);
        }

        void OnItemsSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            _visualChildren.Clear();

            if (args.OldValue != null)
            {
                var coll = args.OldValue as ObservableNotifiableCollection<DataPoint>;
                coll.CollectionCleared -= OnCollectionCleared;
                coll.CollectionChanged -= OnCollectionChanged;
                coll.ItemPropertyChanged -= OnItemPropertyChanged;
            }

            if (args.NewValue != null)
            {
                var coll = args.NewValue as ObservableNotifiableCollection<DataPoint>;
                coll.CollectionCleared += OnCollectionCleared;
                coll.CollectionChanged += OnCollectionChanged;
                coll.ItemPropertyChanged += OnItemPropertyChanged;

                CreateVisualChildren(coll);
            }
        }

        void OnCollectionCleared(object sender, EventArgs args)
        {
            RemoveVisualChildren(_visualChildren);
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null)
                RemoveVisualChildren(args.OldItems);

            if (args.NewItems != null)
                CreateVisualChildren(args.NewItems);
        }

        void OnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs args)
        {
            var dataPoint = args.Item as DataPoint;

            foreach (var child in _visualChildren)
            {
                var drawingVisual = child as DrawingVisualPlus;

                if (dataPoint == drawingVisual.DataPoint)
                {
                    // Assume only VariableX or VariableY are changing
                    var xform = drawingVisual.Transform as TranslateTransform;

                    if (args.PropertyName == "VariableX")
                        xform.X = RenderSize.Width * dataPoint.VariableX;

                    else if (args.PropertyName == "VariableY")
                        xform.Y = RenderSize.Height * dataPoint.VariableY;
                }
            }
        }

        void CreateVisualChildren(IEnumerable coll)
        {
            foreach (var obj in coll)
            {
                var dataPoint = obj as DataPoint;

                var drawingVisual = new DrawingVisualPlus {DataPoint = dataPoint};
                var dc = drawingVisual.RenderOpen();

                var uriSource = new Uri("pack://application:,,,/Resources/Add16.png");
                dc.DrawImage(new BitmapImage(uriSource), new Rect(new Size(16, 16)));

                drawingVisual.Transform = new TranslateTransform(RenderSize.Width * dataPoint.VariableX,
                    RenderSize.Height * dataPoint.VariableY);

                dc.Close();
                _visualChildren.Add(drawingVisual);
            }
        }

        void RemoveVisualChildren(IEnumerable coll)
        {
            foreach (var obj in coll)
            {
                var dataPoint = obj as DataPoint;
                var removeList = new List<DrawingVisualPlus>();

                foreach (var child in _visualChildren)
                {
                    var drawingVisual = child as DrawingVisualPlus;
                    if (drawingVisual.DataPoint == dataPoint)
                    {
                        removeList.Add(drawingVisual);
                        break;
                    }
                }
                foreach (var drawingVisual in removeList)
                    _visualChildren.Remove(drawingVisual);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            foreach (var child in _visualChildren)
            {
                var drawingVisual = child as DrawingVisualPlus;
                var xform = drawingVisual.Transform as TranslateTransform;

                if (sizeInfo.WidthChanged)
                    xform.X = sizeInfo.NewSize.Width * drawingVisual.DataPoint.VariableX;

                if (sizeInfo.HeightChanged)
                    xform.Y = sizeInfo.NewSize.Height * drawingVisual.DataPoint.VariableY;
            }
            base.OnRenderSizeChanged(sizeInfo);
        }

        protected override int VisualChildrenCount => _visualChildren.Count;

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _visualChildren.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _visualChildren[index];
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Background, null, new Rect(RenderSize));
        }

        protected override void OnToolTipOpening(ToolTipEventArgs e)
        {
            var result = VisualTreeHelper.HitTest(this, Mouse.GetPosition(this));

            if (result.VisualHit is DrawingVisualPlus)
            {
                var drawingVisual = (DrawingVisualPlus)result.VisualHit;
                var dataPoint = drawingVisual.DataPoint;
                ToolTip = $"{dataPoint.Id}, X={dataPoint.VariableX}, Y={dataPoint.VariableY}";
            }
            base.OnToolTipOpening(e);
        }

        protected override void OnToolTipClosing(ToolTipEventArgs e)
        {
            ToolTip = "";
            base.OnToolTipClosing(e);
        }

        private class DrawingVisualPlus : DrawingVisual
        {
            public DataPoint DataPoint { get; set; }
        }
    }
}
