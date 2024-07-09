using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField] private InputAction PadInput;
    [SerializeField] private InputAction toggleIsToPlayTutorial;
    private float startTime;  // 開始時間を記録する変数

    private float invalidTime = 0.5f;
    // 有効化
    private void OnEnable()
    {
        //  初回起動チェック(初回起動時、isToPlayTutorial に 1を入れる)
        PlayerPrefs.SetInt("isToPlayTutorial", PlayerPrefs.GetInt("isToPlayTutorial", 1));

        // Actionのコールバックを登録
        PadInput.performed += Onstart;
        toggleIsToPlayTutorial.performed += OnToggleTutorial;

        // InputActionを有効化
        // これをしないと入力を受け取れないことに注意
        PadInput?.Enable();
        toggleIsToPlayTutorial?.Enable();
    }

    // 無効化
    private void OnDisable()
    {
        // Actionのコールバックを解除
        PadInput.performed -= Onstart;
        toggleIsToPlayTutorial.performed -= OnToggleTutorial;

        // 自身が無効化されるタイミングなどで
        // Actionを無効化する必要がある
        PadInput?.Disable();
        toggleIsToPlayTutorial?.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;  // ゲーム開始時刻を記録
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
            if(PlayerPrefs.GetInt("isToPlayTutorial") == 0)
            {
                SceneManager.LoadScene("SampleScene");
            }
            else
            {
                PlayerPrefs.SetInt("isToPlayTutorial", 0);
                SceneManager.LoadScene("Tutorial");
            }
            
        }
    }
    private void OnToggleTutorial(InputAction.CallbackContext context)
    {
        if(PlayerPrefs.GetInt("isToPlayTutorial") != 0)
        {
            PlayerPrefs.SetInt("isToPlayTutorial", 0);
        }
        else
        {
            PlayerPrefs.SetInt("isToPlayTutorial", 1);
        }
    }
}
