using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetPuzzle : MonoBehaviour
{
    public enum Error
    {
        OK,
        NOT_INITED,
        ALREADY_SPLIT_UP,
        CANNOT_LOAD_RESOURCE,
    }

    public struct Config
    {
        public string outlineMeshResourceName;
        public string puzzleMeshResourceName;

        public Material outlineMaterial;
        public Material puzzleMaterial;
    }

    public Error Init(Config config)
    {
        outlineMeshResourceName = config.outlineMeshResourceName;
        puzzleMeshResourceName = config.puzzleMeshResourceName;
        outlineMaterial = config.outlineMaterial;
        puzzleMaterial = config.puzzleMaterial;

        isInited = true;

        return Error.OK;
    }

    public Error Deinit()
    {
        Destroy(planetOutline);
        Destroy(puzzle);
        Destroy(puzzleFrame);

        return Error.OK;
    }

    public Error SplitUp()
    {
        if (!isInited)
            return Error.NOT_INITED;

        if (isSplitUp)
            return Error.ALREADY_SPLIT_UP;

        Hide();

        Error error;

        error = CreatePlanetOutline();
        if (error != Error.OK)
            return error;

        error = CreatePuzzleFrame();
        if (error != Error.OK)
            return error;

        error = CreatePuzzle();
        if (error != Error.OK)
            return error;

        isSplitUp = true;

        return Error.OK;
    }

    public bool IsPuzzleAssembled()
    {
        if (puzzle == null)
            return false;

        return puzzle.GetComponent<Puzzle>().IsAssembled();
    }

    void Start()
    {
    }

    void Update()
    {
        if (!isInited)
            return;
    }

    private bool isInited = false;
    private bool isSplitUp = false;

    private string outlineMeshResourceName;
    private string puzzleMeshResourceName;

    private Material outlineMaterial;
    private Material puzzleMaterial;

    private GameObject planetOutline;
    private GameObject puzzle;
    private GameObject puzzleFrame;

    private void Hide()
    {
        GetComponent<Renderer>().enabled = false;
    }

    private void SetRotatable(GameObject obj)
    {
        obj.AddComponent<Rotatable>();
    }

    private Error CreatePlanetOutline()
    {
        var prefab = Resources.Load(outlineMeshResourceName) as GameObject;

        if (prefab == null)
            return Error.CANNOT_LOAD_RESOURCE;

        planetOutline = Instantiate(prefab) as GameObject;

        planetOutline.GetComponent<Renderer>().material = outlineMaterial;

        planetOutline.transform.position = transform.position;
        planetOutline.transform.rotation = transform.rotation;
        planetOutline.transform.localScale = new Vector3(0.96f, 0.96f, 0.96f);

        SetRotatable(planetOutline);

        return Error.OK;
    }

    private Error CreatePuzzleFrame()
    {
        var prefab = Resources.Load(puzzleMeshResourceName) as GameObject;

        if (prefab == null)
            return Error.CANNOT_LOAD_RESOURCE;

        puzzleFrame = Instantiate(prefab) as GameObject;

        puzzleFrame.transform.position = transform.position;
        puzzleFrame.transform.rotation = transform.rotation;

        SetRotatable(puzzleFrame);

        return Error.OK;
    }

    private Error CreatePuzzle()
    {
        var prefab = Resources.Load(puzzleMeshResourceName) as GameObject;

        if (prefab == null)
            return Error.CANNOT_LOAD_RESOURCE;

        puzzle = Instantiate(prefab) as GameObject;

        puzzle.transform.position = transform.position;

        puzzle.AddComponent<Puzzle>();
        var puzzleScript = puzzle.GetComponent<Puzzle>();

        Puzzle.Config config;

        config.material = puzzleMaterial;
        config.planetOutline = planetOutline;
        config.puzzleFrame = puzzleFrame;
        config.pieceFitOnPos = CountPieceFitOnPos();

        puzzleScript.Init(config);

        return Error.OK;
    }

    private Vector3 CountPieceFitOnPos()
    {
        var outlineBounds = planetOutline.GetComponent<MeshFilter>().mesh.bounds.size.x;
        var currentPos = transform.position;

        return (new Vector3(currentPos.x, currentPos.y, currentPos.z - (outlineBounds / 2)));
    }
}
