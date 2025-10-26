using UnityEngine;

/// <summary>
/// カメラを回転させるコントローラー
/// マウスドラッグ/タッチスワイプでカメラを回転
/// ピンチでズーム対応
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("回転設定")]
    public Transform target; // 注視点（ボックス中心）
    public float rotationSpeed = 5f; // 回転速度
    public float distance = 8f; // カメラ距離
    public float minVerticalAngle = 10f; // 最小垂直角度
    public float maxVerticalAngle = 80f; // 最大垂直角度

    [Header("ズーム設定")]
    public float minDistance = 4f; // 最小距離
    public float maxDistance = 15f; // 最大距離
    public float zoomSpeed = 2f; // ズーム速度
    public float mouseWheelSensitivity = 2f; // マウスホイール感度

    [Header("スムージング")]
    public bool smoothRotation = true;
    public float smoothTime = 0.1f;

    // 内部状態
    private float currentHorizontalAngle = 0f; // 水平角度（Y軸回転）
    private float currentVerticalAngle = 20f; // 垂直角度（X軸回転）
    private float targetHorizontalAngle = 0f;
    private float targetVerticalAngle = 20f;
    private float currentDistance;
    private float targetDistance;

    // 入力状態
    private Vector2 lastInputPosition;
    private bool isRotating = false;
    private bool canRotate = true; // UI操作中は回転無効化

    // タッチズーム用
    private float initialPinchDistance = 0f;
    private float initialDistance = 0f;

    void Start()
    {
        // ターゲットが未設定の場合は原点を見る
        if (target == null)
        {
            GameObject targetObj = new GameObject("CameraTarget");
            targetObj.transform.position = new Vector3(0, 2, 0);
            target = targetObj.transform;
        }

        // 初期角度を現在のカメラ位置から計算
        Vector3 direction = transform.position - target.position;
        currentDistance = direction.magnitude;
        targetDistance = currentDistance;
        distance = currentDistance;

        currentHorizontalAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        currentVerticalAngle = Mathf.Asin(direction.y / currentDistance) * Mathf.Rad2Deg;

        targetHorizontalAngle = currentHorizontalAngle;
        targetVerticalAngle = currentVerticalAngle;
    }

    void LateUpdate()
    {
        if (!canRotate)
            return;

        HandleInput();
        UpdateCameraPosition();
    }

    /// <summary>
    /// 入力処理
    /// </summary>
    void HandleInput()
    {
        // マウス入力
        if (Input.GetMouseButtonDown(1)) // 右クリック
        {
            StartRotation(Input.mousePosition);
        }
        else if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl)) // Ctrl+左クリック
        {
            StartRotation(Input.mousePosition);
        }
        else if (Input.GetMouseButton(1) || (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl)))
        {
            ContinueRotation(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(0))
        {
            StopRotation();
        }

        // マウスホイールでズーム
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetDistance -= scroll * mouseWheelSensitivity;
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }

        // タッチ入力（2本指でズーム、1本指で回転）
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            // UIの上でタッチしている場合は無視
            if (IsPointerOverUI(touch.position))
            {
                StopRotation();
                return;
            }

            if (touch.phase == TouchPhase.Began)
            {
                StartRotation(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved && isRotating)
            {
                ContinueRotation(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                StopRotation();
            }
        }
        else if (Input.touchCount == 2)
        {
            // ピンチズーム
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                initialPinchDistance = Vector2.Distance(touch1.position, touch2.position);
                initialDistance = targetDistance;
                StopRotation(); // ズーム中は回転停止
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                float currentPinchDistance = Vector2.Distance(touch1.position, touch2.position);
                float pinchDelta = initialPinchDistance - currentPinchDistance;
                float zoomDelta = pinchDelta * zoomSpeed * 0.01f;

                targetDistance = initialDistance + zoomDelta;
                targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
            }
        }
        else if (Input.touchCount == 0)
        {
            StopRotation();
        }
    }

    /// <summary>
    /// 回転開始
    /// </summary>
    void StartRotation(Vector2 inputPosition)
    {
        lastInputPosition = inputPosition;
        isRotating = true;
    }

    /// <summary>
    /// 回転継続
    /// </summary>
    void ContinueRotation(Vector2 inputPosition)
    {
        if (!isRotating)
            return;

        Vector2 delta = inputPosition - lastInputPosition;
        lastInputPosition = inputPosition;

        // 水平回転（Y軸）
        targetHorizontalAngle += delta.x * rotationSpeed * 0.1f;

        // 垂直回転（X軸）
        targetVerticalAngle -= delta.y * rotationSpeed * 0.1f;
        targetVerticalAngle = Mathf.Clamp(targetVerticalAngle, minVerticalAngle, maxVerticalAngle);
    }

    /// <summary>
    /// 回転停止
    /// </summary>
    void StopRotation()
    {
        isRotating = false;
    }

    /// <summary>
    /// カメラ位置更新
    /// </summary>
    void UpdateCameraPosition()
    {
        // スムージング
        if (smoothRotation)
        {
            currentHorizontalAngle = Mathf.LerpAngle(currentHorizontalAngle, targetHorizontalAngle, smoothTime);
            currentVerticalAngle = Mathf.Lerp(currentVerticalAngle, targetVerticalAngle, smoothTime);
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, smoothTime);
        }
        else
        {
            currentHorizontalAngle = targetHorizontalAngle;
            currentVerticalAngle = targetVerticalAngle;
            currentDistance = targetDistance;
        }

        // 角度から位置を計算
        float radianH = currentHorizontalAngle * Mathf.Deg2Rad;
        float radianV = currentVerticalAngle * Mathf.Deg2Rad;

        float horizontalDistance = currentDistance * Mathf.Cos(radianV);
        Vector3 position = new Vector3(
            horizontalDistance * Mathf.Sin(radianH),
            currentDistance * Mathf.Sin(radianV),
            horizontalDistance * Mathf.Cos(radianH)
        );

        // カメラ位置と回転を設定
        transform.position = target.position + position;
        transform.LookAt(target);
    }

    /// <summary>
    /// UI上でタッチしているかチェック
    /// </summary>
    bool IsPointerOverUI(Vector2 position)
    {
        if (UnityEngine.EventSystems.EventSystem.current == null)
            return false;

        UnityEngine.EventSystems.PointerEventData eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        eventData.position = position;

        System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    /// <summary>
    /// カメラ回転を有効/無効化（外部から制御可能）
    /// </summary>
    public void SetCanRotate(bool enable)
    {
        canRotate = enable;
        if (!enable)
        {
            StopRotation();
        }
    }

    /// <summary>
    /// 特定の角度にリセット
    /// </summary>
    public void ResetToDefaultView()
    {
        targetHorizontalAngle = 0f;
        targetVerticalAngle = 20f;
        targetDistance = distance;
    }

    /// <summary>
    /// Gizmo表示（エディタ用）
    /// </summary>
    void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(target.position, 0.2f);
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
