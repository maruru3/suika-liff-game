# Unity 3Dスイカゲーム セットアップガイド

## 📋 概要

3D物理演算を使ったスイカゲームのUnity実装ガイドです。

## 🎮 ゲーム仕様

### フルーツの種類（10段階）
1. **さくらんぼ🍒** - レベル0 (サイズ: 0.4m, 赤)
2. **いちご🍓** - レベル1 (サイズ: 0.5m, ピンク)
3. **ぶどう🍇** - レベル2 (サイズ: 0.6m, 紫)
4. **デコポン🍊** - レベル3 (サイズ: 0.7m, オレンジ)
5. **かき🟠** - レベル4 (サイズ: 0.8m, 濃いオレンジ)
6. **りんご🍎** - レベル5 (サイズ: 0.9m, 赤)
7. **なし🍐** - レベル6 (サイズ: 1.0m, 黄)
8. **もも🍑** - レベル7 (サイズ: 1.1m, ピンク)
9. **パイナップル🍍** - レベル8 (サイズ: 1.2m, 黄)
10. **スイカ🍉** - レベル9 (サイズ: 1.4m, 緑) - **最大**

### ゲームルール
- タップ/クリックした位置にフルーツをドロップ
- 同じフルーツが衝突すると次のレベルに合成
- ボックスから溢れたらゲームオーバー

## 🛠️ Unityプロジェクトセットアップ

### 1. シーン構成

#### ヒエラルキー構造
```
Suika3DGame
├── Main Camera
├── Directional Light
├── GameManager (GameManager3D.cs)
├── Box
│   ├── Floor
│   ├── WallFront
│   ├── WallBack
│   ├── WallLeft
│   ├── WallRight
│   └── GameOverLine
├── SpawnPoint
├── FruitContainer
├── Canvas (UI)
│   ├── ScoreText (TextMeshPro)
│   ├── NextFruitText (TextMeshPro)
│   └── GameOverPanel
│       ├── GameOverText
│       ├── FinalScoreText
│       └── RestartButton
└── LIFFBridge (LIFFBridge.cs)
```

### 2. ボックス（Box）作成

#### Floor（床）
1. **Create** → **3D Object** → **Cube**
2. **Transform:**
   - Position: (0, 0, 0)
   - Scale: (4, 0.2, 4)
3. **Tag:** `Floor`
4. **Material:** 茶色のマテリアル

#### WallFront（前壁）
1. **Create** → **3D Object** → **Cube**
2. **Transform:**
   - Position: (0, 2, 2)
   - Scale: (4, 4, 0.2)
3. **Tag:** `Wall`
4. **Material:** 半透明の壁マテリアル

#### WallBack（後壁）
1. **Create** → **3D Object** → **Cube**
2. **Transform:**
   - Position: (0, 2, -2)
   - Scale: (4, 4, 0.2)
3. **Tag:** `Wall`

#### WallLeft（左壁）
1. **Create** → **3D Object** → **Cube**
2. **Transform:**
   - Position: (-2, 2, 0)
   - Scale: (0.2, 4, 4)
3. **Tag:** `Wall`

#### WallRight（右壁）
1. **Create** → **3D Object** → **Cube**
2. **Transform:**
   - Position: (2, 2, 0)
   - Scale: (0.2, 4, 4)
3. **Tag:** `Wall`

#### GameOverLine（ゲームオーバーライン）
1. **Create** → **Empty GameObject**
2. **Transform:**
   - Position: (0, 3.5, 0)
3. **名前:** `GameOverLine`

### 3. カメラ設定

**Main Camera:**
- Position: (0, 3, -6)
- Rotation: (20, 0, 0)
- Field of View: 60

### 4. フルーツプレハブ作成

#### 基本フルーツプレハブ
1. **Create** → **3D Object** → **Sphere**
2. **名前:** `Fruit3D`
3. **コンポーネント追加:**
   - **Rigidbody**
     - Mass: 1
     - Use Gravity: true
     - Is Kinematic: false
     - Collision Detection: Continuous Dynamic
   - **Sphere Collider**
     - Radius: 0.5
     - Material: Physics Material (Bounce: 0.3, Friction: 0.5)
   - **Audio Source**
     - Play On Awake: false
     - Spatial Blend: 1 (3D)
   - **Fruit3D.cs** スクリプト

4. **Prefab化:**
   - `Assets/Prefabs/Fruit3D.prefab` として保存

### 5. GameManager設定

1. **Create** → **Empty GameObject**
2. **名前:** `GameManager`
3. **GameManager3D.cs** をアタッチ
4. **Inspector設定:**
   - **Fruit Prefab:** `Fruit3D` プレハブ
   - **Spawn Point:** SpawnPointオブジェクト
   - **Fruit Container:** FruitContainerオブジェクト
   - **Drop Height:** 5
   - **Box:** Boxオブジェクト
   - **Box Width:** 4
   - **Box Depth:** 4
   - **Game Over Line:** GameOverLineオブジェクト
   - **Main Camera:** Main Cameraオブジェクト
   - **Score Text:** ScoreText (UI)
   - **Next Fruit Text:** NextFruitText (UI)
   - **Game Over Panel:** GameOverPanel (UI)
   - **Restart Button:** RestartButton (UI)

### 6. UI作成

#### Canvas
1. **Create** → **UI** → **Canvas**
2. **Canvas Scaler:**
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1080 x 1920

#### ScoreText
1. **Create** → **UI** → **Text - TextMeshPro**
2. **Transform:**
   - Anchor: Top Left
   - Position: (100, -50)
3. **Text:** "Score: 0"
4. **Font Size:** 48
5. **Color:** White

#### NextFruitText
1. **Create** → **UI** → **Text - TextMeshPro**
2. **Transform:**
   - Anchor: Top Center
   - Position: (0, -50)
3. **Text:** "Next: さくらんぼ"
4. **Font Size:** 36

#### GameOverPanel
1. **Create** → **UI** → **Panel**
2. **Color:** 半透明黒 (0, 0, 0, 200)
3. **Initially Active:** false

### 7. Physics Settings

**Edit** → **Project Settings** → **Physics**
- Gravity Y: -20 (より速い落下)
- Default Material: バウンス0.3のPhysics Material

### 8. Tags作成

**Edit** → **Project Settings** → **Tags**
- `Floor`
- `Wall`

## 🎨 マテリアル作成

### フルーツマテリアル
1. **Create** → **Material** → `FruitMaterial`
2. **Shader:** Standard
3. **Rendering Mode:** Opaque
4. **Smoothness:** 0.5
5. **色はスクリプトから動的に設定**

### 壁マテリアル（半透明）
1. **Create** → **Material** → `WallMaterial`
2. **Shader:** Standard
3. **Rendering Mode:** Transparent
4. **Albedo:** 白
5. **Alpha:** 100

### 床マテリアル
1. **Create** → **Material** → `FloorMaterial`
2. **Shader:** Standard
3. **Albedo:** 茶色 (139, 90, 43)

## 🔊 サウンド（オプション）

### 必要なサウンド
- `drop.wav` - フルーツドロップ音
- `merge0.wav` ～ `merge9.wav` - 合成音（レベル別）
- `gameover.wav` - ゲームオーバー音

**GameManager** の Inspector で設定

## 🌐 WebGL ビルド設定

### Build Settings
1. **File** → **Build Settings**
2. **Platform:** WebGL
3. **Add Open Scenes:** 現在のシーンを追加

### Player Settings
1. **Company Name:** YourCompany
2. **Product Name:** Suika3DGame
3. **Resolution:**
   - Default Canvas Width: 1080
   - Default Canvas Height: 1920
4. **WebGL Template:**
   - Minimal または Custom Template

### Build
1. **Build And Run** または **Build**
2. **出力フォルダ:** `suika-liff/WebGLTemplates/Build`

ビルド後のファイル:
```
Build/
├── WebGLTemplates.data.gz
├── WebGLTemplates.framework.js.gz
├── WebGLTemplates.loader.js
└── WebGLTemplates.wasm.gz
```

### ビルドファイルの展開（GitHub Pages用）
```bash
cd Build
gunzip -c WebGLTemplates.data.gz > WebGLTemplates.data
gunzip -c WebGLTemplates.framework.js.gz > WebGLTemplates.framework.js
gunzip -c WebGLTemplates.wasm.gz > WebGLTemplates.wasm
```

## 🎯 テストプレイ

### Unity Editor
1. **Play ボタン** をクリック
2. **マウスクリック** でフルーツをドロップ
3. **同じフルーツを合成** してスコアアップ

### WebGL
1. ビルド後、ブラウザで `index.html` を開く
2. モバイルでは **タッチ操作**

## 🐛 トラブルシューティング

### フルーツが合成されない
- `Fruit3D.cs` の `mergeDelay` を確認
- Rigidbody の Collision Detection を `Continuous Dynamic` に設定

### フルーツが落下しない
- Rigidbody の `Use Gravity` がtrueか確認
- `Is Kinematic` がfalseか確認

### WebGLビルドで読み込めない
- ファイルパスが正しいか確認（.gz拡張子なし）
- 非圧縮ファイルが存在するか確認
- ブラウザコンソールでエラー確認

### ゲームオーバーにならない
- GameOverLine の位置確認（Y座標: 3.5）
- Box の高さ確認（Y座標: 4）

## 📚 スクリプト説明

### Fruit3D.cs
- フルーツの物理挙動
- 合成ロジック
- 衝突検知

### GameManager3D.cs
- ゲーム全体の管理
- フルーツ生成
- スコア管理
- UI更新

### LIFFBridge.cs
- LINE連携
- スコア共有
- ユーザー情報取得

## 🚀 次のステップ

1. **ビジュアル改善:** フルーツにテクスチャを追加
2. **サウンド追加:** 効果音・BGM
3. **エフェクト追加:** パーティクルシステムで合成エフェクト
4. **ランキング機能:** LIFFでスコア共有・ランキング表示
5. **アニメーション:** フルーツ出現・消滅アニメーション

---

**開発環境:** Unity 2021.3+ / VSCode / Claude Code
**デプロイ先:** GitHub Pages
**LINE連携:** LIFF (LINE Mini App)
