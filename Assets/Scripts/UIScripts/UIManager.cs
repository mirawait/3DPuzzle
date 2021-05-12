using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    //----------------------UI REGION START---------------------
    public VisualElement mainMenuScreen;
    public VisualElement settingScreen;
    public VisualElement pauseScreen;
    public VisualElement areYouSureScreen;
    public VisualElement backButtonScreen;
    //----------------------UI REGION END---------------------



    //----------------------GAME REGION START---------------------
    private TutorialScript tutorial;
    private SolarSystem solarSystem;
    private CameraScript mainCamera;
    private LoadGameScene loadGameScene;
    uint lastActiveInfoPanel;
    uint planetClickSubscription;
    private static bool isOnPause = false;

    private bool isSound = true;
    private Difficulty difficulty = Difficulty.Medium;
    enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
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
    private UI_Phase currentPhase = UI_Phase.SolarSystem;

    //----------------------GAME REGION END---------------------

    private void Start()
    {
        //------------------UI REGION START--------------------------------------
        mainMenuScreen = GameObject.Find("MainMenuUI").GetComponent<UIDocument>().rootVisualElement;
        settingScreen = GameObject.Find("SettingsMenuUI").GetComponent<UIDocument>().rootVisualElement;
        pauseScreen = GameObject.Find("PauseMenuUI").GetComponent<UIDocument>().rootVisualElement;
        areYouSureScreen = GameObject.Find("AreYouSureMenuUI").GetComponent<UIDocument>().rootVisualElement;
        backButtonScreen = GameObject.Find("BackUI").GetComponent<UIDocument>().rootVisualElement;

        settingScreen.style.display = DisplayStyle.None;
        pauseScreen.style.display = DisplayStyle.None;
        areYouSureScreen.style.display = DisplayStyle.None;
        backButtonScreen.style.display = DisplayStyle.None;

        mainMenuScreen?.Q("settings-button")?.RegisterCallback<ClickEvent>(ev => SettingsScreen());
        mainMenuScreen?.Q("play-button")?.RegisterCallback<ClickEvent>(ev => PlayTask());
        mainMenuScreen?.Q("how-to-play-button")?.RegisterCallback<ClickEvent>(ev => HowToPlay());

        settingScreen?.Q("close-button")?.RegisterCallback<ClickEvent>(ev => SettingsBack());
        settingScreen?.Q("sound-button")?.RegisterCallback<ClickEvent>(ev => SettingsSound());
        settingScreen?.Q("difficulty-button")?.RegisterCallback<ClickEvent>(ev => SettingsDifficulty());

        backButtonScreen?.Q("back-button")?.RegisterCallback<ClickEvent>(ev => BackTask());
        //------------------UI REGION END--------------------------------------



        //------------------GAME REGION START-----------------------------------------------
        tutorial = GameObject.Find("Tutorial").GetComponent<TutorialScript>();
        solarSystem = GameObject.FindGameObjectWithTag("Sun").GetComponent<SolarSystem>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
        loadGameScene = GameObject.FindGameObjectWithTag("LoadSceneTag").GetComponent<LoadGameScene>();
        //------------------GAME REGION END-----------------------------------------------
    }

    void SettingsScreen()
    {
        settingScreen.style.display = DisplayStyle.Flex;
    }

    void SettingsBack()
    {
        settingScreen.style.display = DisplayStyle.None;
    }

    void SettingsDifficulty()
    {
        var but = settingScreen?.Q("difficulty-button");
        switch (difficulty)
        {
            case Difficulty.Easy:
                but.style.backgroundImage = Resources.Load("UI/Buttons/MediumButton") as Texture2D;
                difficulty = Difficulty.Medium;
                break;
            case Difficulty.Medium:
                but.style.backgroundImage = Resources.Load("UI/Buttons/HardButton") as Texture2D;
                difficulty = Difficulty.Hard;
                break;
            case Difficulty.Hard:
                but.style.backgroundImage = Resources.Load("UI/Buttons/EasyButton") as Texture2D;
                difficulty = Difficulty.Easy;
                break;
        }


    }

    void SettingsSound()
    {
        var but = settingScreen?.Q("sound-button");
        if (isSound)
        {
            but.style.backgroundImage = Resources.Load("UI/Buttons/DisabledButton") as Texture2D;
            isSound = false;
        }
        else
        {
            but.style.backgroundImage = Resources.Load("UI/Buttons/EnabledButton") as Texture2D;
            isSound = true;
        }

    }

    void HowToPlay()
    {

    }

    void PlayTask()
    {
        mainMenuScreen.style.display = DisplayStyle.None;
        backButtonScreen.style.display = DisplayStyle.Flex;
        loadGameScene.startWaitingForPlanetClick();
        solarSystem.EnableSolarSystemPhase(true);
        _CheckoutToSolarSystemPhase();
    }

    void BackTask()
    {
        Debug.Log("Back clicked");
        if (tutorial.IsTutorialEnabled())
        {
            //currentPhase = UI_Phase.SolarSystem;
            //foreach (GameObject infoPanel in planetInfoPanels)
            //{
            //    infoPanel.SetActive(false);
            //}
            //solveButton.SetActive(false);
        }
        switch (currentPhase)
        {
            case UI_Phase.SolarSystem:
                backButtonScreen.style.display = DisplayStyle.None;
                mainMenuScreen.style.display = DisplayStyle.Flex;
                StartCoroutine(_WaitForCameraMenuPhase());
                ////btmButton.SetActive(false);
                solarSystem.EnableSolarSystemPhase(false);
                mainCamera.GoMenu();
                tutorial.DisableTutorial();
                loadGameScene.stopWaitingForPlanetClick();

                break;
            case UI_Phase.PlanetInfo:
                mainCamera.GoFree();
                loadGameScene.startWaitingForPlanetClick();
                currentPhase = UI_Phase.SolarSystem;
                //foreach (GameObject infoPanel in planetInfoPanels)
                //{
                //    infoPanel.SetActive(false);
                //}
                //solveButton.SetActive(false);
                break;
            case UI_Phase.Puzzle:
                //pauseMenuCanvas.SetActive(true);
                //dificultySwitcherScript.nextButton.GetComponent<Button>().interactable = false;
                //dificultySwitcherScript.prevButton.GetComponent<Button>().interactable = false;
                currentPhase = UI_Phase.Pause;
                Time.timeScale = 0;
                isOnPause = true;
                //pauseMenuBG.SetActive(true);
                break;
            case UI_Phase.Settings:
                StartCoroutine(_WaitForCameraMenuPhase());
                //settingsSwitcher.ResetSwitchers();
                //btmButton.SetActive(false);
                //settingsMenu.SetActive(false);
                mainCamera.GoMenu();
                break;
            case UI_Phase.Pause:
                Time.timeScale = 1;
                currentPhase = UI_Phase.Puzzle;
                //dificultySwitcherScript.nextButton.GetComponent<Button>().interactable = true;
                //dificultySwitcherScript.prevButton.GetComponent<Button>().interactable = true;
                //pauseMenuBG.SetActive(false);
                //pauseMenuCanvas.SetActive(false);
                isOnPause = false;
                break;
            case UI_Phase.PauseSettings:
                currentPhase = UI_Phase.Pause;
                //settingsMenu.SetActive(false);
                //pauseMenuCanvas.SetActive(true);
                break;

        }
    }
    IEnumerator _WaitForCameraMenuPhase()
    {
        while (!mainCamera.isCurrentPhase(CameraScript.Phase.Menu))// IsCurrentPhaseMenu())
        {
            yield return new WaitForSeconds(0.01f);
        }
        //titleText.SetActive(true);
        //playButton.SetActive(true);
        //settingsButton.SetActive(true);
        //htpButton.SetActive(true);
        //exitButton.SetActive(true);
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
        //settingsMenu.SetActive(true);
        yield break;
    }
    IEnumerator _WaitForCameraLock(uint focusedPlanetInex)
    {
        while (!mainCamera.isCurrentPhase(CameraScript.Phase.PlanetLock))// IsCurrentPhasePlanetLock())
        {
            yield return new WaitForSeconds(0.01f);
        }
        //foreach (GameObject infoPanel in planetInfoPanels)
        //{
        //    if (infoPanel.transform.GetComponent<PlanetInfoScript>().GetIndex() == focusedPlanetInex)
        //    {
        //        infoPanel.SetActive(true);
        //    }
        //    else
        //    {
        //        infoPanel.SetActive(false);
        //    }
        //}
        currentPhase = UI_Phase.PlanetInfo;
        //btmButton.SetActive(true);
        //solveButton.SetActive(true);
        yield break;
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
}
