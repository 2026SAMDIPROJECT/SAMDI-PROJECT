using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.UI;

[AutoStaticsCleanup]
public partial class UIManager : MonoBehaviour
{
    public static UIManager instance{ get; private set;}
    public Image interactImg;
    public Image fillImg;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}