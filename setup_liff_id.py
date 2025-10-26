#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
LIFF ID 設定スクリプト
LIFFBridge.cs と unity-liff-template.html に LIFF ID を一括設定します

使い方:
    python setup_liff_id.py YOUR-LIFF-ID
    例: python setup_liff_id.py 2008275057-VqJkXjxy

VSCode + Claude Code環境で開発
"""

import sys
import re
import os
from pathlib import Path


def validate_liff_id(liff_id):
    """LIFF IDの形式をバリデーション"""
    pattern = r'^\d{10}-[a-zA-Z0-9]{8}$'
    if not re.match(pattern, liff_id):
        return False
    return True


def update_liffbridge_cs(liff_id):
    """LIFFBridge.cs の LIFF ID を更新"""
    file_path = Path('LIFFBridge.cs')

    if not file_path.exists():
        print(f"❌ エラー: {file_path} が見つかりません")
        return False

    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()

        # LIFF ID のパターンをマッチ
        pattern = r'private string liffId = "[^"]*";'
        replacement = f'private string liffId = "{liff_id}";'

        new_content = re.sub(pattern, replacement, content)

        with open(file_path, 'w', encoding='utf-8') as f:
            f.write(new_content)

        print(f"✅ {file_path} を更新しました")
        return True

    except Exception as e:
        print(f"❌ エラー: {file_path} の更新に失敗 - {e}")
        return False


def update_unity_template(liff_id):
    """unity-liff-template.html の LIFF ID を更新"""
    file_path = Path('unity-liff-template.html')

    if not file_path.exists():
        print(f"❌ エラー: {file_path} が見つかりません")
        return False

    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()

        # LIFF ID のパターンをマッチ
        pattern = r"liffId: '[^']*'"
        replacement = f"liffId: '{liff_id}'"

        new_content = re.sub(pattern, replacement, content)

        with open(file_path, 'w', encoding='utf-8') as f:
            f.write(new_content)

        print(f"✅ {file_path} を更新しました")
        return True

    except Exception as e:
        print(f"❌ エラー: {file_path} の更新に失敗 - {e}")
        return False


def update_suika_liff(liff_id):
    """suika-liff.html の LIFF ID を更新（オプション）"""
    file_path = Path('suika-liff.html')

    if not file_path.exists():
        print(f"⚠️  {file_path} が見つかりません（スキップ）")
        return True

    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()

        # LIFF ID のパターンをマッチ
        pattern = r"liffId: '[^']*'"
        replacement = f"liffId: '{liff_id}'"

        new_content = re.sub(pattern, replacement, content)

        with open(file_path, 'w', encoding='utf-8') as f:
            f.write(new_content)

        print(f"✅ {file_path} を更新しました")
        return True

    except Exception as e:
        print(f"⚠️  {file_path} の更新に失敗（スキップ） - {e}")
        return True  # オプションなので失敗してもOK


def create_env_file(liff_id):
    """.env ファイルを作成"""
    env_path = Path('.env')

    if env_path.exists():
        print(f"⚠️  .env が既に存在します（スキップ）")
        return True

    try:
        with open(env_path, 'w', encoding='utf-8') as f:
            f.write(f"LIFF_ID={liff_id}\n")
            f.write("LIFF_ENDPOINT_URL_DEV=https://your-ngrok-url.ngrok.io\n")
            f.write("LIFF_ENDPOINT_URL_PROD=https://yourusername.github.io/your-repo\n")

        print(f"✅ .env ファイルを作成しました")
        return True

    except Exception as e:
        print(f"⚠️  .env ファイルの作成に失敗（スキップ） - {e}")
        return True


def main():
    """メイン処理"""
    print("=" * 60)
    print("  LIFF ID 設定スクリプト")
    print("  Unity-LIFF Bridge 自動設定ツール")
    print("=" * 60)
    print()

    # 引数チェック
    if len(sys.argv) < 2:
        print("使い方: python setup_liff_id.py YOUR-LIFF-ID")
        print()
        print("例:")
        print("  python setup_liff_id.py 2008275057-VqJkXjxy")
        print()
        print("LIFF ID の形式: [10桁の数字]-[8桁の英数字]")
        print()
        sys.exit(1)

    liff_id = sys.argv[1]

    # LIFF ID のバリデーション
    print(f"📝 LIFF ID: {liff_id}")

    if not validate_liff_id(liff_id):
        print()
        print("❌ エラー: LIFF ID の形式が正しくありません")
        print()
        print("正しい形式: [10桁の数字]-[8桁の英数字]")
        print("例: 2008275057-VqJkXjxy")
        print()
        sys.exit(1)

    print("✅ LIFF ID の形式が正しいです")
    print()

    # ファイル更新
    print("🔧 ファイルを更新中...")
    print()

    results = []
    results.append(update_liffbridge_cs(liff_id))
    results.append(update_unity_template(liff_id))
    results.append(update_suika_liff(liff_id))
    results.append(create_env_file(liff_id))

    print()

    # 結果表示
    if all(results):
        print("=" * 60)
        print("✅ すべての設定が完了しました！")
        print("=" * 60)
        print()
        print("次のステップ:")
        print("  1. Unityプロジェクトを開く")
        print("  2. LIFFBridge GameObject を確認")
        print("  3. WebGL ビルドを実行")
        print("  4. HTTPSサーバーでホスト")
        print()
        print("詳細は LIFF_SETUP_GUIDE.md を参照してください")
        print()
    else:
        print("=" * 60)
        print("⚠️  一部のファイル更新に失敗しました")
        print("=" * 60)
        print()
        print("手動で以下のファイルを確認してください:")
        print("  - LIFFBridge.cs")
        print("  - unity-liff-template.html")
        print("  - suika-liff.html (オプション)")
        print()


if __name__ == '__main__':
    main()
