using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameScene : MonoBehaviour
{
    private bool isLoaded = false;
    private uint planetType = 0;
    private GameObject manager;
    private SolarSystem solarSystemManager;
    private GameObject clickedPlanet;
    public void LoadScene()
    {
        GameObject Sun = GameObject.Find("Sun");
        DificultySwitcherScript.GetChosenDifficulty();
        if (!isLoaded)
        {
            //Sun.GetComponent<SolarSystem>().EnableSolarSystemMoving(false);
            StartCoroutine(LoadSceneAsync());
            clickedPlanet.SetActive(false);
            isLoaded = true;
        }
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        manager = GameObject.FindGameObjectWithTag("Manager");
        manager.GetComponent<Manager>().Start_Puzzles((int)planetType, (int)DificultySwitcherScript.GetChosenDifficulty());
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

    void Start()
    {
        uint planetClickSubscription = SolarSystemController.subscribeToPlanetClick(
            (GameObject target) =>
            {
                if (target.tag == "Planet")
                {
                    planetType = target.GetComponent<PlanetScript>().GetIndex();
                    clickedPlanet = target;
                }
            });
    }
}
