using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSightManager : MonoBehaviour
{
    public Transform characterParent;

    List<GameObject> enemyCharacters = new List<GameObject>();
    List<GameObject> friendlyChracters = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < characterParent.childCount; i++)
        {
            if (characterParent.GetChild(i).GetComponent<Character>().myTeam == Team.AI)
            {
                enemyCharacters.Add(characterParent.GetChild(i).gameObject);
            }
            else
            {
                friendlyChracters.Add(characterParent.GetChild(i).gameObject);
            }
        }   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LineOfSight.GetLineOfSight(enemyCharacters, friendlyChracters);
        }
    }
}
