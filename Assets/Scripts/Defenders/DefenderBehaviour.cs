using System.Collections.Generic;
using TowerDefense.Enemies;
using UnityEngine;

namespace TowerDefense.Defenders
{
    /// <summary>
    /// Thin view for a <see cref="Defender"/>: ticks it every frame, darkens the placeholder sprite as
    /// health drops, draws a short flash line for each shot and a flash square for a bomb blast, and
    /// destroys its GameObject when the defender is removed. All rules live in <see cref="Defender"/>.
    /// Temporary shapes only; no art.
    /// </summary>
    /// <example>
    /// <code>
    /// var view = defenderObject.AddComponent&lt;DefenderBehaviour&gt;();
    /// view.Initialize(defender, enemies, square, grid.CellSize);
    /// </code>
    /// </example>
    public sealed class DefenderBehaviour : MonoBehaviour
    {
        private const float SHOT_FLASH_SECONDS = 0.1f;
        private const float SHOT_FLASH_THICKNESS_CELLS = 0.08f;
        private const float BLAST_FLASH_SECONDS = 0.25f;
        private const float MIN_HEALTH_BRIGHTNESS = 0.4f;

        private Defender _defender;
        private IReadOnlyList<EnemyHealth> _enemies;
        private Sprite _square;
        private float _cellSize;
        private SpriteRenderer _renderer;
        private Color _baseColor;

        /// <summary>The defender this component drives; null until <see cref="Initialize"/> is called.</summary>
        public Defender Defender => _defender;

        /// <summary>Binds the defender and moves the GameObject onto its cell.</summary>
        /// <param name="defender">Logic to drive.</param>
        /// <param name="enemies">Live enemy list shared with the rest of the game; read each frame.</param>
        /// <param name="square">1x1 world-unit placeholder sprite used for flashes.</param>
        /// <param name="cellSize">Cell side length in world units, used to size flashes.</param>
        /// <example><code>view.Initialize(defender, enemies, square, 1f);</code></example>
        public void Initialize(Defender defender, IReadOnlyList<EnemyHealth> enemies, Sprite square, float cellSize)
        {
            _defender = defender;
            _enemies = enemies;
            _square = square;
            _cellSize = cellSize;
            _renderer = GetComponent<SpriteRenderer>();
            _baseColor = _renderer != null ? _renderer.color : Color.white;
            transform.position = new Vector3(defender.WorldPosition.x, defender.WorldPosition.y, transform.position.z);

            defender.OnFired += HandleFired;
            defender.OnExploded += HandleExploded;
            defender.OnRemoved += HandleRemoved;
        }

        private void Update()
        {
            if (_defender == null)
            {
                return;
            }

            _defender.Tick(Time.deltaTime, _enemies);

            if (_renderer != null && !_defender.IsRemoved)
            {
                float fraction = (float)_defender.Health / _defender.Stats.Health;
                float brightness = Mathf.Lerp(MIN_HEALTH_BRIGHTNESS, 1f, fraction);
                _renderer.color = new Color(_baseColor.r * brightness, _baseColor.g * brightness, _baseColor.b * brightness, _baseColor.a);
            }
        }

        private void OnDestroy()
        {
            if (_defender == null)
            {
                return;
            }

            _defender.OnFired -= HandleFired;
            _defender.OnExploded -= HandleExploded;
            _defender.OnRemoved -= HandleRemoved;
        }

        private void HandleFired(Defender defender, EnemyHealth target)
        {
            float startX = defender.WorldPosition.x;
            float length = Mathf.Max(0f, target.Walker.X - startX);
            CreateFlash(
                new Vector2(startX + length * 0.5f, defender.WorldPosition.y),
                new Vector2(length, _cellSize * SHOT_FLASH_THICKNESS_CELLS),
                new Color(1f, 0.95f, 0.4f),
                SHOT_FLASH_SECONDS);
        }

        private void HandleExploded(Defender defender)
        {
            float side = (2 * defender.Stats.BlastRadius + 1) * _cellSize;
            CreateFlash(defender.WorldPosition, new Vector2(side, side), new Color(1f, 0.6f, 0.1f, 0.6f), BLAST_FLASH_SECONDS);
        }

        private void HandleRemoved(Defender defender)
        {
            Destroy(gameObject);
        }

        private void CreateFlash(Vector2 position, Vector2 size, Color color, float lifetime)
        {
            var flash = new GameObject("Flash");
            flash.transform.SetParent(transform.parent, false);
            flash.transform.position = new Vector3(position.x, position.y, transform.position.z);
            flash.transform.localScale = new Vector3(size.x, size.y, 1f);

            var flashRenderer = flash.AddComponent<SpriteRenderer>();
            flashRenderer.sprite = _square;
            flashRenderer.color = color;
            flashRenderer.sortingOrder = 3;
            Destroy(flash, lifetime);
        }
    }
}
