using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    public static List<Square> GetPath(Square start, Square end)
    {
        List<Square> path = new List<Square>();
        path.Add(GetSquareTowardsEnd(start, end));

        int safety = 0;
        while (!path.Contains(end))
        {
            path.Add(GetSquareTowardsEnd(path[path.Count-1], end));

            safety++;
            if (safety > 1000)
            {
                Debug.LogWarning("Path not found");
                break;
            }
        }
        return path;
    }



    private static Square GetSquareTowardsEnd(Square origin, Square end)
    {
        Square closestSquare = null;
        float distanceToTarget = float.MaxValue;
        for (int i = 0; i < origin.surroundingSquares.Count; i++)
        {
            float testDistance = Vector3.Distance(origin.surroundingSquares[i].transform.position, end.transform.position);
            if (testDistance < distanceToTarget)
            {
                distanceToTarget = testDistance;
                closestSquare = origin.surroundingSquares[i];
            }
        }
        return closestSquare;
    }
}
