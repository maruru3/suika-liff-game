/**
 * Unity-LIFF Bridge
 * Unity WebGLとLINE Front-end Framework (LIFF)間の通信を管理
 * VSCode + Claude Code環境で開発
 */

class UnityLiffBridge {
    constructor() {
        this.unityInstance = null;
        this.liffInitialized = false;
        this.userProfile = null;
        this.gameObjectName = "LIFFBridge"; // Unity側の受信用GameObject名
    }

    /**
     * LIFF初期化
     * @param {string} liffId - LIFF ID
     */
    async initializeLIFF(liffId) {
        try {
            await liff.init({ liffId });
            this.liffInitialized = true;

            if (!liff.isLoggedIn()) {
                liff.login();
                return;
            }

            // プロフィール取得
            this.userProfile = await liff.getProfile();
            console.log('LIFF初期化成功:', this.userProfile);

            // Unity側に通知
            this.sendToUnity('OnLiffInitialized', JSON.stringify({
                userId: this.userProfile.userId,
                displayName: this.userProfile.displayName,
                pictureUrl: this.userProfile.pictureUrl
            }));

            return this.userProfile;
        } catch (error) {
            console.error('LIFF初期化エラー:', error);
            this.sendToUnity('OnLiffError', error.message);
            throw error;
        }
    }

    /**
     * Unity Instance登録
     * @param {Object} instance - UnityのWebGLインスタンス
     */
    setUnityInstance(instance) {
        this.unityInstance = instance;
        console.log('Unity Instance登録完了');

        // Unity側に準備完了を通知
        if (this.liffInitialized && this.userProfile) {
            this.sendToUnity('OnBridgeReady', JSON.stringify({
                ready: true,
                profile: this.userProfile
            }));
        }
    }

    /**
     * Unity → JavaScript メッセージ送信
     * @param {string} methodName - Unity側のメソッド名
     * @param {string} message - 送信するメッセージ
     */
    sendToUnity(methodName, message = '') {
        if (!this.unityInstance) {
            console.warn('Unity Instanceが未設定です');
            return;
        }

        try {
            this.unityInstance.SendMessage(this.gameObjectName, methodName, message);
            console.log(`Unity送信: ${methodName}`, message);
        } catch (error) {
            console.error('Unity送信エラー:', error);
        }
    }

    /**
     * スコアをLINEで共有
     * @param {number} score - ゲームスコア
     * @param {string} message - 共有メッセージ
     */
    async shareScore(score, message = '') {
        if (!this.liffInitialized) {
            console.error('LIFFが初期化されていません');
            return false;
        }

        const shareMessage = message || `スイカゲームで${score}点獲得しました！`;

        try {
            if (liff.isApiAvailable('shareTargetPicker')) {
                await liff.shareTargetPicker([
                    {
                        type: 'text',
                        text: shareMessage
                    }
                ]);
                this.sendToUnity('OnShareSuccess', 'スコア共有成功');
                return true;
            } else {
                console.warn('shareTargetPicker APIが利用できません');
                this.sendToUnity('OnShareError', 'この環境では共有機能が使えません');
                return false;
            }
        } catch (error) {
            console.error('共有エラー:', error);
            this.sendToUnity('OnShareError', error.message);
            return false;
        }
    }

    /**
     * ゲームデータをローカルストレージに保存
     * @param {string} key - 保存キー
     * @param {Object} data - 保存するデータ
     */
    saveGameData(key, data) {
        try {
            const dataStr = typeof data === 'string' ? data : JSON.stringify(data);
            localStorage.setItem(`unity_game_${key}`, dataStr);
            this.sendToUnity('OnSaveSuccess', key);
            return true;
        } catch (error) {
            console.error('保存エラー:', error);
            this.sendToUnity('OnSaveError', error.message);
            return false;
        }
    }

    /**
     * ゲームデータをローカルストレージから読み込み
     * @param {string} key - 読み込みキー
     * @returns {Object|null} 保存されたデータ
     */
    loadGameData(key) {
        try {
            const dataStr = localStorage.getItem(`unity_game_${key}`);
            if (!dataStr) {
                this.sendToUnity('OnLoadComplete', '');
                return null;
            }

            const data = JSON.parse(dataStr);
            this.sendToUnity('OnLoadComplete', JSON.stringify(data));
            return data;
        } catch (error) {
            console.error('読み込みエラー:', error);
            this.sendToUnity('OnLoadError', error.message);
            return null;
        }
    }

    /**
     * ユーザープロフィール取得
     * @returns {Object|null} LINEユーザープロフィール
     */
    getUserProfile() {
        return this.userProfile;
    }

    /**
     * LIFF環境情報取得
     * @returns {Object} LIFF環境情報
     */
    getLiffContext() {
        if (!this.liffInitialized) return null;

        return {
            isInClient: liff.isInClient(),
            isLoggedIn: liff.isLoggedIn(),
            os: liff.getOS(),
            language: liff.getLanguage(),
            version: liff.getVersion()
        };
    }

    /**
     * LIFF終了
     */
    closeLiff() {
        if (this.liffInitialized && liff.isInClient()) {
            liff.closeWindow();
        }
    }

    /**
     * 外部リンクを開く
     * @param {string} url - 開くURL
     */
    openExternalLink(url) {
        if (!this.liffInitialized) {
            window.open(url, '_blank');
            return;
        }

        if (liff.isInClient()) {
            liff.openWindow({
                url: url,
                external: true
            });
        } else {
            window.open(url, '_blank');
        }
    }

    /**
     * スキャンコード機能（カメラ使用）
     */
    async scanCode() {
        if (!this.liffInitialized) {
            console.error('LIFFが初期化されていません');
            return null;
        }

        if (!liff.isApiAvailable('scanCodeV2')) {
            this.sendToUnity('OnScanError', 'スキャン機能が利用できません');
            return null;
        }

        try {
            const result = await liff.scanCodeV2();
            this.sendToUnity('OnScanSuccess', result.value);
            return result.value;
        } catch (error) {
            console.error('スキャンエラー:', error);
            this.sendToUnity('OnScanError', error.message);
            return null;
        }
    }
}

// グローバルインスタンス
window.unityLiffBridge = new UnityLiffBridge();

/**
 * Unity側から呼び出される関数群
 * これらの関数はUnity C#のApplication.ExternalCall()またはApplication.ExternalEval()から呼ばれる
 */

// LIFF初期化
window.InitializeLIFF = function(liffId) {
    return window.unityLiffBridge.initializeLIFF(liffId);
};

// スコア共有
window.ShareScore = function(score, message) {
    return window.unityLiffBridge.shareScore(score, message);
};

// データ保存
window.SaveGameData = function(key, data) {
    return window.unityLiffBridge.saveGameData(key, data);
};

// データ読み込み
window.LoadGameData = function(key) {
    const data = window.unityLiffBridge.loadGameData(key);
    return data ? JSON.stringify(data) : '';
};

// プロフィール取得
window.GetUserProfile = function() {
    const profile = window.unityLiffBridge.getUserProfile();
    return profile ? JSON.stringify(profile) : '';
};

// コンテキスト取得
window.GetLiffContext = function() {
    const context = window.unityLiffBridge.getLiffContext();
    return context ? JSON.stringify(context) : '';
};

// LIFF終了
window.CloseLiff = function() {
    window.unityLiffBridge.closeLiff();
};

// 外部リンクを開く
window.OpenExternalLink = function(url) {
    window.unityLiffBridge.openExternalLink(url);
};

// QRコードスキャン
window.ScanCode = function() {
    return window.unityLiffBridge.scanCode();
};

console.log('Unity-LIFF Bridge loaded');
