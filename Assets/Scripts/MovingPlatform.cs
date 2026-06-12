using UnityEngine;

public sealed class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 localMoveOffset = new Vector3(3f, 0f, 0f);
    [SerializeField] private float moveSpeed = 1.2f;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float t = (Mathf.Sin(Time.time * moveSpeed) + 1f) * 0.5f;
        transform.position = Vector3.Lerp(startPosition, startPosition + localMoveOffset, t);
    }
}
