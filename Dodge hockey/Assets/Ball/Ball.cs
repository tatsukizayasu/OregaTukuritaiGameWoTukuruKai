//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Ball : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {

//        Rigidbody rb = this.GetComponent<Rigidbody>();  // rigidbody���擾
//        Vector3 force = new Vector3(0.0f, 0.0f, 5.0f);  // �͂�ݒ�
//        rb.AddForce(force, ForceMode.Impulse);          // �͂�������
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
        [Tooltip("�����W��")]
        [Range(0, 1)]
        [SerializeField] private float bounciness = 1.0f;
        [Tooltip("�e�̑��x")]
        public float speed = 10.0f;
        [Tooltip("ON�Ȃ�DefaultContactOffset�̒l���Փˌ��m�Ɏg�p����")]
        [SerializeField] private bool useContactOffset = true;

        private Rigidbody rigidbody;
        private SphereCollider sphereCollider;
        private float defaultContactOffset;
        private const float sphereCastMargin = 0.01f; // SphereCast�p�̃}�[�W��
        private Vector3? reboundVelocity; // ���ˑ��x
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
            // isTrigger=false �Ŏg�p����ꍇ��Continuous Dynamics�ɐݒ�
            if (!sphereCollider.isTrigger && rigidbody.collisionDetectionMode != CollisionDetectionMode.ContinuousDynamic)
            {
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }

            // �d�͂̎g�p�͋֎~
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
        /// ���x�����ɕۂ�
        /// �Փ˂����������ɂ�錸�����㏑��������
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
            // �d�Ȃ������
            OverlapDetection();

            // �O�t���[���Ŕ��˂��Ă����甽�ˌ㑬�x�𔽉f
            ApplyReboundVelocity();

            // �i�s�����ɏՓˑΏۂ����邩�ǂ����m�F
            ProcessForwardDetection();

            // 1�t���[���O�̐i�s������ۑ�
            UpdateLastDirection();

        }

        /// <summary>
        /// �I�u�W�F�N�g�Ƃ̏d�Ȃ�����m���ĉ�������悤�Ɉʒu�␳����
        /// ���Transform.position�ňړ����Ă����O���I�u�W�F�N�g���������̂Ɏg��
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
                    // ���g�̃R���C�_�[�Ȃ疳������
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
        /// �v�Z�������˃x�N�g���𔽉f����
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
        /// �O���������Ď�����1�t���[����ɏՓ˂��Ă���ꍇ�͔��˃x�N�g�����v�Z����
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
                    // ���t���[���Ɏg�����ˑ��x���v�Z
                    var normal = hitInfo.normal;
                    var inVecDir = direction;
                    var outVecDir = Vector3.Reflect(inVecDir, normal);
                    outVecDir = new Vector3(outVecDir.x, 0.0f, outVecDir.z);
                    reboundVelocity = outVecDir * speed * bounciness;

                    // �Փ˗\���ɐڂ���悤�ɑ��x�𒲐�
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