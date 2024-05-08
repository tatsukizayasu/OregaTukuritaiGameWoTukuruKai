using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeTest : MonoBehaviour
{
    [SerializeField]
    private string objectName = "name";

    // Start is called before the first frame update
    void Start()
    {
        print("start : " + objectName);
    }

    private void Awake()
    {
        print("awake : " + objectName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
