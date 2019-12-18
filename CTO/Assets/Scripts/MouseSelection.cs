using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MouseSelection : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (TurnManager.instance.currentTurn != Team.Player)
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 roundedPos = GridSystem.instance.GetNearestPointOnGrid(hit.point);

                //If clicked on character
                if (hit.transform.GetComponent<Character>())
                {
                    Character mouseTargetCharacter = hit.transform.GetComponent<Character>();

                    if (mouseTargetCharacter.myTeam == Team.Player)
                    {
                        CharacterManager.SelectedCharacter = mouseTargetCharacter;
                    }
                    else if (mouseTargetCharacter.myTeam == Team.AI)
                    {
                        CharacterManager.SelectedCharacter = null;
                    }
                }
                else
                {
                    CharacterManager.SelectedCharacter = null;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 roundedPos = GridSystem.instance.GetNearestPointOnGrid(hit.point);

                //If clicked on square
                if (hit.transform.GetComponent<Square>())
                {
                    Square clickedOnSquare = hit.transform.GetComponent<Square>();

                    if (clickedOnSquare.occupiedSpace)
                    {

                    }
                    else
                    {
                        GridSystem.SelectedSquare = clickedOnSquare;
                        if (CharacterManager.SelectedCharacter)
                        {
                            CharacterManager.SelectedCharacter.TryToMove(hit.transform.GetComponent<Square>());
                        }
                    }
                }
                //If clicked on character
                else if (hit.transform.GetComponent<Character>())
                    {
                        Character mouseTargetCharacter = hit.transform.GetComponent<Character>();

                        if (mouseTargetCharacter.myTeam == Team.Player)
                        {

                        }
                        else if (mouseTargetCharacter.myTeam == Team.AI)
                        {
                            CharacterManager.TargetCharacter = mouseTargetCharacter;
                        }
                    }
            }
        }
    }
}
