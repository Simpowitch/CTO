using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{


    bool shootingModeActivated;

    [SerializeField] Camera cam = null;
    Vector3 originalPos;
    Quaternion originalRotation;

    public float shootingTime = 2f;

    //public int camSpeed = 5;

    public void InitiateShoot()
    {
        originalPos = cam.transform.position;
        originalRotation = cam.transform.rotation;

        StartCoroutine(EnterShootMode(CharacterManager.SelectedCharacter.transform.position, CharacterManager.TargetCharacter.transform.position, shootingTime));
    }

    float lerpSpeed;
    IEnumerator EnterShootMode(Vector3 endPos, Vector3 target, float time)
    {
        //Do Animation

        shootingModeActivated = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam.transform.position = endPos;
        cam.transform.LookAt(target);
        yield return new WaitForSeconds(time);
        StartCoroutine(ExitShootMode());
    }

    IEnumerator ExitShootMode()
    {
        cam.transform.position = originalPos;
        cam.transform.rotation = originalRotation;
        shootingModeActivated = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        yield return null;
        //do animation
    }


    public float sensitivityX = 3f;
    public float sensitivityY = 3f;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;

    private void Update()
    {
        if (shootingModeActivated)
        {
            float rotationX = cam.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            cam.transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
    }
}
