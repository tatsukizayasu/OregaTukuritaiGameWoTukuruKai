using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalEffectPlay : MonoBehaviour
{
    [SerializeField] private GameObject effect;
    
    GameObject Effect;
    // Start is called before the first frame update

    void Start()
    {
        Effect = Instantiate(effect);
        gameObject.transform.parent = Effect.transform;

    }
    void OnDestroy()
    {
        Destroy(Effect);
    }

}
