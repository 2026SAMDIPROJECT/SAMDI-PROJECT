using UnityEngine;
using UnityEngine.InputSystem;
public class F_CameraRotation : MonoBehaviour
{
        // 카메라 회전
    [SerializeField] private float sensitivity; // 감도
    [SerializeField] private Transform player; // 플레이어
    private float xRot; // x 회전도(위아래)ee

    private void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mY = mouseDelta.y; // y
        float mX = mouseDelta.x; // x

        xRot -= mY;
        xRot = Mathf.Clamp(xRot, -30f, 40f); 
        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);

        // if(player != null)
        //     player.Rotate(Vector3.up * mX); // x 회전
    }
}
