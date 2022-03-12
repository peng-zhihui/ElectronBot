using UnityEngine;


namespace Battlehub.Utils
{
    /// <summary>
    /// http://crappycoding.com/2014/12/create-gameobject-image-using-render-textures/
    /// </summary>
    public class ObjectToTexture : MonoBehaviour
    {
        // Use this for initialization
        public Camera Camera;
        [HideInInspector]
        public int objectImageLayer;

        public bool DestroyScripts = true;
        public int snapshotTextureWidth = 128;
        public int snapshotTextureHeight = 128;
        public Vector3 defaultPosition = new Vector3(0, 0, 0);
        public Vector3 defaultRotation = new Vector3(26, 135, -24);
        public Vector3 defaultScale = new Vector3(1, 1, 1);

        private void Awake()
        {
            if(Camera == null)
            {
                Camera = GetComponent<Camera>();
            }
            Camera.enabled = false;
        }

        private void SetLayerRecursively(GameObject o, int layer)
        {
            foreach (Transform t in o.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.layer = layer;
            }
        }

        public Texture2D TakeObjectSnapshot(GameObject prefab, GameObject fallback)
        {
            return TakeObjectSnapshot(prefab, fallback, defaultPosition, Quaternion.Euler(defaultRotation), defaultScale);
        }

        public Texture2D TakeObjectSnapshot(GameObject prefab, GameObject fallback, Vector3 position)
        {
            return TakeObjectSnapshot(prefab, fallback, position, Quaternion.Euler(defaultRotation), defaultScale);
        }

        public Texture2D TakeObjectSnapshot(GameObject prefab, GameObject fallback, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            // validate properties
            if (Camera == null)
            {
                throw new System.InvalidOperationException("Object Image Camera must be set");
            }
                
            if (objectImageLayer < 0 || objectImageLayer > 31)
            {
                throw new System.InvalidOperationException("Object Image Layer must specify a valid layer between 0 and 31");
            }

   
            // clone the specified game object so we can change its properties at will, and position the object accordingly
            bool isActive = prefab.activeSelf;
            prefab.SetActive(false);

            GameObject go = Instantiate(prefab, position, rotation * Quaternion.Inverse(prefab.transform.rotation)) as GameObject;
            if(DestroyScripts)
            {
                MonoBehaviour[] scripts = go.GetComponentsInChildren<MonoBehaviour>(true);
                for (int i = 0; i < scripts.Length; ++i)
                {   
                    if(scripts[i] == null)
                    {
                        continue;
                    }

                    if(scripts[i].GetType().FullName.StartsWith("UnityEngine"))
                    {
                        continue;
                    }
                    DestroyImmediate(scripts[i]);
                }
            }

            prefab.SetActive(isActive);
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>(false);
            if(renderers.Length == 0 )
            {
                if(fallback != null)
                {
                    DestroyImmediate(go);
                    go = Instantiate(fallback, position, rotation) as GameObject;
                    renderers = new[] { fallback.GetComponentInChildren<Renderer>(false) };
                }
            }
            Texture2D texture = null;
            if (renderers.Length != 0)
            {
                Bounds bounds = go.CalculateBounds();
                float fov = Camera.fieldOfView * Mathf.Deg2Rad;
                float objSize = Mathf.Max(bounds.extents.y, bounds.extents.x, bounds.extents.z);
                float distance = Mathf.Abs(objSize / Mathf.Sin(fov / 2.0f));

                go.SetActive(true);
                for (int i = 0; i < renderers.Length; ++i)
                {
                    renderers[i].gameObject.SetActive(true);
                }

                position += bounds.center;

                Camera.transform.position = position - distance * Camera.transform.forward;
                Camera.orthographicSize = objSize;

                // set the layer so the render to texture camera will see the object 
                SetLayerRecursively(go, objectImageLayer);

                // get a temporary render texture and render the camera
                Camera.targetTexture = RenderTexture.GetTemporary(snapshotTextureWidth, snapshotTextureHeight, 24);
                Camera.enabled = true;
                Camera.Render();
                Camera.enabled = false;
                // activate the render texture and extract the image into a new texture
                RenderTexture saveActive = RenderTexture.active;
                RenderTexture.active = Camera.targetTexture;
                texture = new Texture2D(Camera.targetTexture.width, Camera.targetTexture.height);
                texture.ReadPixels(new Rect(0, 0, Camera.targetTexture.width, Camera.targetTexture.height), 0, 0);
                texture.Apply();

                RenderTexture.active = saveActive;

                // clean up after ourselves
                RenderTexture.ReleaseTemporary(Camera.targetTexture);
            }
           
            DestroyImmediate(go);
            return texture;
        }
    }
}
