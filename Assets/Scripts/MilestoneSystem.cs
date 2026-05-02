using UnityEngine;
using System.Collections.Generic;

public class MilestoneSystem : MonoBehaviour
{
    public static MilestoneSystem Instance { get; private set; }

    public Vector3 lastMilestonePosition { get; private set; }
    public int lastMilestoneIndex { get; private set; }

    // This list would be populated with actual Milestone objects/positions in the scene
    // For now, it's a placeholder.
    public List<Vector3> allMilestonePositions = new List<Vector3>();

    public delegate void OnMilestoneReached(int milestoneIndex);
    public static event OnMilestoneReached onMilestoneReached;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        // Initialize with a starting point (e.g., the beginning of the current country level)
        lastMilestonePosition = Vector3.zero; // Placeholder
        lastMilestoneIndex = -1; // No milestone reached yet
    }

    public void SetMilestone(Vector3 position, int index)
    {
        lastMilestonePosition = position;
        lastMilestoneIndex = index;
        onMilestoneReached?.Invoke(index);
        Debug.Log($"Milestone {index} reached at position {position}");
    }

    public void ResetToLastMilestone()
    {
        // This method would be called on game over if player chooses not to watch ad
        // PlayerController would then move the camel to lastMilestonePosition
        Debug.Log($"Resetting to last milestone at position: {lastMilestonePosition}");
        // HealthSystem.Instance.ResetHealth(); // Optionally reset health on milestone restart
    }

    // In a real game, milestones would be triggered by the player passing through a specific point
    // For example, a trigger collider on a Milestone GameObject.
    public void TriggerMilestone(int index, Vector3 position)
    {
        if (index > lastMilestoneIndex)
        {
            SetMilestone(position, index);
        }
    }

    public void ResetMilestones()
    {
        lastMilestonePosition = Vector3.zero; // Reset to a default starting point
        lastMilestoneIndex = -1; // No milestone reached yet
        Debug.Log("MilestoneSystem: Milestones reset.");
    }
}
