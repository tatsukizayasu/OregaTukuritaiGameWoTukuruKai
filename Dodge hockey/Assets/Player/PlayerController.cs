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
        has_ball = false;
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

    private void OnLook(InputValue value)
    {
        look_vector = value.Get<Vector2>();
    }     

    private void OnBallHandle(InputValue value)
    {
        //  ball�������Ă���Ƃ�������A�����Ă��Ȃ���΃L���b�`����
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
                //  �����w�肵�ē�����
                ball_pos = transform.position + (new Vector3(look_vector.x, 0.0f, look_vector.y) * 2);
                ball.Fire(ball_pos, new Vector3(look_vector.x, 0.0f, look_vector.y));
            }
            else
            {
                //  �����w�肵�Ă��Ȃ��Ƃ��A�����Ă�������ɓ�����
                ball_pos = transform.position + (transform.forward * 2);
                ball.Fire(ball_pos, transform.forward);
            }
        }

        has_ball = false;
    }

    private void Catch()
    {
        Vector3 sphereCenter = transform.position;

        // OverlapSphere���g�p���Ĕ͈͓��̂��ׂẴR���C�_�[���擾
        Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, status.CatchRamge);

        // �擾�����R���C�_�[�����[�v���āA�Q�[���I�u�W�F�N�g���擾
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