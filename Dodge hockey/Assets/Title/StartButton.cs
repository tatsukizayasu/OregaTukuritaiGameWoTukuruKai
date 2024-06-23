using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StartButton : MonoBehaviour
{
    [SerializeField] private InputAction PadInput;
    private float startTime;  // 開始時間を記録する変数

    private float invalidTime = 0.5f;
    // 有効化
    private void OnEnable()
    {
        // Actionのコールバックを登録
        PadInput.performed += Onstart;

        // InputActionを有効化
        // これをしないと入力を受け取れないことに注意
        PadInput?.Enable();
    }

    // 無効化
    private void OnDisable()
    {
        // Actionのコールバックを解除
        PadInput.performed -= Onstart;

        // 自身が無効化されるタイミングなどで
        // Actionを無効化する必要がある
        PadInput?.Disable();
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
            SceneManager.LoadScene("SampleScene");
        }
    }
}
