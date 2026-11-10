using Unity.Netcode;
using UnityEngine;

public class QuickHostStart : MonoBehaviour
{
    private void Start()
    {
        NetworkManager.Singleton.StartHost();
    }
}
