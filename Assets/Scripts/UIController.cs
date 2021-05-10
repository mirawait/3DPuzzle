using UnityEngine;
using UnityEngine.UIElements;


public class UIController : MonoBehaviour
{
    private UIDocument managerUI;

    public VisualElement mainMenuScreen;
    public VisualElement settingScreen;
    public VisualElement pauseScreen;
    public VisualElement areYouSureScreen;

    private bool isSound = true;
    private Difficulty difficulty = Difficulty.Medium;
    enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    private void Start()
    {
        managerUI = this.GetComponent<UIDocument>();

        mainMenuScreen = managerUI.rootVisualElement.Q("MainMenuUI");
        settingScreen = managerUI.rootVisualElement.Q("SettingsMenuUI");
        pauseScreen = managerUI.rootVisualElement.Q("PauseMenuUI");
        areYouSureScreen = managerUI.rootVisualElement.Q("AreYouSureMenuUI");

        mainMenuScreen?.Q("settings-button")?.RegisterCallback<ClickEvent>(ev => SettingsScreen());
        mainMenuScreen?.Q("play-button")?.RegisterCallback<ClickEvent>(ev => PlayTask());
        mainMenuScreen?.Q("how-to-play-button")?.RegisterCallback<ClickEvent>(ev => HowToPlay());

        settingScreen?.Q("close-button")?.RegisterCallback<ClickEvent>(ev => SettingsBack());
        settingScreen?.Q("sound-button")?.RegisterCallback<ClickEvent>(ev => SettingsSound());
        settingScreen?.Q("difficulty-button")?.RegisterCallback<ClickEvent>(ev => SettingsDifficulty());
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
        GameObject.Find("Main Menu").GetComponent<MenuScript>().PlayTask();
    }

}
