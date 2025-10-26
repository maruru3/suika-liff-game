using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Unity-LIFF統合のサンプルゲームマネージャー
/// LIFFBridgeを使用したゲーム実装の例
/// VSCode + Claude Code環境で開発
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button shareButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private RawImage userAvatarImage;

    [Header("Game Settings")]
    [SerializeField] private int currentScore = 0;

    private LIFFBridge liffBridge;
    private bool isGameInitialized = false;

    void Start()
    {
        // LIFFBridge取得
        liffBridge = FindObjectOfType<LIFFBridge>();

        if (liffBridge == null)
        {
            Debug.LogError("LIFFBridgeが見つかりません！");
            return;
        }

        // イベントリスナー登録
        RegisterEventListeners();

        // UIボタン設定
        SetupButtons();

        // Bridge準備待ち
        if (!liffBridge.isBridgeReady)
        {
            Debug.Log("Bridge準備待機中...");
        }
        else
        {
            OnGameReady();
        }
    }

    /// <summary>
    /// イベントリスナー登録
    /// </summary>
    private void RegisterEventListeners()
    {
        liffBridge.OnLiffInitialized += OnLiffInitialized;
        liffBridge.OnBridgeReady += OnBridgeReady;
        liffBridge.OnLiffError += OnLiffError;
        liffBridge.OnShareSuccess += OnShareSuccess;
        liffBridge.OnShareError += OnShareError;
        liffBridge.OnSaveSuccess += OnSaveSuccess;
        liffBridge.OnLoadComplete += OnLoadComplete;
    }

    /// <summary>
    /// UIボタン設定
    /// </summary>
    private void SetupButtons()
    {
        if (shareButton != null)
            shareButton.onClick.AddListener(OnShareButtonClicked);

        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveButtonClicked);

        if (loadButton != null)
            loadButton.onClick.AddListener(OnLoadButtonClicked);
    }

    /// <summary>
    /// LIFF初期化完了時
    /// </summary>
    private void OnLiffInitialized(LIFFBridge.UserProfile profile)
    {
        Debug.Log($"ユーザー情報取得: {profile.displayName}");

        // ユーザー名表示
        if (userNameText != null)
        {
            userNameText.text = $"こんにちは、{profile.displayName}さん！";
        }

        // アバター画像読み込み（コルーチンで非同期）
        if (userAvatarImage != null && !string.IsNullOrEmpty(profile.pictureUrl))
        {
            StartCoroutine(LoadUserAvatar(profile.pictureUrl));
        }
    }

    /// <summary>
    /// Bridge準備完了時
    /// </summary>
    private void OnBridgeReady()
    {
        Debug.Log("Bridge準備完了！ゲーム開始可能");
        OnGameReady();
    }

    /// <summary>
    /// ゲーム準備完了
    /// </summary>
    private void OnGameReady()
    {
        if (isGameInitialized) return;

        isGameInitialized = true;

        // LIFF環境情報取得
        var context = liffBridge.GetContext();
        if (context != null)
        {
            Debug.Log($"LIFF環境: OS={context.os}, InClient={context.isInClient}");
        }

        // 保存データ自動読み込み
        LoadGameData();

        // スコア表示更新
        UpdateScoreDisplay();
    }

    /// <summary>
    /// LIFFエラー時
    /// </summary>
    private void OnLiffError(string error)
    {
        Debug.LogError($"LIFFエラー: {error}");
        // エラー表示UIを実装する場合はここで
    }

    /// <summary>
    /// スコア加算（ゲームロジック例）
    /// </summary>
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();
        Debug.Log($"スコア: {currentScore}");
    }

    /// <summary>
    /// スコア表示更新
    /// </summary>
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"スコア: {currentScore}";
        }
    }

    /// <summary>
    /// 共有ボタンクリック
    /// </summary>
    private void OnShareButtonClicked()
    {
        string message = $"🎮 スコア {currentScore}点を獲得しました！\n一緒に遊びましょう！";
        liffBridge.ShareGameScore(currentScore, message);
    }

    /// <summary>
    /// 共有成功
    /// </summary>
    private void OnShareSuccess()
    {
        Debug.Log("スコア共有成功！");
        // 共有ボーナスなど
        AddScore(100);
    }

    /// <summary>
    /// 共有エラー
    /// </summary>
    private void OnShareError(string error)
    {
        Debug.LogWarning($"共有失敗: {error}");
    }

    /// <summary>
    /// 保存ボタンクリック
    /// </summary>
    private void OnSaveButtonClicked()
    {
        SaveGameData();
    }

    /// <summary>
    /// ゲームデータ保存
    /// </summary>
    private void SaveGameData()
    {
        GameData data = new GameData
        {
            score = currentScore,
            timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        string json = JsonUtility.ToJson(data);
        liffBridge.SaveData("gameData", json);
        Debug.Log("データ保存リクエスト送信");
    }

    /// <summary>
    /// 保存成功
    /// </summary>
    private void OnSaveSuccess(string key)
    {
        Debug.Log($"データ保存完了: {key}");
        // 保存完了通知UIなど
    }

    /// <summary>
    /// 読み込みボタンクリック
    /// </summary>
    private void OnLoadButtonClicked()
    {
        LoadGameData();
    }

    /// <summary>
    /// ゲームデータ読み込み
    /// </summary>
    private void LoadGameData()
    {
        string json = liffBridge.LoadData("gameData");

        if (!string.IsNullOrEmpty(json))
        {
            OnLoadComplete(json);
        }
        else
        {
            Debug.Log("保存データが見つかりません");
        }
    }

    /// <summary>
    /// 読み込み完了
    /// </summary>
    private void OnLoadComplete(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.Log("読み込むデータがありません");
            return;
        }

        try
        {
            GameData data = JsonUtility.FromJson<GameData>(json);
            currentScore = data.score;
            UpdateScoreDisplay();
            Debug.Log($"データ読み込み完了: Score={data.score}, Time={data.timestamp}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"データ解析エラー: {e.Message}");
        }
    }

    /// <summary>
    /// ユーザーアバター画像読み込み
    /// </summary>
    private System.Collections.IEnumerator LoadUserAvatar(string url)
    {
        UnityEngine.Networking.UnityWebRequest request =
            UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url);

        yield return request.SendWebRequest();

        if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Texture2D texture = UnityEngine.Networking.DownloadHandlerTexture.GetContent(request);
            userAvatarImage.texture = texture;
            Debug.Log("アバター画像読み込み完了");
        }
        else
        {
            Debug.LogWarning($"アバター画像読み込み失敗: {request.error}");
        }
    }

    /// <summary>
    /// クリーンアップ
    /// </summary>
    private void OnDestroy()
    {
        if (liffBridge != null)
        {
            liffBridge.OnLiffInitialized -= OnLiffInitialized;
            liffBridge.OnBridgeReady -= OnBridgeReady;
            liffBridge.OnLiffError -= OnLiffError;
            liffBridge.OnShareSuccess -= OnShareSuccess;
            liffBridge.OnShareError -= OnShareError;
            liffBridge.OnSaveSuccess -= OnSaveSuccess;
            liffBridge.OnLoadComplete -= OnLoadComplete;
        }
    }

    // ゲームデータ構造
    [System.Serializable]
    private class GameData
    {
        public int score;
        public string timestamp;
    }
}
