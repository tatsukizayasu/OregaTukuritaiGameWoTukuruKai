using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class ScoreBoard : MonoBehaviour
{
    private int[] score = { 0, 0 };

    public GUIStyle custom_style;

    private GameObject[] players;

    public float box_padding = 20;
    public float box_height = 60;
    public float custom_box_padding = 20;
    public int hp = 3;  // �\������HP�̐�

    private Texture2D boxTexture;  // �e�N�X�`���������o�[�ϐ��ɒǉ�

    private void Awake()
    {

    }

    // ������
    void Start()
    {
        Init();

        // GUIStyle�̏�����
        custom_style = new GUIStyle();
        custom_style.fontSize = 36; // �t�H���g�T�C�Y��ݒ�
        custom_style.fontStyle = FontStyle.Bold; // �t�H���g�X�^�C���𑾎��ɐݒ�
        custom_style.alignment = TextAnchor.MiddleCenter;   //  �e�L�X�g�𒆉����낦��

        Color color = new Color(0xF9 / 255.0f, 1.0f, 0.0f, 1.0f);
        //ColorUtility.TryParseHtmlString("F9FF00", out color);
        custom_style.normal.textColor = color; // �e�L�X�g�̐F��ݒ�

        // �w�i�F��ݒ肷��ꍇ�i�{�b�N�X�X�^�C���Ȃǁj
        custom_style.normal.background = MakeTex(2, 2, new Color(0.25f, 0.25f, 0.25f, 0.5f)); // �������̃O���[

        players = GetComponent<GameManager>().Players;
        // �{�b�N�X�̔w�i�e�N�X�`�����쐬
        boxTexture = MakeTex(1, 1, Color.green);  // �ԐF�̃e�N�X�`�����쐬
    }

    private void Init()
    {
        score[0] = 0;
        score[1] = 0;
    }

    // �X�V
    void Update()
    {        

    }

    private void OnGUI()
    {
        // ���x���̕��ƍ���
        float label_height = 60;
        float label_width = 150;
        float label_posY = 30;

        string GUI_text = score[0] + " - " + score[1];

        // ��ʂ̒����Ƀ��x����z�u
        Rect label_rect = new Rect((Screen.width - label_width) / 2, label_posY, label_width, label_height);
        GUI.Label(label_rect, GUI_text, custom_style);



        // HP�̃{�b�N�X��\��
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.normal.background = boxTexture;

        float box_width = 60;
        float total_box_width = (box_width + box_padding) * hp;

        // �����̃v���C���[��HP�{�b�N�X��\��
        for (int i = 0; i < players[0].GetComponent<PlayerStatus>().Life; i++)
        {
            float posX = (Screen.width - label_width) / 2 - custom_box_padding - total_box_width + i * (box_width + box_padding);
            Rect boxRect = new Rect(posX, label_posY, box_width, box_height);
            GUI.Box(boxRect, GUIContent.none, boxStyle);
        }

        // �E���̃v���C���[��HP�{�b�N�X��\��
        for (int i = 0; i < players[1].GetComponent<PlayerStatus>().Life; i++)
        {
            float posX = (Screen.width + label_width) / 2 + custom_box_padding + i * (box_width + box_padding);
            Rect boxRect = new Rect(posX, label_posY, box_width, box_height);
            GUI.Box(boxRect, GUIContent.none, boxStyle);
        }

    }

    // �e�N�X�`�����쐬����w���p�[�֐�
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    public void AddScore(int player_num)
    {
        if ((0 <= player_num) && (player_num <= 1))
        {
            score[player_num]++;
        }
    }

    public int GetScore(int player_num)
    {
        if ((0 <= player_num) && (player_num <= 1))
        {
            return score[player_num];
        }
        else
        {
            return 0;
        }
    }
}
 