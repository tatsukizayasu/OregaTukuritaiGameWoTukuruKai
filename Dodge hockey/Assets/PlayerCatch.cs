using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatch : MonoBehaviour
{
    const int COOL_TIME = 60;
    private int ct; 

    [SerializeField]
    private string Fire1;

    // Start is called before the first frame update
    void Start()
    {
        ct = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ct -= 1;
        if (Input.GetButtonDown(Fire1) && (ct < 0))
        {
            print(ct);
            ct = COOL_TIME;
        }
    }
}
