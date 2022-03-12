using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Battlehub.RTCommon
{
    public interface IRTEGraphics
    {
        void RegisterCamera(Camera camera);
        void UnregisterCamera(Camera camera);
        IRTECamera GetOrCreateCamera(Camera camera, CameraEvent cameraEvent);
        IRTECamera CreateCamera(Camera camera, CameraEvent cameraEvent, bool meshesCache = false, bool renderersCache = false);

        IMeshesCache CreateSharedMeshesCache(CameraEvent cameraEvent);
        IRenderersCache CreateSharedRenderersCache(CameraEvent cameraEvent);

        void DestroySharedMeshesCache(IMeshesCache cache);
        void DestroySharedRenderersCache(IRenderersCache cache);
    }

    [DefaultExecutionOrder(-60)]
    public class RTEGraphics : MonoBehaviour, IRTEGraphics
    {
        private void Awake()
        {
            IOC.RegisterFallback<IRTEGraphics>(this);
        }

        private void OnDestroy()
        {
            IOC.UnregisterFallback<IRTEGraphics>(this);
        }

        private Dictionary<Camera, Dictionary<CameraEvent, RTECamera>> m_cameras = new Dictionary<Camera, Dictionary<CameraEvent, RTECamera>>();

        private class Data
        {
            public MonoBehaviour MonoBehaviour;
            public List<RTECamera> RTECameras;
            public CameraEvent Event;
            public Data(MonoBehaviour behaviour, CameraEvent cameraEvent, List<RTECamera> cameras)
            {
                MonoBehaviour = behaviour;
                Event = cameraEvent;
                RTECameras = cameras;
            }
        }

        private readonly Dictionary<IMeshesCache, Data> m_meshesCache = new Dictionary<IMeshesCache, Data>();
        private readonly Dictionary<IRenderersCache, Data> m_renderersCache = new Dictionary<IRenderersCache, Data>();

        public void RegisterCamera(Camera camera)
        {
            foreach (KeyValuePair<IMeshesCache, Data> kvp in m_meshesCache)
            {
                IMeshesCache cache = kvp.Key;
                Data data = kvp.Value;
                CreateRTECamera(camera.gameObject, data.Event, cache, data.RTECameras);
            }

            foreach (KeyValuePair<IRenderersCache, Data> kvp in m_renderersCache)
            {
                IRenderersCache cache = kvp.Key;
                Data data = kvp.Value;
                CreateRTECamera(camera.gameObject, data.Event, cache, data.RTECameras);
            }

            if (!m_cameras.ContainsKey(camera))
            {
                m_cameras.Add(camera, new Dictionary<CameraEvent, RTECamera>());
            }
        }

        public void UnregisterCamera(Camera camera)
        {
            foreach (KeyValuePair<IMeshesCache, Data> kvp in m_meshesCache)
            {
                Data data = kvp.Value;
                DestroyRTECameras(camera, data);
            }

            foreach (KeyValuePair<IRenderersCache, Data> kvp in m_renderersCache)
            {
                Data data = kvp.Value;
                DestroyRTECameras(camera, data);
            }

            Dictionary<CameraEvent, RTECamera> rteCameras;
            if(m_cameras.TryGetValue(camera, out rteCameras))
            {
                foreach(IRTECamera rteCamera in rteCameras.Values)
                {
                    rteCamera.Destroy();
                }
                m_cameras.Remove(camera);
            }
        }

        public IRTECamera GetOrCreateCamera(Camera camera, CameraEvent cameraEvent)
        {
            Dictionary<CameraEvent, RTECamera> rteCameras;
            if (!m_cameras.TryGetValue(camera, out rteCameras))
            {
                return null;
            }

            RTECamera rteCamera;
            if(!rteCameras.TryGetValue(cameraEvent, out rteCamera))
            {
                rteCamera = _CreateCamera(camera, cameraEvent, true, true);
                rteCameras.Add(cameraEvent, rteCamera);
            }

            return rteCamera;
        }

        public IRTECamera CreateCamera(Camera camera, CameraEvent cameraEvent, bool createMeshesCache = false, bool createRenderersCache = false)
        {
            return _CreateCamera(camera, cameraEvent, createMeshesCache, createRenderersCache);
        }

        private RTECamera _CreateCamera(Camera camera, CameraEvent cameraEvent, bool createMeshesCache, bool createRenderersCache)
        {
            bool wasActive = camera.gameObject.activeSelf;
            camera.gameObject.SetActive(false);
            RTECamera rteCamera = camera.gameObject.AddComponent<RTECamera>();
            rteCamera.Event = cameraEvent;

            if (createMeshesCache)
            {
                rteCamera.CreateMeshesCache();
            }

            if (createRenderersCache)
            {
                rteCamera.CreateRenderersCache();
            }

            camera.gameObject.SetActive(wasActive);
            return rteCamera;
        }

        public IMeshesCache CreateSharedMeshesCache(CameraEvent cameraEvent)
        {
            MeshesCache cache = gameObject.AddComponent<MeshesCache>();
            cache.RefreshMode = CacheRefreshMode.Manual;

            List<RTECamera> rteCameras = new List<RTECamera>();
            foreach (Camera camera in m_cameras.Keys)
            {
                CreateRTECamera(camera.gameObject, cameraEvent, cache, rteCameras);
            }

            m_meshesCache.Add(cache, new Data(cache, cameraEvent, rteCameras));
            return cache;
        }

        public IRenderersCache CreateSharedRenderersCache(CameraEvent cameraEvent)
        {
            RenderersCache cache = gameObject.AddComponent<RenderersCache>();
            List<RTECamera> rteCameras = new List<RTECamera>();
            foreach (Camera camera in m_cameras.Keys)
            {
                CreateRTECamera(camera.gameObject, cameraEvent, cache, rteCameras);
            }

            m_renderersCache.Add(cache, new Data(cache, cameraEvent, rteCameras));
            return cache;
        }

        public void DestroySharedMeshesCache(IMeshesCache cache)
        {
            Data tuple;
            if (m_meshesCache.TryGetValue(cache, out tuple))
            {
                Destroy(tuple.MonoBehaviour);
                for (int i = 0; i < tuple.RTECameras.Count; ++i)
                {
                    Destroy(tuple.RTECameras[i]);
                }
                m_meshesCache.Remove(cache);
            }
        }

        public void DestroySharedRenderersCache(IRenderersCache cache)
        {
            Data tuple;
            if (m_renderersCache.TryGetValue(cache, out tuple))
            {
                Destroy(tuple.MonoBehaviour);
                for (int i = 0; i < tuple.RTECameras.Count; ++i)
                {
                    Destroy(tuple.RTECameras[i]);
                }
                m_renderersCache.Remove(cache);
            }
        }

        private static void DestroyRTECameras(Camera camera, Data data)
        {
            List<RTECamera> rteCameras = data.RTECameras;
            for (int i = rteCameras.Count - 1; i >= 0; i--)
            {
                RTECamera rteCamera = rteCameras[i];
                if (rteCamera != null && rteCamera.gameObject == camera.gameObject)
                {
                    Destroy(rteCameras[i]);
                    rteCameras.RemoveAt(i);
                }
            }
        }

        private static void CreateRTECamera(GameObject camera, CameraEvent cameraEvent, IMeshesCache cache, List<RTECamera> rteCameras)
        {
            bool wasActive = camera.gameObject.activeSelf;
            camera.SetActive(false);

            RTECamera rteCamera = camera.AddComponent<RTECamera>();
            rteCamera.Event = cameraEvent;
            rteCamera.MeshesCache = cache;
            rteCameras.Add(rteCamera);

            camera.SetActive(wasActive);
        }

        private static void CreateRTECamera(GameObject camera, CameraEvent cameraEvent, IRenderersCache cache, List<RTECamera> rteCameras)
        {
            bool wasActive = camera.gameObject.activeSelf;
            camera.SetActive(false);

            RTECamera rteCamera = camera.AddComponent<RTECamera>();
            rteCamera.Event = cameraEvent;
            rteCamera.RenderersCache = cache;
            rteCameras.Add(rteCamera);

            camera.SetActive(wasActive);
        }


    }

}
