using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameScene : MonoBehaviour
{
    private bool isLoaded = false;
    private GameObject go;
    public void LoadScene()
    {
        GameObject Sun = GameObject.Find("Sun");

        if (!isLoaded)
        {
            Sun.GetComponent<SolarSystem>().EnableSolarSystemPhase(false);
            go = transform.gameObject;
            go.SetActive(false);
            uint planetClickSubscription = SolarSystemController.subscribeToPlanetClick(
                (GameObject target) =>
                {
                    if (target.tag == "Planet")
                        target.GetComponent<PlanetScript>().GetIndex();
                });
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            GameObject manager = GameObject.FindGameObjectWithTag("Manager");
            //manager.GetComponent<Manager>().StartPuzzle(1);
            isLoaded = true;
        }
    }

    public void UnloadScene()
    {
        if (isLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            isLoaded = false;
        }
    }
}
