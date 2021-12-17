using UnityEngine;
using UnityEngine.UIElements;
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
    //9 - NEPTUNE

    //Parts
    //0 - 6_PARTS,
    //1 - 24_PARTS,
    //2 - 96_PARTS


    private bool isPlanetPuzzleSplitUp = false;
    private bool isEnd = false;
    private GameObject planetPuzzle;
    private GameObject targetGameObject;
    private bool loaded = false;
    int type;
    int level;
 //   public Object prefab;

    private SaveManager saveManager;
   

    void Start()
    {
        saveManager = GameObject.FindGameObjectWithTag("LoadSceneTag").GetComponent<SaveManager>();
    }

    public void Start_Puzzles(int planetType, int puzzleLevel)
    {
        type = planetType;
        level = puzzleLevel;

        //saveManager.MakeDone(planetType, puzzleLevel);

        Debug.LogError("MANAGER PUZZLE TYPE " + planetType);


        if (targetGameObject == null)
            targetGameObject = GameObject.Find("--------USER INTERFACE--------");
       // if (targetGameObject != null)
            //targetGameObject.SetActive(false);

        //Debug.Log("Screen logger started");

        PlanetPuzzle.Config config;

        switch (planetType)
        {
            case 0:
                config.outlineMaterial = Resources.Load("Materials/SunDark") as Material;
                config.puzzleMaterial = Resources.Load("Materials/SunOpaque") as Material;
                config.transparentMaterial = Resources.Load("Materials/SunTransparent") as Material;
                break;
            case 1:
                config.outlineMaterial = Resources.Load("Materials/MercuryDark") as Material;
                config.puzzleMaterial = Resources.Load("Materials/MercuryOpaque") as Material;
                config.transparentMaterial = Resources.Load("Materials/MercuryTransparent") as Material;
                break;
            case 2:
                config.outlineMaterial = Resources.Load("Materials/VenusDark") as Material;
                config.puzzleMaterial = Resources.Load("Materials/VenusOpaque") as Material;
                config.transparentMaterial = Resources.Load("Materials/VenusTransparent") as Material;
                break;
            case 3:
                config.outlineMaterial = Resources.Load("Materials/EarthDark") as Material;
                config.puzzleMaterial = Resources.Load("Materials/EarthOpaque") as Material;
                config.transparentMaterial = Resources.Load("Materials/EarthTransparent") as Material;
                break;
            case 4:
                config.outlineMaterial = Resources.Load("Materials/MarsDark") as Material;
                config.puzzleMaterial = Resources.Load("Materials/MarsOpaque") as Material;
                config.transparentMaterial = Resources.Load("Materials/MarsTransparent") as Material;
                break;
            case 5:
                config.outlineMaterial = Resources.Load("Materials/SaturnDark") as Material;
                config.puzzleMaterial = Resources.Load("Materials/SaturnOpaque") as Material;
                config.transparentMaterial = Resources.Load("Materials/SaturnTransparent") as Material;
                break;
            case 6:
                config.outlineMaterial = Resources.Load("Materials/JupiterDark") as Material;
                config.puzzleMaterial = Resources.Load("Materials/JupiterOpaque") as Material;
                config.transparentMaterial = Resources.Load("Materials/JupiterTransparent") as Material;
                break;
            case 7:
                config.outlineMaterial = Resources.Load("Materials/UranusDark") as Material;
                config.puzzleMaterial = Resources.Load("Materials/UranusOpaque") as Material;
                config.transparentMaterial = Resources.Load("Materials/UranusTransparent") as Material;
                break;
            case 8:
                config.outlineMaterial = Resources.Load("Materials/NeptuneDark") as Material;
                config.puzzleMaterial = Resources.Load("Materials/NeptuneOpaque") as Material;
                config.transparentMaterial = Resources.Load("Materials/NeptuneTransparent") as Material;
                break;
            default:
                config.outlineMaterial = Resources.Load("Materials/SunDark") as Material;
                config.puzzleMaterial = Resources.Load("Materials/SunOpaque") as Material;
                config.transparentMaterial = Resources.Load("Materials/SunTransparent") as Material;
                break;
        }

        switch (puzzleLevel)
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

        GameObject prefab = Resources.Load("Meshes/WholePlanet") as GameObject;
        var spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 10;
        Debug.Log(prefab);
        
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
                        targetGameObject.GetComponent<UIManager>().PuzzleSolvedShow();
                    //print("victory!");

                    saveManager.MakeDone(type, level);

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
