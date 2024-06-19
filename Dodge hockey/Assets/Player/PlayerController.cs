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

    private void OnCatch(InputValue value)
    {
        // 子オブジェクトを格納する配列を作成
        var Children = new Transform[transform.childCount];
        int ChildCount = 0;
        bool ballFound = false; // Ball が見つかったかどうかを追跡するフラグ

        // 子オブジェクトを配列に格納し、特定の名前を持つオブジェクトを探す
        foreach (Transform Child in transform)
        {
            Children[ChildCount++] = Child;
            if (Child.name == "Ball(Clone)")
            {
                // Ball スクリプトを取得
                Ball ballscript = Child.GetComponent<Ball>();

                if (ballscript != null)
                {
                    Vector3 BallFire = new Vector3(10f, 0f, 10f);
                    // Ball スクリプトの関数を実行
                    ballscript.Fire(transform ,BallFire);
                    ballFound = true; // Ball が見つかったのでフラグを更新
                }
                else
                {
                    Debug.LogError("Ball script not found on the Ball object.");
                }
            }
        }

        // Ball が見つからなかった場合の処理（キャッチしていない時）
        if (!ballFound)
        {
            Vector3 sphereCenter = transform.position;

            // OverlapSphereを使用して範囲内のすべてのコライダーを取得
            Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, status.CatchRamge);

            // 取得したコライダーをループして、ゲームオブジェクトを取得
            foreach (Collider hitCollider in hitColliders)
            {
                GameObject hitObject = hitCollider.gameObject;
                Ball ball = hitObject.GetComponent<Ball>();
                if (ball != null)
                {
                    ball.Find(transform);
                }
            }
        }

    }
}
