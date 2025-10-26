# Unity-LINEミニアプリ 連携システム（2025年版）

Unity WebGLゲームとLINEミニアプリを統合するための完全なシステムです。
VSCode + Claude Code環境で開発されています。

> **2025年最新版**: LIFFはLINEミニアプリに統合されました。このシステムは最新の方式に対応しています。
> コード側は従来のLIFF SDK v2をそのまま使用できます。

## 📋 目次

- [概要](#概要)
- [システム構成](#システム構成)
- [セットアップ](#セットアップ)
- [使い方](#使い方)
- [API リファレンス](#apiリファレンス)
- [サンプルコード](#サンプルコード)
- [トラブルシューティング](#トラブルシューティング)

## 🎯 概要

このシステムは以下の機能を提供します:

- ✅ Unity WebGL ↔ LINEミニアプリ間の双方向通信
- ✅ LINEユーザープロフィール取得
- ✅ スコア・結果のLINE共有機能
- ✅ ローカルストレージでのデータ保存/読み込み
- ✅ QRコードスキャン機能
- ✅ 外部リンク開く機能
- ✅ Unity Editor での動作シミュレーション
- ✨ 外部ブラウザ対応（2025年10月〜）

## 🏗️ システム構成

```
┌─────────────────────────────────────┐
│         LINE Platform               │
│  (プロフィール、共有、スキャン)      │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│      LIFF SDK (JavaScript)          │
│  (LINE Front-end Framework)         │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│   Unity-LIFF Bridge (JavaScript)    │
│  - unity-liff-bridge.js             │
│  - イベント管理                      │
│  - データ変換                        │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│     Unity WebGL (C#)                │
│  - LIFFBridge.cs                    │
│  - GameManager.cs                   │
│  - Your Game Logic                  │
└─────────────────────────────────────┘
```

## 🚀 セットアップ

### 1. ファイル構成

プロジェクトに以下のファイルを配置します:

```
YourProject/
├── Assets/
│   └── Scripts/
│       ├── LIFFBridge.cs          # Unity側Bridge
│       └── GameManager.cs         # サンプルゲームマネージャー
├── WebGLTemplates/               # Unity WebGLテンプレート
│   └── LIFFTemplate/
│       ├── index.html            # unity-liff-template.htmlをリネーム
│       └── unity-liff-bridge.js  # JavaScript Bridge
└── Build/                        # ビルド出力先
```

### 2. Unity プロジェクト設定

#### 2.1 WebGL ビルド設定

1. **File > Build Settings**
2. Platform: **WebGL** を選択
3. **Player Settings** を開く
4. **Publishing Settings**:
   - Compression Format: **Disabled** または **Gzip**
   - Decompression Fallback: チェック

#### 2.2 WebGL テンプレート設定

1. `Assets/WebGLTemplates/LIFFTemplate/` フォルダを作成
2. `unity-liff-template.html` を `index.html` にリネームして配置
3. `unity-liff-bridge.js` を同じフォルダに配置
4. **Player Settings > Resolution and Presentation**:
   - WebGL Template: **LIFFTemplate** を選択

#### 2.3 LIFFBridge GameObject作成

1. **Hierarchy** で右クリック > **Create Empty**
2. 名前を `LIFFBridge` に変更（**重要**: この名前は固定）
3. `LIFFBridge.cs` スクリプトをアタッチ
4. Inspector で LIFF ID を設定

### 3. LINEミニアプリ設定

#### 3.1 LINE Developers設定

1. [LINE Developers Console](https://developers.line.biz/console/)にログイン
2. **LINEミニアプリチャネル** を作成
   - チャネルタイプ: **「LINEミニアプリ」** を選択
3. **LIFF アプリ** を追加（LIFFタブから）
4. 以下の設定:
   - **Size**: `Full`（フルスクリーン）
   - **Endpoint URL**: WebGLビルドをホストするURL（HTTPS必須）
   - **Scopes**: `profile`, `openid`, `chat_message.write`
   - **Scan QR**: `On`（QRスキャンを使う場合）

> **詳しい手順**: [MINIAPP_SETUP_GUIDE.md](MINIAPP_SETUP_GUIDE.md) を参照

#### 3.2 LIFF ID設定

取得したLIFF IDを以下の箇所に設定:

**自動設定（推奨）:**
```bash
python setup_liff_id.py YOUR-LIFF-ID
```

**手動設定:**
- `LIFFBridge.cs`: `liffId` フィールド
- `unity-liff-template.html`: `CONFIG.liffId`

### 4. ビルドとデプロイ

#### 4.1 Unityビルド

1. **File > Build Settings**
2. **Build** をクリック
3. 出力先: `Build` フォルダ

#### 4.2 ファイル構成確認

ビルド後、以下の構成になっているか確認:

```
Build/
├── index.html               # メインHTML
├── unity-liff-bridge.js     # Bridge
├── Build/
│   ├── UnityBuild.loader.js
│   ├── UnityBuild.data
│   ├── UnityBuild.framework.js
│   └── UnityBuild.wasm
└── TemplateData/           # Unity生成ファイル
```

#### 4.3 HTTPS サーバーでホスト

LIFFはHTTPS必須です。以下のいずれかの方法でホスト:

**方法1: Python HTTPS サーバー (開発用)**

```bash
# 既存のhttps_server.pyを使用
python https_server.py
```

**方法2: ngrok (外部アクセス可能)**

```bash
cd Build
npx http-server -p 8080
# 別ターミナルで
ngrok http 8080
```

**方法3: 本番環境 (推奨)**

- GitHub Pages (HTTPS自動)
- Netlify
- Vercel
- Firebase Hosting

## 📖 使い方

### Unity側 (C#)

#### 基本的な使い方

```csharp
using UnityEngine;

public class MyGame : MonoBehaviour
{
    private LIFFBridge liffBridge;

    void Start()
    {
        // LIFFBridge取得
        liffBridge = FindObjectOfType<LIFFBridge>();

        // イベント登録
        liffBridge.OnLiffInitialized += OnLiffReady;
        liffBridge.OnShareSuccess += OnShareSuccess;
    }

    void OnLiffReady(LIFFBridge.UserProfile profile)
    {
        Debug.Log($"ようこそ、{profile.displayName}さん！");
    }

    public void ShareMyScore(int score)
    {
        liffBridge.ShareGameScore(score, $"{score}点獲得！");
    }

    void OnShareSuccess()
    {
        Debug.Log("共有完了！");
    }
}
```

#### データ保存/読み込み

```csharp
// 保存
[System.Serializable]
public class SaveData
{
    public int level;
    public int score;
}

SaveData data = new SaveData { level = 5, score = 1000 };
string json = JsonUtility.ToJson(data);
liffBridge.SaveData("mySave", json);

// 読み込み
string json = liffBridge.LoadData("mySave");
SaveData data = JsonUtility.FromJson<SaveData>(json);
```

#### QRコードスキャン

```csharp
liffBridge.OnScanSuccess += (code) => {
    Debug.Log($"スキャン結果: {code}");
};

liffBridge.StartScan();
```

### JavaScript側

JavaScript側から直接操作する場合:

```javascript
// LIFF初期化
await window.unityLiffBridge.initializeLIFF('YOUR-LIFF-ID');

// Unity側にメッセージ送信
window.unityLiffBridge.sendToUnity('OnCustomEvent', 'Hello Unity!');

// ユーザープロフィール取得
const profile = window.unityLiffBridge.getUserProfile();
console.log(profile);
```

## 🔌 API リファレンス

### LIFFBridge (C#)

#### プロパティ

| プロパティ | 型 | 説明 |
|-----------|-----|------|
| `isLiffInitialized` | bool | LIFF初期化済みか |
| `isBridgeReady` | bool | Bridge準備完了か |
| `userId` | string | LINEユーザーID |
| `displayName` | string | LINEユーザー名 |
| `pictureUrl` | string | LINEアバターURL |

#### メソッド

##### InitializeLIFFBridge()
LIFF Bridgeを初期化します（自動的にStart()で呼ばれます）

```csharp
public void InitializeLIFFBridge()
```

##### ShareGameScore(score, message)
スコアをLINEで共有します

```csharp
public void ShareGameScore(int score, string customMessage = "")
```

**Parameters:**
- `score`: 共有するスコア
- `customMessage`: カスタムメッセージ（省略可）

##### SaveData(key, jsonData)
ゲームデータを保存します

```csharp
public void SaveData(string key, string jsonData)
```

**Parameters:**
- `key`: 保存キー
- `jsonData`: JSON形式のデータ

##### LoadData(key)
ゲームデータを読み込みます

```csharp
public string LoadData(string key)
```

**Returns:** JSON文字列（データがない場合は空文字列）

##### GetProfile()
ユーザープロフィールを取得します

```csharp
public UserProfile GetProfile()
```

**Returns:** `UserProfile` オブジェクト

##### GetContext()
LIFF環境情報を取得します

```csharp
public LiffContext GetContext()
```

**Returns:** `LiffContext` オブジェクト

##### Close()
LIFFウィンドウを閉じます

```csharp
public void Close()
```

##### OpenLink(url)
外部リンクを開きます

```csharp
public void OpenLink(string url)
```

##### StartScan()
QRコードスキャンを開始します

```csharp
public void StartScan()
```

#### イベント

| イベント | パラメータ | 説明 |
|---------|-----------|------|
| `OnLiffInitialized` | UserProfile | LIFF初期化完了時 |
| `OnBridgeReady` | - | Bridge準備完了時 |
| `OnLiffError` | string | LIFFエラー発生時 |
| `OnShareSuccess` | - | 共有成功時 |
| `OnShareError` | string | 共有失敗時 |
| `OnSaveSuccess` | string | 保存成功時 |
| `OnSaveError` | string | 保存失敗時 |
| `OnLoadComplete` | string | 読み込み完了時 |
| `OnLoadError` | string | 読み込み失敗時 |
| `OnScanSuccess` | string | スキャン成功時 |
| `OnScanError` | string | スキャン失敗時 |

### UnityLiffBridge (JavaScript)

#### メソッド

##### initializeLIFF(liffId)
LIFF SDKを初期化します

```javascript
await window.unityLiffBridge.initializeLIFF('YOUR-LIFF-ID')
```

##### setUnityInstance(instance)
Unity WebGLインスタンスを登録します

```javascript
window.unityLiffBridge.setUnityInstance(unityInstance)
```

##### sendToUnity(methodName, message)
Unity側にメッセージを送信します

```javascript
window.unityLiffBridge.sendToUnity('OnCustomEvent', 'data')
```

##### shareScore(score, message)
スコアを共有します

```javascript
await window.unityLiffBridge.shareScore(1000, 'ハイスコア達成！')
```

##### saveGameData(key, data)
データを保存します

```javascript
window.unityLiffBridge.saveGameData('save1', {level: 5})
```

##### loadGameData(key)
データを読み込みます

```javascript
const data = window.unityLiffBridge.loadGameData('save1')
```

## 💡 サンプルコード

### 例1: スコアシステム

```csharp
public class ScoreManager : MonoBehaviour
{
    private LIFFBridge liffBridge;
    private int currentScore = 0;

    void Start()
    {
        liffBridge = FindObjectOfType<LIFFBridge>();
        liffBridge.OnBridgeReady += LoadSavedScore;
    }

    public void AddScore(int points)
    {
        currentScore += points;

        // 自動保存
        SaveScore();
    }

    void SaveScore()
    {
        var data = new { score = currentScore };
        liffBridge.SaveData("score", JsonUtility.ToJson(data));
    }

    void LoadSavedScore()
    {
        string json = liffBridge.LoadData("score");
        if (!string.IsNullOrEmpty(json))
        {
            var data = JsonUtility.FromJson<ScoreData>(json);
            currentScore = data.score;
        }
    }

    [System.Serializable]
    class ScoreData { public int score; }
}
```

### 例2: ランキング共有

```csharp
public class RankingSystem : MonoBehaviour
{
    private LIFFBridge liffBridge;

    void Start()
    {
        liffBridge = FindObjectOfType<LIFFBridge>();
    }

    public void ShareRanking(int rank, int score)
    {
        string message = $"🏆 ランキング {rank}位！\n" +
                        $"📊 スコア: {score}点\n" +
                        $"一緒にプレイしよう！";

        liffBridge.ShareGameScore(score, message);
    }
}
```

### 例3: マルチプレイヤー招待

```csharp
public class MultiplayerManager : MonoBehaviour
{
    private LIFFBridge liffBridge;
    private string roomId;

    public void CreateRoom()
    {
        roomId = System.Guid.NewGuid().ToString();
        InviteFriends();
    }

    void InviteFriends()
    {
        string message = $"🎮 マルチプレイに参加しよう！\n" +
                        $"ルームID: {roomId}";

        liffBridge.ShareGameScore(0, message);
    }
}
```

## 🔧 トラブルシューティング

### よくある問題

#### 1. LIFFが初期化されない

**症状:** `LIFF初期化エラー`がコンソールに表示される

**解決策:**
- LIFF IDが正しいか確認
- HTTPSで動作しているか確認
- LINEアプリ内または外部ブラウザで開いているか確認

#### 2. Unity側でメッセージを受信できない

**症状:** JavaScriptからのメッセージがUnityに届かない

**解決策:**
- GameObjectの名前が `LIFFBridge` であることを確認（大文字小文字も一致）
- `LIFFBridge.cs`がGameObjectにアタッチされているか確認
- コンソールでエラーがないか確認

#### 3. 共有機能が動作しない

**症状:** `shareTargetPicker APIが利用できません`

**解決策:**
- LINEアプリ内で開いているか確認（外部ブラウザでは使えない）
- LIFF設定でscopeに `chat_message.write` が含まれているか確認

#### 4. WebGLビルドが読み込まれない

**症状:** 白い画面のまま

**解決策:**
- ブラウザのコンソールでエラーを確認
- ビルドファイルのパスが正しいか確認
- CORSエラーの場合、サーバー設定を確認

#### 5. エディタでテストしたい

**症状:** WebGLビルドせずに動作確認したい

**解決策:**
- `#if UNITY_EDITOR` ブロック内のシミュレーションコードが動作します
- `SimulateLiffInitialized()` で擬似的なプロフィールが設定されます
- PlayerPrefsでデータ保存をシミュレート

### デバッグ方法

#### ブラウザコンソールの確認

```javascript
// LIFF状態確認
console.log(window.unityLiffBridge.liffInitialized);
console.log(window.unityLiffBridge.userProfile);

// Unity Instance確認
console.log(window.unityLiffBridge.unityInstance);
```

#### Unity側デバッグ

```csharp
void OnEnable()
{
    liffBridge.OnLiffError += (error) => {
        Debug.LogError($"[LIFF ERROR] {error}");
    };
}
```

## 📝 ベストプラクティス

### 1. エラーハンドリング

必ずエラーイベントを登録してユーザーにフィードバック:

```csharp
liffBridge.OnLiffError += (error) => {
    ShowErrorDialog($"エラー: {error}");
};
```

### 2. Bridge準備待機

Bridgeの準備完了を待ってから操作:

```csharp
if (liffBridge.isBridgeReady)
{
    // 操作実行
}
else
{
    liffBridge.OnBridgeReady += () => {
        // 準備完了後の操作
    };
}
```

### 3. データ同期

重要なデータは自動保存:

```csharp
void Update()
{
    if (Time.time - lastSaveTime > 60f) // 1分ごと
    {
        SaveGameData();
        lastSaveTime = Time.time;
    }
}
```

## 🎓 さらに学ぶ

### LINEミニアプリ関連
- [LINEミニアプリ ドキュメント](https://developers.line.biz/ja/docs/line-mini-app/)
- [LIFF APIリファレンス](https://developers.line.biz/ja/reference/liff/)
- [LINEミニアプリ統合のお知らせ](https://developers.line.biz/ja/news/2025/02/12/line-mini-app/)
- [MINIAPP_SETUP_GUIDE.md](MINIAPP_SETUP_GUIDE.md) - 詳細な設定ガイド

### Unity関連
- [Unity WebGL Documentation](https://docs.unity3d.com/Manual/webgl.html)
- [Unity WebGL と JavaScript の連携](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html)

## 📄 ライセンス

このプロジェクトは教育・学習目的で作成されています。

## 🤝 サポート

問題が発生した場合:
1. このドキュメントのトラブルシューティングを確認
2. ブラウザコンソールとUnityコンソールのエラーを確認
3. LIFF設定を再確認

---

**開発環境:** VSCode + Claude Code + Serena MCP
**最終更新:** 2025年10月
