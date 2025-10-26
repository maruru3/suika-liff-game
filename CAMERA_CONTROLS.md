# 3Dスイカゲーム カメラ操作ガイド

## 🎮 操作方法

### PC（マウス）

#### フルーツ配置
- **左クリック** - フルーツを移動
- **左クリック離す** - フルーツをドロップ

#### カメラ回転
- **右クリック + ドラッグ** - カメラを回転
- **Ctrl + 左クリック + ドラッグ** - カメラを回転
- **マウスホイール** - ズームイン/アウト

### モバイル（タッチ）

#### フルーツ配置
- **1本指タッチ** - フルーツを移動
- **1本指離す** - フルーツをドロップ

#### カメラ操作
- **1本指スワイプ（フルーツ配置後）** - カメラを回転
- **2本指ピンチ** - ズームイン/アウト

## ⚙️ カメラ設定

### CameraController パラメータ

| パラメータ | 説明 | デフォルト値 |
|-----------|------|------------|
| **Target** | カメラの注視点 | ボックス中心(0,2,0) |
| **Distance** | カメラ距離 | 8m |
| **Rotation Speed** | 回転速度 | 5 |
| **Min Distance** | 最小距離 | 4m |
| **Max Distance** | 最大距離 | 15m |
| **Min Vertical Angle** | 最小垂直角度 | 10° |
| **Max Vertical Angle** | 最大垂直角度 | 80° |
| **Smooth Rotation** | 滑らかな回転 | ON |
| **Smooth Time** | スムージング時間 | 0.1秒 |

## 🔧 Unity設定

### 自動セットアップ
1. **Tools > Build Suika 3D Scene** を実行
2. カメラに自動的に `CameraController` が追加されます

### 手動セットアップ
1. Main Camera に `CameraController.cs` をアタッチ
2. Camera Target オブジェクトを作成（位置: 0,2,0）
3. Inspector で Target に Camera Target を設定

## 💡 使い方のコツ

### フルーツ配置のコツ
- **1本指/左クリック**でフルーツを狙った位置に配置
- 配置中はカメラが回転しないので安心

### カメラ操作のコツ
- **右クリック**でグルグル回転して最適な角度を探す
- **マウスホイール/ピンチ**で拡大して細かい配置を確認
- 斜め上から見ると合成しやすい！

### 便利な視点
- **真上（90°）** - 全体像を把握
- **斜め（20-45°）** - プレイしやすい角度
- **横（10-15°）** - スイカの高さをチェック

## 🎯 ゲームプレイ時の注意

### UI操作とカメラ回転の競合回避
- UI（ボタンなど）をタッチしている時はカメラが回転しません
- フルーツ配置中（1本指ドラッグ中）もカメラ回転は無効です
- 意図しない回転を防ぐため、2本指でのみズーム可能

### パフォーマンス
- スムージングをOFFにすると軽量化
- ただし、動きがカクカクになります

## 📱 プラットフォーム別の違い

### PC (WebGL)
- 右クリックドラッグが最も快適
- マウスホイールで正確なズーム

### モバイル (WebGL)
- タッチUIに最適化済み
- ピンチジェスチャーでスムーズなズーム
- 1本指スワイプで直感的な回転

## 🐛 トラブルシューティング

### カメラが回転しない
- CameraController がアタッチされているか確認
- Target が設定されているか確認
- `Can Rotate` が true になっているか確認

### カメラが勝手に回転する
- GameManager3D でカメラ回転制御が正しく動いているか確認
- UI上でタッチしていないか確認

### ズームができない
- Min/Max Distance の範囲を確認
- マウスホイールの感度（Mouse Wheel Sensitivity）を調整

### カメラの動きがカクカクする
- Smooth Rotation を ON にする
- Smooth Time を 0.1 ～ 0.2 に設定

## 🎨 カスタマイズ例

### 高速回転
```csharp
cameraController.rotationSpeed = 10f;
cameraController.smoothTime = 0.05f;
```

### 遠くから見る
```csharp
cameraController.distance = 12f;
cameraController.maxDistance = 20f;
```

### 真上から見る視点に制限
```csharp
cameraController.minVerticalAngle = 60f;
cameraController.maxVerticalAngle = 90f;
```

### スムージングなし（軽量化）
```csharp
cameraController.smoothRotation = false;
```

## 📚 スクリプトAPI

### CameraController メソッド

```csharp
// カメラ回転の有効/無効切り替え
cameraController.SetCanRotate(bool enable);

// デフォルト視点にリセット
cameraController.ResetToDefaultView();
```

### GameManager3D からの制御

```csharp
// フルーツ配置中は自動的にカメラ回転が無効化されます
// 配置完了後は自動的に再有効化されます
```

---

**開発:** Unity 2021.3+ / VSCode / Claude Code
**カメラシステム:** Orbital Camera (軌道カメラ)
**入力:** Mouse + Touch 対応
