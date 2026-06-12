using UnityEditor;
using UnityEditor.SceneManagement;
using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PrototypeSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string StarterPlayerPrefabPath = "Assets/StarterAssets/ThirdPersonController/Prefabs/PlayerArmature.prefab";
    private static readonly Vector3 PlayerStartPosition = new Vector3(0f, 0.08f, -2.2f);

    [MenuItem("Tools/Programming Assignment/Build 3D Jump Map")]
    public static void BuildScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Material ground = MakeMaterial("MAT_Ground", new Color(0.42f, 0.52f, 0.42f));
        Material platform = MakeMaterial("MAT_Platform", new Color(0.28f, 0.32f, 0.38f));
        Material accent = MakeMaterial("MAT_Accent", new Color(0.86f, 0.64f, 0.24f));
        Material player = MakeMaterial("MAT_Player", new Color(0.18f, 0.48f, 0.88f));
        Material hazard = MakeMaterial("MAT_Hazard", new Color(0.72f, 0.18f, 0.16f));
        Material goal = MakeMaterial("MAT_Goal", new Color(0.22f, 0.72f, 0.38f));

        CreateGameManager();
        CreateLight();
        Camera camera = CreateCamera();
        GameObject playerObject = CreatePlayer(player);
        camera.GetComponent<SmoothFollowCamera>().SetTarget(playerObject.transform);

        CreatePlatform("Start Platform", new Vector3(0f, -0.25f, 0f), new Vector3(8f, 0.5f, 8f), ground);

        // Section 1: readable warm-up jumps.
        CreatePlatform("Warmup 1", new Vector3(0f, 0.25f, 6.5f), new Vector3(4.8f, 0.45f, 3.6f), platform);
        CreatePlatform("Warmup 2", new Vector3(2.4f, 0.65f, 11.8f), new Vector3(4f, 0.45f, 3.4f), platform);
        CreatePlatform("Warmup 3", new Vector3(-2.2f, 1.05f, 17.1f), new Vector3(4f, 0.45f, 3.4f), platform);
        CreatePlatform("Checkpoint Island", new Vector3(0f, 1.45f, 22.6f), new Vector3(7f, 0.45f, 5f), ground);

        // Section 2: a wide platform with an obstacle, then short stepping stones.
        CreatePlatform("Obstacle Island", new Vector3(0f, 1.85f, 30f), new Vector3(7f, 0.45f, 6f), ground);
        CreatePlatform("Stone Step 1", new Vector3(2.5f, 2.25f, 36f), new Vector3(3.2f, 0.45f, 3f), platform);
        CreatePlatform("Stone Step 2", new Vector3(-2f, 2.65f, 41f), new Vector3(3.2f, 0.45f, 3f), platform);
        CreatePlatform("Stone Step 3", new Vector3(1.6f, 3.05f, 46f), new Vector3(3.4f, 0.45f, 3f), platform);
        CreatePlatform("Rest Island", new Vector3(0f, 3.45f, 52f), new Vector3(7f, 0.45f, 5f), ground);

        // Section 3: moving platform and final bridge. Gaps stay small enough to sprint-jump.
        CreateMovingPlatform("Moving Platform", new Vector3(0f, 4.05f, 59f), new Vector3(4.2f, 0.42f, 3.4f), new Vector3(4f, 0f, 0f), platform);
        CreatePlatform("Landing Island", new Vector3(1.8f, 4.65f, 66f), new Vector3(6.4f, 0.45f, 5f), ground);
        CreatePlatform("Final Bridge", new Vector3(1.8f, 5.05f, 74f), new Vector3(2.1f, 0.35f, 10f), platform);
        CreatePlatform("Final Platform", new Vector3(1.8f, 5.55f, 84f), new Vector3(7f, 0.45f, 7f), goal);

        CreatePlatform("Start Guard Left", new Vector3(-4.25f, 0.35f, 0f), new Vector3(0.35f, 1.2f, 8f), platform);
        CreatePlatform("Start Guard Right", new Vector3(4.25f, 0.35f, 0f), new Vector3(0.35f, 1.2f, 8f), platform);
        CreateDeathZone("Death Zone", new Vector3(0f, -0.8f, 42f), new Vector3(24f, 0.15f, 100f), hazard);
        CreateRotatingHazard("Island Spinner", new Vector3(0f, 2.3f, 30f), new Vector3(4.2f, 0.12f, 0.22f), hazard, 70f);
        CreateRotatingHazard("Bridge Spinner", new Vector3(1.8f, 5.45f, 74f), new Vector3(3.3f, 0.12f, 0.22f), hazard, -65f);
        CreateGoalZone("Goal Zone", new Vector3(1.8f, 5.82f, 84f), new Vector3(4.4f, 0.18f, 4.4f), goal);

        CreateCoin("Coin 1", new Vector3(0f, 1.25f, 6.5f), accent);
        CreateCoin("Coin 2", new Vector3(2.4f, 1.65f, 11.8f), accent);
        CreateCoin("Coin 3", new Vector3(-2.2f, 2.05f, 17.1f), accent);
        CreateCoin("Checkpoint Coin", new Vector3(0f, 2.45f, 22.6f), accent);
        CreateCoin("Step Coin 1", new Vector3(2.5f, 3.25f, 36f), accent);
        CreateCoin("Step Coin 2", new Vector3(-2f, 3.65f, 41f), accent);
        CreateCoin("Step Coin 3", new Vector3(1.6f, 4.05f, 46f), accent);
        CreateCoin("Rest Coin", new Vector3(0f, 4.45f, 52f), accent);
        CreateCoin("Moving Coin", new Vector3(0f, 5.05f, 59f), accent);
        CreateCoin("Landing Coin", new Vector3(1.8f, 5.65f, 66f), accent);
        CreateCoin("Goal Coin", new Vector3(1.8f, 6.95f, 80.5f), accent, 1.6f);

        RenderSettings.ambientIntensity = 1.1f;
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
    }

    private static Material MakeMaterial(string name, Color color)
    {
        const string folder = "Assets/Materials";
        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        string path = $"{folder}/{name}.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void CreateLight()
    {
        GameObject lightObject = new GameObject("Sun Light");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 2.2f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    private static void CreateGameManager()
    {
        GameObject managerObject = new GameObject("Jump Map Game Manager");
        managerObject.AddComponent<JumpMapGameManager>();
    }

    private static Camera CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 6f, -8f);
        cameraObject.transform.rotation = Quaternion.Euler(35f, 0f, 0f);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.fieldOfView = 58f;
        camera.nearClipPlane = 0.05f;
        cameraObject.AddComponent<AudioListener>();
        cameraObject.AddComponent<SmoothFollowCamera>();
        return camera;
    }

    private static GameObject CreatePlayer(Material material)
    {
        GameObject starterPlayer = CreateStarterAssetsPlayer();
        if (starterPlayer != null)
        {
            return starterPlayer;
        }

        GameObject playerObject = new GameObject("Player");
        playerObject.transform.position = new Vector3(0f, 1.1f, PlayerStartPosition.z);

        CharacterController controller = playerObject.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.35f;
        controller.center = new Vector3(0f, 0.9f, 0f);
        controller.stepOffset = 0.35f;
        controller.slopeLimit = 50f;

        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        visual.name = "PlayerVisual";
        visual.transform.SetParent(playerObject.transform);
        visual.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        visual.transform.localRotation = Quaternion.identity;
        visual.transform.localScale = new Vector3(0.72f, 0.9f, 0.72f);
        Object.DestroyImmediate(visual.GetComponent<Collider>());
        visual.GetComponent<MeshRenderer>().sharedMaterial = material;

        GameObject nose = GameObject.CreatePrimitive(PrimitiveType.Cube);
        nose.name = "ForwardMarker";
        nose.transform.SetParent(visual.transform);
        nose.transform.localPosition = new Vector3(0f, 0.22f, 0.62f);
        nose.transform.localScale = new Vector3(0.18f, 0.18f, 0.28f);
        Object.DestroyImmediate(nose.GetComponent<Collider>());
        nose.GetComponent<MeshRenderer>().sharedMaterial = material;

        GameObject groundCheck = new GameObject("GroundCheck");
        groundCheck.transform.SetParent(playerObject.transform);
        groundCheck.transform.localPosition = new Vector3(0f, 0.12f, 0f);

        SmoothPlayerController mover = playerObject.AddComponent<SmoothPlayerController>();
        SerializedObject serializedMover = new SerializedObject(mover);
        serializedMover.FindProperty("groundCheck").objectReferenceValue = groundCheck.transform;
        serializedMover.FindProperty("visualRoot").objectReferenceValue = visual.transform;
        serializedMover.ApplyModifiedPropertiesWithoutUndo();

        playerObject.AddComponent<FallRespawn>();
        return playerObject;
    }

    private static GameObject CreateStarterAssetsPlayer()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(StarterPlayerPrefabPath);
        if (prefab == null)
        {
            return null;
        }

        GameObject playerObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        playerObject.name = "Player";
        playerObject.transform.SetPositionAndRotation(PlayerStartPosition, Quaternion.identity);

        ThirdPersonController controller = playerObject.GetComponent<ThirdPersonController>();
        if (controller != null)
        {
            controller.MoveSpeed = 3.2f;
            controller.SprintSpeed = 6.4f;
            controller.SpeedChangeRate = 12f;
            controller.JumpHeight = 1.55f;
            controller.Gravity = -18f;
            EditorUtility.SetDirty(controller);
        }

        if (playerObject.GetComponent<FallRespawn>() == null)
        {
            playerObject.AddComponent<FallRespawn>();
        }

        return playerObject;
    }

    private static void CreatePlatform(string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject platformObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platformObject.name = name;
        platformObject.transform.position = position;
        platformObject.transform.localScale = scale;
        platformObject.GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    private static void CreateDeathZone(string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject deathZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        deathZone.name = name;
        deathZone.transform.position = position;
        deathZone.transform.localScale = scale;
        deathZone.GetComponent<MeshRenderer>().sharedMaterial = material;

        Collider collider = deathZone.GetComponent<Collider>();
        collider.isTrigger = true;
        deathZone.AddComponent<DeathZone>();
    }

    private static void CreateMovingPlatform(string name, Vector3 position, Vector3 scale, Vector3 moveOffset, Material material)
    {
        GameObject platformObject = CreatePlatformObject(name, position, scale, material);
        MovingPlatform movingPlatform = platformObject.AddComponent<MovingPlatform>();
        SerializedObject serializedPlatform = new SerializedObject(movingPlatform);
        serializedPlatform.FindProperty("localMoveOffset").vector3Value = moveOffset;
        serializedPlatform.FindProperty("moveSpeed").floatValue = 1.35f;
        serializedPlatform.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateRotatingHazard(string name, Vector3 position, Vector3 scale, Material material, float speed = 100f)
    {
        GameObject hazardObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hazardObject.name = name;
        hazardObject.transform.position = position;
        hazardObject.transform.localScale = scale;
        hazardObject.GetComponent<MeshRenderer>().sharedMaterial = material;

        Collider collider = hazardObject.GetComponent<Collider>();
        collider.isTrigger = true;

        RotatingHazard rotatingHazard = hazardObject.AddComponent<RotatingHazard>();
        SerializedObject serializedHazard = new SerializedObject(rotatingHazard);
        serializedHazard.FindProperty("rotationSpeed").floatValue = speed;
        serializedHazard.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateGoalZone(string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pad.name = "Goal Pad";
        pad.transform.position = position;
        pad.transform.localScale = scale;
        pad.GetComponent<MeshRenderer>().sharedMaterial = material;
        Object.DestroyImmediate(pad.GetComponent<Collider>());

        GameObject goalObject = new GameObject(name);
        goalObject.transform.position = position + Vector3.up * 0.9f;
        BoxCollider collider = goalObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(scale.x, 1.8f, scale.z);
        goalObject.AddComponent<GoalZone>();

        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        marker.name = "Goal Marker";
        marker.transform.position = position + Vector3.up * 0.18f;
        marker.transform.localScale = new Vector3(1.2f, 0.08f, 1.2f);
        marker.GetComponent<MeshRenderer>().sharedMaterial = material;
        Object.DestroyImmediate(marker.GetComponent<Collider>());
    }

    private static void CreateCoin(string name, Vector3 position, Material material, float scale = 1f, bool isGoalCoin = false)
    {
        GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        coin.name = name;
        coin.transform.position = position;
        coin.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        coin.transform.localScale = new Vector3(0.45f * scale, 0.08f * scale, 0.45f * scale);
        Collider collider = coin.GetComponent<Collider>();
        collider.isTrigger = true;
        coin.GetComponent<MeshRenderer>().sharedMaterial = material;
        coin.AddComponent<RotatingCoin>();
        CoinPickup pickup = coin.AddComponent<CoinPickup>();
        if (isGoalCoin)
        {
            SerializedObject serializedPickup = new SerializedObject(pickup);
            serializedPickup.FindProperty("isGoalCoin").boolValue = true;
            serializedPickup.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    private static GameObject CreatePlatformObject(string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject platformObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platformObject.name = name;
        platformObject.transform.position = position;
        platformObject.transform.localScale = scale;
        platformObject.GetComponent<MeshRenderer>().sharedMaterial = material;
        return platformObject;
    }
}
