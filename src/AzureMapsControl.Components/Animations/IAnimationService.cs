﻿namespace AzureMapsControl.Components.Animations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using AzureMapsControl.Components.Animations.Options;
    using AzureMapsControl.Components.Atlas;
    using AzureMapsControl.Components.Data;
    using AzureMapsControl.Components.Layers;
    using AzureMapsControl.Components.Markers;

    public interface IAnimationService
    {
        /// <summary>
        /// Animates the path of a LineString. 
        /// </summary>
        /// <param name="line">A LineString shape to animate.</param>
        /// <param name="source">DataSource the given line string is attached to</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns>Animation</returns>
        Task<ISnakeLineAnimation> SnakelineAsync(LineString line, DataSource source, SnakeLineAnimationOptions options = default);

        /// <summary>
        /// Animates a map and/or a Point shape, or marker along a path.
        /// </summary>
        /// <param name="path">The path to animate the point along.</param>
        /// <param name="pathSource">The data source the given line is attached to</param>
        /// <param name="pin">A point to animate along the path</param>
        /// <param name="pinSource">The data source the given point is attached to</param>
        /// <param name="options">Options for the animation</param>
        /// <returns>Animation</returns>
        Task<IMoveAlongPathAnimation> MoveAlongPathAsync(LineString path, DataSource pathSource, Point pin, DataSource pinSource, MoveAlongPathAnimationOptions options = default);

        /// <summary>
        /// Animates a map and/or a Point shape, or marker along a path.
        /// </summary>
        /// <param name="path">The path to animate the point along.</param>
        /// <param name="lineSource">The data source the given line is attached to</param>
        /// <param name="pin">An HTML Marker to animate along the path</param>
        /// <param name="options">Options for the animation</param>
        /// <returns>Animation</returns>
        Task<IMoveAlongPathAnimation> MoveAlongPathAsync(LineString path, DataSource pathSource, HtmlMarker pin, MoveAlongPathAnimationOptions options = default);

        /// <summary>
        /// Animates a map and/or a Point shape, or marker along a path.
        /// </summary>
        /// <param name="path">The path to animate the point along.</param>
        /// <param name="pin">A point to animate along the path</param>
        /// <param name="pinSource">The data source the given point is attached to</param>
        /// <param name="options">Options for the animation</param>
        /// <returns>Animation</returns>
        Task<IMoveAlongPathAnimation> MoveAlongPathAsync(IEnumerable<Position> path, Point pin, DataSource pinSource, MoveAlongPathAnimationOptions options = default);

        /// <summary>
        /// Animates a map and/or a Point shape, or marker along a path.
        /// </summary>
        /// <param name="path">The path to animate the point along.</param>
        /// <param name="pin">A point to animate along the path</param>
        /// <param name="options">Options for the animation</param>
        /// <returns>Animation</returns>
        Task<IMoveAlongPathAnimation> MoveAlongPathAsync(IEnumerable<Position> path, HtmlMarker pin, MoveAlongPathAnimationOptions options = default);

        /// <summary>
        /// Animates the dash-array of a line layer to make it appear to flow. 
        /// </summary>
        /// <param name="layer">LineLayer to animate</param>
        /// <param name="options">Options of the animations</param>
        /// <returns>Animation</returns>
        Task<IFlowingDashedLineAnimation> FlowingDashedLineAsync(LineLayer layer, MovingDashLineOptions options = default);

        /// <summary>
        /// Adds an offset to HtmlMarkers to animate its y value to simulate dropping. Animation modifies `pixelOffset` value of HtmlMarkers.
        /// </summary>
        /// <param name="markers">HtmlMarkers to drop in.</param>
        /// <param name="height">The height at which to drop the shape from.</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns></returns>
        Task<IDropMarkersAnimation> DropMarkersAsync(IEnumerable<HtmlMarker> markers, decimal? height = null, DropMarkersAnimationOptions options = default);

        /// <summary>
        /// Adds an offset to an HtmlMarker to animate its y value to simulate dropping. Animation modifies `pixelOffset` value of HtmlMarker.
        /// </summary>
        /// <param name="markers">HtmlMarkers to drop in.</param>
        /// <param name="height">The height at which to drop the shape from.</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns></returns>
        Task<IDropMarkersAnimation> DropMarkerAsync(HtmlMarker marker, decimal? height = null, DropMarkersAnimationOptions options = default);

        /// <summary>
        /// Group animations
        /// </summary>
        /// <param name="animations">Animations to group</param>
        /// <param name="options">Options for the animation</param>
        /// <returns>Animation</returns>
        Task<IGroupAnimation> GroupAnimationAsync(IEnumerable<IAnimation> animations, GroupAnimationOptions options = default);

        /// <summary>
        /// Adds an offset array property to point shapes and animates its y value to simulate dropping.
        /// </summary>
        /// <param name="points">A one or more point geometries to drop in</param>
        /// <param name="source">The data source to drop the point shapes into.</param>
        /// <param name="height">The height at which to drop the shape from</param>
        /// <param name="options">Options for the animation</param>
        /// <returns></returns>
        Task<IDropAnimation> DropAsync(IEnumerable<Point> points, DataSource source, decimal? height = null, DropAnimationOptions options = default);

        /// <summary>
        /// Adds an offset array property to a point shape and animates its y value to simulate dropping.
        /// </summary>
        /// <param name="point">A point to drop in</param>
        /// <param name="source">The data source to drop the point shapes into.</param>
        /// <param name="height">The height at which to drop the shape from</param>
        /// <param name="options">Options for the animation</param>
        /// <returns></returns>
        Task<IDropAnimation> DropAsync(Point point, DataSource source, decimal? height = null, DropAnimationOptions options = default);
    }
}