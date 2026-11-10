using Unity.Netcode;
using UnityEngine;

public class WheelchairFinishTrigger : NetworkBehaviour
{
    [SerializeField] private WheelchairRaceManager raceManager;
    private bool hasFinished;

    // RegisterRacer/UnregisterRacer 내부에서 IsServer 체크하니까 여기선 그냥 호출
    public override void OnNetworkSpawn()
    {
        raceManager.RegisterRacer(OwnerClientId, transform);
    }

    public override void OnNetworkDespawn()
    {
        raceManager.UnregisterRacer(OwnerClientId);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner || hasFinished) return;
        if (!other.CompareTag("FinishLine")) return;

        hasFinished = true;
        raceManager.SubmitFinishRpc(NetworkManager.LocalClientId);
    }
}
