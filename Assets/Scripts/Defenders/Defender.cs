using System;
using System.Collections.Generic;
using TowerDefense.Enemies;
using TowerDefense.Grid;
using UnityEngine;

namespace TowerDefense.Defenders
{
    /// <summary>
    /// Logic of one placed defender (design/game-brief.md, MVP feature 2; story 002). Plain C#:
    /// time and the enemy list are passed to <see cref="Tick"/>, nothing reads <c>Time</c>.
    /// <list type="bullet">
    /// <item>Sentry: every <c>FireInterval</c> seconds shoots the nearest enemy on its own lane that is
    /// in front of it (larger X than the defender, within <c>Range</c> cells). Never other lanes or behind.</item>
    /// <item>Barricade: never acts; it is removed when its health reaches 0.</item>
    /// <item>Bomb: as soon as an enemy is in any cell of the (2R+1)x(2R+1) area around it, every enemy in
    /// that area takes <c>Damage</c>, then the bomb is removed. Explodes at most once.</item>
    /// </list>
    /// Not thread-safe.
    /// </summary>
    /// <example>
    /// <code>
    /// var sentry = new Defender(DefenderType.Sentry, config.Sentry, new CellId(2, 1), grid);
    /// sentry.OnFired += (self, target) => ShowShot(self, target);
    /// sentry.Tick(deltaSeconds, enemies);
    /// </code>
    /// </example>
    public sealed class Defender
    {
        private readonly LaneGrid _grid;
        private float _cooldown;

        /// <summary>Creates a defender standing on <paramref name="cell"/> at full health.</summary>
        /// <param name="type">Which defender this is.</param>
        /// <param name="stats">Its stats (from <see cref="DefenderConfig"/>).</param>
        /// <param name="cell">The grid cell it occupies; must be inside <paramref name="grid"/>.</param>
        /// <param name="grid">The lane grid, used to locate the defender and enemies.</param>
        /// <exception cref="ArgumentException">The cell is outside the grid.</exception>
        /// <example><code>var wall = new Defender(DefenderType.Barricade, config.Barricade, new CellId(0, 3), grid);</code></example>
        public Defender(DefenderType type, DefenderStats stats, CellId cell, LaneGrid grid)
        {
            if (!grid.TryCellToWorld(cell, out Vector2 worldPosition))
            {
                throw new ArgumentException($"Cell {cell} is outside the grid.", nameof(cell));
            }

            Type = type;
            Stats = stats;
            Cell = cell;
            WorldPosition = worldPosition;
            Health = stats.Health;
            _grid = grid;
        }

        /// <summary>Raised when a Sentry shoots. Arguments: this defender, the enemy that was hit.</summary>
        /// <example><code>defender.OnFired += (self, target) => Debug.Log("pew");</code></example>
        public event Action<Defender, EnemyHealth> OnFired;

        /// <summary>Raised once when a Bomb explodes, before <see cref="OnRemoved"/>.</summary>
        /// <example><code>defender.OnExploded += self => SpawnFlash(self.WorldPosition);</code></example>
        public event Action<Defender> OnExploded;

        /// <summary>Raised exactly once when the defender leaves the grid: destroyed (0 health) or exploded.</summary>
        /// <example><code>defender.OnRemoved += self => Destroy(view);</code></example>
        public event Action<Defender> OnRemoved;

        /// <summary>Which defender this is.</summary>
        public DefenderType Type { get; }

        /// <summary>The stats this defender was created with.</summary>
        public DefenderStats Stats { get; }

        /// <summary>The cell this defender occupies.</summary>
        public CellId Cell { get; }

        /// <summary>World position of the centre of its cell.</summary>
        public Vector2 WorldPosition { get; }

        /// <summary>Current hit points.</summary>
        public int Health { get; private set; }

        /// <summary>True once the defender has been removed (destroyed or exploded). A removed defender ignores everything.</summary>
        public bool IsRemoved { get; private set; }

        /// <summary>
        /// Reduces health and removes the defender (raising <see cref="OnRemoved"/>) when it reaches 0.
        /// Does nothing once removed or for a non-positive amount.
        /// </summary>
        /// <param name="amount">Hit points to remove.</param>
        /// <example><code>barricade.TakeDamage(10);</code></example>
        public void TakeDamage(int amount)
        {
            if (IsRemoved || amount <= 0)
            {
                return;
            }

            Health = Math.Max(0, Health - amount);
            if (Health == 0)
            {
                Remove();
            }
        }

        /// <summary>
        /// Advances the defender by <paramref name="deltaSeconds"/> and acts on the enemies.
        /// Enemies that die during the call may be removed from <paramref name="enemies"/> by their
        /// <c>OnDied</c> handlers; the list is walked backwards so that is safe as long as handlers
        /// only remove the enemy that just died.
        /// </summary>
        /// <param name="deltaSeconds">Elapsed time since the previous tick, in seconds.</param>
        /// <param name="enemies">The enemies currently alive.</param>
        /// <example><code>defender.Tick(Time.deltaTime, enemies); // from a MonoBehaviour's Update</code></example>
        public void Tick(float deltaSeconds, IReadOnlyList<EnemyHealth> enemies)
        {
            if (IsRemoved || !(deltaSeconds > 0f))
            {
                return;
            }

            switch (Type)
            {
                case DefenderType.Sentry:
                    TickSentry(deltaSeconds, enemies);
                    break;
                case DefenderType.Bomb:
                    TickBomb(enemies);
                    break;
            }
        }

        private void TickSentry(float deltaSeconds, IReadOnlyList<EnemyHealth> enemies)
        {
            _cooldown = Math.Max(0f, _cooldown - deltaSeconds);
            if (_cooldown > 0f)
            {
                return;
            }

            float maxDistance = Stats.Range * _grid.CellSize;
            EnemyHealth target = null;
            float targetDistance = float.MaxValue;
            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyHealth enemy = enemies[i];
                float distance = enemy.Walker.X - WorldPosition.x;
                if (enemy.Walker.Lane == Cell.Lane && distance > 0f && distance <= maxDistance && distance < targetDistance)
                {
                    target = enemy;
                    targetDistance = distance;
                }
            }

            if (target == null)
            {
                return;
            }

            _cooldown = Stats.FireInterval;
            target.TakeDamage(Stats.Damage);
            OnFired?.Invoke(this, target);
        }

        private void TickBomb(IReadOnlyList<EnemyHealth> enemies)
        {
            bool triggered = false;
            for (int i = 0; i < enemies.Count && !triggered; i++)
            {
                triggered = IsInBlastArea(enemies[i]);
            }

            if (!triggered)
            {
                return;
            }

            // Mark removed first so a re-entrant call cannot explode a second time.
            IsRemoved = true;
            OnExploded?.Invoke(this);
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (IsInBlastArea(enemies[i]))
                {
                    enemies[i].TakeDamage(Stats.Damage);
                }
            }

            OnRemoved?.Invoke(this);
        }

        private bool IsInBlastArea(EnemyHealth enemy)
        {
            // Cells off the grid (e.g. an enemy still at the spawn edge) never count.
            if (!_grid.TryGetLaneY(enemy.Walker.Lane, out float laneY)
                || !_grid.TryWorldToCell(new Vector2(enemy.Walker.X, laneY), out CellId enemyCell))
            {
                return false;
            }

            return Math.Abs(enemyCell.Lane - Cell.Lane) <= Stats.BlastRadius
                && Math.Abs(enemyCell.Column - Cell.Column) <= Stats.BlastRadius;
        }

        private void Remove()
        {
            IsRemoved = true;
            OnRemoved?.Invoke(this);
        }
    }
}
