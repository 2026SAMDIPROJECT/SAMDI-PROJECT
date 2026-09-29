using Unity.Netcode;
using UnityEngine;

public class PlayerMove : NetworkBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private float detectRadius;
    [SerializeField] private float checkInterval;

    private void HandleMove(Vector2 value)
    {
        moveInput = value;
    }

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            if(cam != null)
                cam.gameObject.SetActive(false);
        }
        else
        {
            Camera main = Camera.main;
            if(main != null && main != cam)
                main.gameObject.SetActive(false);

            if(InputManager.Instance != null)
                InputManager.Instance.MoveEvent += HandleMove;
            else
                Debug.LogWarning("InputManager.Instance가 null입니다. 씬에 InputManager가 있는지 확인하세요.");
        }
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) return;
        if(InputManager.Instance != null)
            InputManager.Instance.MoveEvent -= HandleMove;
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;
        MovePlayer();
    }
    
    private void MovePlayer()
    {
        Vector3 dir = (transform.forward * moveInput.y) + (transform.right * moveInput.x);
        dir.Normalize();

        Vector3 tar = dir * moveSpeed;

        rigid.linearVelocity = new Vector3(tar.x, rigid.linearVelocity.y, tar.z);
    }
}