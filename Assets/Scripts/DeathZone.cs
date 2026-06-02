using UnityEngine;

public sealed class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        FallRespawn respawn = other.GetComponentInParent<FallRespawn>();
        if (respawn == null)
        {
            return;
        }

        respawn.Respawn();
    }
}
