using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    private GameObject playText, tapText, btmButton, fadeOutEffect;
    private SolarSystem solarSystem;
    private CameraScript mainCamera;
    private bool fadeOutComplete = false;
    enum UI_Phase
    {
        PlanetInfo,
        SolarSystem,
        Start
    }
    private UI_Phase currentPhase;
    // Start is called before the first frame update
    void Start()
    {
        playText = GameObject.Find("3dPuzzleText");
        tapText = GameObject.Find("TapToPlayText");
        //tapText.SetActive(false);
        btmButton = GameObject.Find("BackToMenuButton");
        fadeOutEffect = GameObject.Find("FadeOutEffect");
        btmButton.GetComponent<Button>().onClick.AddListener(BackTask);

        btmButton.SetActive(false);

        solarSystem = GameObject.FindGameObjectWithTag("Sun").GetComponent<SolarSystem>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();

        currentPhase = UI_Phase.Start;
        StartCoroutine(_FadeOut());
    }
    IEnumerator _FadeOut()
    {
        yield return new WaitForSeconds(2f);
        fadeOutEffect.GetComponent<Image>().CrossFadeAlpha(0, 2, false);
        yield return new WaitForSeconds(2f);
        fadeOutComplete = true;
        //tapText.SetActive(true);
        while (tapText.GetComponent<TextMeshProUGUI>().color.a != 255)
        {
            float currentA = tapText.GetComponent<TextMeshProUGUI>().color.a;
            tapText.GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, currentA + 0.1f);//CrossFadeAlpha(1, 1, false);
            yield return new WaitForSeconds(0.05f);
        }

        yield break;
    }
    void BackTask()
    {
        mainCamera.GoFree();
        currentPhase = UI_Phase.SolarSystem;
        btmButton.SetActive(false);
    }
    void _HandlePlanetClick()
    {
        if ((Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Began) && mainCamera.ReadyForLock())
        {
            Ray raycast = mainCamera.camera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit, Mathf.Infinity))
            {
                Debug.DrawRay(transform.position, raycastHit.point, Color.red, 5f);
                if (raycastHit.transform.gameObject.tag == "Planet")
                {
                    mainCamera.FocusOn(raycastHit.transform.gameObject);
                    currentPhase = UI_Phase.PlanetInfo;
                    btmButton.SetActive(true);
                }
            }
        }
    }
    void _HandleStartClick()
    {
        if (Input.touchCount > 0 && fadeOutComplete)
        {
            currentPhase = UI_Phase.SolarSystem;
            mainCamera.GoFree();
            playText.SetActive(false);
            tapText.SetActive(false);
            //btmButton.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentPhase)
        {
            case UI_Phase.Start:
                _HandleStartClick();
                break;
            case UI_Phase.SolarSystem:
                _HandlePlanetClick();
                break;
        }
    }
}
