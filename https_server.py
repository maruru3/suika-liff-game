#!/usr/bin/env python3
import http.server
import ssl
import socketserver
import os

# 簡易SSL証明書生成（開発用）
def create_self_signed_cert():
    import subprocess
    try:
        # 自己署名証明書の生成
        subprocess.run([
            'openssl', 'req', '-x509', '-newkey', 'rsa:2048',
            '-keyout', 'server.key', '-out', 'server.crt',
            '-days', '365', '-nodes', '-batch',
            '-subj', '/C=JP/ST=Tokyo/L=Tokyo/O=Dev/CN=192.168.1.150'
        ], check=True)
        return True
    except:
        # OpenSSLが使えない場合のフォールバック
        return False

class HTTPSServer:
    def __init__(self, port=8443):
        self.port = port

    def start(self):
        # 証明書ファイルの確認
        if not (os.path.exists('server.crt') and os.path.exists('server.key')):
            print("SSL証明書を生成中...")
            if not create_self_signed_cert():
                print("注意: SSL証明書の自動生成に失敗しました")
                print("手動で証明書を生成してください")
                return

        # HTTPSサーバーの起動
        Handler = http.server.SimpleHTTPRequestHandler

        with socketserver.TCPServer(("", self.port), Handler) as httpd:
            # SSL設定
            try:
                context = ssl.create_default_context(ssl.Purpose.CLIENT_AUTH)
                context.load_cert_chain('server.crt', 'server.key')
                httpd.socket = context.wrap_socket(httpd.socket, server_side=True)

                print(f"HTTPSサーバーを開始しました: https://192.168.1.150:{self.port}")
                print("証明書の警告が表示されますが、開発用なので「詳細設定」→「安全でないサイトに移動」で進んでください")
                httpd.serve_forever()
            except Exception as e:
                print(f"HTTPS設定エラー: {e}")
                print("通常のHTTPサーバーで起動します...")
                httpd.serve_forever()

if __name__ == "__main__":
    server = HTTPSServer(8443)
    server.start()