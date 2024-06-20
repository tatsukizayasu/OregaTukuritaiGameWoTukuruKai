
using System.Collections;
using UnityEngine;



[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]


public class Ball : MonoBehaviour
{
    [Tooltip("�����W��")]
    [Range(0, 2)]
    [SerializeField] private float bounciness = 1.0f;

    [SerializeField] private const float ball_posY = 1.7f;

    const float max_speed = 100.0f;

    [Tooltip("�e�̑��x")]
    public float speed = 10.0f;


    
    private Rigidbody rigidbody;
    private SphereCollider sphereCollider;
    private float defaultContactOffset;
    private const float sphereCastMargin = 0.01f; // SphereCast�p�̃}�[�W��
    private Vector3? reboundVelocity; // ���ˑ��x
    private Vector3 lastDirection;
    private bool canKeepSpeed;

    private bool catchFlg = false;
    public bool CatchFlg { get; set; }
    private GameObject GoalObj = null;

    GameManager gamemanager;

    public void SetVelocity(Vector3 velocity)
    {
        rigidbody.velocity = velocity;
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

        //Vector3 debugDirection;

        //debugDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad), Mathf.Sin(Mathf.Deg2Rad), 0f);
        //rigidbody.velocity = debugDirection * speed;

        gamemanager = FindObjectOfType<GameManager>();
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

        if (catchFlg)
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
    private void Update()
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, ball_posY, pos.z);
    }


    /// <summary>
    /// �v�Z�������˃x�N�g���𔽉f����
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
            

            gamemanager.Goal(GoalObj);
        }

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
        var colliderRadius = sphereCollider.radius;
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
            if (max_speed < speed * bounciness)
            {
                reboundVelocity = outVecDir * max_speed;
            }
            else
            {
                reboundVelocity = outVecDir * speed * bounciness;
            }

            // �Փ˗\���ɐڂ���悤�ɑ��x�𒲐�
            var adjustVelocity = distance / Time.fixedDeltaTime * direction;
            rigidbody.velocity = adjustVelocity;
            canKeepSpeed = false;

            //���������I�u�W�F�N�g�̃^�O
            string collisionTag = hitInfo.collider.gameObject.tag;
            if (collisionTag == "Goal")
            {
                GoalObj = hitInfo.collider.gameObject;
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

    public void Find(Transform parent)
    {
        transform.parent = parent;
        rigidbody.velocity = Vector3.zero;
        catchFlg = true;
    }

    public void Fire(Vector3 start_pos, Vector3 FireDirection)
    {
        transform.parent = null;
        reboundVelocity = null;
        canKeepSpeed = false;
        transform.position = start_pos;
        rigidbody.velocity = FireDirection * speed;

        catchFlg = false;
    }

}
