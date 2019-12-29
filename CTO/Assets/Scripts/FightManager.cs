using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public static bool shootingModeActivated;

    [SerializeField] Camera cam = null;
    Vector3 originalPos;
    Quaternion originalRotation;

    public float shootingTime = 2f;

    int bulletsRemaining;

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

        GridSystem.instance.ChangeSquareVisualsAll(SquareVisualMode.Invisible);


        shootingModeActivated = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam.transform.position = endPos;
        cam.transform.LookAt(target);

        activeWeapon = CharacterManager.SelectedCharacter.weapon;
        timeBetweenShots = 60f / (float)activeWeapon.rpm;
        shootCooldown = 0;
        bulletsRemaining = activeWeapon.bulletsPerBurst;

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

        CharacterManager.SelectedCharacter = null;
        CharacterManager.TargetCharacter = null;

        GridSystem.instance.ChangeSquareVisualsAll(SquareVisualMode.Default);
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

    Weapon activeWeapon;
    float shootCooldown;
    float timeBetweenShots;

    private void Update()
    {
        if (shootingModeActivated)
        {
            float rotationX = cam.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            cam.transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (shootCooldown <= 0)
                {
                    //if (activeWeapon.bulletsRemaining > 0)
                    //{
                    //Shoot();
                    //}
                    //else
                    //{
                    //    Debug.Log("No more bullets");
                    //}
                    if (bulletsRemaining > 0)
                    {
                        Shoot();
                    }
                }
                else
                {
                    shootCooldown -= Time.deltaTime;
                }
            }
        }
    }

    private void Shoot()
    {
        shootCooldown = timeBetweenShots;
        //activeWeapon.bulletsRemaining--;
        bulletsRemaining--;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.GetComponent<Character>())
            {
                hit.transform.GetComponent<Character>().TakeDamage(activeWeapon.damage);
                Debug.Log("Target hit: " + hit.transform.name);
            }
            else
            {
                Debug.Log("Missed");
            }
        }
        else
        {
            Debug.Log("Missed");
        }
    }
}
