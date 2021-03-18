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
    bool legPairs; //true = pair even, false = pair odd

    private void Start()
    {

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
            //replace distance with sphere and fixedtargetlocation with a point on the sphere
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
                targets[i].position = targetLocationsOnGround[i];
            }
        }
        if (changed)
        {
            Vector3 vector1 = targetLocationsOnGround[2] - targetLocationsOnGround[0];
            Vector3 vector2 = targetLocationsOnGround[3] - targetLocationsOnGround[1];
            Vector3 normalizedCrossProduct = Vector3.Cross(vector1, vector2).normalized;
            Vector3 rotatedCrossProduct = Quaternion.AngleAxis(90, transform.right) * normalizedCrossProduct;
            transform.rotation = Quaternion.LookRotation(rotatedCrossProduct, normalizedCrossProduct);
            Vector3 averageOfTargetPositions = Vector3.zero;
            foreach (Vector3 targetPosition in targetLocationsOnGround)
            {
                averageOfTargetPositions += targetPosition;
            }
            averageOfTargetPositions /= targetLocationsOnGround.Length;
            transform.position = new Vector3(transform.position.x, averageOfTargetPositions.y + distanceToKeepFromGround, transform.position.z);
            changed = false;
        }
    }

    IEnumerator MoveLegs()
    {
        legMoving = true;
        bool finishedMoving = false;
        while (!finishedMoving)
        {
            for (int i = 0; i < fixedTargetLocationsOnGround.Length; i++)
            {
                if (!legPairs)
                {
                    if (i % 2 == 1)
                    {
                        targets[i].position = Vector3.MoveTowards(targets[i].position, fixedTargetLocationsOnGround[i], Time.deltaTime * legSpeed);
                        targetLocationsOnGround[i] = targets[i].position;
                        if (targets[i].position == fixedTargetLocationsOnGround[i])
                        {
                            finishedMoving = true;
                        }
                    }
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        targets[i].position = Vector3.MoveTowards(targets[i].position, fixedTargetLocationsOnGround[i], Time.deltaTime * legSpeed);
                        targetLocationsOnGround[i] = targets[i].position;
                        if (targets[i].position == fixedTargetLocationsOnGround[i])
                        {
                            finishedMoving = true;
                        }
                    }
                }
            }
            yield return null;
        }
        changed = true;
        legMoving = false;
    }
}
