using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.AI;
using Unity.VisualScripting;
using Unity.AI.Navigation;
using UnityEngine.UIElements;

public enum EnemyState
{
    Patrolling,
    Chasing,
    Searching,
    Flee,
    Scanning,
    Charge
}

public class EnemyMovement : MonoBehaviour
{
    private FieldOfView fov;

    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private int currentPatrolIndex = 0;
    [SerializeField] private float lostPlayerTimer = 0f;
    [SerializeField] private Vector3 lastKnownPlayerPosition;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject SoundListerner;
    [SerializeField] string patrolPointsName = "PatrolPoints";
    [SerializeField] private EnemyState currentState = EnemyState.Patrolling;
    [SerializeField] private float searchRadius = 10f;
    [SerializeField] private float BossRotateSpeed = 1f;
    [SerializeField] Animator animation;
    [SerializeField] private float DashSpeed = 20f;
    [SerializeField] private float DashAcceleration = 20f;
    private float OriginalAccelation;
    private float OriginalSpeed;
    private BossType CheckbossType;
    [SerializeField] private float FinalDestination = 2f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float RunPitch = 1.5f;
    [SerializeField] private float originalPitch;
    [SerializeField] private AudioClip ChargeAudioClip;
    [SerializeField] private AudioClip RushingAudioClip;
    [SerializeField] private AudioClip FoundAudioClip;

    void Start()
    {
        Debug.Log("Enemy working");
        fov = GetComponent<FieldOfView>();
        agent = GetComponent<NavMeshAgent>();

        originalPitch = audioSource.pitch;
        RunPitch *= originalPitch
            ;
        OriginalAccelation = agent.acceleration;
        OriginalSpeed = agent.speed;

        CheckbossType = GetComponent<BossData>().bossType;

        //Assure that SoundHitbox set to false at start
        SoundListerner.SetActive(false);
        //if Enemy don't already has patrolPoints, give it
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            GameObject patrolParent = GameObject.Find(patrolPointsName);
            Transform[] patrolParentChild = patrolParent.GetComponentsInChildren<Transform>();
            if (patrolParent != null)
            {
                List<Transform> points = new List<Transform>();
                for (int i = 1; i < patrolParentChild.Length; i++)
                {
                    points.Add(patrolParentChild[i]);
                }
                //Now Enemy has patrol points for patrolling 
                patrolPoints = points.ToArray();
            }
        }
    }

    void Update()
    {
        EnemyBehaviour();
        animation.SetBool("IsRunning", currentState != EnemyState.Patrolling);

        
    }

    private void EnemyBehaviour()
    {
        switch (currentState)
        {
            case EnemyState.Chasing:
                agent.destination = lastKnownPlayerPosition;

                if (!fov.canSeePlayer)
                {
                    lostPlayerTimer += Time.deltaTime;
                    if (lostPlayerTimer >= 2f && Vector3.Distance(transform.position, agent.destination) < FinalDestination)
                    {
                        if (agent.speed != OriginalSpeed || agent.acceleration != OriginalAccelation)
                        {
                            agentSpeedCon(OriginalAccelation, OriginalSpeed);
                        }
                        currentState = EnemyState.Searching;
                        lostPlayerTimer = 0f;
                    }
                    
                }
                break;

            case EnemyState.Searching:
                lostPlayerTimer += Time.deltaTime;
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    Vector2 randomCircle = Random.insideUnitCircle * searchRadius;
                    Vector3 randomSearchPoint =
                        lastKnownPlayerPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(randomSearchPoint, out hit, searchRadius, NavMesh.AllAreas))
                    {
                        agent.SetDestination(hit.position);
                    }
                }

                //After x (time) of seraching, return to patrolling
                if (lostPlayerTimer >= 4f)
                {
                    currentState = EnemyState.Patrolling;
                    lostPlayerTimer = 0f;

                }
                break;
            case EnemyState.Flee:
                // Flee behavior: move away from flashlight hit direction, then return to patrolling
                lostPlayerTimer += Time.deltaTime;

                // Assume you have a Vector3 'flashlightHitDirection' set externally when hit by flashlight
                // Calculate a random direction away from the flashlight hit
                GameObject Player = GameObject.FindGameObjectsWithTag("Player")[0];
                Vector3 PlayerPos = Player.transform.position;
                Vector3 fleeDirection = (transform.position - PlayerPos).normalized;
                Vector2 randomOffset = Random.insideUnitCircle * 3f; // Add some randomness
                Vector3 fleeTarget = transform.position + fleeDirection * 8f + new Vector3(randomOffset.x, 0, randomOffset.y);

                NavMeshHit fleeHit;
                if (Vector3.Distance(transform.position, agent.destination) < 1.5f || agent.destination == Vector3.zero)
                {
                    if (NavMesh.SamplePosition(fleeTarget, out fleeHit, 2f, NavMesh.AllAreas))
                    {
                        agent.destination = fleeHit.position;
                    }
                }

                // Wait for a short time after reaching the flee destination, then return to patrolling
                if (lostPlayerTimer >= 2.5f && Vector3.Distance(transform.position, agent.destination) < FinalDestination)
                {
                    currentState = EnemyState.Patrolling;
                    lostPlayerTimer = 0f;
                }
                break;
            case EnemyState.Charge:
                
                lostPlayerTimer += Time.deltaTime;
                if (lostPlayerTimer >= 3f)
                {
                    currentState = EnemyState.Scanning;
                    SoundListerner.SetActive(true);
                    lostPlayerTimer = 0f;
                }
                break;
            case EnemyState.Scanning:

                lostPlayerTimer += Time.deltaTime;
                if (lostPlayerTimer >= 6f)
                {
                    animation.SetTrigger("AttackTriggered");
                    currentState = EnemyState.Patrolling;
                    SoundListerner.SetActive(false);
                    lostPlayerTimer = 0f;
                }

                
                break;
            case EnemyState.Patrolling:
                //Randomly select next patrol point when reached current one
                if (patrolPoints != null && patrolPoints.Length > 0)
                {

                    if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 2f || agent.destination == Vector3.zero)
                    {
                        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
                    }
                    agent.destination = patrolPoints[currentPatrolIndex].position;

                    //if This is GoodEar and can't find player for x-y seconds, go to scanning state
                    if (CheckbossType == BossType.GoodEar)
                    {
                        lostPlayerTimer += Time.deltaTime;
                        if (lostPlayerTimer >= 4f)
                        {
                            currentState = EnemyState.Charge;
                            animation.SetTrigger("AttackTriggered");
                            agent.destination = transform.position;
                            lostPlayerTimer = 0f;
                        }
                    }
                }
                break;

        }
        if (fov.canSeePlayer && currentState != EnemyState.Flee)
        {
            lastKnownPlayerPosition = fov.target.position;
            if (currentState != EnemyState.Chasing) 
            {
                AudioManager.instance.PlaySFX(FoundAudioClip);
            }
            
            audioSource.pitch = RunPitch;
            agent.speed *= 1.5f;
            currentState = EnemyState.Chasing;
            lostPlayerTimer = 0f;
            agent.destination = lastKnownPlayerPosition;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Flashlight") && currentState == EnemyState.Chasing && currentState != EnemyState.Flee)
        {
            agent.destination = transform.position;
            audioSource.pitch = originalPitch;
            agent.speed = OriginalSpeed;
            currentState = EnemyState.Flee;
        }
        else if (other.CompareTag("PlayerSound"))
        {
            Debug.Log("PlayerSound Detected");
            if (currentState == EnemyState.Scanning)
            {
                //Go to Player Sound Position
                SoundListerner.SetActive(false);
                lastKnownPlayerPosition = other.transform.position;
                currentState = EnemyState.Chasing;
                animation.SetTrigger("AttackTriggered");
                lostPlayerTimer = 0f;
                agentSpeedCon(OriginalAccelation * DashAcceleration, OriginalSpeed * DashSpeed);
            }
            else
            {
                Debug.Log("Hear Footstep");
                lastKnownPlayerPosition = fov.target.position;
                if (currentState != EnemyState.Chasing)
                {
                    AudioManager.instance.PlaySFX(FoundAudioClip);
                }

                audioSource.pitch = RunPitch;
                agent.speed *= 1.5f;
                currentState = EnemyState.Chasing;
                lostPlayerTimer = 0f;
                agent.destination = lastKnownPlayerPosition;
            }
            
        }

    }
    public string agentSpeedCon(float acc,float speed)
    {
        agent.acceleration = acc;
        agent.speed = speed;

        return "agent Acceleration : " + acc + " agent Speed : " + speed;
    }
}

