using UnityEngine;

public sealed class CoinPickup : MonoBehaviour
{
    private static int collectedCount;

    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private float destroyDelay = 0.02f;
    [SerializeField] private bool isGoalCoin;

    private bool collected;

    public static int CollectedCount => collectedCount;

    public static void ResetCollectedCount()
    {
        collectedCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected || other.GetComponentInParent<FallRespawn>() == null)
        {
            return;
        }

        collected = true;
        collectedCount++;
        if (JumpMapGameManager.Instance != null)
        {
            JumpMapGameManager.Instance.AddCoinScore();
            if (isGoalCoin)
            {
                JumpMapGameManager.Instance.ClearGoal();
            }
        }

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        Debug.Log($"Coin collected: {collectedCount}");
        Destroy(gameObject, destroyDelay);
    }
}
