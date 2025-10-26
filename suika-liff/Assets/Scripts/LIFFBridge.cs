using System;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Unity-LIFF Bridge
/// Unity WebGLとLIFF (LINE Front-end Framework)間の通信を管理するC#スクリプト
/// VSCode + Claude Code環境で開発
///
/// 使い方:
/// 1. このスクリプトを"LIFFBridge"という名前のGameObjectにアタッチ
/// 2. LIFF IDを設定
/// 3. イベントハンドラーを登録
/// </summary>
public class LIFFBridge : MonoBehaviour
{
    [Header("LIFF Settings")]
    [SerializeField] private string liffId = "2008275057-xxx"; // LIFF IDを設定

    [Header("User Info")]
    public string userId = "";
    public string displayName = "";
    public string pictureUrl = "";

    [Header("Status")]
    public bool isLiffInitialized = false;
    public bool isBridgeReady = false;

    // イベント
    public event Action<UserProfile> OnLiffInitialized;
    public event Action OnBridgeReady;
    public event Action<string> OnLiffError;
    public event Action OnShareSuccess;
    public event Action<string> OnShareError;
    public event Action<string> OnSaveSuccess;
    public event Action<string> OnSaveError;
    public event Action<string> OnLoadComplete;
    public event Action<string> OnLoadError;
    public event Action<string> OnScanSuccess;
    public event Action<string> OnScanError;

    // JavaScript関数インポート (WebGLのみ)
    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void InitializeLIFF(string liffId);

    [DllImport("__Internal")]
    private static extern void ShareScore(int score, string message);

    [DllImport("__Internal")]
    private static extern void SaveGameData(string key, string data);

    [DllImport("__Internal")]
    private static extern string LoadGameData(string key);

    [DllImport("__Internal")]
    private static extern string GetUserProfile();

    [DllImport("__Internal")]
    private static extern string GetLiffContext();

    [DllImport("__Internal")]
    private static extern void CloseLiff();

    [DllImport("__Internal")]
    private static extern void OpenExternalLink(string url);

    [DllImport("__Internal")]
    private static extern void ScanCode();
    #endif

    void Start()
    {
        // LIFFBridgeはシーン間で永続化
        DontDestroyOnLoad(gameObject);

        // LIFF初期化
        InitializeLIFFBridge();
    }

    /// <summary>
    /// LIFF Bridge初期化
    /// </summary>
    public void InitializeLIFFBridge()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("LIFF初期化開始: " + liffId);
        InitializeLIFF(liffId);
        #else
        Debug.LogWarning("WebGL以外の環境では動作しません");
        // エディタでのテスト用
        SimulateLiffInitialized();
        #endif
    }

    /// <summary>
    /// スコアをLINEで共有
    /// </summary>
    public void ShareGameScore(int score, string customMessage = "")
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        string message = string.IsNullOrEmpty(customMessage)
            ? $"スコア: {score}点を獲得しました！"
            : customMessage;
        ShareScore(score, message);
        #else
        Debug.Log($"スコア共有 (エディタ): {score}");
        #endif
    }

    /// <summary>
    /// ゲームデータ保存
    /// </summary>
    public void SaveData(string key, string jsonData)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        SaveGameData(key, jsonData);
        #else
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.Save();
        Debug.Log($"データ保存 (エディタ): {key}");
        #endif
    }

    /// <summary>
    /// ゲームデータ読み込み
    /// </summary>
    public string LoadData(string key)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        return LoadGameData(key);
        #else
        return PlayerPrefs.GetString(key, "");
        #endif
    }

    /// <summary>
    /// ユーザープロフィール取得
    /// </summary>
    public UserProfile GetProfile()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        string json = GetUserProfile();
        if (!string.IsNullOrEmpty(json))
        {
            return JsonUtility.FromJson<UserProfile>(json);
        }
        #endif
        return null;
    }

    /// <summary>
    /// LIFF環境情報取得
    /// </summary>
    public LiffContext GetContext()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        string json = GetLiffContext();
        if (!string.IsNullOrEmpty(json))
        {
            return JsonUtility.FromJson<LiffContext>(json);
        }
        #endif
        return null;
    }

    /// <summary>
    /// LIFFウィンドウを閉じる
    /// </summary>
    public void Close()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        CloseLiff();
        #else
        Debug.Log("LIFF終了 (エディタ)");
        #endif
    }

    /// <summary>
    /// 外部リンクを開く
    /// </summary>
    public void OpenLink(string url)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        OpenExternalLink(url);
        #else
        Application.OpenURL(url);
        #endif
    }

    /// <summary>
    /// QRコードスキャン
    /// </summary>
    public void StartScan()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        ScanCode();
        #else
        Debug.Log("QRスキャン起動 (エディタ)");
        #endif
    }

    // ===== JavaScript側から呼ばれるコールバック関数 =====

    /// <summary>
    /// LIFF初期化完了時に呼ばれる
    /// </summary>
    public void OnLiffInitializedCallback(string json)
    {
        Debug.Log("LIFF初期化完了: " + json);
        isLiffInitialized = true;

        UserProfile profile = JsonUtility.FromJson<UserProfile>(json);
        userId = profile.userId;
        displayName = profile.displayName;
        pictureUrl = profile.pictureUrl;

        OnLiffInitialized?.Invoke(profile);
    }

    /// <summary>
    /// Bridge準備完了時に呼ばれる
    /// </summary>
    public void OnBridgeReadyCallback(string json)
    {
        Debug.Log("Bridge準備完了: " + json);
        isBridgeReady = true;
        OnBridgeReady?.Invoke();
    }

    /// <summary>
    /// LIFFエラー時に呼ばれる
    /// </summary>
    public void OnLiffErrorCallback(string error)
    {
        Debug.LogError("LIFFエラー: " + error);
        OnLiffError?.Invoke(error);
    }

    /// <summary>
    /// 共有成功時に呼ばれる
    /// </summary>
    public void OnShareSuccessCallback(string message)
    {
        Debug.Log("共有成功: " + message);
        OnShareSuccess?.Invoke();
    }

    /// <summary>
    /// 共有エラー時に呼ばれる
    /// </summary>
    public void OnShareErrorCallback(string error)
    {
        Debug.LogError("共有エラー: " + error);
        OnShareError?.Invoke(error);
    }

    /// <summary>
    /// 保存成功時に呼ばれる
    /// </summary>
    public void OnSaveSuccessCallback(string key)
    {
        Debug.Log("保存成功: " + key);
        OnSaveSuccess?.Invoke(key);
    }

    /// <summary>
    /// 保存エラー時に呼ばれる
    /// </summary>
    public void OnSaveErrorCallback(string error)
    {
        Debug.LogError("保存エラー: " + error);
        OnSaveError?.Invoke(error);
    }

    /// <summary>
    /// 読み込み完了時に呼ばれる
    /// </summary>
    public void OnLoadCompleteCallback(string data)
    {
        Debug.Log("読み込み完了");
        OnLoadComplete?.Invoke(data);
    }

    /// <summary>
    /// 読み込みエラー時に呼ばれる
    /// </summary>
    public void OnLoadErrorCallback(string error)
    {
        Debug.LogError("読み込みエラー: " + error);
        OnLoadError?.Invoke(error);
    }

    /// <summary>
    /// スキャン成功時に呼ばれる
    /// </summary>
    public void OnScanSuccessCallback(string code)
    {
        Debug.Log("スキャン成功: " + code);
        OnScanSuccess?.Invoke(code);
    }

    /// <summary>
    /// スキャンエラー時に呼ばれる
    /// </summary>
    public void OnScanErrorCallback(string error)
    {
        Debug.LogError("スキャンエラー: " + error);
        OnScanError?.Invoke(error);
    }

    // エディタ用シミュレーション
    #if UNITY_EDITOR
    private void SimulateLiffInitialized()
    {
        UserProfile testProfile = new UserProfile
        {
            userId = "test_user_123",
            displayName = "テストユーザー",
            pictureUrl = "https://via.placeholder.com/150"
        };

        OnLiffInitializedCallback(JsonUtility.ToJson(testProfile));
        OnBridgeReadyCallback("{}");
    }
    #endif

    // データ構造
    [Serializable]
    public class UserProfile
    {
        public string userId;
        public string displayName;
        public string pictureUrl;
    }

    [Serializable]
    public class LiffContext
    {
        public bool isInClient;
        public bool isLoggedIn;
        public string os;
        public string language;
        public string version;
    }
}
