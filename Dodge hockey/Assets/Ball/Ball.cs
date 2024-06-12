
using System.Collections;
using UnityEngine;

namespace Norikatuo.ReboundShot
{


    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]


    public class Ball : MonoBehaviour
    {
        [Tooltip("反発係数")]
        [Range(0, 2)]
        [SerializeField] private float bounciness = 1.0f;

        [Tooltip("弾の速度")]
        public float speed = 10.0f;

        private Rigidbody rigidbody;
        private SphereCollider sphereCollider;
        private float defaultContactOffset;
        private const float sphereCastMargin = 0.01f; // SphereCast用のマージン
        private Vector3? reboundVelocity; // 反射速度
        private Vector3 lastDirection;
        private bool canKeepSpeed;

        public Vector3 CatchBallLocathion;
        private bool C_F = false;



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
            
            if(C_F)
            {

            }
            else
            {
                //Debug.Log("Move");
                // 前フレームで反射していたら反射後速度を反映
                ApplyReboundVelocity();

                // 進行方向に衝突対象があるかどうか確認
                ProcessForwardDetection();

                //減速の制限
                KeepConstantSpeed();
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
            //移動スピード
            var velocity = rigidbody.velocity;
            //移動ベクトル
            var direction = velocity.normalized;
            //
            var nextMoveDistance = velocity.magnitude * Time.fixedDeltaTime;

            var origin = transform.position - direction * (sphereCastMargin);
            var colliderRadius = sphereCollider.radius ;
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
                reboundVelocity = outVecDir * speed * bounciness;

                // 衝突予定先に接するように速度を調整
                var adjustVelocity = distance / Time.fixedDeltaTime * direction;
                rigidbody.velocity = adjustVelocity;
                canKeepSpeed = false;

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
            transform.parent = parent;
            rigidbody.velocity = Vector3.zero;
            C_F = true;
            Debug.Log("にぇ");
        }

        public void Fire(Transform parent ,Vector3 FireDirection)
        {

            rigidbody.velocity = FireDirection * speed;
            transform.parent = null;
            C_F = false;
        }

    }
}