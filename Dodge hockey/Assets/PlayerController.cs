
using UnityEngine;
using System.Collections;

public class Player1 : MonoBehaviour
{
    [SerializeField]
    private string horizontalString;
    [SerializeField]
    private string verticalString;

    private Animator animator;
    private CharacterController cCon;
    private Vector3 velocity;

    void Start()
    {
        animator = GetComponent<Animator>();
        cCon = GetComponent<CharacterController>();
        velocity = Vector3.zero;
    }

    void Update()
    {
        velocity = Vector3.zero;

        var input = new Vector3(Input.GetAxis(horizontalString), 0f, Input.GetAxis(verticalString));

        //�@�����L�[������������Ă���
        if (input.magnitude > 0f)
        {
            transform.LookAt(transform.position + input);

            velocity += input.normalized * 2f;
            //�@�L�[�̉���������������ꍇ�͈ړ����Ȃ�

            print(velocity);
        }

    }
}
