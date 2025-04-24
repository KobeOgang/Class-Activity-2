using UnityEngine;
using TMPro; 
using UnityStandardAssets.Utility;

public class AICarController : MonoBehaviour
{
    [SerializeField] private float waypointRadius = 2f; 
    private int currentWaypointIndex = 0; 
    private Vector3 targetPosition; 

    private int currentLap = 0; 
    private static bool raceFinished = false; 

    
    [SerializeField] private GameObject raceStatusPanel; 
    [SerializeField] private TextMeshProUGUI raceStatusText; 
    [SerializeField] private TextMeshProUGUI lapCounterUI; 

    
    public int laneIndex; 
    private float laneSpacing = 40f; 

    
    private CarSystem carSystem; 
    [SerializeField] private WaypointCircuit waypointCircuit; 

    private void Start()
    {
        
        laneIndex = UnityEngine.Random.Range(-1, 2);

        
        carSystem = GetComponent<CarSystem>();
        if (carSystem == null)
        {
            Debug.LogError("No CarSystem found on the car! Make sure the script is attached.");
            enabled = false;
            return;
        }

        
        if (waypointCircuit == null || waypointCircuit.Waypoints.Length == 0)
        {
            Debug.LogError("No waypoint circuit assigned or no waypoints found!");
            enabled = false;
            return;
        }

        
        if (raceStatusPanel != null)
        {
            raceStatusPanel.SetActive(false); 
        }

        
        UpdateTargetToWaypoint();

        
        if (lapCounterUI != null)
        {
            lapCounterUI.text = $"Laps: {currentLap}/3";
        }
    }

    private void Update()
    {
        if (waypointCircuit == null || waypointCircuit.Waypoints.Length == 0 || raceFinished) return;

        
        Vector3 direction = targetPosition - transform.position;

        
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            carSystem.rotSpeed * Time.deltaTime
        );

        
        if (Vector3.Distance(transform.position, targetPosition) > waypointRadius * 2)
        {
            carSystem.ApplyAcceleration();
        }
        else
        {
            carSystem.ApplyDeceleration();
        }

        
        carSystem.ApplyTranslation();

        
        if (Vector3.Distance(transform.position, targetPosition) <= waypointRadius)
        {
            ProgressToNextWaypoint();
        }
    }

    private void UpdateTargetToWaypoint()
    {
        
        targetPosition = GetOffsetWaypointPosition(waypointCircuit.Waypoints[currentWaypointIndex]);
    }

    private void ProgressToNextWaypoint()
    {
        
        currentWaypointIndex = (currentWaypointIndex + 1) % waypointCircuit.Waypoints.Length;

        
        if (currentWaypointIndex == 0) 
        {
            currentLap++;
            if (lapCounterUI != null)
            {
                lapCounterUI.text = $"Laps: {currentLap}/3";
            }

            
            if (currentLap >= 3 && !raceFinished)
            {
                raceFinished = true;

                
                if (raceStatusPanel != null && raceStatusText != null)
                {
                    raceStatusPanel.SetActive(true); 
                    raceStatusText.text = $"{gameObject.name} won!"; 
                }
                Debug.Log($"{gameObject.name} won the race!");
            }
        }

        
        UpdateTargetToWaypoint();
    }

    private Vector3 GetOffsetWaypointPosition(Transform waypoint)
    {
        
        Vector3 offset = waypoint.right * laneIndex * laneSpacing;
        return waypoint.position + offset;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && waypointCircuit != null && waypointCircuit.Waypoints.Length > 0)
        {
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetPosition);

            
            Gizmos.color = Color.yellow;
            foreach (Transform waypoint in waypointCircuit.Waypoints)
            {
                Gizmos.DrawWireSphere(GetOffsetWaypointPosition(waypoint), waypointRadius);
            }

            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPosition, waypointRadius);
        }
    }
}