namespace WpfDrawing.Panel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Media;

    using WpfDrawing.Utility;

    public class PanelVisuals : FrameworkElement
    {
        #region DependencyProperties

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

        public static readonly DependencyProperty ViewOffsetProperty = DependencyProperty.Register(
            "ViewOffset",
            typeof(Point),
            typeof(PanelVisuals),
            new PropertyMetadata(new Point(0, 0), OnViewOffsetChanged));

        public static readonly DependencyProperty WorldSizeProperty = DependencyProperty.Register(
            "WorldSize",
            typeof(Rect),
            typeof(PanelVisuals),
            new PropertyMetadata(new Rect(new Size(0, 0)), (o, args) => ((PanelVisuals)o).Refresh()),
            value => ((Rect)value).Width >= 0 && ((Rect)value).Height >= 0);

        #endregion

        private readonly VisualCollection _visualChildren;

        public PanelVisuals()
        {
            _visualChildren = new VisualCollection(this);
            SizeChanged += (sender, args) => Refresh();
        }

        #region Properties

        public ObservableNotifiableCollection<PanelObject> ItemsSource
        {
            get => (ObservableNotifiableCollection<PanelObject>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        public Point ViewOffset
        {
            get => (Point)GetValue(ViewOffsetProperty);
            set => SetValue(ViewOffsetProperty, value);
        }

        public Rect WorldSize
        {
            get => (Rect)GetValue(WorldSizeProperty);
            set => SetValue(WorldSizeProperty, value);
        }

        protected override int VisualChildrenCount => _visualChildren.Count;

        #endregion

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            foreach (var child in _visualChildren)
            {
                var drawingVisual = child as PanelDrawingVisual;
                if (!(drawingVisual?.Transform is TranslateTransform translateTransform))
                {
                    continue;
                }

                if (sizeInfo.WidthChanged)
                {
                    translateTransform.X = sizeInfo.NewSize.Width * drawingVisual.PanelObject.X;
                }

                if (sizeInfo.HeightChanged)
                {
                    translateTransform.Y = sizeInfo.NewSize.Height * drawingVisual.PanelObject.Y;
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

        private static void OnViewOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as PanelVisuals)?.Refresh();
        }

        private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            _visualChildren.Clear();

            if (args.OldValue != null && args.OldValue is ObservableNotifiableCollection<PanelObject> oldColl)
            {
                oldColl.CollectionCleared -= OnCollectionCleared;
                oldColl.CollectionChanged -= OnCollectionChanged;
                oldColl.ItemPropertyChanged -= OnItemPropertyChanged;
            }

            if (args.NewValue != null && args.NewValue is ObservableNotifiableCollection<PanelObject> newColl)
            {
                newColl.CollectionCleared += OnCollectionCleared;
                newColl.CollectionChanged += OnCollectionChanged;
                newColl.ItemPropertyChanged += OnItemPropertyChanged;

                CreateVisualChildren(newColl);
            }
        }

        private void OnCollectionCleared(object sender, EventArgs args)
        {
            _visualChildren.Clear();
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
            if (!(args.Item is PanelObject panelObject))
            {
                return;
            }

            foreach (var child in _visualChildren)
            {
                if (!(child is PanelDrawingVisual drawingVisual) || drawingVisual.PanelObject != panelObject)
                {
                    continue;
                }

                drawingVisual.Transform = panelObject.TransformGroup;
                break;
            }
        }

        private void Refresh()
        {
            if (RenderSize.IsEmpty || Math.Abs(RenderSize.Width) < double.Epsilon || Math.Abs(RenderSize.Height) < double.Epsilon)
            {
                return;
            }

            UpdateRenderTransform();
        }

        private void UpdateRenderTransform()
        {
            if (WorldSize.Height <= 0 || WorldSize.Width <= 0 || RenderSize.Height <= 0 || RenderSize.Width <= 0)
            {
                return;
            }

            RenderTransform = CoordinateSystem.CalculateWorldToViewTransformation(RenderSize, WorldSize, ViewOffset, Zoom);
        }

        private void CreateVisualChildren(IEnumerable coll)
        {
            if (coll == null || WorldSize.Width <= 0 || WorldSize.Height <= 0 || RenderSize.Width <= 0 || RenderSize.Height <= 0)
            {
                return;
            }

            foreach (var obj in coll)
            {
                if (!(obj is PanelObject panelObject))
                {
                    continue;
                }

                var drawingVisual = new PanelDrawingVisual { PanelObject = panelObject };
                var dc = drawingVisual.RenderOpen();
                panelObject.Draw(dc);
                drawingVisual.Transform = panelObject.TransformGroup;
                dc.Close();
                _visualChildren.Add(drawingVisual);
            }

            UpdateRenderTransform();
        }

        private void RemoveVisualChildren(IEnumerable coll)
        {
            foreach (var obj in coll)
            {
                if (!(obj is PanelObject panelObject))
                {
                    continue;
                }

                var removeList = new List<PanelDrawingVisual>();
                foreach (var child in _visualChildren)
                {
                    if (!(child is PanelDrawingVisual drawingVisual))
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
