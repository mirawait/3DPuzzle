using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class SolarSystem : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject[] planets;
    
    void Start()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet");
        EnableSolarSystemPhase(true);
    }

    public void EnableSolarSystemPhase(bool enable)
    {
        foreach (GameObject planet in planets)
        {
            planet.transform.GetComponent<PlanetScript>().EnableSolarRotation(enable);
        }
    }
    public void EnableSolarSystemMoving(bool enable)
    {
        foreach (GameObject planet in planets)
        {
            planet.transform.GetComponent<PlanetScript>().EnableMoving(enable);
        }
    }

    public GameObject FindPlanetByIndex(uint index)
    {
        foreach (GameObject planet in planets)
        {
            if (planet.transform.GetComponent<PlanetScript>().GetIndex() == index)
            {
                return planet;
            }
        }
        return null;
    }
    //Update is called once per frame
    void Update()
    {
       
    }
}
