using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : NetworkBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private float moveSpeed;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Camera cam;

    private void OnMove(InputValue value)
    {
        if(!IsOwner) return;
        moveInput = value.Get<Vector2>();
    }

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            if(playerInput != null)
                playerInput.enabled = false;
            if(cam != null)
                cam.gameObject.SetActive(false);
        }
        else
        {
            Camera main = Camera.main;
            if(main != null && main != cam)
                main.gameObject.SetActive(false);
        }
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

        Vector3 tar = (dir * moveSpeed);

        rigid.linearVelocity = new Vector3(tar.x, rigid.linearVelocity.y, tar.z);
    }
}