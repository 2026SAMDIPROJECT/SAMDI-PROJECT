using System;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[AutoStaticsCleanup]
public partial class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public event Action<Vector2> MoveEvent;
    public event Action<InputAction.CallbackContext> InteractEvent;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    // 인스펙터 Action Events에서 연결
    public void OnMove(InputAction.CallbackContext ctx)
    {
        MoveEvent?.Invoke(ctx.ReadValue<Vector2>());
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        InteractEvent?.Invoke(ctx);
    }
}
