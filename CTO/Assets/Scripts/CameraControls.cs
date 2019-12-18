using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [SerializeField] float cameraMinHeight = 5f;
    [SerializeField] float cameraMaxHeight = 20f;

    [SerializeField] float camMoveSpeed = 50f;
    [SerializeField] float camRotationSpeed = 20f;
    [SerializeField] float camZoomSpeed = 20f;

    Transform cameraTransform;

    private void Start()
    {
        cameraTransform = transform.GetChild(0);
    }

    private void Update()
    {
        float deltaYParentRotation = 0;
        Vector3 deltaParentMove = Vector3.zero;

        float deltaCameraHeight = 0f;

        //Rotation
        if (Input.GetKey(KeyCode.Q))
        {
            deltaYParentRotation += camRotationSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            deltaYParentRotation -= camRotationSpeed;
        }

        //Move
        if (Input.GetKey(KeyCode.W))
        {
            deltaParentMove += transform.forward * camMoveSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            deltaParentMove += -transform.forward * camMoveSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            deltaParentMove += transform.right * camMoveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            deltaParentMove += -transform.right * camMoveSpeed;
        }

        //Zoom out
        if (Input.mouseScrollDelta.y < 0 || Input.GetKey(KeyCode.LeftControl))
        {
            deltaCameraHeight += camZoomSpeed;
            if (cameraTransform.localPosition.y > cameraMaxHeight)
            {
                deltaCameraHeight = 0;
            }
        }
        //Zoom In
        if (Input.mouseScrollDelta.y > 0 || Input.GetKey(KeyCode.LeftShift))
        {
            deltaCameraHeight -= camZoomSpeed;
            if (cameraTransform.localPosition.y < cameraMinHeight)
            {
                deltaCameraHeight = 0;
            }
        }

        transform.Rotate(new Vector3(0, deltaYParentRotation, 0) * Time.deltaTime);
        transform.position += deltaParentMove * Time.deltaTime;
        cameraTransform.position += (new Vector3(0, deltaCameraHeight, 0) * Time.deltaTime);
        cameraTransform.LookAt(this.transform);
    }
}
