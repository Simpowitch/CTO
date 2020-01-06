using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    [SerializeField] CameraControls cameraControls = null;


    [SerializeField] Camera cam = null;
    [SerializeField] Transform cameraLookAt = null;
    Vector3 originalCameraPos;
    Vector3 originalLookAtPos;

    Weapon activeWeapon;
    float shootCooldown;
    float timeBetweenShots;
    public bool shootingModeActivated;

    [SerializeField] GameObject bulletBlueprint = null;
    [SerializeField] GameObject damageStatPopupBlueprint = null;


    public float shootingTime = 2f;

    int bulletsRemaining;

    public void ShowCharacterPerspective()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalCameraPos = cam.transform.position;
        originalLookAtPos = cameraLookAt.position;


        Vector3 lookPosition = CharacterManager.TargetCharacter != null ? CharacterManager.TargetCharacter.transform.position : CharacterManager.SelectedCharacter.transform.position + CharacterManager.SelectedCharacter.transform.forward * 1;
        GridSystem.instance.ChangeSquareVisualsAll(SquareVisualMode.Invisible);
        cameraControls.SetCameraPositions(lookPosition, CharacterManager.SelectedCharacter.transform.position, CameraMode.Character, false);
    }

    private void InitiateShoot()
    {
        //Display that we are in shooting mode

        shootingModeActivated = true;

        activeWeapon = CharacterManager.SelectedCharacter.weapon;
        timeBetweenShots = 60f / (float)activeWeapon.rpm;
        shootCooldown = 0;
        bulletsRemaining = activeWeapon.bulletsPerBurst;

        StartCoroutine(ExitShootModeInSeconds(shootingTime));
    }

    private void ExitShootMode()
    {
        shootingModeActivated = false;

        GridSystem.instance.ChangeSquareVisualsAll(SquareVisualMode.Default);
        cameraControls.SetCameraPositions(originalLookAtPos, originalCameraPos, CameraMode.TopDown, true);

        CharacterManager.SelectedCharacter = null;
        CharacterManager.TargetCharacter = null;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator ExitShootModeInSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        ExitShootMode();
    }

    
    private void Update()
    {
        if (cameraControls.cameraMode == CameraMode.Character)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                ExitShootMode();
                return;
            }
            if (shootingModeActivated && bulletsRemaining <= 0)
            {
                ExitShootMode();
                return;
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (!shootingModeActivated)
                {
                    InitiateShoot();
                }
                if (shootCooldown <= 0)
                {
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
        Vector3 shootingPosition = CharacterManager.SelectedCharacter.squareStandingOn.transform.position + new Vector3(0, CharacterManager.SelectedCharacter.weaponHeight, 0) + CharacterManager.SelectedCharacter.transform.forward * 0.5f;
        FireBullet(CharacterManager.SelectedCharacter.transform.position, Camera.main.transform.forward);

        shootCooldown = timeBetweenShots;
        //activeWeapon.bulletsRemaining--;
        bulletsRemaining--;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.GetComponent<Bodypart>())
            {
                hit.transform.GetComponent<Bodypart>().ReceiveDamage(activeWeapon.damage);
                Debug.Log("Target hit: " + hit.transform.name);

                GameObject instantiatedObject = Instantiate(damageStatPopupBlueprint);
                instantiatedObject.GetComponent<PopupStat>().WriteText("-" + CharacterManager.SelectedCharacter.weapon.damage.ToString());
                instantiatedObject.transform.position = hit.transform.position + new Vector3(0, hit.transform.lossyScale.y, 0);
            }
            else
            {
                Debug.Log("Did not hit a character");
            }
        }
        else
        {
            Debug.Log("Missed");
        }
    }

    private void FireBullet(Vector3 startPos, Vector3 direction)
    {
        GameObject spawnedBullet = Instantiate(bulletBlueprint);
        spawnedBullet.transform.position = startPos;
        spawnedBullet.GetComponent<Bullet>().Fire(direction);
    }
}
