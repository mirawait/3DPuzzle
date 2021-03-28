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
                    axis = Vector3.left;
                    if (deltaPos.x < 0)
                    {
                        rotationDirY = 1;
                    }
                    else if (deltaPos.x > 0)
                    {
                        rotationDirY = -1;
                    }
                }
                if (Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.y))
                {
                    axis = Vector3.left;
                    if (deltaPos.y < 0)
                    {
                        rotationDirX = -1;
                    }
                    else if (deltaPos.y > 0)
                    {
                        rotationDirX = 1;
                    }
                }

                if (Mathf.Abs(deltaPos.x) == Mathf.Abs(deltaPos.y))
                {
                    axis = Vector3.left;
                    if (deltaPos.y < 0)
                    {
                        rotationDirY = 1;
                        rotationDirX = -1;
                    }
                    else if (deltaPos.y > 0)
                    {
                        rotationDirY = -1;
                        rotationDirX = 1;
                    }
                }
                Rotate(rotationDirX, rotationDirY, rotationDirZ);

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
                        rotationDirZ = 1;
                    }
                    else if ((deltaPos0.y > 0) && (deltaPos1.y < 0))
                    {
                        rotationDirZ = -1;
                    }
                

                
                Rotate(rotationDirX, rotationDirY, rotationDirZ);
            }

        }
    }

    void Rotate(int dirX, int dirY, int dirZ)
    {
        transform.Rotate(dirX * rotationSpeed, dirY * rotationSpeed, rotationSpeed * dirZ, Space.World);

        //transform.Rotate(0f, dirY * rotationSpeed, 0f, Space.World);
       // transform.Rotate(dirX * rotationSpeed, 0f, 0f, Space.World);

        //transform.Rotate(-Vector3.left, rotationSpeed * dirX);
        //transform.Rotate(0f,0f , rotationSpeed * dirZ, Space.World);


    }
}
