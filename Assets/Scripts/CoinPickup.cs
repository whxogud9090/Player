using UnityEngine;

public sealed class CoinPickup : MonoBehaviour
{
    private static int collectedCount;

    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private float destroyDelay = 0.02f;

    private bool collected;

    public static int CollectedCount => collectedCount;

    private void OnTriggerEnter(Collider other)
    {
        if (collected || other.GetComponentInParent<FallRespawn>() == null)
        {
            return;
        }

        collected = true;
        collectedCount++;

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        Debug.Log($"Coin collected: {collectedCount}");
        Destroy(gameObject, destroyDelay);
    }
}
