using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Alurax
{
    public class UIRawImage : RawImage
    {
        [SerializeField]
        string m_SpriteName;
        [SerializeField]
        string m_MaterialName;
  
        private Sprite m_CacheSprite;
        private Material m_CacheMaterial;
        [NonSerialized] public bool isNativeSize;
        public System.Action OnLoaded;
        public System.Action OnMatLoaded;
        public string SpriteName
        {
            get { return m_SpriteName; }
            set
            {
                if (m_SpriteName != value)
                {
                    Assets.ReleaseAsset(m_SpriteName);
                    m_SpriteName = value;
                    SetSprite();
                }
                else
                {
                    if(texture!=null && texture.name == GetNameWithoutExtension(m_SpriteName))
                        OnLoaded?.Invoke();
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
                else
                {
                    if(material!=null && material.name == GetNameWithoutExtension(m_MaterialName))
                        OnMatLoaded?.Invoke();
                }
            }
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            TryLoadSprite();
        }

        protected override void OnDisable()
        {
            TryReleaseSprite();
            base.OnDisable();
        }

        void TryLoadSprite()
        {
            if(Application.isPlaying)
            {
                if (!string.IsNullOrEmpty(m_SpriteName) && ((this.texture != null && GetNameWithoutExtension(m_SpriteName) != this.texture.name ) || ( this.texture == null)))
                {
                    SetSprite();
                }
                if (!string.IsNullOrEmpty(m_MaterialName) && ((GetNameWithoutExtension(m_MaterialName) != this.material.name )))
                {
                   SetMaterial();
                }
            }
        }
        
        void TryReleaseSprite()
        {
            if (!string.IsNullOrEmpty(m_SpriteName) && (this.texture != null && GetNameWithoutExtension(m_SpriteName) == this.texture.name))
            {
                Assets.ReleaseAsset(m_SpriteName);
                this.texture = UGUIHelper.EmptySprite.texture;
            }
            
            if (!string.IsNullOrEmpty(m_MaterialName) && ((GetNameWithoutExtension(m_MaterialName) == this.material.name )))
            {
                Assets.ReleaseAsset(m_MaterialName);
                this.material = null;
            }
        }
        
        void SetSprite()
        {
            if (!this.IsActive()) return;
            if (!string.IsNullOrEmpty(m_SpriteName))
            {
                var loadSpriteName = m_SpriteName;
                Assets.LoadAssetAsync<Sprite>(loadSpriteName, (s) =>
                {
                    if (this && loadSpriteName == m_SpriteName)
                    {
                        m_CacheSprite = s;
                        TryRefresh();
                    }
                    else
                    {
                        Assets.ReleaseAsset(loadSpriteName);
                    }
                });
            }
            else
            {
                TryRefresh();
            }
        }

        void SetMaterial()
        {
            if (!this.IsActive()) return;
            if (!string.IsNullOrEmpty(m_MaterialName))
            {
                var loadMaterialName = m_MaterialName;
                Assets.LoadAssetAsync<Material>(loadMaterialName, (m) =>
                {
                    if (this && loadMaterialName == m_MaterialName)
                    {
                        m_CacheMaterial = m;
                        TryRefresh();
                    }
                    else
                    {
                        Assets.ReleaseAsset(loadMaterialName);
                    }
                });
            }
            else
            {
                TryRefresh(); 
            }
        }

        void TryRefresh()
        {
            if (m_CacheSprite != null && !string.IsNullOrEmpty(m_SpriteName))
            {
                this.texture = m_CacheSprite.texture;
                if(isNativeSize) this.SetNativeSize();
                OnLoaded?.Invoke();
            }

            if (m_CacheMaterial != null && !string.IsNullOrEmpty(m_MaterialName))
            {
                this.material = m_CacheMaterial;
                OnMatLoaded?.Invoke();
            }

            if (string.IsNullOrEmpty(m_SpriteName))
            {
                this.texture = null;
                OnLoaded?.Invoke();
            }
            
            if (string.IsNullOrEmpty(m_MaterialName))
            {
                this.material = null;
                OnMatLoaded?.Invoke();
            }
        }
        
        string GetNameWithoutExtension(string name)
        {
            return Path.GetFileNameWithoutExtension(name);
        }
    }
}