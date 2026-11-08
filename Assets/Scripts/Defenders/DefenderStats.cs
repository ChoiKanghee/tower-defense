using System;
using UnityEngine;

namespace TowerDefense.Defenders
{
    /// <summary>
    /// The three defender kinds of story 002 (design/game-brief.md, MVP feature 2).
    /// Vietnamese names: Sentry = tu ve, Barricade = chien luy, Bomb = bom ba cang.
    /// </summary>
    public enum DefenderType
    {
        /// <summary>Tu ve: shoots the nearest enemy in front of it on its own lane.</summary>
        Sentry,

        /// <summary>Chien luy: never shoots; blocks and soaks damage until destroyed.</summary>
        Barricade,

        /// <summary>Bom ba cang: one-shot mine that explodes when an enemy enters its blast area.</summary>
        Bomb,
    }

    /// <summary>
    /// Tunable numbers of one defender type. Field names match the JSON keys in
    /// <c>defenders.json</c>. Fields a type does not use are ignored by that type's logic.
    /// </summary>
    /// <example>
    /// <code>DefenderStats sentry = DefenderConfig.Load().Get(DefenderType.Sentry);</code>
    /// </example>
    [Serializable]
    public sealed class DefenderStats
    {
        /// <summary>Hit points. Must be positive. The defender is removed when it reaches 0.</summary>
        public int Health;

        /// <summary>Damage per shot (Sentry) or per enemy hit by the blast (Bomb). Unused by Barricade.</summary>
        public int Damage;

        /// <summary>Seconds between Sentry shots. Must be positive for Sentry; unused otherwise.</summary>
        public float FireInterval;

        /// <summary>Sentry shooting range, in cells in front of it. Must be positive for Sentry; unused otherwise.</summary>
        public float Range;

        /// <summary>Bomb blast radius, in cells (1 = 3x3 area around the bomb). Must be at least 1 for Bomb; unused otherwise.</summary>
        public int BlastRadius;
    }

    /// <summary>
    /// Data-driven settings for defenders (and, for now, the enemy starting health), read from
    /// <c>Assets/Resources/Config/defenders.json</c>.
    /// </summary>
    /// <remarks>
    /// <see cref="EnemyMaxHealth"/> lives here only because enemies need minimal health to be
    /// damageable in story 002; per-enemy-type stats are story 003, which should move it.
    /// </remarks>
    /// <example>
    /// <code>
    /// DefenderConfig config = DefenderConfig.Load();
    /// DefenderStats bomb = config.Get(DefenderType.Bomb);
    /// </code>
    /// </example>
    [Serializable]
    public sealed class DefenderConfig
    {
        /// <summary>Starting hit points of every enemy. Must be positive.</summary>
        public int EnemyMaxHealth;

        /// <summary>Stats of <see cref="DefenderType.Sentry"/>.</summary>
        public DefenderStats Sentry;

        /// <summary>Stats of <see cref="DefenderType.Barricade"/>.</summary>
        public DefenderStats Barricade;

        /// <summary>Stats of <see cref="DefenderType.Bomb"/>.</summary>
        public DefenderStats Bomb;

        /// <summary>
        /// Loads the config from a JSON <see cref="TextAsset"/> under a <c>Resources</c> folder,
        /// parses it with <see cref="JsonUtility"/> and validates it.
        /// </summary>
        /// <param name="resourcePath">Resources-relative path without extension.</param>
        /// <returns>A validated config.</returns>
        /// <exception cref="InvalidOperationException">
        /// The file is missing, empty, lacks a defender section, or holds an invalid value.
        /// </exception>
        /// <example>
        /// <code>DefenderConfig config = DefenderConfig.Load("Config/defenders");</code>
        /// </example>
        public static DefenderConfig Load(string resourcePath = "Config/defenders")
        {
            TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
            if (textAsset == null)
            {
                throw new InvalidOperationException(
                    $"Defender config not found at Resources path '{resourcePath}'.");
            }

            DefenderConfig config = JsonUtility.FromJson<DefenderConfig>(textAsset.text);
            Resources.UnloadAsset(textAsset);
            if (config == null)
            {
                throw new InvalidOperationException(
                    $"Defender config at Resources path '{resourcePath}' is empty.");
            }

            config.Validate(resourcePath);
            return config;
        }

        /// <summary>Gets the stats of a defender type.</summary>
        /// <param name="type">The defender type.</param>
        /// <returns>The stats for that type.</returns>
        /// <example><code>int hp = config.Get(DefenderType.Barricade).Health;</code></example>
        public DefenderStats Get(DefenderType type)
        {
            switch (type)
            {
                case DefenderType.Sentry: return Sentry;
                case DefenderType.Barricade: return Barricade;
                default: return Bomb;
            }
        }

        private void Validate(string source)
        {
            string errors = string.Empty;
            if (EnemyMaxHealth <= 0) errors += $" EnemyMaxHealth must be positive (got {EnemyMaxHealth}).";

            // JsonUtility leaves a missing nested object as a default instance, so a missing
            // section shows up as Health == 0 and is reported by the checks below.
            errors += ValidateCommon(nameof(Sentry), Sentry);
            errors += ValidateCommon(nameof(Barricade), Barricade);
            errors += ValidateCommon(nameof(Bomb), Bomb);

            if (Sentry != null)
            {
                if (Sentry.Damage <= 0) errors += $" Sentry.Damage must be positive (got {Sentry.Damage}).";
                if (!(Sentry.FireInterval > 0f)) errors += $" Sentry.FireInterval must be positive (got {Sentry.FireInterval}).";
                if (!(Sentry.Range > 0f)) errors += $" Sentry.Range must be positive (got {Sentry.Range}).";
            }

            if (Bomb != null)
            {
                if (Bomb.Damage <= 0) errors += $" Bomb.Damage must be positive (got {Bomb.Damage}).";
                if (Bomb.BlastRadius < 1) errors += $" Bomb.BlastRadius must be at least 1 (got {Bomb.BlastRadius}).";
            }

            if (errors.Length > 0)
            {
                throw new InvalidOperationException($"Invalid defender config '{source}':{errors}");
            }
        }

        private static string ValidateCommon(string name, DefenderStats stats)
        {
            if (stats == null) return $" Section '{name}' is missing.";
            if (stats.Health <= 0) return $" {name}.Health must be positive (got {stats.Health}).";
            return string.Empty;
        }
    }
}
