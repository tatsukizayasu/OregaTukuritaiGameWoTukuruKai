using UnityEngine;
using UnityEngine.InputSystem;

public class ReceiveNotificationExample : MonoBehaviour
{
    // �v���C���[�������Ɏ󂯎��ʒm
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        print($"�v���C���[#{playerInput.user.index}�������I");
    }

    // �v���C���[�ގ����Ɏ󂯎��ʒm
    public void OnPlayerLeft(PlayerInput playerInput)
    {
        print($"�v���C���[#{playerInput.user.index}���ގ��I");
    }
}