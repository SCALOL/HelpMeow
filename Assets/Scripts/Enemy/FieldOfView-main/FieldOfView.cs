using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0,360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;
    public Transform target;
    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            for (int i = 0; i < rangeChecks.Length; i++)
            {
                //Check if player in rangeChecks is obstructed or not
                if (rangeChecks[i] == playerRef.GetComponent<Collider>())
                {
                    target = rangeChecks[i].transform;
                    if (CheckifObstructed())
                    {
                        canSeePlayer = true;
                        return;
                    }
                    else
                    {
                        canSeePlayer = false;
                        break;
                    }
                } 
                //if player in rangeChecks is Obsurcted, continue loop
            }
            for (int i = 0; i < rangeChecks.Length; i++)
            {
                target = rangeChecks[i].transform;
                if (CheckifObstructed())
                {
                    canSeePlayer = true;
                    break;
                }
                else
                {
                    canSeePlayer = false;
                }
            }

                
        }
        else if (canSeePlayer) {canSeePlayer = false;}
            
    }

    public bool CheckifObstructed()
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    } 
                        
                } else
                {
                    return false;
                }
    }
}
