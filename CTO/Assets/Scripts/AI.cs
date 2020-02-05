using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public List<Character> aiCharacters = new List<Character>();
    public List<Character> detectedEnemies = new List<Character>();

    public int hitReward = 2;
    public int exposurePenalty = 1;
    public int pointsPerCover = 5;
    public float bulletControl = 0; //from 0 to 1;


    private bool coroutineStop = true;

    [SerializeField] GameObject bulletBlueprint = null;
    [SerializeField] GameObject damageStatPopupBlueprint = null;

    private void Start()
    {
        if (bulletControl > 1 || bulletControl < 0)
        {
            Debug.LogError("Bullet control of AI not set to a correct number, needs to be within 0 to 1");
        }
    }


    public void StartTurn()
    {
        Debug.Log("AI Start turn");
        GridSystem.instance.ChangeSquareVisualsAll(SquareVisualMode.Invisible);


        for (int i = 0; i < aiCharacters.Count; i++)
        {
            aiCharacters[i].NewTurn();
        }

        StartCoroutine(PerformActions());
    }

    IEnumerator PerformActions()
    {
        for (int i = 0; i < aiCharacters.Count; i++)
        {
            for (int j = 0; j < detectedEnemies.Count; j++)
            {
                if (detectedEnemies[j] == null)
                {
                    detectedEnemies.RemoveAt(j);
                    j--;
                }
            }


            //Move character to best square
            aiCharacters[i].CalculateValidMoves();
            Square bestSquare = GetBestSquare(aiCharacters[i]);
            DebugSquareChoice(aiCharacters[i], bestSquare); //Send data to the character about the choice
            MoveCharacter(aiCharacters[i], bestSquare);

            //Wait until character has moved
            NavMeshAgent agent = aiCharacters[i].GetComponent<NavMeshAgent>();
            coroutineStop = true;
            while (coroutineStop)
            {
                yield return new WaitForSeconds(0.5f);
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        coroutineStop = false;
                    }
                }
                yield return new WaitForEndOfFrame();
            }

            //Shoot the best target
            Character target = GetBestTarget(aiCharacters[i]);
            if (target != null)
            {
                StartCoroutine(Shoot(aiCharacters[i], target));

                coroutineStop = true;
                while (coroutineStop)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            Debug.Log("Character Done");
        }

        //TurnManager.instance.ChangeTurn();
        Debug.LogWarning("AI TURN NOT FINISHED SINCE IT IS COMMENTED OUT RIGHT NOW");
    }

    private Square GetBestSquare(Character character)
    {
        Debug.Log("AI calculating squares and returning the best one");
        character.aiSquareRanking.Clear();

        foreach (var square in character.GetValidEndSquares())
        {
            SquarePoint squarePoint = new SquarePoint(square);

            //Give points for enemy targets
            int targetPoints = CalculateTargetPoints(character, square, out List<Line> targetHits);

            //Maximum points = bullets that the character can fire
            targetPoints = Mathf.Min(character.weapon.bulletsPerBurst, targetPoints) * hitReward;
            squarePoint.points += targetPoints;

            //Take away points per possible hit by enemies
            int exposurePoints = CalculateExposure(character, square, out List<Line> exposureHits);
            squarePoint.points -= exposurePoints;

            //Take away points if too close to an enemy and if too far from enemies
            int distancePoints = CalculateDistanceScores(character, square);
            squarePoint.points += distancePoints;

            //Give points for cover
            int coverPoints = 0;
            foreach (var item in square.objects)
            {
                if (item != null)
                {
                    coverPoints += pointsPerCover;
                }
            }
            foreach (var item in square.surroundingSquares)
            {
                if (item != null && item.objects[0] != null)
                {
                    coverPoints += pointsPerCover;
                }
            }
            squarePoint.points += coverPoints;

            //Add to list of squares
            character.aiSquareRanking.Add(squarePoint);
        }
        //Sort the squares and their points
        character.aiSquareRanking = character.aiSquareRanking.OrderBy(x => x.points).ToList();
        character.aiSquareRanking.Reverse();
        return character.aiSquareRanking[0].square;
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
                Ray ray = new Ray(firePosition, sensor.position - firePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.GetComponent<Bodypart>() && hit.transform.GetComponent<Bodypart>().GetCharacter() == enemy)
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
                Vector3 sensorFuturePosition = sensor.position + (square.transform.position - character.squareStandingOn.transform.position);
                Ray ray = new Ray(sensorFuturePosition, firePosition - sensorFuturePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.GetComponent<Bodypart>() && hit.transform.GetComponent<Bodypart>().GetCharacter() == enemy)
                    {
                        exposure += exposurePenalty;
                        lines.Add(new Line(ray.origin, hit.point));
                    }
                }
            }
        }
        return exposure;
    }

    private int CalculateDistanceScores(Character character, Square square)
    {
        int pointReductionFromDistanceErrors = 10;
        float preferedMinDistance = 2f;
        float preferedMaxDistance = character.weapon.range;

        int points = 0;

        foreach (var enemy in detectedEnemies)
        {
            float distance = Vector3.Distance(square.transform.position, enemy.transform.position);
            if (distance < preferedMinDistance)
            {
                points -= pointReductionFromDistanceErrors;
            }
            if (distance > preferedMaxDistance)
            {
                points -= pointReductionFromDistanceErrors;
            }
        }
        return points;
    }

    private void DebugSquareChoice(Character character, Square square)
    {
        int targetPoints = CalculateTargetPoints(character, square, out List<Line> hitRays);
        character.debugHitOppertunities = hitRays;

        int exposure = CalculateExposure(character, square, out List<Line> exposureRays);
        character.debugExposureLines = exposureRays;
    }

    private Character GetBestTarget(Character character)
    {
        Character target = null;
        int bestPoint = int.MinValue;
        foreach (var enemy in detectedEnemies)
        {
            int points = 0;

            //If out of range of character weapon
            if (character.weapon.range < Vector3.Distance(enemy.transform.position, character.squareStandingOn.transform.position))
            {
                continue;
            }

            points -= Mathf.RoundToInt(Vector3.Distance(enemy.transform.position, character.squareStandingOn.transform.position));

            Vector3 firePosition = character.squareStandingOn.transform.position + new Vector3(0, character.weaponHeight, 0);

            foreach (var sensor in enemy.GetBodySensors())
            {
                Ray ray = new Ray(firePosition, sensor.position - firePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.GetComponent<Bodypart>().GetCharacter() == enemy)
                    {
                        points++;
                    }
                }
            }

            if (points > bestPoint)
            {
                bestPoint = points;
                target = enemy;
            }
        }

        character.debugBestTarget = new Line(character.transform.position, target.transform.position); //Send debugline to character
        return target;
    }

    IEnumerator Shoot(Character myCharacter, Character target)
    {
        Debug.Log(myCharacter.name + " is shooting at: " + target.name);

        //play character animation
        myCharacter.transform.LookAt(target.transform);

        //camera focus on this character and on target (zoom to fit both)


        Vector3 shootingPosition = myCharacter.squareStandingOn.transform.position + new Vector3(0, myCharacter.weaponHeight, 0) + myCharacter.transform.forward * 0.5f;
        float timeBetweenShots = 60f / (float)myCharacter.weapon.rpm;


        float maxSway = 1 - bulletControl;

        float xOffset = Random.Range(-maxSway, maxSway);
        float yOffset = Random.Range(-maxSway, maxSway);
        float zOffset = Random.Range(-maxSway, maxSway);
        Vector3 aPoint = target.transform.position + new Vector3(xOffset, yOffset, zOffset);

        xOffset = Random.Range(-maxSway, maxSway);
        yOffset = Random.Range(-maxSway, maxSway);
        zOffset = Random.Range(-maxSway, maxSway);
        Vector3 bPoint = target.transform.position + new Vector3(xOffset, yOffset, zOffset);

        for (int i = 0; i < myCharacter.weapon.bulletsPerBurst; i++)
        {
            //Play audio

            float bulletPercentageFired = i / myCharacter.weapon.bulletsPerBurst - 1;

            Vector3 aimPosition = Vector3.Lerp(aPoint, bPoint, bulletPercentageFired);

            RaycastHit hit;

            FireBullet(shootingPosition, aimPosition - shootingPosition);

            if (Physics.Raycast(shootingPosition, aimPosition - shootingPosition, out hit))
            {
                if (hit.transform.GetComponent<Bodypart>())
                {
                    Debug.DrawRay(shootingPosition, aimPosition - shootingPosition, Color.green);
                    hit.transform.GetComponent<Bodypart>().ReceiveDamage(myCharacter.weapon.damage);
                    Debug.Log("Target hit: " + hit.transform.name);
                }
                else
                {
                    Debug.DrawRay(shootingPosition, aimPosition - shootingPosition, Color.red);
                    Debug.Log("Did not hit a character");
                }
            }
            else
            {
                Debug.Log("Miss");
            }
            yield return new WaitForSeconds(timeBetweenShots * 1.5f);
        }
        coroutineStop = false;
    }

    private void FireBullet(Vector3 startPos, Vector3 direction)
    {
        GameObject spawnedBullet = Instantiate(bulletBlueprint);
        spawnedBullet.transform.position = startPos;
        spawnedBullet.GetComponent<Bullet>().Fire(direction);
    }

    private void MoveCharacter(Character character, Square square)
    {
        Debug.Log("AI moving character");
        character.TryToMove(square);
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
