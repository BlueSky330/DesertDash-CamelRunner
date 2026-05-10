// AIG-509 / AIG-514: Editor utility — auto-creates all required GameObjects
// and wires components for both scenes.
// Menu: Desert Dash > Setup > [item]
// Run ONCE per scene after opening the project for the first time.

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace DesertDash.Editor
{
    /// <summary>
    /// One-click scene scaffolding wizard.
    /// Reduces manual Unity Editor work for AIG-514.
    /// </summary>
    public static class SceneSetupWizard
    {
        // ── Tags ──────────────────────────────────────────────────────────
        private static readonly string[] RequiredTags =
        {
            GameConstants.LayerGround,
            GameConstants.LayerObstacle,
            GameConstants.LayerCollectible,
            GameConstants.LayerPlayer,
        };

        // ── Menu items ────────────────────────────────────────────────────

        [MenuItem("Desert Dash/Setup/1. Add Required Tags and Layers")]
        public static void AddTagsAndLayers()
        {
            var tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            var tagsProp = tagManager.FindProperty("tags");

            foreach (var tag in RequiredTags)
            {
                bool found = false;
                for (int i = 0; i < tagsProp.arraySize; i++)
                {
                    if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag)
                    { found = true; break; }
                }
                if (!found)
                {
                    tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
                    tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
                }
            }

            tagManager.ApplyModifiedProperties();
            Debug.Log("[DesertDash] Tags added: " + string.Join(", ", RequiredTags));
        }

        [MenuItem("Desert Dash/Setup/2. Create Placeholder Prefabs")]
        public static void CreatePlaceholderPrefabs()
        {
            EnsureFolder("Assets/Prefabs/Obstacles");
            EnsureFolder("Assets/Prefabs/Environment");
            EnsureFolder("Assets/Prefabs/Collectibles");

            // Ground tile
            CreatePrefab(
                "GroundTile",
                "Assets/Prefabs/Environment/GroundTile.prefab",
                new Vector2(GameConstants.TileWidth, 1f),
                Color.yellow,
                GameConstants.LayerGround,
                isObstacle: false,
                isTrigger: false
            );

            // Obstacles
            CreatePrefab("ObstacleRock",   "Assets/Prefabs/Obstacles/ObstacleRock.prefab",   new Vector2(1f, 1f),   new Color(0.6f, 0.4f, 0.2f), GameConstants.LayerObstacle, true,  false);
            CreatePrefab("ObstacleCactus", "Assets/Prefabs/Obstacles/ObstacleCactus.prefab", new Vector2(0.6f, 2f), new Color(0.2f, 0.7f, 0.2f), GameConstants.LayerObstacle, true,  false);
            CreatePrefab("ObstaclePalm",   "Assets/Prefabs/Obstacles/ObstaclePalm.prefab",   new Vector2(0.8f, 3f), new Color(0.4f, 0.6f, 0.1f), GameConstants.LayerObstacle, true,  false);

            // Score collectible (trigger)
            CreatePrefab("ScoreCollectible", "Assets/Prefabs/Collectibles/ScoreCollectible.prefab",
                new Vector2(0.5f, 0.5f), new Color(1f, 0.9f, 0f), GameConstants.LayerCollectible, false, true);

            // Camel player
            CreateCamelPrefab();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[DesertDash] Placeholder prefabs created in Assets/Prefabs/");
        }

        [MenuItem("Desert Dash/Setup/3. Wire Game Scene (Assets/Scenes/Main.unity)")]
        public static void WireGameScene()
        {
            // Open scene non-destructively if not already open
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);

            // ── Camera ────────────────────────────────────────────────────
            var cam = FindOrCreate("Main Camera");
            var camComp = cam.GetOrAdd<Camera>();
            camComp.orthographic = true;
            camComp.orthographicSize = 5f;
            cam.transform.position = new Vector3(0f, 0f, -10f);

            // ── SpeedController ───────────────────────────────────────────
            var sc = FindOrCreate("SpeedController");
            sc.GetOrAdd<SpeedController>();

            // ── ScoreManager ──────────────────────────────────────────────
            var sm = FindOrCreate("ScoreManager");
            sm.GetOrAdd<ScoreManager>();

            // ── GameManager ───────────────────────────────────────────────
            var gm = FindOrCreate("GameManager");
            gm.GetOrAdd<GameManager>();

            // ── CoinManager ───────────────────────────────────────────────
            var cm = FindOrCreate("CoinManager");
            cm.GetOrAdd<CoinManager>();

            // ── AudioManager ──────────────────────────────────────────────
            var am = FindOrCreate("AudioManager");
            am.GetOrAdd<AudioManager>();

            // ── Player ────────────────────────────────────────────────────
            var player = FindOrCreate("Player");
            player.tag = GameConstants.LayerPlayer;
            player.transform.position = new Vector3(GameConstants.PlayerStartX, GameConstants.PlayerGroundY, 0f);

            var sr = player.GetOrAdd<SpriteRenderer>();
            sr.color = new Color(0.8f, 0.6f, 0.3f); // tan camel placeholder
            sr.sprite = CreatePlaceholderSprite(1f, 1.5f);

            var rb = player.GetOrAdd<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.gravityScale = GameConstants.PlayerGravityScale;

            var col = player.GetOrAdd<BoxCollider2D>();
            col.size = new Vector2(0.8f, 1.4f);

            player.GetOrAdd<Animator>(); // Controller assigned manually by dev
            player.GetOrAdd<PlayerController>();

            // ── Ground ────────────────────────────────────────────────────
            var groundPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/GroundTile.prefab");
            var trackGen = FindOrCreate("TrackGenerator");
            var tg = trackGen.GetOrAdd<TrackGenerator>();
            if (groundPrefab != null)
                SetSerializedField(tg, "tilePrefab", groundPrefab);

            // ── Obstacle Spawner ──────────────────────────────────────────
            var obsGo = FindOrCreate("ObstacleSpawner");
            obsGo.GetOrAdd<ObstacleSpawner>();
            // ObstacleData assets assigned manually — remind dev

            // ── Collectible Spawner ───────────────────────────────────────
            var colGo = FindOrCreate("CollectibleSpawner");
            colGo.GetOrAdd<CollectibleSpawner>();

            // ── HUD Canvas ────────────────────────────────────────────────
            var hudCanvas = FindOrCreate("HUD Canvas");
            SetupCanvas(hudCanvas, RenderMode.ScreenSpaceOverlay);
            var hud = hudCanvas.GetOrAdd<GameHUD>();

            var scoreLabel  = CreateTMPLabel(hudCanvas, "ScoreLabel",  new Vector2(0f, -20f),  "0", 24);
            var coinLabel   = CreateTMPLabel(hudCanvas, "CoinLabel",   new Vector2(0f, -60f),  "0", 20);
            SetSerializedField(hud, "scoreLabel",  scoreLabel);
            SetSerializedField(hud, "coinLabel",   coinLabel);

            // ── Game Over Canvas ──────────────────────────────────────────
            var goCanvas = FindOrCreate("GameOver Canvas");
            SetupCanvas(goCanvas, RenderMode.ScreenSpaceOverlay);
            var gameOverUI = goCanvas.GetOrAdd<GameOverUI>();

            var panel = FindOrCreateChild(goCanvas, "Panel");
            panel.GetOrAdd<Image>().color = new Color(0f, 0f, 0f, 0.85f);
            var panelRect = panel.GetOrAdd<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;

            var finalScoreLabel  = CreateTMPLabel(panel, "FinalScoreLabel",  new Vector2(0f, 80f),  "Score: 0",    28);
            var coinsEarnedLabel = CreateTMPLabel(panel, "CoinsEarnedLabel", new Vector2(0f, 40f),  "+0 coins",    22);
            var highScoreLabel   = CreateTMPLabel(panel, "HighScoreLabel",   new Vector2(0f, 0f),   "Best: 0",     20);
            var restartBtn       = CreateButton(panel,   "RestartButton",    new Vector2(0f, -60f), "Restart");
            var menuBtn          = CreateButton(panel,   "MenuButton",       new Vector2(0f, -120f),"Main Menu");

            SetSerializedField(gameOverUI, "panel",            panel);
            SetSerializedField(gameOverUI, "finalScoreLabel",  finalScoreLabel.GetComponent<TMP_Text>());
            SetSerializedField(gameOverUI, "coinsEarnedLabel", coinsEarnedLabel.GetComponent<TMP_Text>());
            SetSerializedField(gameOverUI, "highScoreLabel",   highScoreLabel.GetComponent<TMP_Text>());
            SetSerializedField(gameOverUI, "restartButton",    restartBtn.GetComponent<Button>());
            SetSerializedField(gameOverUI, "mainMenuButton",   menuBtn.GetComponent<Button>());

            panel.SetActive(false); // hidden at start

            // ── Parallax Backgrounds ──────────────────────────────────────
            CreateParallaxLayer("BG_Far",  -5f, GameConstants.ParallaxFarSpeedFactor,  new Color(0.9f, 0.7f, 0.4f));
            CreateParallaxLayer("BG_Mid",  -4f, GameConstants.ParallaxMidSpeedFactor,  new Color(0.85f, 0.6f, 0.3f));
            CreateParallaxLayer("BG_Near", -3f, GameConstants.ParallaxNearSpeedFactor, new Color(0.8f, 0.55f, 0.25f));

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log("[DesertDash] Game scene wired. Assign ObstacleData assets to ObstacleSpawner manually.");
        }

        [MenuItem("Desert Dash/Setup/4. Wire Main Menu Scene (Assets/Scenes/MainMenu.unity)")]
        public static void WireMainMenuScene()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity", OpenSceneMode.Single);

            // ── Bootstrap ─────────────────────────────────────────────────
            var bootstrap = FindOrCreate("Bootstrap");
            bootstrap.GetOrAdd<GameBootstrap>();

            // ── Camera ────────────────────────────────────────────────────
            var cam = FindOrCreate("Main Camera");
            var camComp = cam.GetOrAdd<Camera>();
            camComp.orthographic = true;
            camComp.orthographicSize = 5f;
            cam.transform.position = new Vector3(0f, 0f, -10f);

            // ── GameManager / CoinManager / AudioManager (persistent) ─────
            FindOrCreate("GameManager").GetOrAdd<GameManager>();
            FindOrCreate("CoinManager").GetOrAdd<CoinManager>();
            FindOrCreate("AudioManager").GetOrAdd<AudioManager>();

            // ── Main Menu Canvas ──────────────────────────────────────────
            var canvas = FindOrCreate("Main Menu Canvas");
            SetupCanvas(canvas, RenderMode.ScreenSpaceOverlay);
            var menuUI = canvas.GetOrAdd<MainMenuUI>();

            var coinLabel      = CreateTMPLabel(canvas, "CoinBalance",  new Vector2(0f, 120f), "Coins: 0",    24);
            var highScoreLabel = CreateTMPLabel(canvas, "HighScore",    new Vector2(0f, 80f),  "Best: 0",     20);
            var playBtn        = CreateButton(canvas,   "PlayButton",   new Vector2(0f, 0f),   "PLAY");
            var adBtn          = CreateButton(canvas,   "RewardedAd",   new Vector2(0f, -70f), "Watch Ad (+150 coins)");
            adBtn.GetComponent<Button>().interactable = false;

            SetSerializedField(menuUI, "playButton",        playBtn.GetComponent<Button>());
            SetSerializedField(menuUI, "rewardedAdButton",  adBtn.GetComponent<Button>());
            SetSerializedField(menuUI, "coinBalanceLabel",  coinLabel.GetComponent<TMP_Text>());
            SetSerializedField(menuUI, "highScoreLabel",    highScoreLabel.GetComponent<TMP_Text>());

            // ── Parallax Background ───────────────────────────────────────
            CreateParallaxLayer("BG_Far",  -5f, GameConstants.ParallaxFarSpeedFactor,  new Color(0.9f, 0.7f, 0.4f));

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log("[DesertDash] Main Menu scene wired.");
        }

        [MenuItem("Desert Dash/Setup/5. Add Both Scenes to Build Settings")]
        public static void AddScenesToBuildSettings()
        {
            var scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Main.unity",     true),
            };
            EditorBuildSettings.scenes = scenes;
            Debug.Log("[DesertDash] Build Settings updated: MainMenu(0), Main(1).");
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private static GameObject FindOrCreate(string name)
        {
            var existing = GameObject.Find(name);
            return existing != null ? existing : new GameObject(name);
        }

        private static GameObject FindOrCreateChild(GameObject parent, string childName)
        {
            var t = parent.transform.Find(childName);
            if (t != null) return t.gameObject;
            var child = new GameObject(childName);
            child.transform.SetParent(parent.transform, false);
            return child;
        }

        private static void SetupCanvas(GameObject go, RenderMode mode)
        {
            var canvas = go.GetOrAdd<Canvas>();
            canvas.renderMode = mode;
            go.GetOrAdd<CanvasScaler>();
            go.GetOrAdd<GraphicRaycaster>();
        }

        private static GameObject CreateTMPLabel(GameObject parent, string name, Vector2 anchoredPos, string defaultText, float fontSize)
        {
            var go = FindOrCreateChild(parent, name);
            var tmp = go.GetOrAdd<TextMeshProUGUI>();
            tmp.text = defaultText;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;

            var rect = go.GetOrAdd<RectTransform>();
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = new Vector2(400f, 50f);
            return go;
        }

        private static GameObject CreateButton(GameObject parent, string name, Vector2 anchoredPos, string label)
        {
            var go = FindOrCreateChild(parent, name);
            var img = go.GetOrAdd<Image>();
            img.color = new Color(0.2f, 0.6f, 1f);
            go.GetOrAdd<Button>();

            var rect = go.GetOrAdd<RectTransform>();
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = new Vector2(280f, 60f);

            // Label child
            var labelGo = FindOrCreateChild(go, "Label");
            var tmp = labelGo.GetOrAdd<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 20f;
            tmp.alignment = TextAlignmentOptions.Center;
            var labelRect = labelGo.GetOrAdd<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;

            return go;
        }

        private static void CreateParallaxLayer(string name, float z, float speedFactor, Color color)
        {
            var go = FindOrCreate(name);
            go.transform.position = new Vector3(0f, 0f, z);
            var sr = go.GetOrAdd<SpriteRenderer>();
            sr.color = color;
            sr.sprite = CreatePlaceholderSprite(25f, 10f);
            sr.sortingOrder = (int)(z * -1);
            var px = go.GetOrAdd<ParallaxBackground>();
            SetSerializedField(px, "speedFactor", speedFactor);
        }

        private static void CreatePrefab(string name, string path, Vector2 size, Color color,
            string tag, bool isObstacle, bool isTrigger)
        {
            var go = new GameObject(name);
            go.tag = tag;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.color = color;
            sr.sprite = CreatePlaceholderSprite(size.x, size.y);

            var col2d = go.AddComponent<BoxCollider2D>();
            col2d.size = size;
            col2d.isTrigger = isTrigger;

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void CreateCamelPrefab()
        {
            var go = new GameObject("CamelPlayer");
            go.tag = GameConstants.LayerPlayer;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.8f, 0.6f, 0.3f);
            sr.sprite = CreatePlaceholderSprite(1f, 1.5f);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.gravityScale = GameConstants.PlayerGravityScale;

            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.8f, 1.4f);

            go.AddComponent<Animator>();
            go.AddComponent<PlayerController>();

            EnsureFolder("Assets/Prefabs/Player");
            PrefabUtility.SaveAsPrefabAsset(go, "Assets/Prefabs/Player/CamelPlayer.prefab");
            Object.DestroyImmediate(go);
        }

        private static Sprite CreatePlaceholderSprite(float width, float height)
        {
            int w = Mathf.Max(1, (int)(width  * 32));
            int h = Mathf.Max(1, (int)(height * 32));
            var tex = new Texture2D(w, h);
            var pixels = new Color[w * h];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 32f);
        }

        private static void SetSerializedField(Object target, string fieldName, object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop == null) return;

            switch (value)
            {
                case Object o:   prop.objectReferenceValue = o; break;
                case float f:    prop.floatValue  = f;          break;
                case int i:      prop.intValue    = i;          break;
                case bool b:     prop.boolValue   = b;          break;
                case string s:   prop.stringValue = s;          break;
            }
            so.ApplyModifiedProperties();
        }

        private static void EnsureFolder(string path)
        {
            var parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }

    /// <summary>Extension to avoid repetitive GetComponent + AddComponent patterns.</summary>
    internal static class GameObjectExtensions
    {
        public static T GetOrAdd<T>(this GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            return c != null ? c : go.AddComponent<T>();
        }
    }
}
#endif
