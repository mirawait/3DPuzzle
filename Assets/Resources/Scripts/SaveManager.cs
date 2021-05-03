using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private class Save
    {
        public bool[] light = new bool[8];
        public bool[] medium = new bool[8];
        public bool[] hard = new bool[8];
        public bool tutorial = false;
    }

    private Save save = new Save();

    public bool IsTutorialDone()
    {
        return save.tutorial;
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
    private void DeleteData()
    {
        PlayerPrefs.DeleteKey("Save");
    }


    public void MakeTutorialDone()
    {
        save.tutorial = true;
    }

    public void MakeDone(int type, int level)
    {
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

    //void OnGUI()
    //{
    //    if (save.light[3])
    //    {
    //        GUI.Label(new Rect(330, 120, 100, 20), "sdfdsf");
    //    }
    //}

}
