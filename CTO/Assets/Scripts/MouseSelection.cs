using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MouseSelection : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                Vector3 roundedPos = GridSystem.instance.GetNearestPointOnGrid(hit.point);
                Debug.Log(roundedPos);
                if (hit.transform.GetComponent<Character>())
                {
                    CharacterManager.SelectedCharacter = hit.transform.GetComponent<Character>();
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

                if (hit.transform.GetComponent<Square>())
                {
                    GridSystem.SelectedSquare = hit.transform.GetComponent<Square>();
                    if (CharacterManager.SelectedCharacter)
                    {
                        CharacterManager.SelectedCharacter.GetComponent<NavMeshAgent>().SetDestination(GridSystem.SelectedSquare.transform.position);
                    }
                }
            }
        }
    }
}
