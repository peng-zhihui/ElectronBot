using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Battlehub.Utils;
using Battlehub.RTCommon;
using System;

using UnityObject = UnityEngine.Object;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Battlehub.RTHandles
{
    public enum FocusMode
    {
        Selected,
        AllActive,
        Default = Selected
    }

    public interface IScenePivot
    {
        Vector3 Pivot
        {
            get;
            set;
        }

        Vector3 SecondaryPivot
        {
            get;
            set;
        }

        Vector3 CameraPosition
        {
            get;
            set;
        }

        bool IsOrthographic
        {
            get;
            set;
        }

        float OrthographicSize
        {
            get;
            set;
        }

        [Obsolete]
        void Focus();
        void Focus(FocusMode mode = FocusMode.Default);
        void Focus(Vector3 objPosition, float objSize);
    }

    public class RuntimeSelectionChangingArgs : EventArgs
    {
        public bool Cancel
        {
            get;
            set;
        }

        public IList<UnityObject> Selected
        {
            get;
            private set;
        }

        public RuntimeSelectionChangingArgs(IEnumerable<UnityObject> selected)
        {
            Selected = selected.ToList();
        }
    }

    public class RuntimeSelectionFilteringArgs : EventArgs
    {
        public IList<RaycastHit> Hits
        {
            get;
            private set;
        }

        public RuntimeSelectionFilteringArgs(IEnumerable<RaycastHit> hits)
        {
            Hits = hits.ToList();
        }
    }

    public interface IRuntimeSelectionComponent : IScenePivot
    {
        event EventHandler<RuntimeSelectionFilteringArgs> Filtering;
        event EventHandler<RuntimeSelectionChangingArgs> SelectionChanging;
        event EventHandler SelectionChanged;

        PositionHandle PositionHandle
        {
            get;
        }

        RotationHandle RotationHandle
        {
            get;
        }

        ScaleHandle ScaleHandle
        {
            get;
        }

        RectTool RectTool
        {
            get;
        }

        BaseHandle CustomHandle
        {
            get;
            set;
        }

        BoxSelection BoxSelection
        {
            get;
        }

        bool IsPositionHandleEnabled
        {
            get;
            set;
        }

        bool IsRotationHandleEnabled
        {
            get;
            set;
        }

        bool IsScaleHandleEnabled
        {
            get;
            set;
        }

        bool IsRectToolEnabled
        {
            get;
            set;
        }

        bool IsBoxSelectionEnabled
        {
            get;
            set;
        }

        bool IsSelectionVisible
        {
            get;
            set;
        }

        bool CanSelect
        {
            get;
            set;
        }

        bool CanSelectAll
        {
            get;
            set;
        }

        float SizeOfGrid
        {
            get;
            set;
        }

        bool IsGridVisible
        {
            get;
            set;
        }

        bool IsGridEnabled
        {
            get;
            set;
        }

        bool GridZTest
        {
            get;
            set;
        }

        RuntimeWindow Window
        {
            get;
        }

        IRuntimeSelection Selection
        {
            get;
            set;
        }

        Transform[] GetHandleTargets();
    }

    [DefaultExecutionOrder(-55)]
    public class RuntimeSelectionComponent : RTEComponent, IRuntimeSelectionComponent
    {
        public event EventHandler<RuntimeSelectionFilteringArgs> Filtering;
        public event EventHandler<RuntimeSelectionChangingArgs> SelectionChanging;
        public event EventHandler SelectionChanged;

        [SerializeField]
        private OutlineManager m_outlineManager = null;
        [SerializeField]
        private PositionHandle m_positionHandle = null;
        [SerializeField]
        private RotationHandle m_rotationHandle = null;
        [SerializeField]
        private ScaleHandle m_scaleHandle = null;
        [SerializeField]
        private RectTool m_rectTool = null;
        [SerializeField]
        private BaseHandle m_customHandle = null;
        [SerializeField]
        private BoxSelection m_boxSelection = null;
        [SerializeField]
        private SceneGrid m_grid = null;
        [SerializeField]
        private Transform m_pivot = null;
        [SerializeField]
        private Transform m_secondaryPivot = null;
        //[SerializeField]
        private bool m_isPositionHandleEnabled = true;
        //[SerializeField]
        private bool m_isRotationHandleEnabled = true;
        //[SerializeField]
        private bool m_isScaleHandleEnabled = true;
        //[SerializeField]
        private bool m_isRectToolEnabled = true;
        //[SerializeField]
        private bool m_isBoxSelectionEnabled = true;
        //[SerializeField]
        private bool m_isSelectionVisible = true;
        [SerializeField]
        private bool m_canSelect = true;
        [SerializeField]
        private bool m_canSelectAll = true;
        [SerializeField]
        private bool m_canSelectExposedOnly = true;

        protected Transform PivotTransform
        {
            get { return m_pivot; }
        }

        protected Transform SecondaryPivotTransform
        {
            get { return m_secondaryPivot; }
        }

        public virtual bool IsOrthographic
        {
            get { return Window.Camera.orthographic; }
            set { Window.Camera.orthographic = value; }
        }

        public virtual float OrthographicSize
        {
            get { return Window.Camera.orthographicSize; }
            set { Window.Camera.orthographicSize = value; }
        }

        public virtual Vector3 CameraPosition
        {
            get { return Window.Camera.transform.position; }
            set
            {
                Window.Camera.transform.position = value;
                Window.Camera.transform.LookAt(Pivot);
            }
        }

        public virtual Vector3 Pivot
        {
            get { return m_pivot.transform.position; }
            set
            {
                m_pivot.transform.position = value;
                Window.Camera.transform.LookAt(Pivot);
            }
        }

        public virtual Vector3 SecondaryPivot
        {
            get { return m_secondaryPivot.transform.position; }
            set { m_secondaryPivot.transform.position = value; }
        }

        public BoxSelection BoxSelection
        {
            get { return m_boxSelection; }
        }

        public PositionHandle PositionHandle
        {
            get { return m_positionHandle; }
        }

        public RotationHandle RotationHandle
        {
            get { return m_rotationHandle; }
        }

        public ScaleHandle ScaleHandle
        {
            get { return m_scaleHandle; }
        }

        public RectTool RectTool
        {
            get { return m_rectTool; }
        }

        public BaseHandle CustomHandle
        {
            get { return m_customHandle; }
            set
            {
                if (m_customHandle == value)
                {
                    return;
                }

                if (m_customHandle != null)
                {
                    m_customHandle.BeforeDrag.RemoveListener(OnBeforeDrag);
                    m_customHandle.Drop.RemoveListener(OnDrop);
                }

                m_customHandle = value;

                if (m_customHandle != null)
                {
                    m_customHandle.Window = Window;
                    m_customHandle.gameObject.SetActive(false);

                    if (m_customHandle.BeforeDrag == null)
                    {
                        m_customHandle.BeforeDrag = new BaseHandleUnityEvent();
                    }
                    m_customHandle.BeforeDrag.AddListener(OnBeforeDrag);

                    if (m_customHandle.Drop == null)
                    {
                        m_customHandle.Drop = new BaseHandleUnityEvent();
                    }
                    m_customHandle.Drop.AddListener(OnDrop);

                    if (Editor.Tools.Current == RuntimeTool.Custom)
                    {
                        Transform[] targets = GetHandleTargets();
                        if (targets != null && targets.Length > 0)
                        {
                            m_customHandle.Targets = targets;
                            m_customHandle.gameObject.SetActive(true);
                        }
                        else
                        {
                            m_customHandle.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        public SceneGrid Grid
        {
            get { return m_grid; }
        }

        public bool IsPositionHandleEnabled
        {
            get { return m_isPositionHandleEnabled && m_positionHandle != null; }
            set
            {
                m_isPositionHandleEnabled = value;
                if (m_positionHandle != null)
                {
                    if (value && Editor.Tools.Current == RuntimeTool.Move)
                    {
                        m_positionHandle.Targets = GetHandleTargets();
                    }
                    m_positionHandle.gameObject.SetActive(value && Editor.Tools.Current == RuntimeTool.Move && m_positionHandle.Target != null);
                }
            }
        }

        public bool IsRotationHandleEnabled
        {
            get { return m_isRotationHandleEnabled && m_rotationHandle != null; }
            set
            {
                m_isRotationHandleEnabled = value;
                if (m_rotationHandle != null)
                {
                    if (value && Editor.Tools.Current == RuntimeTool.Rotate)
                    {
                        m_rotationHandle.Targets = GetHandleTargets();
                    }
                    m_rotationHandle.gameObject.SetActive(value && Editor.Tools.Current == RuntimeTool.Rotate && m_rotationHandle.Target != null);
                }
            }
        }

        public bool IsScaleHandleEnabled
        {
            get { return m_isScaleHandleEnabled && m_scaleHandle != null; }
            set
            {
                m_isScaleHandleEnabled = value;
                if (m_scaleHandle != null)
                {
                    if (value && Editor.Tools.Current == RuntimeTool.Scale)
                    {
                        m_scaleHandle.Targets = GetHandleTargets();
                    }
                    m_scaleHandle.gameObject.SetActive(value && Editor.Tools.Current == RuntimeTool.Scale && m_scaleHandle.Target != null);
                }
            }
        }

        public bool IsRectToolEnabled
        {
            get { return m_isRectToolEnabled && m_rectTool != null; }
            set
            {
                m_isRectToolEnabled = value;
                if (m_rectTool != null)
                {
                    if (value && Editor.Tools.Current == RuntimeTool.Rect)
                    {
                        m_rectTool.Targets = GetHandleTargets();
                    }
                    m_rectTool.gameObject.SetActive(value && Editor.Tools.Current == RuntimeTool.Rect && m_rectTool.Target != null);
                }
            }
        }

        public bool IsBoxSelectionEnabled
        {
            get { return m_isBoxSelectionEnabled && m_boxSelection != null; }
            set
            {
                m_isBoxSelectionEnabled = value;
                if (m_boxSelection != null)
                {
                    if (value)
                    {
                        if (Editor != null && Editor.ActiveWindow == Window)
                        {
                            m_boxSelection.enabled = true;
                        }
                    }
                    else
                    {
                        m_boxSelection.enabled = false;
                    }
                }
            }
        }

        public bool IsSelectionVisible
        {
            get { return m_isSelectionVisible; }
            set { m_isSelectionVisible = value; }
        }

        public bool CanSelect
        {
            get { return m_canSelect; }
            set { m_canSelect = value; }
        }

        public bool CanSelectAll
        {
            get { return m_canSelectAll; }
            set { m_canSelectAll = value; }
        }

        private bool m_isGridVisible = true;
        public bool IsGridVisible
        {
            get { return m_isGridVisible; }
            set
            {
                if (m_isGridVisible != value)
                {
                    m_isGridVisible = value;
                    ApplyIsGridVisible();
                }
            }
        }

        private void ApplyIsGridVisible()
        {
            if (m_grid != null)
            {
                m_grid.gameObject.SetActive(m_isGridVisible);
            }
        }

        private bool m_isGridEnabled;
        public bool IsGridEnabled
        {
            get { return m_isGridEnabled; }
            set
            {
                if (m_isGridEnabled != value)
                {
                    m_isGridEnabled = value;
                    ApplyIsGridEnabled();
                }
            }
        }

        private void ApplyIsGridEnabled()
        {
            if (m_positionHandle != null)
            {
                m_positionHandle.SnapToGrid = IsGridEnabled;
            }

            if (m_scaleHandle != null)
            {
                m_scaleHandle.SnapToGrid = IsGridEnabled;
            }

            if (m_customHandle != null)
            {
                m_customHandle.SnapToGrid = IsGridEnabled;
            }
        }

        private bool m_gridZTest = true;
        public bool GridZTest
        {
            get { return m_gridZTest; }
            set
            {
                if (m_gridZTest != value)
                {
                    m_gridZTest = value;
                    ApplyGridZTest();
                }
            }
        }

        private void ApplyGridZTest()
        {
            if (m_grid != null)
            {
                m_grid.ZTest = m_gridZTest;
            }
        }

        public float SizeOfGrid
        {
            get
            {
                if (m_grid == null)
                {
                    return 0.5f;
                }
                return m_grid.SizeOfGrid;
            }
            set
            {
                if (m_grid == null)
                {
                    return;
                }

                m_grid.SizeOfGrid = value;
                ApplySizeOfGrid();
            }
        }

        private void ApplySizeOfGrid()
        {
            if (m_positionHandle != null)
            {
                m_positionHandle.SizeOfGrid = SizeOfGrid;
            }

            if (m_scaleHandle != null)
            {
                m_scaleHandle.SizeOfGrid = SizeOfGrid;
            }

            if (m_rectTool != null)
            {
                m_rectTool.SizeOfGrid = SizeOfGrid;
            }

            if (m_customHandle != null)
            {
                m_customHandle.SizeOfGrid = SizeOfGrid;
            }
        }

        private IRuntimeSelection m_selectionOverride;
        public IRuntimeSelection Selection
        {
            get
            {
                if (m_selectionOverride != null)
                {
                    return m_selectionOverride;
                }

                return Editor.Selection;
            }
            set
            {
                if (m_selectionOverride != value)
                {
                    if (m_selectionOverride != null)
                    {
                        m_selectionOverride.SelectionChanged -= OnRuntimeSelectionChanged;
                    }

                    m_selectionOverride = value;
                    if (m_selectionOverride == Editor.Selection)
                    {
                        m_selectionOverride = null;
                    }

                    if (m_selectionOverride != null)
                    {
                        OnRuntimeSelectionChanged(Editor.Selection.objects);
                        m_selectionOverride.SelectionChanged += OnRuntimeSelectionChanged;
                    }

                    if (m_outlineManager != null)
                    {
                        m_outlineManager.Selection = m_selectionOverride;
                    }
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
        
            Window.IOCContainer.RegisterFallback<IScenePivot>(this);
            Window.IOCContainer.RegisterFallback<IRuntimeSelectionComponent>(this);

            if (m_outlineManager == null)
            {
                m_outlineManager = GetComponentInChildren<OutlineManager>(true);
                if (m_outlineManager != null)
                {
                    m_outlineManager.Camera = Window.Camera;
                }
            }

            if (m_boxSelection == null)
            {
                m_boxSelection = GetComponentInChildren<BoxSelection>(true);
            }
            if (m_positionHandle == null)
            {
                m_positionHandle = GetComponentInChildren<PositionHandle>(true);

            }
            if (m_rotationHandle == null)
            {
                m_rotationHandle = GetComponentInChildren<RotationHandle>(true);
            }
            if (m_scaleHandle == null)
            {
                m_scaleHandle = GetComponentInChildren<ScaleHandle>(true);
            }
            if (m_rectTool == null)
            {
                m_rectTool = GetComponentInChildren<RectTool>(true);
            }
            if (m_grid == null)
            {
                m_grid = GetComponentInChildren<SceneGrid>(true);
            }

            if (m_boxSelection != null)
            {
                if (m_boxSelection.Window == null)
                {
                    m_boxSelection.Window = Window;
                }

                m_boxSelection.Filtering += OnBoxSelectionFiltering;
                m_boxSelection.Selection += OnBoxSelection;
            }

            if (m_positionHandle != null)
            {
                if (m_positionHandle.Window == null)
                {
                    m_positionHandle.Window = Window;
                }

                m_positionHandle.gameObject.SetActive(true);
                m_positionHandle.gameObject.SetActive(false);

                m_positionHandle.BeforeDrag.AddListener(OnBeforeDrag);
                m_positionHandle.Drop.AddListener(OnDrop);
            }

            if (m_rotationHandle != null)
            {
                if (m_rotationHandle.Window == null)
                {
                    m_rotationHandle.Window = Window;
                }

                m_rotationHandle.gameObject.SetActive(true);
                m_rotationHandle.gameObject.SetActive(false);

                m_rotationHandle.BeforeDrag.AddListener(OnBeforeDrag);
                m_rotationHandle.Drop.AddListener(OnDrop);
            }

            if (m_scaleHandle != null)
            {
                if (m_scaleHandle.Window == null)
                {
                    m_scaleHandle.Window = Window;
                }
                m_scaleHandle.gameObject.SetActive(true);
                m_scaleHandle.gameObject.SetActive(false);

                m_scaleHandle.BeforeDrag.AddListener(OnBeforeDrag);
                m_scaleHandle.Drop.AddListener(OnDrop);
            }

            if (m_rectTool != null)
            {
                if (m_rectTool.Window == null)
                {
                    m_rectTool.Window = Window;
                }
                m_rectTool.gameObject.SetActive(true);
                m_rectTool.gameObject.SetActive(false);

                m_rectTool.BeforeDrag.AddListener(OnBeforeDrag);
                m_rectTool.Drop.AddListener(OnDrop);
            }

            if (m_grid != null)
            {
                if (m_grid.Window == null)
                {
                    m_grid.Window = Window;
                }
            }

            Editor.Selection.SelectionChanged += OnRuntimeEditorSelectionChanged;
            Editor.Tools.ToolChanged += OnRuntimeToolChanged;

            if (m_pivot == null)
            {
                GameObject pivot = new GameObject("Pivot");
                pivot.transform.SetParent(transform, true);
                pivot.transform.position = Vector3.zero;
                m_pivot = pivot.transform;
            }

            if (m_secondaryPivot == null)
            {
                GameObject secondaryPivot = new GameObject("SecondaryPivot");
                secondaryPivot.transform.SetParent(transform, true);
                secondaryPivot.transform.position = Vector3.zero;
                m_secondaryPivot = secondaryPivot.transform;
            }

            ApplySizeOfGrid();
            ApplyIsGridEnabled();
            ApplyIsGridVisible();
            ApplyGridZTest();

            OnRuntimeEditorSelectionChanged(null);
        }

        protected override void Start()
        {
            base.Start();
        
            if (GetComponent<RuntimeSelectionInputBase>() == null)
            {
                gameObject.AddComponent<RuntimeSelectionInput>();
            }

            if (m_positionHandle != null && !m_positionHandle.gameObject.activeSelf)
            {
                m_positionHandle.gameObject.SetActive(true);
                m_positionHandle.gameObject.SetActive(false);
            }

            if (m_rotationHandle != null && !m_rotationHandle.gameObject.activeSelf)
            {
                m_rotationHandle.gameObject.SetActive(true);
                m_rotationHandle.gameObject.SetActive(false);
            }

            if (m_scaleHandle != null && !m_scaleHandle.gameObject.activeSelf)
            {
                m_scaleHandle.gameObject.SetActive(true);
                m_scaleHandle.gameObject.SetActive(false);
            }

            if (m_rectTool != null && !m_rectTool.gameObject.activeSelf)
            {
                m_rectTool.gameObject.SetActive(true);
                m_rectTool.gameObject.SetActive(false);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        
            Window.IOCContainer.UnregisterFallback<IScenePivot>(this);
            Window.IOCContainer.UnregisterFallback<IRuntimeSelectionComponent>(this);

            if (m_boxSelection != null)
            {
                m_boxSelection.Filtering -= OnBoxSelectionFiltering;
                m_boxSelection.Selection -= OnBoxSelection;
            }

            if (Editor != null)
            {
                Editor.Tools.ToolChanged -= OnRuntimeToolChanged;
                Editor.Selection.SelectionChanged -= OnRuntimeEditorSelectionChanged;
            }

            if (m_positionHandle != null)
            {
                m_positionHandle.BeforeDrag.RemoveListener(OnBeforeDrag);
                m_positionHandle.Drop.RemoveListener(OnDrop);
            }

            if (m_rotationHandle != null)
            {
                m_rotationHandle.BeforeDrag.RemoveListener(OnBeforeDrag);
                m_rotationHandle.Drop.RemoveListener(OnDrop);
            }

            if (m_scaleHandle != null)
            {
                m_scaleHandle.BeforeDrag.RemoveListener(OnBeforeDrag);
                m_scaleHandle.Drop.RemoveListener(OnDrop);
            }

            if (m_rectTool != null)
            {
                m_rectTool.BeforeDrag.RemoveListener(OnBeforeDrag);
                m_rectTool.Drop.RemoveListener(OnDrop);
            }

            if (m_customHandle != null)
            {
                m_customHandle.BeforeDrag.RemoveListener(OnBeforeDrag);
                m_customHandle.Drop.RemoveListener(OnDrop);
            }

            if (m_selectionOverride != null)
            {
                m_selectionOverride.SelectionChanged -= OnRuntimeSelectionChanged;
                m_selectionOverride = null;
            }
        }

        protected virtual IEnumerable<BaseRaycaster> FindRaycasters()
        {
            return FindObjectsOfType<BaseRaycaster>().Where(raycaster => raycaster.GetComponentInParent<IRTE>() == null);
        }

        protected readonly List<RaycastResult> m_uiRaycastResults = new List<RaycastResult>();
        protected virtual IList<RaycastResult> RaycastUIObjects()
        {
            m_uiRaycastResults.Clear();

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            
            foreach (BaseRaycaster raycaster in FindRaycasters())
            {
                Camera canvasCamera;
                Canvas canvas = raycaster.GetComponent<Canvas>();
                canvasCamera = canvas.worldCamera;

                canvas.worldCamera = Window.Camera;

                if (raycaster is GraphicRaycaster)
                {
                    GraphicRaycaster.BlockingObjects blockingObjects = GraphicRaycaster.BlockingObjects.None;

                    GraphicRaycaster graphicRaycaster = (GraphicRaycaster)raycaster;
                    blockingObjects = graphicRaycaster.blockingObjects;
                    graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.All;

                    pointerData.position = Window.Pointer.ScreenPoint;
                    raycaster.Raycast(pointerData, m_uiRaycastResults);
                    graphicRaycaster.blockingObjects = blockingObjects;

                }
                else
                {
                    raycaster.Raycast(pointerData, m_uiRaycastResults);
                }

                canvas.worldCamera = canvasCamera;
            }
                        
            m_uiRaycastResults.Sort(RaycastComparer);
            return m_uiRaycastResults;
        }

        protected HashSet<GameObject> m_uiBoxcastResults = new HashSet<GameObject>();
        protected virtual IEnumerable<GameObject> BoxcastUIObjects()
        {
            m_uiBoxcastResults.Clear();

            Bounds bounds = m_boxSelection.SelectionBounds;
            
            Vector3[] corners = new Vector3[4];
            foreach (BaseRaycaster raycaster in FindRaycasters())
            {
                CanvasRenderer[] renderers = raycaster.GetComponentsInChildren<CanvasRenderer>();

                for(int i = 0; i < renderers.Length; ++i)
                {
                    CanvasRenderer renderer = renderers[i];
                    RectTransform rectTransform = (RectTransform)renderer.transform;
                    rectTransform.GetWorldCorners(corners);
                    for (int j = 0; j < 4; ++j)
                    {
                        Vector3 corner = Window.Camera.WorldToScreenPoint(corners[j]);
                        corner.z = 0;

                        if (bounds.Contains(corner))
                        {
                            if (CanSelectObject(renderer.gameObject))
                            {
                                ExposeToEditor exposeToEditor = renderer.GetComponentInParent<ExposeToEditor>();
                                if(exposeToEditor != null)
                                {
                                    m_uiBoxcastResults.Add(exposeToEditor.gameObject);
                                }
                            }
                            break;
                        }
                    }
                }

            }
            return m_uiBoxcastResults;
        }

        private static int RaycastComparer(RaycastResult lhs, RaycastResult rhs)
        {
            if (lhs.module != rhs.module)
            {
                if (lhs.module.eventCamera != null && rhs.module.eventCamera != null && lhs.module.eventCamera.depth != rhs.module.eventCamera.depth)
                {
                    // need to reverse the standard compareTo
                    if (lhs.module.eventCamera.depth < rhs.module.eventCamera.depth)
                        return 1;
                    if (lhs.module.eventCamera.depth == rhs.module.eventCamera.depth)
                        return 0;

                    return -1;
                }

                if (lhs.module.sortOrderPriority != rhs.module.sortOrderPriority)
                    return rhs.module.sortOrderPriority.CompareTo(lhs.module.sortOrderPriority);

                if (lhs.module.renderOrderPriority != rhs.module.renderOrderPriority)
                    return rhs.module.renderOrderPriority.CompareTo(lhs.module.renderOrderPriority);
            }

            if (lhs.sortingLayer != rhs.sortingLayer)
            {
                // Uses the layer value to properly compare the relative order of the layers.
                var rid = SortingLayer.GetLayerValueFromID(rhs.sortingLayer);
                var lid = SortingLayer.GetLayerValueFromID(lhs.sortingLayer);
                return rid.CompareTo(lid);
            }


            if (lhs.sortingOrder != rhs.sortingOrder)
                return rhs.sortingOrder.CompareTo(lhs.sortingOrder);

            if (lhs.depth != rhs.depth)
                return rhs.depth.CompareTo(lhs.depth);

            if (lhs.distance != rhs.distance)
                return lhs.distance.CompareTo(rhs.distance);

            return lhs.index.CompareTo(rhs.index);
        }

        protected virtual RaycastHit[] Raycast3DObjects()
        {
            RaycastHit[] hits = Physics.RaycastAll(Window.Pointer, float.MaxValue).Where(hit => CanSelectObject(hit.collider.gameObject)).OrderBy(hit => GetDepth(hit.transform)).ToArray();
            return hits;
        }

        public virtual void SelectGO(bool multiselect, bool allowUnselect)
        {
            if (!CanSelect)
            {
                return;
            }

            if(m_boxSelection != null && m_boxSelection.IsThresholdPassed)
            {
                return;
            }
            IList<RaycastResult> raycastResults = RaycastUIObjects();
            if(raycastResults.Count > 0)
            {
                SelectGO(multiselect, allowUnselect, raycastResults, raycastResult => raycastResult.gameObject);
            }
            else
            {
                RaycastHit[] hits = Raycast3DObjects();
                if (hits.Length > 0)
                {
                    bool canSelect = hits.Length > 0;
                    if (canSelect)
                    {
                        hits = FilterHits(hits);
                    }
                    else
                    {
                        hits = new RaycastHit[0];
                    }

                    SelectGO(multiselect, allowUnselect, hits, hit => hit.collider.gameObject);
                }
                else
                {
                    Renderer[] renderers = m_boxSelection != null ? m_boxSelection.Pick() : new Renderer[0];
                    SelectGO(multiselect, allowUnselect, renderers, renderer => renderer.gameObject);
                }
            }
        }

        private void SelectGO<T>(bool multiselect, bool allowUnselect, IList<T> hits, Func<T, GameObject> getGameObject)
        {
            if (hits.Count > 0)
            {
                int nextIndex = GetNextIndex(hits, getGameObject);
                GameObject go = getGameObject(hits[nextIndex]);
                ExposeToEditor exposeToEditor = go.GetComponentInParent<ExposeToEditor>();
                GameObject hitGO = exposeToEditor != null ? exposeToEditor.gameObject : go;
                if(CanSelectObject(hitGO))
                {
                    SelectGO(multiselect, allowUnselect, hitGO);
                }
                else
                {
                    if (!multiselect)
                    {
                        TryToClearSelection();
                    }
                }
            }
            else
            {
                if (!multiselect)
                {
                    TryToClearSelection();
                }
            }
        }

        private void SelectGO(bool multiselect, bool allowUnselect, GameObject hitGO)
        {
            if (multiselect)
            {
                List<UnityObject> selectionList;
                if (Selection.objects != null)
                {
                    selectionList = Selection.objects.ToList();
                }
                else
                {
                    selectionList = new List<UnityObject>();
                }

                if (selectionList.Contains(hitGO))
                {
                    selectionList.Remove(hitGO);
                    if (!allowUnselect)
                    {
                        selectionList.Insert(0, hitGO);
                    }
                }
                else
                {
                    selectionList.Insert(0, hitGO);
                }

                UnityObject[] selection = selectionList.ToArray();
                UnityObject[] filteredSelection;
                if (RaiseSelectionChanging(selection, out filteredSelection))
                {
                    if (filteredSelection.Length == 0)
                    {
                        Selection.objects = null;
                    }
                    else
                    {
                        filteredSelection = filteredSelection.OrderByDescending(o => o == hitGO).ToArray();
                        Editor.Undo.Select(Selection, selection, filteredSelection.FirstOrDefault());
                    }
                    RaiseSelectionChanged();
                }
            }
            else
            {
                UnityObject[] filteredSelection;
                if (RaiseSelectionChanging(new[] { hitGO }, out filteredSelection))
                {
                    if (filteredSelection.Length == 0)
                    {
                        Selection.objects = null;
                    }
                    else
                    {
                        Selection.objects = filteredSelection;
                    }

                    RaiseSelectionChanged();
                }
            }
        }

        private int GetDepth(Transform tr)
        {
            int depth = 0;

            while (tr.parent != null)
            {
                depth++;
                tr = tr.parent;
            }

            return depth;
        }

        private bool IsReachable(Transform t1, Transform t2)
        {
            Transform p1 = t1;
            while (p1 != null)
            {
                if (p1 == t2)
                {
                    return true;
                }

                p1 = p1.parent;
            }

            Transform p2 = t2;
            while (p2 != null)
            {
                if (p2 == t1)
                {
                    return true;
                }

                p2 = p2.parent;
            }

            return false;
        }

        private RaycastHit[] FilterHits(RaycastHit[] hits)
        {
            IEnumerable<RaycastHit> orderedHits = hits.OrderBy(hit => hit.distance);
            if (Filtering != null)
            {
                RuntimeSelectionFilteringArgs args = new RuntimeSelectionFilteringArgs(orderedHits);
                Filtering(this, args);
                orderedHits = args.Hits;
            }

            RaycastHit closestHit = orderedHits.FirstOrDefault();
            return orderedHits.Where(h => IsReachable(h.transform, closestHit.transform)).ToArray();
        }
        private int GetNextIndex<T>(IList<T> hits, Func<T, GameObject> getGameObject)
        {
            int index = -1;
            if (hits == null || hits.Count == 0)
            {
                return index;
            }

            if (Selection.activeGameObject != null)
            {
                for (int i = 0; i < hits.Count; ++i)
                {
                    if (Selection.IsSelected(getGameObject(hits[i])))
                    {
                        index = i;
                    }
                }
            }

            index++;
            index %= hits.Count;
            return index;
        }

        private void TryToClearSelection()
        {
            UnityObject[] filteredSelection;
            if (RaiseSelectionChanging(new UnityObject[0], out filteredSelection))
            {
                if (filteredSelection.Length == 0)
                {
                    Selection.activeObject = null;
                }
                else
                {
                    Selection.objects = filteredSelection;
                }

                RaiseSelectionChanged();
            }
        }

        private bool RaiseSelectionChanging(UnityObject[] selected, out UnityObject[] filteredSelection)
        {
            if (SelectionChanging != null)
            {
                RuntimeSelectionChangingArgs args = new RuntimeSelectionChangingArgs(selected);
                SelectionChanging(this, args);
                filteredSelection = args.Selected.ToArray();
                return !args.Cancel;
            }

            filteredSelection = selected;
            return true;
        }

        private void RaiseSelectionChanged()
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, EventArgs.Empty);
            }
        }

        public virtual void SnapToGrid()
        {
            GameObject[] selection = Selection.gameObjects;
            if (selection == null || selection.Length == 0)
            {
                return;
            }

            Transform activeTransform = selection[0].transform;

            Vector3 position = activeTransform.position;
            if (SizeOfGrid < 0.01)
            {
                SizeOfGrid = 0.01f;
            }
            position.x = Mathf.Round(position.x / SizeOfGrid) * SizeOfGrid;
            position.y = Mathf.Round(position.y / SizeOfGrid) * SizeOfGrid;
            position.z = Mathf.Round(position.z / SizeOfGrid) * SizeOfGrid;
            Vector3 offset = position - activeTransform.position;

            Editor.Undo.BeginRecord();
            for (int i = 0; i < selection.Length; ++i)
            {
                Editor.Undo.BeginRecordTransform(selection[i].transform);
                selection[i].transform.position += offset;
                Editor.Undo.EndRecordTransform(selection[i].transform);
            }
            Editor.Undo.EndRecord();

            if (m_rectTool != null && Editor.Tools.Current == RuntimeTool.Rect)
            {
                m_rectTool.RecalculateBoundsAndRebuild();
            }
        }

        public virtual void SelectAll()
        {
            if (!CanSelect || !CanSelectAll)
            {
                return;
            }

            UnityObject[] selection = Editor.Object.Get(false).Select(exposed => exposed.gameObject).ToArray();
            UnityObject[] filteredSelection = selection;
            if (RaiseSelectionChanging(selection, out filteredSelection))
            {
                if (filteredSelection.Length == 0)
                {
                    Selection.objects = null;
                }
                else
                {
                    Selection.objects = selection;
                }
                RaiseSelectionChanged();
            }
        }

        private void OnRuntimeToolChanged()
        {
            bool hasSelection = Selection.activeTransform != null;

            if (m_positionHandle != null)
            {
                if (hasSelection && Editor.Tools.Current == RuntimeTool.Move && IsPositionHandleEnabled)
                {
                    m_positionHandle.transform.position = Selection.activeTransform.position;
                    m_positionHandle.Targets = GetHandleTargets();
                    m_positionHandle.gameObject.SetActive(m_positionHandle.Targets.Length > 0);
                }
                else
                {
                    m_positionHandle.gameObject.SetActive(false);
                }
            }
            if (m_rotationHandle != null)
            {
                if (hasSelection && Editor.Tools.Current == RuntimeTool.Rotate && IsRotationHandleEnabled)
                {
                    m_rotationHandle.transform.position = Selection.activeTransform.position;
                    m_rotationHandle.Targets = GetHandleTargets();
                    m_rotationHandle.gameObject.SetActive(m_rotationHandle.Targets.Length > 0);
                }
                else
                {
                    m_rotationHandle.gameObject.SetActive(false);
                }
            }
            if (m_scaleHandle != null)
            {
                if (hasSelection && Editor.Tools.Current == RuntimeTool.Scale && IsScaleHandleEnabled)
                {
                    m_scaleHandle.transform.position = Selection.activeTransform.position;
                    m_scaleHandle.Targets = GetHandleTargets();
                    m_scaleHandle.gameObject.SetActive(m_scaleHandle.Targets.Length > 0);
                }
                else
                {
                    m_scaleHandle.gameObject.SetActive(false);
                }
            }
            if (m_rectTool != null)
            {
                if (hasSelection && Editor.Tools.Current == RuntimeTool.Rect && IsRectToolEnabled)
                {
                    m_rectTool.transform.position = Selection.activeTransform.position;
                    m_rectTool.Targets = GetHandleTargets();
                    m_rectTool.gameObject.SetActive(m_rectTool.Targets.Length > 0);
                }
                else
                {
                    m_rectTool.gameObject.SetActive(false);
                }
            }
            if (m_customHandle != null)
            {
                if (hasSelection && Editor.Tools.Current == RuntimeTool.Custom)
                {
                    m_customHandle.transform.position = Selection.activeTransform.position;
                    m_customHandle.Targets = GetHandleTargets();
                    m_customHandle.gameObject.SetActive(m_customHandle.Targets.Length > 0);
                }
                else
                {
                    m_customHandle.gameObject.SetActive(false);
                }
            }
        }

        private void OnBoxSelectionFiltering(object sender, FilteringArgs e)
        {
            if (e.Object == null)
            {
                e.Cancel = true;
            }

            ExposeToEditor exposeToEditor = e.Object.GetComponent<ExposeToEditor>();
            if (!exposeToEditor && m_canSelectExposedOnly)
            {
                e.Cancel = true;
            }
        }

        private void OnBoxSelection(object sender, BoxSelectionArgs e)
        {
            if (!CanSelect)
            {
                return;
            }

            IEnumerable<GameObject> gameObjects = BoxcastUIObjects();
            UnityObject[] filteredSelection;
            if (RaiseSelectionChanging(gameObjects.Union(e.GameObjects).ToArray(), out filteredSelection))
            {
                if (filteredSelection.Length == 0)
                {
                    Selection.objects = null;
                }
                else
                {
                    Selection.objects = filteredSelection;
                }
                RaiseSelectionChanged();
            }
        }

        private void OnRuntimeEditorSelectionChanged(UnityObject[] unselected)
        {
            HandleRuntimeSelectionChange(Editor.Selection, unselected);

            if (Editor.Selection == Selection)
            {
                UpdateHandlesState();
            }
        }

        private void OnRuntimeSelectionChanged(UnityObject[] unselected)
        {
            HandleRuntimeSelectionChange(m_selectionOverride, unselected);

            UpdateHandlesState();
        }

        private void UpdateHandlesState()
        {
            if (Selection.activeGameObject == null || Selection.activeGameObject.IsPrefab())
            {
                SetHandlesActive(false);
            }
            else
            {
                SetHandlesActive(false);
                OnRuntimeToolChanged();
            }
        }

        private void HandleRuntimeSelectionChange(IRuntimeSelection selection, UnityObject[] unselected)
        {
            if (!IsSelectionVisible)
            {
                return;
            }

            if (unselected != null)
            {
                for (int i = 0; i < unselected.Length; ++i)
                {
                    GameObject unselectedObj = unselected[i] as GameObject;
                    if (unselectedObj != null)
                    {
                        ExposeToEditor exposeToEditor = unselectedObj.GetComponent<ExposeToEditor>();
                        if (exposeToEditor)
                        {
                            if (exposeToEditor.Unselected != null)
                            {
                                exposeToEditor.Unselected.Invoke(exposeToEditor);
                            }
                        }
                    }
                }
            }

            GameObject[] selected = selection.gameObjects;
            if (selected != null)
            {
                for (int i = 0; i < selected.Length; ++i)
                {
                    GameObject selectedObj = selected[i];
                    ExposeToEditor exposeToEditor = selectedObj.GetComponent<ExposeToEditor>();
                    if (exposeToEditor && !selectedObj.IsPrefab() && !selectedObj.isStatic)
                    {
                        if (exposeToEditor.Selected != null)
                        {
                            exposeToEditor.Selected.Invoke(exposeToEditor);
                        }
                    }
                }
            }
        }

        private void SetHandlesActive(bool isActive)
        {
            if (m_positionHandle != null)
            {
                m_positionHandle.gameObject.SetActive(isActive);
            }
            if (m_rotationHandle != null)
            {
                m_rotationHandle.gameObject.SetActive(isActive);
            }
            if (m_scaleHandle != null)
            {
                m_scaleHandle.gameObject.SetActive(isActive);
            }
            if (m_rectTool != null)
            {
                m_rectTool.gameObject.SetActive(isActive);
            }
            if (m_customHandle != null)
            {
                m_customHandle.gameObject.SetActive(isActive);
            }
        }

        protected virtual bool CanSelectObject(GameObject go)
        {
            return !m_canSelectExposedOnly || go.GetComponentInParent<ExposeToEditor>();
        }

        protected virtual bool CanTransformObject(GameObject go)
        {
            if (go == null)
            {
                return false;
            }

            ExposeToEditor exposeToEditor = go.GetComponentInParent<ExposeToEditor>();
            if (exposeToEditor == null)
            {
                return true;
            }
            return exposeToEditor.CanTransform;
        }

        public virtual Transform[] GetHandleTargets()
        {
            if (Selection.gameObjects == null)
            {
                return null;
            }

            return Selection.gameObjects.Where(g => CanTransformObject(g)).Select(g => g.transform).OrderByDescending(g => Selection.activeTransform == g).ToArray();
        }

        [Obsolete]
        public virtual void Focus()
        {

        }

        public virtual void Focus(FocusMode focusMode = FocusMode.Default)
        {

        }

        public virtual void Focus(Vector3 objPositon, float objSize)
        {

        }


        private bool m_wasUnitSnappingEnabled;
        private void OnBeforeDrag(BaseHandle handle)
        {
            if (IsGridEnabled)
            {
                m_wasUnitSnappingEnabled = Editor.Tools.UnitSnapping;
                Editor.Tools.UnitSnapping = true;
            }

        }

        private void OnDrop(BaseHandle handle)
        {
            if (IsGridEnabled)
            {
                Editor.Tools.UnitSnapping = m_wasUnitSnappingEnabled;
            }
        }



        #region Obsolete
        [Obsolete("Use GetHandleTargets() instead")]
        protected virtual Transform[] GetTargets()
        {
            return GetHandleTargets();
        }
        #endregion
    }
}
