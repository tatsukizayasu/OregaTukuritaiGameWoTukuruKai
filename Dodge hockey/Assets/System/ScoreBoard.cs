using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ScoreBoard : MonoBehaviour
{
    private int[] score = { 0, 0 };

    public GUIStyle custom_style;

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
}
 