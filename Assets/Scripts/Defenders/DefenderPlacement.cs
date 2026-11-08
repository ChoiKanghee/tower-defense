using System.Collections.Generic;
using TowerDefense.Grid;

namespace TowerDefense.Defenders
{
    /// <summary>Outcome of <see cref="DefenderPlacement.TryPlace"/>.</summary>
    public enum PlacementResult
    {
        /// <summary>The defender was placed.</summary>
        Placed,

        /// <summary>Rejected: the cell is outside the grid.</summary>
        OutOfGrid,

        /// <summary>Rejected: the cell already holds a defender.</summary>
        CellOccupied,
    }

    /// <summary>
    /// Tracks which grid cell holds which defender (story 002). Placing on an empty cell succeeds;
    /// an occupied or out-of-grid cell is rejected with a <see cref="PlacementResult"/> (never an
    /// exception). A cell is freed automatically when its defender is removed (destroyed or
    /// exploded), so it can be reused. No supply or cost logic here (story 004). Plain C#, not thread-safe.
    /// </summary>
    /// <example>
    /// <code>
    /// var placement = new DefenderPlacement(grid, DefenderConfig.Load());
    /// PlacementResult result = placement.TryPlace(DefenderType.Sentry, new CellId(2, 1), out Defender sentry);
    /// </code>
    /// </example>
    public sealed class DefenderPlacement
    {
        private readonly LaneGrid _grid;
        private readonly DefenderConfig _config;
        private readonly Dictionary<CellId, Defender> _occupied = new Dictionary<CellId, Defender>();

        /// <summary>Creates an empty placement map for a grid.</summary>
        /// <param name="grid">The lane grid defenders are placed on.</param>
        /// <param name="config">Defender stats source.</param>
        /// <example><code>var placement = new DefenderPlacement(grid, config);</code></example>
        public DefenderPlacement(LaneGrid grid, DefenderConfig config)
        {
            _grid = grid;
            _config = config;
        }

        /// <summary>True when the cell currently holds a defender.</summary>
        /// <example><code>bool taken = placement.IsOccupied(new CellId(0, 0));</code></example>
        public bool IsOccupied(CellId cell) => _occupied.ContainsKey(cell);

        /// <summary>Gets the defender on a cell.</summary>
        /// <param name="cell">The cell to look up.</param>
        /// <param name="defender">The defender, or null when the cell is empty or outside the grid.</param>
        /// <returns><c>false</c> when there is no defender on the cell.</returns>
        /// <example><code>if (placement.TryGetDefender(cell, out Defender d)) { d.TakeDamage(5); }</code></example>
        public bool TryGetDefender(CellId cell, out Defender defender) => _occupied.TryGetValue(cell, out defender);

        /// <summary>Tries to place a new defender of <paramref name="type"/> on <paramref name="cell"/>.</summary>
        /// <param name="type">Which defender to place.</param>
        /// <param name="cell">Target cell.</param>
        /// <param name="defender">The new defender when placed, otherwise null.</param>
        /// <returns><see cref="PlacementResult.Placed"/>, or the reason it was rejected.</returns>
        /// <example>
        /// <code>
        /// if (placement.TryPlace(DefenderType.Bomb, cell, out Defender bomb) == PlacementResult.Placed) { /* spawn view */ }
        /// </code>
        /// </example>
        public PlacementResult TryPlace(DefenderType type, CellId cell, out Defender defender)
        {
            defender = null;
            if (!_grid.Contains(cell))
            {
                return PlacementResult.OutOfGrid;
            }

            if (_occupied.ContainsKey(cell))
            {
                return PlacementResult.CellOccupied;
            }

            defender = new Defender(type, _config.Get(type), cell, _grid);
            defender.OnRemoved += OnDefenderRemoved;
            _occupied.Add(cell, defender);
            return PlacementResult.Placed;
        }

        private void OnDefenderRemoved(Defender defender)
        {
            _occupied.Remove(defender.Cell);
        }
    }
}
