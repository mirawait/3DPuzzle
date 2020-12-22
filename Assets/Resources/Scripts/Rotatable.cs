using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatable : MonoBehaviour
{
    public void Permit()
    {
        isPermited = true;
    }

    public void Forbid()
    {
        isPermited = false;
    }

    void Start()
    {
    }

    void Update()
    {
        HandleInput();
    }

    private const float rotationSpeed = 3.0f;

    private bool isPermited = false;

    void HandleInput()
    {
        if (isPermited)
        {
#if UNITY_STANDALONE
            if (Input.GetMouseButton(0))
            {
                int rotationDirX = 0;
                int rotationDirY = 0;

                if (Input.GetAxis("Mouse X") < 0)
                    rotationDirY = 1;
                else if (Input.GetAxis("Mouse X") > 0)
                    rotationDirY = -1;

                if (Input.GetAxis("Mouse Y") < 0)
                    rotationDirX = -1;
                else if (Input.GetAxis("Mouse Y") > 0)
                    rotationDirX = 1;

                Rotate(rotationDirX, rotationDirY);
            }
#endif

#if UNITY_ANDROID
            int rotationDirX = 0;
            int rotationDirY = 0;

            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                Vector2 deltaPos = touch.deltaPosition;
                Vector3 moveDirection = new Vector3(0, 0, 0);

                if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y))
                {
                    if (deltaPos.x < 0)
                    {
                        rotationDirY = 1;
                    }
                    else if (deltaPos.x > 0)
                    {
                        rotationDirY = -1;
                    }
                }
                else
                {
                    if (deltaPos.y < 0)
                    {
                        rotationDirX = -1;
                    }
                    else if (deltaPos.y > 0)
                    {
                        rotationDirX = 1;
                    }
                }

                Rotate(rotationDirX, rotationDirY);
            }
#endif
        }
    }

    void Rotate(int dirX, int dirY)
    {
        transform.Rotate(dirX * rotationSpeed, dirY * rotationSpeed, 0, Space.World);
    }
}
