using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour {

    public Canvas menu;
    
	public void closeMenu()
    {
        menu.enabled = false;
    }
}
