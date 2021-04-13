using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

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
    private bool loaded = false;

    //void Start() 
    //{
    //    Start_Puzzles(2, 0);
    //}

    public void Start_Puzzles(int PlanetType, int PuzzleLevel)
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
                config.outlineMaterial = Resources.Load("Materials/MarsTransparent") as Material;
                config.puzzleMaterial = Resources.Load("Materials/MarsOpaque") as Material;
                break;
            case 5:
                config.outlineMaterial = Resources.Load("Materials/MoonTransparent") as Material;
                config.puzzleMaterial = Resources.Load("Materials/MoonOpaque") as Material;
                break;
        }

        switch (PuzzleLevel)
        {

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
        var spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 10;
        planetPuzzle = Instantiate(prefab, spawnPos, Quaternion.identity) as GameObject;
        planetPuzzle.tag = "puzzle";
        planetPuzzle.GetComponent<Renderer>().material = config.puzzleMaterial;

        planetPuzzle.AddComponent<PlanetPuzzle>();

        config.outlineMeshResourceName = "Meshes/WholePlanet";

        planetPuzzle.GetComponent<PlanetPuzzle>().Init(config);
        loaded = true;

        if (!isPlanetPuzzleSplitUp)
        {

            planetPuzzle.GetComponent<PlanetPuzzle>().SplitUp();
            isPlanetPuzzleSplitUp = true;

        }
    }

    void Update()
    {
        /*while (myLogQueue.Count > 0)
            log = myLogQueue.Dequeue() + log;
        if (log.Length > MAXCHARS)
            log = log.Substring(0, MAXCHARS);*/

        if (loaded)
        {
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
