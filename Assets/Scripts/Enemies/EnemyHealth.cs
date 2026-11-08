using System;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Hit points of one enemy, tied to the <see cref="LaneWalker"/> that moves it so defenders can
    /// find it (lane and X). Plain C#, no engine dependency. Starting health comes from config
    /// (<c>defenders.json</c> for now); per-enemy-type stats are story 003. Not thread-safe.
    /// </summary>
    /// <example>
    /// <code>
    /// var health = new EnemyHealth(walker, config.EnemyMaxHealth);
    /// health.OnDied += dead => enemies.Remove(dead);
    /// health.TakeDamage(20);
    /// </code>
    /// </example>
    public sealed class EnemyHealth
    {
        /// <summary>Creates a full-health enemy.</summary>
        /// <param name="walker">The walker that moves this enemy.</param>
        /// <param name="maxHealth">Starting (and maximum) hit points; expected positive (validated in config).</param>
        /// <example><code>var health = new EnemyHealth(walker, 100);</code></example>
        public EnemyHealth(LaneWalker walker, int maxHealth)
        {
            Walker = walker;
            MaxHealth = maxHealth;
            Current = maxHealth;
        }

        /// <summary>Raised exactly once, when health reaches 0. The argument is this enemy.</summary>
        /// <example><code>health.OnDied += dead => Debug.Log("Enemy died");</code></example>
        public event Action<EnemyHealth> OnDied;

        /// <summary>The walker that moves this enemy (gives its lane and X).</summary>
        public LaneWalker Walker { get; }

        /// <summary>Starting and maximum hit points.</summary>
        public int MaxHealth { get; }

        /// <summary>Current hit points; never below 0.</summary>
        public int Current { get; private set; }

        /// <summary>True once health has reached 0.</summary>
        public bool IsDead => Current <= 0;

        /// <summary>
        /// Subtracts damage, clamping at 0, and raises <see cref="OnDied"/> if this killed the enemy.
        /// Does nothing for a dead enemy or a non-positive amount.
        /// </summary>
        /// <param name="amount">Hit points to remove.</param>
        /// <example><code>health.TakeDamage(config.Get(DefenderType.Sentry).Damage);</code></example>
        public void TakeDamage(int amount)
        {
            if (IsDead || amount <= 0)
            {
                return;
            }

            Current = Math.Max(0, Current - amount);
            if (IsDead)
            {
                OnDied?.Invoke(this);
            }
        }
    }
}
