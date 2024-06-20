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

    // 初期化
    void Start()
    {
        Init();

        // GUIStyleの初期化
        custom_style = new GUIStyle();
        custom_style.fontSize = 36; // フォントサイズを設定
        custom_style.fontStyle = FontStyle.Bold; // フォントスタイルを太字に設定
        custom_style.alignment = TextAnchor.MiddleCenter;   //  テキストを中央ぞろえに

        Color color = new Color(0xF9 / 255.0f, 1.0f, 0.0f, 1.0f);
        //ColorUtility.TryParseHtmlString("F9FF00", out color);
        custom_style.normal.textColor = color; // テキストの色を設定

        // 背景色を設定する場合（ボックススタイルなど）
        custom_style.normal.background = MakeTex(2, 2, new Color(0.25f, 0.25f, 0.25f, 0.5f)); // 半透明のグレー

    }

    private void Init()
    {
        score[0] = 0;
        score[1] = 0;
    }

    // 更新
    void Update()
    {        

    }

    private void OnGUI()
    {
        // ラベルの幅と高さ
        float label_height = 60;
        float label_width = 150;
        float label_posY = 30;

        string GUI_text = score[0] + " - " + score[1];

        // 画面の中央にラベルを配置
        Rect label_rect = new Rect((Screen.width - label_width) / 2, label_posY, label_width, label_height);
        GUI.Label(label_rect, GUI_text, custom_style);

    }

    // テクスチャを作成するヘルパー関数
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
 