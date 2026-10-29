using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAddInteract : InteractiveObject
{
    public override void Interact(NetworkBehaviourReference inventoryHolderRef)
    {
        InputManager.Instance.enabled = false;
        InputManager.Instance.root.SetActive(false);
        Camera.main.gameObject.SetActive(false);
        SceneManager.LoadSceneAsync("TestMiniGame", LoadSceneMode.Additive);
    }
}