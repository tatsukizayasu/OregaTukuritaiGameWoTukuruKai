
using System.Collections;
using UnityEngine;

namespace Norikatuo.ReboundShot
{


    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]


    public class Ball : MonoBehaviour
    {
        [Tooltip("�����W��")]
        [Range(0, 2)]
        [SerializeField] private float bounciness = 1.0f;

        [Tooltip("�e�̑��x")]
        public float speed = 10.0f;

        private Rigidbody rigidbody;
        private SphereCollider sphereCollider;
        private float defaultContactOffset;
        private const float sphereCastMargin = 0.01f; // SphereCast�p�̃}�[�W��
        private Vector3? reboundVelocity; // ���ˑ��x
        private Vector3 lastDirection;
        private bool canKeepSpeed;

        public Vector3 CatchBallLocathion;
        private bool C_F = false;



        //Start()�O�Ɏ��s�@Rigidbody�ESphereCollider���쐬
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            sphereCollider = GetComponent<SphereCollider>();
        }

        //Awake()��Ɏ��s
        //������ݒ�
        private void Start()
        {
            Init();

            Vector3 debugDirection;

            debugDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad), Mathf.Sin(Mathf.Deg2Rad), 0f);
            rigidbody.velocity = debugDirection * speed;


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


        private void FixedUpdate()
        {
            
            if(C_F)
            {

            }
            else
            {
                //Debug.Log("Move");
                // �O�t���[���Ŕ��˂��Ă����甽�ˌ㑬�x�𔽉f
                ApplyReboundVelocity();

                // �i�s�����ɏՓˑΏۂ����邩�ǂ����m�F
                ProcessForwardDetection();

                //�����̐���
                KeepConstantSpeed();
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
            //�ړ��X�s�[�h
            var velocity = rigidbody.velocity;
            //�ړ��x�N�g��
            var direction = velocity.normalized;
            //
            var nextMoveDistance = velocity.magnitude * Time.fixedDeltaTime;

            var origin = transform.position - direction * (sphereCastMargin);
            var colliderRadius = sphereCollider.radius ;
            var isHit = Physics.SphereCast(origin, colliderRadius, direction, out var hitInfo, nextMoveDistance);
            if (isHit)
            {
                var distance = hitInfo.distance - sphereCastMargin;

                // ���t���[���Ɏg�����ˑ��x���v�Z
                var normal = hitInfo.normal;
                var inVecDir = direction;
                //���˂��v�Z
                var outVecDir = Vector3.Reflect(inVecDir, normal);
                //�񎟌���
                outVecDir = new Vector3(outVecDir.x, 0.0f, outVecDir.z);
                reboundVelocity = outVecDir * speed * bounciness;

                // �Փ˗\���ɐڂ���悤�ɑ��x�𒲐�
                var adjustVelocity = distance / Time.fixedDeltaTime * direction;
                rigidbody.velocity = adjustVelocity;
                canKeepSpeed = false;

            }
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

        public void Find(Transform parent)
        {
            transform.parent = parent;
            rigidbody.velocity = Vector3.zero;
            C_F = true;
            Debug.Log("�ɂ�");
        }

        public void Fire(Transform parent ,Vector3 FireDirection)
        {

            rigidbody.velocity = FireDirection * speed;
            transform.parent = null;
            C_F = false;
        }

    }
}