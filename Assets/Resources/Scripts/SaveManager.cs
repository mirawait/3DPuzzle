using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private class Save
    {
        public bool[] light = new bool[9];
        public bool[] medium = new bool[9];
        public bool[] hard = new bool[9];
        public bool tutorial = false;
        public bool sound = true;
        public UIManager.Difficulty difficulty = UIManager.Difficulty.Medium;
    }


    private Save save = new Save();

    public bool IsTutorialDone()
    {
        return save.tutorial;
    }

    public bool GetSound()
    {
        return save.sound;
    } 
    
    public UIManager.Difficulty GetDifficulty()
    {
        return save.difficulty;
    }


public bool IsPlanetDone(int PlanetType, int PuzzleLevel)
    {
        if (PuzzleLevel == 0)
        {
            return save.light[PlanetType];
        }
        else if (PuzzleLevel == 1)
        {
            return save.medium[PlanetType];
        }
        else if (PuzzleLevel == 2)
        {
            return save.hard[PlanetType];
        }
        else return false;
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("Save"))
        {
            LoadData();
            print(save.difficulty);
            print(save.sound);
        }
        else
        {
            SaveData();
        }
    }
    private void LoadData()
    {
        save = JsonUtility.FromJson<Save>(PlayerPrefs.GetString("Save"));
    }
    private void SaveData()
    {
        PlayerPrefs.SetString("Save", JsonUtility.ToJson(save));
    }

    public void MakeTutorialDone()
    {
        save.tutorial = true;
        SaveData();
    }

    public void MakeDone(int type, int level)
    {
        //LoadData();

        if (level == 0)
        {
            save.light[type] = true;
            print("saved_light");
        }
        else if (level == 1)
        {
            save.medium[type] = true;
            print("saved_medium");
        }
        else if (level == 2)
        {
            save.hard[type] = true;
            print("saved_hard");
        }

        SaveData();
    }

    public void SaveSettings(bool sound, UIManager.Difficulty difficulty)
    {
        save.sound = sound;
        save.difficulty = difficulty;
        SaveData();
    }



}
