using System;
using UnityEngine;

namespace TowerDefense.Grid
{
    /// <summary>
    /// Identifies one cell of a <see cref="LaneGrid"/> by its 0-based lane and column.
    /// </summary>
    /// <example>
    /// <code>var cell = new CellId(lane: 2, column: 0);</code>
    /// </example>
    public readonly struct CellId : IEquatable<CellId>
    {
        /// <summary>Creates a cell id. The values are not range-checked; use <see cref="LaneGrid.Contains"/>.</summary>
        /// <param name="lane">0-based lane index (0 is the top lane).</param>
        /// <param name="column">0-based column index (0 is the leftmost column).</param>
        public CellId(int lane, int column)
        {
            Lane = lane;
            Column = column;
        }

        /// <summary>0-based lane index; 0 is the top lane.</summary>
        public int Lane { get; }

        /// <summary>0-based column index; 0 is the leftmost column.</summary>
        public int Column { get; }

        /// <summary>True when both lane and column match.</summary>
        /// <example><code>bool same = new CellId(1, 2).Equals(new CellId(1, 2)); // true</code></example>
        public bool Equals(CellId other) => Lane == other.Lane && Column == other.Column;

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is CellId other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Lane, Column);

        /// <summary>Equality by lane and column.</summary>
        /// <example><code>bool same = new CellId(1, 2) == new CellId(1, 2); // true</code></example>
        public static bool operator ==(CellId left, CellId right) => left.Equals(right);

        /// <summary>Inequality by lane and column.</summary>
        /// <example><code>bool differ = new CellId(1, 2) != new CellId(1, 3); // true</code></example>
        public static bool operator !=(CellId left, CellId right) => !left.Equals(right);

        /// <inheritdoc/>
        public override string ToString() => $"(lane {Lane}, column {Column})";
    }

    /// <summary>
    /// A rectangular lane grid of square cells, centred on a world point. Lane 0 is the top row,
    /// column 0 the leftmost column. Enemies enter at the right edge (<see cref="LaneStartX"/>) and
    /// walk left to the left edge (<see cref="LaneEndX"/>). Plain C#, no engine lifecycle; not
    /// thread-safe for mutation, but it is immutable after construction so concurrent reads are safe.
    /// Queries outside the grid return <c>false</c> and never throw.
    /// </summary>
    /// <example>
    /// <code>
    /// var grid = new LaneGrid(lanes: 5, columns: 9, cellSize: 1f, center: Vector2.zero);
    /// if (grid.TryCellToWorld(new CellId(2, 4), out Vector2 world)
    ///     &amp;&amp; grid.TryWorldToCell(world, out CellId back))
    /// {
    ///     // back == new CellId(2, 4)
    /// }
    /// </code>
    /// </example>
    public sealed class LaneGrid
    {
        private readonly Vector2 _bottomLeft;

        /// <summary>
        /// Creates a grid. Sizes are expected to be positive; they are validated where they enter
        /// the game (<see cref="LaneGridConfig.Load"/>), not here.
        /// </summary>
        /// <param name="lanes">Number of lanes (rows).</param>
        /// <param name="columns">Number of columns per lane.</param>
        /// <param name="cellSize">Side length of one cell, in world units.</param>
        /// <param name="center">World position of the grid's centre.</param>
        /// <example><code>var grid = new LaneGrid(5, 9, 1f, Vector2.zero);</code></example>
        public LaneGrid(int lanes, int columns, float cellSize, Vector2 center)
        {
            Lanes = lanes;
            Columns = columns;
            CellSize = cellSize;
            Center = center;
            Width = columns * cellSize;
            Height = lanes * cellSize;
            _bottomLeft = center - new Vector2(Width, Height) * 0.5f;
        }

        /// <summary>Number of lanes (rows).</summary>
        public int Lanes { get; }

        /// <summary>Number of columns per lane.</summary>
        public int Columns { get; }

        /// <summary>Side length of one cell, in world units.</summary>
        public float CellSize { get; }

        /// <summary>World position of the grid's centre.</summary>
        public Vector2 Center { get; }

        /// <summary>Total width in world units (<c>Columns * CellSize</c>).</summary>
        public float Width { get; }

        /// <summary>Total height in world units (<c>Lanes * CellSize</c>).</summary>
        public float Height { get; }

        /// <summary>World X where enemies enter a lane: the grid's right edge.</summary>
        /// <example><code>var walker = new LaneWalker(2, grid.LaneStartX, grid.LaneEndX, 1f);</code></example>
        public float LaneStartX => _bottomLeft.x + Width;

        /// <summary>World X where a lane ends: the grid's left edge.</summary>
        /// <example><code>var walker = new LaneWalker(2, grid.LaneStartX, grid.LaneEndX, 1f);</code></example>
        public float LaneEndX => _bottomLeft.x;

        /// <summary>True when the cell lies inside the grid.</summary>
        /// <example><code>bool inside = grid.Contains(new CellId(5, 0)); // false on a 5-lane grid</code></example>
        public bool Contains(CellId cell) =>
            cell.Lane >= 0 && cell.Lane < Lanes && cell.Column >= 0 && cell.Column < Columns;

        /// <summary>Gets the world position of a cell's centre.</summary>
        /// <param name="cell">The cell to convert.</param>
        /// <param name="center">The cell centre, or <c>default</c> when the cell is outside the grid.</param>
        /// <returns><c>false</c> when the cell is outside the grid.</returns>
        /// <example><code>if (grid.TryCellToWorld(new CellId(0, 0), out Vector2 p)) { /* top-left cell centre */ }</code></example>
        public bool TryCellToWorld(CellId cell, out Vector2 center)
        {
            if (!Contains(cell))
            {
                center = default;
                return false;
            }

            center = new Vector2(
                _bottomLeft.x + (cell.Column + 0.5f) * CellSize,
                _bottomLeft.y + (Lanes - 1 - cell.Lane + 0.5f) * CellSize);
            return true;
        }

        /// <summary>Gets the cell containing a world position.</summary>
        /// <param name="world">World position to look up.</param>
        /// <param name="cell">The containing cell, or <c>default</c> when the position is outside the grid.</param>
        /// <returns><c>false</c> when the position is outside the grid (or not a number).</returns>
        /// <example><code>if (grid.TryWorldToCell(clickWorld, out CellId cell)) { /* place on cell */ }</code></example>
        public bool TryWorldToCell(Vector2 world, out CellId cell)
        {
            float localX = world.x - _bottomLeft.x;
            float localY = world.y - _bottomLeft.y;

            // Written as a negated conjunction so NaN coordinates are rejected too.
            if (!(localX >= 0f && localX < Width && localY >= 0f && localY < Height))
            {
                cell = default;
                return false;
            }

            // Min guards against float rounding putting a point just inside the far edge one index past it.
            int column = Math.Min(Mathf.FloorToInt(localX / CellSize), Columns - 1);
            int rowFromBottom = Math.Min(Mathf.FloorToInt(localY / CellSize), Lanes - 1);
            cell = new CellId(Lanes - 1 - rowFromBottom, column);
            return true;
        }

        /// <summary>Gets the world Y of a lane's centre line.</summary>
        /// <param name="lane">0-based lane index.</param>
        /// <param name="y">The lane centre Y, or 0 when the lane is outside the grid.</param>
        /// <returns><c>false</c> when the lane is outside the grid.</returns>
        /// <example><code>if (grid.TryGetLaneY(2, out float y)) { /* spawn on lane 2 at y */ }</code></example>
        public bool TryGetLaneY(int lane, out float y)
        {
            if (lane < 0 || lane >= Lanes)
            {
                y = 0f;
                return false;
            }

            y = _bottomLeft.y + (Lanes - 1 - lane + 0.5f) * CellSize;
            return true;
        }
    }
}
