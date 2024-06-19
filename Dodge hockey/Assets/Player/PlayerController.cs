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

    // �ʒm���󂯎�郁�\�b�h���́uOn + Action���v�ł���K�v������
    private void OnMove(InputValue value)
    {
        // MoveAction�̓��͒l���擾
        var axis = value.Get<Vector2>();

        // �ړ����x��ێ�
        _velocity = new Vector3(axis.x, 0, axis.y);
    }

    private void Start()
    {
        status = GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        // �I�u�W�F�N�g�ړ�
        transform.position += _velocity * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("����������������������������������");
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
        // �q�I�u�W�F�N�g���i�[����z����쐬
        var Children = new Transform[transform.childCount];
        int ChildCount = 0;
        bool ballFound = false; // Ball �������������ǂ�����ǐՂ���t���O

        // �q�I�u�W�F�N�g��z��Ɋi�[���A����̖��O�����I�u�W�F�N�g��T��
        foreach (Transform Child in transform)
        {
            Children[ChildCount++] = Child;
            if (Child.name == "Ball(Clone)")
            {
                // Ball �X�N���v�g���擾
                Ball ballscript = Child.GetComponent<Ball>();

                if (ballscript != null)
                {
                    Vector3 BallFire = new Vector3(10f, 0f, 10f);
                    // Ball �X�N���v�g�̊֐������s
                    ballscript.Fire(transform ,BallFire);
                    ballFound = true; // Ball �����������̂Ńt���O���X�V
                }
                else
                {
                    Debug.LogError("Ball script not found on the Ball object.");
                }
            }
        }

        // Ball ��������Ȃ������ꍇ�̏����i�L���b�`���Ă��Ȃ����j
        if (!ballFound)
        {
            Vector3 sphereCenter = transform.position;

            // OverlapSphere���g�p���Ĕ͈͓��̂��ׂẴR���C�_�[���擾
            Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, status.CatchRamge);

            // �擾�����R���C�_�[�����[�v���āA�Q�[���I�u�W�F�N�g���擾
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
