using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class DificultySwitcherScript : MonoBehaviour
{
    GameObject  text;
    public GameObject nextButton, prevButton;
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }
    static Difficulty currentDifficulty = Difficulty.Normal;
    // Start is called before the first frame update
    void Start()
    {
        nextButton = GameObject.Find("NextDifButton");
        prevButton = GameObject.Find("PrevDifButton");
        text = GameObject.Find("CurDifText");

        nextButton.GetComponent<Button>().onClick.AddListener(NextTask);
        prevButton.GetComponent<Button>().onClick.AddListener(PrevTask);
    }

    void _UpdateText()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                text.GetComponent<TextMeshProUGUI>().text = "Легко";
                break;
            case Difficulty.Normal:
                text.GetComponent<TextMeshProUGUI>().text = "Средне";
                break;
            case Difficulty.Hard:
                text.GetComponent<TextMeshProUGUI>().text = "Сложно";
                break;
        }
    }

    void NextTask()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                currentDifficulty = Difficulty.Normal;
                break;
            case Difficulty.Normal:
                currentDifficulty = Difficulty.Hard;
                break;
            case Difficulty.Hard:
                break;
        }
        _UpdateText();
    }

    void PrevTask()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                break;
            case Difficulty.Normal:
                currentDifficulty = Difficulty.Easy;
                break;
            case Difficulty.Hard:
                currentDifficulty = Difficulty.Normal;
                break;
        }
        _UpdateText();
    }

    public void ResetTitle(Difficulty status)
    {
        currentDifficulty = status;
        _UpdateText();
    }

    public static Difficulty GetChosenDifficulty()
    {
        return currentDifficulty;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
