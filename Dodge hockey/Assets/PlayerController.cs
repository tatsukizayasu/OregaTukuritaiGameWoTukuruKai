
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
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
        Vector3 pos = new Vector3();
        pos = transform.position;

        velocity = Vector3.zero;

        var input = new Vector3(Input.GetAxis(horizontalString), 0f, Input.GetAxis(verticalString));

        //@•ûŒüƒL[‚ª‘½­‰Ÿ‚³‚ê‚Ä‚¢‚é
        if (input.magnitude > 0f)
        {
            pos += input.normalized * 0.05f;
            transform.position = pos;
        }

    }
}
