using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    
    private GameObject titleText, playButton, settingsButton, htpButton, exitButton, btmButton, solveButton,
                       settingsMenu;
    private SolarSystem solarSystem;
    private CameraScript mainCamera;
    private LoadGameScene loadGameScene;
    private GameObject[] planetInfoPanels;
    private GameObject pauseMenuBG, pauseMenuCanvas, pauseSettingsButton, pauseMainMenuButton;
    private GameObject areYouSurePanel;
    private DificultySwitcherScript dificultySwitcherScript;
    private SaveManager saveManager;
    //private GameObject[] difButtons;
    ConfirmSettingsScript settingsSwitcher;
    TutorialScript tutorial;
    private GameObject planetNameInGame;
    private GameObject planetIsDoneText;
    uint lastActiveInfoPanel;
    uint planetClickSubscription;
    private static bool isOnPause = false;
    enum UI_Phase
    {
        PlanetInfo,
        SolarSystem,
        MainMenu,
        Settings,
        Puzzle,
        Pause,
        PauseSettings
    }
    private UI_Phase currentPhase;
    // Start is called before the first frame update
    void Start()
    {
        tutorial = GameObject.Find("Tutorial").GetComponent<TutorialScript>();
        loadGameScene = GameObject.FindGameObjectWithTag("LoadSceneTag").GetComponent<LoadGameScene>();
        titleText = GameObject.Find("3dPuzzleText");
        playButton = GameObject.Find("PlayButton");
        settingsButton = GameObject.Find("SettingsButton");
        settingsMenu = GameObject.Find("SettingsMenu");
        htpButton = GameObject.Find("HowToPlayButton");
        exitButton = GameObject.Find("ExitButton");
        btmButton = GameObject.Find("BackToMenuButton");
        solveButton = GameObject.Find("SolveButton");
        planetNameInGame = GameObject.Find("PlanetNameInGame");
        planetIsDoneText = GameObject.Find("PlanetIsDoneText");
        pauseMenuBG = GameObject.Find("PauseMenuBG");
        pauseMenuCanvas = GameObject.Find("PauseMenuCanvas");
        pauseSettingsButton = GameObject.Find("PauseSettingsButton");
        pauseMainMenuButton = GameObject.Find("PauseMainMenuButton");
        dificultySwitcherScript = GameObject.Find("DificultySwitch").GetComponent<DificultySwitcherScript>();
        saveManager = GameObject.FindGameObjectWithTag("LoadSceneTag").GetComponent<SaveManager>();
        //difButtons[0] = GameObject.Find("NextDifButton");
        //difButtons[1] = GameObject.Find("PrevDifButton");
        settingsSwitcher = settingsMenu.GetComponent<ConfirmSettingsScript>();
        playButton.GetComponent<Button>().onClick.AddListener(PlayTask);
        settingsButton.GetComponent<Button>().onClick.AddListener(SettingsTask);
        btmButton.GetComponent<Button>().onClick.AddListener(BackTask);
        exitButton.GetComponent<Button>().onClick.AddListener(ExitTask);
        htpButton.GetComponent<Button>().onClick.AddListener(HowToPlayTask);
        solveButton.GetComponent<Button>().onClick.AddListener(SolveTask);
        pauseSettingsButton.GetComponent<Button>().onClick.AddListener(SettingFromPauseTask);
        pauseMainMenuButton.GetComponent<Button>().onClick.AddListener(MainMenuButtonTask);

        btmButton.SetActive(false);
        solveButton.SetActive(false);
        settingsMenu.SetActive(false);
        planetNameInGame.SetActive(false);
        pauseMenuBG.SetActive(false);
        pauseMenuCanvas.SetActive(false);
        planetIsDoneText.SetActive(false);
        solarSystem = GameObject.FindGameObjectWithTag("Sun").GetComponent<SolarSystem>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();

        planetInfoPanels = GameObject.FindGameObjectsWithTag("InfoPanel");
        foreach (GameObject infoPanel in planetInfoPanels)
        {
            infoPanel.SetActive(false);
        }

        currentPhase = UI_Phase.MainMenu;
    }
    void SolveTask()
    {
        currentPhase = UI_Phase.Puzzle;
        mainCamera.EnablePuzzleLock(true);
        planetNameInGame.SetActive(true);
        planetNameInGame.GetComponent<TextMeshProUGUI>().text = IndexToName(loadGameScene.planetType);
        foreach (GameObject infoPanel in planetInfoPanels)
        {
            infoPanel.SetActive(false);
        }
        solveButton.SetActive(false);
        planetIsDoneText.SetActive(false);
        loadGameScene.LoadScene();
        loadGameScene.stopWaitingForPlanetClick();
        
    }

    string IndexToName(uint index)
    {
        switch (index)
        {
            case 0:
                return "СОЛНЦЕ";
            case 1:
                return "МЕРКУРИЙ";
            case 2:
                return "ВЕНЕРА";
            case 3:
                return "ЗЕМЛЯ";
            case 4:
                return "МАРС";
            case 5:
                return "ДИЧЬ";
        }
        return "ПЛАНЕТА";
    }
    void PlayTask()
    {
        if (saveManager.IsTutorialDone() == false)
        {
            tutorial.EnableTutorial();
        }
        titleText.SetActive(false);
        playButton.SetActive(false);
        settingsButton.SetActive(false);
        htpButton.SetActive(false);
        exitButton.SetActive(false);
        btmButton.SetActive(true);
        loadGameScene.startWaitingForPlanetClick();
        solarSystem.EnableSolarSystemPhase(true);
        _CheckoutToSolarSystemPhase();
    }
    void _CheckoutToSolarSystemPhase()
    {
        mainCamera.GoFree();
        planetClickSubscription = GesturesController.subscribeToPlanetClick(
            (GameObject target) => 
            {
                if (currentPhase == UI_Phase.SolarSystem && target.tag == "Planet")
                {
                    lastActiveInfoPanel = target.GetComponent<PlanetScript>().GetIndex();
                    StartCoroutine(_WaitForCameraLock(lastActiveInfoPanel));
                    loadGameScene.stopWaitingForPlanetClick();
                }
            });
        currentPhase = UI_Phase.SolarSystem;
    }
    void BackTask()
    {
        Debug.Log("Back clicked");
        if (tutorial.IsTutorialEnabled())
        {
            currentPhase = UI_Phase.SolarSystem;
            foreach (GameObject infoPanel in planetInfoPanels)
            {
                infoPanel.SetActive(false);
            }
            solveButton.SetActive(false);
        }
        switch (currentPhase)
        {
            case UI_Phase.SolarSystem:
                StartCoroutine(_WaitForCameraMenuPhase());
                btmButton.SetActive(false);
                solarSystem.EnableSolarSystemPhase(false);
                mainCamera.GoMenu();
                tutorial.DisableTutorial();
                loadGameScene.stopWaitingForPlanetClick();
                break;
            case UI_Phase.PlanetInfo:
                mainCamera.GoFree();
                loadGameScene.startWaitingForPlanetClick();
                currentPhase = UI_Phase.SolarSystem;
                foreach (GameObject infoPanel in planetInfoPanels)
                {
                    infoPanel.SetActive(false);
                }
                solveButton.SetActive(false);
                planetIsDoneText.SetActive(false);
                break;
            case UI_Phase.Puzzle:
                //settingsMenu.SetActive(true);
                pauseMenuCanvas.SetActive(true);
                dificultySwitcherScript.nextButton.GetComponent<Button>().interactable = false;
                dificultySwitcherScript.prevButton.GetComponent<Button>().interactable = false;
                currentPhase = UI_Phase.Pause;
                Time.timeScale = 0;
                isOnPause = true;
                pauseMenuBG.SetActive(true);
                break;
            case UI_Phase.Settings:
                StartCoroutine(_WaitForCameraMenuPhase());
                settingsSwitcher.ResetSwitchers();
                btmButton.SetActive(false);
                settingsMenu.SetActive(false);
                mainCamera.GoMenu();
                break;
            case UI_Phase.Pause:
                Time.timeScale = 1;
                //mainCamera.EnablePuzzleLock(false);
                currentPhase = UI_Phase.Puzzle;
                //StartCoroutine(_WaitForCameraLock(lastActiveInfoPanel));
                //solveButton.SetActive(true);
                dificultySwitcherScript.nextButton.GetComponent<Button>().interactable = true;
                dificultySwitcherScript.prevButton.GetComponent<Button>().interactable = true;
                pauseMenuBG.SetActive(false);
                //settingsMenu.SetActive(false);
                pauseMenuCanvas.SetActive(false);
                isOnPause = false;


                break;
            case UI_Phase.PauseSettings:
                currentPhase = UI_Phase.Pause;
                settingsMenu.SetActive(false);
                pauseMenuCanvas.SetActive(true);
                break;
                
        }
    }
    void SettingsTask()
    {
        mainCamera.GoSettings();
        titleText.SetActive(false);
        playButton.SetActive(false);
        settingsButton.SetActive(false);
        htpButton.SetActive(false);
        exitButton.SetActive(false);
        btmButton.SetActive(true);
        StartCoroutine(_WaitForCameraSettingsPhase());
    }
    void HowToPlayTask()
    {
        tutorial.EnableTutorial();
        PlayTask();
    }
    void ExitTask()
    {
        Application.Quit(0);
    }

    void SettingFromPauseTask()
    {
        settingsMenu.SetActive(true);
        pauseMenuCanvas.SetActive(false);
        currentPhase = UI_Phase.PauseSettings;
    }

    void MainMenuButtonTask()
    {
       areYouSurePanel = Instantiate(Resources.Load("Prefabs/AreYouSurePanel") as GameObject, pauseMenuCanvas.transform);
    }

    public static bool IsOnPause()
    {
        return isOnPause;
    }

    public void GoToMainMenu()
    {
        Destroy(GameObject.Find("AreYouSurePanel(Clone)"));
        planetNameInGame.SetActive(false);
        Time.timeScale = 1;
        isOnPause = false;
        mainCamera.EnablePuzzleLock(false);
        currentPhase = UI_Phase.PlanetInfo;
        StartCoroutine(_WaitForCameraLock(lastActiveInfoPanel));
        solveButton.SetActive(true);
        dificultySwitcherScript.nextButton.GetComponent<Button>().interactable = true;
        dificultySwitcherScript.prevButton.GetComponent<Button>().interactable = true;
        pauseMenuBG.SetActive(false);
        settingsMenu.SetActive(false);
        pauseMenuCanvas.SetActive(false);
        loadGameScene.UnloadScene();
        BackTask();
        BackTask();
    }
    IEnumerator _WaitForCameraMenuPhase()
    {
        while (!mainCamera.isCurrentPhase(CameraScript.Phase.Menu))// IsCurrentPhaseMenu())
        {
            yield return new WaitForSeconds(0.01f);
        }
        titleText.SetActive(true);
        playButton.SetActive(true);
        settingsButton.SetActive(true);
        htpButton.SetActive(true);
        exitButton.SetActive(true);
        currentPhase = UI_Phase.MainMenu;
        yield break;
    }
    IEnumerator _WaitForCameraSettingsPhase()
    {
        while (!mainCamera.isCurrentPhase(CameraScript.Phase.Settings))// IsCurrentPhaseMenu())
        {
            yield return new WaitForSeconds(0.01f);
        }
        currentPhase = UI_Phase.Settings;
        settingsMenu.SetActive(true);
        yield break;
    }
    IEnumerator _WaitForCameraLock(uint focusedPlanetInex)
    {
        while(!mainCamera.isCurrentPhase(CameraScript.Phase.PlanetLock))// IsCurrentPhasePlanetLock())
        {
            yield return new WaitForSeconds(0.01f);
        }
        foreach (GameObject infoPanel in planetInfoPanels)
        {
            if (infoPanel.transform.GetComponent<PlanetInfoScript>().GetIndex() == focusedPlanetInex)
            {
                infoPanel.SetActive(true);
            }
            else
            {
                infoPanel.SetActive(false);
            }
        }
        currentPhase = UI_Phase.PlanetInfo;
        if (saveManager.IsPlanetDone((int)focusedPlanetInex, (int)DificultySwitcherScript.GetChosenDifficulty()))
        {
            planetIsDoneText.SetActive(true);
        }
        btmButton.SetActive(true);
        solveButton.SetActive(true);
        yield break;
    }
   
   

    // Update is called once per frame
    void Update()
    {
    }
}
