using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class MyBall : MonoBehaviour
{
    [Tooltip("反発係数")]
    [Range(0, 1)]
    [SerializeField] private float bounciness = 1.0f; // 反発係数

    [Tooltip("弾の速度")]
    public float speed = 10.0f; // 弾の速度

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
        // 高速移動時のすり抜けを防止するために設定
        if (!sphereCollider.isTrigger && rigitBody.collisionDetectionMode != CollisionDetectionMode.ContinuousDynamic)
        {
            rigitBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        // 重力の使用を禁止
        rigitBody.useGravity = false;
        transform.position = new Vector3(0.0f, 1.5f, 0.0f);
        oldPosition = transform.position;
        rigitBody.constraints |= RigidbodyConstraints.FreezePositionY;

        //  自身を無視するようにLayerに自身を追加する
        gameObject.layer = LayerMask.NameToLayer("SphereMoveLayer");
    }

    private void DetectCollision()
    {
        Vector3 sphereCenter = transform.position;
        Vector3 velocity = GetComponent<Rigidbody>().velocity;
        float maxDistance = velocity.magnitude * (Time.fixedDeltaTime * 2);
        LayerMask ignoreLayer = gameObject.layer;

        // OverlapSphereを使用して範囲内のすべてのコライダーを取得
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
