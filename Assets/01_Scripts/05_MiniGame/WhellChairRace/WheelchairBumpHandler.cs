using Unity.Netcode;
using UnityEngine;

// 충돌 시 서로 밀어냄. 클라 권한 이동이라 상대방은 서버 경유 RPC로 밀어줌
// 자기 쪽도 로컬로 같이 밀면 양쪽 다 두 번 밀리니까 상대방만 밀 것
public class WheelchairBumpHandler : NetworkBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private WheelchairController controller;
    [SerializeField] private float bumpForce = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;
        if (!controller.IsFullChargeMove) return; // 풀차징 이동 중일 때만 상대를 밀 수 있음
        if (!collision.gameObject.CompareTag("Racer")) return;
        if (!collision.gameObject.TryGetComponent(out NetworkObject otherNetObj)) return;

        Vector3 pushDir = (collision.transform.position - transform.position).normalized;
        RequestBumpRpc(otherNetObj, pushDir);
    }

    // 클라끼리 직접 RPC 못 보내서 서버 거쳐서 상대 오너한테 전달
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestBumpRpc(NetworkObjectReference targetRef, Vector3 pushDir)
    {
        if (!targetRef.TryGet(out NetworkObject targetObj)) return;
        if (!targetObj.TryGetComponent(out WheelchairBumpHandler targetHandler)) return;

        targetHandler.ApplyBumpRpc(pushDir, RpcTarget.Single(targetObj.OwnerClientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void ApplyBumpRpc(Vector3 pushDir, RpcParams rpcParams = default)
    {
        rigid.AddForce(pushDir * bumpForce, ForceMode.Impulse);
    }
}
