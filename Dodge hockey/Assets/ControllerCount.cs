using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCount : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var controllerNames = Input.GetJoystickNames();

            print(controllerNames);
        }
    }
}
