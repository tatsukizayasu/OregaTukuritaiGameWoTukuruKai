using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private InputAction StartButton;
    private void OnEnable()
    {
        // Actionのコールバックを登録
        StartButton.performed += OnStart;

        // InputActionを有効化
        StartButton?.Enable();
    }

    private void OnDisable()
    {
        // Actionのコールバックを解除
        StartButton.performed -= OnStart;

        // Actionを無効化
        StartButton?.Disable();
    }

    

    private void OnStart(InputAction.CallbackContext context)
    {
        PlayerPrefs.SetInt("isToPlayTutorial", 0);
        SceneManager.LoadScene("SampleScene");
    }
}
