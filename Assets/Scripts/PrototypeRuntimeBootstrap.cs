using UnityEngine;
#if UNITY_EDITOR
using StarterAssets;
using UnityEditor;
#endif

public static class PrototypeRuntimeBootstrap
{
    private const string StarterPlayerPrefabPath = "Assets/StarterAssets/ThirdPersonController/Prefabs/PlayerArmature.prefab";
    private static readonly Vector3 PlayerStartPosition = new Vector3(0f, 0.08f, -2.2f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void BuildIfEmpty()
    {
        if (Object.FindFirstObjectByType<SmoothPlayerController>() != null || HasStarterAssetsPlayer())
        {
            return;
        }

        Material ground = MakeMaterial("Runtime Ground", new Color(0.42f, 0.52f, 0.42f));
        Material platform = MakeMaterial("Runtime Platform", new Color(0.28f, 0.32f, 0.38f));
        Material accent = MakeMaterial("Runtime Accent", new Color(0.86f, 0.64f, 0.24f));
        Material player = MakeMaterial("Runtime Player", new Color(0.18f, 0.48f, 0.88f));
        Material hazard = MakeMaterial("Runtime Hazard", new Color(0.72f, 0.18f, 0.16f));

        EnsureLight();
        GameObject playerObject = CreatePlayer(player);
        Camera camera = EnsureCamera();
        SmoothFollowCamera follow = camera.GetComponent<SmoothFollowCamera>();
        if (follow == null)
        {
            follow = camera.gameObject.AddComponent<SmoothFollowCamera>();
        }
        follow.SetTarget(playerObject.transform);

        CreatePlatform("Start Platform", new Vector3(0f, -0.25f, 0f), new Vector3(8f, 0.5f, 8f), ground);
        CreatePlatform("Step 1", new Vector3(0f, 0.4f, 7f), new Vector3(4f, 0.45f, 3f), platform);
        CreatePlatform("Step 2", new Vector3(3.5f, 1.15f, 12f), new Vector3(3.2f, 0.45f, 3f), platform);
        CreatePlatform("Step 3", new Vector3(-2.4f, 2.05f, 16.5f), new Vector3(3.2f, 0.45f, 3f), platform);
        CreatePlatform("Long Landing", new Vector3(0.4f, 2.9f, 22f), new Vector3(6.5f, 0.45f, 3.5f), ground);
        CreatePlatform("Final Platform", new Vector3(0.4f, 4.05f, 29f), new Vector3(5f, 0.45f, 5f), accent);
        CreatePlatform("Low Wall Left", new Vector3(-4.25f, 0.35f, 0f), new Vector3(0.35f, 1.2f, 8f), platform);
        CreatePlatform("Low Wall Right", new Vector3(4.25f, 0.35f, 0f), new Vector3(0.35f, 1.2f, 8f), platform);
        CreateDeathZone("Death Zone", new Vector3(0f, -0.55f, 14f), new Vector3(12f, 0.15f, 30f), hazard);

        CreateCoin("Coin 1", new Vector3(0f, 1.5f, 7f), accent, 1f);
        CreateCoin("Coin 2", new Vector3(3.5f, 2.25f, 12f), accent, 1f);
        CreateCoin("Coin 3", new Vector3(-2.4f, 3.15f, 16.5f), accent, 1f);
        CreateCoin("Goal Coin", new Vector3(0.4f, 5.15f, 29f), accent, 1.6f);
    }

    private static bool HasStarterAssetsPlayer()
    {
#if UNITY_EDITOR
        return Object.FindFirstObjectByType<ThirdPersonController>() != null;
#else
        return false;
#endif
    }

    private static Material MakeMaterial(string name, Color color)
    {
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.name = name;
        material.color = color;
        return material;
    }

    private static void EnsureLight()
    {
        if (Object.FindFirstObjectByType<Light>() != null)
        {
            return;
        }

        GameObject lightObject = new GameObject("Sun Light");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 2.2f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    private static Camera EnsureCamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            camera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        camera.transform.position = new Vector3(0f, 6f, -8f);
        camera.transform.rotation = Quaternion.Euler(35f, 0f, 0f);
        camera.fieldOfView = 58f;
        camera.nearClipPlane = 0.05f;
        return camera;
    }

    private static GameObject CreatePlayer(Material material)
    {
#if UNITY_EDITOR
        GameObject starterPlayer = CreateStarterAssetsPlayer();
        if (starterPlayer != null)
        {
            return starterPlayer;
        }
#endif

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
        visual.transform.localScale = new Vector3(0.72f, 0.9f, 0.72f);
        Object.Destroy(visual.GetComponent<Collider>());
        visual.GetComponent<MeshRenderer>().sharedMaterial = material;

        GameObject nose = GameObject.CreatePrimitive(PrimitiveType.Cube);
        nose.name = "ForwardMarker";
        nose.transform.SetParent(visual.transform);
        nose.transform.localPosition = new Vector3(0f, 0.22f, 0.62f);
        nose.transform.localScale = new Vector3(0.18f, 0.18f, 0.28f);
        Object.Destroy(nose.GetComponent<Collider>());
        nose.GetComponent<MeshRenderer>().sharedMaterial = material;

        GameObject groundCheck = new GameObject("GroundCheck");
        groundCheck.transform.SetParent(playerObject.transform);
        groundCheck.transform.localPosition = new Vector3(0f, 0.12f, 0f);

        SmoothPlayerController mover = playerObject.AddComponent<SmoothPlayerController>();
        SetPrivateField(mover, "groundCheck", groundCheck.transform);
        SetPrivateField(mover, "visualRoot", visual.transform);

        playerObject.AddComponent<FallRespawn>();
        return playerObject;
    }

#if UNITY_EDITOR
    private static GameObject CreateStarterAssetsPlayer()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(StarterPlayerPrefabPath);
        if (prefab == null)
        {
            return null;
        }

        GameObject playerObject = Object.Instantiate(prefab, PlayerStartPosition, Quaternion.identity);
        playerObject.name = "Player";

        ThirdPersonController controller = playerObject.GetComponent<ThirdPersonController>();
        if (controller != null)
        {
            controller.MoveSpeed = 3.2f;
            controller.SprintSpeed = 6.4f;
            controller.SpeedChangeRate = 12f;
            controller.JumpHeight = 1.55f;
            controller.Gravity = -18f;
        }

        if (playerObject.GetComponent<FallRespawn>() == null)
        {
            playerObject.AddComponent<FallRespawn>();
        }

        return playerObject;
    }
#endif

    private static void SetPrivateField(Object target, string fieldName, object value)
    {
        target.GetType()
            .GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(target, value);
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

    private static void CreateCoin(string name, Vector3 position, Material material, float scale)
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
        coin.AddComponent<CoinPickup>();
    }
}
