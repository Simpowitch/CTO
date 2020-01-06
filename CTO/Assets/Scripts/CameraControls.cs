using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode {LockedInAnimation, TopDown, Character }
public class CameraControls : MonoBehaviour
{
    public CameraMode cameraMode = CameraMode.TopDown;
    Transform cameraTransform;

    [SerializeField] float cameraMinHeight = 5f;
    [SerializeField] float cameraMaxHeight = 20f;

    [SerializeField] float camMoveSpeed = 50f;
    [SerializeField] float camRotationSpeed = 20f;
    [SerializeField] float camZoomSpeed = 20f;

    public float fpsSensitivityX = 3f;
    public float fpsSensitivityY = 3f;

    public float fpsMinimumX = -360F;
    public float fpsMaximumX = 360F;

    public float fpsMinimumY = -60F;
    public float fpsMaximumY = 60F;

    float fpsRotationY = 0F;

    private void Start()
    {
        cameraTransform = transform.GetChild(0);
        cameraMode = CameraMode.TopDown;
    }


    private void Update()
    {
        switch (cameraMode)
        {
            case CameraMode.LockedInAnimation:
                //Do nothing
                break;
            case CameraMode.TopDown:
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
                break;
            case CameraMode.Character:
                float rotationX = cameraTransform.localEulerAngles.y + Input.GetAxis("Mouse X") * fpsSensitivityX;

                fpsRotationY += Input.GetAxis("Mouse Y") * fpsSensitivityY;
                fpsRotationY = Mathf.Clamp(fpsRotationY, fpsMinimumY, fpsMaximumY);

                cameraTransform.localEulerAngles = new Vector3(-fpsRotationY, rotationX, 0);

                CharacterManager.SelectedCharacter.transform.LookAt(cameraTransform.transform.position + cameraTransform.transform.forward);
                break;
        }
    }


    int changes = 0;
    public void SetCameraPositions(Vector3 newLookAtPosition, Vector3 newCameraPos, CameraMode newMode, bool changeModeImmidiately)
    {
        cameraMode = CameraMode.LockedInAnimation;
        StartCoroutine(Move(this.transform, newLookAtPosition));
        StartCoroutine(Move(cameraTransform, newCameraPos));
        if (changeModeImmidiately)
        {
            cameraMode = newMode;
        }
        else
        {
            StartCoroutine(ChangeCameraModeWhenDone(newMode));
        }
    }

    IEnumerator ChangeCameraModeWhenDone(CameraMode newMode)
    {
        while (changes > 0)
        {
            yield return new WaitForEndOfFrame();
        }
        cameraMode = newMode;
    }

    float cameraMoveSpeed = 10;
    IEnumerator Move(Transform transformToMove, Vector3 endPos)
    {
        changes++;
        // Camera start position
        Vector3 startPos = transformToMove.position;

        // Keep a note of the time the movement started.
        float startTime = Time.time;

        // Calculate the journey length.
        float journeyLength = Vector3.Distance(startPos, endPos);

        float fractionOfJourney = 0;

        if (journeyLength > 0)
        {
            while (fractionOfJourney < 1)
            {
                // Distance moved equals elapsed time times speed..
                float distCovered = (Time.time - startTime) * cameraMoveSpeed;

                // Fraction of journey completed equals current distance divided by total distance.
                fractionOfJourney = distCovered / journeyLength;

                // Set our position as a fraction of the distance between the markers.
                transformToMove.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);
                yield return new WaitForEndOfFrame();
            }
        }
        changes--;
    }
}
