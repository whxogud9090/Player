using UnityEngine;

public sealed class FallRespawn : MonoBehaviour
{
    [SerializeField] private float fallY = -12f;
    [SerializeField] private Transform respawnPoint;

    private CharacterController controller;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        startPosition = respawnPoint != null ? respawnPoint.position : transform.position;
        startRotation = respawnPoint != null ? respawnPoint.rotation : transform.rotation;
    }

    private void Update()
    {
        if (transform.position.y > fallY)
        {
            return;
        }

        if (JumpMapGameManager.Instance != null)
        {
            JumpMapGameManager.Instance.GameOver("You Fell!");
            return;
        }

        Respawn();
    }

    public void Respawn()
    {
        if (controller != null)
        {
            controller.enabled = false;
        }

        transform.SetPositionAndRotation(startPosition, startRotation);

        if (controller != null)
        {
            controller.enabled = true;
        }
    }
}
