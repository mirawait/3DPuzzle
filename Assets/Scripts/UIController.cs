using UnityEngine;
using UnityEngine.UIElements;

namespace SpacePuzzleLegends
{
    public class UIController : VisualElement
    {
        private VisualElement mainMenuScreen;
        private VisualElement settingsScreen;

        private Button settingsButton, howToPlayButton, playButton;

        private void OnEnable()
        {
            var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

            settingsButton = rootVisualElement.Q<Button>("settings-button");
            playButton = rootVisualElement.Q<Button>("play-button");
            howToPlayButton = rootVisualElement.Q<Button>("how-to-play-button");

            settingsButton.RegisterCallback<ClickEvent>(ev => GoSettingsTask());
        }

        private void GoSettingsTask()
        {

        }

        private void GoHowToPlayTask()
        {

        }

        private void GoPlayTask()
        {

        }
    }
}