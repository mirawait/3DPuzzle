using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    GameObject playButton, settingsButton, htpButton, exitButton, btmButton;
    SolarSystem solarSystem;
    // Start is called before the first frame update
    void Start()
    {
        playButton = GameObject.Find("PlayButton");
        settingsButton = GameObject.Find("SettingsButton");
        htpButton = GameObject.Find("HowToPlayButton");
        exitButton = GameObject.Find("ExitButton");
        btmButton = GameObject.Find("BackToMenuButton");

        playButton.GetComponent<Button>().onClick.AddListener(PlayTask);
        btmButton.GetComponent<Button>().onClick.AddListener(BackTask);

        btmButton.SetActive(false);

        solarSystem = GameObject.FindGameObjectWithTag("Sun").GetComponent<SolarSystem>();
    }
    void PlayTask()
    {
        playButton.SetActive(false);
        settingsButton.SetActive(false);
        htpButton.SetActive(false);
        exitButton.SetActive(false);
        btmButton.SetActive(true);
        solarSystem.EnableSolarSystemPhase(true);
    }
    void BackTask()
    {
        playButton.SetActive(true);
        settingsButton.SetActive(true);
        htpButton.SetActive(true);
        exitButton.SetActive(true);
        btmButton.SetActive(false);
        solarSystem.EnableSolarSystemPhase(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
