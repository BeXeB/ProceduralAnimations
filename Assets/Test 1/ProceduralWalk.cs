using UnityEngine;

public class ProceduralWalk : MonoBehaviour
{
    [SerializeField] Transform[] targets;
    [SerializeField] Transform[] targetsFixedToBody;
    [SerializeField] float distanceToMove = 5f;
    [SerializeField] LayerMask mask;
    [SerializeField] Transform body;

    Vector3[] targetLocationsOnGround;
    Vector3[] fixedTargetLocationsOnGround;
    Vector3 normal;
    Vector3 average;

    private void Start()
    {
        targetLocationsOnGround = new Vector3[targets.Length];
        fixedTargetLocationsOnGround = new Vector3[targetsFixedToBody.Length];
        RaycastHit hit;
        for (int i = 0; i < targets.Length; i++)
        {
            Physics.Raycast(targets[i].position, Vector3.down, out hit, Mathf.Infinity, mask);
            targetLocationsOnGround[i] = hit.point;
        }
    }

    void Update()
    {
        normal = Vector3.zero;
        average = Vector3.zero;
        RaycastHit hit;
        for (int i = 0; i < targetsFixedToBody.Length; i++)
        {
            Physics.Raycast(targetsFixedToBody[i].position, Vector3.down, out hit, Mathf.Infinity, mask);
            fixedTargetLocationsOnGround[i] = hit.point;
            normal += hit.normal;
        }
        for (int i = 0; i < targets.Length; i++)
        {
            if (Vector3.Distance(targetLocationsOnGround[i], fixedTargetLocationsOnGround[i]) > distanceToMove)
            {
                targets[i].position = fixedTargetLocationsOnGround[i];
                targetLocationsOnGround[i] = fixedTargetLocationsOnGround[i];
            }
            else
            {
                targets[i].position = targetLocationsOnGround[i];
            }
        }
    }
}
