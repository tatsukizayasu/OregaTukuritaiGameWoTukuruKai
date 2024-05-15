using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(prefab);
        Instantiate(prefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
