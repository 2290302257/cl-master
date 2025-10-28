using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Alurax
{
    public class UITextMeshPro : TextMeshProUGUI
    {
        [SerializeField]
        string m_FontName;
        [SerializeField]
        string m_MaterialName;

        [SerializeField] private LangLibraryData m_lang = new LangLibraryData();
        
        
        private TMP_FontAsset m_CacheFont;
        private Material m_CacheMaterial;
        private bool m_Awaked = false;
        public System.Action<UITextMeshPro> OnLoaded;
        
        public bool IsLoaded { get; private set; }
        
        public string FontName
        {
            get
            {
                return m_FontName;
            }
            set
            {
                if (m_FontName != value)
                {
                    Assets.ReleaseAsset(m_FontName);
                    m_FontName = value;
                    SetFontName();
                }
            }
        }
        public string MaterialName
        {
            get { return m_MaterialName; }
            set
            {
                if (m_MaterialName != value)
                {
                    Assets.ReleaseAsset(m_MaterialName);
                    m_MaterialName = value;
                    SetMaterial();
                }
            }
        }


        private Material _copyMaterial = null;
        public Material CopyMaterial 
        {
            get
            {
                if (_copyMaterial == null)
                {
                    _copyMaterial = new Material(this.fontMaterial);
                    this.fontMaterial = _copyMaterial;
                }
                return  this.fontMaterial;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            if (!string.IsNullOrEmpty(m_lang.key))
            {
                this.text = m_lang.library.GetText(m_lang.key);
            }
        }

        protected override void OnEnable()
        {
            if(m_Awaked)
                base.OnEnable();

            TryLoadText();
        }
        protected override void OnDisable()
        {
            TryReleaseFont();
            base.OnDisable();
        }
        
        void TryReleaseFont()
        {
            if (!string.IsNullOrEmpty(m_FontName) && (this.font != null && GetNameWithoutExtension(m_FontName) == this.font.name))
            {
                Assets.ReleaseAsset(m_FontName);
                this.font = null;
            }
            if (!string.IsNullOrEmpty(m_MaterialName) && ((GetNameWithoutExtension(m_MaterialName) == this.material.name )))
            {
                Assets.ReleaseAsset(m_MaterialName);
                this.material = null;
            }
        }
        void TryLoadText()
        {
            if(Application.isPlaying)
            {
                if (!string.IsNullOrEmpty(m_FontName) && ((this.font != null && GetNameWithoutExtension(m_FontName) != this.font.name) || ( this.font == null)))
                {
                    SetFontName();
                }
                if (!string.IsNullOrEmpty(m_MaterialName) && ((GetNameWithoutExtension(m_MaterialName) != this.material.name )))
                {
                    SetMaterial();
                }
            }
        }
               
        void SetFontName()
        {
            IsLoaded = false;
            if (!string.IsNullOrEmpty(m_FontName))
            {
                var loadFontName = m_FontName;
                if (!Assets.HasAsset(loadFontName))
                {
                    Debug.LogError($"Font {loadFontName} is not found in {this.gameObject.GetScenePath()}", this.gameObject);
                }
                Assets.LoadAssetAsync<TMP_FontAsset>(loadFontName, (f) =>
                {
                    if (this && loadFontName == m_FontName)
                    {
                        m_CacheFont = f;
                        TryRefresh();
                    }
                    else
                    {
                        Assets.ReleaseAsset(loadFontName);
                    }
                });
            }
        }

        void SetMaterial()
        {
            IsLoaded = false;
            if (!string.IsNullOrEmpty(m_MaterialName))
            {
                if (Path.GetExtension(m_MaterialName) == ".mat")
                    InternalSetMaterial<Material>();
                else
                    InternalSetMaterial<TMP_Asset>();
            }
        }

        void InternalSetMaterial<T>()  where T : Object
        {
            if (!string.IsNullOrEmpty(m_MaterialName))
            {
                string loadMaterialName = m_MaterialName;
                Assets.LoadAssetAsync<T>(loadMaterialName, (m) =>
                {
                    if (this && loadMaterialName == m_MaterialName)
                    {
                        if (m is Material mat)
                            m_CacheMaterial = mat;
                        else if (m is TMP_Asset tmp)
                            m_CacheMaterial = tmp.material;
                        TryRefresh();
                    }
                    else
                    {
                        Assets.ReleaseAsset(loadMaterialName);
                    }
                });
            }
        }
        
        void TryRefresh()
        {
            if (m_CacheFont != null)
            {
                if (!m_Awaked)
                {
                    m_Awaked = true;
                    base.Awake();
                    base.OnEnable();
                }
              
                this.font = m_CacheFont;
                if (m_CacheMaterial != null)
                    this.fontMaterial = m_CacheMaterial;
                IsLoaded = true;
                OnLoaded?.Invoke(this);
            }
        }
        string GetNameWithoutExtension(string name)
        {
            return Path.GetFileNameWithoutExtension(name);
        }
    }
}