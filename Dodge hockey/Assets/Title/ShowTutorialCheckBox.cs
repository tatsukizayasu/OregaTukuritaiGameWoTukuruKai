using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;

public class ShowTutorialCheckBox : MonoBehaviour
{
    private string label_text = "Play The Tutorial"; // テキストの内容
    private bool is_checked = false; // チェックボックスの状態       
    private GUIStyle text_style; // GUIスタイル
    [SerializeField] private Texture2D check_false;
    [SerializeField] private Texture2D check_true;

    bool isChecked = false;
    private void Start()
    {
        is_checked = (PlayerPrefs.GetInt("isToPlayTutorial") == 1) ? true : false;

        // GUIスタイルを初期化
        text_style = new GUIStyle();
        text_style.normal.textColor = new Color(0.3f, 0.3f, 0.3f,1); // フォントカラーを変更
        text_style.alignment = TextAnchor.MiddleRight; // 右揃えにする
    }

    private void Update()
    {
        is_checked = (PlayerPrefs.GetInt("isToPlayTutorial") != 0) ? true : false;
    }

    void OnGUI()
    {
        // 基準解像度 1280x720に基づくスケール
        float scale_width = Screen.width / 1280;
        float scale_height = Screen.height / 720;

        // 画面の大きさに基づいてフォントサイズを変更
        int font_size = (int)(40f * (Mathf.Min(scale_width, scale_height)));
        text_style.fontSize = font_size;

        // チェックボックスとテキストのサイズ
        float checkbox_width = font_size * 1.2f * scale_width;
        float checkbox_height = font_size * 1.2f * scale_height;
        float label_width = (font_size / 2.2f * label_text.Length) * scale_width;
        float label_height = (font_size + 4) * scale_height;

        // マージンを画面サイズに合わせて調整
        float margin_x = 10f * scale_width;
        float margin_y = 10f * scale_height;

        // 画面右下の位置を計算
        float x = Screen.width - checkbox_width - label_width - margin_x;
        float y = Screen.height - checkbox_height - margin_y;

        // チェックボックスのRect
        Rect checkbox_rect = new Rect(x, y, checkbox_width, checkbox_height);

        // テキストのRect
        Rect label_rect = new Rect(x + checkbox_width, y, label_width, label_height);

        // チェックボックスとテキストを描画
        if(is_checked)
        {
            GUI.DrawTexture(checkbox_rect, check_true); 
        }
        else
        {
            GUI.DrawTexture(checkbox_rect, check_false);
        }
        GUI.Label(label_rect, label_text, text_style);
    }

}
