namespace WpfDrawing.Panel
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    public static class CoordinateSystem
    {
        /// <summary>
        /// Calculates the width to height ratio of a size.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>The ratio.</returns>
        /// <exception cref="ArgumentException">The width or height of <paramref name="size"/> is smaller or equal to 0.</exception>
        public static double CalculateWidthToHeightRatio(Size size)
        {
            if (size.Width <= 0 || size.Height <= 0)
            {
                throw new ArgumentException("Size must be positive.");
            }

            return size.Width / size.Height;
        }

        /// <summary>
        /// Calculates the world to view coordinate scale factor. In other words, how many pixels fit into one unit of the world.
        /// </summary>
        /// <param name="worldSize">The world size.</param>
        /// <param name="viewSize">The view size.</param>
        /// <returns>The scale factor.</returns>
        /// <exception cref="ArgumentException">Any width or height of either <paramref name="worldSize"/> or <paramref name="viewSize"/> is smaller or equal to 0.</exception>
        public static double CalculateWorldToViewCoordinateScaleFactor(Size worldSize, Size viewSize)
        {
            if (worldSize.Width <= 0 || worldSize.Height <= 0 || viewSize.Width <= 0 || viewSize.Height <= 0)
            {
                throw new ArgumentException("All sizes must be positive.");
            }

            var worldRatio = CalculateWidthToHeightRatio(worldSize);
            var viewRatio = CalculateWidthToHeightRatio(viewSize);
            return worldRatio >= viewRatio ? viewSize.Width / worldSize.Width : viewSize.Height / worldSize.Height;
        }

        /// <summary>
        /// Calculates the view to world coordinate scale factor. In other words, how many world units fit into one pixel.
        /// </summary>
        /// <param name="worldSize">The world size.</param>
        /// <param name="viewSize">The view size.</param>
        /// <returns>The scale factor.</returns>
        /// <exception cref="ArgumentException">Any width or height of either <paramref name="worldSize"/> or <paramref name="viewSize"/> is smaller or equal to 0.</exception>
        public static double CalculateViewToWorldCoordinateScaleFactor(Size worldSize, Size viewSize)
        {
            if (worldSize.Width <= 0 || worldSize.Height <= 0 || viewSize.Width <= 0 || viewSize.Height <= 0)
            {
                throw new ArgumentException("All sizes must be positive.");
            }

            var worldRatio = CalculateWidthToHeightRatio(worldSize);
            var viewRatio = CalculateWidthToHeightRatio(viewSize);
            return worldRatio >= viewRatio ? worldSize.Width / viewSize.Width : worldSize.Height / viewSize.Height;
        }

        /// <summary>
        /// Calculates the world to view conversion change when either the world size or view size has changed.
        /// </summary>
        /// <param name="previousWorldSize">The previous world size.</param>
        /// <param name="currentWorldSize">The current world size.</param>
        /// <param name="previousViewSize">The previous view size.</param>
        /// <param name="currentViewSize">The current view size.</param>
        /// <returns>The world to render conversion change.</returns>
        /// <exception cref="ArgumentException">Any width or height of either <paramref name="previousWorldSize"/> or <paramref name="currentWorldSize"/> or <paramref name="previousViewSize"/> or <paramref name="currentViewSize"/> is smaller or equal to 0.</exception>
        public static double CalculateWorldToViewConversionChange(Size previousWorldSize, Size currentWorldSize, Size previousViewSize, Size currentViewSize)
        {
            if (previousWorldSize.Width <= 0
                || previousWorldSize.Height <= 0
                || currentWorldSize.Width <= 0
                || currentWorldSize.Height <= 0
                || previousViewSize.Width <= 0
                || previousViewSize.Height <= 0
                || currentViewSize.Width <= 0
                || currentViewSize.Height <= 0)
            {
                throw new ArgumentException("All sizes must be positive.");
            }

            var previousWorldToViewConversion = CalculateWorldToViewCoordinateScaleFactor(previousWorldSize, previousViewSize);
            var currentWorldToViewConversion = CalculateWorldToViewCoordinateScaleFactor(currentWorldSize, currentViewSize);
            return previousWorldToViewConversion / currentWorldToViewConversion;
        }

        /// <summary>
        /// Calculates the world to view transformation.
        /// </summary>
        /// <param name="view">The view size.</param>
        /// <param name="world">The world rectangle.</param>
        /// <param name="viewOffset">The view offset.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <returns>The transformation.</returns>
        /// <exception cref="ArgumentException"><paramref name="view"/> is either empty or its width or height is smaller or equal to 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="world"/> is either empty or its width or height is smaller or equal to 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="zoom"/> is smaller or equal to 0.</exception>
        public static TransformGroup CalculateWorldToViewTransformation(Size view, Rect world, Point viewOffset, double zoom = 1.0)
        {
            if (view.IsEmpty || view.Width <= 0 || view.Height <= 0)
            {
                throw new ArgumentException(@"View must not be empty, nor may its width or height be smaller or equal to 0.", nameof(view));
            }

            if (world.IsEmpty || world.Width <= 0 || world.Height <= 0)
            {
                throw new ArgumentException(@"World size must not be empty, nor may its width or height be smaller or equal to 0.", nameof(world));
            }

            if (zoom <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(zoom), zoom, @"Zoom must not be smaller or equal to 0.");
            }

            // Calculate the centers of the view and world.
            var viewCenter = new Point(view.Width * 0.5, view.Height * 0.5);
            var worldCenter = new Point(world.Left + world.Width * 0.5, world.Top + world.Height * 0.5);

            // Move world center to view center.
            var translate = new TranslateTransform(viewCenter.X - worldCenter.X, viewCenter.Y - worldCenter.Y);

            // Calculate scale factor.
            var worldToViewScaleFactor = CalculateWorldToViewCoordinateScaleFactor(world.Size, view);
            var scaleFactor = zoom * worldToViewScaleFactor;

            // Scale the world at the view center.
            var scale = new ScaleTransform(scaleFactor, scaleFactor, viewCenter.X, viewCenter.Y);

            // Add the view offset.
            var offset = new TranslateTransform(viewOffset.X, viewOffset.Y);

            var group = new TransformGroup();
            group.Children.Add(translate);
            group.Children.Add(scale);
            group.Children.Add(offset);
            return group;
        }

        /// <summary>
        /// Calculates the view offset creating by zooming in or out.
        /// </summary>
        /// <param name="view">The view size.</param>
        /// <param name="world">The world rectangle.</param>
        /// <param name="previousZoom">The previous zoom factor.</param>
        /// <param name="currentZoom">The current zoom factor.</param>
        /// <param name="previousZoomCenterView">The previous zoom center in view coordinates.</param>
        /// <param name="previousOffset">The previous view offset that is added to the calculated offset.</param>
        /// <returns>The view offset.</returns>
        /// <exception cref="ArgumentException"><paramref name="view"/> is either empty or its width or height is smaller or equal to 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="world"/> is either empty or its width or height is smaller or equal to 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="previousZoom"/> or <paramref name="currentZoom"/> is smaller or equal to 0.</exception>
        public static Point CalculateZoomingOffset(Size view, Rect world, double previousZoom, double currentZoom, Point previousZoomCenterView, Point previousOffset)
        {
            if (view.IsEmpty || view.Width <= 0 || view.Height <= 0)
            {
                throw new ArgumentException(@"View must not be empty, nor may its width or height be smaller or equal to 0.", nameof(view));
            }

            if (world.IsEmpty || world.Width <= 0 || world.Height <= 0)
            {
                throw new ArgumentException(@"World size must not be empty, nor may its width or height be smaller or equal to 0.", nameof(world));
            }

            if (previousZoom <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(previousZoom), previousZoom, @"Zoom must not be smaller or equal to 0.");
            }

            if (currentZoom <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(currentZoom), currentZoom, @"Zoom must not be smaller or equal to 0.");
            }

            // The previous zoom center in world coordinates. After zooming, these have to be at the same view coordinates as before.
            var previousZoomCenterWorld = TransformPointFromViewToWorld(previousZoomCenterView, view, world, previousOffset, previousZoom);

            // After zooming, the previous zoom center in world coordinates has these view coordinates.
            var currentZoomCenterView = TransformPointFromWorldToView(previousZoomCenterWorld, view, world, previousOffset, currentZoom);

            // The vector from the current zoom center to the previous zoom center in view coordinates.
            var currentToPreviousZoomCenterView = previousZoomCenterView - currentZoomCenterView;

            // Add the previous offset to the vector to get the new offset.
            var currentOffset = previousOffset + currentToPreviousZoomCenterView;

            return currentOffset;
        }

        /// <summary>
        /// Transforms a point from world coordinates to view coordinates.
        /// </summary>
        /// <param name="point">The point to transform.</param>
        /// <param name="view">The view size.</param>
        /// <param name="world">The world rectangle.</param>
        /// <param name="viewOffset">The view offset.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <returns>The transformed point.</returns>
        /// <exception cref="ArgumentException"><paramref name="view"/> is either empty or its width or height is smaller or equal to 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="world"/> is either empty or its width or height is smaller or equal to 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="zoom"/> is smaller or equal to 0.</exception>
        public static Point TransformPointFromWorldToView(Point point, Size view, Rect world, Point viewOffset, double zoom = 1.0)
        {
            if (view.IsEmpty || view.Width <= 0 || view.Height <= 0)
            {
                throw new ArgumentException(@"View must not be empty, nor may its width or height be smaller or equal to 0.", nameof(view));
            }

            if (world.IsEmpty || world.Width <= 0 || world.Height <= 0)
            {
                throw new ArgumentException(@"World size must not be empty, nor may its width or height be smaller or equal to 0.", nameof(world));
            }

            if (zoom <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(zoom), zoom, @"Zoom must not be smaller or equal to 0.");
            }

            var transformGroup = CalculateWorldToViewTransformation(view, world, viewOffset, zoom);
            return transformGroup.Transform(point);
        }

        /// <summary>
        /// Transforms a point from view coordinates to world coordinates.
        /// </summary>
        /// <param name="point">The point to transform.</param>
        /// <param name="view">The view size.</param>
        /// <param name="world">The world rectangle.</param>
        /// <param name="viewOffset">The view offset.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <returns>The transformed point.</returns>
        /// <exception cref="ArgumentException"><paramref name="view"/> is either empty or its width or height is smaller or equal to 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="world"/> is either empty or its width or height is smaller or equal to 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="zoom"/> is smaller or equal to 0.</exception>
        /// <exception cref="InvalidOperationException">The inverse of the world to view transformation does not exist.</exception>
        public static Point TransformPointFromViewToWorld(Point point, Size view, Rect world, Point viewOffset, double zoom = 1.0)
        {
            if (view.IsEmpty || view.Width <= 0 || view.Height <= 0)
            {
                throw new ArgumentException(@"View must not be empty, nor may its width or height be smaller or equal to 0.", nameof(view));
            }

            if (world.IsEmpty || world.Width <= 0 || world.Height <= 0)
            {
                throw new ArgumentException(@"World size must not be empty, nor may its width or height be smaller or equal to 0.", nameof(world));
            }

            if (zoom <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(zoom), zoom, @"Zoom must not be smaller or equal to 0.");
            }

            var transformGroup = CalculateWorldToViewTransformation(view, world, viewOffset, zoom);
            return transformGroup.Inverse?.Transform(point) ?? throw new InvalidOperationException("Cannot inverse world to view transformation.");
        }

        /// <summary>
        /// Calculates the maximum zoom factor for the needed resolution in world units and a given world size and view size.
        /// </summary>
        /// <param name="worldResolution">The resolution in world units -> How many world units should be 1 pixel.</param>
        /// <param name="worldSize">The world size.</param>
        /// <param name="viewSize">The view size.</param>
        /// <returns>The maximum zoom factor.</returns>
        /// <exception cref="ArgumentException"><paramref name="worldResolution"/> is smaller or equal to 0.</exception>
        /// <exception cref="ArgumentException">Any width or height of either <paramref name="worldSize"/> or <paramref name="viewSize"/> is smaller or equal to 0.</exception>
        public static double CalculateMaxZoomFactor(double worldResolution, Size worldSize, Size viewSize)
        {
            if (worldResolution <= 0)
            {
                throw new ArgumentException("World resolution must be greater than 0.");
            }

            if (worldSize.Width <= 0 || worldSize.Height <= 0 || viewSize.Width <= 0 || viewSize.Height <= 0)
            {
                throw new ArgumentException("All sizes must be positive.");
            }

            var worldUnitsInOnePixel = CalculateViewToWorldCoordinateScaleFactor(worldSize, viewSize);
            return worldUnitsInOnePixel / worldResolution;
        }
    }
}
