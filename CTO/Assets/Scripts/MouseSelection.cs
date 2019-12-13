using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelection : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (TurnManager.instance.currentTurn != Team.Player)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                Vector3 roundedPos = GridSystem.instance.GetNearestPointOnGrid(hit.point);
                Debug.Log(roundedPos);
                if (hit.transform.GetComponent<Character>() && hit.transform.GetComponent<Character>().myTeam == Team.Player)
                {
                    CharacterManager.SelectedCharacter = hit.transform.GetComponent<Character>();
                    Debug.Log("Character selected:" + CharacterManager.SelectedCharacter.gameObject.name);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                Vector3 roundedPos = GridSystem.instance.GetNearestPointOnGrid(hit.point);
                Debug.Log(roundedPos);

                if (hit.transform.GetComponent<Square>() && !hit.transform.GetComponent<Square>().occupiedSpace)
                {
                    GridSystem.SelectedSquare = hit.transform.GetComponent<Square>();

                    Debug.Log("Distance between character and target is: " + GridSystem.SelectedSquare.path.Count);
                    if (CharacterManager.SelectedCharacter)
                    {
                        CharacterManager.SelectedCharacter.TryToMove(hit.transform.GetComponent<Square>());
                    }
                }
            }
        }
    }
}
