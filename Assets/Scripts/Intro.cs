using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour {

    [SerializeField]
    GameObject intro_scene_anim;
    [SerializeField]
    GameObject cambrio_title_anim;
    [SerializeField]
    GameObject play_button_anim;

    public Canvas sceneMenu;

    void Awake()
    {
        sceneMenu.enabled = false;
    }

    // Use this for initialization
    void Start()
    {
        GameObject.Instantiate(intro_scene_anim);
        StartCoroutine(showTitle());
        StartCoroutine(showPlayButton());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*Touch touch = Input.touches[0];
        if (touch.phase == TouchPhase.Began) {
            Ray ray = Camera.main.ScreenPointToRay(touch.position);*/
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.tag == "PlayButton")
            {
                sceneMenu.enabled = true;
            }
        }
    }

    IEnumerator showTitle()
    {
        yield return new WaitForSeconds(4);
        GameObject.Instantiate(cambrio_title_anim);
    }

    IEnumerator showPlayButton()
    {
        yield return new WaitForSeconds(4);
        GameObject.Instantiate(play_button_anim);
    }
}
