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
        MOON,
        MARS,
        //JUPITER,
        //SATURN,
        //URANUS,
        //NEPTUNE,
        //PLUTO
    }

    public enum PuzzleLevel
    {
        PUZZLE_6_PARTS,
        PUZZLE_24_PARTS,
        PUZZLE_96_PARTS
    }

    public PlanetType planetType = PlanetType.EARTH;
    public PuzzleLevel puzzleLevel = PuzzleLevel.PUZZLE_24_PARTS;

    private bool isPlanetPuzzleSplitUp = false;
    private bool isEnd = false;
    private GameObject planetPuzzle;

    void Start()
    {
        PlanetPuzzle.Config config;
        Material material;

        switch (planetType)
        {
            case PlanetType.SUN:
                config.outlineMaterialResourceName = "Materials/SunTransparent";
                config.puzzleMaterialResourceName = "Materials/SunOpaque";
                material = Resources.Load("Materials/SunOpaque") as Material;
                break;
            case PlanetType.MERCURY:
                config.outlineMaterialResourceName = "Materials/MercuryTransparent";
                config.puzzleMaterialResourceName = "Materials/MercuryOpaque";
                material = Resources.Load("Materials/MercuryOpaque") as Material;
                break;
            case PlanetType.VENUS:
                config.outlineMaterialResourceName = "Materials/VenusTransparent";
                config.puzzleMaterialResourceName = "Materials/VenusOpaque";
                material = Resources.Load("Materials/VenusOpaque") as Material;
                break;
            case PlanetType.EARTH:
            default:
                config.outlineMaterialResourceName = "Materials/EarthTransparent";
                config.puzzleMaterialResourceName = "Materials/EarthOpaque";
                material = Resources.Load("Materials/EarthOpaque") as Material;
                break;
            case PlanetType.MOON:
                config.outlineMaterialResourceName = "Materials/MoonTransparent";
                config.puzzleMaterialResourceName = "Materials/MoonOpaque";
                material = Resources.Load("Materials/MoonOpaque") as Material;
                break;
            case PlanetType.MARS:
                config.outlineMaterialResourceName = "Materials/MarsTransparent";
                config.puzzleMaterialResourceName = "Materials/MarsOpaque";
                material = Resources.Load("Materials/MarsOpaque") as Material;
                break;
        }

        switch (puzzleLevel)
        {
            case PuzzleLevel.PUZZLE_6_PARTS:
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

        planetPuzzle.GetComponent<Renderer>().material = material;

        var cameraPos = Camera.main.transform.position;
        planetPuzzle.transform.position = new Vector3(cameraPos.x, cameraPos.y, cameraPos.z + 6);

        planetPuzzle.AddComponent<PlanetPuzzle>();

        config.outlineMeshResourceName = "Meshes/WholePlanet";

        planetPuzzle.GetComponent<PlanetPuzzle>().Init(config);
    }

    void Update()
    {
        if (isEnd)
            return;

        if (!isEnd)
        {
            if (planetPuzzle.GetComponent<PlanetPuzzle>().IsPuzzleAssembled())
            {
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
