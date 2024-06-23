using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StartButton : MonoBehaviour
{
    [SerializeField] private InputAction PadInput;

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Onstart(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("SampleScene");
    }
}
