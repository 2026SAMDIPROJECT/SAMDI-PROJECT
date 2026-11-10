using Unity.Netcode;
using UnityEngine;

public class OwnerCameraToggle : NetworkBehaviour
{
    [SerializeField] private Camera cam;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            if (cam != null) cam.gameObject.SetActive(false);
            return;
        }

        Camera main = Camera.main;
        if (main != null && main != cam)
            main.gameObject.SetActive(false);
    }
}
