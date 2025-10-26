# LINEミニアプリ 設定ガイド（2025年版）

LINE Front-end Framework (LIFF) を使うためのLINEミニアプリID取得から設定までの完全ガイドです。

> **重要**: 2025年2月、LINEはLIFFをLINEミニアプリに統合することを発表しました。
> このガイドは最新の「LINEミニアプリ」方式に対応しています。

## 📋 目次

1. [LINEミニアプリとは](#lineミニアプリとは)
2. [前提条件](#前提条件)
3. [LINE Developersアカウント作成](#line-developersアカウント作成)
4. [LINEミニアプリチャネルの作成](#lineミニアプリチャネルの作成)
5. [LIFF アプリの設定](#liffアプリの設定)
6. [LIFF ID の取得](#liff-idの取得)
7. [コードへの設定](#コードへの設定)
8. [テストと確認](#テストと確認)
9. [LIFF vs ミニアプリ](#liff-vs-ミニアプリ)
10. [よくある質問](#よくある質問)

---

## 🎯 LINEミニアプリとは

### 概要

LINEミニアプリ(LINE Mini App)は、LINEプラットフォーム上で動作するWebアプリケーションです。
従来のLIFF (LINE Front-end Framework)の後継として、2025年に統合されました。

### 主な特徴

- **LINEアプリ内で動作**: ネイティブアプリのような体験
- **外部ブラウザ対応**: 2025年10月から外部ブラウザでも利用可能
- **LIFF SDK使用**: 既存のLIFF APIをそのまま使用
- **LINE連携**: ユーザープロフィール、友達、トーク連携

### 統合タイムライン

| 時期 | 変更内容 |
|------|---------|
| **2025年2月** | LIFFのミニアプリへの統合を発表 |
| **2025年10月** | 外部ブラウザでミニアプリ利用開始 |
| **2025年後半** | LIFF→ミニアプリ移行ツール提供予定 |

---

## 📱 前提条件

以下が必要です：

- ✅ **LINEアカウント**: 個人のLINEアカウント（スマホアプリで使用中のもの）
- ✅ **メールアドレス**: LINE Developersアカウント登録用
- ✅ **HTTPSサーバー**: ミニアプリをホストするURL（後で設定可能）

> **注意**: LINEミニアプリは**HTTPS必須**です。HTTPでは動作しません。

---

## 🔐 LINE Developersアカウント作成

### ステップ 1: LINE Developers Console にアクセス

1. ブラウザで以下のURLを開く:
   ```
   https://developers.line.biz/console/
   ```

2. **「LINEアカウントでログイン」** をクリック

3. LINEアカウントの **メールアドレス** と **パスワード** を入力

4. スマホのLINEアプリに**認証通知**が届くので、**「ログイン」**をタップ

### ステップ 2: 開発者登録

初回ログイン時は開発者登録が必要です：

1. **名前** を入力（開発者名として表示されます）
2. **メールアドレス** を入力（通知受信用）
3. 利用規約に同意して**「作成」**をクリック

### ステップ 3: プロバイダー作成

プロバイダーは、あなたのアプリやサービスを管理する「組織」です。

1. LINE Developers Console のトップページで **「Create a new provider」** をクリック

2. **プロバイダー名** を入力
   - 例: `My Game Studio`, `個人開発`, `Test Provider`
   - 後から変更可能です

3. **「Create」** をクリック

---

## 🎮 LINEミニアプリチャネルの作成

### 方法A: 直接LINEミニアプリを作成（推奨）

1. 作成したプロバイダーをクリック

2. **「Create a new channel」** をクリック

3. チャネルタイプで **「LINEミニアプリ」** を選択

   ```
   ┌──────────────────────────────┐
   │ チャネルタイプを選択           │
   ├──────────────────────────────┤
   │ ○ Messaging API             │
   │ ○ LINE Login                │
   │ ● LINEミニアプリ             │  ← これ！
   └──────────────────────────────┘
   ```

4. 以下の情報を入力：

   | 項目 | 入力例 | 説明 |
   |------|--------|------|
   | **チャネル名** | `Unity LIFF Game` | チャネルの名前 |
   | **チャネル説明** | `スイカゲーム ミニアプリ` | 説明文 |
   | **カテゴリ** | `ゲーム` | カテゴリ選択 |
   | **サブカテゴリ** | `カジュアルゲーム` | サブカテゴリ |
   | **メールアドレス** | your@email.com | 連絡先 |
   | **サービス提供地域** | `日本` | 提供地域 |

5. プライバシーポリシーと利用規約のURL（開発中は空欄でOK）

6. 利用規約に同意して **「作成」** をクリック

### 方法B: Messaging API経由で作成

地域によってはLINEミニアプリが直接選択できない場合があります。
その場合は以下の手順を使用してください：

1. チャネルタイプで **「Messaging API」** を選択
2. 同様に情報を入力して作成
3. チャネル作成後、**「LIFF」タブ**から設定（後述）

---

## 🔧 LIFF アプリの設定

チャネル作成後、LIFF アプリを追加します。

### ステップ 1: LIFF タブを開く

1. 作成したチャネルをクリック

2. 上部のタブから **「LIFF」** をクリック

   ```
   [Basic settings] [Messaging API] [LIFF] [LINE Login] ...
                                      ^^^^
   ```

### ステップ 2: LIFF アプリを追加

1. **「Add」** ボタンをクリック

2. 以下の設定を入力：

#### 基本設定

| 項目 | 設定値 | 説明 |
|------|--------|------|
| **LIFF app name** | `Unity LIFF Game` | LIFF アプリの名前 |
| **Size** | `Full` | 画面サイズ（フルスクリーン推奨） |
| **Endpoint URL** | `https://your-domain.com` | アプリのURL（後で変更可能） |
| **Scope** | ✅ `profile`<br>✅ `openid`<br>✅ `chat_message.write` | 必要な権限 |
| **Bot link feature** | `On (Normal)` | LINEボットとの連携（任意） |
| **Scan QR** | `On` | QRスキャン機能を使う場合 |

#### 詳細説明

**Size (画面サイズ):**
- `Full`: 全画面表示（ゲームに最適）⭐ 推奨
- `Tall`: 縦長画面（75%）
- `Compact`: コンパクト（50%）

**Endpoint URL:**
- 開発中の設定例:
  - ❌ `http://localhost:8000` - HTTP不可
  - ❌ `https://localhost:8000` - LINEアプリからアクセス不可
  - ✅ `https://abc123.ngrok.io` - ngrok経由でOK
  - ✅ `https://example.com` - 仮URLでもOK（後で変更）

- 本番環境の例:
  - `https://yourgame.netlify.app`
  - `https://username.github.io/repo-name`
  - `https://your-domain.com`

**Scope (権限):**
- `profile`: ユーザー名、アバター画像の取得（**必須**）
- `openid`: ユーザーIDの取得（**必須**）
- `chat_message.write`: メッセージ送信、シェア機能（共有機能を使う場合）

3. **「Add」** をクリックしてLIFF アプリを作成

---

## 🔑 LIFF ID の取得

### LIFF アプリ作成後

1. LIFF タブに作成したアプリが表示されます

2. **LIFF ID** が表示されています：
   ```
   LIFF ID: 1234567890-abcdefgh
            ^^^^^^^^^^^^^^^^^^^^
            これをコピーします
   ```

3. LIFF ID の形式:
   ```
   [10桁の数字]-[8桁の英数字]
   例: 2008275057-VqJkXjxy
   ```

4. **LIFF ID をコピー**してメモ帳などに保存

### LIFF URL も確認

LIFF URL も表示されています：
```
LIFF URL: https://liff.line.me/1234567890-abcdefgh
```

このURLがあなたのLINEミニアプリの公式URLです。

### Channel ID も取得（参考）

**「Basic settings」** タブで Channel ID も確認できます：
```
Channel ID: 1234567890
```

これはチャネルの識別子で、LIFF IDとは異なります。

---

## 💻 コードへの設定

取得したLIFF IDを、以下の箇所に設定します。

### 自動設定（推奨）

Pythonスクリプトで一括設定：

```bash
python setup_liff_id.py 1234567890-abcdefgh
```

これで以下のファイルが自動更新されます：
- ✅ `LIFFBridge.cs`
- ✅ `unity-liff-template.html`
- ✅ `suika-liff.html`
- ✅ `.env`

### 手動設定

#### 1. LIFFBridge.cs (Unity C#)

ファイルを開いて、`liffId` フィールドを変更：

**変更前:**
```csharp
[Header("LIFF Settings")]
[SerializeField] private string liffId = "2008275057-xxx"; // ← 変更前
```

**変更後:**
```csharp
[Header("LIFF Settings")]
[SerializeField] private string liffId = "1234567890-abcdefgh"; // ✅ あなたのLIFF ID
```

#### 2. unity-liff-template.html (Unity WebGL)

ファイルを開いて、`CONFIG.liffId` を変更：

**変更前:**
```javascript
const CONFIG = {
    liffId: '2008275057-xxx', // ← 変更前
    unityBuildPath: 'Build',
    // ...
};
```

**変更後:**
```javascript
const CONFIG = {
    liffId: '1234567890-abcdefgh', // ✅ あなたのLIFF ID
    unityBuildPath: 'Build',
    // ...
};
```

#### 3. suika-liff.html (既存ゲーム、必要な場合)

**変更前:**
```javascript
liff.init({
    liffId: '2008275057-xxx' // ← 変更前
})
```

**変更後:**
```javascript
liff.init({
    liffId: '1234567890-abcdefgh' // ✅ あなたのLIFF ID
})
```

---

## ✅ 設定確認チェックリスト

すべて完了したか確認：

- [ ] LINE Developersアカウントを作成した
- [ ] プロバイダーを作成した
- [ ] LINEミニアプリチャネルを作成した
- [ ] LIFF アプリを追加した
- [ ] LIFF ID をコピーした
- [ ] `LIFFBridge.cs` に LIFF ID を設定した
- [ ] `unity-liff-template.html` に LIFF ID を設定した
- [ ] （必要なら）`suika-liff.html` に LIFF ID を設定した

---

## 🧪 テストと確認

### ローカル開発でのテスト

#### 方法1: ngrok を使う（推奨）

1. **ngrok をインストール:**
   ```bash
   # Windows (Scoop)
   scoop install ngrok

   # または公式サイトからダウンロード
   # https://ngrok.com/download
   ```

2. **ローカルサーバーを起動:**
   ```bash
   # Pythonサーバー
   python https_server.py

   # または Node.js
   npx http-server -p 8000
   ```

3. **ngrok でトンネルを作成:**
   ```bash
   ngrok http 8000
   ```

4. **表示されたHTTPS URLをコピー:**
   ```
   Forwarding   https://abcd1234.ngrok.io -> http://localhost:8000
                ^^^^^^^^^^^^^^^^^^^^^^^^
                このURLをコピー
   ```

5. **LINE Developers で Endpoint URL を更新:**
   - チャネル > LIFF タブ > アプリを選択 > **Edit**
   - Endpoint URL に ngrok の URL を貼り付け
   - **Update** をクリック

6. **LINEアプリでテスト:**
   - LIFF URL（`https://liff.line.me/あなたのLIFF-ID`）をコピー
   - LINEの友達に送る、または自分のLINE「Keep」にメモ
   - URLをタップしてアプリを開く

#### 方法2: GitHub Pages（本番環境）

1. **GitHubにリポジトリを作成**

2. **ビルドファイルをプッシュ:**
   ```bash
   git add Build/
   git commit -m "Add WebGL build"
   git push
   ```

3. **GitHub Pages を有効化:**
   - リポジトリの Settings > Pages
   - Source: `main` ブランチ
   - Folder: `/` (root) または `/Build`
   - Save

4. **公開URLを取得:**
   ```
   https://username.github.io/repository-name/
   ```

5. **LINE Developers で Endpoint URL を設定**

### テスト項目

LINEミニアプリを開いたら、以下を確認：

- [ ] アプリが正常に読み込まれる
- [ ] ローディング画面が表示される
- [ ] ユーザー名とアバターが表示される
- [ ] ブラウザコンソールに初期化成功ログが出る
- [ ] スコア共有ボタンが動作する
- [ ] データ保存/読み込みが動作する

### デバッグ方法

#### スマホでブラウザコンソールを見る

**Android:**
1. PCとスマホをUSBで接続
2. Chrome で `chrome://inspect` を開く
3. LINEアプリ内のページを選択
4. DevTools でコンソールを確認

**iOS (Safari + Mac):**
1. iPhoneで「設定」>「Safari」>「詳細」>「Webインスペクタ」をON
2. Macで Safari > 開発 > [あなたのiPhone] > ミニアプリページを選択
3. Webインスペクタでコンソールを確認

---

## 🔄 LIFF vs ミニアプリ

### 統合の背景

2025年2月、LINEはLIFFとLINEミニアプリのブランド統合を発表しました。

### 主な違い

| 項目 | LIFF（旧方式） | LINEミニアプリ（新方式） |
|------|--------------|----------------------|
| **チャネルタイプ** | Messaging API > LIFF | LINEミニアプリ |
| **アプリ数** | チャネルあたり複数可能 | チャネルあたり1つ |
| **外部ブラウザ** | 一部対応 | 2025年10月〜全面対応 |
| **モジュールモード** | 対応 | 非対応 |
| **LIFF SDK** | v2 | **同じ** v2 |
| **LIFF API** | すべて使用可能 | **同じ**、すべて使用可能 |
| **コード** | LIFF SDK使用 | **変更不要** |

### 開発者への影響

**良いニュース**: コード側は**ほぼ変更なし**！

- ✅ LIFF SDK v2 をそのまま使用
- ✅ `liff.init()` の使い方は同じ
- ✅ `liff.getProfile()` など既存API使用可能
- ✅ 既存のLIFFアプリのコードがそのまま動く

**変更点**: チャネル作成時に選択肢が異なるだけ

### 移行について

- **既存のLIFFアプリ**: 引き続き利用可能、移行は任意
- **新規開発**: LINEミニアプリとして作成推奨
- **移行ツール**: 2025年後半に提供予定

---

## ❓ よくある質問

### Q1: LIFF ID はどこで確認できますか？

**A:** LINE Developers Console > チャネル > LIFF タブ で確認できます。

### Q2: 複数のミニアプリを作れますか？

**A:** 1チャネルにつき1つのミニアプリです。複数必要な場合は、複数のチャネルを作成してください。

### Q3: LIFF ID を間違えて設定したらどうなりますか？

**A:** 初期化エラーが発生します。ブラウザコンソールに以下のようなエラーが出ます：
```
LIFF初期化エラー: Invalid liffId
```
正しいLIFF IDに修正して再読み込みしてください。

### Q4: Endpoint URL は後から変更できますか？

**A:** はい、いつでも変更可能です。LIFF タブ > Edit から変更してください。

### Q5: HTTPで動作しますか？

**A:** いいえ、LINEミニアプリは **HTTPS 必須** です。HTTP では動作しません。

### Q6: ローカルホスト（localhost）でテストできますか？

**A:** ブラウザから直接開く場合はテストできますが、LINEアプリからは開けません。ngrok などのトンネリングツールが必要です。

### Q7: ミニアプリを削除したいのですが？

**A:** LIFF タブ > アプリを選択 > Delete で削除できます。

### Q8: 既存のLIFFアプリはどうなりますか？

**A:** 引き続き利用可能です。2025年後半に移行ツールが提供される予定ですが、移行は任意です。

### Q9: 外部ブラウザ対応はいつから？

**A:** 2025年10月から、すべてのユーザーが外部ブラウザでミニアプリを利用できるようになります。

### Q10: 審査は必要ですか？

**A:**
- **未認証ミニアプリ**: 審査不要、すぐに使える（テスト・社内利用向け）
- **認証済みミニアプリ**: 審査必要、一般公開可能

---

## 🔗 参考リンク

- [LINE Developers Console](https://developers.line.biz/console/)
- [LINEミニアプリ ドキュメント](https://developers.line.biz/ja/docs/line-mini-app/)
- [LIFF API リファレンス](https://developers.line.biz/ja/reference/liff/)
- [LINEミニアプリ統合のお知らせ](https://developers.line.biz/ja/news/2025/02/12/line-mini-app/)
- [ngrok 公式サイト](https://ngrok.com/)
- [GitHub Pages ドキュメント](https://docs.github.com/ja/pages)

---

**作成日:** 2025年10月26日
**対応バージョン:** LINEミニアプリ 2025年版
**開発環境:** VSCode + Claude Code + Serena MCP
