using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerStatus))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private Vector3 spawn_pos;
    private Vector3 _velocity;
    private PlayerStatus status;

    private Transform hand_position;
    private Vector2 look_vector;

    [SerializeField] private const float death_time = 3.0f;
    private float count_death_time;

    private bool hasController;

    private SE_Player se_players;

    // �ʒm���󂯎�郁�\�b�h���́uOn + Action���v�ł���K�v������
    private void OnMove(InputValue value)
    {
        // MoveAction�̓��͒l���擾
        Vector2 axis = value.Get<Vector2>();
        axis = axis.normalized;

        // �ړ����x��ێ�
        _velocity = new Vector3(axis.x, 0, axis.y);
    }

    private void Start()
    {
        spawn_pos = transform.position;
        status = GetComponent<PlayerStatus>();
        count_death_time = 0;
        hasController = true;

        // ���f������"Character1_RightHandThumb4"���ċA�I�ɒT��
        hand_position = FindChildByName(transform, "Character1_RightHand");
        if (hand_position != null)
        {
            print(hand_position);
        }
        else
        {
            print("hand not found ");
        }
        se_players = GetComponent<SE_Player>();

    }

    private void Update()
    {
        // �I�u�W�F�N�g�ړ�
        if (hasController)
        {
            transform.position += _velocity * status.Speed * Time.deltaTime;
        }

        if (status.Life <= 0)
        {
            count_death_time -= Time.deltaTime;
            if(count_death_time <= 0)
            {
                Respawn();
            }
        }
    }

    private void OnLook(InputValue value)
    {
        look_vector = value.Get<Vector2>().normalized;

        Transform child = hand_position.Find("Ball(Clone)");
        //  �{�[���������Ă��ĉE�X�e�B�b�N�̓��͂�����Ƃ��A�_���Ă������������
        if ((child != null) && (look_vector != Vector2.zero))
        {
            //  �\���Ă���Ƃ��ɃX�s�[�h��x������
            status.Speed = 3.0f;

            UnityChanController controller = GetComponent<UnityChanController>();
            if (controller != null)
            {
                controller.IsRotation = false;
            }
            Vector3 look_vector3 = new Vector3(look_vector.x, 0.0f, look_vector.y);
            Quaternion rotation = Quaternion.LookRotation(look_vector3, Vector3.up);
            transform.rotation = rotation;
        }
        else
        {
            UnityChanController controller = GetComponent<UnityChanController>();
            if (controller != null)
            {
                controller.IsRotation = true;
            }
            status.Speed = status.default_speed;
        }
    }

    private void OnBallHandle(InputValue value)
    {
        //  ball�������Ă���Ƃ�������A�����Ă��Ȃ���΃L���b�`����
        Transform child = hand_position.Find("Ball(Clone)");
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
        if (!hasController) { return; }

        Ball ball = child.GetComponent<Ball>();
        if (ball != null)
        {
            Vector3 ball_pos;
            float ball_y = ball.transform.position.y;
            if (look_vector != Vector2.zero)
            {
                //  �����w�肵�ē�����
                ball_pos = transform.position + (new Vector3(look_vector.x, 0.0f, look_vector.y) * 2);
                ball_pos.y = ball_y;
                ball.Fire(ball_pos, new Vector3(look_vector.x, 0.0f, look_vector.y));
            }
            else
            {
                //  �����w�肵�Ă��Ȃ��Ƃ��A�����Ă�������ɓ�����
                ball_pos = transform.position + (new Vector3(transform.forward.x, 0.0f, transform.forward.z) * 2);
                ball_pos.y = ball_y;
                ball.Fire(ball_pos, transform.forward);
            }

            UnityChanController animator = GetComponent<UnityChanController>();
            if (animator != null)
            {
                //SE
                se_players.PlayThrow();
                animator.PlayThrowAnim();
            }

        }
    }

    public void Catch()
    {
        if (!hasController) { return; }

        Vector3 sphereCenter = transform.position;

        // OverlapSphere���g�p���Ĕ͈͓��̂��ׂẴR���C�_�[���擾
        Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, status.CatchRamge);

        // �擾�����R���C�_�[�����[�v���āA�Q�[���I�u�W�F�N�g���擾
        foreach (Collider hitCollider in hitColliders)
        {
            GameObject hitObject = hitCollider.gameObject;
            Ball ball = hitObject.GetComponent<Ball>();
            if ((ball != null) && (!ball.CatchFlg))
            {
                Vector3 ball_velocity = ball.GetComponent<Rigidbody>().velocity;
                ball_velocity = ball_velocity.normalized * -1;
                if(ball_velocity.magnitude > 0)
                {
                    Quaternion ratation = Quaternion.LookRotation(ball_velocity, Vector3.up); 
                    transform.rotation = ratation;
                }
                ball.Find(hand_position);
                //SE
                se_players.PlayCatch();
            }
        }
    }

    public void ApplyDamage(Vector3 normal_vector, float speed)
    {
        UnityChanController animator = gameObject.GetComponent<UnityChanController>();

        status.Life--;

        if (animator != null)
        {
            //  ������ꂽ����������
            Quaternion rotation = Quaternion.LookRotation(normal_vector, Vector3.up);
            transform.rotation = rotation;
            animator.OnDamaged();
        }

        if (status.Life <= 0)
        {
            //  �@���̋t�x�N�g����^���邱�ƂŔ���p�I�ɔ�΂�
            Death(normal_vector * -1, speed);
        }

    }

    private void Death(Vector3 direction, float speed)
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        NavMeshAgent nav = GetComponent<NavMeshAgent>();
        PlayerInput player_input = GetComponent<PlayerInput>();
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();

        Vector3 force_direction = direction.normalized;
        force_direction.y = 0.5f;

        transform.position = new Vector3(transform.position.x, 3, transform.position.z);

        //  �����蔻��̖�����
        if (collider != null)
        {
            collider.enabled = false;
        }

        //  NavMeshAgent�𖳌���
        if(nav != null)
        {
            nav.enabled = false;
        }

        //  ���͂𖳌���
        if(player_input != null)
        {
            //player_input.enabled = false;
        }

        //  ������΂�
        if(rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(force_direction * speed * 30);
        }

        count_death_time = death_time;
        hasController = false;
    }

    private void Respawn()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        NavMeshAgent nav = GetComponent<NavMeshAgent>();
        PlayerInput player_input = GetComponent<PlayerInput>();
        Rigidbody rb = GetComponent<Rigidbody>();

        transform.position = spawn_pos;

        //  �����蔻��̗L����
        if (collider != null)
        {
            collider.enabled = true;
        }

        //  NavMeshAgent��L����
        if (nav != null)
        {
            nav.enabled = true;
        }

        //  ���͂�L����
        if (player_input != null)
        {
            player_input.enabled = true;
        }

        //  ������΂�
        if (rb != null)
        {
            Destroy(rb);
        }

        status.Life = 2;
        hasController = true;
    }

    Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }
            Transform found = FindChildByName(child, name);
            if (found != null)
            {
                return found;
                
            }
        }
        return null;
    }

    private void OnGUI()
    {
        // ���x���̕��ƍ���
        float label_height = 60;
        float label_width = 150;
        float label_posY = 50;

        string GUI_text = "" + status.Life;

        // ��ʂ̒����Ƀ��x����z�u
        Rect label_rect = new Rect((Screen.width - label_width) / 2, label_posY, label_width, label_height);
        GUI.Label(label_rect, GUI_text );

    }
}