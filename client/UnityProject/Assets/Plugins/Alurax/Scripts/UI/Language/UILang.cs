using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Alurax
{
    public class UILang : MonoBehaviour, IExportProcess
    {
        public UILangLibrary library;
        public string key;
        public void Process()
        {
#if UNITY_EDITOR
            if (TryGetComponent<TextMeshProUGUI>(out var tmp))
            {
                tmp.text = string.Empty;
            }
            else if (TryGetComponent<Text>(out var text))
            {
                text.text = string.Empty;
            }
#endif
        }
    }
}
