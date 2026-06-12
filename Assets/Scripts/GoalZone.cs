using UnityEngine;

public sealed class GoalZone : MonoBehaviour
{
    private const float LandingTolerance = 0.18f;

    private Collider goalCollider;

    private void Awake()
    {
        goalCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other) => TryClear(other);

    private void OnTriggerStay(Collider other) => TryClear(other);

    private void TryClear(Collider other)
    {
        CharacterController controller = other.GetComponentInParent<CharacterController>();
        if (controller == null || goalCollider == null || !HasReachedGoalFloor(controller))
        {
            return;
        }

        if (JumpMapGameManager.Instance != null)
        {
            JumpMapGameManager.Instance.ClearGoal();
        }
    }

    private bool HasReachedGoalFloor(CharacterController controller)
    {
        float goalFloorY = goalCollider.bounds.min.y;
        float playerFeetY = controller.bounds.min.y;
        return playerFeetY <= goalFloorY + LandingTolerance;
    }
}
