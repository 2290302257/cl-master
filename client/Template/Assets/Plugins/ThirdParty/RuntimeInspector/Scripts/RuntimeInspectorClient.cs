using UnityEngine;
using UnityEngine.UI;

public class RuntimeInspectorClient : MonoBehaviour
{
    static GameObject Go;

    public static void Show()
    {
        if (!Go)
        {
            Go = GameObject.Instantiate(Resources.Load<GameObject>("RuntimeInspector/RuntimeInspectorClient"));
        }
        Go.SetActive(true);
    }
    
    public Button CloseBtn;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        CloseBtn.onClick.AddListener(() =>
        {
            Go.SetActive(false);
        });
    }
}
