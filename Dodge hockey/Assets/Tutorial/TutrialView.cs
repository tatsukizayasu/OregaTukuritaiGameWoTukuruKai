using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutrialView : MonoBehaviour
{
    public Camera mainCamera;
    //public Camera secondaryCamera;

    void Start()
    {
        // メインカメラのViewport Rectを設定 (左半分)
        mainCamera.rect = new Rect(0, 0, 0.7f, 1);

        // セカンダリカメラのViewport Rectを設定 (右半分)
        //secondaryCamera.rect = new Rect(0.5f, 0, 0.5f, 1);
    }
}
