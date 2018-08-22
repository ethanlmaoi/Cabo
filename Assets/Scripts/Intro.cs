using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour {

    [SerializeField] GameObject intro_scene;
    [SerializeField] GameObject cambrio_title;
    [SerializeField] GameObject play_button;


    // Use this for initialization
    void Start()
    {
        GameObject.Instantiate(intro_scene);
        StartCoroutine(showTitle());
        StartCoroutine(showPlayButton());
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    IEnumerator showTitle()
    {
        yield return new WaitForSeconds(4);
        GameObject.Instantiate(cambrio_title);
    }

    IEnumerator showPlayButton()
    {
        yield return new WaitForSeconds(4);
        GameObject.Instantiate(play_button);
    }

    private void OnMouseDown()
    {
        SceneManager.LoadScene(1);
    }
}
