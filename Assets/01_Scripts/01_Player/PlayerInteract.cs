using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class PlayerInteract : NetworkBehaviour
{
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private Image interactionUI;
    [SerializeField] private Image interactionFill;
    [SerializeField] private float interval;
    [SerializeField] private float distance;
    [SerializeField] Camera cam;
    private float holdDuration;
    private bool ishold;
    private bool isInteract;
    private Timer holdTime = new Timer();
    private InteractiveObject interactTarget;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;

        if(InputManager.Instance != null)
            InputManager.Instance.InteractEvent += HandleInteract;
        else
            Debug.LogWarning("InputManager.Instance가 null입니다. 씬에 InputManager가 있는지 확인하세요.");
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) return;
        if(InputManager.Instance != null)
            InputManager.Instance.InteractEvent -= HandleInteract;
    }

    private void Update()
    {
        if(ishold && isInteract)
        {
            holdTime.RunTimer(); // 타이머 실행
            interactionFill.fillAmount = holdTime.progress; // 타이머 진행도 프로퍼티 사용해 백분율 구함
        }
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, distance, interactionLayer))
        {
            if(hit.collider.TryGetComponent(out InteractiveObject obj)) // 콜라이더에 함수를 넣을 순 없으니까 GetComponent
            {
                interactionUI.gameObject.SetActive(true);
                isInteract = true;
                interactTarget = obj;
            }
            else
            {
                Debug.Log("the obj not has interactiveobj");
            }
        }
        else
        {
            interactionUI.gameObject.SetActive(false);
            holdTime.EndTimer();
            isInteract = false;
        }
    }

    private void HandleInteract(InputAction.CallbackContext callback)
    {
        if(callback.started)
        {
            if(callback.interaction is HoldInteraction hold)
            {
                holdDuration = hold.duration > 0? hold.duration : InputSystem.settings.defaultHoldTime;
                holdTime.StartTimer(holdDuration);
            }
            ishold = true;
        }

        if(callback.performed)
        {
            interactTarget.Interact();
        }

        if(callback.canceled)
        {
            holdTime.EndTimer();
            interactionFill.fillAmount = 0f;
            ishold = false;
        }
    }
}
