using System;
using UnityEngine;

namespace TowerDefense.Grid
{
    /// <summary>
    /// Data-driven settings for the lane grid and the generic lane walker, read from
    /// <c>Assets/Resources/Config/lane-grid.json</c>. Field names match the JSON keys.
    /// </summary>
    /// <example>
    /// <code>
    /// LaneGridConfig config = LaneGridConfig.Load();
    /// var grid = new LaneGrid(config.Lanes, config.Columns, config.CellSize, Vector2.zero);
    /// </code>
    /// </example>
    [Serializable]
    public sealed class LaneGridConfig
    {
        /// <summary>Number of lanes (rows). Must be positive.</summary>
        public int Lanes;

        /// <summary>Number of columns per lane. Must be positive.</summary>
        public int Columns;

        /// <summary>Side length of one square cell, in world units. Must be positive.</summary>
        public float CellSize;

        /// <summary>Enemy walking speed, in world units per second. Must be positive.</summary>
        public float EnemySpeed;

        /// <summary>
        /// Loads the config from a JSON <see cref="TextAsset"/> under a <c>Resources</c> folder,
        /// parses it with <see cref="JsonUtility"/> and validates it.
        /// </summary>
        /// <param name="resourcePath">Resources-relative path without extension.</param>
        /// <returns>A validated config.</returns>
        /// <exception cref="InvalidOperationException">
        /// The file is missing, empty, or holds a non-positive value.
        /// </exception>
        /// <example>
        /// <code>LaneGridConfig config = LaneGridConfig.Load("Config/lane-grid");</code>
        /// </example>
        public static LaneGridConfig Load(string resourcePath = "Config/lane-grid")
        {
            TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
            if (textAsset == null)
            {
                throw new InvalidOperationException(
                    $"Lane grid config not found at Resources path '{resourcePath}'.");
            }

            LaneGridConfig config = JsonUtility.FromJson<LaneGridConfig>(textAsset.text);
            Resources.UnloadAsset(textAsset);
            if (config == null)
            {
                throw new InvalidOperationException(
                    $"Lane grid config at Resources path '{resourcePath}' is empty.");
            }

            config.Validate(resourcePath);
            return config;
        }

        private void Validate(string source)
        {
            string errors = string.Empty;
            if (Lanes <= 0) errors += $" Lanes must be positive (got {Lanes}).";
            if (Columns <= 0) errors += $" Columns must be positive (got {Columns}).";
            if (!(CellSize > 0f)) errors += $" CellSize must be positive (got {CellSize}).";
            if (!(EnemySpeed > 0f)) errors += $" EnemySpeed must be positive (got {EnemySpeed}).";

            if (errors.Length > 0)
            {
                throw new InvalidOperationException($"Invalid lane grid config '{source}':{errors}");
            }
        }
    }
}
