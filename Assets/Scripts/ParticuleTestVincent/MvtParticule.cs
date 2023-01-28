using UnityEngine;

public class MvtParticule : MonoBehaviour
{
    [Range(0.1f,10f)]public float distanceMaxTarget = 5f;
    [Range(0.1f, 10f)] public float speed = 2f;
    public bool randomDistance;
    public bool randomSpeed;
    public Color idleColor;
    bool gizmo;
    Vector2 target;
    DetectionParticule detectionManager;
    private void Awake()
    {
        detectionManager = GameObject.Find("LineBall (2)").GetComponent<DetectionParticule>();
    }
    private void OnEnable()
    {
        target = transform.position;
        gizmo = true;
        GetComponent<SpriteRenderer>().color = idleColor;
    }
    private void Update()
    {
        //Comportement
        GoToTarget(target);
        float distanceToTarget = Vector2.Distance(transform.position, target);
        if(distanceToTarget <= 0.5f)
        {
            target = CreateTargetPos();
            if (randomDistance)
            {
                distanceMaxTarget = ChangeDistanceMaxTarget();
            }
            if (randomSpeed)
            {
                speed = ChangeSpeed();
            }
        }
        //Transition
        float distanceToPoint = Vector3.Distance(detectionManager.gameObject.transform.position, transform.position);
        if (distanceToPoint <= detectionManager.rangeDetection)
        {
            GetComponent<MvtFollowPoint>().enabled = true;
            enabled = false;
        }
    }
    private void OnDisable()
    {
        gizmo = false;
    }
    Vector2 CreateTargetPos()
    {
        //créer un point cible ou se rend la particule
        Vector2 target = new Vector2(transform.position.x + Random.Range(-distanceMaxTarget, distanceMaxTarget), transform.position.y + Random.Range(-distanceMaxTarget, distanceMaxTarget));
        return target;
    }
    void GoToTarget(Vector2 target)
    {
        //Se rendre a l'endroit cible
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
    float ChangeDistanceMaxTarget()
    {
        float newDistance = Random.Range(0.1f, 10f);
        return newDistance;
    }
    float ChangeSpeed()
    {
        float newSpeed = Random.Range(0.1f, 10f);
        return newSpeed;
    }
    private void OnDrawGizmos()
    {
        if (gizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(target, 0.2f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target);      
        }
    }
}
