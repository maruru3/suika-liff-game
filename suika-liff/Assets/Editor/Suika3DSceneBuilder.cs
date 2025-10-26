using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 3Dスイカゲームのシーンを自動構築するエディタスクリプト
/// メニュー: Tools > Build Suika 3D Scene
/// </summary>
public class Suika3DSceneBuilder : EditorWindow
{
    [MenuItem("Tools/Build Suika 3D Scene")]
    public static void BuildScene()
    {
        if (EditorUtility.DisplayDialog(
            "3Dスイカゲーム シーン構築",
            "現在のシーンに3Dスイカゲームのオブジェクトを自動生成します。\n\n既存のオブジェクトは削除されませんが、名前が重複する場合は上書きされます。\n\n続行しますか？",
            "はい", "キャンセル"))
        {
            BuildSuika3DScene();
        }
    }

    static void BuildSuika3DScene()
    {
        Debug.Log("=== 3Dスイカゲーム シーン構築開始 ===");

        // 1. ボックス作成（カメラのターゲット位置を決めるため先に作成）
        GameObject box = CreateBox();

        // 2. カメラ設定
        SetupCamera(box);

        // 3. ライト設定
        SetupLight();

        // 4. スポーンポイント作成
        CreateSpawnPoint();

        // 5. フルーツコンテナ作成
        GameObject fruitContainer = CreateFruitContainer();

        // 6. フルーツプレハブ作成
        GameObject fruitPrefab = CreateFruitPrefab();

        // 7. UI作成
        GameObject canvas = CreateUI();

        // 8. GameManager作成
        CreateGameManager(fruitPrefab, box, fruitContainer, canvas);

        // 9. LIFFBridge作成
        CreateLIFFBridge();

        Debug.Log("=== 3Dスイカゲーム シーン構築完了 ===");
        EditorUtility.DisplayDialog("完了", "3Dスイカゲームのシーン構築が完了しました！\n\nPlayボタンを押してテストプレイできます。", "OK");
    }

    static void SetupCamera(GameObject box)
    {
        Camera mainCamera = Camera.main;
        GameObject cameraObj;

        if (mainCamera == null)
        {
            cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
        }
        else
        {
            cameraObj = mainCamera.gameObject;
        }

        mainCamera.transform.position = new Vector3(0, 3, -8);
        mainCamera.transform.rotation = Quaternion.Euler(20, 0, 0);
        mainCamera.fieldOfView = 60;
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = new Color(0.2f, 0.3f, 0.5f);

        // CameraControllerを追加
        CameraController cameraController = cameraObj.GetComponent<CameraController>();
        if (cameraController == null)
        {
            cameraController = cameraObj.AddComponent<CameraController>();
        }

        // ターゲットをボックス中心に設定
        GameObject cameraTarget = GameObject.Find("CameraTarget");
        if (cameraTarget == null)
        {
            cameraTarget = new GameObject("CameraTarget");
        }
        cameraTarget.transform.position = new Vector3(0, 2, 0);

        cameraController.target = cameraTarget.transform;
        cameraController.distance = 8f;
        cameraController.rotationSpeed = 5f;
        cameraController.minDistance = 4f;
        cameraController.maxDistance = 15f;
        cameraController.minVerticalAngle = 10f;
        cameraController.maxVerticalAngle = 80f;

        EditorUtility.SetDirty(cameraController);

        Debug.Log("✓ カメラ設定完了（回転機能付き）");
    }

    static void SetupLight()
    {
        Light directionalLight = FindObjectOfType<Light>();
        if (directionalLight == null)
        {
            GameObject lightObj = new GameObject("Directional Light");
            directionalLight = lightObj.AddComponent<Light>();
            directionalLight.type = LightType.Directional;
        }

        directionalLight.transform.rotation = Quaternion.Euler(50, -30, 0);
        directionalLight.intensity = 1f;

        Debug.Log("✓ ライト設定完了");
    }

    static GameObject CreateBox()
    {
        GameObject box = GameObject.Find("Box");
        if (box == null)
        {
            box = new GameObject("Box");
        }

        // 床
        GameObject floor = CreateOrGetChild(box, "Floor");
        AddPrimitive(floor, PrimitiveType.Cube, new Vector3(0, 0, 0), new Vector3(4, 0.2f, 4), new Color(0.55f, 0.35f, 0.17f), "Floor");

        // 前壁
        GameObject wallFront = CreateOrGetChild(box, "WallFront");
        AddPrimitive(wallFront, PrimitiveType.Cube, new Vector3(0, 2, 2), new Vector3(4, 4, 0.2f), new Color(0.8f, 0.8f, 0.8f, 0.3f), "Wall", true);

        // 後壁
        GameObject wallBack = CreateOrGetChild(box, "WallBack");
        AddPrimitive(wallBack, PrimitiveType.Cube, new Vector3(0, 2, -2), new Vector3(4, 4, 0.2f), new Color(0.8f, 0.8f, 0.8f, 0.3f), "Wall", true);

        // 左壁
        GameObject wallLeft = CreateOrGetChild(box, "WallLeft");
        AddPrimitive(wallLeft, PrimitiveType.Cube, new Vector3(-2, 2, 0), new Vector3(0.2f, 4, 4), new Color(0.8f, 0.8f, 0.8f, 0.3f), "Wall", true);

        // 右壁
        GameObject wallRight = CreateOrGetChild(box, "WallRight");
        AddPrimitive(wallRight, PrimitiveType.Cube, new Vector3(2, 2, 0), new Vector3(0.2f, 4, 4), new Color(0.8f, 0.8f, 0.8f, 0.3f), "Wall", true);

        // ゲームオーバーライン
        GameObject gameOverLine = CreateOrGetChild(box, "GameOverLine");
        gameOverLine.transform.localPosition = new Vector3(0, 3.5f, 0);

        Debug.Log("✓ ボックス作成完了");
        return box;
    }

    static GameObject CreateSpawnPoint()
    {
        GameObject spawnPoint = GameObject.Find("SpawnPoint");
        if (spawnPoint == null)
        {
            spawnPoint = new GameObject("SpawnPoint");
        }
        spawnPoint.transform.position = new Vector3(0, 5, 0);

        Debug.Log("✓ スポーンポイント作成完了");
        return spawnPoint;
    }

    static GameObject CreateFruitContainer()
    {
        GameObject container = GameObject.Find("FruitContainer");
        if (container == null)
        {
            container = new GameObject("FruitContainer");
        }

        Debug.Log("✓ フルーツコンテナ作成完了");
        return container;
    }

    static GameObject CreateFruitPrefab()
    {
        // Prefabsフォルダ作成
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // 既存のプレハブをチェック
        string prefabPath = "Assets/Prefabs/Fruit3D.prefab";
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existingPrefab != null)
        {
            Debug.Log("✓ フルーツプレハブは既に存在します");
            return existingPrefab;
        }

        // 新しいプレハブ作成
        GameObject fruit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        fruit.name = "Fruit3D";

        // Rigidbody追加
        Rigidbody rb = fruit.AddComponent<Rigidbody>();
        rb.mass = 1;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // SphereCollider設定
        SphereCollider collider = fruit.GetComponent<SphereCollider>();

        // Physics Material作成
        PhysicsMaterial physicsMat = new PhysicsMaterial("FruitPhysics");
        physicsMat.bounciness = 0.3f;
        physicsMat.dynamicFriction = 0.5f;
        physicsMat.staticFriction = 0.5f;
        AssetDatabase.CreateAsset(physicsMat, "Assets/Prefabs/FruitPhysics.physicMaterial");
        collider.material = physicsMat;

        // AudioSource追加
        AudioSource audioSource = fruit.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;

        // Fruit3Dスクリプト追加
        fruit.AddComponent<Fruit3D>();

        // Material作成
        Material mat = new Material(Shader.Find("Standard"));
        mat.name = "FruitMaterial";
        fruit.GetComponent<Renderer>().material = mat;
        AssetDatabase.CreateAsset(mat, "Assets/Prefabs/FruitMaterial.mat");

        // Prefab保存
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(fruit, prefabPath);
        DestroyImmediate(fruit);

        Debug.Log("✓ フルーツプレハブ作成完了: " + prefabPath);
        return prefab;
    }

    static GameObject CreateUI()
    {
        // Canvas作成
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            canvas = new GameObject("Canvas");
            Canvas canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
        }

        // ScoreText
        GameObject scoreText = CreateOrGetChild(canvas, "ScoreText");
        AddTextMeshPro(scoreText, "Score: 0", 48, TextAlignmentOptions.TopLeft, new Vector2(100, -50));

        // NextFruitText
        GameObject nextFruitText = CreateOrGetChild(canvas, "NextFruitText");
        AddTextMeshPro(nextFruitText, "Next: さくらんぼ", 36, TextAlignmentOptions.Top, new Vector2(0, -50));

        // GameOverPanel
        GameObject gameOverPanel = CreateOrGetChild(canvas, "GameOverPanel");
        RectTransform panelRect = gameOverPanel.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            panelRect = gameOverPanel.AddComponent<RectTransform>();
        }
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        Image panelImage = gameOverPanel.GetComponent<Image>();
        if (panelImage == null)
        {
            panelImage = gameOverPanel.AddComponent<Image>();
        }
        panelImage.color = new Color(0, 0, 0, 0.8f);
        gameOverPanel.SetActive(false);

        // GameOverText
        GameObject gameOverText = CreateOrGetChild(gameOverPanel, "GameOverText");
        AddTextMeshPro(gameOverText, "GAME OVER", 72, TextAlignmentOptions.Center, new Vector2(0, 100));

        // FinalScoreText
        GameObject finalScoreText = CreateOrGetChild(gameOverPanel, "FinalScoreText");
        AddTextMeshPro(finalScoreText, "Score: 0", 48, TextAlignmentOptions.Center, new Vector2(0, 0));

        // RestartButton
        GameObject restartButton = CreateOrGetChild(gameOverPanel, "RestartButton");
        RectTransform buttonRect = restartButton.GetComponent<RectTransform>();
        if (buttonRect == null)
        {
            buttonRect = restartButton.AddComponent<RectTransform>();
        }
        buttonRect.anchoredPosition = new Vector2(0, -150);
        buttonRect.sizeDelta = new Vector2(300, 80);

        Button button = restartButton.GetComponent<Button>();
        if (button == null)
        {
            button = restartButton.AddComponent<Button>();
        }

        Image buttonImage = restartButton.GetComponent<Image>();
        if (buttonImage == null)
        {
            buttonImage = restartButton.AddComponent<Image>();
        }
        buttonImage.color = new Color(0.2f, 0.6f, 0.2f);

        GameObject buttonText = CreateOrGetChild(restartButton, "Text");
        AddTextMeshPro(buttonText, "リスタート", 36, TextAlignmentOptions.Center, Vector2.zero);

        Debug.Log("✓ UI作成完了");
        return canvas;
    }

    static void CreateGameManager(GameObject fruitPrefab, GameObject box, GameObject fruitContainer, GameObject canvas)
    {
        GameObject gameManager = GameObject.Find("GameManager");
        if (gameManager == null)
        {
            gameManager = new GameObject("GameManager");
        }

        GameManager3D gm = gameManager.GetComponent<GameManager3D>();
        if (gm == null)
        {
            gm = gameManager.AddComponent<GameManager3D>();
        }

        // 設定
        gm.fruitPrefab = fruitPrefab;
        gm.spawnPoint = GameObject.Find("SpawnPoint").transform;
        gm.fruitContainer = fruitContainer.transform;
        gm.dropHeight = 5f;
        gm.box = box.transform;
        gm.boxWidth = 4f;
        gm.boxDepth = 4f;
        gm.gameOverLine = box.transform.Find("GameOverLine");
        gm.mainCamera = Camera.main;
        gm.scoreText = canvas.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        gm.nextFruitText = canvas.transform.Find("NextFruitText").GetComponent<TextMeshProUGUI>();
        gm.gameOverPanel = canvas.transform.Find("GameOverPanel").gameObject;
        gm.restartButton = canvas.transform.Find("GameOverPanel/RestartButton").GetComponent<Button>();

        EditorUtility.SetDirty(gm);

        Debug.Log("✓ GameManager設定完了");
    }

    static void CreateLIFFBridge()
    {
        GameObject liffBridge = GameObject.Find("LIFFBridge");
        if (liffBridge == null)
        {
            liffBridge = new GameObject("LIFFBridge");
        }

        if (liffBridge.GetComponent<LIFFBridge>() == null)
        {
            liffBridge.AddComponent<LIFFBridge>();
        }

        Debug.Log("✓ LIFFBridge作成完了");
    }

    // ヘルパー関数
    static GameObject CreateOrGetChild(GameObject parent, string name)
    {
        Transform child = parent.transform.Find(name);
        if (child != null)
        {
            return child.gameObject;
        }

        GameObject newChild = new GameObject(name);
        newChild.transform.SetParent(parent.transform);
        return newChild;
    }

    static void AddPrimitive(GameObject obj, PrimitiveType type, Vector3 position, Vector3 scale, Color color, string tag = null, bool transparent = false)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            GameObject temp = GameObject.CreatePrimitive(type);
            meshFilter = obj.AddComponent<MeshFilter>();
            meshFilter.mesh = temp.GetComponent<MeshFilter>().mesh;
            DestroyImmediate(temp);
        }

        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            renderer = obj.AddComponent<MeshRenderer>();
        }

        Material mat = new Material(Shader.Find("Standard"));
        if (transparent)
        {
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
        mat.color = color;
        renderer.material = mat;

        BoxCollider collider = obj.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = obj.AddComponent<BoxCollider>();
        }

        obj.transform.localPosition = position;
        obj.transform.localScale = scale;

        if (!string.IsNullOrEmpty(tag))
        {
            obj.tag = tag;
        }
    }

    static void AddTextMeshPro(GameObject obj, string text, float fontSize, TextAlignmentOptions alignment, Vector2 anchoredPosition)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect == null)
        {
            rect = obj.AddComponent<RectTransform>();
        }

        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            tmp = obj.AddComponent<TextMeshProUGUI>();
        }

        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.white;

        rect.anchoredPosition = anchoredPosition;
    }
}
