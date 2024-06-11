using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class MyBall : MonoBehaviour
{
    [Tooltip("�����W��")]
    [Range(0, 1)]
    [SerializeField] private float bounciness = 1.0f; // �����W��

    [Tooltip("�e�̑��x")]
    public float speed = 10.0f; // �e�̑��x

    SphereCollider sphereCollider;
    Vector3 oldPosition;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
        GetComponent<Rigidbody>().velocity = new Vector3(1.0f, 0.0f, 0.0f) * 10;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DetectCollision();

        oldPosition = transform.position;
    }

    private void Init()
    {
        Rigidbody rigitBody = GetComponent<Rigidbody>();
        // �����ړ����̂��蔲����h�~���邽�߂ɐݒ�
        if (!sphereCollider.isTrigger && rigitBody.collisionDetectionMode != CollisionDetectionMode.ContinuousDynamic)
        {
            rigitBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        // �d�͂̎g�p���֎~
        rigitBody.useGravity = false;
        transform.position = new Vector3(0.0f, 1.5f, 0.0f);
        oldPosition = transform.position;
        rigitBody.constraints |= RigidbodyConstraints.FreezePositionY;

        //  ���g�𖳎�����悤��Layer�Ɏ��g��ǉ�����
        gameObject.layer = LayerMask.NameToLayer("SphereMoveLayer");
    }

    private void DetectCollision()
    {
        Vector3 sphereCenter = transform.position;
        Vector3 velocity = GetComponent<Rigidbody>().velocity;
        float maxDistance = velocity.magnitude * (Time.fixedDeltaTime * 2);
        LayerMask ignoreLayer = gameObject.layer;

        // OverlapSphere���g�p���Ĕ͈͓��̂��ׂẴR���C�_�[���擾
        Collider[] hitColliders = Physics.OverlapSphere(sphereCenter,sphereCollider.radius,ignoreLayer);

        if (hitColliders.Length != 0)
        {
            foreach(Collider collider in hitColliders)
            {
                GameObject gameObject = collider.gameObject;
                print(gameObject.name);
            };

            Vector3 direction = GetComponent<Rigidbody>().velocity.normalized;
            RaycastHit hitInfo;
            bool isHit = Physics.SphereCast(oldPosition, sphereCollider.radius, direction, out hitInfo, maxDistance);
            
            if(isHit)
            {
                Collider hitCollider = hitInfo.collider;
                GameObject hitObject = hitCollider.gameObject;

                print("gameobject : " + hitObject);
            }

        }



    }
}
