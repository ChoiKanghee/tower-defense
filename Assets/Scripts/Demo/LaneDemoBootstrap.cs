using TowerDefense.Enemies;
using TowerDefense.Grid;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense.Demo
{
    /// <summary>
    /// Throwaway runtime demo for story 001, so the lane grid is visible without a scene edit.
    /// After the first scene loads, and only in SampleScene, it frames the main camera on the
    /// grid, draws every cell as a tinted square, and sends one walker down lane 2. The walker
    /// is removed when it reaches the end. Delete this class once real scene wiring exists.
    /// </summary>
    public static class LaneDemoBootstrap
    {
        private const string DEMO_SCENE_NAME = "SampleScene";
        private const int DEMO_LANE = 2;
        private const float CELL_FILL = 0.95f;
        private const float ENEMY_FILL = 0.6f;
        private const float CAMERA_MARGIN_CELLS = 0.5f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Run()
        {
            if (SceneManager.GetActiveScene().name != DEMO_SCENE_NAME)
            {
                return;
            }

            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("LaneDemoBootstrap: no camera tagged MainCamera; demo skipped.");
                return;
            }

            LaneGridConfig config = LaneGridConfig.Load();
            var grid = new LaneGrid(config.Lanes, config.Columns, config.CellSize, Vector2.zero);
            if (!grid.TryGetLaneY(DEMO_LANE, out float laneY))
            {
                Debug.LogError($"LaneDemoBootstrap: demo lane {DEMO_LANE} is outside a {grid.Lanes}-lane grid.");
                return;
            }

            // Frame the camera so the whole grid fits whatever the aspect ratio.
            mainCamera.orthographic = true;
            mainCamera.transform.position = new Vector3(grid.Center.x, grid.Center.y, mainCamera.transform.position.z);
            mainCamera.orthographicSize =
                Mathf.Max(grid.Height * 0.5f, grid.Width * 0.5f / mainCamera.aspect)
                + CAMERA_MARGIN_CELLS * grid.CellSize;

            // A 1x1 world-unit white square; SpriteRenderer.color tints it.
            Texture2D texture = Texture2D.whiteTexture;
            Sprite square = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                texture.width);

            var root = new GameObject("LaneDemo");
            var cellColorA = new Color(0.36f, 0.62f, 0.32f);
            var cellColorB = new Color(0.30f, 0.54f, 0.27f);
            float cellScale = grid.CellSize * CELL_FILL;

            for (int lane = 0; lane < grid.Lanes; lane++)
            {
                for (int column = 0; column < grid.Columns; column++)
                {
                    var cell = new CellId(lane, column);
                    grid.TryCellToWorld(cell, out Vector2 cellCenter);

                    var cellObject = new GameObject($"Cell {cell}");
                    cellObject.transform.SetParent(root.transform, false);
                    cellObject.transform.localPosition = cellCenter;
                    cellObject.transform.localScale = new Vector3(cellScale, cellScale, 1f);

                    var cellRenderer = cellObject.AddComponent<SpriteRenderer>();
                    cellRenderer.sprite = square;
                    cellRenderer.color = (lane + column) % 2 == 0 ? cellColorA : cellColorB;
                    cellRenderer.sortingOrder = 0;
                }
            }

            var walker = new LaneWalker(DEMO_LANE, grid.LaneStartX, grid.LaneEndX, config.EnemySpeed);
            var enemyObject = new GameObject($"Enemy (lane {DEMO_LANE})");
            enemyObject.transform.SetParent(root.transform, false);
            float enemyScale = grid.CellSize * ENEMY_FILL;
            enemyObject.transform.localScale = new Vector3(enemyScale, enemyScale, 1f);

            var enemyRenderer = enemyObject.AddComponent<SpriteRenderer>();
            enemyRenderer.sprite = square;
            enemyRenderer.color = new Color(0.85f, 0.2f, 0.2f);
            enemyRenderer.sortingOrder = 1;

            walker.OnReachedEnd += lane =>
            {
                Debug.Log($"LaneDemoBootstrap: enemy reached the end of lane {lane}.");
                Object.Destroy(enemyObject);
            };
            enemyObject.AddComponent<LaneWalkerBehaviour>().Initialize(walker, laneY);
        }
    }
}
