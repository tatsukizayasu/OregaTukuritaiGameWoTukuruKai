
using System.Collections;
using UnityEngine;



[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Ball : MonoBehaviour
{

    [Tooltip("反発係数")]
    [Range(0, 2)]
    [SerializeField] private float bounciness = 1.0f;

    [SerializeField] private const float ball_posY = 1.7f;

    [Tooltip("弾の速度")]
    [SerializeField] private float speed = 10.0f;
    public float Speed { get { return speed; } set { speed = value; } }

    [SerializeField] public GameObject goaleffect;

    const float max_speed = 100.0f;
    private Rigidbody rigidbody;
    private SphereCollider sphereCollider;
    private float defaultContactOffset;
    private const float sphereCastMargin = 0.01f; // SphereCast用のマージン
    private Vector3? reboundVelocity; // 反射速度
    private Vector3 lastDirection;
    private bool canKeepSpeed;

    private bool catchFlg = false;
    public bool CatchFlg { get; set; }
    private GameObject GoalObj = null;

    GameManager gamemanager;
    SE_Player se_players;

    int count = 0;

    public void SetVelocity(Vector3 velocity)
    {
        rigidbody.velocity = velocity;
    }

    //Start()前に実行　Rigidbody・SphereColliderを作成
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    //Awake()後に実行
    //初速を設定
    private void Start()
    {
        Init();

        //Vector3 debugDirection;

        //debugDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad), Mathf.Sin(Mathf.Deg2Rad), 0f);
        //rigidbody.velocity = debugDirection * speed;

        gamemanager = FindObjectOfType<GameManager>();
        se_players = GetComponent<SE_Player>();

    }

    private void Init()
    {
        // isTrigger=false で使用する場合はContinuous Dynamicsに設定
        if (!sphereCollider.isTrigger && rigidbody.collisionDetectionMode != CollisionDetectionMode.ContinuousDynamic)
        {
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        // 重力の使用は禁止
        if (rigidbody.useGravity)
        {
            rigidbody.useGravity = false;
        }

        defaultContactOffset = Physics.defaultContactOffset;
        canKeepSpeed = true;
    }


    private void FixedUpdate()
    {

        if (catchFlg)
        {

        }
        else
        {
            // 前フレームで反射していたら反射後速度を反映
            ApplyReboundVelocity();

            // 進行方向に衝突対象があるかどうか確認
            ProcessForwardDetection();

            //  減速の制限
            KeepConstantSpeed();

            
        }

    }
    private void Update()
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, ball_posY, pos.z);
    }


    /// <summary>
    /// 計算した反射ベクトルを反映する
    /// </summary>
    private void ApplyReboundVelocity()
    {

        if (reboundVelocity == null) return;
                
        rigidbody.velocity = reboundVelocity.Value;
        if(max_speed < speed * bounciness )
        {
            speed = max_speed;
        }
        else
        {
            speed *= bounciness;
        }
        reboundVelocity = null;
        canKeepSpeed = true;

        if (GoalObj != null)
        {
            gamemanager.Goal(GoalObj,gameObject.transform.position);
        }

    }

    /// <summary>
    /// 前方方向を監視して1フレーム後に衝突している場合は反射ベクトルを計算する
    /// </summary>
    private void ProcessForwardDetection()
    {
        //移動スピード
        var velocity = rigidbody.velocity;
        //移動ベクトル
        var direction = velocity.normalized;
        //
        var nextMoveDistance = velocity.magnitude * Time.fixedDeltaTime;

        var origin = transform.position - direction * (sphereCastMargin);
        var colliderRadius = sphereCollider.radius;
        var isHit = Physics.SphereCast(origin, colliderRadius, direction, out var hitInfo, nextMoveDistance);
        if (isHit)
        {
            var distance = hitInfo.distance - sphereCastMargin;

            // 次フレームに使う反射速度を計算
            var normal = hitInfo.normal;
            var inVecDir = direction;
            //反射を計算
            var outVecDir = Vector3.Reflect(inVecDir, normal);
            //二次元科
            outVecDir = new Vector3(outVecDir.x, 0.0f, outVecDir.z);
            if (max_speed < speed * bounciness)
            {
                reboundVelocity = outVecDir * max_speed;
            }
            else
            {
                reboundVelocity = outVecDir * speed * bounciness;
            }

            // 衝突予定先に接するように速度を調整
            var adjustVelocity = distance / Time.fixedDeltaTime * direction;
            rigidbody.velocity = adjustVelocity;
            canKeepSpeed = false;

            //当たったオブジェクトのタグ
            string collisionTag = hitInfo.collider.gameObject.tag;
            if (collisionTag == "Goal")
            {
                GoalObj = hitInfo.collider.gameObject;

            }
            else if(collisionTag == "Player")
            {
                HitPlayer(hitInfo);
            }
            else
            {
                se_players.PlayBallHtiWall();
            }

        }
    }

    /// <summary>
    /// 速度を一定に保つ
    /// 衝突や引っかかりによる減速を上書きする役目
    /// </summary>
    private void KeepConstantSpeed()
    {
        if (!canKeepSpeed) return;

        var velocity = rigidbody.velocity;
        var nowSqrSpeed = velocity.sqrMagnitude;
        var sqrSpeed = speed * speed;

        if (!Mathf.Approximately(nowSqrSpeed, sqrSpeed))
        {
            var dir = velocity != Vector3.zero ? velocity.normalized : lastDirection;
            rigidbody.velocity = dir * speed;
        }
    }

    public void Find(Transform parent)
    {
        transform.position = parent.position;
        transform.parent = parent;
        rigidbody.velocity = Vector3.zero;
        speed *= bounciness;
        catchFlg = true;
        sphereCollider.enabled = false;

    }

    public void Fire(Vector3 start_pos, Vector3 FireDirection)
    {
        transform.parent = null;
        reboundVelocity = null;
        canKeepSpeed = false;
        transform.position = start_pos;
        rigidbody.velocity = FireDirection * speed;

        catchFlg = false;
        sphereCollider.enabled = true;
    }

    private void HitPlayer(RaycastHit hit_info)
    {
        PlayerController player = hit_info.collider.gameObject.GetComponent<PlayerController>();


        if (player != null)
        {
            // 法線を取得(基本1面しか当たらない処理になっているので合成を考えない)
            Vector3 normal = hit_info.normal;

            count++;
            print("hit player!" + count);

            if (speed > 40)
            {
                player.ApplyDamage(normal, speed);
            }
            else
            {
                player.Catch();
            }
        }
        
    } 

    public void Block()
    {
        reboundVelocity = null;
        Vector3 new_velocity = (new Vector3(0,1.7f,0) - transform.position).normalized;
        transform.position += new_velocity;
        rigidbody.velocity = new_velocity * speed;
    }
}
