using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ScoreBoard : MonoBehaviour
{
    [SerializeField] GameObject score_board_UI;
    private GameObject score_object = null; // Text�I�u�W�F�N�g
    private TextMeshProUGUI test;

    private int score_player1;
    private int score_player2;

    private void Awake()
    {
        score_object = Instantiate(score_board_UI).GetComponent<GameObject>();
        test = Instantiate(score_board_UI).GetComponent<TextMeshProUGUI>();
    }

    // ������
    void Start()
    {
        Init();
    }

    private void Init()
    {
        score_player1 = 0;
        score_player2 = 0;
    }

    // �X�V
    void Update()
    {        
        // �I�u�W�F�N�g����Text�R���|�[�l���g���擾
        TextMeshProUGUI score_text = score_object.GetComponent<TextMeshProUGUI>();
        // �e�L�X�g�̕\�������ւ���
        test.text = score_player1 + "-" + score_player2;
    }
}
 