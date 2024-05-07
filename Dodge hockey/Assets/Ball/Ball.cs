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
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Norikatuo.ReboundShot
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class Ball : MonoBehaviour
    {
        [Tooltip("反発係数")]
        [Range(0, 1)]
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

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            sphereCollider = GetComponent<SphereCollider>();
        }

        private void Start()
        {
            Init();
            StartCoroutine(StartWaitForFixedUpdate());
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

        private IEnumerator StartWaitForFixedUpdate()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();

                WaitForFixedUpdate();
            }
        }

        private void WaitForFixedUpdate()
        {
            KeepConstantSpeed();
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

        private void FixedUpdate()
        {
            // 重なりを解消
            OverlapDetection();

            // 前フレームで反射していたら反射後速度を反映
            ApplyReboundVelocity();

            // 進行方向に衝突対象があるかどうか確認
            ProcessForwardDetection();

            // 1フレーム前の進行方向を保存
            UpdateLastDirection();

        }

        /// <summary>
        /// オブジェクトとの重なりを検知して解消するように位置補正する
        /// 主にTransform.positionで移動してきた外部オブジェクトを回避するのに使う
        /// </summary>
        private void OverlapDetection()
        {
            // Overlap
            var colliders = Physics.OverlapSphere(sphereCollider.transform.position, sphereCollider.radius);
            var isOverlap = 1 < colliders.Length;
            if (isOverlap)
            {
                var pushVec = Vector3.zero;
                var pushDistance = 0f;
                var totalPushPos = Vector3.zero;
                var pushCount = 0;

                foreach (var targetCollider in colliders)
                {
                    // 自身のコライダーなら無視する
                    if (targetCollider == sphereCollider) continue;

                    var isCollision = Physics.ComputePenetration(
                        sphereCollider, sphereCollider.transform.position, sphereCollider.transform.rotation,
                        targetCollider, targetCollider.transform.position, targetCollider.transform.rotation,
                        out pushVec, out pushDistance);

                    if (isCollision && pushDistance != 0)
                    {
                        totalPushPos += pushDistance * pushVec;
                        pushCount++;
                    }
                }

                if (pushCount != 0)
                {
                    var pos = transform.position;
                    pos += totalPushPos / pushCount;
                    transform.position = pos;
                }
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

        private void UpdateLastDirection()
        {
            var velocity = rigidbody.velocity;
            if (velocity != Vector3.zero)
            {
                lastDirection = velocity.normalized;
            }
        }

        #region Debug
        private float debugAngle;
        private Vector3 debugDirection;
        private void Update()
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                debugAngle -= 1f;
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                debugAngle += 1f;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                debugDirection = new Vector3(Mathf.Cos(debugAngle * Mathf.Deg2Rad), Mathf.Sin(debugAngle * Mathf.Deg2Rad), 0f);
                rigidbody.velocity = debugDirection * speed;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (EditorApplication.isPlaying && rigidbody.velocity == Vector3.zero)
            {
                Gizmos.color = Color.green;
                debugDirection = new Vector3(Mathf.Cos(debugAngle * Mathf.Deg2Rad), Mathf.Sin(debugAngle * Mathf.Deg2Rad), 0f);
                var colliderRadius = sphereCollider.radius + (useContactOffset ? Physics.defaultContactOffset : 0);
                var isHit = Physics.SphereCast(transform.position, colliderRadius, debugDirection * 10, out var hit);
                if (isHit)
                {
                    Gizmos.DrawRay(transform.position, debugDirection * hit.distance);
                    Gizmos.DrawWireSphere(transform.position + debugDirection * hit.distance, colliderRadius);
                }
                else
                {
                    Gizmos.DrawRay(transform.position, transform.position + debugDirection * 10);
                }
            }
        }
    }
#endif
    #endregion

}