# GitHub Pages デプロイガイド（Unity WebGL + LINEミニアプリ）

Unity WebGLで作成したLINEミニアプリをGitHub Pagesで公開する方法です。

## ✅ GitHub Pagesの利点

- 🆓 **完全無料** - ホスティング費用なし
- 🔒 **HTTPS対応** - LINEミニアプリの必須要件を満たす
- 🚀 **簡単デプロイ** - git push だけで公開
- 🌐 **グローバルCDN** - 高速配信
- 📱 **カスタムドメイン対応** - 独自ドメインも使える

---

## 📋 前提条件

- ✅ GitHubアカウント
- ✅ Unityでビルド完了したWebGLファイル
- ✅ LINEミニアプリのチャネル作成済み

---

## 🚀 デプロイ手順

### ステップ1: GitHubリポジトリ作成

1. **GitHubにログイン**: https://github.com

2. **新規リポジトリ作成**:
   - 右上の `+` > `New repository`
   - Repository name: `suika-liff-game`（任意）
   - Public を選択
   - `Create repository` をクリック

### ステップ2: ローカルリポジトリ初期化

Unity WebGLビルドフォルダで：

```bash
# プロジェクトルートに移動
cd c:\Users\ishim\a001

# Gitリポジトリ初期化（まだの場合）
git init

# .gitignoreファイル作成
```

### ステップ3: .gitignore作成

Unity WebGLプロジェクト用の `.gitignore` を作成：

```bash
# プロジェクトルートで実行
cat > .gitignore << 'EOF'
# Unity
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Uu]ser[Ss]ettings/

# Visual Studio
.vs/
*.csproj
*.unityproj
*.sln
*.suo
*.tmp
*.user
*.userprefs
*.pidb
*.booproj
*.svd
*.pdb
*.mdb
*.opendb
*.VC.db

# OS
.DS_Store
Thumbs.db
desktop.ini

# Python
__pycache__/
*.py[cod]
.venv/

# セキュリティ
*.pem
*.key
server.crt
.env

# BUT: Buildフォルダは公開するので除外しない
# （通常はBuildを.gitignoreに入れるが、GitHub Pagesでは必要）
EOF
```

### ステップ4: Unity WebGLビルドを配置

#### オプションA: Build フォルダをそのまま使う

```bash
# Unityでビルドしたファイルを確認
ls Build/

# 必要なファイル:
# - index.html
# - unity-liff-bridge.js
# - Build/UnityBuild.loader.js
# - Build/UnityBuild.data
# - Build/UnityBuild.framework.js
# - Build/UnityBuild.wasm
```

#### オプションB: ルートに配置（推奨）

GitHub Pagesはルートの `index.html` を自動で表示します：

```bash
# Buildフォルダの中身をルートにコピー
cp Build/index.html ./
cp Build/unity-liff-bridge.js ./

# Buildフォルダはそのまま
# （Build/UnityBuild.* ファイルが必要）
```

### ステップ5: パス調整

**index.htmlのパス確認** （必要に応じて修正）:

```javascript
// index.html 内で確認
const CONFIG = {
    liffId: 'YOUR-LIFF-ID',  // ← 設定済みか確認
    dataUrl: 'Build/UnityBuild.data',         // ← パス確認
    frameworkUrl: 'Build/UnityBuild.framework.js',
    codeUrl: 'Build/UnityBuild.wasm',
    // ...
};
```

### ステップ6: Gitにコミット

```bash
# ファイルを追加
git add .

# コミット
git commit -m "🎮 Add Unity WebGL LIFF game"

# リモートリポジトリを追加（GitHubのURLに置き換え）
git remote add origin https://github.com/YOUR-USERNAME/suika-liff-game.git

# プッシュ
git branch -M main
git push -u origin main
```

### ステップ7: GitHub Pagesを有効化

1. **GitHubリポジトリページを開く**

2. **Settings タブ** をクリック

3. 左メニューから **Pages** を選択

4. **Source** セクション:
   - Branch: `main` を選択
   - Folder: `/` (root) または `/Build` を選択
   - **Save** をクリック

5. **数分待つ** と、以下のようなURLが表示されます:
   ```
   Your site is published at https://YOUR-USERNAME.github.io/suika-liff-game/
   ```

### ステップ8: LIFF Endpoint URL を更新

1. **LINE Developers Console** を開く: https://developers.line.biz/console/

2. チャネル > **LIFF** タブ

3. LIFF アプリを選択 > **Edit**

4. **Endpoint URL** を更新:
   ```
   https://YOUR-USERNAME.github.io/suika-liff-game/
   ```

5. **Update** をクリック

### ステップ9: テスト

1. **LIFF URL を開く**:
   ```
   https://liff.line.me/YOUR-LIFF-ID
   ```

2. **LINEアプリで開く** または **外部ブラウザで開く**

3. 動作確認:
   - ✅ ゲームが読み込まれる
   - ✅ ユーザー名・アバターが表示される
   - ✅ 共有機能が動作する

---

## 📁 推奨フォルダ構造

### パターンA: ルート配置（推奨）

```
Repository Root/
├── index.html                  # メインHTML
├── unity-liff-bridge.js        # LIFF Bridge
├── Build/                      # Unityビルドファイル
│   ├── UnityBuild.loader.js
│   ├── UnityBuild.data
│   ├── UnityBuild.framework.js
│   └── UnityBuild.wasm
├── .gitignore
├── README.md
└── ...
```

**GitHub Pages URL**: `https://username.github.io/repo-name/`

### パターンB: Buildフォルダ配置

```
Repository Root/
├── Build/                      # すべてここに
│   ├── index.html
│   ├── unity-liff-bridge.js
│   ├── Build/
│   │   ├── UnityBuild.loader.js
│   │   └── ...
├── .gitignore
├── README.md
└── ...
```

**GitHub Pages設定**: Folder を `/Build` に設定
**GitHub Pages URL**: `https://username.github.io/repo-name/`

---

## 🔧 トラブルシューティング

### 問題1: 404 Not Found

**原因**: ファイルパスが間違っている

**解決策**:
1. GitHub Pages の設定を確認（root または /Build）
2. index.html が正しい場所にあるか確認
3. リポジトリのファイル構造を確認

### 問題2: ゲームが読み込まれない

**原因**: Unityビルドファイルのパスが間違っている

**解決策**:

index.html のパスを修正：

```javascript
// 絶対パスから相対パスに変更
const CONFIG = {
    dataUrl: 'Build/UnityBuild.data',  // ✅ 相対パス
    // NOT: '/Build/UnityBuild.data'   // ❌ 絶対パス（ルート配置時以外NG）
};
```

またはブラウザのDevToolsでエラーを確認：
- F12 > Console タブ
- Network タブで 404 エラーがないか確認

### 問題3: LINEミニアプリが動かない

**原因**: LIFF初期化エラー

**解決策**:
1. LIFF ID が正しく設定されているか確認
2. `unity-liff-bridge.js` が読み込まれているか確認
3. ブラウザコンソールでエラーをチェック

### 問題4: 共有機能が動かない

**原因**: LINEアプリ内で開いていない

**解決策**:
- LIFF URL（`https://liff.line.me/...`）からアクセス
- LINEの友達に送るか、Keep にメモして開く
- 外部ブラウザでは共有機能は使えません

### 問題5: ファイルサイズが大きすぎる

**原因**: Unity WebGLビルドが大きい

**解決策**:

**Unity側で圧縮を有効化**:
1. **Edit > Project Settings > Player**
2. **Publishing Settings**
3. **Compression Format**: `Gzip` または `Brotli` を選択
4. 再ビルド

**GitHub Pages の制限**:
- リポジトリサイズ: 推奨 1GB 以下
- ファイルサイズ: 100MB 以下（推奨）

---

## 🎨 カスタムドメイン（オプション）

独自ドメインを使いたい場合：

### ステップ1: ドメインを取得

- お名前.com、ムームードメインなど

### ステップ2: DNS設定

Aレコードを追加：

```
Type: A
Name: @
Value: 185.199.108.153
Value: 185.199.109.153
Value: 185.199.110.153
Value: 185.199.111.153
```

CNAMEレコード（サブドメインの場合）：

```
Type: CNAME
Name: game
Value: YOUR-USERNAME.github.io
```

### ステップ3: GitHub Pages設定

1. Settings > Pages
2. **Custom domain** に独自ドメインを入力
3. **Save**
4. **Enforce HTTPS** をチェック

### ステップ4: LIFF Endpoint URL を更新

```
https://yourdomain.com/
```

---

## 🔄 更新方法

コードを変更したら：

```bash
# Unityで再ビルド

# 変更をコミット
git add .
git commit -m "🔧 Update game logic"

# プッシュ（自動的にGitHub Pagesが更新される）
git push
```

数分で反映されます。

---

## 📊 アクセス解析（オプション）

### Google Analytics追加

index.html の `<head>` に追加：

```html
<!-- Google Analytics -->
<script async src="https://www.googletagmanager.com/gtag/js?id=G-XXXXXXXXXX"></script>
<script>
  window.dataLayer = window.dataLayer || [];
  function gtag(){dataLayer.push(arguments);}
  gtag('js', new Date());
  gtag('config', 'G-XXXXXXXXXX');
</script>
```

---

## ✅ デプロイ完了チェックリスト

- [ ] GitHubリポジトリ作成
- [ ] Unity WebGLビルドをコミット
- [ ] GitHub Pagesを有効化
- [ ] 公開URLが表示される
- [ ] LINE Developers で Endpoint URL を更新
- [ ] LIFF URL でアクセスできる
- [ ] ゲームが正常に動作する
- [ ] ユーザー情報が表示される
- [ ] 共有機能が動作する（LINEアプリ内）

---

## 🎯 まとめ

GitHub Pagesを使えば：

- ✅ **完全無料** でLINEミニアプリを公開
- ✅ **HTTPS対応** で安全
- ✅ **簡単デプロイ** git push だけ
- ✅ **グローバル配信** CDN経由で高速

他のホスティングサービスも検討できます：
- **Netlify**: ビルドの自動化が便利
- **Vercel**: Next.jsなどに最適
- **Firebase Hosting**: Googleのサービス

---

## 📖 関連ドキュメント

- [MINIAPP_SETUP_GUIDE.md](MINIAPP_SETUP_GUIDE.md) - LINEミニアプリ設定
- [UNITY_INTEGRATION.md](UNITY_INTEGRATION.md) - Unity統合ガイド
- [UNITY_SETUP_FIX.md](UNITY_SETUP_FIX.md) - ビルドエラー解決
- [GitHub Pages 公式ドキュメント](https://docs.github.com/ja/pages)

---

**作成日:** 2025年10月26日
**対象:** Unity WebGL + LINEミニアプリ
**ホスティング:** GitHub Pages
