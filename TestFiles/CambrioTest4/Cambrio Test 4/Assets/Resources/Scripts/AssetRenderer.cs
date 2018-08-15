using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetRenderer : MonoBehaviour {

    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/ace_of_diamonds");
        gameObject.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/ace_of_diamonds");
    }

    void Update()
    {
        
    }
}
