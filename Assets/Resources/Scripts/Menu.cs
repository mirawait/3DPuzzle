using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Menu : MonoBehaviour
{

	public bool isOpened = false;

	void Start()
	{

	}


	void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			ShowHideMenu();
		}
	}

	public void GoToMain()
	{
		SceneManager.LoadScene("Menu");
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void ShowHideMenu()
	{
		isOpened = !isOpened;
		GetComponent<Canvas>().enabled = isOpened;
	}

	
}