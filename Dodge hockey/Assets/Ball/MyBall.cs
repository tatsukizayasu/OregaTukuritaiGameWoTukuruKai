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
        GetComponent<Rigidbody>().velocity = new Vector3(1.0f, 0.0f, 0.0f);
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
    }

    private void DetectCollision()
    {
        Vector3 sphereCenter = transform.position;

        // OverlapSphere���g�p���Ĕ͈͓��̂��ׂẴR���C�_�[���擾
        Collider[] hitColliders = Physics.OverlapSphere(sphereCenter,sphereCollider.radius);

        if (hitColliders != null)
        {
            foreach(Collider collider in hitColliders)
            {
                GameObject gameObject = collider.gameObject;
                print(gameObject.name);
            }

            Vector3 direction = GetComponent<Rigidbody>().velocity.normalized;
            RaycastHit hitInfo;
            bool isHit = Physics.SphereCast(oldPosition, sphereCollider.radius, direction, out hitInfo);
            
            if(isHit)
            {

                Collider collider = hitInfo.collider;
                Transform parent_transform = collider.transform.parent;
                GameObject parent_object = parent_transform.gameObject;

                print("gameobject : " + parent_object);
            }

        }



    }
}
