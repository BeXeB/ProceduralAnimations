using UnityEngine;

public class ProceduralWalk : MonoBehaviour
{
    [SerializeField] Transform[] targets;
    [SerializeField] Transform[] targetsFixedToBody;
    [SerializeField] float distanceToMove = 5f;
    [SerializeField] float distanceToKeepFromGround = 0.3f;
    [SerializeField] LayerMask mask;

    Vector3[] targetLocationsOnGround;
    Vector3[] fixedTargetLocationsOnGround;

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
        bool changed = false;
        for (int i = 0; i < targetsFixedToBody.Length; i++)
        {
            Physics.Raycast(targetsFixedToBody[i].position, -transform.up, out hit, Mathf.Infinity, mask);
            fixedTargetLocationsOnGround[i] = hit.point;
        }
        for (int i = 0; i < targets.Length; i++)
        {
            //replace distance with sphere
            if (Vector3.Distance(targetLocationsOnGround[i], fixedTargetLocationsOnGround[i]) > distanceToMove)
            {
                //only move odd/even leggs
                targets[i].position = fixedTargetLocationsOnGround[i];
                targetLocationsOnGround[i] = fixedTargetLocationsOnGround[i];
                changed = true;
            }
            else
            {
                targets[i].position = targetLocationsOnGround[i];
            }
        }
        if (changed)
        {
            //vector 1 and 2 temp
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
        }
    }
}
