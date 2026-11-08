using System;
using UnityEngine;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Movement logic for a generic enemy walking along one lane, from a start X to an end X at a
    /// constant speed. The lane is fixed at construction and never changes. Time is passed in to
    /// <see cref="Tick"/>; nothing here reads <c>Time</c>. Each walker owns all of its state, so
    /// walkers on different lanes are independent. Not thread-safe: tick and subscribe from one thread.
    /// </summary>
    /// <example>
    /// <code>
    /// var walker = new LaneWalker(lane: 2, startX: grid.LaneStartX, endX: grid.LaneEndX, speed: 1f);
    /// walker.OnReachedEnd += lane => Debug.Log($"Enemy got through on lane {lane}");
    /// walker.Tick(deltaSeconds);
    /// </code>
    /// </example>
    public sealed class LaneWalker
    {
        private readonly float _endX;
        private readonly float _speed;

        /// <summary>Creates a walker standing at <paramref name="startX"/>.</summary>
        /// <param name="lane">0-based lane the walker walks on.</param>
        /// <param name="startX">World X where the walker starts.</param>
        /// <param name="endX">World X where the lane ends.</param>
        /// <param name="speed">World units per second; expected positive (validated in config).</param>
        /// <example><code>var walker = new LaneWalker(2, 4.5f, -4.5f, 1f);</code></example>
        public LaneWalker(int lane, float startX, float endX, float speed)
        {
            Lane = lane;
            X = startX;
            _endX = endX;
            _speed = speed;
        }

        /// <summary>
        /// Raised exactly once, during the <see cref="Tick"/> in which the walker reaches the end of
        /// its lane. The argument is the lane number.
        /// </summary>
        /// <example><code>walker.OnReachedEnd += lane => breachedLanes.Add(lane);</code></example>
        public event Action<int> OnReachedEnd;

        /// <summary>The lane this walker walks on. Never changes.</summary>
        public int Lane { get; }

        /// <summary>Current world X along the lane.</summary>
        public float X { get; private set; }

        /// <summary>True once the walker has reached the end of its lane.</summary>
        public bool HasReachedEnd { get; private set; }

        /// <summary>
        /// Advances the walker toward the lane end by <c>speed * deltaSeconds</c>, stopping at the end.
        /// Does nothing once the end is reached or when <paramref name="deltaSeconds"/> is not positive.
        /// </summary>
        /// <param name="deltaSeconds">Elapsed time since the previous tick, in seconds.</param>
        /// <example><code>walker.Tick(Time.deltaTime); // from a MonoBehaviour's Update</code></example>
        public void Tick(float deltaSeconds)
        {
            if (HasReachedEnd || !(deltaSeconds > 0f))
            {
                return;
            }

            X = Mathf.MoveTowards(X, _endX, _speed * deltaSeconds);
            if (X == _endX)
            {
                HasReachedEnd = true;
                OnReachedEnd?.Invoke(Lane);
            }
        }
    }
}
