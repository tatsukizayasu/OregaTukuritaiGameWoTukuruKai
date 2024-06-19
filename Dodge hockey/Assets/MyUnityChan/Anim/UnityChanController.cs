using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class UnityChanController : MonoBehaviour
{
    private Animator animator; //Unity�����̃W�����v�ݒ肷�邽�߂�Animator

    // �ő�̉�]�p���x[deg/s]
    [SerializeField] private float maxAngularSpeed = Mathf.Infinity;

    // �i�s�����Ɍ����̂ɂ����邨���悻�̎���[s]
    [SerializeField] private float smoothTime = 0.1f;

    private float currentAngularVelocity;

    private Transform _transform;
    private Vector3 prevPosition;

    // Start is called before the first frame update
    void Start()
    {
        //Untiy������Animator���擾����B
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
        // ���݃t���[���̃��[���h�ʒu
        Vector3 position = _transform.position;

        //  �ړ��ʂ̌v�Z
        Vector3 delta = position - prevPosition;

        //  �O�t���[���ʒu�̍X�V
        prevPosition = position;

        //  �Î~��Ԃ͐i�s���������ł��Ȃ����߉�]���Ȃ�
        if(delta == Vector3.zero)
        {
            return;
        }

        //  �i�s�����Ɍ����N�H�[�^�j�I�����擾
        Quaternion targetRot = Quaternion.LookRotation(delta, Vector3.up);

        // ���݂̌����Ɛi�s�����Ƃ̊p�x�����v�Z
        float diffAngle = Vector3.Angle(_transform.forward, delta);
        // ���݃t���[���ŉ�]����p�x�̌v�Z
        float rotAngle = Mathf.SmoothDampAngle(
            0,
            diffAngle,
            ref currentAngularVelocity,
            smoothTime,
            maxAngularSpeed
        );
        // ���݃t���[���ɂ������]���v�Z
        Quaternion nextRot = Quaternion.RotateTowards(
            _transform.rotation,
            targetRot,
            rotAngle
        );

        // �I�u�W�F�N�g�̉�]�ɔ��f
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
