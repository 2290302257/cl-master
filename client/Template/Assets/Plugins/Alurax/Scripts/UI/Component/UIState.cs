using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Alurax
{
    public class UIState : MonoBehaviour, IExport,IExportProcess
    {
        [HideInInspector] public string CurrentState;
        [HideInInspector] public List<State> States = new List<State>();
        [HideInInspector] public List<GameObject> Defaults = new List<GameObject>();

        private HashSet<GameObject> m_Selected = new HashSet<GameObject>();

        public void SetState(string state)
        {
            if (CurrentState != state)
            {
                for (int i = 0; i < States.Count; i++)
                {
                    if (States[i].stateName == state)
                    {
                        SetState(i);
                    }
                }
            }
        }
        
        private void SetState(int index)
        {
            if (index >= 0 && index < States.Count)
            {
                m_Selected.Clear();
                CurrentState = States[index].stateName;
                foreach (var state in States[index].stateEnable)
                {
                    state.Set();
                    m_Selected.Add(state.gameObject);
                }

                foreach (var state in States[index].statePos)
                {
                    state.Set();
                    m_Selected.Add(state.rectTransform.gameObject);
                }

                foreach (var state in States[index].stateImage)
                {
                    state.Set();
                    m_Selected.Add(state.go);
                }

                foreach (var state in States[index].stateTextMesh)
                {
                    state.Set();
                    m_Selected.Add(state.go);
                }

                foreach (var defau in Defaults)
                {
                    if (!m_Selected.Contains(defau))
                        defau.gameObject.SetActive(false);
                }
            }
        }

        [System.Serializable]
        public class State
        {
            public string stateName;
            public List<StateEnable> stateEnable = new List<StateEnable>();
            public List<StatePos> statePos = new List<StatePos>();
            public List<StateImage> stateImage = new List<StateImage>();
            public List<StateTextMesh> stateTextMesh = new List<StateTextMesh>();
        }

        [System.Serializable]
        public class StateEnable
        {
            [System.NonSerialized] public bool foldout = true;
            [System.NonSerialized] public string name = "隐藏组件";

            public GameObject gameObject;
            public bool enable = true;

            public void Set()
            {
                if (gameObject)
                    gameObject.SetActive(enable);
            }
        }

        [System.Serializable]
        public class StatePos
        {
            [System.NonSerialized] public bool foldout = true;
            [System.NonSerialized] public string name = "变换组件";
            public RectTransform rectTransform;
            public Vector3 anchoredPosition = Vector3.zero;
            public Vector2 sizeDelta = Vector3.zero;
            public Vector3 roation = Quaternion.identity.eulerAngles;
            public Vector3 scale = Vector3.one;
            public Vector2 pivot = Vector2.one * 0.5f;
            public bool isSetAnchor = false;
            public Vector2 anchorMin=Vector2.one * 0.5f;
            public Vector2 anchorMan=Vector2.one * 0.5f;

            public void Set()
            {
                if (rectTransform)
                {
                    rectTransform.gameObject.SetActive(true);
                    rectTransform.anchoredPosition3D = anchoredPosition;
                    rectTransform.sizeDelta = sizeDelta;
                    rectTransform.pivot = pivot;
                    rectTransform.localRotation = Quaternion.Euler(roation);
                    rectTransform.localScale = scale;
                    if (isSetAnchor)
                    {
                        rectTransform.anchorMin = anchorMin;
                        rectTransform.anchorMax = anchorMan;
                    }
                   
                }
            }
        }

        [System.Serializable]
        public class StateImage
        {
            [System.NonSerialized] public bool foldout = true;
            [System.NonSerialized] public string name = "图片组件";
            public GameObject go;
            public Image image;
            public UIImage uiImage;
            public Sprite sprite;
            public string spriteName;
            public Color color = Color.white;
            public bool isNativeSize = false;

            public void Set()
            {
                if (!image && !uiImage)
                    uiImage = go.GetComponent<UIImage>();

                if (uiImage)
                {
                    go = uiImage.gameObject;
                    go.SetActive(true);
                    uiImage.SpriteName = spriteName;
                    uiImage.color = color;
                    uiImage.isNativeSize = isNativeSize;
                }else if (image)
                {
                    go = image.gameObject;
                    go.SetActive(true);
                    image.sprite = sprite;
                    image.color = color;
                    if (isNativeSize)
                        image.SetNativeSize();
                }
            }
        }

        [System.Serializable]
        public class StateTextMesh
        {
            [System.NonSerialized] public bool foldout = true;
            [System.NonSerialized] public string name = "字体组件";
            public GameObject go;
            public TextMeshProUGUI textMesh;
            public UITextMeshPro uiTextMesh;
            public TMP_FontAsset font;
            public string fontName;
            public Color color = Color.white;
            public Material material;
            public string materialName;
            public bool enableContent = true;
            public string content;
            public float fontSize = 0;

            public void Set()
            {
                if (!textMesh && !uiTextMesh)
                    uiTextMesh = go.GetComponent<UITextMeshPro>();

                if (uiTextMesh)
                {
                    go = uiTextMesh.gameObject;
                    go.SetActive(true);
                    uiTextMesh.FontName = fontName;
                    if(enableContent)
                        uiTextMesh.text = content;
                    uiTextMesh.color = color;
                    uiTextMesh.fontSize = (int)fontSize;
                    if(!string.IsNullOrEmpty(materialName))
                        uiTextMesh.MaterialName = materialName;
                }else if (textMesh)
                {
                    go = textMesh.gameObject;
                    go.SetActive(true);
                    textMesh.font = font;
                    if(enableContent)
                        textMesh.text = content;
                    textMesh.color = color;
                    textMesh.fontSize = fontSize;
                    textMesh.fontMaterial = material;
                }
            }
        }
        
 #if UNITY_EDITOR
        [CustomEditor(typeof(UIState))]
        public class UIStateInspector : UnityEditor.Editor
        {
            private UIState m_State;

            private void OnEnable()
            {
                m_State = target as UIState;
                stateIndex = m_State.States.FindIndex((state) => { return state.stateName == m_State.CurrentState; });

            }

            private int _index = 0;

            private int stateIndex
            {
                get { return _index; }
                set { _index = value; }
            }

            private UIState.State currentState => m_State.States[stateIndex];
            private int stateCount => m_State.States.Count;
            bool hashState => stateCount > 0;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                using (new GUILayout.HorizontalScope())
                {
                    if (hashState)
                    {
                        List<string> options = m_State.States.ConvertAll<string>((a) => { return a.stateName; });
                        EditorGUI.BeginChangeCheck();
                        stateIndex = EditorGUILayout.Popup(stateIndex, options.ToArray());
                        if (EditorGUI.EndChangeCheck())
                        {
                            TrySave();
                        }

                        string name = EditorGUILayout.TextField(currentState.stateName);
                        options.RemoveAt(stateIndex);
                        currentState.stateName = ObjectNames.GetUniqueName(options.ToArray(), name);
                    }

                    if (GUILayout.Button("新增"))
                    {
                        List<string> options = m_State.States.ConvertAll<string>((a) => { return a.stateName; });
                        stateIndex = stateCount;
                        m_State.States.Add(new UIState.State()
                            { stateName = ObjectNames.GetUniqueName(options.ToArray(), "state") });
                        TrySave();
                    }

                    if (hashState && GUILayout.Button("删除"))
                    {
                        m_State.States.RemoveAt(stateIndex);
                        if (stateIndex >= stateCount)
                            stateIndex = stateCount - 1;
                        else if (m_State.States.Count == 0)
                            stateIndex = -1;
                        TrySave();
                    }
                }

                if (hashState)
                    DrawStateInspector();


                if (hashState && GUILayout.Button("增加控制组件", GUILayout.Width(150)))
                {
                    var options = new GUIContent[]
                    {
                        new GUIContent("隐藏组件"),
                        new GUIContent("坐标组件"),
                        new GUIContent("图片组件"),
                        new GUIContent("文本组件"),
                    };
                    var current = Event.current;
                    var mousePosition = current.mousePosition;
                    var width = options.Length * 10;
                    var height = 100;
                    var position = new Rect(mousePosition.x, mousePosition.y - height, width, height);
                    var selected = -1;
                    EditorUtility.DisplayCustomMenu(position, options, selected,
                        delegate(object callBackUserData, string[] callBackOptions, int callBackSelected)
                        {
                            UIState.State callBackState = callBackUserData as UIState.State;
                            string name = callBackOptions[callBackSelected];

                            switch (name)
                            {
                                case "隐藏组件":
                                    callBackState.stateEnable.Add(new UIState.StateEnable() { enable = true });
                                    break;
                                case "坐标组件":
                                    callBackState.statePos.Add(new UIState.StatePos());
                                    break;
                                case "图片组件":
                                    callBackState.stateImage.Add(new UIState.StateImage());
                                    break;
                                case "文本组件":
                                    callBackState.stateTextMesh.Add(new UIState.StateTextMesh());
                                    break;
                            }
                        }, currentState);
                }
            }

            void DrawStateInspector()
            {
                for (int i = 0; i < currentState.stateEnable.Count; i++)
                {
                    var com = currentState.stateEnable[i];
                    com.foldout = EditorGUILayout.Foldout(com.foldout, com.name);
                    if (com.foldout)
                    {
                        EditorGUI.BeginChangeCheck();
                        using (new GUILayout.HorizontalScope())
                        {
                            com.gameObject =
                                (GameObject)EditorGUILayout.ObjectField("组件", com.gameObject, typeof(GameObject), true);
                            if (GUILayout.Button("删除"))
                            {
                                currentState.stateEnable.RemoveAt(i);
                                i--;
                            }
                        }

                        if (com.gameObject)
                        {
                            com.enable = GUILayout.Toggle(com.enable, "激活");
                        }

                        if (EditorGUI.EndChangeCheck())
                        {
                            TrySave();
                        }
                    }
                }

                for (int i = 0; i < currentState.statePos.Count; i++)
                {
                    var com = currentState.statePos[i];
                    com.foldout = EditorGUILayout.Foldout(com.foldout, com.name);
                    if (com.foldout)
                    {
                        EditorGUI.BeginChangeCheck();
                        using (new GUILayout.HorizontalScope())
                        {
                            using (new GUILayout.HorizontalScope()){
                            com.rectTransform =
                                (RectTransform)EditorGUILayout.ObjectField("组件", com.rectTransform, typeof(RectTransform),
                                    true);
                            }
                            if (GUILayout.Button("删除"))
                            {
                                currentState.statePos.RemoveAt(i);
                                i--;
                            }
                        }

                        if (com.rectTransform)
                        {
                            EditorGUI.indentLevel = 1;
                            com.anchoredPosition = EditorGUILayout.Vector3Field("Position",
                                (com.anchoredPosition != Vector3.zero)
                                    ? com.anchoredPosition
                                    : com.rectTransform.anchoredPosition3D);
                            com.sizeDelta = EditorGUILayout.Vector2Field("Width&Height",
                                (com.sizeDelta != Vector2.zero) ? com.sizeDelta : com.rectTransform.sizeDelta);
                            com.pivot = EditorGUILayout.Vector2Field("Pivot", com.rectTransform.pivot);
                            com.roation =
                                EditorGUILayout.Vector3Field("Rotation", com.rectTransform.localRotation.eulerAngles);
                            com.scale = EditorGUILayout.Vector3Field("Scale", com.rectTransform.localScale);
                            com.isSetAnchor = EditorGUILayout.Toggle("设置Anchor", com.isSetAnchor);
                            if (com.isSetAnchor)
                            {
                                com.anchorMin = EditorGUILayout.Vector2Field("Anchor最小值", com.anchorMin);
                                com.anchorMan = EditorGUILayout.Vector2Field("Anchor最大值", com.anchorMan);
                            }
                            EditorGUI.indentLevel = 0;
                        }

                        if (EditorGUI.EndChangeCheck())
                        {
                            TrySave();
                        }
                    }
                }

                for (int i = 0; i < currentState.stateImage.Count; i++)
                {
                    var com = currentState.stateImage[i];
                    com.foldout = EditorGUILayout.Foldout(com.foldout, com.name);
                    if (com.foldout)
                    {
                        EditorGUI.BeginChangeCheck();
                        using (new GUILayout.HorizontalScope())
                        {
                            using (new GUILayout.VerticalScope())
                            {
                                com.image = (Image)EditorGUILayout.ObjectField("组件", com.image, typeof(Image), true);
                                if (Application.isPlaying)
                                    com.uiImage =
                                        (UIImage)EditorGUILayout.ObjectField("组件", com.uiImage, typeof(UIImage), true);
                            }

                            if (GUILayout.Button("删除"))
                            {
                                currentState.stateImage.RemoveAt(i);
                                TrySave();
                                i--;
                            }
                        }

                        if (com.image)
                        {
                            EditorGUI.indentLevel = 1;
                            com.sprite = (Sprite)EditorGUILayout.ObjectField("精灵",
                                (com.sprite != null) ? com.sprite : com.image.sprite, typeof(Sprite), true,
                                GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            com.color = EditorGUILayout.ColorField("颜色", com.color);
                            com.isNativeSize = GUILayout.Toggle(com.isNativeSize, "自适应");
                            EditorGUI.indentLevel = 0;
                        }

                        if (com.uiImage)
                        {
                            EditorGUI.indentLevel = 1;
                            com.spriteName = (string)EditorGUILayout.TextField("精灵",com.spriteName,
                                GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            com.color = EditorGUILayout.ColorField("颜色", com.color);
                            com.isNativeSize = GUILayout.Toggle(com.isNativeSize, "自适应");
                            EditorGUI.indentLevel = 0;
                        }
                        
                        if (EditorGUI.EndChangeCheck())
                        {
                            TrySave();
                        }
                    }
                }

                for (int i = 0; i < currentState.stateTextMesh.Count; i++)
                {
                    var com = currentState.stateTextMesh[i];
                    com.foldout = EditorGUILayout.Foldout(com.foldout, com.name);
                    if (com.foldout)
                    {
                        EditorGUI.BeginChangeCheck();
                        using (new GUILayout.HorizontalScope())
                        {
                            using (new GUILayout.VerticalScope())
                            {
                                com.textMesh =
                                    (TextMeshProUGUI)EditorGUILayout.ObjectField("组件", com.textMesh,
                                        typeof(TextMeshProUGUI), true);
                                if (Application.isPlaying)
                                {
                                    com.uiTextMesh =
                                        (UITextMeshPro)EditorGUILayout.ObjectField("组件", com.uiTextMesh,
                                            typeof(UIText),
                                            true);
                                }
                            }

                            if (GUILayout.Button("删除"))
                            {
                                currentState.stateTextMesh.RemoveAt(i);
                                TrySave();
                                i--;
                            }
                        }

                        if (com.textMesh)
                        {
                            EditorGUI.indentLevel = 1;
                            com.font = (TMP_FontAsset)EditorGUILayout.ObjectField("字体",
                                (com.font != null) ? com.font : com.textMesh.font, typeof(TMP_FontAsset), true);
                            com.material = (Material)EditorGUILayout.ObjectField("样式",
                                (com.material != null) ? com.material : com.textMesh.material, typeof(Material), true);
                            com.enableContent = EditorGUILayout.Toggle("启动文本", com.enableContent);
                            if (com.enableContent)
                            {
                                com.content = EditorGUILayout.TextField("文本",
                                    (!string.IsNullOrEmpty(com.content) ? com.content : com.textMesh.text));
                            }
                            com.color = EditorGUILayout.ColorField("颜色", com.color);
                            com.fontSize = EditorGUILayout.FloatField("大小",
                                (com.fontSize > 0 ? com.fontSize : com.textMesh.fontSize));
                            EditorGUI.indentLevel = 0;
                        }

                        if (com.uiTextMesh)
                        {
                            EditorGUI.indentLevel = 1;
                            com.fontName = EditorGUILayout.TextField("字体", com.fontName);
                            com.materialName = EditorGUILayout.TextField("样式", com.materialName);
                            com.enableContent = EditorGUILayout.Toggle("启动文本", com.enableContent);
                            if (com.enableContent)
                            {
                                com.content = EditorGUILayout.TextField("文本",
                                    (!string.IsNullOrEmpty(com.content) ? com.content : com.textMesh.text));
                            }

                            com.color = EditorGUILayout.ColorField("颜色", com.color);
                            com.fontSize = EditorGUILayout.FloatField("大小",
                                (com.fontSize > 0 ? com.fontSize : com.textMesh.fontSize));
                            EditorGUI.indentLevel = 0;
                        }

                        if (EditorGUI.EndChangeCheck())
                        {
                            TrySave();
                        }
                    }
                }
            }

            public void SetDefault()
            {
                HashSet<GameObject> hash = new HashSet<GameObject>();
                for (int i = 0; i < m_State.States.Count; i++)
                {
                    foreach (var com in m_State.States[i].stateEnable)
                    {
                        if (com.gameObject)
                            hash.Add(com.gameObject);
                    }

                    foreach (var com in m_State.States[i].statePos)
                    {
                        if (com.rectTransform)
                            hash.Add(com.rectTransform.gameObject);
                    }

                    foreach (var com in m_State.States[i].stateImage)
                    {
                        if (com.image)
                            hash.Add(com.image.gameObject);
                    }

                    foreach (var com in m_State.States[i].stateTextMesh)
                    {
                        if (com.textMesh)
                            hash.Add(com.textMesh.gameObject);
                    }
                }

                m_State.Defaults.Clear();
                m_State.Defaults.AddRange(hash);
            }

            void TrySave()
            {
                SetDefault();
                m_State.SetState(stateIndex);
                EditorUtility.SetDirty(m_State);
            }
        }
        bool InPorject(Object asset,out string assetName)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            assetName = Path.GetFileName(path);
            return path.StartsWith("Assets");
        }
#endif
        public void Process()
        {
#if UNITY_EDITOR
            foreach (var state in States)
            {
                foreach (var images in state.stateImage)
                {
                    if (InPorject(images.sprite,out var spriteName))
                    {
                        images.spriteName = spriteName; 
                    }
                    images.sprite = null;
                }
                foreach (var texts in state.stateTextMesh)
                {
                    if (InPorject(texts.font,out var fontName))
                    {
                        texts.fontName = fontName; 
                    }
                    texts.font = null;
                    if (InPorject(texts.material,out var materialName))
                    {
                        texts.materialName = materialName; 
                    }
                    texts.material = null;
                }
            }
#endif
        }
    }


}