using UnityEngine;
using System.Collections.Generic;

namespace Battlehub.Utils
{
    public enum KnownCursor
    {
        VResize,
        HResize,
        DropNotAllowed,
        DropAllowed
    }

    public class CursorHelper
    {
        private object m_locker;
        private Texture2D m_texture;

        private readonly Dictionary<KnownCursor, Texture2D> m_knownCursorToTexture = new Dictionary<KnownCursor, Texture2D>();
        public void Map(KnownCursor cursorType, Texture2D texture)
        {
            m_knownCursorToTexture[cursorType] = texture;
        }

        private Texture2D m_defaultCursorTexture;
        private Vector2 m_defaultCursorHotspot;
        public Texture2D DefaultCursorTexture
        {
            get 
            {
                return m_defaultCursorTexture;
            }
        }
        public Vector2 DefaultCursorHotspot
        {
            get 
            {
                return m_defaultCursorHotspot;
            }
        }
        public void SetDefaultCursor(Texture2D texture, Vector2 hotspot)
        {
            m_defaultCursorTexture = texture;
            m_defaultCursorHotspot = hotspot;
            ResetCursor(null);
        }

        public void Reset()
        {
            m_knownCursorToTexture.Clear();
        }

        public void SetCursor(object locker, KnownCursor cursorType)
        {
            SetCursor(locker, cursorType, new Vector2(0.5f, 0.5f), CursorMode.Auto);
        }

        public void SetCursor(object locker, KnownCursor cursorType, Vector2 hotspot, CursorMode mode)
        {
            Texture2D texture;
            if(!m_knownCursorToTexture.TryGetValue(cursorType, out texture))
            {
                texture = null;
            }
            SetCursor(locker, texture, hotspot, mode);
        }

        public void SetCursor(object locker, Texture2D texture)
        {
            SetCursor(locker, texture, new Vector2(0.5f, 0.5f), CursorMode.Auto);
        }

        public void SetCursor(object locker, Texture2D texture, Vector2 hotspot, CursorMode mode)
        {
            if (m_locker != null && m_locker != locker)
            {
                return;
            }

            if(texture != null)
            {
                hotspot = new Vector2(texture.width * hotspot.x, texture.height * hotspot.y);
            } 
            else 
            {
                texture = DefaultCursorTexture;
                if(texture != null)
                {
                    hotspot = new Vector2(texture.width * DefaultCursorHotspot.x, texture.height * DefaultCursorHotspot.y);
                }
            }

            m_locker = locker;
            if(m_texture != texture)
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                Cursor.SetCursor(texture, hotspot, mode);
                m_texture = texture;
            }
        }

        public void ResetCursor(object locker)
        {            
            if (m_locker != locker)
            {
                return;
            }
            m_locker = null;

            SetCursor(null, DefaultCursorTexture, DefaultCursorHotspot, CursorMode.Auto);
        }
    }

}
