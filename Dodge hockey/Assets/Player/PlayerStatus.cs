using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private float catchRange;
    public float CatchRamge { set { catchRange = value; } get { return catchRange; } }

    private void Awake()
    {
        catchRange = 5;
    }

    // Start is called before the first frame update
    void Start()
    {
        float a = CatchRamge;   
    }

    // Update is cal
    // led once per frame
    void Update()
    {

    }
    
}