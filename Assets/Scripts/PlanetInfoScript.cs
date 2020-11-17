using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetInfoScript : MonoBehaviour
{
    [SerializeField]
    private  uint planetIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public uint GetIndex()
    {
        return planetIndex;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
