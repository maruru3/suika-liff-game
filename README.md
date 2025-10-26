# スイカゲーム - LINEミニアプリ（2025年版）

LINEで遊べるスイカゲームです。物理エンジンを使用したリアルな落ち物パズルゲーム。
**Unity WebGL連携対応** - Unityゲームとの統合も可能です。

> **2025年最新**: LIFFはLINEミニアプリに統合されました。このプロジェクトは最新の方式に対応しています。

## 機能

### ゲーム機能
- 🍇 11種類のフルーツの合体システム
- 🎮 物理エンジン（Matter.js）による自然な動き
- 📱 モバイル完全対応
- 👤 LINEプロフィール連携
- 📤 スコアシェア機能
- 🎯 レスポンシブデザイン

### Unity連携機能（NEW!）
- 🎮 Unity WebGLゲーム統合
- 🔗 Unity ↔ LINEミニアプリ 双方向通信
- 💾 データ保存/読み込み
- 📷 QRコードスキャン
- 🌐 外部ブラウザ対応（2025年10月〜）

## プレイ方法

1. フルーツを落として同じ種類同士を合体させる
2. 最終目標は🍉（スイカ）を作ること
3. 上部にフルーツが積み上がるとゲームオーバー

## 技術スタック

- HTML5 Canvas
- Matter.js (物理エンジン)
- **LIFF SDK v2** (LINEミニアプリ対応)
- CSS3 (レスポンシブ対応)
- JavaScript ES6

## LINEミニアプリ設定

### クイックスタート

ミニアプリ IDの設定は簡単です：

```bash
# Python スクリプトで一括設定
python setup_liff_id.py YOUR-LIFF-ID
```

詳細は以下のガイドを参照：
- **[MINIAPP_QUICKSTART.md](MINIAPP_QUICKSTART.md)** - 5分で完了！最短手順
- **[MINIAPP_SETUP_GUIDE.md](MINIAPP_SETUP_GUIDE.md)** - 詳細な設定ガイド

### Unity連携

Unity WebGLゲームとの統合方法：
- **[UNITY_INTEGRATION.md](UNITY_INTEGRATION.md)** - Unity連携の完全ガイド

### プロジェクト全体

プロジェクトの詳細情報：
- **[agents.md](agents.md)** - 技術スタック、アーキテクチャ、開発環境

## ファイル構成

```
.
├── suika-liff.html          # メインゲーム（LIFF版）
├── index.html               # 拡張版（ランキング・サウンド）
├── unity-liff-bridge.js     # Unity-LIFF Bridge (JS)
├── unity-liff-template.html # Unity WebGLテンプレート
├── LIFFBridge.cs            # Unity-LIFF Bridge (C#)
├── GameManager.cs           # Unityサンプル
├── setup_liff_id.py         # LIFF ID設定スクリプト
├── MINIAPP_QUICKSTART.md    # LINEミニアプリ設定クイックガイド
├── MINIAPP_SETUP_GUIDE.md   # LINEミニアプリ設定詳細ガイド
├── UNITY_INTEGRATION.md     # Unity統合ガイド
└── agents.md                # プロジェクト詳細
```

## 開発環境

- **VSCode** + **Claude Code** (AI支援コーディング)
- **Serena MCP** (セマンティックコード検索)
- **Unity 2021.3+** (WebGL対応)
- **Python HTTPS Server** (ローカル開発)

## デプロイ

本アプリはGitHub Pagesなどでホストでき、HTTPS対応でLINEミニアプリとして安全に動作します。

### デプロイ先の例
- GitHub Pages
- Netlify
- Vercel
- Firebase Hosting

## ライセンス

MIT License - 教育・学習目的で作成されています