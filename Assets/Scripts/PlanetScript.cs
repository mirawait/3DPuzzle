using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlanetScript : MonoBehaviour
{
    public float selfRotationSpeed, solarRotationSpeed;
    public int planetIndex;
    bool solarRotationEnabled = false;
    int distancingSpeed = 10;
    GameObject sun;
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
        transform.RotateAround(transform.position, new Vector3(0, 1, 0), selfRotationSpeed);
        if (solarRotationEnabled)
        {
            transform.RotateAround(sun.transform.position, new Vector3(0, 1, 0), solarRotationSpeed);
            if (Vector3.Distance(transform.position, sun.transform.position) <= planetIndex * 7.5)
            {
                Vector3 direction = transform.position - sun.transform.position;
                direction.Normalize();
                Vector3 newPos = transform.position + direction * distancingSpeed;
                transform.position = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime * distancingSpeed);
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
