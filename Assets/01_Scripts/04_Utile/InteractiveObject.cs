using Unity.Netcode;
using UnityEngine;

public class InteractiveObject : NetworkBehaviour
{
    public virtual void Interact(PlayerInteract interactor) //추가: PlayerInteract 매개변수
    {
        
    }
}