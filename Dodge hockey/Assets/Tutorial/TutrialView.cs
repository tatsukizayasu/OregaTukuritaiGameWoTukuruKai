using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutrialView : MonoBehaviour
{
    public Camera main_camera;
    public Texture2D background;

    public float x, y, width, height;

    //  背景描画用オブジェクト
    private Camera background_camera;
    private GameObject background_quad;

    void Start()
    {
        // 背景用カメラを作成
        GameObject background_camera_obj = new GameObject("BackgroundCamera");
        background_camera = background_camera_obj.AddComponent<Camera>();
        background_camera.depth = -1; // メインカメラよりも前に描画されるようにする
        //background_camera.clearFlags = CameraClearFlags.Depth; // 深度のみにクリア
        //background_camera.cullingMask = LayerMask.GetMask("Background"); // 背景レイヤーのみ描画
        background_camera.transform.position = new Vector3(100, 100, 100);

        // 背景用のQuadを作成
        background_quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        background_quad.transform.parent = background_camera.transform;
        background_quad.transform.localPosition = new Vector3(0, 0, 7.8f); // カメラの前に配置
        background_quad.transform.localScale = new Vector3(16, 9, 1); // 画面のアスペクト比に合わせる

        //  ライティングの影響から角度を変更する
        background_camera.transform.rotation = Quaternion.Euler(90, 0, 0);

        // Quadに背景テクスチャを設定
        Renderer quad_renderer = background_quad.GetComponent<Renderer>();
        quad_renderer.material.mainTexture = background;

        //  メインカメラのViewPort Rectを設定
        main_camera.rect = new Rect(x, y, width, height);
        main_camera.clearFlags = CameraClearFlags.Depth;    //  深度のみにクリア(余計な背景が描画されない)
        main_camera.depth = 0;
    }
    
    private void Update()
    {

        main_camera.rect = new Rect(x, y, width, height);
    }

    void OnGUI()
    {
        if (background != null)
        {
            // 背景テクスチャを全画面に表示
           // GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.StretchToFill);
        }
        else
        {
            Debug.LogError("Background texture not assigned!");
        }
    }
}
