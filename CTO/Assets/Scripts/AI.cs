using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public bool debugAIChoice = false;

    public List<Character> aiCharacters = new List<Character>();
    public List<Character> detectedEnemies = new List<Character>();
    public List<Character> enemiesInRange = new List<Character>();

    public int hitReward = 2;
    public int exposurePenalty = 1;

    public void StartTurn()
    {
        Debug.Log("AI Start turn");
        GridSystem.instance.ChangeSquareVisualsAll(SquareVisualMode.Invisible);


        for (int i = 0; i < aiCharacters.Count; i++)
        {
            aiCharacters[i].NewTurn();
            aiCharacters[i].CalculateValidMoves();
        }

        CalculatePoints();

        StartCoroutine(PerformActions());
    }

    IEnumerator PerformActions()
    {
        //Move characters
        for (int i = 0; i < aiCharacters.Count; i++)
        {
            if (debugAIChoice)
            {
                DebugChoice(aiCharacters[i], aiCharacters[i].aiSquareRanking[0].square);
                GridSystem.instance.ChangeSquareVisuals(aiCharacters[i].GetValidEndSquares(), SquareVisualMode.AIPointDebug);
                foreach (var squarePoint in aiCharacters[i].aiSquareRanking)
                {
                    squarePoint.square.transform.GetComponentInChildren<Text>().text = squarePoint.points.ToString();
                }
            }


            MoveCharacter(aiCharacters[i], aiCharacters[i].aiSquareRanking[0].square);

            NavMeshAgent agent = aiCharacters[i].GetComponent<NavMeshAgent>();
            bool characterDone = false;
            while (!characterDone)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        characterDone = true;
                    }
                }
                yield return new WaitForEndOfFrame();
            }

            Debug.Log("Character Done");
        }

        //TurnManager.instance.ChangeTurn();
        Debug.LogWarning("AI TURN NOT FINISHED SINCE IT IS COMMENTED OUT RIGHT NOW");
    }

    private void CalculatePoints()
    {
        Debug.Log("AI calculating squares");
        foreach (var character in aiCharacters)
        {
            character.aiSquareRanking.Clear();



            foreach (var square in character.GetValidEndSquares())
            {
                SquarePoint squarePoint = new SquarePoint(square);

                //Give points for enemy targets
                squarePoint.points += CalculateTargetPoints(character, square, out List<Line> targetHits);

                //Give points if position is behind a cover
                squarePoint.points -= CalculateExposure(character, square, out List<Line> exposureHits);

                //Add to list of squares
                character.aiSquareRanking.Add(squarePoint);
            }
            //Sort the squares and their points
            character.aiSquareRanking = character.aiSquareRanking.OrderBy(x => x.points).ToList();
            character.aiSquareRanking.Reverse();
        }
    }

    private int CalculateTargetPoints(Character character, Square square, out List<Line> lines)
    {
        lines = new List<Line>();


        int targetPoints = 0;

        foreach (var enemy in detectedEnemies)
        {
            //If out of range of character weapon
            if (character.weapon.range < Vector3.Distance(enemy.transform.position, square.transform.position))
            {
                continue;
            }

            Vector3 firePosition = square.transform.position + new Vector3(0, character.weaponHeight, 0);

            foreach (var sensor in enemy.GetBodySensors())
            {
                Ray ray = new Ray(firePosition, (enemy.squareStandingOn.transform.position + sensor) - firePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.GetComponent<Character>() == enemy)
                    {
                        targetPoints += hitReward;
                        lines.Add(new Line(ray.origin, hit.point));
                    }
                }
            }
        }

        return targetPoints;
    }

    private int CalculateExposure(Character character, Square square, out List<Line> lines)
    {
        lines = new List<Line>();

        int exposure = 0;

        foreach (var enemy in detectedEnemies)
        {
            //If out of range of enemy weapon
            if (enemy.weapon.range < Vector3.Distance(enemy.transform.position, square.transform.position))
            {
                continue;
            }

            Vector3 firePosition = enemy.squareStandingOn.transform.position + new Vector3(0, enemy.weaponHeight, 0);

            foreach (var sensor in character.GetBodySensors())
            {
                Ray ray = new Ray(square.transform.position + sensor, firePosition - (square.transform.position + sensor));

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.GetComponent<Character>() == enemy)
                    {
                        exposure += exposurePenalty;
                        lines.Add(new Line(ray.origin, hit.point));
                    }
                }
            }
        }
        return exposure;
    }

    //List<Ray> debugHitRays = new List<Ray>();
    //List<Ray> debugExposureRays = new List<Ray>();
    List<Line> debugHitLines = new List<Line>();
    List<Line> debugExposureLines = new List<Line>();


    private void DebugChoice(Character character, Square square)
    {
        int totPoint = 0;

        int targetPoints = CalculateTargetPoints(character, square, out List<Line> hitRays);
        debugHitLines = hitRays;
        totPoint += targetPoints;
        Debug.Log("Hit score: " + targetPoints);


        int exposure = CalculateExposure(character, square, out List<Line> exposureRays);
        debugExposureLines = exposureRays;
        totPoint -= exposure;
        Debug.Log("Exposure score:" + exposure);

        Debug.Log("Total points: " + totPoint);
    }


    private void MoveCharacter(Character character, Square square)
    {
        Debug.Log("AI moving character");
        character.TryToMove(square);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < debugHitLines.Count; i++)
        {
            Gizmos.DrawLine(debugHitLines[i].startPos, debugHitLines[i].endPos);
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < debugExposureLines.Count; i++)
        {
            Gizmos.DrawLine(debugExposureLines[i].startPos, debugExposureLines[i].endPos);
        }
    }
}

[System.Serializable]
public struct SquarePoint
{
    public Square square;
    public int points;

    public SquarePoint(Square square)
    {
        this.square = square;
        points = 0;
    }
}

public struct Line
{
    public Vector3 startPos;
    public Vector3 endPos;

    public Line(Vector3 startPos, Vector3 endPos)
    {
        this.startPos = startPos;
        this.endPos = endPos;
    }
}
