using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    //Planets
        //0 - SUN,
        //1 - MERCURY,
        //2 - VENUS,
        //3 - EARTH,
        //4 - MOON,
        //5 - MARS,
        //6 - JUPITER,
        //7 - SATURN,
        //8 - URANUS,
        //9 - NEPTUNE,
        //10 - PLUTO

    //Parts
        //0 - 6_PARTS,
        //1 - 24_PARTS,
        //2 - 96_PARTS
        

    private bool isPlanetPuzzleSplitUp = false;
    private bool isEnd = false;
    private GameObject planetPuzzle;
    private GameObject targetGameObject;

    //void Start() 
    //{
    //    Start_Puzzles(2, 0);
    //}

    void Start_Puzzles(int PlanetType, int PuzzleLevel)
    {
        if (targetGameObject == null)
            targetGameObject = GameObject.FindWithTag("Text");
        if (targetGameObject != null)
            targetGameObject.SetActive(false);

        //Debug.Log("Screen logger started");

        PlanetPuzzle.Config config;

        switch (PlanetType)
        {
            case 0:
                config.outlineMaterial = Resources.Load("Materials/SunTransparent") as Material;
                config.puzzleMaterial = Resources.Load("Materials/SunOpaque") as Material;
                break;
            case 1:
                config.outlineMaterial = Resources.Load("Materials/MercuryTransparent") as Material;
                config.puzzleMaterial = Resources.Load("Materials/MercuryOpaque") as Material;
                break;
            case 2:
                config.outlineMaterial = Resources.Load("Materials/VenusTransparent") as Material;
                config.puzzleMaterial = Resources.Load("Materials/VenusOpaque") as Material;
                break;
            case 3:
            default:
                config.outlineMaterial = Resources.Load("Materials/EarthTransparent") as Material;
                config.puzzleMaterial = Resources.Load("Materials/EarthOpaque") as Material;
                break;
            case 4:
                config.outlineMaterial = Resources.Load("Materials/MoonTransparent") as Material;
                config.puzzleMaterial = Resources.Load("Materials/MoonOpaque") as Material;
                break;
            case 5:
                config.outlineMaterial = Resources.Load("Materials/MarsTransparent") as Material;
                config.puzzleMaterial = Resources.Load("Materials/MarsOpaque") as Material;
                break;
        }

        switch (PuzzleLevel)
        {
            case 0:
            default:
                config.puzzleMeshResourceName = "Meshes/DividedBy6Planet";
                break;
            case 1:
                config.puzzleMeshResourceName = "Meshes/DividedBy24Planet";
                break;
            case 2:
                config.puzzleMeshResourceName = "Meshes/DividedBy96Planet";
                break;
        }

        var prefab = Resources.Load("Meshes/WholePlanet") as GameObject;
        planetPuzzle = Instantiate(prefab) as GameObject;

        planetPuzzle.GetComponent<Renderer>().material = config.puzzleMaterial;

        var cameraPos = Camera.main.transform.position;
        planetPuzzle.transform.position = new Vector3(cameraPos.x, cameraPos.y, cameraPos.z + 6);

        planetPuzzle.AddComponent<PlanetPuzzle>();

        config.outlineMeshResourceName = "Meshes/WholePlanet";

        planetPuzzle.GetComponent<PlanetPuzzle>().Init(config);
    }

    void Update()
    {
        /*while (myLogQueue.Count > 0)
            log = myLogQueue.Dequeue() + log;
        if (log.Length > MAXCHARS)
            log = log.Substring(0, MAXCHARS);*/

        if (isEnd)
        {
            return;
        }

            

        if (!isEnd)
        {
            if (planetPuzzle.GetComponent<PlanetPuzzle>().IsPuzzleAssembled())
            {
                if (targetGameObject != null)
                    targetGameObject.SetActive(true);
                print("victory!");
                isEnd = true;
                return;
            }
        }

        if (!isPlanetPuzzleSplitUp)
        {
            if (Input.GetMouseButtonUp(0))
            {
                planetPuzzle.GetComponent<PlanetPuzzle>().SplitUp();

                isPlanetPuzzleSplitUp = true;
            }
        }
    }

    /*void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue("\n [" + type + "] : " + logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue("\n" + stackTrace);
    }

    void OnGUI()
    {
        GUILayout.Label(log);
    }*/
}
