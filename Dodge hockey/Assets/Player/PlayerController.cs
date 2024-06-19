using Norikatuo.ReboundShot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerStatus))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private Vector3 _velocity;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private GameObject ball_prefab;
    private PlayerStatus status;

    private Vector2 look_vector;
    private bool has_ball;

    // 通知を受け取るメソッド名は「On + Action名」である必要がある
    private void OnMove(InputValue value)
    {
        // MoveActionの入力値を取得
        var axis = value.Get<Vector2>();

        // 移動速度を保持
        _velocity = new Vector3(axis.x, 0, axis.y);
    }

    private void Start()
    {
        status = GetComponent<PlayerStatus>();
        has_ball = false;
    }

    private void Update()
    {
        // オブジェクト移動
        transform.position += _velocity * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("あああああああああああああああああ");
        if (col.gameObject.name == "Ball(Clone)")
        {
            Collider collider = col.collider;
            Transform parent_transform = collider.transform.parent;
            GameObject parent_object = parent_transform.gameObject;

            col.transform.parent = this.transform;
            parent_object.GetComponent<Ball_test>().BallCondition = BallCondition_test.stop;

            //col.rigidbody.velocity = Vector3.zero;
            //Debug.Log("wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww");
           // ball.transform.SetParent(transform);
        }
    }

    private void OnLook(InputValue value)
    {
        look_vector = value.Get<Vector2>();
    }     

    private void OnBallHandle(InputValue value)
    {
        //  ballを持っているとき投げる、持っていなければキャッチする
        Transform child = transform.Find("Ball(Clone)");
        if (child != null)
        {
            Throw(child);
        }
        else
        {
            Catch();
        }

    }

    private void Throw(Transform child)
    {        
        Ball ball = child.GetComponent<Ball>();
        if (ball != null)
        {
            Vector3 ball_pos;
            if (look_vector != Vector2.zero)
            {
                //  方向指定して投げる
                ball_pos = transform.position + (new Vector3(look_vector.x, 0.0f, look_vector.y) * 2);
                ball.Fire(ball_pos, new Vector3(look_vector.x, 0.0f, look_vector.y));
            }
            else
            {
                //  方向指定していないとき、向いている方向に投げる
                ball_pos = transform.position + (transform.forward * 2);
                ball.Fire(ball_pos, transform.forward);
            }
        }

        has_ball = false;
    }

    private void Catch()
    {
        Vector3 sphereCenter = transform.position;

        // OverlapSphereを使用して範囲内のすべてのコライダーを取得
        Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, status.CatchRamge);

        // 取得したコライダーをループして、ゲームオブジェクトを取得
        foreach (Collider hitCollider in hitColliders)
        {
            print(hitCollider);
            GameObject hitObject = hitCollider.gameObject;
            Ball ball = hitObject.GetComponent<Ball>();
            if ((ball != null) && (!ball.CatchFlg))
            {
                ball.Find(transform);
                has_ball = true;
            }
        }
    }

}