using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class UnityChanController : MonoBehaviour
{
    private Animator animator; //Unityちゃんのジャンプ設定するためのAnimator

    // 最大の回転角速度[deg/s]
    [SerializeField] private float maxAngularSpeed = Mathf.Infinity;

    // 進行方向に向くのにかかるおおよその時間[s]
    [SerializeField] private float smoothTime = 0.1f;

    private float currentAngularVelocity;

    private Transform _transform;
    private Vector3 prevPosition;

    // Start is called before the first frame update
    void Start()
    {
        //UntiyちゃんのAnimatorを取得する。
        animator = GetComponent<Animator>();

        _transform = transform;
        prevPosition = _transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        RotateToMovementDirection();
    }

    private void RotateToMovementDirection()
    {
        // 現在フレームのワールド位置
        Vector3 position = _transform.position;

        //  移動量の計算
        Vector3 delta = position - prevPosition;

        //  前フレーム位置の更新
        prevPosition = position;

        //  静止状態は進行方向を特定できないため回転しない
        if(delta == Vector3.zero)
        {
            return;
        }

        //  進行方向に向くクォータニオンを取得
        Quaternion targetRot = Quaternion.LookRotation(delta, Vector3.up);

        // 現在の向きと進行方向との角度差を計算
        float diffAngle = Vector3.Angle(_transform.forward, delta);
        // 現在フレームで回転する角度の計算
        float rotAngle = Mathf.SmoothDampAngle(
            0,
            diffAngle,
            ref currentAngularVelocity,
            smoothTime,
            maxAngularSpeed
        );
        // 現在フレームにおける回転を計算
        Quaternion nextRot = Quaternion.RotateTowards(
            _transform.rotation,
            targetRot,
            rotAngle
        );

        // オブジェクトの回転に反映
        _transform.rotation = nextRot;
    }

    private void OnMove(InputValue value)
    {
        if(value.Get<Vector2>() != Vector2.zero)
        {
            animator.CrossFade("Running loop", 0.1f);
        }
        else
        {
            animator.CrossFade("Standing loop", 0.3f);
        }
    }
}
