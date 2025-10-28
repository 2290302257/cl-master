using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Alurax
{
    public class UIText : Text
    {
        [SerializeField]
        string m_FontName;
        [SerializeField]
        string m_MaterialName;

        private bool m_IsFontLoading = false;
        private bool m_IsMaterialLoading = false;
        private Font m_CacheFont;
        private Material m_CacheMaterial;
        public System.Action OnLoaded;
        
        public string FoneName
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
        
        protected override void OnEnable()
        {
            base.OnEnable();
            TryLoadText();
        }
        
        protected override void OnDestroy()
        {
            if (!m_IsFontLoading)
            {
                Assets.ReleaseAsset(m_FontName);
            }
            if (!string.IsNullOrEmpty(m_MaterialName) && !m_IsMaterialLoading)
            {
                Assets.ReleaseAsset(m_MaterialName);
            }
            base.OnDestroy();
        } 

        void TryLoadText()
        {
            if(Application.isPlaying)
            {
                if (!string.IsNullOrEmpty(m_FontName) && (!m_IsFontLoading || this.font == null))
                {
                    SetFontName();
                }
                if ((!m_IsMaterialLoading && !string.IsNullOrEmpty(m_MaterialName)))
                {
                    SetMaterial();
                }
            }
        }
               
        void SetFontName()
        {
            m_IsFontLoading = true;
            Assets.LoadAssetAsync<Font>(m_FontName, (f) =>
            {
                if (this)
                {
                    m_IsFontLoading = false;
                    m_CacheFont = f;
                    TryRefresh();
                }
                else
                {
                    Assets.ReleaseAsset(m_FontName);
                }
            });
        }

        void SetMaterial()
        {
            m_IsMaterialLoading= true;
            Assets.LoadAssetAsync<Material>(m_MaterialName, (m) =>
            {
                if (this)
                {
                    m_IsMaterialLoading = false;
                    m_CacheMaterial = m;
                    TryRefresh();
                }
                else
                {
                    Assets.ReleaseAsset(m_MaterialName);
                }
            });
        }

        void TryRefresh()
        {
            if (m_CacheFont != null)
            {
                this.font = m_CacheFont;
                if(m_CacheMaterial != null)
                    this.material = m_CacheMaterial;
                OnLoaded?.Invoke();
            }
        }
    }
}