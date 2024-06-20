using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GameManager))]
public class PlayerManager : MonoBehaviour
{
    public GameObject[] players;
    private GameManager game_manager;

    void Start()
    {
        game_manager = GetComponent<GameManager>();

        players = game_manager.Players;        

        // �Q�[���p�b�h���ڑ����ꂽ�Ƃ��ɌĂяo�����C�x���g��o�^
        InputSystem.onDeviceChange += OnDeviceChange;

        // ���łɐڑ�����Ă���f�o�C�X���`�F�b�N
        AssignDevices();
    }

    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        print("on device change");
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
        {
            AssignDevices();
        }
    }

    void AssignDevices()
    {
        var gamepads = Gamepad.all;
        //  print($"Gamepads 0 #{gamepads[0]}");
        //  print($"Gamepads 1 #{gamepads[1]}");
        if (gamepads.Count >= 1)
        {
            PlayerInput player1_input = players[0].GetComponent<PlayerInput>();
            player1_input.SwitchCurrentControlScheme("Gamepad", gamepads[0]);
        }

        if (gamepads.Count >= 2)
        {
            PlayerInput player2_input = players[1].GetComponent<PlayerInput>();
            player2_input.SwitchCurrentControlScheme("Gamepad", gamepads[1]);
        }
    }
}
