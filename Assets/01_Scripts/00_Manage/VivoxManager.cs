using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Vivox;
using System;

public class VivoxManager : MonoBehaviour
{
    public bool IsInitialized { get; private set; } = false;
    public bool IsInitializing { get; private set; } = false;
    [SerializeField] private LoadingManager loadingManage;

    public async Awaitable InitializeVivoxAsync()
    {
        // 이미 초기화됐거나 초기화 중이면 중복 실행 방지
        if (IsInitialized || IsInitializing)
        {
            Debug.LogWarning("Vivox는 이미 초기화되었거나 초기화 중입니다.");
            return;
        }

        IsInitializing = true;
        loadingManage.Show();

        try
        {

            // 2. 유니티 게임 서비스(UGS) 초기화
            await UnityServices.InitializeAsync();

            // 3. Vivox 서비스 초기화
            await VivoxService.Instance.InitializeAsync();

            IsInitialized = true;
            Debug.Log("Vivox 초기화 완료!");

            LoginOptions option = new LoginOptions();
            option.DisplayName = "None"; // 나중에 유저 닉네임을 받을 때 string 변수 넣을 것
            await LoginVivox(option);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Vivox 초기화 실패: {e.Message}");
            // TODO: 여기서 에러 UI 표시 (재시도 버튼 등) 나중에...
        }
        finally
        {
            IsInitializing = false;
            loadingManage.Hide();
        }
    }

    private async Awaitable LoginVivox(LoginOptions option)
    {
        await VivoxService.Instance.LoginAsync(option);
    }

    public async Awaitable JoinChannel(string code)
    {
        await VivoxService.Instance.JoinGroupChannelAsync(code, ChatCapability.AudioOnly);
    }
}