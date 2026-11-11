using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAddInteract : InteractiveObject
{
    public string nextSceneName;
    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEventReceived;
        }
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if(NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEventReceived;
        }
        base.OnNetworkDespawn();
    }
    public override void Interact(NetworkBehaviourReference inventoryHolderRef)
    {
        if(!IsServer)
        {
            Debug.Log("호스트 혹은 서버만 상호작용할 수 있습니다.");
            return;
        }
        if (NetworkManager.Singleton.NetworkConfig.EnableSceneManagement)
        {
            SceneEventProgressStatus status = NetworkManager.Singleton.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Additive);

            if (status == SceneEventProgressStatus.Started)
            {
                Debug.Log("미니게임 씬 비동기 로드 및 클라이언트 동기화 시작!");
            }
            else
            {
                Debug.LogError($"씬 로드 실패: {status}");
                return;
            }
        }
    }

    private void OnSceneEventReceived(SceneEvent sceneEvent)
    {
        if(sceneEvent.SceneName != nextSceneName ||
        sceneEvent.ClientId != NetworkManager.Singleton.LocalClientId ||
        sceneEvent.LoadSceneMode != LoadSceneMode.Additive) return; 
        
        if(sceneEvent.SceneEventType == SceneEventType.LoadComplete)
        {
            if (InputManager.Instance != null && PlayerRegistry.Instance != null)
            {
                InputManager.Instance.enabled = false;
                InputManager.Instance.root.SetActive(false);
                PlayerRegistry.Instance.DisActivePlayers();
            }
        }
    }
}