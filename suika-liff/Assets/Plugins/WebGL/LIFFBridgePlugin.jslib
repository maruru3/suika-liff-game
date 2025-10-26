/**
 * Unity WebGL用 LIFF Bridge プラグイン
 * このファイルをUnityプロジェクトの Assets/Plugins/WebGL/ に配置してください
 *
 * LIFFBridge.csから呼ばれるJavaScript関数を定義します
 */

mergeInto(LibraryManager.library, {

    /**
     * LIFF初期化
     * @param {string} liffId - LIFF ID
     */
    InitializeLIFF: function(liffId) {
        var liffIdStr = UTF8ToString(liffId);

        if (typeof window.InitializeLIFF === 'function') {
            window.InitializeLIFF(liffIdStr);
        } else {
            console.error('InitializeLIFF function not found. Make sure unity-liff-bridge.js is loaded.');
        }
    },

    /**
     * スコア共有
     * @param {number} score - スコア
     * @param {string} message - メッセージ
     */
    ShareScore: function(score, message) {
        var messageStr = UTF8ToString(message);

        if (typeof window.ShareScore === 'function') {
            window.ShareScore(score, messageStr);
        } else {
            console.error('ShareScore function not found.');
        }
    },

    /**
     * データ保存
     * @param {string} key - キー
     * @param {string} data - データ
     */
    SaveGameData: function(key, data) {
        var keyStr = UTF8ToString(key);
        var dataStr = UTF8ToString(data);

        if (typeof window.SaveGameData === 'function') {
            window.SaveGameData(keyStr, dataStr);
        } else {
            console.error('SaveGameData function not found.');
        }
    },

    /**
     * データ読み込み
     * @param {string} key - キー
     * @returns {string} データ
     */
    LoadGameData: function(key) {
        var keyStr = UTF8ToString(key);

        if (typeof window.LoadGameData === 'function') {
            var data = window.LoadGameData(keyStr);

            // 文字列をUnityに返すためのメモリ確保
            var bufferSize = lengthBytesUTF8(data) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(data, buffer, bufferSize);
            return buffer;
        } else {
            console.error('LoadGameData function not found.');
            return null;
        }
    },

    /**
     * ユーザープロフィール取得
     * @returns {string} プロフィールJSON
     */
    GetUserProfile: function() {
        if (typeof window.GetUserProfile === 'function') {
            var profile = window.GetUserProfile();

            if (profile) {
                var bufferSize = lengthBytesUTF8(profile) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(profile, buffer, bufferSize);
                return buffer;
            }
        } else {
            console.error('GetUserProfile function not found.');
        }
        return null;
    },

    /**
     * LIFF環境情報取得
     * @returns {string} 環境情報JSON
     */
    GetLiffContext: function() {
        if (typeof window.GetLiffContext === 'function') {
            var context = window.GetLiffContext();

            if (context) {
                var bufferSize = lengthBytesUTF8(context) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(context, buffer, bufferSize);
                return buffer;
            }
        } else {
            console.error('GetLiffContext function not found.');
        }
        return null;
    },

    /**
     * LIFF終了
     */
    CloseLiff: function() {
        if (typeof window.CloseLiff === 'function') {
            window.CloseLiff();
        } else {
            console.error('CloseLiff function not found.');
        }
    },

    /**
     * 外部リンクを開く
     * @param {string} url - URL
     */
    OpenExternalLink: function(url) {
        var urlStr = UTF8ToString(url);

        if (typeof window.OpenExternalLink === 'function') {
            window.OpenExternalLink(urlStr);
        } else {
            console.error('OpenExternalLink function not found.');
        }
    },

    /**
     * QRコードスキャン
     */
    ScanCode: function() {
        if (typeof window.ScanCode === 'function') {
            window.ScanCode();
        } else {
            console.error('ScanCode function not found.');
        }
    }
});
