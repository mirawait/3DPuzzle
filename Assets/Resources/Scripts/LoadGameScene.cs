using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameScene : MonoBehaviour
{
    private bool isLoaded = false;
    public uint planetType = 0;
    private GameObject manager;
    private SolarSystem solarSystemManager;
    private GameObject clickedPlanet;
    uint subsctiptionID;
    bool isWaitingForPlanetClick = false;
    public void LoadScene()
    {
        GameObject Sun = GameObject.Find("Sun");
        if (isLoaded)
        {
            UnloadScene();
        }

        //Sun.GetComponent<SolarSystem>().EnableSolarSystemMoving(false);
        StartCoroutine(LoadSceneAsync());
        clickedPlanet.SetActive(false);
        isLoaded = true;
        
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        manager = GameObject.FindGameObjectWithTag("Manager");
        Debug.LogError("LOAD SCENE ASYNC PUZZLE TYPE " + planetType);
        manager.GetComponent<Manager>().Start_Puzzles((int)planetType, (int)UIManager.GetDifficulty());
    }

    public void UnloadScene()
    {
        if (isLoaded)
        {
            foreach (var obj in GameObject.FindGameObjectsWithTag("puzzle"))
            {
                Destroy(obj);
            }

            GameObject go = GameObject.Find("PuzzleHud");

            SceneManager.UnloadSceneAsync(gameObject.name);
            clickedPlanet.SetActive(true);
            isLoaded = false;
            
        }
    }

    public void startWaitingForPlanetClick()
    {
        if (!isWaitingForPlanetClick)
        {
            subsctiptionID = GesturesController.subscribeToPlanetClick(
                (GameObject target) =>
                {
                    Debug.LogError("Im inside load scene handler");
                    if (target.tag == "Planet")
                    {
                        planetType = target.GetComponent<PlanetScript>().GetIndex();
                        clickedPlanet = target;
                        Debug.LogError("NEW PLANET TYPE SETTED:" + planetType);
                    }
                });
            isWaitingForPlanetClick = true;
        }
    }
    public void stopWaitingForPlanetClick()
    {
        if (isWaitingForPlanetClick)
        {
            GesturesController.unsubscribeToPlanetClick(subsctiptionID);
            isWaitingForPlanetClick = false;
        }
    }
}
