# Unity WebGL ビルドエラーの解決方法

## 🔴 エラー内容

```
wasm-ld: error: undefined symbol: InitializeLIFF
wasm-ld: error: undefined symbol: ShareScore
wasm-ld: error: undefined symbol: SaveGameData
...
```

このエラーは、UnityのC#コードから呼ばれるJavaScript関数が見つからないことを示しています。

## ✅ 解決方法

UnityのWebGLビルドでは、JavaScript関数を`.jslib`ファイルとして提供する必要があります。

### ステップ1: プラグインファイルを配置

1. **Unityプロジェクトのフォルダ構造を作成**:
   ```
   YourUnityProject/
   └── Assets/
       └── Plugins/
           └── WebGL/
               └── LIFFBridgePlugin.jslib  ← ここに配置
   ```

2. **フォルダを作成**（まだ存在しない場合）:
   - Unity エディタで: `Assets`フォルダを右クリック
   - `Create > Folder` で `Plugins` フォルダを作成
   - `Plugins`フォルダ内に `WebGL` フォルダを作成

3. **`LIFFBridgePlugin.jslib` をコピー**:
   - プロジェクトルートにある `LIFFBridgePlugin.jslib` を
   - `Assets/Plugins/WebGL/` にコピー

### ステップ2: ファイルが認識されたか確認

1. Unityエディタで `Assets/Plugins/WebGL/LIFFBridgePlugin.jslib` が表示されることを確認

2. ファイルを選択して Inspector を確認:
   - Platform: `WebGL` にチェックが入っているはず
   - Include: `Editor`, `Standalone`, `WebGL` など

### ステップ3: 再ビルド

1. **File > Build Settings**
2. **WebGL** を選択
3. **Build** または **Build And Run**

これでエラーが解消されるはずです！

---

## 📁 正しいフォルダ構造

ビルド成功後の構造：

```
YourUnityProject/
├── Assets/
│   ├── Plugins/
│   │   └── WebGL/
│   │       └── LIFFBridgePlugin.jslib  ✅ 必須！
│   ├── Scripts/
│   │   ├── LIFFBridge.cs
│   │   └── GameManager.cs
│   └── WebGLTemplates/
│       └── LIFFTemplate/
│           ├── index.html
│           └── unity-liff-bridge.js
├── Build/                           # ビルド出力先
│   ├── index.html
│   ├── unity-liff-bridge.js
│   └── Build/
│       ├── UnityBuild.loader.js
│       ├── UnityBuild.data
│       ├── UnityBuild.framework.js
│       └── UnityBuild.wasm
└── ...
```

---

## 🔍 トラブルシューティング

### エラーが解消されない場合

1. **フォルダ名の確認**:
   - `Plugins` と `WebGL` のスペルが正しいか
   - 大文字小文字が正確か（`WebGL` であって `webgl` ではない）

2. **ファイル拡張子の確認**:
   - `.jslib` であること（`.js` ではない）

3. **Unityエディタを再起動**:
   - Unity Hub から一度プロジェクトを閉じて再度開く

4. **キャッシュをクリア**:
   - `Library` フォルダを削除（Unityが再生成します）
   - 再度ビルド

### それでもエラーが出る場合

**LIFFBridge.csをEditor専用にする**（一時的な回避策）:

```csharp
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void InitializeLIFF(string liffId);
    // ... 他の関数
#else
    // エディタ用のダミー実装
    private static void InitializeLIFF(string liffId) {
        Debug.Log($"[Editor] InitializeLIFF: {liffId}");
    }
    // ... 他の関数も同様に
#endif
```

---

## 📝 .jslibファイルとは？

`.jslib` ファイルは、Unity WebGLビルドでJavaScript関数をC#から呼び出すためのプラグインファイルです。

### 基本構造

```javascript
mergeInto(LibraryManager.library, {
    // C#から呼ばれる関数
    MyFunction: function(param) {
        // JavaScript処理
    }
});
```

### Unity C# 側

```csharp
[DllImport("__Internal")]
private static extern void MyFunction(string param);
```

これで `MyFunction` がC#からJavaScriptに橋渡しされます。

---

## 🎯 完了チェックリスト

ビルド成功までの確認項目：

- [ ] `Assets/Plugins/WebGL/` フォルダが存在する
- [ ] `LIFFBridgePlugin.jslib` がその中にある
- [ ] ファイル名が正確（スペル・拡張子）
- [ ] Unityエディタでファイルが認識されている
- [ ] WebGLプラットフォームが選択されている
- [ ] ビルドを実行
- [ ] エラーなくビルド完了 ✅

---

## 🚀 次のステップ

ビルドが成功したら：

1. **ビルドファイルを確認**:
   - `Build/` フォルダに以下が生成されているか:
     - `index.html`
     - `unity-liff-bridge.js`
     - `Build/UnityBuild.loader.js` など

2. **ローカルでテスト**:
   ```bash
   cd Build
   python -m http.server 8000
   ```

3. **ngrokでトンネル作成**:
   ```bash
   ngrok http 8000
   ```

4. **LINE Developersで設定**:
   - Endpoint URL を ngrok の URL に更新

5. **LINEアプリでテスト**:
   - LIFF URL を開いて動作確認

---

## 📖 関連ドキュメント

- [UNITY_INTEGRATION.md](UNITY_INTEGRATION.md) - Unity統合の全体ガイド
- [MINIAPP_SETUP_GUIDE.md](MINIAPP_SETUP_GUIDE.md) - LINEミニアプリ設定
- [Unity公式: WebGLとJavaScriptの連携](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html)

---

**作成日:** 2025年10月26日
**対象:** Unity WebGL + LINEミニアプリ
**開発環境:** Unity 6000.2.9f1
