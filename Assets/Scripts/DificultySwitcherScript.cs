using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class DificultySwitcherScript : MonoBehaviour
{
    GameObject nextButton, prevButton, text;

    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }
    Difficulty currentDifficulty = Difficulty.Normal;
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
                text.GetComponent<TextMeshProUGUI>().text = "Легкая";
                break;
            case Difficulty.Normal:
                text.GetComponent<TextMeshProUGUI>().text = "Нормальная";
                break;
            case Difficulty.Hard:
                text.GetComponent<TextMeshProUGUI>().text = "Тяжелая";
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

    public Difficulty GetChosenDifficulty()
    {
        return currentDifficulty;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
