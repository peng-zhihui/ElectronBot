using System;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.Utils
{
    public class TakeSnapshot : MonoBehaviour
    {
        public GameObject CameraPrefab;
        public GameObject TargetPrefab;
        public GameObject FallbackPrefab;
        public Vector3 Scale = new Vector3(0.9f, 0.9f, 0.9f);
        public Image TargetImage;
        private Texture2D m_texture = null;

        private void Start()
        {
            if (TargetPrefab == null)
            {
                return;
            }

            Run();
        }

        private void OnDestroy()
        {
            if(m_texture != null)
            {
                Destroy(m_texture);
            }
        }

        public Sprite Run()
        {
            GameObject objectToTextureCamera;
            if (CameraPrefab != null)
            {
                objectToTextureCamera = Instantiate(CameraPrefab);
            }
            else
            {
                objectToTextureCamera = new GameObject();
            }

            if (!objectToTextureCamera.GetComponent<Camera>())
            {
                Camera cam = objectToTextureCamera.AddComponent<Camera>();
                cam.orthographic = true;
                cam.orthographicSize = 1;
            }

            ObjectToTexture otot = objectToTextureCamera.GetComponent<ObjectToTexture>();
            if (otot == null)
            {
                otot = objectToTextureCamera.AddComponent<ObjectToTexture>();
            }
            otot.defaultScale = Scale;

            if(m_texture != null)
            {
                Destroy(m_texture);
            }

            m_texture = otot.TakeObjectSnapshot(TargetPrefab, FallbackPrefab);
            Sprite sprite = null;
            if (m_texture != null)
            {
                sprite = Sprite.Create(m_texture, new Rect(0, 0, m_texture.width, m_texture.height), new Vector2(0.5f, 0.5f));
                if (TargetImage != null)
                {
                    TargetImage.sprite = sprite;
                }
            }

            Destroy(objectToTextureCamera);
            return sprite;
        }
    }
}
