using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {

    public Transform target;

    public float speed;
    public bool move;

	// Use this for initialization
	void Start () {
		speed = 5.5f;
        move = false;
    }
	
	// Update is called once per frame
	void Update () {
        OnMouseOver();
        if (move)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            move = true;
        }
    }
}
