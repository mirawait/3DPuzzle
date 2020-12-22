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

        public string outlineMaterialResourceName;
        public string puzzleMaterialResourceName;
    }

    public Error Init(Config config)
    {
        outlineMeshResourceName = config.outlineMeshResourceName;
        puzzleMeshResourceName = config.puzzleMeshResourceName;
        outlineMaterialResourceName = config.outlineMaterialResourceName;
        puzzleMaterialResourceName = config.puzzleMaterialResourceName;

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
        if (Error.OK != error)
            return error;

        error = CreatePuzzleFrame();
        if (Error.OK != error)
            return error;

        error = CreatePuzzle();
        if (Error.OK != error)
            return error;

        isSplitUp = true;

        return Error.OK;
    }

    public bool IsPuzzleAssembled()
    {
        if (null == puzzle)
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

    private string outlineMaterialResourceName;
    private string puzzleMaterialResourceName;

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

        if (null == prefab)
            return Error.CANNOT_LOAD_RESOURCE;

        planetOutline = Instantiate(prefab) as GameObject;

        var material = Resources.Load(outlineMaterialResourceName) as Material;

        if (null == material)
            return Error.CANNOT_LOAD_RESOURCE;

        planetOutline.GetComponent<Renderer>().material = material;

        planetOutline.transform.position = transform.position;
        planetOutline.transform.rotation = transform.rotation;

        SetRotatable(planetOutline);

        return Error.OK;
    }

    private Error CreatePuzzleFrame()
    {
        var prefab = Resources.Load(puzzleMeshResourceName) as GameObject;

        if (null == prefab)
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

        if (null == prefab)
            return Error.CANNOT_LOAD_RESOURCE;

        puzzle = Instantiate(prefab) as GameObject;

        puzzle.transform.position = transform.position;

        puzzle.AddComponent<Puzzle>();
        var puzzleScript = puzzle.GetComponent<Puzzle>();

        Puzzle.Config config;

        config.materialResourceName = puzzleMaterialResourceName;
        config.planetOutline = planetOutline;
        config.puzzleFrame = puzzleFrame;
        config.pieceFitOnPos = CountPieceFitOnPos();

        Puzzle.Error error;

        error = puzzleScript.Init(config);
        if (Puzzle.Error.OK != error)
        {
            if (Puzzle.Error.CANNOT_LOAD_RESOURCE == error)
                return Error.CANNOT_LOAD_RESOURCE;
        }

        return Error.OK;
    }

    private Vector3 CountPieceFitOnPos()
    {
        var outlineBounds = planetOutline.GetComponent<MeshFilter>().mesh.bounds.size.x * transform.localScale.x;
        var currentPos = transform.position;

        return (new Vector3(currentPos.x, currentPos.y, currentPos.z - (outlineBounds / 2)));
    }
}
