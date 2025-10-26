using UnityEngine;

/// <summary>
/// 3Dフルーツのスクリプト
/// 球体の物理演算とフルーツ合成を管理
/// </summary>
public class Fruit3D : MonoBehaviour
{
    [Header("フルーツ設定")]
    public int fruitLevel = 0; // 0=さくらんぼ, 9=スイカ
    public bool canMerge = false; // 合成可能フラグ

    [Header("合成設定")]
    public float mergeDelay = 0.1f; // 合成までの遅延
    private bool hasMerged = false;

    [Header("エフェクト")]
    public ParticleSystem mergeEffect;

    // フルーツ情報
    private Rigidbody rb;
    private SphereCollider sphereCollider;
    private GameManager3D gameManager;

    // サウンド用
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        audioSource = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager3D>();

        // 初期状態では合成不可（落下中）
        Invoke(nameof(EnableMerge), mergeDelay);
    }

    void EnableMerge()
    {
        canMerge = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 床や壁との衝突音
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
        {
            PlayCollisionSound(collision.relativeVelocity.magnitude);
        }

        // フルーツ同士の衝突
        Fruit3D otherFruit = collision.gameObject.GetComponent<Fruit3D>();
        if (otherFruit != null)
        {
            PlayCollisionSound(collision.relativeVelocity.magnitude * 0.5f);

            // 合成条件チェック
            if (CanMergeWith(otherFruit))
            {
                MergeWith(otherFruit);
            }
        }
    }

    /// <summary>
    /// 合成可能かチェック
    /// </summary>
    bool CanMergeWith(Fruit3D other)
    {
        // 両方とも合成可能で、同じレベルで、まだ合成していない
        return canMerge &&
               other.canMerge &&
               fruitLevel == other.fruitLevel &&
               !hasMerged &&
               !other.hasMerged &&
               fruitLevel < 9; // スイカ（レベル9）は最大
    }

    /// <summary>
    /// フルーツ合成処理
    /// </summary>
    void MergeWith(Fruit3D other)
    {
        // 先に衝突した方が処理を行う（重複防止）
        if (transform.GetInstanceID() < other.transform.GetInstanceID())
        {
            // 合成済みフラグ
            hasMerged = true;
            other.hasMerged = true;

            // 中間地点を計算
            Vector3 mergePosition = (transform.position + other.transform.position) / 2f;

            // エフェクト再生
            if (mergeEffect != null)
            {
                Instantiate(mergeEffect, mergePosition, Quaternion.identity);
            }

            // 新しいフルーツを生成
            if (gameManager != null)
            {
                gameManager.SpawnFruitAt(fruitLevel + 1, mergePosition);

                // スコア加算
                int score = (fruitLevel + 1) * 10;
                gameManager.AddScore(score);
            }

            // 合成音を再生
            PlayMergeSound();

            // 両方のフルーツを削除
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 衝突音を再生
    /// </summary>
    void PlayCollisionSound(float intensity)
    {
        if (audioSource != null && intensity > 0.5f)
        {
            audioSource.volume = Mathf.Clamp01(intensity / 5f);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.Play();
        }
    }

    /// <summary>
    /// 合成音を再生
    /// </summary>
    void PlayMergeSound()
    {
        if (gameManager != null)
        {
            gameManager.PlayMergeSound(fruitLevel);
        }
    }

    /// <summary>
    /// フルーツの外観を設定
    /// </summary>
    public void SetupFruit(int level, Color color, float size)
    {
        fruitLevel = level;

        // サイズ設定
        transform.localScale = Vector3.one * size;

        // 色設定
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }

    /// <summary>
    /// Rigidbodyを有効化（ドロップ時）
    /// </summary>
    public void EnablePhysics()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    /// <summary>
    /// Rigidbodyを無効化（配置中）
    /// </summary>
    public void DisablePhysics()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
