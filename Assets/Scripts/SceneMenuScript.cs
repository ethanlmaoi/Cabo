using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMenuScript : MonoBehaviour {
    public Canvas onlineMenu;
    public Canvas localMenu;

    public void openOnlineMenu()
    {
        onlineMenu.enabled = true;
    }

    public void openLocalMenu()
    {
        localMenu.enabled = true;
    }

    public void openTutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }
}
