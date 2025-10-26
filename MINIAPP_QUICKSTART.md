# LINEミニアプリ設定 - クイックスタート

**5分で完了！** LINEミニアプリ IDを取得してコードに設定するまでの最短手順です。

> **2025年最新版**: LIFFはLINEミニアプリに統合されました。このガイドは最新の方式に対応しています。

## 🚀 手順（全4ステップ）

### ステップ1: LINE Developers にログイン（1分）

1. https://developers.line.biz/console/ を開く
2. 「LINEアカウントでログイン」をクリック
3. LINEでログイン（スマホで認証）

### ステップ2: プロバイダーを作成（30秒）

初回のみ必要です。2回目以降はスキップできます。

1. **「Create a new provider」** をクリック
2. Provider name: `MyProvider`（任意の名前）
3. **「Create」** をクリック

### ステップ3: LINEミニアプリチャネルを作成（2分）

1. 作成したプロバイダーをクリック

2. **「Create a new channel」** をクリック

3. チャネルタイプで **「LINEミニアプリ」** を選択 ⭐
   ```
   ┌─────────────────────────┐
   │ ○ Messaging API         │
   │ ○ LINE Login            │
   │ ● LINEミニアプリ         │  ← これを選択！
   └─────────────────────────┘
   ```

4. 必須項目を入力：
   ```
   チャネル名: Unity Game
   チャネル説明: Unity LIFF Game
   カテゴリ: ゲーム
   サブカテゴリ: カジュアルゲーム
   メールアドレス: your@email.com
   ```

5. サービス提供地域を選択（例: 日本）

6. 利用規約に同意して **「作成」** をクリック

### ステップ4: ミニアプリ ID をコピーして設定（1分）

#### 1. ミニアプリ ID をコピー

チャネルが作成されると、自動的にミニアプリIDが発行されます：

1. 作成したチャネルをクリック
2. **「Basic settings」** タブを開く
3. **「Channel ID」** をコピー

   または

1. チャネル一覧で ID を確認
   ```
   Channel ID: 1234567890
   ```

> **注意**: ミニアプリIDは従来のLIFF IDと異なり、**10桁の数字のみ**です。

#### 2. エンドポイントURLを設定

1. チャネル設定画面で **「LIFF」** タブをクリック
2. **「Add」** ボタンをクリック
3. 以下を設定：
   ```
   LIFF app name: Unity Game
   Size: Full
   Endpoint URL: https://example.com （仮のURLでOK、後で変更可能）
   Scope: ✅ profile ✅ openid ✅ chat_message.write
   Scan QR: On
   ```
4. **「Add」** をクリック
5. 表示される **LIFF ID** をコピー
   ```
   LIFF ID: 1234567890-abcdefgh
   ```

#### 3. コードに貼り付け

**Unity C# ([LIFFBridge.cs:10](LIFFBridge.cs#L10)):**
```csharp
[SerializeField] private string liffId = "1234567890-abcdefgh"; // ← ここに貼り付け
```

**HTML ([unity-liff-template.html:184](unity-liff-template.html#L184)):**
```javascript
const CONFIG = {
    liffId: '1234567890-abcdefgh', // ← ここに貼り付け
```

**既存のスイカゲーム ([suika-liff.html](suika-liff.html)):**
```javascript
liff.init({
    liffId: '1234567890-abcdefgh' // ← ここに貼り付け
})
```

## ✅ 完了！

これでLINEミニアプリの設定は完了です。

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
# チャネル > LIFF > Edit > Endpoint URL: https://abc123.ngrok.io

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
- HTTPではなくHTTPS で開く
- ngrok を使う
- GitHub Pages などにデプロイ

### チャネルタイプに「LINEミニアプリ」がない

**原因:** 地域やアカウントの設定による制限

**解決策:**
1. Messaging API チャネルを作成
2. その後 LIFF タブから追加
3. 結果は同じです

---

## 🆕 LINEミニアプリとLIFFの違い

### 統合について

- **2025年2月発表**: LIFFはLINEミニアプリに統合
- **既存のLIFFアプリ**: 引き続き利用可能
- **新規開発**: LINEミニアプリとして作成推奨

### 主な違い

| 項目 | LIFF（旧） | LINEミニアプリ（新） |
|------|-----------|-------------------|
| チャネルあたりのアプリ数 | 複数可能 | 1つのみ |
| 外部ブラウザ対応 | 一部のみ | 2025年10月〜全面対応 |
| コード | LIFF SDK v2 | **同じ** LIFF SDK v2 |
| API | LIFF API | **同じ** LIFF API |

### 開発者への影響

**良いニュース**: コード側はほぼ変更なし！

- ✅ LIFF SDK v2 をそのまま使用
- ✅ `liff.init()` の使い方は同じ
- ✅ 既存のコードがそのまま動く

**変更点**: チャネル作成時に「LINEミニアプリ」を選択するだけ

---

## 💡 自動設定スクリプト

手動設定が面倒な場合、自動設定スクリプトを使用できます：

```bash
python setup_liff_id.py 1234567890-abcdefgh
```

このコマンド1つで：
- ✅ `LIFFBridge.cs` 更新
- ✅ `unity-liff-template.html` 更新
- ✅ `suika-liff.html` 更新
- ✅ `.env` ファイル作成

---

## 📖 詳細ガイド

さらに詳しい情報は以下を参照：

- **[MINIAPP_SETUP_GUIDE.md](MINIAPP_SETUP_GUIDE.md)** - 完全な設定ガイド
- **[UNITY_INTEGRATION.md](UNITY_INTEGRATION.md)** - Unity統合ドキュメント
- **[agents.md](agents.md)** - プロジェクト全体の説明

---

## 🔗 公式リンク

- [LINE Developers Console](https://developers.line.biz/console/)
- [LINEミニアプリドキュメント](https://developers.line.biz/ja/docs/line-mini-app/)
- [LIFF APIリファレンス](https://developers.line.biz/ja/reference/liff/)
- [LINEミニアプリ統合のお知らせ](https://developers.line.biz/ja/news/2025/02/12/line-mini-app/)

---

**作成日:** 2025年10月26日
**対応バージョン:** LINEミニアプリ 2025年版
**開発環境:** VSCode + Claude Code
