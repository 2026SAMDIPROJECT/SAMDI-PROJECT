using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
public class NetworkManagerUI : MonoBehaviour
{
    [Header("서버 클라 버튼")]
    [SerializeField] private Button hostBnt;
    [SerializeField] private Button clientBnt;

    [Header("코드 UI")]
    [SerializeField] private TMP_Text hostCode;
    [SerializeField] private TMP_InputField clientCode;

    private async void Start()
    {

        // 유니티 클라우드 비동기 대기(서비스 초기화, 익명 로그인)
        await UnityServices.InitializeAsync();

        // 로그인 안 되어 있을 때
        if(!AuthenticationService.Instance.IsSignedIn)
        {
            // 익명 로그인
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"익명 로그인 :: {AuthenticationService.Instance.PlayerId}");
        }

        // 로그인이 모두 완료 되었을 때 버튼에 리스너 추가(그 전엔 눌러도 반응 없음)
        hostBnt.onClick.AddListener(() => CreateRelayHost());
        clientBnt.onClick.AddListener(() => JoinRelayClient());
    }
    // UI 비활성화
    private void DisableUI()
    {
        gameObject.SetActive(false);
    }

    // Host
    private async void CreateRelayHost()
    {
        try
        {
            hostCode.transform.parent.gameObject.SetActive(true);
            // 방 최대 인원 4명(IP, 포트번호, 접속 키 같은게 다 있음)
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

            // 코드 발급
            string code = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            hostCode.text = code;
            
            // 서버 정보(Allocation을 RelayServerData로 바꿈) dtls는 변경 방식
            RelayServerData serverData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

            NetworkManager.Singleton.StartHost(); // 호스트 시작
            DisableUI();
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    //Client
    private async void JoinRelayClient()
    {
        try
        {
            if(clientCode.text == null)
            {
                Debug.Log("코드를 비울 수 없음");
            }
            string code = clientCode.text; // 입력된 코드 받음

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(code); // 입장 allocation 받음

            RelayServerData serverData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls"); // 서버 데이터 바꿈
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData); // 이 코드로 서버 찾음

            NetworkManager.Singleton.StartClient(); // 클라이언트 시작
            DisableUI();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
