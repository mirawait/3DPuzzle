using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public enum PlanetType
    {
        SUN,
        MERCURY,
        VENUS,
        EARTH,
        MARS,
        JUPITER,
        SATURN,
        URANUS,
        NEPTUNE,
        PLUTO,
        MOON,
    }

    public enum PuzzleLevel
    {
        PUZZLE_6_PARTS,
        PUZZLE_24_PARTS,
        PUZZLE_96_PARTS
    }

    public PlanetType planetType = PlanetType.EARTH;
    public PuzzleLevel puzzleLevel = PuzzleLevel.PUZZLE_24_PARTS;

    /*private string log;
    private const int MAXCHARS = 10000;
    private Queue myLogQueue = new Queue();*/

    private bool isPlanetPuzzleSplitUp = false;
    private bool isEnd = false;
    private GameObject planetPuzzle;
    private GameObject targetGameObject;


    public void Start()
    {
        if (targetGameObject == null)
            targetGameObject = GameObject.FindWithTag("Text");
        if (targetGameObject != null)
            targetGameObject.SetActive(false);

        PlanetPuzzle.Config config;

        switch (planetType)
        {
            default:
                config.outlineMaterial = Resources.Load("Materials/EarthTransparent") as Material;
                config.puzzleMaterial = Resources.Load("Materials/EarthOpaque") as Material;
                break;
            case PlanetType.SUN:
                config.outlineMaterial = Resources.Load("Materials/SunTransparent") as Material;
                config.puzzleMaterial = Resources.Load("Materials/SunOpaque") as Material;
                break;
            case PlanetType.MERCURY:
                config.outlineMaterial = Resources.Load("Materials/MercuryTransparent") as Material;
                config.puzzleMaterial = Resources.Load("Materials/MercuryOpaque") as Material;
                break;
            case PlanetType.VENUS:
                config.outlineMaterial = Resources.Load("Materials/VenusTransparent") as Material;
                config.puzzleMaterial = Resources.Load("Materials/VenusOpaque") as Material;
                break;
            case PlanetType.MOON:
                config.outlineMaterial = Resources.Load("Materials/MoonTransparent") as Material;
                config.puzzleMaterial = Resources.Load("Materials/MoonOpaque") as Material;
                break;
            case PlanetType.MARS:
                config.outlineMaterial = Resources.Load("Materials/MarsTransparent") as Material;
                config.puzzleMaterial = Resources.Load("Materials/MarsOpaque") as Material;
                break;
        }

        switch (puzzleLevel)
        {
            default:
                config.puzzleMeshResourceName = "Meshes/DividedBy6Planet";
                break;
            case PuzzleLevel.PUZZLE_24_PARTS:
                config.puzzleMeshResourceName = "Meshes/DividedBy24Planet";
                break;
            case PuzzleLevel.PUZZLE_96_PARTS:
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

 
}
