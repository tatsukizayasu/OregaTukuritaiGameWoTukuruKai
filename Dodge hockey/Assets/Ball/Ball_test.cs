using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Ball_test : MonoBehaviour
{
    [Tooltip("反発係数")]
    [Range(0, 2)]
    [SerializeField] private float bounciness = 1.0f; // 反発係数

    [Tooltip("弾の速度")]
    public float speed = 10.0f; // 弾の速度

    [Tooltip("ONならDefaultContactOffsetの値を衝突検知に使用する")]
    [SerializeField] private bool useContactOffset = true; // 衝突検知にDefaultContactOffsetを使用するかどうか

    //private Rigidbody rigidbody; // Rigidbodyの参照
    private SphereCollider sphereCollider; // SphereColliderの参照
    private float defaultContactOffset; // デフォルトの接触オフセット
    private const float sphereCastMargin = 0.01f; // SphereCast用のマージン
    private Vector3? reboundVelocity; // 反射速度
    private Vector3 lastDirection; // 最後の進行方向
    private bool canKeepSpeed; // 速度を維持できるかどうか

    public Vector3 CatchBallLocathion; // キャッチするボールの位置

    // 外部から速度を設定するメソッド
    public void SetVelocity(Vector3 vector)
    {
        GetComponent<Rigidbody>().velocity = vector * speed;
    }

    // 初期化処理
    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>(); // SphereColliderコンポーネントを取得
    }

    // 初期化処理（Awake後に実行）
    private void Start()
    {
        Init();

        Vector3 debugDirection;

        // デバッグ用の初期方向を設定し速度を与える
        debugDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad), Mathf.Sin(Mathf.Deg2Rad), 0f);
        GetComponent<Rigidbody>().velocity = debugDirection * speed;
    }

    // 初期設定を行うメソッド
    private void Init()
    {
        // isTrigger=falseで使用する場合はContinuous Dynamicsに設定
        if (!sphereCollider.isTrigger && GetComponent<Rigidbody>().collisionDetectionMode != CollisionDetectionMode.ContinuousDynamic)
        {
            GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        // 重力の使用を禁止
        if (GetComponent<Rigidbody>().useGravity)
        {
            GetComponent<Rigidbody>().useGravity = false;
        }

        defaultContactOffset = Physics.defaultContactOffset; // デフォルトの接触オフセットを保存
        canKeepSpeed = true; // 速度を維持可能に設定
    }

    // 固定フレームごとに呼ばれるメソッド
    private void FixedUpdate()
    {
        // 前フレームで反射していたら反射後速度を反映
        ApplyReboundVelocity();

        // 進行方向に衝突対象があるかどうか確認
        ProcessForwardDetection();

        // 減速の制限
        KeepConstantSpeed();

    }

    /// <summary>
    /// 計算した反射ベクトルを反映する
    /// </summary>
    private void ApplyReboundVelocity()
    {
        if (reboundVelocity == null) return;

        GetComponent<Rigidbody>().velocity = reboundVelocity.Value; // 反射速度を設定
        speed *= bounciness; // 速度に反発係数を掛ける
        reboundVelocity = null; // 反射速度をリセット
        canKeepSpeed = true; // 速度を維持可能に設定
    }

    /// <summary>
    /// 前方方向を監視して1フレーム後に衝突している場合は反射ベクトルを計算する
    /// </summary>
    private void ProcessForwardDetection()
    {
        var velocity = GetComponent<Rigidbody>().velocity;
        var direction = velocity.normalized;

        var offset = useContactOffset ? defaultContactOffset * 2 : 0;
        var origin = transform.position - direction * (sphereCastMargin + offset);
        var colliderRadius = sphereCollider.radius + offset;
        var isHit = Physics.SphereCast(origin, colliderRadius, direction, out var hitInfo, velocity.magnitude * Time.fixedDeltaTime);

        if (isHit)
        {
            var distance = hitInfo.distance - sphereCastMargin;

            // 次フレームに使う反射速度を計算
            var normal = hitInfo.normal;
            var inVecDir = direction;
            var outVecDir = Vector3.Reflect(inVecDir, normal);
            outVecDir = new Vector3(outVecDir.x, 0.0f, outVecDir.z);
            reboundVelocity = outVecDir * speed * bounciness;

            // 衝突予定先に接するように速度を調整
            var adjustVelocity = distance / Time.fixedDeltaTime * direction;
            GetComponent<Rigidbody>().velocity = adjustVelocity;
            canKeepSpeed = false; // 速度を維持できない状態に設定
            
        }
    }

    /// <summary>
    /// 速度を一定に保つ
    /// 衝突や引っかかりによる減速を上書きする役目
    /// </summary>
    private void KeepConstantSpeed()
    {
        if (!canKeepSpeed) return;

        var velocity = GetComponent<Rigidbody>().velocity;
        var nowSqrSpeed = velocity.sqrMagnitude;
        var sqrSpeed = speed * speed;

        if (!Mathf.Approximately(nowSqrSpeed, sqrSpeed))
        {
            var dir = velocity != Vector3.zero ? velocity.normalized : lastDirection;
            GetComponent<Rigidbody>().velocity = dir * speed;
        }
    }
}
