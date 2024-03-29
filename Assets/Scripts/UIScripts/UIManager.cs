﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class UIManager : MonoBehaviour
{
    //----------------------UI REGION START---------------------
    public VisualElement mainMenuScreen;
    public VisualElement settingScreen;
    public VisualElement pauseScreen;
    public VisualElement areYouSureScreen;
    public VisualElement backButtonScreen;
    public VisualElement planetInfoScreen;
    public VisualElement endScreen;

    public string currentPlanetName = "";
    //----------------------UI REGION END---------------------



    //----------------------GAME REGION START---------------------
    private TutorialScript tutorial;
    private SolarSystem solarSystem;
    private CameraScript mainCamera;
    private LoadGameScene loadGameScene;
    private SaveManager saveManager;
    private AudioSource audioSource;
    private GameObject planetIsDoneText;
    private TapController tapController;
    uint lastActiveInfoPanel;
    int planetClickSubscription;
    private static bool isOnPause = false;
    private bool isInGame = false;

    private bool isSound = true;
    static private Difficulty difficulty = Difficulty.Medium;
    public enum Difficulty
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
    private UI_Phase currentPhase = UI_Phase.MainMenu;

    //----------------------GAME REGION END---------------------

    private void Start()
    {
        //------------------UI REGION START--------------------------------------
        mainMenuScreen = GameObject.Find("MainMenuUI").GetComponent<UIDocument>().rootVisualElement;
        settingScreen = GameObject.Find("SettingsMenuUI").GetComponent<UIDocument>().rootVisualElement;
        pauseScreen = GameObject.Find("PauseMenuUI").GetComponent<UIDocument>().rootVisualElement;
        areYouSureScreen = GameObject.Find("AreYouSureMenuUI").GetComponent<UIDocument>().rootVisualElement;
        backButtonScreen = GameObject.Find("BackUI").GetComponent<UIDocument>().rootVisualElement;
        planetInfoScreen = GameObject.Find("PlanetInfoUI").GetComponent<UIDocument>().rootVisualElement;
        endScreen = GameObject.Find("EndUI").GetComponent<UIDocument>().rootVisualElement;

        settingScreen.style.display = DisplayStyle.None;
        pauseScreen.style.display = DisplayStyle.None;
        areYouSureScreen.style.display = DisplayStyle.None;
        backButtonScreen.style.display = DisplayStyle.None;
        planetInfoScreen.style.display = DisplayStyle.None;
        endScreen.style.display = DisplayStyle.None;

        mainMenuScreen?.Q("settings-button")?.RegisterCallback<ClickEvent>(ev => SettingsScreen());
        mainMenuScreen?.Q("play-button")?.RegisterCallback<ClickEvent>(ev => PlayTask());
        mainMenuScreen?.Q("how-to-play-button")?.RegisterCallback<ClickEvent>(ev => HowToPlay());

        settingScreen?.Q("close-button")?.RegisterCallback<ClickEvent>(ev => SettingsBack());
        settingScreen?.Q("sound-button")?.RegisterCallback<ClickEvent>(ev => SettingsSound());
        settingScreen?.Q("difficulty-button")?.RegisterCallback<ClickEvent>(ev => SettingsDifficulty());

        pauseScreen?.Q("settings-button")?.RegisterCallback<ClickEvent>(ev => SettingsScreen());
        pauseScreen?.Q("close-button")?.RegisterCallback<ClickEvent>(ev => BackTask());
        pauseScreen?.Q("mainmenu-button")?.RegisterCallback<ClickEvent>(ev => MainMenuTask());

        areYouSureScreen?.Q("agree-button")?.RegisterCallback<ClickEvent>(ev => AgreeTask());
        areYouSureScreen?.Q("disagree-button")?.RegisterCallback<ClickEvent>(ev => DisagreeTask());

        backButtonScreen?.Q("back-button")?.RegisterCallback<ClickEvent>(ev => BackTask());

        planetInfoScreen?.Q("solve-button")?.RegisterCallback<ClickEvent>(ev => SolveTask());

        endScreen?.Q("exit-button")?.RegisterCallback<ClickEvent>(ev => { PuzzleSolvedHide(); AgreeTask();  });
        endScreen?.Q("next-button")?.RegisterCallback<ClickEvent>(ev =>
        {
            difficulty = (difficulty == Difficulty.Hard) ? Difficulty.Easy : difficulty+1;
            SolveTask();
            endScreen.style.display = DisplayStyle.None;
        });

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //------------------UI REGION END--------------------------------------


        //------------------GAME REGION START-----------------------------------------------
        planetIsDoneText = GameObject.Find("PlanetIsDoneText");
        planetIsDoneText.SetActive(false);
        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        tutorial = GameObject.Find("Tutorial").GetComponent<TutorialScript>();
        solarSystem = GameObject.FindGameObjectWithTag("Sun").GetComponent<SolarSystem>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
        loadGameScene = GameObject.FindGameObjectWithTag("LoadSceneTag").GetComponent<LoadGameScene>();
        saveManager = GameObject.FindGameObjectWithTag("LoadSceneTag").GetComponent<SaveManager>();
        tapController = GameObject.Find("Controller").GetComponent<TapController>();
        //------------------GAME REGION END-----------------------------------------------
    }

    void SettingsScreen()
    {
        var but = settingScreen?.Q("difficulty-button");
        var butSound = settingScreen?.Q("sound-button");
        if (currentPhase != UI_Phase.MainMenu)
        {
            but.SetEnabled(false);
        }
        settingScreen.style.display = DisplayStyle.Flex;
        difficulty = saveManager.GetDifficulty();
        isSound = saveManager.GetSound();
        switch (difficulty)
        {
            case Difficulty.Easy:
                but.style.backgroundImage = Resources.Load("UI/Buttons/EasyButton") as Texture2D;
                break;
            case Difficulty.Medium:
                but.style.backgroundImage = Resources.Load("UI/Buttons/MediumButton") as Texture2D;
                break;
            case Difficulty.Hard:
                but.style.backgroundImage = Resources.Load("UI/Buttons/HardButton") as Texture2D;
                break;
        }
        
        if (isSound)
        {
            butSound.style.backgroundImage = Resources.Load("UI/Buttons/EnabledButton") as Texture2D;
            audioSource.Play();
        }
        else
        {
            butSound.style.backgroundImage = Resources.Load("UI/Buttons/DisabledButton") as Texture2D;
            audioSource.Stop();
        }
    }

    void SettingsBack()
    {
        var but = settingScreen?.Q("difficulty-button");
        but.SetEnabled(true);
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
        saveManager.SaveSettings(isSound, difficulty);
    }

    void SettingsSound()
    {
        var but = settingScreen?.Q("sound-button");
        if (isSound)
        {
            but.style.backgroundImage = Resources.Load("UI/Buttons/DisabledButton") as Texture2D;
            isSound = false;
            audioSource.Stop();
        }
        else
        {
            but.style.backgroundImage = Resources.Load("UI/Buttons/EnabledButton") as Texture2D;
            isSound = true;
            audioSource.Play();
        }
        saveManager.SaveSettings(isSound, difficulty);
    }

    void MainMenuTask()
    {
        areYouSureScreen.style.display = DisplayStyle.Flex;
    }

    void AgreeTask()
    {
        //planetNameInGame.SetActive(false);
        Time.timeScale = 1;
        isOnPause = false;
        mainCamera.EnablePuzzleLock(false);
        currentPhase = UI_Phase.PlanetInfo;
        StartCoroutine(_WaitForCameraLock(lastActiveInfoPanel));
        areYouSureScreen.style.display = DisplayStyle.None;
        pauseScreen.style.display = DisplayStyle.None;
        var planetName = backButtonScreen.Q<Label>("name-label");
        planetName.style.display = DisplayStyle.None;
        //solveButton.SetActive(true);
        //dificultySwitcherScript.nextButton.GetComponent<Button>().interactable = true;
        //dificultySwitcherScript.prevButton.GetComponent<Button>().interactable = true;
        //pauseMenuBG.SetActive(false);
        //settingsMenu.SetActive(false);
        //pauseMenuCanvas.SetActive(false);
        loadGameScene.UnloadScene();
        BackTask();
        BackTask();
    }
    void DisagreeTask()
    {
        areYouSureScreen.style.display = DisplayStyle.None;
    }
    void HowToPlay()
    {
        tutorial.EnableTutorial();
        PlayTask();
    }

    void SolveTask()
    {
        currentPhase = UI_Phase.Puzzle;
        mainCamera.EnablePuzzleLock(true);
        planetInfoScreen.style.display = DisplayStyle.None;
        var planetName = backButtonScreen.Q<Label>("name-label");
        planetName.style.display = DisplayStyle.Flex;
        planetName.text = currentPlanetName;
        planetIsDoneText.SetActive(false);
        //planetNameInGame.SetActive(true);
        // planetNameInGame.GetComponent<TextMeshProUGUI>().text = IndexToName(loadGameScene.planetType);
        
        //GesturesController.RecogniseDoubleTouchGesturesAs_Shuffle();
        
        loadGameScene.LoadScene();
        loadGameScene.StopWaitingForPlanetClick();
    }
    void PlayTask()
    {
        if (saveManager.IsTutorialDone() == false)
        {
            tutorial.EnableTutorial();
        }
        currentPhase = UI_Phase.SolarSystem;
        mainMenuScreen.style.display = DisplayStyle.None;
        backButtonScreen.style.display = DisplayStyle.Flex;
        loadGameScene.StartWaitingForPlanetClick();
        solarSystem.EnableSolarSystemPhase(true);
        
        //GesturesController.RecogniseDoubleTouchGesturesAs_PinchSpread();
        
        _CheckoutToSolarSystemPhase();
    }


    void SetFactText()
    {
        var factLabel = endScreen.Q<Label>("fact-label");
        var factText = TextTable.GetLines(IndexToName(lastActiveInfoPanel), "WinInfo");
        factLabel.text = factText[1];
    }

    public void PuzzleSolvedShow()
    {
        SetFactText();

        endScreen.Q<Label>("complete-label").style.opacity = 0;
        
        endScreen.style.display = DisplayStyle.Flex;

        endScreen.Q<Label>("complete-label").experimental.animation
            .Start(new StyleValues { width = 1095, height = 532, opacity = 0 },
                new StyleValues { width = 1095, height = 532, opacity = 1 }, 3000).Ease(Easing.OutQuad)
            .OnCompleted(() => {
                endScreen.Q<Label>("complete-label").experimental.animation
                    .Start(new StyleValues { opacity = 1 }, new StyleValues { opacity = 1 }, 2000).Ease(Easing.OutQuad)
                    .OnCompleted(() => {
                        endScreen.Q<Label>("complete-label").experimental.animation
                            .Start(new StyleValues { width = 1095, height = 532, marginTop = 256 }, new StyleValues { width = 714, height = 365, marginTop = -100 }, 2000).Ease(Easing.OutQuad);
                    });
            });

        endScreen.Q<Label>("fact-label").experimental.animation
            .Start(new StyleValues { opacity = 0 },
                new StyleValues { opacity = 0 }, 5000).Ease(Easing.OutQuad).OnCompleted(() =>
            {
                endScreen.Q<Label>("fact-label").experimental.animation
                    .Start(new StyleValues { opacity = 0 },
                        new StyleValues { opacity = 1 }, 2000).Ease(Easing.OutQuad);
            });

        endScreen.Q<Button>("exit-button").experimental.animation
            .Start(new StyleValues { opacity = 0 },
                new StyleValues { opacity = 0 }, 5000).Ease(Easing.OutQuad).OnCompleted(() =>
            {
                endScreen.Q<Button>("exit-button").experimental.animation
                    .Start(new StyleValues { opacity = 0 },
                        new StyleValues { opacity = 1 }, 2000).Ease(Easing.OutQuad);
            });


        endScreen.Q<Button>("next-button").experimental.animation
            .Start(new StyleValues { opacity = 0 },
                new StyleValues { opacity = 0 }, 5000).Ease(Easing.OutQuad).OnCompleted(() =>
            {
                endScreen.Q<Button>("next-button").experimental.animation
                    .Start(new StyleValues { opacity = 0 },
                        new StyleValues { opacity = 1 }, 2000).Ease(Easing.OutQuad);
            });
    }

    public void PuzzleSolvedHide()
    {
        endScreen.style.display = DisplayStyle.None;
    }
    void BackTask()
    {
        Debug.Log("Back clicked");
        if (TutorialScript.IsTutorialEnabled())
        {
            currentPhase = UI_Phase.SolarSystem;
            planetInfoScreen.style.display = DisplayStyle.None;
            var planetName = backButtonScreen.Q<Label>("name-label");
            planetName.style.display = DisplayStyle.None;
            loadGameScene.UnloadScene();
            planetIsDoneText.SetActive(false);
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
                currentPhase = UI_Phase.MainMenu;
                StartCoroutine(_WaitForCameraMenuPhase());
                ////btmButton.SetActive(false);
                //solarSystem.EnableSolarSystemPhase(false);
                mainCamera.GoMenu();
                tutorial.DisableTutorial();
                loadGameScene.StopWaitingForPlanetClick();

                break;
            case UI_Phase.PlanetInfo:
                mainCamera.GoFree();
                loadGameScene.StartWaitingForPlanetClick();
                currentPhase = UI_Phase.SolarSystem;
                planetInfoScreen.style.display = DisplayStyle.None;
                planetIsDoneText.SetActive(false);
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
                pauseScreen.style.display = DisplayStyle.Flex;
                backButtonScreen.style.display = DisplayStyle.None;
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
                pauseScreen.style.display = DisplayStyle.None;
                backButtonScreen.style.display = DisplayStyle.Flex;
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
        while (!mainCamera.IsCurrentPhase(CameraScript.Phase.Menu))// IsCurrentPhaseMenu())
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
        while (!mainCamera.IsCurrentPhase(CameraScript.Phase.Settings))// IsCurrentPhaseMenu())
        {
            yield return new WaitForSeconds(0.01f);
        }
        currentPhase = UI_Phase.Settings;
        //settingsMenu.SetActive(true);
        yield break;
    }
    IEnumerator _WaitForCameraLock(uint focusedPlanetInex)
    {
        while (!mainCamera.IsCurrentPhase(CameraScript.Phase.PlanetLock))// IsCurrentPhasePlanetLock())
        {
            yield return new WaitForSeconds(0.01f);
        }

        planetInfoScreen.style.display = DisplayStyle.Flex;
        var planetInfoText = planetInfoScreen.Q<Label>("info-label");
        var planetInfoName = planetInfoScreen.Q<Label>("name-label");
        var planetInfo = TextTable.GetLines(IndexToName(focusedPlanetInex), "PlanetInfo");
        planetInfoName.text = planetInfo[0];
        planetInfoText.text = planetInfo[1];
        currentPlanetName = planetInfo[0];
        if (saveManager.IsPlanetDone((int)focusedPlanetInex, (int)UIManager.GetDifficulty()))
        {
            planetIsDoneText.SetActive(true);
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
        planetClickSubscription = tapController.SubscribeToTap(TapController.Tap.Tap,
            (GameObject target) =>
            {
                if (currentPhase == UI_Phase.SolarSystem && target != null && target.tag == "Planet")
                {
                    lastActiveInfoPanel = target.GetComponent<PlanetScript>().GetIndex();
                    StartCoroutine(_WaitForCameraLock(lastActiveInfoPanel));
                    loadGameScene.StopWaitingForPlanetClick();
                }
            });
        currentPhase = UI_Phase.SolarSystem;
    }
    string IndexToName(uint index)
    {
        switch (index)
        {
            case 0:
                return "Солнце";
            case 1:
                return "Меркурий";
            case 2:
                return "Венера";
            case 3:
                return "Земля";
            case 4:
                return "Марс";
            case 5:
                return "Сатурн";
            case 6:
                return "Юпитер";
            case 7:
                return "Уран";
            case 8:
                return "Нептун";
        }
        return "ПЛАНЕТА";
    }

    static public Difficulty GetDifficulty()
    {
        return difficulty;
    }
}

