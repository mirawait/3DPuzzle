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

    private const float rotationSpeed = 3f;

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


            int rotationDirX = 0;
            int rotationDirY = 0;
            int rotationDirZ = 0;

            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                Vector2 deltaPos = touch.deltaPosition;
                Vector3 moveDirection = new Vector3(0, 0, 0);
                Vector3 axis = Vector3.zero;


                if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y))
                {
                    
                    if (deltaPos.x < 0)
                    {
                        axis += Camera.main.transform.up;
                        rotationDirY = 1;
                    }
                    else if (deltaPos.x > 0)
                    {
                        axis += Camera.main.transform.up * -1;
                        rotationDirY = -1;
                    }
                }
                if (Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.y))
                {
                    if (deltaPos.y < 0)
                    {
                        axis += Camera.main.transform.right * -1;
                        rotationDirX = -1;
                    }
                    else if (deltaPos.y > 0)
                    {
                        axis += Camera.main.transform.right;
                        rotationDirX = 1;
                    }
                }

                if (Mathf.Abs(Mathf.Abs(deltaPos.x) - Mathf.Abs(deltaPos.y)) < 5)
                {
                    if (deltaPos.x < 0)
                    {
                        axis += Camera.main.transform.up;
                        rotationDirY = 1;
                    }
                    else if (deltaPos.x > 0)
                    {
                        axis += Camera.main.transform.up * -1;
                        rotationDirY = -1;
                    }

                    if (deltaPos.y < 0)
                    {
                        axis += Camera.main.transform.right * -1;
                        rotationDirX = -1;
                    }
                    else if (deltaPos.y > 0)
                    {
                        axis += Camera.main.transform.right;
                        rotationDirX = 1;
                    }
                }
                Rotate(axis);

            }
            else if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                Vector2 deltaPos0 = touch0.deltaPosition;
                Vector2 deltaPos1 = touch1.deltaPosition;

                Vector3 moveDirection = new Vector3(0, 0, 0);
                Vector3 axis = Vector3.zero;

                if ((deltaPos0.y < 0) && (deltaPos1.y > 0))
                {
                    axis += Camera.main.transform.forward;
                }
                else if ((deltaPos0.y > 0) && (deltaPos1.y < 0))
                {
                    axis += Camera.main.transform.forward * -1;
                }
                Rotate(axis);
            }

        }
    }

    void Rotate(Vector3 axis)
    {
        transform.Rotate(axis * rotationSpeed, Space.World);
    }
}
