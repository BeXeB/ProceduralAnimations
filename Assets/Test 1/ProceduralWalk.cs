using UnityEngine;
using System.Collections;

public class ProceduralWalk : MonoBehaviour
{
    [SerializeField] Transform[] targets;
    [SerializeField] Transform[] targetsFixedToBody;
    [SerializeField] float distanceToMove = 5f;
    [SerializeField] float distanceToKeepFromGround = 0.3f;
    [SerializeField] float legSpeed = 2f;
    [SerializeField] LayerMask mask;

    Vector3[] targetLocationsOnGround;
    Vector3[] fixedTargetLocationsOnGround;

    bool changed = false;
    bool legMoving;
    bool legPairs; //true even, false odd
    bool[] finishedMoving; //ammount of legs finished moving

    private void Start()
    {
        finishedMoving = new bool[targets.Length / 2];
        targetLocationsOnGround = new Vector3[targets.Length];
        fixedTargetLocationsOnGround = new Vector3[targetsFixedToBody.Length];
        RaycastHit hit;
        for (int i = 0; i < targets.Length; i++)
        {
            Physics.Raycast(targets[i].position, -transform.up, out hit, Mathf.Infinity, mask);
            targetLocationsOnGround[i] = hit.point;
        }
    }

    void Update()
    {
        RaycastHit hit;
        for (int i = 0; i < targetsFixedToBody.Length; i++)
        {
            Physics.Raycast(targetsFixedToBody[i].position, -transform.up, out hit, Mathf.Infinity, mask);
            if (legMoving)
            {
                if (legPairs)
                {
                    if (i % 2 == 1)
                    {
                        fixedTargetLocationsOnGround[i] = hit.point;
                    }
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        fixedTargetLocationsOnGround[i] = hit.point;
                    }
                }
            }
            else
            {
                fixedTargetLocationsOnGround[i] = hit.point;
            }
        }
        for (int i = 0; i < targets.Length; i++)
        {
            //replace distance with sphere and add a nextstep target with a point on the sphere
            if (Vector3.Distance(targetLocationsOnGround[i], fixedTargetLocationsOnGround[i]) > distanceToMove)
            {
                if (!legMoving)
                {
                    if (i % 2 == 0)
                    {
                        legPairs = true;
                    }
                    else
                    {
                        legPairs = false;
                    }
                    StartCoroutine(MoveLegs());
                }
            }
            else
            {
                if (!legMoving)
                {
                    targets[i].position = targetLocationsOnGround[i];
                }
                else
                {
                    if (legPairs && i % 2 == 1)
                    {
                        targets[i].position = targetLocationsOnGround[i];
                    }
                    if (!legPairs && i % 2 == 0)
                    {
                        targets[i].position = targetLocationsOnGround[i];
                    }
                }
            }
        }
        if (changed)
        {
            ChangeRotation();
            ChangePosition();
            changed = false;
        }
    }

    void ChangeRotation()
    {
        //shit happens here
        //when the 2 pairs are not in an X shape
        Vector3 vector1 = fixedTargetLocationsOnGround[2] - fixedTargetLocationsOnGround[0];
        Vector3 vector2 = fixedTargetLocationsOnGround[3] - fixedTargetLocationsOnGround[1];
        Vector3 normalizedCrossProduct = Vector3.Cross(vector1, vector2).normalized;
        Vector3 rotatedCrossProduct = Quaternion.AngleAxis(90, transform.right) * normalizedCrossProduct;
        transform.rotation = Quaternion.LookRotation(rotatedCrossProduct, normalizedCrossProduct);
    }

    void ChangePosition()
    {
        Vector3 averageOfTargetPositions = Vector3.zero;
        foreach (Vector3 targetPosition in fixedTargetLocationsOnGround)
        {
            averageOfTargetPositions += targetPosition;
        }
        averageOfTargetPositions /= fixedTargetLocationsOnGround.Length;
        transform.position = new Vector3(transform.position.x, averageOfTargetPositions.y + distanceToKeepFromGround, transform.position.z);
    }

    IEnumerator MoveLegs()
    {
        legMoving = true;
        for (int i = 0; i < finishedMoving.Length; i++)
        {
            finishedMoving[i] = false;
        }
        bool allFinished = false;
        while (!allFinished)
        {
            for (int i = 0; i < fixedTargetLocationsOnGround.Length; i++)
            {
                if (legPairs)
                {
                    if (i % 2 == 0)
                    {
                        MoveLeg(i);
                    }
                }
                else
                {
                    if (i % 2 == 1)
                    {
                        MoveLeg(i);
                    }
                }
            }
            foreach (bool b in finishedMoving)
            {
                allFinished = true;
                allFinished = b && allFinished;
            }
            yield return null;
        }
        changed = true;
        legMoving = false;
    }

    void MoveLeg(int i)
    {
        Vector3 movementVector = Vector3.MoveTowards(targets[i].position, fixedTargetLocationsOnGround[i], Time.deltaTime * legSpeed);
        targets[i].position = movementVector;
        if (targets[i].position == fixedTargetLocationsOnGround[i] && !finishedMoving[i / 2])
        {
            targetLocationsOnGround[i] = targets[i].position;
            finishedMoving[i / 2] = true;
        }
    }
}
