using UnityEngine;

public sealed class RotatingHazard : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<FallRespawn>() == null)
        {
            return;
        }

        if (JumpMapGameManager.Instance != null)
        {
            JumpMapGameManager.Instance.GameOver("Hit Hazard!");
        }
    }
}
