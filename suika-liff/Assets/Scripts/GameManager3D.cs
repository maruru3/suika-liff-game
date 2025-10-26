using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 3Dスイカゲームのメインマネージャー
/// フルーツ生成、スコア管理、ゲームロジック
/// </summary>
public class GameManager3D : MonoBehaviour
{
    [Header("フルーツ設定")]
    public GameObject fruitPrefab; // 基本フルーツプレハブ
    public Transform spawnPoint; // 生成位置
    public Transform fruitContainer; // フルーツの親オブジェクト
    public float dropHeight = 5f; // ドロップ高さ

    [Header("ボックス設定")]
    public Transform box; // ゲームボックス
    public float boxWidth = 4f;
    public float boxDepth = 4f;
    public Transform gameOverLine; // ゲームオーバーライン

    [Header("カメラ設定")]
    public Camera mainCamera;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI nextFruitText;
    public GameObject gameOverPanel;
    public Button restartButton;

    [Header("サウンド")]
    public AudioClip[] mergeSounds; // フルーツレベルごとの合成音
    public AudioClip dropSound;
    public AudioClip gameOverSound;
    private AudioSource audioSource;

    [Header("フルーツデータ")]
    public FruitData[] fruitDataArray = new FruitData[10];

    // ゲーム状態
    private int currentScore = 0;
    private GameObject currentFruit;
    private int nextFruitLevel = 0;
    private bool isGameOver = false;
    private bool canDrop = true;

    // LIFF連携
    private LIFFBridge liffBridge;

    // カメラコントローラー
    private CameraController cameraController;

    [System.Serializable]
    public class FruitData
    {
        public string name;
        public Color color;
        public float size; // 直径（メートル）
    }

    void Start()
    {
        // 初期化
        audioSource = gameObject.AddComponent<AudioSource>();
        liffBridge = FindObjectOfType<LIFFBridge>();
        cameraController = FindObjectOfType<CameraController>();

        // フルーツデータ初期化
        InitializeFruitData();

        // UI初期化
        UpdateScoreUI();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        // 最初のフルーツを準備
        PrepareNextFruit();
    }

    void Update()
    {
        if (isGameOver || currentFruit == null)
            return;

        // マウス/タッチ入力でフルーツを移動
        HandleInput();

        // ゲームオーバーチェック
        CheckGameOver();
    }

    /// <summary>
    /// フルーツデータを初期化
    /// </summary>
    void InitializeFruitData()
    {
        fruitDataArray[0] = new FruitData { name = "さくらんぼ", color = new Color(0.8f, 0.1f, 0.1f), size = 0.4f };
        fruitDataArray[1] = new FruitData { name = "いちご", color = new Color(1f, 0.2f, 0.2f), size = 0.5f };
        fruitDataArray[2] = new FruitData { name = "ぶどう", color = new Color(0.5f, 0.2f, 0.6f), size = 0.6f };
        fruitDataArray[3] = new FruitData { name = "デコポン", color = new Color(1f, 0.6f, 0.2f), size = 0.7f };
        fruitDataArray[4] = new FruitData { name = "かき", color = new Color(1f, 0.5f, 0.1f), size = 0.8f };
        fruitDataArray[5] = new FruitData { name = "りんご", color = new Color(0.9f, 0.1f, 0.1f), size = 0.9f };
        fruitDataArray[6] = new FruitData { name = "なし", color = new Color(0.9f, 0.9f, 0.5f), size = 1.0f };
        fruitDataArray[7] = new FruitData { name = "もも", color = new Color(1f, 0.7f, 0.7f), size = 1.1f };
        fruitDataArray[8] = new FruitData { name = "パイナップル", color = new Color(0.9f, 0.8f, 0.2f), size = 1.2f };
        fruitDataArray[9] = new FruitData { name = "スイカ", color = new Color(0.2f, 0.7f, 0.2f), size = 1.4f };
    }

    /// <summary>
    /// 入力処理
    /// </summary>
    void HandleInput()
    {
        Vector3 inputPosition = Vector3.zero;
        bool hasInput = false;

        // マウス入力（左クリックのみ、右クリックとCtrl+左クリックはカメラ回転用）
        if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftControl))
        {
            inputPosition = Input.mousePosition;
            hasInput = true;

            // カメラ回転を一時的に無効化
            if (cameraController != null)
                cameraController.SetCanRotate(false);
        }
        // タッチ入力（1本指のみ、2本指はズーム用）
        else if (Input.touchCount == 1)
        {
            inputPosition = Input.GetTouch(0).position;
            hasInput = true;

            // カメラ回転を一時的に無効化
            if (cameraController != null)
                cameraController.SetCanRotate(false);
        }
        else
        {
            // カメラ回転を再有効化
            if (cameraController != null)
                cameraController.SetCanRotate(true);
        }

        if (hasInput)
        {
            // スクリーン座標をワールド座標に変換
            Ray ray = mainCamera.ScreenPointToRay(inputPosition);
            Plane plane = new Plane(Vector3.up, new Vector3(0, dropHeight, 0));
            float distance;

            if (plane.Raycast(ray, out distance))
            {
                Vector3 worldPoint = ray.GetPoint(distance);

                // ボックス内に制限
                float halfWidth = boxWidth / 2f - fruitDataArray[nextFruitLevel].size / 2f;
                float halfDepth = boxDepth / 2f - fruitDataArray[nextFruitLevel].size / 2f;

                worldPoint.x = Mathf.Clamp(worldPoint.x, -halfWidth, halfWidth);
                worldPoint.z = Mathf.Clamp(worldPoint.z, -halfDepth, halfDepth);
                worldPoint.y = dropHeight;

                // フルーツを移動
                currentFruit.transform.position = worldPoint;
            }
        }

        // クリック/タップ離したらドロップ
        if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)) && canDrop)
        {
            DropFruit();
        }
    }

    /// <summary>
    /// 次のフルーツを準備
    /// </summary>
    void PrepareNextFruit()
    {
        // ランダムに小さいフルーツ（レベル0-4）を選択
        nextFruitLevel = Random.Range(0, 5);

        // フルーツ生成
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : new Vector3(0, dropHeight, 0);
        currentFruit = Instantiate(fruitPrefab, spawnPos, Quaternion.identity, fruitContainer);

        // フルーツ設定
        Fruit3D fruit = currentFruit.GetComponent<Fruit3D>();
        if (fruit != null)
        {
            FruitData data = fruitDataArray[nextFruitLevel];
            fruit.SetupFruit(nextFruitLevel, data.color, data.size);
            fruit.DisablePhysics(); // 配置中は物理無効
        }

        // UI更新
        if (nextFruitText != null)
        {
            nextFruitText.text = "Next: " + fruitDataArray[nextFruitLevel].name;
        }

        canDrop = true;
    }

    /// <summary>
    /// フルーツをドロップ
    /// </summary>
    void DropFruit()
    {
        if (currentFruit == null)
            return;

        canDrop = false;

        // 物理有効化
        Fruit3D fruit = currentFruit.GetComponent<Fruit3D>();
        if (fruit != null)
        {
            fruit.EnablePhysics();
        }

        // ドロップ音再生
        if (audioSource != null && dropSound != null)
        {
            audioSource.PlayOneShot(dropSound);
        }

        // 次のフルーツを少し遅延して準備
        StartCoroutine(PrepareNextFruitDelayed());

        currentFruit = null;
    }

    IEnumerator PrepareNextFruitDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        if (!isGameOver)
        {
            PrepareNextFruit();
        }
    }

    /// <summary>
    /// 指定位置にフルーツを生成（合成時）
    /// </summary>
    public void SpawnFruitAt(int level, Vector3 position)
    {
        if (level >= fruitDataArray.Length)
            return;

        GameObject newFruit = Instantiate(fruitPrefab, position, Quaternion.identity, fruitContainer);
        Fruit3D fruit = newFruit.GetComponent<Fruit3D>();

        if (fruit != null)
        {
            FruitData data = fruitDataArray[level];
            fruit.SetupFruit(level, data.color, data.size);
            fruit.EnablePhysics();
        }
    }

    /// <summary>
    /// スコア加算
    /// </summary>
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
    }

    /// <summary>
    /// スコアUI更新
    /// </summary>
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }

    /// <summary>
    /// 合成音を再生
    /// </summary>
    public void PlayMergeSound(int level)
    {
        if (audioSource != null && mergeSounds != null && level < mergeSounds.Length && mergeSounds[level] != null)
        {
            audioSource.PlayOneShot(mergeSounds[level]);
        }
    }

    /// <summary>
    /// ゲームオーバーチェック
    /// </summary>
    void CheckGameOver()
    {
        if (gameOverLine == null || fruitContainer == null)
            return;

        // ゲームオーバーラインより上にフルーツがあるかチェック
        foreach (Transform child in fruitContainer)
        {
            Fruit3D fruit = child.GetComponent<Fruit3D>();
            if (fruit != null && fruit.canMerge && child.position.y > gameOverLine.position.y)
            {
                // 少し待ってから再チェック（誤判定防止）
                StartCoroutine(CheckGameOverDelayed(child));
                return;
            }
        }
    }

    IEnumerator CheckGameOverDelayed(Transform fruitTransform)
    {
        yield return new WaitForSeconds(2f);

        if (fruitTransform != null && fruitTransform.position.y > gameOverLine.position.y)
        {
            GameOver();
        }
    }

    /// <summary>
    /// ゲームオーバー
    /// </summary>
    void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        // ゲームオーバー音再生
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        // UI表示
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 現在のフルーツを削除
        if (currentFruit != null)
        {
            Destroy(currentFruit);
        }

        // LIFFにスコア送信
        if (liffBridge != null)
        {
            string message = $"🍉 3Dスイカゲーム\nスコア: {currentScore}点";
            // liffBridge.ShareGameScore(currentScore, message);
        }

        Debug.Log("Game Over! Final Score: " + currentScore);
    }

    /// <summary>
    /// ゲーム再開
    /// </summary>
    void RestartGame()
    {
        // シーンリロード
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
