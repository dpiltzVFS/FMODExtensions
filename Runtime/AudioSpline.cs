// This script was created by dpiltz@vfs.com and heavily plagerizes the stuff created by the guy who made these videos - https://www.youtube.com/watch?v=IGmkDpNSpB8&list=TLPQMDIwOTIwMjKWemA7Mug_cw&index=1 https://www.youtube.com/watch?v=PPqtdpE4GM0&t=8s 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpline : MonoBehaviour
{
    private Vector3[] splinePoint;
    private int splineCount;

    public bool debug_drawspline = true;

    private void Start()
    {
        splineCount = transform.childCount;
        splinePoint = new Vector3[splineCount];

        for (int i = 0; i < splineCount; i++)
        {
            splinePoint[i] = transform.GetChild(i).position;
        }

    }

    private void Update()
    {
        if(splineCount > 1 && debug_drawspline)
        {
            for (int i = 0; i < splineCount - 1; i++)
            {
                Debug.DrawLine(splinePoint[i], splinePoint[i + 1], Color.green);
            }
        }

    }

    public Vector3 WhereOnSpline (Vector3 pos)
    {
        int closestSplinePoint = GetClosestSplinePoint(pos);

        if (closestSplinePoint == 0)
        {
            return SplineSegment(splinePoint[0], splinePoint[1], pos);
        }

        else if (closestSplinePoint == splineCount - 1)
        {
            return SplineSegment(splinePoint[splineCount - 1], splinePoint[splineCount - 2], pos);
        }

        else
        {
            Vector3 leftSeg = SplineSegment(splinePoint[closestSplinePoint - 1], splinePoint[closestSplinePoint], pos);
            Vector3 rightSeg = SplineSegment(splinePoint[closestSplinePoint + 1], splinePoint[closestSplinePoint], pos);

            if ((pos - leftSeg).sqrMagnitude <= (pos - rightSeg).sqrMagnitude)
            {
                return leftSeg;
            }
            else
            {
                return rightSeg;
            }
        }

    }

    private int GetClosestSplinePoint(Vector3 pos)
    {
        int closestPoint = -1;
        float shortestDistance = 0.0f;

        for (int i = 0; i < splineCount; i++)
        {
            float sqrDistance = (splinePoint[i] - pos).sqrMagnitude;
            if (shortestDistance == 0.0f || sqrDistance < shortestDistance)
            {
                shortestDistance = sqrDistance;
                closestPoint = i;
            }
        }
        return closestPoint;
    }

    public Vector3 SplineSegment(Vector3 v1, Vector3 v2, Vector3 pos)
    {
        Vector3 v1ToPos = pos - v1;
        Vector3 segDirection = (v2 - v1).normalized;

        float distanceFromV1 = Vector3.Dot(segDirection, v1ToPos);

        if (distanceFromV1 < 0.0f)
        {
            return v1;
        }

        else if (distanceFromV1 * distanceFromV1 > (v2 - v1).sqrMagnitude)
        {
            return v2;
        }

        else
        {
            Vector3 fromV1 = segDirection * distanceFromV1;
            return v1 + fromV1;
        }
    }
}
