using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    public float selfRotationSpeed, solarRotationSpeed;
    public int distancingSpeed = 40;
    [SerializeField]
    private uint planetIndex;
    private bool solarRotationEnabled = false;
    private GameObject sun;
    private CameraScript mainCamera;
    private Vector3 standartPosition;

    private bool isMoving = true;
    // Start is called before the first frame update
    void Start()
    {
        sun = GameObject.FindGameObjectWithTag("Sun");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
        standartPosition = transform.position;
    }
    public uint GetIndex()
    {
        return planetIndex;
    }

    public void EnableMoving(bool enable)
    {
        isMoving = enable;
    }
    public void EnableSolarRotation(bool enable)
    {
        solarRotationEnabled = enable;
    }
    // Update is called once per frame
    void _Move()
    {
        transform.RotateAround(transform.position, new Vector3(0, 1, 0), selfRotationSpeed * Time.deltaTime);
        if (isMoving)
        {
            if (solarRotationEnabled)
            {
                if (Vector3.Distance(transform.position, sun.transform.position) < planetIndex * 7.5)
                {
                    transform.RotateAround(sun.transform.position, new Vector3(0, 1, 0), solarRotationSpeed * 0.5f);
                    Vector3 direction = transform.position - sun.transform.position;
                    direction.Normalize();
                    Vector3 newPos = direction * planetIndex * 7.5f;
                    transform.position =
                        Vector3.MoveTowards(transform.position, newPos, distancingSpeed * Time.deltaTime);
                }
                else
                {
                    transform.RotateAround(sun.transform.position, new Vector3(0, 1, 0),
                        solarRotationSpeed * Time.deltaTime);
                }

                Vector3 targerPosY = transform.position;
                targerPosY.y = 0;
                if (Vector3.Distance(transform.position, targerPosY) != 0)
                {
                    transform.position =
                        Vector3.MoveTowards(transform.position, targerPosY, Time.deltaTime * distancingSpeed);
                }
            }

            if (!solarRotationEnabled)
            {
                if (Vector3.Distance(transform.position, standartPosition) != 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, standartPosition,
                        Time.deltaTime * distancingSpeed * 3);
                }
            }
        }
    }
    void _HandleCameraHit()
    {
        //if (Vector3.Distance(mainCamera.transform.position, transform.position) < 3)
        //{
        //    var renderer = gameObject.GetComponent<Renderer>();
        //    renderer.material.shader = Shader.Find("Transparent/Diffuse");
        //    var color = renderer.material.color;
        //    color.a = 0.5f;
        //    renderer.material.color = color;
        //}
        //else
        //{
        //    var renderer = gameObject.GetComponent<Renderer>();
        //    renderer.material.shader = Shader.Find("Transparent/Diffuse");
        //    var color = renderer.material.color;
        //    color.a = 1f;
        //    renderer.material.color = color;
        //}
    }
    void Update()
    {
        _Move();
        _HandleCameraHit();
    }
}
