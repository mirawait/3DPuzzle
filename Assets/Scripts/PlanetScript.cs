using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    public float selfRotationSpeed, solarRotationSpeed;
    public int planetIndex;
    public int distancingSpeed = 40;
    GameObject sun;
    bool solarRotationEnabled = false;


    Vector3 standartPosition;
    // Start is called before the first frame update
    void Start()
    {
        sun = GameObject.FindGameObjectWithTag("Sun");
        standartPosition = transform.position;
    }
    public void EnableSolarRotation(bool enable)
    {
        solarRotationEnabled = enable;
    }
    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, new Vector3(0, 1, 0), selfRotationSpeed * Time.deltaTime);
        if (solarRotationEnabled)
        {
            
            if (Vector3.Distance(transform.position, sun.transform.position) < planetIndex * 7.5)
            {
                transform.RotateAround(sun.transform.position, new Vector3(0, 1, 0), solarRotationSpeed);
                Vector3 direction = transform.position - sun.transform.position;
                direction.Normalize();
                Vector3 newPos = direction * planetIndex * 7.5f;
                transform.position = Vector3.MoveTowards(transform.position, newPos, distancingSpeed * Time.deltaTime);
            }
            else
            {
                transform.RotateAround(sun.transform.position, new Vector3(0, 1, 0), solarRotationSpeed * Time.deltaTime);
            }
            Vector3 targerPosY = transform.position;
            targerPosY.y = 0;
            if (Vector3.Distance(transform.position, targerPosY) != 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, targerPosY, Time.deltaTime * distancingSpeed);
            }
        }
        if (!solarRotationEnabled)
        {
            if (Vector3.Distance(transform.position, standartPosition) != 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, standartPosition, Time.deltaTime * distancingSpeed * 3);
            }
        }


    }
}
