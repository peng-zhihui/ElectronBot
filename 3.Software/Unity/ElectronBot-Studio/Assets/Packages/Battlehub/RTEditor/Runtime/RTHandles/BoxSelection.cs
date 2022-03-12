using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battlehub.RTCommon;

namespace Battlehub.RTHandles
{
    public enum BoxSelectionMethod
    {
        Vertex,
        PixelPerfectDepthTest,
        LooseFitting,
        BoundsCenter,
        TansformCenter,
        Default = Vertex
    }

    public class BeginBoxSelectionArgs : EventArgs
    {
        public bool Cancel
        {
            get;
            set;
        }
    }

    public class FilteringArgs : EventArgs
    {
        private bool m_cancel;

        public bool Cancel
        {
            get { return m_cancel; }
            set
            {
                if (value) //can't reset cancel flag
                {
                    m_cancel = true;
                }
            }
        }

        public GameObject Object
        {
            get;
            set;
        }

        public void Reset()
        {
            m_cancel = false;
        }
    }

    public class BoxSelectionArgs
    {
        public GameObject[] GameObjects;
    }
    

    public interface IBoxSelection
    {
        event EventHandler<BeginBoxSelectionArgs> Begin;
        event EventHandler<FilteringArgs> Filtering;
        event EventHandler<BoxSelectionArgs> Selection;
        bool IsDragging
        {
            get;
        }

        bool IsThresholdPassed
        {
            get;
        }

        Bounds SelectionBounds
        {
            get;
        }

        Canvas Canvas
        {
            get;
        }

        BoxSelectionMethod MethodOverride
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Box Selection
    /// </summary>
    public class BoxSelection : RTEComponent, IBoxSelection
    {
        public Sprite Graphics;
        protected Image m_image;
        protected RectTransform m_rectTransform;
        protected Canvas m_canvas;
        protected bool m_isDragging;
        protected Vector3 m_startMousePosition;
        protected Vector2 m_startPt;
        protected Vector2 m_endPt;

        private Camera m_windowCanvasCamera;
        private RectTransform m_windowRectTransform;

        public Vector2 ScreenSpaceMargin = new Vector2(2, 2);
        public bool UseCameraSpace = false;

        [SerializeField]
        private BoxSelectionMethod m_method = BoxSelectionMethod.Default;
        private BoxSelectionMethod m_methodOverride = BoxSelectionMethod.Default;
        public BoxSelectionMethod MethodOverride
        {
            get
            {
                if(m_methodOverride != BoxSelectionMethod.Default)
                {
                    return m_methodOverride;
                }

                return m_method;
            }
            set
            {
                m_methodOverride = value;
            }
        }

        public event EventHandler<BeginBoxSelectionArgs> Begin;
        public event EventHandler<FilteringArgs> Filtering;
        public event EventHandler<BoxSelectionArgs> Selection;

        public Bounds SelectionBounds
        {
            get;
            private set;
        }

        public bool IsThresholdPassed
        {
            get { return m_rectTransform != null && m_rectTransform.sizeDelta.magnitude > 25f; }
        }

        public bool IsDragging
        {
            get { return m_isDragging; }
        }       


        public Canvas Canvas
        {
            get { return m_canvas; }
        }

        protected override void Awake()
        {
            base.Awake();
            Window.IOCContainer.RegisterFallback<IBoxSelection>(this);
            if (m_canvas == null)
            {
                GameObject go = new GameObject("BoxSelection");
                go.layer = gameObject.layer;
                go.transform.SetParent(transform.parent, false);
                m_canvas = go.AddComponent<Canvas>();
                m_canvas.sortingOrder = 2;
            }
            if (UseCameraSpace)
            {
                m_canvas.worldCamera = Window.Camera;
                m_canvas.renderMode = RenderMode.ScreenSpaceCamera;
                m_canvas.planeDistance = Window.Camera.nearClipPlane + 0.05f;
                m_canvas.transform.rotation = Quaternion.identity;
                m_canvas.transform.position = Vector3.zero;
            }
            else
            {
                m_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            transform.SetParent(m_canvas.gameObject.transform, false);
            transform.rotation = Quaternion.identity;
            transform.position = Vector3.zero;

            CanvasScaler scaler = m_canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = m_canvas.gameObject.AddComponent<CanvasScaler>();
            }
            scaler.referencePixelsPerUnit = 1;
            

            if(!GetComponent<BoxSelectionInput>())
            {
                gameObject.AddComponent<BoxSelectionInput>();
            }

            m_rectTransform = GetComponent<RectTransform>();
            if (m_rectTransform == null)
            {
                m_rectTransform = gameObject.AddComponent<RectTransform>();
            }
            m_rectTransform.sizeDelta = new Vector2(0, 0);
            m_rectTransform.pivot = new Vector2(0, 0);
            m_rectTransform.anchoredPosition = new Vector3(0, 0);

            m_image = gameObject.AddComponent<Image>();
            m_image.type = Image.Type.Sliced;
            if (Graphics == null)
            {
                Graphics = Resources.Load<Sprite>("RTH_BoxSelection");
            }
            m_image.sprite = Graphics;
            m_image.raycastTarget = false;
        }

        protected override void Start()
        {
            base.Start();
        
            m_windowRectTransform = (RectTransform)Window.transform;
            Canvas windowCanvas = m_windowRectTransform.GetComponentInParent<Canvas>();
            m_windowCanvasCamera = windowCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? windowCanvas.worldCamera : null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Window.IOCContainer.UnregisterFallback<IBoxSelection>(this);
            if (Editor != null && Editor.Tools != null && Editor.Tools.ActiveTool == this)
            {
                if (Editor.Tools.ActiveTool == this)
                {
                    Editor.Tools.ActiveTool = null;
                }
            }
        }

        protected override void OnWindowActivated()
        {
            base.OnWindowActivated();
            m_canvas.enabled = true;
        }

        protected override void OnWindowDeactivated()
        {
            m_canvas.enabled = false;

            base.OnWindowDeactivated();
            if (Editor != null && Editor.Tools != null && Editor.Tools.ActiveTool == this)
            {
                if (Editor.Tools.ActiveTool == this)
                {
                    Editor.Tools.ActiveTool = null;
                }
            }
        }

        public void BeginSelect()
        {
            if(Editor.Tools.ActiveTool != null)
            {
                return;
            }

            if(Begin != null)
            {
                BeginBoxSelectionArgs args = new BeginBoxSelectionArgs();
                Begin(this, args);
                if(args.Cancel)
                {
                    return;
                }
            }

            m_startMousePosition = Window.Pointer.ScreenPoint;
            m_isDragging = GetPoint(out m_startPt) && (!Window.Editor.IsOpened || Window.IsPointerOver);
            if (m_isDragging)
            {
                m_rectTransform.anchoredPosition = m_startPt;
                m_rectTransform.sizeDelta = new Vector2(0, 0);
            }
        }

        public void EndSelect()
        {
            if (m_isDragging)
            {
                m_isDragging = false;

                HitTest();
                m_rectTransform.sizeDelta = new Vector2(0, 0);
                if (Editor.Tools.ActiveTool == this)
                {
                    Editor.Tools.ActiveTool = null;
                }
            }
        }

        public Renderer[] Pick(Renderer[] renderers = null, bool filterObjects = true)
        {
            return Pick(renderers, new Bounds(Window.Pointer.ScreenPoint, Vector2.zero), filterObjects);
        }

        private Renderer[] Pick(Renderer[] renderers, Bounds bounds, bool filterObjects = true)
        {
            if (renderers == null)
            {
                renderers = FindObjectsOfType<Renderer>();
            }

            IEnumerable<Renderer> selection = PixelPerfectDepthTest(renderers, bounds);
            if(m_rectTransform != null)
            {
                m_rectTransform.sizeDelta = new Vector2(0, 0);
            }
            
            if (filterObjects)
            {
                FilteringArgs filteringArgs = new FilteringArgs();
                return FilterObjects(filteringArgs, selection).ToArray();
            }

            return selection.ToArray();
        }

        public Color32[] BeginPick(out Vector2Int texSize, Renderer[] renderers = null)
        {
            if (renderers == null)
            {
                renderers = FindObjectsOfType<Renderer>();
            }

            Canvas canvas = Window.GetComponentInParent<Canvas>();
            return BoxSelectionRenderer.Render(Window.Camera, renderers, new Vector2Int(Mathf.RoundToInt(canvas.pixelRect.width), Mathf.RoundToInt(canvas.pixelRect.height)), out texSize);
        }

        public Renderer[] EndPick(Color32[] texPixels, Vector2Int texSize, Renderer[] renderers = null)
        {
            if (renderers == null)
            {
                renderers = FindObjectsOfType<Renderer>();
            }

            Bounds bounds = new Bounds(Window.Pointer.ScreenPoint, Vector2.zero);
            return EndPick(texPixels, texSize, renderers, bounds);
        }

        private Renderer[] EndPick(Color32[] texPixels, Vector2Int texSize, Renderer[] renderers, Bounds bounds)
        {
            Canvas canvas = Window.GetComponentInParent<Canvas>();
            RectTransform sceneOutput = Window.GetComponent<RectTransform>();
            if (sceneOutput.childCount > 0)
            {
                sceneOutput = (RectTransform)Window.GetComponent<RectTransform>().GetChild(0);
            }

            Rect selectionRect = SelectionBoundsToSelectionRect(bounds, canvas, sceneOutput);
            return BoxSelectionRenderer.PickRenderersInRect(Window.Camera, selectionRect, renderers, texPixels, texSize);
        }

        private void Update()
        {
            if (!Editor.Selection.Enabled)
            {
                return;
            }

            if (Editor.Tools.ActiveTool != this && Editor.Tools.ActiveTool != null)
            {
                return;
            }

            if (Editor.Tools.IsViewing || /*Editor.Tools.Current == RuntimeTool.None ||*/ !Editor.Tools.IsBoxSelectionEnabled)
            {
                if(Editor.Tools.ActiveTool == this)
                {
                    Editor.Tools.ActiveTool = null;
                }
                
                m_isDragging = false;
                m_rectTransform.sizeDelta = new Vector2(0, 0);
                return;
            }

            if (m_isDragging)
            {
                GetPoint(out m_endPt);

                Vector2 size = m_endPt - m_startPt;
                Editor.Tools.ActiveTool = this;
                if (size != Vector2.zero)
                {
                    Editor.Tools.ActiveTool = this;
                }
                m_rectTransform.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
                m_rectTransform.localScale = new Vector3(Mathf.Sign(size.x), Mathf.Sign(size.y), 1);
            }
        }

        private void HitTest()
        {
            if (!IsThresholdPassed)
            {
                return;
            }

            Vector3 center = (m_startMousePosition + (Vector3)Window.Pointer.ScreenPoint) / 2;
            center.z = 0.0f;
            Bounds selectionBounds = new Bounds(center, m_rectTransform.sizeDelta);
            SelectionBounds = selectionBounds;

            FilteringArgs filteringArgs = new FilteringArgs();
            HashSet<GameObject> selection;
            Renderer[] renderers = FindObjectsOfType<Renderer>();

            if(MethodOverride == BoxSelectionMethod.PixelPerfectDepthTest)
            {
                selection = new HashSet<GameObject>(FilterObjects(filteringArgs, PixelPerfectDepthTest(renderers, SelectionBounds)).Select(rend => rend.gameObject));
            }
            else
            {
                selection = new HashSet<GameObject>();

                Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Window.Camera);
                for (int i = 0; i < renderers.Length; ++i)
                {
                    Renderer r = renderers[i];
                    Bounds bounds = r.bounds;
                    GameObject go = r.gameObject;
                    TrySelect(ref selectionBounds, selection, filteringArgs, ref bounds, go, frustumPlanes);
                }
            }

            SpriteGizmo[] spriteGizmos = FindObjectsOfType<SpriteGizmo>();
            for (int i = 0; i < spriteGizmos.Length; ++i)
            {
                SpriteGizmo spriteGizmo = spriteGizmos[i];
                bool select = TransformCenter(ref selectionBounds, spriteGizmo.transform);
                if (select)
                {
                    Filter(selection, filteringArgs, spriteGizmo.gameObject);
                }
            }

            if(Selection != null)
            {
                Selection(this, new BoxSelectionArgs { GameObjects = selection.ToArray() });
            }
            else
            {
                Editor.Selection.objects = selection.ToArray();
            }
        }

        private IEnumerable<Renderer> PixelPerfectDepthTest(Renderer[] renderers, Bounds bounds)
        {
            Canvas canvas = Window.GetComponentInParent<Canvas>();

            RectTransform sceneOutput = Window.GetComponent<RectTransform>();
            if (sceneOutput.childCount > 0)
            {
                sceneOutput = (RectTransform)Window.GetComponent<RectTransform>().GetChild(0);
            }

            Rect rect = SelectionBoundsToSelectionRect(bounds, canvas, sceneOutput);
            return BoxSelectionRenderer.PickRenderersInRect(Window.Camera, rect, renderers, Mathf.RoundToInt(canvas.pixelRect.width), Mathf.RoundToInt(canvas.pixelRect.height));
        }

        private Rect SelectionBoundsToSelectionRect(Bounds bounds, Canvas canvas, RectTransform sceneOutput)
        {
            Vector2 min = bounds.min;
            Vector2 max = bounds.max;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(sceneOutput, min, canvas.worldCamera, out min);
            min.y = sceneOutput.rect.height - min.y;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(sceneOutput, max, canvas.worldCamera, out max);
            max.y = sceneOutput.rect.height - max.y;

            /*quick fix for ui scale issue. TODO: replace with better solution*/
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if(scaler != null)
            {
                min *= scaler.scaleFactor;
                max *= scaler.scaleFactor;
            }

            Rect rect = new Rect(new Vector2(Mathf.Min(min.x, max.x), Mathf.Min(min.y, max.y)), new Vector2(Mathf.Max(Mathf.Abs(max.x - min.x), 1), Mathf.Max(Mathf.Abs(max.y - min.y), 1)));
            rect.x += Window.Camera.pixelRect.x;
            rect.y += canvas.pixelRect.height - (Window.Camera.pixelRect.y + Window.Camera.pixelRect.height);
            return rect;
        }

        private HashSet<Renderer> FilterObjects(FilteringArgs filteringArgs, IEnumerable<Renderer> renderers)
        {
            HashSet<Renderer> selection = new HashSet<Renderer>();
            foreach (Renderer rend in renderers)
            {
                if (!selection.Contains(rend))
                {
                    if (Filtering != null)
                    {
                        filteringArgs.Object = rend.gameObject;
                        Filtering(this, filteringArgs);
                        if (!filteringArgs.Cancel)
                        {
                            selection.Add(rend);
                        }
                        filteringArgs.Reset();
                    }
                    else
                    {
                        selection.Add(rend);
                    }
                }
            }

            return selection;
        }

        private void TrySelect(ref Bounds selectionBounds, HashSet<GameObject> selection, FilteringArgs args, ref Bounds bounds, GameObject go, Plane[] frustumPlanes)
        {
            if (!GeometryUtility.TestPlanesAABB(frustumPlanes, bounds))
            {
                return;
            }
            bool select;
            if (MethodOverride == BoxSelectionMethod.LooseFitting)
            {
                select = LooseFitting(ref selectionBounds, ref bounds);              
            }
            else if(MethodOverride == BoxSelectionMethod.Vertex)
            {
                select = LooseFitting(ref selectionBounds, ref bounds);
                if (select && !selection.Contains(go))
                {
                    select = false;
                    MeshFilter meshFilter = go.GetComponent<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        Vector3[] vertices = meshFilter.sharedMesh.vertices;

                        for (int i = 0; i < vertices.Length; ++i)
                        {
                            Vector3 vertex = go.transform.TransformPoint(vertices[i]);
                            vertex = Window.Camera.WorldToScreenPoint(vertex);
                            vertex.z = 0;
                            if (selectionBounds.Contains(vertex))
                            {
                                select = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
                        
                        if (smr != null && smr.sharedMesh != null)
                        {
                            Mesh bakedMesh = new Mesh();
                            smr.BakeMesh(bakedMesh);

                            Matrix4x4 m = Matrix4x4.TRS(go.transform.localPosition, go.transform.localRotation, Vector3.one);
                            if(smr.transform.parent != null)
                            {
                                m = m * smr.transform.parent.localToWorldMatrix;
                            }

                            Vector3[] vertices = bakedMesh.vertices;

                            for (int i = 0; i < vertices.Length; ++i)
                            {
                                Vector3 vertex = m.MultiplyPoint(vertices[i]);
                                vertex = Window.Camera.WorldToScreenPoint(vertex);
                                vertex.z = 0;
                                if (selectionBounds.Contains(vertex))
                                {
                                    select = true;
                                    break;
                                }
                            }

                           Destroy(bakedMesh);
                        }
                    }
                    
                }
            }
            else if (MethodOverride == BoxSelectionMethod.BoundsCenter)
            {
                select = BoundsCenter(ref selectionBounds, ref bounds);
            }
            else
            {
                select = TransformCenter(ref selectionBounds, go.transform);
            }

            if (select)
            {
                Filter(selection, args, go);
            }
        }

        private void Filter(HashSet<GameObject> selection, FilteringArgs args, GameObject go)
        {
            if (!selection.Contains(go))
            {
                if (Filtering != null)
                {
                    args.Object = go;
                    Filtering(this, args);
                    if (!args.Cancel)
                    {
                        selection.Add(go);
                    }
                    args.Reset();
                }
                else
                {
                    selection.Add(go);
                }
            }
        }

        private bool TransformCenter(ref Bounds selectionBounds, Transform tr)
        {
            Vector3 screenPoint = Window.Camera.WorldToScreenPoint(tr.position);
            screenPoint.z = 0;
            return selectionBounds.Contains(screenPoint);
        }

        private bool BoundsCenter(ref Bounds selectionBounds, ref Bounds bounds)
        {
            Vector3 screenPoint = Window.Camera.WorldToScreenPoint(bounds.center);
            screenPoint.z = 0;
            return selectionBounds.Contains(screenPoint);
        }

        private bool LooseFitting(ref Bounds selectionBounds, ref Bounds bounds)
        {
            Vector3 p0 = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
            Vector3 p1 = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z);
            Vector3 p2 = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z);
            Vector3 p3 = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z);
            Vector3 p4 = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
            Vector3 p5 = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z);
            Vector3 p6 = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z);
            Vector3 p7 = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z);

            p0 = Window.Camera.WorldToScreenPoint(p0);
            p1 = Window.Camera.WorldToScreenPoint(p1);
            p2 = Window.Camera.WorldToScreenPoint(p2);
            p3 = Window.Camera.WorldToScreenPoint(p3);
            p4 = Window.Camera.WorldToScreenPoint(p4);
            p5 = Window.Camera.WorldToScreenPoint(p5);
            p6 = Window.Camera.WorldToScreenPoint(p6);
            p7 = Window.Camera.WorldToScreenPoint(p7);

            float minX = Mathf.Min(p0.x, p1.x, p2.x, p3.x, p4.x, p5.x, p6.x, p7.x);
            float maxX = Mathf.Max(p0.x, p1.x, p2.x, p3.x, p4.x, p5.x, p6.x, p7.x);
            float minY = Mathf.Min(p0.y, p1.y, p2.y, p3.y, p4.y, p5.y, p6.y, p7.y);
            float maxY = Mathf.Max(p0.y, p1.y, p2.y, p3.y, p4.y, p5.y, p6.y, p7.y);
            Vector3 min = new Vector2(minX, minY);
            Vector3 max = new Vector2(maxX, maxY);

            Bounds b = new Bounds((min + max) / 2, (max - min));
            return selectionBounds.Intersects(b);
        }

        private bool GetPoint(out Vector2 localPoint)
        {
            Camera cam = null;
            if(m_canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                cam = m_canvas.worldCamera;
            }

            Vector3 screenPoint;
            if (UseCameraSpace)
            {
                screenPoint = Window.Pointer.ScreenPoint;
            }
            else
            {
                screenPoint = Window.Editor.Input.GetPointerXY(0);
                Rect rect = m_windowRectTransform.rect;

                Vector2 min = RectTransformUtility.WorldToScreenPoint(m_windowCanvasCamera, m_windowRectTransform.TransformPoint(rect.min)) + ScreenSpaceMargin;
                Vector2 max = RectTransformUtility.WorldToScreenPoint(m_windowCanvasCamera, m_windowRectTransform.TransformPoint(rect.max)) - ScreenSpaceMargin;

                screenPoint.x = Mathf.Clamp(screenPoint.x, min.x, max.x);
                screenPoint.y = Mathf.Clamp(screenPoint.y, min.y, max.y);
            }

            return RectTransformUtility.ScreenPointToLocalPointInRectangle(m_canvas.GetComponent<RectTransform>(), screenPoint, cam, out localPoint);
        }
    }

}
