# LIFF ID 設定 - クイックスタート

**5分で完了！** LIFF IDを取得してコードに設定するまでの最短手順です。

## 🚀 手順（全4ステップ）

### ステップ1: LINE Developers にログイン（1分）

1. https://developers.line.biz/console/ を開く
2. 「LINEアカウントでログイン」をクリック
3. LINEでログイン（スマホで認証）

### ステップ2: チャネルを作成（2分）

1. **「Create a new provider」** をクリック
   - Provider name: `MyProvider`（任意の名前）

2. **「Create a new channel」** をクリック
   - Channel type: **Messaging API** を選択

3. 必須項目を入力：
   ```
   Channel name: Unity Game
   Channel description: LIFF Game
   Category: Games
   Subcategory: Casual games
   Email: your@email.com
   ```

4. 利用規約に同意して **「Create」**

### ステップ3: LIFF アプリを追加（1分）

1. 作成したチャネルを開く

2. 上部タブの **「LIFF」** をクリック

3. **「Add」** ボタンをクリック

4. 設定を入力：
   ```
   LIFF app name: Unity Game
   Size: Full
   Endpoint URL: https://example.com （仮のURLでOK、後で変更可能）
   Scope: ✅ profile ✅ openid ✅ chat_message.write
   Scan QR: On
   ```

5. **「Add」** をクリック

### ステップ4: LIFF ID をコピーして設定（1分）

#### 1. LIFF ID をコピー

LIFFタブに表示される：
```
LIFF ID: 1234567890-abcdefgh
```
このIDをコピー（クリップボードにコピー）

#### 2. コードに貼り付け

**Unity C# ([LIFFBridge.cs:10](LIFFBridge.cs#L10)):**
```csharp
[SerializeField] private string liffId = "1234567890-abcdefgh"; // ← ここに貼り付け
```

**HTML ([unity-liff-template.html:184](unity-liff-template.html#L184)):**
```javascript
const CONFIG = {
    liffId: '1234567890-abcdefgh', // ← ここに貼り付け
```

**既存のスイカゲーム ([suika-liff.html](suika-liff.html)) も使う場合:**
```javascript
liff.init({
    liffId: '1234567890-abcdefgh' // ← ここに貼り付け
})
```

## ✅ 完了！

これでLIFF IDの設定は完了です。

---

## 📝 次のステップ

### Unityプロジェクトの場合

1. **Unityプロジェクトを作成**
2. **スクリプトを配置**:
   - `LIFFBridge.cs` → `Assets/Scripts/`
   - `GameManager.cs` → `Assets/Scripts/`

3. **WebGLテンプレートを設定**:
   ```
   Assets/WebGLTemplates/LIFFTemplate/
   ├── index.html (unity-liff-template.htmlをリネーム)
   └── unity-liff-bridge.js
   ```

4. **GameObject作成**:
   - Hierarchy で右クリック > Create Empty
   - 名前を `LIFFBridge` に変更
   - `LIFFBridge.cs` をアタッチ
   - Inspector で LIFF ID が正しいか確認

5. **WebGLビルド**:
   - File > Build Settings > WebGL
   - Player Settings > WebGL Template: `LIFFTemplate`
   - Build

### HTMLのみの場合（既存のスイカゲーム）

1. `suika-liff.html` を編集（LIFF ID設定済み）
2. HTTPSサーバーで起動:
   ```bash
   python https_server.py
   ```
3. ngrokでトンネル作成:
   ```bash
   ngrok http 8000
   ```
4. Endpoint URLを更新

---

## 🧪 テスト方法

### ローカルでテスト

```bash
# 1. サーバー起動
python https_server.py

# 2. ngrok起動（別ターミナル）
ngrok http 8000

# 3. ngrokのHTTPS URLをコピー
# 例: https://abc123.ngrok.io

# 4. LINE Developers で Endpoint URL を更新
# LIFF > Edit > Endpoint URL: https://abc123.ngrok.io

# 5. LIFF URL を開く
# https://liff.line.me/1234567890-abcdefgh
```

### 動作確認

LINEアプリで開いたら：

- ✅ ユーザー名が表示される
- ✅ アバター画像が表示される
- ✅ ゲームが正常に動く
- ✅ 共有ボタンが動く

---

## ⚠️ トラブルシューティング

### エラー: "Invalid liffId"

**原因:** LIFF IDが間違っている

**解決策:**
1. LINE Developers で LIFF ID を再確認
2. コピー＆ペーストミスがないか確認
3. `"` や `'` の前後にスペースがないか確認

### エラー: "LIFF初期化エラー"

**原因:** HTTPS でアクセスしていない

**解決策:**
- HTTPではなくHTTPSで開く
- ngrok を使う
- GitHub Pages などにデプロイ

### 画面が真っ白

**原因:** Endpoint URLが間違っている

**解決策:**
1. LINE Developers で Endpoint URL を確認
2. 実際にアクセスできるHTTPS URLか確認
3. ファイルパスが正しいか確認

---

## 📖 詳細ガイド

さらに詳しい情報は以下を参照：

- **[LIFF_SETUP_GUIDE.md](LIFF_SETUP_GUIDE.md)** - 完全な設定ガイド
- **[UNITY_INTEGRATION.md](UNITY_INTEGRATION.md)** - Unity統合ドキュメント
- **[agents.md](agents.md)** - プロジェクト全体の説明

---

## 💡 Tips

### LIFF ID を環境変数で管理

開発環境と本番環境で LIFF ID を分けたい場合：

**Unity:**
```csharp
#if UNITY_EDITOR
    private string liffId = "開発用LIFF-ID";
#else
    private string liffId = "本番用LIFF-ID";
#endif
```

**JavaScript:**
```javascript
const CONFIG = {
    liffId: process.env.LIFF_ID || '1234567890-abcdefgh'
};
```

### 複数のLIFFアプリを使い分け

用途別にLIFFアプリを作成：

```
Unity Game用:    1234567890-aaaaaaaa
スイカゲーム用:   1234567890-bbbbbbbb
テスト用:        1234567890-cccccccc
```

---

**作成日:** 2025年10月26日
**開発環境:** VSCode + Claude Code
