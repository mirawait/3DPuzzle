using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreYouSureScript : MonoBehaviour
{
    private MenuScript menuScript;
    void Start()
    {
        menuScript = GameObject.Find("Main Menu").GetComponent<MenuScript>();
    }
    public void YesButtonTask()
    {
        menuScript.GoToMainMenu();
    }

    public void NoButtonTask()
    {
        Destroy(transform.gameObject);
    }
}
