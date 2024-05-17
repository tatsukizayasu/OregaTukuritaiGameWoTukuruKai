using Norikatuo.ReboundShot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector3 _velocity;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private GameObject ball_prefab;

    // �ʒm���󂯎�郁�\�b�h���́uOn + Action���v�ł���K�v������
    private void OnMove(InputValue value)
    {
        // MoveAction�̓��͒l���擾
        var axis = value.Get<Vector2>();

        // �ړ����x��ێ�
        _velocity = new Vector3(axis.x, 0, axis.y);
    }

    private void Update()
    {
        // �I�u�W�F�N�g�ړ�
        transform.position += _velocity * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("����������������������������������");
        if (col.gameObject.name == "Ball(Clone)")
        {
            Collider collider = col.collider;
            Transform parent_transform = collider.transform.parent;
            GameObject parent_object = parent_transform.gameObject;
            //col.rigidbody.velocity = Vector3.zero;
            Debug.Log("wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww");
           // ball.transform.SetParent(transform);
        }
    }
}
