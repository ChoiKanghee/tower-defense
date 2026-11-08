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
        private const float MIN_HEALTH_BRIGHTNESS = 0.4f;

        private LaneWalker _walker;
        private EnemyHealth _health;
        private float _laneY;
        private SpriteRenderer _renderer;
        private Color _baseColor;

        /// <summary>The walker this component drives; null until <see cref="Initialize"/> is called.</summary>
        public LaneWalker Walker => _walker;

        /// <summary>
        /// Binds the walker and the lane's world Y, and moves the GameObject to the start. When
        /// <paramref name="health"/> is given, the GameObject is destroyed when the enemy dies and its
        /// sprite darkens as health drops.
        /// </summary>
        /// <param name="walker">Movement logic to drive.</param>
        /// <param name="laneY">World Y of the walker's lane centre; stays fixed for the walker's life.</param>
        /// <param name="health">Optional hit points of this enemy; null for an enemy that cannot be damaged.</param>
        /// <example><code>behaviour.Initialize(walker, laneY, health);</code></example>
        public void Initialize(LaneWalker walker, float laneY, EnemyHealth health = null)
        {
            _walker = walker;
            _laneY = laneY;
            _health = health;
            _renderer = GetComponent<SpriteRenderer>();
            _baseColor = _renderer != null ? _renderer.color : Color.white;
            if (_health != null)
            {
                _health.OnDied += HandleDied;
            }

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

            if (_health != null && _renderer != null && !_health.IsDead)
            {
                float brightness = Mathf.Lerp(MIN_HEALTH_BRIGHTNESS, 1f, (float)_health.Current / _health.MaxHealth);
                _renderer.color = new Color(_baseColor.r * brightness, _baseColor.g * brightness, _baseColor.b * brightness, _baseColor.a);
            }
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.OnDied -= HandleDied;
            }
        }

        private void HandleDied(EnemyHealth health)
        {
            Destroy(gameObject);
        }

        private void ApplyPosition()
        {
            transform.position = new Vector3(_walker.X, _laneY, transform.position.z);
        }
    }
}
