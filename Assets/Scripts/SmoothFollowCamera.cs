using UnityEngine;

public sealed class SmoothFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5.5f, -7.5f);
    [SerializeField] private float followSmoothTime = 0.12f;
    [SerializeField] private float lookHeight = 1.1f;

    private Vector3 followVelocity;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref followVelocity,
            followSmoothTime);

        Vector3 lookPoint = target.position + Vector3.up * lookHeight;
        transform.rotation = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
    }
}
