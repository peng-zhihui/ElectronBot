using Battlehub.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Battlehub.RTCommon
{
    [Obsolete]
    public struct ComponentEditorSettings
    {
        public bool ShowResetButton;
        public bool ShowExpander;
        public bool ShowEnableButton;
        public bool ShowRemoveButton;

        public ComponentEditorSettings(bool showExpander, bool showResetButton, bool showEnableButton, bool showRemoveButton)
        {
            ShowResetButton = showResetButton;
            ShowExpander = showExpander;
            ShowEnableButton = showEnableButton;
            ShowRemoveButton = showRemoveButton;
        }
    }

    [Serializable]
    public struct CameraLayerSettings
    {
        public int ResourcePreviewLayer;
        public int RuntimeGraphicsLayer;
        public int MaxGraphicsLayers;
        public int AllScenesLayer;
        public int ExtraLayer2;
        public int ExtraLayer;
        public int UIBackgroundLayer;

        public int RaycastMask
        {
            get
            {
                return ~((((1 << MaxGraphicsLayers) - 1) << RuntimeGraphicsLayer) | (1 << AllScenesLayer) | (1 << ExtraLayer) | (1 << ExtraLayer2) | (1 << ResourcePreviewLayer));
            }
        }

        public CameraLayerSettings(int resourcePreviewLayer, int runtimeGraphicsLayer, int maxLayers, int allSceneLayer, int extraLayer, int hiddenLayer, int uiBackgroundLayer)
        {
            ResourcePreviewLayer = resourcePreviewLayer;
            RuntimeGraphicsLayer = runtimeGraphicsLayer;
            MaxGraphicsLayers = maxLayers;
            AllScenesLayer = allSceneLayer;
            ExtraLayer = extraLayer;
            ExtraLayer2 = hiddenLayer;
            UIBackgroundLayer = uiBackgroundLayer;
        }
    }

    public interface IRTE
    {
        event RTEEvent BeforePlaymodeStateChange;
        event RTEEvent PlaymodeStateChanging;
        event RTEEvent PlaymodeStateChanged;
        event RTEEvent<RuntimeWindow> ActiveWindowChanging;
        event RTEEvent<RuntimeWindow> ActiveWindowChanged;
        event RTEEvent<RuntimeWindow> WindowRegistered;
        event RTEEvent<RuntimeWindow> WindowUnregistered;
        event RTEEvent IsOpenedChanged;
        event RTEEvent IsDirtyChanged;
        event RTEEvent<GameObject[]> ObjectsRegistered;
        event RTEEvent<GameObject[]> ObjectsDuplicated;
        event RTEEvent<GameObject[]> ObjectsDeleted;

        [Obsolete("Use ISettingsComponent.InspectorSettings instead")]
        ComponentEditorSettings ComponentEditorSettings
        {
            get;
        }

        CameraLayerSettings CameraLayerSettings
        {
            get;
        }

        IUIRaycaster Raycaster
        {
            get;
        }

        EventSystem EventSystem
        {
            get;
        }

        //[Obsolete]
        bool IsVR
        {
            get;
        }

        IInput Input
        {
            get;
        }

        IRuntimeSelection Selection
        {
            get;
        }

        IRuntimeUndo Undo
        {
            get;
        }

        RuntimeTools Tools
        {
            get;
        }

        CursorHelper CursorHelper
        {
            get;
        }

        IRuntimeObjects Object
        {
            get;
        }

        IDragDrop DragDrop
        {
            get;
        }

        bool IsDirty
        {
            get;
            set;
        }

        bool IsOpened
        {
            get;
            set;
        }

        bool IsBusy
        {
            get;
            set;
        }

        bool IsPlaymodeStateChanging
        {
            get;
        }

        bool IsPlaying
        {
            get;
            set;
        }

        bool IsApplicationPaused
        {
            get;
        }

        Transform Root
        {
            get;
        }

        bool IsInputFieldActive
        {
            get;
        }

        bool IsInputFieldFocused
        {
            get;
        }

        void UpdateCurrentInputField();

        RuntimeWindow ActiveWindow
        {
            get;
        }

        RuntimeWindow PointerOverWindow
        {
            get;
        }

        RuntimeWindow[] Windows
        {
            get;
        }

        bool Contains(RuntimeWindow window);
        int GetIndex(RuntimeWindowType windowType);
        RuntimeWindow GetWindow(RuntimeWindowType windowType);
        void ActivateWindow(RuntimeWindowType window);
        void ActivateWindow(RuntimeWindow window);
        void SetPointerOverWindow(RuntimeWindow window);
        void RegisterWindow(RuntimeWindow window);
        void UnregisterWindow(RuntimeWindow window);
        void RegisterCreatedObjects(GameObject[] go, bool select = true);
        void Duplicate(GameObject[] go);
        void Delete(GameObject[] go);
        void Close();

        void AddGameObjectToHierarchy(GameObject go, bool scaleStays = true);

        Coroutine StartCoroutine(IEnumerator method);
    }

    public delegate void RTEEvent();
    public delegate void RTEEvent<T>(T arg);

    [DefaultExecutionOrder(-90)]
    public class RTEBase : MonoBehaviour, IRTE
    {
        private IUIRaycaster m_uiRaycaster;
        [SerializeField]
        protected EventSystem m_eventSystem;

        [SerializeField]
        private CameraLayerSettings m_cameraLayerSettings = new CameraLayerSettings(20, 21, 4, 17, 18, 19, 16);
        [SerializeField]
        private bool m_createHierarchyRoot = false;

        [SerializeField]
        private bool m_useBuiltinUndo = true;

        [SerializeField]
        private bool m_enableVRIfAvailable = true;

        [SerializeField]
        private bool m_isOpened = true;
        [SerializeField]
        private UnityEvent IsOpenedEvent = null;
        [SerializeField]
        private UnityEvent IsClosedEvent = null;

        public event RTEEvent BeforePlaymodeStateChange;
        public event RTEEvent PlaymodeStateChanging;
        public event RTEEvent PlaymodeStateChanged;
        public event RTEEvent<RuntimeWindow> ActiveWindowChanging;
        public event RTEEvent<RuntimeWindow> ActiveWindowChanged;
        public event RTEEvent<RuntimeWindow> WindowRegistered;
        public event RTEEvent<RuntimeWindow> WindowUnregistered;
        public event RTEEvent IsOpenedChanged;
        public event RTEEvent IsDirtyChanged;
        public event RTEEvent IsBusyChanged;
        public event RTEEvent<GameObject[]> ObjectsRegistered;
        public event RTEEvent<GameObject[]> ObjectsDuplicated;
        public event RTEEvent<GameObject[]> ObjectsDeleted;

        private IInput m_disabledInput;
        private IInput m_input;
        private IInput m_activeInput;
        private RuntimeSelection m_selection;
        private RuntimeTools m_tools = new RuntimeTools();
        private CursorHelper m_cursorHelper = new CursorHelper();
        private IRuntimeUndo m_undo;
        private DragDrop m_dragDrop;
        private IRuntimeObjects m_object;

        protected GameObject m_currentSelectedGameObject;
        protected TMP_InputField m_currentInputFieldTMP;
        protected InputField m_currentInputFieldUI;
        protected float m_zAxis;

        public IUIRaycaster Raycaster
        {
            get { return m_uiRaycaster; }
        }

        public EventSystem EventSystem
        {
            get { return m_eventSystem; }
        }

        protected readonly HashSet<GameObject> m_windows = new HashSet<GameObject>();
        protected RuntimeWindow[] m_windowsArray;
        public bool IsInputFieldActive
        {
            get { return m_currentInputFieldTMP != null || m_currentInputFieldUI != null; }
        }

        public bool IsInputFieldFocused
        {
            get
            {
                if (m_currentInputFieldTMP != null)
                {
                    return m_currentInputFieldTMP.isFocused;
                }
                if (m_currentInputFieldUI != null)
                {
                    return m_currentInputFieldUI.isFocused;
                }
                return false;
            }

        }

        private RuntimeWindow m_activeWindow;
        public virtual RuntimeWindow ActiveWindow
        {
            get { return m_activeWindow; }
        }

        private RuntimeWindow m_pointerOverWindow;
        public virtual RuntimeWindow PointerOverWindow
        {
            get { return m_pointerOverWindow; }
        }

        public virtual RuntimeWindow[] Windows
        {
            get { return m_windowsArray; }
        }

        public bool Contains(RuntimeWindow window)
        {
            return m_windows.Contains(window.gameObject);
        }

#pragma warning disable CS0612
        private ComponentEditorSettings m_componentEditorSettings = new ComponentEditorSettings();
        [Obsolete]
        public virtual ComponentEditorSettings ComponentEditorSettings
        {
            get { return m_componentEditorSettings; }
        }
#pragma warning restore CS0612


        public virtual CameraLayerSettings CameraLayerSettings
        {
            get { return m_cameraLayerSettings; }
        }

        //[Obsolete]
        public virtual bool IsVR
        {
            get;
            private set;
        }

        public virtual IInput Input
        {
            get
            {
                return m_activeInput;
            }
        }

        public virtual IRuntimeSelection Selection
        {
            get { return m_selection; }
        }

        public virtual IRuntimeUndo Undo
        {
            get { return m_undo; }
        }

        public virtual RuntimeTools Tools
        {
            get { return m_tools; }
        }

        public virtual CursorHelper CursorHelper
        {
            get { return m_cursorHelper; }
        }

        public virtual IRuntimeObjects Object
        {
            get { return m_object; }
        }

        public virtual IDragDrop DragDrop
        {
            get { return m_dragDrop; }
        }

        private bool m_isDirty;
        public virtual bool IsDirty
        {
            get { return m_isDirty; }
            set
            {
                if (m_isDirty != value)
                {
                    m_isDirty = value;
                    if (IsDirtyChanged != null)
                    {
                        IsDirtyChanged();
                    }
                }
            }
        }

        public virtual bool IsOpened
        {
            get { return m_isOpened; }
            set
            {
                if (m_isOpened != value)
                {
                    if (IsBusy)
                    {
                        return;
                    }

                    m_isOpened = value;
                    SetInput();
                    if (!m_isOpened)
                    {
                        IsPlaying = false;
                    }

                    if (!m_isOpened)
                    {
                        ActivateWindow(GetWindow(RuntimeWindowType.Game));
                    }

                    if (Root != null)
                    {
                        Root.gameObject.SetActive(m_isOpened);
                    }

                    if (IsOpenedChanged != null)
                    {
                        IsOpenedChanged();
                    }
                    if (m_isOpened)
                    {
                        if (IsOpenedEvent != null)
                        {
                            IsOpenedEvent.Invoke();
                        }
                    }
                    else
                    {
                        if (IsClosedEvent != null)
                        {
                            IsClosedEvent.Invoke();
                        }
                    }
                }
            }
        }

        private bool m_isBusy;
        public virtual bool IsBusy
        {
            get { return m_isBusy; }
            set
            {
                if (m_isBusy != value)
                {
                    m_isBusy = value;
                    if (m_isBusy)
                    {
                        Application.logMessageReceived += OnApplicationLogMessageReceived;
                    }
                    else
                    {
                        Application.logMessageReceived -= OnApplicationLogMessageReceived;
                    }

                    SetInput();
                    if (IsBusyChanged != null)
                    {
                        IsBusyChanged();
                    }
                }
            }
        }

        private void OnApplicationLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                IsBusy = false;
            }
        }

        private bool m_isPlayModeStateChanging;
        public virtual bool IsPlaymodeStateChanging
        {
            get { return m_isPlayModeStateChanging; }
        }

        private bool m_isPlaying;
        public virtual bool IsPlaying
        {
            get
            {
                return m_isPlaying;
            }
            set
            {
                if (IsBusy)
                {
                    return;
                }

                if (!m_isOpened && value)
                {
                    return;
                }

                if (m_isPlaying != value)
                {
                    if (BeforePlaymodeStateChange != null)
                    {
                        BeforePlaymodeStateChange();
                    }

                    m_isPlayModeStateChanging = true;
                    m_isPlaying = value;

                    //Wait for possible cleanup performed in BeforePlaymodeStateChange handler
                    if (gameObject.activeInHierarchy)
                    {
                        StartCoroutine(CoIsPlayingChanged());
                    }
                    else
                    {
                        RaisePlaymodeStateChangeEvents();
                    }

                }
            }
        }

        private IEnumerator CoIsPlayingChanged()
        {
            yield return new WaitForEndOfFrame();

            RaisePlaymodeStateChangeEvents();
        }

        private void RaisePlaymodeStateChangeEvents()
        {
            if (PlaymodeStateChanging != null)
            {
                PlaymodeStateChanging();
            }

            if (PlaymodeStateChanged != null)
            {
                PlaymodeStateChanged();
            }
            m_isPlayModeStateChanging = false;
        }

        public virtual Transform Root
        {
            get { return transform; }
        }

        private static IRTE Instance
        {
            get { return IOC.Resolve<IRTE>("Instance"); }
            set
            {
                if (value != null)
                {
                    IOC.Register<IRTE>("Instance", value);
                }
                else
                {
                    IOC.Unregister<IRTE>("Instance", Instance);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            Debug.Log("RTE Initialized, 2_2");
            IOC.RegisterFallback<IRTE>(RegisterRTE);
        }

        private static IRTE RegisterRTE()
        {
            if (Instance == null)
            {
                GameObject editor = new GameObject("RTE");
                RTEBase instance = editor.AddComponent<RTEBase>();
                instance.BuildUp(editor);
            }
            return Instance;
        }

        protected virtual void BuildUp(GameObject editor)
        {
            GameObject ui = new GameObject("UI");
            ui.transform.SetParent(editor.transform);

            RenderPipelineInfo.ForceUseRenderTextures = false;

            Canvas canvas = ui.AddComponent<Canvas>();
            if (IsVR)
            {
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.worldCamera = Camera.main;
            }
            else
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            canvas.sortingOrder = short.MinValue;
            editor.AddComponent<RTEGraphics>();

            GameObject scene = new GameObject("SceneWindow");
            scene.transform.SetParent(ui.transform, false);
            RuntimeCameraWindow sceneView = scene.AddComponent<RuntimeCameraWindow>();
            sceneView.IsPointerOver = true;
            sceneView.WindowType = RuntimeWindowType.Scene;
            if (Camera.main == null)
            {
                GameObject camera = new GameObject();
                camera.name = "RTE SceneView Camera";
                sceneView.Camera = camera.AddComponent<Camera>();
            }
            else
            {
                sceneView.Camera = Camera.main;
            }

            if (RenderPipelineInfo.Type == RPType.Standard)
            {
                scene.AddComponent<RTEGraphicsLayer>();
            }

            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                eventSystem = editor.AddComponent<EventSystem>();
                if (IsVR)
                {
                    //
                }
                else
                {
                    editor.AddComponent<StandaloneInputModule>();
                }
            }

            RectTransform rectTransform = sceneView.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                RectTransform parentTransform = rectTransform.parent as RectTransform;
                if (parentTransform != null)
                {
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    // rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.pivot = new Vector2(0.0f, 0.0f);
                    rectTransform.offsetMax = new Vector2(0, 0);
                    rectTransform.offsetMin = new Vector2(0, 0);
                }
            }

            m_uiRaycaster = ui.AddComponent<RTEUIRaycaster>();
            m_eventSystem = eventSystem;
        }

        private bool m_isPaused;
        public bool IsApplicationPaused
        {
            get { return m_isPaused; }
        }

        private void OnApplicationQuit()
        {
            m_isPaused = true;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (Application.isEditor)
            {
                return;
            }
            m_isPaused = !hasFocus;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            m_isPaused = pauseStatus;
        }

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Another instance of RTE exists");
                return;
            }
            if (m_useBuiltinUndo)
            {
                m_undo = new RuntimeUndo(this);
            }
            else
            {
                m_undo = new DisabledUndo();
            }

            m_uiRaycaster = IOC.Resolve<IUIRaycaster>();
            if (m_uiRaycaster == null)
            {
                m_uiRaycaster = GetComponentInChildren<IUIRaycaster>();
            }

            IsVR = /*UnityEngine.XR.XRDevice.isPresent*/ false && m_enableVRIfAvailable;
            m_selection = new RuntimeSelection(this);
            m_dragDrop = new DragDrop(this);
            m_object = gameObject.GetComponent<RuntimeObjects>();
            m_disabledInput = new DisabledInput();
            m_activeInput = m_disabledInput;

            Instance = this;

            bool isOpened = m_isOpened;
            m_isOpened = !isOpened;
            IsOpened = isOpened;

            if (m_createHierarchyRoot)
            {
                GameObject hierarchyRoot = GameObject.FindGameObjectWithTag(ExposeToEditor.HierarchyRootTag);
                if (hierarchyRoot == null)
                {
                    hierarchyRoot = new GameObject("HierarchyRoot");
                }
                hierarchyRoot.transform.position = Vector3.zero;
                hierarchyRoot.tag = ExposeToEditor.HierarchyRootTag;
            }
        }

        protected virtual void Start()
        {
            m_input = IOC.Resolve<IInput>();
            if(m_input == null)
            {
                m_input = new InputLow();
            }

            SetInput();

            if (GetComponent<RTEBaseInput>() == null)
            {
                gameObject.AddComponent<RTEBaseInput>();
            }

            if (m_eventSystem == null)
            {
                m_eventSystem = FindObjectOfType<EventSystem>();
                if (m_eventSystem == null)
                {
                    GameObject eventSystem = new GameObject("EventSystem");
                    eventSystem.transform.SetParent(transform, false);
                    m_eventSystem = eventSystem.AddComponent<EventSystem>();
                    eventSystem.AddComponent<StandaloneInputModule>();
                }
            }

            if (m_object == null)
            {
                m_object = gameObject.AddComponent<RuntimeObjects>();
            }
        }

        protected virtual void OnDestroy()
        {
            IsOpened = false;

            if (m_object != null)
            {
                m_object = null;
            }

            if (m_dragDrop != null)
            {
                m_dragDrop.Reset();
            }
            if (((object)Instance) == this)
            {
                Instance = null;
            }
        }

        private void SetInput()
        {
            if (!IsOpened || IsBusy || m_input == null)
            {
                m_activeInput = m_disabledInput;
            }
            else
            {
                m_activeInput = m_input;
            }
        }

        public void RegisterWindow(RuntimeWindow window)
        {
            if (!m_windows.Contains(window.gameObject))
            {
                m_windows.Add(window.gameObject);
            }

            if (WindowRegistered != null)
            {
                WindowRegistered(window);
            }

            m_windowsArray = m_windows.Select(w => w.GetComponent<RuntimeWindow>()).ToArray();

            if (m_windows.Count == 1)
            {
                ActivateWindow(window);
            }
        }

        public void UnregisterWindow(RuntimeWindow window)
        {
            m_windows.Remove(window.gameObject);

            if (IsApplicationPaused)
            {
                return;
            }

            if (WindowUnregistered != null)
            {
                WindowUnregistered(window);
            }

            if (m_activeWindow == window)
            {
                RuntimeWindow activeWindow = m_windows.Select(w => w.GetComponent<RuntimeWindow>()).Where(w => w.WindowType == window.WindowType).FirstOrDefault();
                if (activeWindow == null)
                {
                    activeWindow = m_windows.Select(w => w.GetComponent<RuntimeWindow>()).FirstOrDefault();
                }

                if (IsOpened)
                {
                    ActivateWindow(activeWindow);
                }
            }

            m_windowsArray = m_windows.Select(w => w.GetComponent<RuntimeWindow>()).ToArray();
        }


        protected virtual void Update()
        {
            UpdateCurrentInputField();

            bool mwheel = false;
            if (m_zAxis != Mathf.CeilToInt(Mathf.Abs(Input.GetAxis(InputAxis.Z))))
            {
                mwheel = m_zAxis == 0;
                m_zAxis = Mathf.CeilToInt(Mathf.Abs(Input.GetAxis(InputAxis.Z)));
            }

            bool pointerDownOrUp = Input.GetPointerDown(0) ||
                Input.GetPointerDown(1) ||
                Input.GetPointerDown(2) ||
                Input.GetPointerUp(0);

            if (pointerDownOrUp ||
                mwheel ||
                Input.IsAnyKeyDown() && !IsInputFieldFocused)
            {
                List<RaycastResult> results = new List<RaycastResult>();
                m_uiRaycaster.Raycast(results);

                IEnumerable<Selectable> selectables = results.Select(r => r.gameObject.GetComponent<Selectable>()).Where(s => s != null);
                if (selectables.Count() == 1)
                {
                    Selectable selectable = selectables.First() as Selectable;
                    if (selectable != null)
                    {
                        selectable.Select();
                    }
                }

                foreach (RaycastResult result in results)
                {
                    if (m_windows.Contains(result.gameObject))
                    {
                        RuntimeWindow editorWindow = result.gameObject.GetComponent<RuntimeWindow>();
                        if (pointerDownOrUp || editorWindow.ActivateOnAnyKey)
                        {
                            ActivateWindow(editorWindow);
                            break;
                        }
                    }
                }
            }
        }

        public void UpdateCurrentInputField()
        {
            if (m_eventSystem != null && m_eventSystem.currentSelectedGameObject != null && m_eventSystem.currentSelectedGameObject.activeInHierarchy)
            {
                if (m_eventSystem.currentSelectedGameObject != m_currentSelectedGameObject)
                {
                    m_currentSelectedGameObject = m_eventSystem.currentSelectedGameObject;
                    if (m_currentSelectedGameObject != null)
                    {
                        m_currentInputFieldTMP = m_currentSelectedGameObject.GetComponent<TMP_InputField>();
                        if (m_currentInputFieldTMP == null)
                        {
                            m_currentInputFieldUI = m_currentSelectedGameObject.GetComponent<InputField>();
                        }
                    }
                    else
                    {
                        if (m_currentInputFieldTMP != null)
                        {
                            m_currentInputFieldTMP.DeactivateInputField();
                        }
                        m_currentInputFieldTMP = null;

                        if (m_currentInputFieldUI != null)
                        {
                            m_currentInputFieldUI.DeactivateInputField();
                        }
                        m_currentInputFieldUI = null;
                    }
                }
            }
            else
            {
                m_currentSelectedGameObject = null;
                if (m_currentInputFieldTMP != null)
                {
                    m_currentInputFieldTMP.DeactivateInputField();
                }
                m_currentInputFieldTMP = null;

                if (m_currentInputFieldUI != null)
                {
                    m_currentInputFieldUI.DeactivateInputField();
                }
                m_currentInputFieldUI = null;
            }
        }

        public int GetIndex(RuntimeWindowType windowType)
        {
            IEnumerable<RuntimeWindow> windows = m_windows.Select(w => w.GetComponent<RuntimeWindow>()).Where(w => w.WindowType == windowType).OrderBy(w => w.Index);
            int freeIndex = 0;
            foreach (RuntimeWindow window in windows)
            {
                if (window.Index != freeIndex)
                {
                    return freeIndex;
                }
                freeIndex++;
            }
            return freeIndex;
        }

        public RuntimeWindow GetWindow(RuntimeWindowType window)
        {
            return m_windows.Select(w => w.GetComponent<RuntimeWindow>()).FirstOrDefault(w => w.WindowType == window);
        }

        public virtual void ActivateWindow(RuntimeWindowType windowType)
        {
            RuntimeWindow window = GetWindow(windowType);
            if (window != null)
            {
                ActivateWindow(window);
            }
        }

        public virtual void ActivateWindow(RuntimeWindow window)
        {
            if (m_activeWindow != window && (window == null || window.CanActivate))
            {
                RuntimeWindow deactivatedWindow = m_activeWindow;

                if (ActiveWindowChanging != null)
                {
                    ActiveWindowChanging(window);
                }
                m_activeWindow = window;
                if (ActiveWindowChanged != null)
                {
                    ActiveWindowChanged(deactivatedWindow);
                }
            }
        }

        public virtual void SetPointerOverWindow(RuntimeWindow window)
        {
            m_pointerOverWindow = window;
        }

        public void RegisterCreatedObjects(GameObject[] gameObjects, bool select = true)
        {
            ExposeToEditor[] exposeToEditor = gameObjects.Select(o => o.GetComponent<ExposeToEditor>()).Where(o => o != null).OrderByDescending(o => o.transform.GetSiblingIndex()).ToArray();

            bool isRecording = Undo.IsRecording;
            if (!isRecording)
            {
                Undo.BeginRecord();
            }

            if (exposeToEditor.Length == 0)
            {
                Debug.LogWarning("To register created object GameObject add ExposeToEditor script to it");
            }
            else
            {
                Undo.RegisterCreatedObjects(exposeToEditor);
            }

            if (select)
            {
                Selection.objects = gameObjects;
            }

            if (!isRecording)
            {
                Undo.EndRecord();
            }

            if (ObjectsRegistered != null)
            {
                ObjectsRegistered(gameObjects);
            }
        }

        public void Duplicate(GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0)
            {
                return;
            }

            if (!Undo.Enabled)
            {
                for (int i = 0; i < gameObjects.Length; ++i)
                {
                    GameObject go = gameObjects[i];
                    if (go != null)
                    {
                        ExposeToEditor exposed = go.GetComponent<ExposeToEditor>();
                        if (exposed == null || exposed.CanDuplicate)
                        {
                            Instantiate(go, go.transform.position, go.transform.rotation);
                        }
                    }
                }

                if (ObjectsDuplicated != null)
                {
                    ObjectsDuplicated(gameObjects);
                }

                return;
            }

            List<GameObject> duplicates = new List<GameObject>();
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                GameObject go = gameObjects[i];
                if (go == null)
                {
                    continue;
                }

                ExposeToEditor exposed = go.GetComponent<ExposeToEditor>();
                if (exposed != null && !exposed.CanDuplicate)
                {
                    continue;
                }

                GameObject duplicate = Instantiate(go, go.transform.position, go.transform.rotation);
                duplicate.SetActive(true);
                duplicate.SetActive(go.activeSelf);
                if (go.transform.parent != null)
                {
                    duplicate.transform.SetParent(go.transform.parent, true);
                }

                duplicates.Add(duplicate);
            }

            if (duplicates.Count > 0)
            {
                ExposeToEditor[] exposeToEditor = duplicates.Select(o => o.GetComponent<ExposeToEditor>()).OrderByDescending(o => o.transform.GetSiblingIndex()).ToArray();
                Undo.BeginRecord();
                Undo.RegisterCreatedObjects(exposeToEditor);
                Selection.objects = duplicates.ToArray();
                Undo.EndRecord();
            }

            if (ObjectsDuplicated != null)
            {
                ObjectsDuplicated(gameObjects);
            }
        }

        public void Delete(GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0)
            {
                return;
            }

            if (!Undo.Enabled)
            {
                for (int i = 0; i < gameObjects.Length; ++i)
                {
                    GameObject go = gameObjects[i];
                    if (go != null)
                    {
                        ExposeToEditor exposed = go.GetComponent<ExposeToEditor>();

                        if (exposed == null || exposed.CanDelete)
                        {
                            Destroy(go);
                        }
                    }
                }

                if (ObjectsDeleted != null)
                {
                    ObjectsDeleted(gameObjects);
                }

                return;
            }

            ExposeToEditor[] exposeToEditor = gameObjects.Select(o => o.GetComponent<ExposeToEditor>()).Where(exposed => exposed != null && exposed.CanDelete).OrderByDescending(o => o.transform.GetSiblingIndex()).ToArray();
            if (exposeToEditor.Length == 0)
            {
                return;
            }

            HashSet<GameObject> removeObjectsHs = new HashSet<GameObject>(exposeToEditor.Select(exposed => exposed.gameObject));
            bool isRecording = Undo.IsRecording;
            if (!isRecording)
            {
                Undo.BeginRecord();
            }

            if (Selection.objects != null)
            {
                List<UnityEngine.Object> selection = Selection.objects.ToList();
                for (int i = selection.Count - 1; i >= 0; --i)
                {
                    if (removeObjectsHs.Contains(selection[i]))
                    {
                        selection.RemoveAt(i);
                    }
                }

                Selection.objects = selection.ToArray();
            }

            Undo.DestroyObjects(exposeToEditor);

            if (!isRecording)
            {
                Undo.EndRecord();
            }

            if (ObjectsDeleted != null)
            {
                ObjectsDeleted(gameObjects);
            }
        }

        public void Close()
        {
            IsOpened = false;
            Destroy(gameObject);
        }

        public void AddGameObjectToHierarchy(GameObject go, bool scaleStays = true)
        {
            if (m_createHierarchyRoot)
            {
                GameObject hierarchyRoot = GameObject.FindGameObjectWithTag(ExposeToEditor.HierarchyRootTag);
                if (hierarchyRoot != null)
                {
                    Vector3 localScale = go.transform.localScale;

                    go.transform.SetParent(hierarchyRoot.transform, true);

                    if (scaleStays)
                    {
                        go.transform.localScale = localScale;
                    }
                }
            }
        }
    }
}
