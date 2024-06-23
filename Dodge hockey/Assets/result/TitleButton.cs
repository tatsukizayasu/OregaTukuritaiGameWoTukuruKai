using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleButton : MonoBehaviour
{
    [SerializeField] private InputAction PadInput;

    private float startTime;  // 開始時間を記録する変数
    private float invalidTime = 0.5f;

    private SE_Player se_players;

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
    private void Awake()
    {

        se_players = GetComponent<SE_Player>();
    }
    // Start is called before the first frame update
    void Start()
    {

        startTime = Time.time;  // ゲーム開始時刻を記録
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
