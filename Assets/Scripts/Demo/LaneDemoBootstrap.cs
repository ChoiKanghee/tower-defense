using System.Collections.Generic;
using TowerDefense.Defenders;
using TowerDefense.Enemies;
using TowerDefense.Grid;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TowerDefense.Demo
{
    /// <summary>
    /// TEMPORARY throwaway runtime demo (stories 001 and 002), so the lane grid and defenders are
    /// playable without a scene edit. After the first scene loads, and only in SampleScene, it
    /// frames the main camera on the grid, draws every cell as a tinted square, sends an enemy down
    /// a random lane every few seconds, and lets you place defenders with the mouse.
    /// <list type="bullet">
    /// <item>Pick a type: click one of the three debug buttons (top left) or press 1 / 2 / 3.</item>
    /// <item>Left click a cell to place the picked defender (rejected on an occupied cell).</item>
    /// <item>Right click a defender to damage it by a fixed debug amount (to see a Barricade break).</item>
    /// </list>
    /// The type picker is a debug control only; the real card bar is story 007. Delete this class
    /// once real scene wiring exists.
    /// </summary>
    public sealed class LaneDemoBootstrap : MonoBehaviour
    {
        private const string DEMO_SCENE_NAME = "SampleScene";
        private const float CELL_FILL = 0.95f;
        private const float ENEMY_FILL = 0.6f;
        private const float CAMERA_MARGIN_CELLS = 0.5f;

        // Demo-only tuning (not gameplay config): how often an enemy appears, and the right-click debug hit.
        private const float SPAWN_INTERVAL_SECONDS = 3f;
        private const int DEBUG_CLICK_DAMAGE = 50;

        private static readonly string[] TypeLabels = { "1: Tu ve", "2: Chien luy", "3: Bom ba cang" };

        private Camera _camera;
        private LaneGridConfig _gridConfig;
        private DefenderConfig _defenderConfig;
        private LaneGrid _grid;
        private DefenderPlacement _placement;
        private Sprite _square;
        private Transform _root;
        private readonly List<EnemyHealth> _enemies = new List<EnemyHealth>();
        private DefenderType _selected = DefenderType.Sentry;
        private float _spawnTimer;
        private string _status = "Chon loai don vi, roi bam vao mot o.";
        private Rect _panelRect;

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

            var demoObject = new GameObject("LaneDemo");
            demoObject.AddComponent<LaneDemoBootstrap>().Setup(mainCamera);
        }

        private void Setup(Camera mainCamera)
        {
            _camera = mainCamera;
            _gridConfig = LaneGridConfig.Load();
            _defenderConfig = DefenderConfig.Load();
            _grid = new LaneGrid(_gridConfig.Lanes, _gridConfig.Columns, _gridConfig.CellSize, Vector2.zero);
            _placement = new DefenderPlacement(_grid, _defenderConfig);
            _root = transform;
            _panelRect = new Rect(10f, 10f, 380f, 110f);

            // Frame the camera so the whole grid fits whatever the aspect ratio.
            _camera.orthographic = true;
            _camera.transform.position = new Vector3(_grid.Center.x, _grid.Center.y, _camera.transform.position.z);
            _camera.orthographicSize =
                Mathf.Max(_grid.Height * 0.5f, _grid.Width * 0.5f / _camera.aspect)
                + CAMERA_MARGIN_CELLS * _grid.CellSize;

            // A 1x1 world-unit white square; SpriteRenderer.color tints it.
            Texture2D texture = Texture2D.whiteTexture;
            _square = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                texture.width);

            var cellColorA = new Color(0.36f, 0.62f, 0.32f);
            var cellColorB = new Color(0.30f, 0.54f, 0.27f);
            float cellScale = _grid.CellSize * CELL_FILL;

            for (int lane = 0; lane < _grid.Lanes; lane++)
            {
                for (int column = 0; column < _grid.Columns; column++)
                {
                    var cell = new CellId(lane, column);
                    _grid.TryCellToWorld(cell, out Vector2 cellCenter);

                    var cellObject = new GameObject($"Cell {cell}");
                    cellObject.transform.SetParent(_root, false);
                    cellObject.transform.localPosition = cellCenter;
                    cellObject.transform.localScale = new Vector3(cellScale, cellScale, 1f);

                    var cellRenderer = cellObject.AddComponent<SpriteRenderer>();
                    cellRenderer.sprite = _square;
                    cellRenderer.color = (lane + column) % 2 == 0 ? cellColorA : cellColorB;
                    cellRenderer.sortingOrder = 0;
                }
            }
        }

        private void Update()
        {
            if (_grid == null)
            {
                return;
            }

            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f)
            {
                _spawnTimer = SPAWN_INTERVAL_SECONDS;
                SpawnEnemy();
            }

            HandleKeys();
            HandleMouse();
        }

        private void HandleKeys()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.digit1Key.wasPressedThisFrame) _selected = DefenderType.Sentry;
            if (keyboard.digit2Key.wasPressedThisFrame) _selected = DefenderType.Barricade;
            if (keyboard.digit3Key.wasPressedThisFrame) _selected = DefenderType.Bomb;
        }

        private void HandleMouse()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            bool left = mouse.leftButton.wasPressedThisFrame;
            bool right = mouse.rightButton.wasPressedThisFrame;
            if (!left && !right)
            {
                return;
            }

            Vector2 screen = mouse.position.ReadValue();

            // The IMGUI panel's Y runs top-down while the mouse Y runs bottom-up.
            if (_panelRect.Contains(new Vector2(screen.x, Screen.height - screen.y)))
            {
                return;
            }

            Vector3 world = _camera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, 0f));
            if (!_grid.TryWorldToCell(world, out CellId cell))
            {
                _status = "Bam ra ngoai luoi.";
                return;
            }

            if (left)
            {
                PlaceDefender(cell);
            }
            else if (_placement.TryGetDefender(cell, out Defender target))
            {
                target.TakeDamage(DEBUG_CLICK_DAMAGE);
            }
        }

        private void PlaceDefender(CellId cell)
        {
            PlacementResult result = _placement.TryPlace(_selected, cell, out Defender defender);
            if (result != PlacementResult.Placed)
            {
                _status = result == PlacementResult.CellOccupied
                    ? $"Bi tu choi: o {cell} da co don vi."
                    : $"Bi tu choi: o {cell} ngoai luoi.";
                return;
            }

            _status = $"Da dat {TypeLabels[(int)_selected]} o {cell}.";

            float fill;
            Color color;
            switch (_selected)
            {
                case DefenderType.Sentry:
                    fill = 0.55f;
                    color = new Color(0.2f, 0.45f, 0.95f);
                    break;
                case DefenderType.Barricade:
                    fill = 0.85f;
                    color = new Color(0.55f, 0.38f, 0.2f);
                    break;
                default:
                    fill = 0.35f;
                    color = new Color(0.12f, 0.12f, 0.12f);
                    break;
            }

            var defenderObject = new GameObject($"Defender {_selected} {cell}");
            defenderObject.transform.SetParent(_root, false);
            float scale = _grid.CellSize * fill;
            defenderObject.transform.localScale = new Vector3(scale, scale, 1f);

            var defenderRenderer = defenderObject.AddComponent<SpriteRenderer>();
            defenderRenderer.sprite = _square;
            defenderRenderer.color = color;
            defenderRenderer.sortingOrder = 1;

            defenderObject.AddComponent<DefenderBehaviour>().Initialize(defender, _enemies, _square, _grid.CellSize);
        }

        private void SpawnEnemy()
        {
            int lane = Random.Range(0, _grid.Lanes);
            _grid.TryGetLaneY(lane, out float laneY);

            var walker = new LaneWalker(lane, _grid.LaneStartX, _grid.LaneEndX, _gridConfig.EnemySpeed);
            var health = new EnemyHealth(walker, _defenderConfig.EnemyMaxHealth);

            var enemyObject = new GameObject($"Enemy (lane {lane})");
            enemyObject.transform.SetParent(_root, false);
            float enemyScale = _grid.CellSize * ENEMY_FILL;
            enemyObject.transform.localScale = new Vector3(enemyScale, enemyScale, 1f);

            var enemyRenderer = enemyObject.AddComponent<SpriteRenderer>();
            enemyRenderer.sprite = _square;
            enemyRenderer.color = new Color(0.85f, 0.2f, 0.2f);
            enemyRenderer.sortingOrder = 2;

            health.OnDied += dead => _enemies.Remove(dead);
            walker.OnReachedEnd += reachedLane =>
            {
                Debug.Log($"LaneDemoBootstrap: enemy reached the end of lane {reachedLane}.");
                _enemies.Remove(health);
                Destroy(enemyObject);
            };

            enemyObject.AddComponent<LaneWalkerBehaviour>().Initialize(walker, laneY, health);
            _enemies.Add(health);
        }

        private void OnGUI()
        {
            if (_grid == null)
            {
                return;
            }

            GUILayout.BeginArea(_panelRect, GUI.skin.box);
            GUILayout.Label("TAM THOI (debug) - the that la story 007");
            _selected = (DefenderType)GUILayout.Toolbar((int)_selected, TypeLabels);
            GUILayout.Label(_status);
            GUILayout.Label("Chuot trai: dat | Chuot phai: gay sat thuong cho don vi");
            GUILayout.EndArea();
        }
    }
}
