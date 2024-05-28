//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Ball : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {

//        Rigidbody rb = this.GetComponent<Rigidbody>();  // rigidbodyを取得
//        Vector3 force = new Vector3(0.0f, 0.0f, 5.0f);  // 力を設定
//        rb.AddForce(force, ForceMode.Impulse);          // 力を加える
//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//}
using UnityEngine;

using System.Collections;
using UnityEngine;

namespace Norikatuo.ReboundShot
{
    public enum BallCondition_test
    {
        stop,
        Move,
        CatchInit,
        Catch,
    }

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]


    public class Ball_test : MonoBehaviour
    {
        [Tooltip("反発係数")]
        [Range(0, 2)]
        [SerializeField] private float bounciness = 1.0f;

        [Tooltip("弾の速度")]
        public float speed = 10.0f;

        [Tooltip("ONならDefaultContactOffsetの値を衝突検知に使用する")]
        [SerializeField] private bool useContactOffset = true;

        private Rigidbody rigidbody;
        private SphereCollider sphereCollider;
        private float defaultContactOffset;
        private const float sphereCastMargin = 0.01f; // SphereCast用のマージン
        private Vector3? reboundVelocity; // 反射速度
        private Vector3 lastDirection;
        private bool canKeepSpeed;

        public Vector3 CatchBallLocathion;

        private BallCondition_test ballCondition = BallCondition_test.stop;
        public BallCondition_test BallCondition { get { return ballCondition; } set { ballCondition = value; } }   

        public void SetVelocity(Vector3 vector)
        {
                    rigidbody.velocity = vector * speed;
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

            Vector3 debugDirection;

            debugDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad), Mathf.Sin(Mathf.Deg2Rad), 0f);
            rigidbody.velocity = debugDirection * speed;


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



            switch (ballCondition)
            {
                case BallCondition_test.stop:
                    Debug.Log("stop");

                    rigidbody.velocity = Vector3.zero;

                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        ballCondition = BallCondition_test.Move;

                        Vector3 debugDirection;
                        debugDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad), Mathf.Sin(Mathf.Deg2Rad), 0f);
                        rigidbody.velocity = debugDirection * speed;
                    }
                    break;
                case BallCondition_test.Move:
                    Debug.Log("Move");
                    // 前フレームで反射していたら反射後速度を反映
                    ApplyReboundVelocity();

                    // 進行方向に衝突対象があるかどうか確認
                    ProcessForwardDetection();

                    //減速の制限
                    KeepConstantSpeed();

                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        ballCondition = BallCondition_test.CatchInit;
                    }

                    break;
                case BallCondition_test.CatchInit:
                    //Debug.Log("CatchInit");

                    //GameObject obj = GameObject.Find("Player1");
                    //if (obj == null)
                    //{
                    //    Debug.LogError("Player1 オブジェクトが見つかりませんでした");
                    //    break;
                    //}
                    //Vector3 locathion = obj.transform.position;
                    //locathion.x += 10f;

                    //CatchBallLocathion = locathion;

                    ballCondition = BallCondition_test.Catch;

                    break;
                case BallCondition_test.Catch:

                    Debug.Log("CatchInit");

                    GameObject obj = GameObject.Find("Player1");
                    if (obj == null)
                    {
                        Debug.LogError("Player1 オブジェクトが見つかりませんでした");
                        break;
                    }
                    Vector3 locathion = obj.transform.position;
                    locathion.x += 10f;

                    CatchBallLocathion = locathion;



                    Debug.Log("Catch");
                    Debug.Log(CatchBallLocathion);
                    this.transform.position = CatchBallLocathion;
                    rigidbody.velocity = Vector3.zero;

                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        ballCondition = BallCondition_test.stop;
                    }
                    break;
                    //this.transform.SetParent
            }

        }


        /// <summary>
        /// 計算した反射ベクトルを反映する
        /// </summary>
        private void ApplyReboundVelocity()
        {
            if (reboundVelocity == null) return;

            rigidbody.velocity = reboundVelocity.Value;
            speed *= bounciness;
            reboundVelocity = null;
            canKeepSpeed = true;
        }

        /// <summary>
        /// 前方方向を監視して1フレーム後に衝突している場合は反射ベクトルを計算する
        /// </summary>
        private void ProcessForwardDetection()
        {
            var velocity = rigidbody.velocity;
            var direction = velocity.normalized;

            var offset = useContactOffset ? defaultContactOffset * 2 : 0;
            var origin = transform.position - direction * (sphereCastMargin + offset);
            var colliderRadius = sphereCollider.radius + offset;
            var isHit = Physics.SphereCast(origin, colliderRadius, direction, out var hitInfo);
            if (isHit)
            {
                var distance = hitInfo.distance - sphereCastMargin;
                var nextMoveDistance = velocity.magnitude * Time.fixedDeltaTime;
                if (distance <= nextMoveDistance)
                {
                    // 次フレームに使う反射速度を計算
                    var normal = hitInfo.normal;
                    var inVecDir = direction;
                    var outVecDir = Vector3.Reflect(inVecDir, normal);
                    outVecDir = new Vector3(outVecDir.x, 0.0f, outVecDir.z);
                    reboundVelocity = outVecDir * speed * bounciness;

                    // 衝突予定先に接するように速度を調整
                    var adjustVelocity = distance / Time.fixedDeltaTime * direction;
                    rigidbody.velocity = adjustVelocity;
                    canKeepSpeed = false;
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



    }
}