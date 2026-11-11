using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public void OnClickMove()
    {
        SceneManager.LoadScene(sceneName);
    }
}
