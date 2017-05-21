using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using WpfDrawing.Utility;

namespace WpfDrawing.Panel
{
    public class PanelVisuals : FrameworkElement
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(ObservableNotifiableCollection<PanelObject>),
            typeof(PanelVisuals),
            new PropertyMetadata(OnItemsSourceChanged));

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
            "Zoom",
            typeof(double),
            typeof(PanelVisuals),
            new PropertyMetadata(1.0, OnZoomChanged),
            value => (double)value > 0.0);

        public static readonly DependencyProperty MinZoomProperty = DependencyProperty.Register(
            "MinZoom",
            typeof(double),
            typeof(PanelVisuals),
            new PropertyMetadata(0.1),
            value => (double)value > 0.0);

        public static readonly DependencyProperty MaxZoomProperty = DependencyProperty.Register(
            "MaxZoom",
            typeof(double),
            typeof(PanelVisuals),
            new PropertyMetadata(100.0),
            value => (double)value > 0.0);

        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
            "Center",
            typeof(Point),
            typeof(PanelVisuals),
            new PropertyMetadata(new Point(0.5, 0.5)),
            value => ((Point)value).X >= 0.0 && ((Point)value).X <= 1.0 && ((Point)value).Y >= 0.0 && ((Point)value).Y <= 1.0);

        public static readonly DependencyProperty WorldSizeProperty = DependencyProperty.Register(
            "WorldSize",
            typeof(Size),
            typeof(PanelVisuals),
            new PropertyMetadata(new Size(0, 0), (o, args) => ((PanelVisuals) o).Refresh()),
            value => ((Size)value).Width >= 0 && ((Size)value).Height >= 0);

        private readonly VisualCollection _visualChildren;
        private Size _viewSize;

        public PanelVisuals()
        {
            _visualChildren = new VisualCollection(this);
            _viewSize = new Size(RenderSize.Width * Zoom, RenderSize.Height * Zoom);
            SizeChanged += (sender, args) => Refresh();
        }

        public ObservableNotifiableCollection<PanelObject> ItemsSource
        {
            set => SetValue(ItemsSourceProperty, value);
            get => (ObservableNotifiableCollection<PanelObject>)GetValue(ItemsSourceProperty);
        }

        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        public double MinZoom
        {
            get => (double)GetValue(MinZoomProperty);
            set => SetValue(MinZoomProperty, value);
        }

        public double MaxZoom
        {
            get => (double)GetValue(MaxZoomProperty);
            set => SetValue(MaxZoomProperty, value);
        }

        public Point Center
        {
            get => (Point) GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        public Size WorldSize
        {
            get => (Size) GetValue(WorldSizeProperty);
            set => SetValue(WorldSizeProperty, value);
        }

        protected override int VisualChildrenCount => _visualChildren.Count;

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            foreach (var child in _visualChildren)
            {
                var drawingVisual = child as PanelDrawingVisual;
                var xform = drawingVisual?.Transform as TranslateTransform;
                if (xform == null)
                {
                    continue;
                }

                if (sizeInfo.WidthChanged)
                {
                    xform.X = sizeInfo.NewSize.Width * drawingVisual.PanelObject.X;
                }

                if (sizeInfo.HeightChanged)
                {
                    xform.Y = sizeInfo.NewSize.Height * drawingVisual.PanelObject.Y;
                }
            }

            base.OnRenderSizeChanged(sizeInfo);
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _visualChildren.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _visualChildren[index];
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect(RenderSize));
        }

        private static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as PanelVisuals)?.OnItemsSourceChanged(args);
        }

        private static void OnZoomChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as PanelVisuals)?.Refresh();
        }

        private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            _visualChildren.Clear();

            if (args.OldValue != null)
            {
                var coll = args.OldValue as ObservableNotifiableCollection<PanelObject>;
                if (coll != null)
                {
                    coll.CollectionCleared -= OnCollectionCleared;
                    coll.CollectionChanged -= OnCollectionChanged;
                    coll.ItemPropertyChanged -= OnItemPropertyChanged;
                }
            }

            if (args.NewValue != null)
            {
                var coll = args.NewValue as ObservableNotifiableCollection<PanelObject>;
                if (coll != null)
                {
                    coll.CollectionCleared += OnCollectionCleared;
                    coll.CollectionChanged += OnCollectionChanged;
                    coll.ItemPropertyChanged += OnItemPropertyChanged;

                    CreateVisualChildren(coll);
                }
            }
        }

        private void OnCollectionCleared(object sender, EventArgs args)
        {
            RemoveVisualChildren(_visualChildren);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null)
            {
                RemoveVisualChildren(args.OldItems);
            }

            if (args.NewItems != null)
            {
                CreateVisualChildren(args.NewItems);
            }
        }

        private void OnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs args)
        {
            var panelObject = args.Item as PanelObject;
            if (panelObject == null)
            {
                return;
            }

            foreach (var child in _visualChildren)
            {
                var drawingVisual = child as PanelDrawingVisual;
                if (drawingVisual == null || drawingVisual.PanelObject != panelObject)
                {
                    continue;
                }

                var transform = drawingVisual.Transform as TranslateTransform;
                if (transform == null)
                {
                    continue;
                }

                if (args.PropertyName == nameof(panelObject.X))
                {
                    transform.X = RenderSize.Width * panelObject.X;
                }
                else if (args.PropertyName == nameof(panelObject.Y))
                {
                    transform.Y = RenderSize.Height * panelObject.Y;
                }
            }
        }

        private void Refresh()
        {
            if (RenderSize.IsEmpty || Math.Abs(RenderSize.Width) < double.Epsilon || Math.Abs(RenderSize.Height) < double.Epsilon)
            {
                return;
            }

            _viewSize = new Size(RenderSize.Width * Zoom, RenderSize.Height * Zoom);
            _visualChildren.Clear();
            CreateVisualChildren(ItemsSource);
        }

        private void CreateVisualChildren(IEnumerable coll)
        {
            if (coll == null || WorldSize.Width <= 0 || WorldSize.Height <= 0 || RenderSize.Width <= 0 || RenderSize.Height <= 0)
            {
                return;
            }

            foreach (var obj in coll)
            {
                var panelObject = obj as PanelObject;
                if (panelObject == null)
                {
                    continue;
                }

                var drawingVisual = new PanelDrawingVisual { PanelObject = panelObject };
                var dc = drawingVisual.RenderOpen();
                dc.DrawDrawing(panelObject.GetDrawing());
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(new ScaleTransform(panelObject.Scale, panelObject.Scale));
                transformGroup.Children.Add(new TranslateTransform(panelObject.X, panelObject.Y));
                transformGroup.Children.Add(new RotateTransform(panelObject.Angle, panelObject.X, panelObject.Y));
                drawingVisual.Transform = transformGroup;
                dc.Close();
                _visualChildren.Add(drawingVisual);
            }

            var worldWidth = WorldSize.Width;
            var worldHeight = WorldSize.Height;
            var renderWidth = RenderSize.Width;
            var renderHeight = RenderSize.Height;
            var worldRatio = worldWidth / worldHeight;
            var renderRatio = renderWidth / renderHeight;
            var fitWidth = worldRatio >= renderRatio;
            var worldToRenderConversion = fitWidth ? renderWidth / worldWidth : renderHeight / worldHeight;
            var translateX = fitWidth ? 0 : (renderWidth - worldWidth * worldToRenderConversion) * 0.5;
            var translateY = fitWidth ? (renderHeight - worldHeight * worldToRenderConversion) * 0.5 : 0;
            var group = new TransformGroup();
            var scale = new ScaleTransform(Zoom * worldToRenderConversion, Zoom * worldToRenderConversion);
            var zoomOffset = (Zoom - 1) * (fitWidth ? renderWidth : renderHeight) * 0.5;
            var translate = new TranslateTransform(translateX - zoomOffset, translateY - zoomOffset);
            group.Children.Add(scale);
            group.Children.Add(translate);
            RenderTransform = group;
        }

        private void RemoveVisualChildren(IEnumerable coll)
        {
            foreach (var obj in coll)
            {
                var panelObject = obj as PanelObject;
                if (panelObject == null)
                {
                    continue;
                }

                var removeList = new List<PanelDrawingVisual>();
                foreach (var child in _visualChildren)
                {
                    var drawingVisual = child as PanelDrawingVisual;
                    if (drawingVisual == null)
                    {
                        continue;
                    }

                    if (drawingVisual.PanelObject == panelObject)
                    {
                        removeList.Add(drawingVisual);
                        break;
                    }
                }

                foreach (var drawingVisual in removeList)
                {
                    _visualChildren.Remove(drawingVisual);
                }
            }
        }
    }
}
