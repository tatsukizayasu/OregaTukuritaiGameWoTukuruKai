using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ScoreBoard : MonoBehaviour
{
    [SerializeField] GameObject score_board_UI;
    private GameObject score_object = null; // Textオブジェクト
    private TextMeshProUGUI test;

    private int score_player1;
    private int score_player2;

    private void Awake()
    {
        score_object = Instantiate(score_board_UI).GetComponent<GameObject>();
        test = Instantiate(score_board_UI).GetComponent<TextMeshProUGUI>();
    }

    // 初期化
    void Start()
    {
        Init();
    }

    private void Init()
    {
        score_player1 = 0;
        score_player2 = 0;
    }

    // 更新
    void Update()
    {        
        // オブジェクトからTextコンポーネントを取得
        TextMeshProUGUI score_text = score_object.GetComponent<TextMeshProUGUI>();
        // テキストの表示を入れ替える
        test.text = score_player1 + "-" + score_player2;
    }
}
 