using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class WheelchairController : NetworkBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private float minCharge = 0.2f;
    [SerializeField] private float maxCharge = 1.5f;
    [SerializeField] private float chargeToForce = 10f;
    [SerializeField] private float drag = 2f;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float fullChargeMoveSpeed = 10f;

    private float chargeStartTime;
    private float charge;
    private bool isCharging;
    private bool isMoving;
    private bool isFullChargeMove;
    private float currentMoveSpeed;
    private Vector3 moveTarget;

    public bool IsMoving => isMoving;
    // WheelchairBumpHandler가 읽어서 밀치기 가능 여부 판단함
    public bool IsFullChargeMove => isMoving && isFullChargeMove;
    public float ChargeProgress01 => isCharging
        ? Mathf.Clamp01((Time.time - chargeStartTime) / maxCharge)
        : Mathf.Clamp01(charge / maxCharge);

    public void OnWheelchairCharge(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;

        if (ctx.started)
        {
            isCharging = true;
            chargeStartTime = Time.time;
        }
        else if (ctx.canceled)
        {
            isCharging = false;
            float held = Time.time - chargeStartTime;
            charge = Mathf.Clamp(held, minCharge, maxCharge);
        }
    }

    public void OnWheelchairPush(InputAction.CallbackContext ctx)
    {
        if (!IsOwner || !ctx.performed || isCharging) return;
        if (charge <= 0f) return;

        isFullChargeMove = charge >= maxCharge;
        currentMoveSpeed = isFullChargeMove ? fullChargeMoveSpeed : moveSpeed;
        moveTarget = rigid.position + transform.forward * charge * chargeToForce;
        isMoving = true;
        charge = 0f;
    }

    public void WheelchairSpinL(InputAction.CallbackContext ctx)
    {
        if (!IsOwner || !ctx.performed || isCharging) return;
        if (charge <= 0f) return;

        rigid.AddTorque(-transform.up * charge * chargeToForce, ForceMode.Impulse);
        charge = 0f;
    }

    public void WheelchairSpinR(InputAction.CallbackContext ctx)
    {
        if (!IsOwner || !ctx.performed || isCharging) return;
        if (charge <= 0f) return;

        rigid.AddTorque(transform.up * charge * chargeToForce, ForceMode.Impulse);
        charge = 0f;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        rigid.linearVelocity *= 1f - drag * Time.fixedDeltaTime;

        if (isMoving)
        {
            // AddForce 누적 대신 좌표 이동 방식으로 바꿈. 연속 발사해도 속도가 무한히 쌓이지 않음
            Vector3 next = Vector3.MoveTowards(rigid.position, moveTarget, currentMoveSpeed * Time.fixedDeltaTime);
            rigid.MovePosition(next);
            if (next == moveTarget) isMoving = false;
        }
    }
}
