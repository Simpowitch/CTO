using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LineOfSight
{
    static List<Character> enemyCharacters = new List<Character>();
    static List<Character> friendlyChracters = new List<Character>();
    static public float visionRange;

    private static void Start()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");

        foreach (GameObject character in characters)
        {
            if (character.GetComponent<Character>().myTeam == Team.AI)
            {
                enemyCharacters.Add(character.GetComponent<Character>());
            }
            else
            {
                friendlyChracters.Add(character.GetComponent<Character>());
            }
        }
    }

    public static void GetLineOfSight()
    {
        foreach (Character friendly in friendlyChracters)
        {
            //
            //Check if character is alive
            //
            foreach (Character enemy in enemyCharacters)
            {
                //
                //Check if character is alive
                //
                Ray lineOfSight = new Ray(friendly.transform.position, enemy.transform.position - friendly.transform.position);
                RaycastHit hit;

                if (Physics.Raycast(lineOfSight, out hit, visionRange))
                {
                    if (hit.collider.GetComponent<Character>().myTeam != Team.AI)
                    {
                        
                    }
                    else
                    {
                        
                    }
                }
            }
        }
    }
}
