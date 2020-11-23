using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;


public class SoundSwitcherScript : MonoBehaviour
{

    GameObject nextButton, prevButton, text;

    public enum Sound
    {
        On,
        Off
    }
    Sound currentStatus = Sound.On;
    // Start is called before the first frame update
    void Start()
    {
        nextButton = GameObject.Find("NextSoundButton");
        prevButton = GameObject.Find("PrevSoundButton");
        text = GameObject.Find("CurSoundText");

        nextButton.GetComponent<Button>().onClick.AddListener(NextTask);
        prevButton.GetComponent<Button>().onClick.AddListener(PrevTask);
    }

    void _UpdateText()
    {
        switch (currentStatus)
        {
            case Sound.On:
                text.GetComponent<TextMeshProUGUI>().text = "Включен";
                break;
            case Sound.Off:
                text.GetComponent<TextMeshProUGUI>().text = "Выключен";
                break;
        }
    }

    public Sound GetSoundStatus()
    {
        return currentStatus;
    }

    public void ResetTitle(Sound status)
    {
        currentStatus = status;
        _UpdateText();
    }

    void NextTask()
    {
        currentStatus = Sound.On;
        _UpdateText();
    }

    void PrevTask()
    {
        currentStatus = Sound.Off;
        _UpdateText();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
