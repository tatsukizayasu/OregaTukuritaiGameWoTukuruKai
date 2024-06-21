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

    private SE_Player se_players;

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
        se_players = GetComponent<SE_Player>();
    }

    private void Update()
    {
        // オブジェクト移動
        transform.position += _velocity * speed * Time.deltaTime;
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
            float ball_y = ball.transform.position.y;
            if (look_vector != Vector2.zero)
            {
                //  方向指定して投げる
                ball_pos = transform.position + (new Vector3(look_vector.x, 0.0f, look_vector.y) * 2);
                ball_pos.y = ball_y;
                ball.Fire(ball_pos, new Vector3(look_vector.x, 0.0f, look_vector.y));
            }
            else
            {
                //  方向指定していないとき、向いている方向に投げる
                ball_pos = transform.position + (new Vector3(transform.forward.x, 0.0f, transform.forward.z) * 2);
                ball_pos.y = ball_y;
                ball.Fire(ball_pos, transform.forward);
            }
            //SE
            se_players.PlayThrow();
        }
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
                //SE
                se_players.PlayCatch();
            }
        }
    }

}