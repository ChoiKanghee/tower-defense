using UnityEngine;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Thin driver that ticks a <see cref="LaneWalker"/> every frame and places this GameObject at
    /// the walker's X on a fixed lane Y. All movement rules live in <see cref="LaneWalker"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// var behaviour = enemyObject.AddComponent&lt;LaneWalkerBehaviour&gt;();
    /// behaviour.Initialize(new LaneWalker(2, grid.LaneStartX, grid.LaneEndX, config.EnemySpeed), laneY);
    /// </code>
    /// </example>
    public sealed class LaneWalkerBehaviour : MonoBehaviour
    {
        private LaneWalker _walker;
        private float _laneY;

        /// <summary>The walker this component drives; null until <see cref="Initialize"/> is called.</summary>
        public LaneWalker Walker => _walker;

        /// <summary>Binds the walker and the lane's world Y, and moves the GameObject to the start.</summary>
        /// <param name="walker">Movement logic to drive.</param>
        /// <param name="laneY">World Y of the walker's lane centre; stays fixed for the walker's life.</param>
        /// <example><code>behaviour.Initialize(walker, laneY);</code></example>
        public void Initialize(LaneWalker walker, float laneY)
        {
            _walker = walker;
            _laneY = laneY;
            ApplyPosition();
        }

        private void Update()
        {
            // The component can be added from the Inspector without a walker; it then stays idle.
            if (_walker == null)
            {
                return;
            }

            _walker.Tick(Time.deltaTime);
            ApplyPosition();
        }

        private void ApplyPosition()
        {
            transform.position = new Vector3(_walker.X, _laneY, transform.position.z);
        }
    }
}
