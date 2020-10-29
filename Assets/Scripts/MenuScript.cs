using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    private GameObject playButton, settingsButton, htpButton, exitButton, btmButton;
    private SolarSystem solarSystem;
    private CameraScript mainCamera;
    enum UI_Phase
    {
        PlanetInfo,
        SolarSystem,
        MainMenu
    }
    private UI_Phase currentPhase;
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
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();

        currentPhase = UI_Phase.MainMenu;
    }
    void PlayTask()
    {
        playButton.SetActive(false);
        settingsButton.SetActive(false);
        htpButton.SetActive(false);
        exitButton.SetActive(false);
        btmButton.SetActive(true);

        solarSystem.EnableSolarSystemPhase(true);
        mainCamera.GoFree();
        currentPhase = UI_Phase.SolarSystem;
    }
    void BackTask()
    {
        switch (currentPhase)
        {
            case UI_Phase.SolarSystem:
                playButton.SetActive(true);
                settingsButton.SetActive(true);
                htpButton.SetActive(true);
                exitButton.SetActive(true);
                btmButton.SetActive(false);
                solarSystem.EnableSolarSystemPhase(false);
                currentPhase = UI_Phase.MainMenu;
                break;
            case UI_Phase.PlanetInfo:
                mainCamera.GoFree();
                currentPhase = UI_Phase.SolarSystem;
                break;
        }

    }

    void _HandlePlanetClick()
    {
        if ((Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Began) && mainCamera.ReadyForLock())
        {
            Ray raycast = mainCamera.camera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit, Mathf.Infinity))
            {
                Debug.DrawRay(transform.position, raycastHit.point, Color.red, 5f);
                if (raycastHit.transform.gameObject.tag == "Planet")
                {
                    mainCamera.FocusOn(raycastHit.transform.gameObject);
                    currentPhase = UI_Phase.PlanetInfo;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        _HandlePlanetClick();
    }
}
