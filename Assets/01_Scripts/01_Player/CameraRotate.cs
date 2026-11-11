using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotate : MonoBehaviour
{
    // 카메라 회전
    [SerializeField] private float sensitivity; // 감도
    [SerializeField] private Transform player; // 플레이어
    private float xRot; // x 회전도(위아래)
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 커서 잠금
    }

    private void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue(); // 마우스 이동 감지

        float mY = mouseDelta.y; // y
        float mX = mouseDelta.x; // x

        xRot -= mY; // x 회전 계산(카메라의 회적 방향은 마우스의 반대)
        xRot = Mathf.Clamp(xRot, -90f, 90f); // 최대 제한(90도 넘어서 볼 수 없음)
        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f); // 회전

        if(player != null)
            player.Rotate(Vector3.up * mX); // x 회전
    }
}
