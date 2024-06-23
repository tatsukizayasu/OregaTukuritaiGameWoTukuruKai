using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleButton : MonoBehaviour
{
    [SerializeField] private InputAction PadInput;

    private float startTime;  // �J�n���Ԃ��L�^����ϐ�
    private float invalidTime = 0.5f;

    private SE_Player se_players;

    // �L����
    private void OnEnable()
    {
        // Action�̃R�[���o�b�N��o�^
        PadInput.performed += Onstart;

        // InputAction��L����
        // ��������Ȃ��Ɠ��͂��󂯎��Ȃ����Ƃɒ���
        PadInput?.Enable();
    }

    // ������
    private void OnDisable()
    {
        // Action�̃R�[���o�b�N������
        PadInput.performed -= Onstart;

        // ���g�������������^�C�~���O�Ȃǂ�
        // Action�𖳌�������K�v������
        PadInput?.Disable();
    }
    private void Awake()
    {

        se_players = GetComponent<SE_Player>();
    }
    // Start is called before the first frame update
    void Start()
    {

        startTime = Time.time;  // �Q�[���J�n�������L�^
        se_players.PlayWin();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Onstart(InputAction.CallbackContext context)
    {
        float currentTime = Time.time;
        float elapsedTime = currentTime - startTime;

        if (elapsedTime >= invalidTime)
        {
            SceneManager.LoadScene("Title");
        }
    }
}
