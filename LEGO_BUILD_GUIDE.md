# LEGO Microgame - WebGLビルドガイド

## 📋 概要

Unity LEGO MicrogameをWebGLビルドしてGitHub Pagesにデプロイする手順です。

## 📁 プロジェクト情報

- **プロジェクトパス**: `D:\unity project\lego001`
- **Unity バージョン**: 2021.3+ 推奨
- **ビルドターゲット**: WebGL

## 🛠️ ビルド手順

### 1. Unityプロジェクトを開く

1. Unity Hubを起動
2. **Add** → **Add project from disk**
3. `D:\unity project\lego001` を選択
4. プロジェクトをダブルクリックして開く

### 2. Build Settingsを開く

1. メニュー: **File** → **Build Settings**
2. Platform: **WebGL** を選択
3. まだWebGLがインストールされていない場合:
   - **Install with Unity Hub** をクリック
   - Unity Hubで WebGL Build Support をインストール

### 3. Player Settingsを設定

**Build Settings** ウィンドウで **Player Settings** をクリック

#### Company/Product名
- **Company Name**: Unity (または任意)
- **Product Name**: LEGO Microgame
- **Version**: 1.0.0

#### Resolution and Presentation
- **Default Canvas Width**: 1080
- **Default Canvas Height**: 1920
- **Run In Background**: ✓ ON

#### WebGL Template
- **Template**: Minimal (または Default)

#### Publishing Settings
- **Compression Format**: Disabled (GitHub Pages用)
  - または Gzip (`.gz`ファイル生成)
- **Exception Support**: Explicitly Thrown Exceptions Only
- **Enable Exceptions**: None (軽量化)

#### Quality Settings
- **Quality**: Medium ～ High
- 必要に応じて調整

### 4. シーンを追加

1. **Build Settings** ウィンドウで **Add Open Scenes** をクリック
2. または、以下のシーンを手動で追加:
   - `Assets/LEGO/Scenes/Microgame - Cannonball Bingo.unity`
   - その他必要なシーン

### 5. ビルド実行

1. **Build** ボタンをクリック
2. 保存先フォルダを選択:
   ```
   D:\unity project\lego001\WebGL_Build
   ```
3. **フォルダーの選択** をクリック
4. ビルド完了を待つ（数分～10分程度）

### 6. ビルドファイルの確認

ビルド完了後、以下のファイルが生成されます：

```
WebGL_Build/
├── Build/
│   ├── lego001.data.gz (または .data)
│   ├── lego001.framework.js.gz (または .framework.js)
│   ├── lego001.loader.js
│   └── lego001.wasm.gz (または .wasm)
├── TemplateData/
│   ├── favicon.ico
│   ├── MemoryProfiler.png
│   └── style.css
└── index.html
```

### 7. ファイル名の確認と調整

ビルドファイル名が `lego001.*` でない場合、`lego-game.html` の設定を更新してください：

```javascript
const CONFIG = {
    dataUrl: 'Build/[実際のファイル名].data',
    frameworkUrl: 'Build/[実際のファイル名].framework.js',
    codeUrl: 'Build/[実際のファイル名].wasm',
    // ...
};
```

## 📦 デプロイ準備

### GitHub Pages用の準備

#### 1. 圧縮ファイルの展開（必要な場合）

GitHub Pagesは `.gz` ファイルを自動展開しないため、非圧縮版も用意します：

```bash
cd D:/unity\ project/lego001/WebGL_Build/Build/
gunzip -c lego001.data.gz > lego001.data
gunzip -c lego001.framework.js.gz > lego001.framework.js
gunzip -c lego001.wasm.gz > lego001.wasm
```

#### 2. ファイルのコピー

ビルドフォルダを `suika-liff-game` プロジェクトにコピー：

```bash
# Build フォルダのみコピー
cp -r D:/unity\ project/lego001/WebGL_Build/Build/* C:/Users/ishim/a001/Build/
```

または、個別にコピー：

```bash
# Windows (PowerShell)
Copy-Item "D:\unity project\lego001\WebGL_Build\Build\*" -Destination "C:\Users\ishim\a001\Build\" -Recurse

# Git Bash / Linux / macOS
cp -r "D:/unity project/lego001/WebGL_Build/Build/"* "C:/Users/ishim/a001/Build/"
```

## 🚀 デプロイ手順

### 1. ファイルをGitに追加

```bash
cd C:/Users/ishim/a001
git add Build/lego001.*
git add lego-game.html
git add menu.html
```

### 2. コミット

```bash
git commit -m "🧱 Add LEGO Microgame WebGL build"
```

### 3. プッシュ

```bash
git push
```

### 4. GitHub Pagesで公開

1. GitHubリポジトリ: https://github.com/maruru3/suika-liff-game
2. **Settings** → **Pages**
3. **Source**: `main` branch
4. **Save** をクリック

数分待ってから、以下のURLでアクセス：

```
https://maruru3.github.io/suika-liff-game/lego-game.html
```

または、メニューから：

```
https://maruru3.github.io/suika-liff-game/menu.html
```

## 🎮 テストプレイ

### ローカルテスト

ビルド完了後、すぐにテストできます：

1. **Build Settings** ウィンドウで **Build And Run** を使う
2. または、`WebGL_Build/index.html` をブラウザで開く
3. **注意**: ローカルでは `file://` プロトコルの制約があるため、一部機能が動作しない場合があります

### ローカルサーバーでテスト（推奨）

```bash
# Python 3
cd D:/unity\ project/lego001/WebGL_Build
python -m http.server 8000

# または Node.js
npx http-server -p 8000
```

ブラウザで開く: `http://localhost:8000`

## 🐛 トラブルシューティング

### ビルドエラー

#### エラー: "Failed to build player"
- **解決**: Player Settings で WebGL platform を確認
- Scripting Backend が適切か確認 (IL2CPP推奨)

#### エラー: "Missing assemblies"
- **解決**: Unity パッケージを再インポート
- `Assets` → `Reimport All`

### 読み込みエラー

#### 白い画面のみ表示
- **原因**: ビルドファイルのパスが間違っている
- **解決**: ブラウザの Console でエラー確認
- ファイル名が `lego-game.html` の設定と一致しているか確認

#### 0%から進まない
- **原因**: Loader スクリプトが読み込めていない
- **解決**: `Build/lego001.loader.js` が存在するか確認
- CORS エラーの場合は、ローカルサーバーを使用

#### ファイルが見つからない (404)
- **原因**: ビルドファイルが正しい場所にない
- **解決**: `Build/` フォルダの構造を確認
- GitHub Pages でファイルが正しくアップロードされているか確認

### パフォーマンス問題

#### 動作が重い
- **解決1**: Player Settings で Quality を下げる
- **解決2**: Code Optimization を Size に設定
- **解決3**: Texture compression を有効化

#### メモリ不足
- **解決**: Player Settings で Memory Size を調整
- WebGL Memory Size: 256MB ～ 1024MB

## 📚 参考情報

### Unity WebGL ドキュメント
- https://docs.unity3d.com/Manual/webgl-building.html

### LEGO Microgame チュートリアル
- Unity Learn: https://learn.unity.com/project/lego-template

### ファイルサイズ最適化
- Texture compression
- Audio compression
- Disable unnecessary scenes
- Strip unused code

## ✅ チェックリスト

- [ ] Unity プロジェクトを開く
- [ ] Platform を WebGL に変更
- [ ] Player Settings を設定
- [ ] シーンを Build Settings に追加
- [ ] Build を実行
- [ ] ビルドファイルを確認
- [ ] 非圧縮ファイルを生成（.gz の場合）
- [ ] ファイルを suika-liff-game/Build/ にコピー
- [ ] Git add, commit, push
- [ ] GitHub Pages でデプロイ確認
- [ ] ブラウザでテストプレイ

---

**開発環境**: Unity 2021.3+ / WebGL
**デプロイ先**: GitHub Pages
**LINE連携**: LIFF (LINE Mini App)
