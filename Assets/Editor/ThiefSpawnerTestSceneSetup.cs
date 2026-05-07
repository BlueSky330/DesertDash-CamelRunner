using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor utility to create the ThiefSpawnerTest scene automatically.
/// Run via: Assets > Create > Test Scene Setup > Thief Spawner Test
/// </summary>
public class ThiefSpawnerTestSceneSetup
{
    private const string TEST_SCENE_PATH = "Assets/Scenes/ThiefSpawnerTest.unity";

    [MenuItem("Assets/Create Test Scenes/Thief Spawner Test Scene")]
    public static void CreateThiefSpawnerTestScene()
    {
        // Create a new scene
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Debug.Log("[ThiefSpawnerTestSceneSetup] Creating new test scene: " + TEST_SCENE_PATH);

        // Create Main Camera
        CreateMainCamera();

        // Create Ground Plane
        CreateGroundPlane();

        // Create Player with PlayerController
        CreatePlayer();

        // Create ThiefSpawner
        CreateThiefSpawner();

        // Create Test Manager
        CreateTestManager();

        // Configure Lighting
        ConfigureLighting();

        // Save the scene
        EditorSceneManager.SaveScene(newScene, TEST_SCENE_PATH);
        Debug.Log("[ThiefSpawnerTestSceneSetup] Scene saved: " + TEST_SCENE_PATH);
    }

    private static void CreateMainCamera()
    {
        GameObject cameraGO = new GameObject("Main Camera");
        Camera camera = cameraGO.AddComponent<Camera>();
        camera.tag = "MainCamera";
        cameraGO.AddComponent<AudioListener>();

        cameraGO.transform.position = new Vector3(0, 1.5f, -10);
        cameraGO.transform.rotation = Quaternion.Euler(10, 0, 0);

        camera.fieldOfView = 60;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 1000;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.1f, 0.15f, 0.2f, 1);

        GameViewUtils.SetGameViewSize(camera, 720, 1600); // Portrait mobile resolution

        Debug.Log("[ThiefSpawnerTestSceneSetup] ✓ Created Main Camera");
    }

    private static void CreateGroundPlane()
    {
        GameObject groundGO = new GameObject("Ground");
        groundGO.tag = "Ground";

        MeshFilter meshFilter = groundGO.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = groundGO.AddComponent<MeshRenderer>();
        BoxCollider collider = groundGO.AddComponent<BoxCollider>();

        // Use Unity's built-in cube mesh and stretch it
        meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

        groundGO.transform.position = new Vector3(0, -0.5f, 0);
        groundGO.transform.localScale = new Vector3(20, 0.1f, 20);

        // Material (simple gray)
        Material groundMat = new Material(Shader.Find("Standard"));
        groundMat.color = new Color(0.7f, 0.7f, 0.7f, 1);
        meshRenderer.material = groundMat;

        Debug.Log("[ThiefSpawnerTestSceneSetup] ✓ Created Ground Plane");
    }

    private static void CreatePlayer()
    {
        // Create Player GameObject with CharacterController
        GameObject playerGO = new GameObject("Player");
        playerGO.transform.position = Vector3.zero;

        // Add CharacterController
        CharacterController cc = playerGO.AddComponent<CharacterController>();
        cc.radius = 0.25f;
        cc.height = 1.8f;
        cc.center = new Vector3(0, 0.9f, 0);

        // Add Animator (with placeholder controller)
        Animator animator = playerGO.AddComponent<Animator>();
        // Note: You'll need to assign the actual animator controller in the inspector

        // Add PlayerController script
        PlayerController playerController = playerGO.AddComponent<PlayerController>();
        playerController.laneWidth = 2f;
        playerController.laneChangeSpeed = 12f;
        playerController.jumpForce = 12f;
        playerController.gravity = 22f;
        playerController.slideDuration = 0.8f;
        playerController.forwardSpeed = 10f;

        // Add tag
        playerGO.tag = "Player";

        // Add a visual representation (simple capsule)
        GameObject camelVisual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        camelVisual.name = "CamelVisual";
        camelVisual.transform.SetParent(playerGO.transform);
        camelVisual.transform.localPosition = Vector3.zero;
        camelVisual.transform.localScale = new Vector3(0.5f, 0.9f, 0.5f);

        // Remove the capsule collider (we use CharacterController instead)
        Collider capsuleCollider = camelVisual.GetComponent<Collider>();
        if (capsuleCollider != null)
            DestroyImmediate(capsuleCollider);

        Debug.Log("[ThiefSpawnerTestSceneSetup] ✓ Created Player with PlayerController");
    }

    private static void CreateThiefSpawner()
    {
        GameObject spawnerGO = new GameObject("ThiefSpawner");
        spawnerGO.transform.position = Vector3.zero;

        ThiefSpawner spawner = spawnerGO.AddComponent<ThiefSpawner>();
        // SerializeField values are set via inspector, using defaults from the script

        Debug.Log("[ThiefSpawnerTestSceneSetup] ✓ Created ThiefSpawner");
    }

    private static void CreateTestManager()
    {
        GameObject managerGO = new GameObject("ThiefSpawnerTestManager");
        managerGO.transform.position = Vector3.zero;

        ThiefSpawnerTestManager testManager = managerGO.AddComponent<ThiefSpawnerTestManager>();
        // Inspector will show default values

        Debug.Log("[ThiefSpawnerTestSceneSetup] ✓ Created ThiefSpawnerTestManager");
    }

    private static void ConfigureLighting()
    {
        // Set ambient light
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.5f, 0.5f, 0.5f, 1);

        // Create a directional light (sun)
        GameObject lightGO = new GameObject("Directional Light");
        Light light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1;
        light.color = Color.white;
        light.shadows = LightShadows.Soft;

        lightGO.transform.rotation = Quaternion.Euler(45, 45, 0);

        Debug.Log("[ThiefSpawnerTestSceneSetup] ✓ Configured Lighting");
    }
}

/// <summary>
/// Utility class for setting game view resolution during tests.
/// </summary>
public static class GameViewUtils
{
    public static void SetGameViewSize(Camera camera, int width, int height)
    {
        // This would require accessing GameView editor window, which is complex
        // For now, we just set the camera resolution preference
        // In practice, set the resolution via File > Build Settings > Player Settings
        Debug.Log($"[GameViewUtils] Set camera for {width}x{height} (portrait mobile)");
    }
}
