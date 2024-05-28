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
        [Tooltip("�����W��")]
        [Range(0, 2)]
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

        public Vector3 CatchBallLocathion;

        private BallCondition_test ballCondition = BallCondition_test.stop;
        public BallCondition_test BallCondition { get { return ballCondition; } set { ballCondition = value; } }   

        public void SetVelocity(Vector3 vector)
        {
                    rigidbody.velocity = vector * speed;
        }

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
                    // �O�t���[���Ŕ��˂��Ă����甽�ˌ㑬�x�𔽉f
                    ApplyReboundVelocity();

                    // �i�s�����ɏՓˑΏۂ����邩�ǂ����m�F
                    ProcessForwardDetection();

                    //�����̐���
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
                    //    Debug.LogError("Player1 �I�u�W�F�N�g��������܂���ł���");
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
                        Debug.LogError("Player1 �I�u�W�F�N�g��������܂���ł���");
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



    }
}