using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LineOfSight
{

    static public float visionRange;

    public static void GetLineOfSight(List<GameObject> enemyCharacters, List<GameObject> friendlyChracters)
    {
        foreach (GameObject friendly in friendlyChracters)
        {
            //
            //Check if character is alive
            //
            foreach (GameObject enemy in enemyCharacters)
            {
                //
                //Check if character is alive
                //
                Ray lineOfSight = new Ray(friendly.transform.position, enemy.transform.position);
                RaycastHit hit;

                if (Physics.Raycast(lineOfSight, out hit, visionRange) && hit.collider.GetComponent<Character>().myTeam == Team.AI)
                {
                        enemy.transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    enemy.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }
}
