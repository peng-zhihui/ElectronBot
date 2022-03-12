using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using System.Collections;
using System.Reflection;
using System;

using UnityObject = UnityEngine.Object;

namespace Battlehub.RTCommon
{
    public class UndoStack<T> : IEnumerable<UndoStack<T>.Node> where T : class
    {
        public class Node
        {
            public Node Next;
            public Node Prev;
            public T Data;

            public UndoStack<T> Stack
            {
                get { return m_stack; }
            }

            private UndoStack<T> m_stack;
            public Node(UndoStack<T> stack)
            {
                m_stack = stack;
            }
        }

        private Node m_first;
        private Node m_last;
        private Node m_tos;
        private int m_tosIndex;
        private int m_count;
        private T[] m_empty = new T[0];

        public int Count
        {
            get { return m_count; }
        }

        public bool CanPop
        {
            get { return m_tosIndex > 0; }
        }

        public bool CanRestore
        {
            get { return m_tosIndex < m_count; }
        }

        public UndoStack(int size)
        {
            if(size < 1)
            {
                throw new ArgumentOutOfRangeException("size", "size < 1");
            }

            size = size + 1;

            m_first = new Node(this);
            
            Node node = m_first;
            for(int i = 1; i < size; ++i)
            {
                Node next = new Node(this)
                {
                    Prev = node,
                };
                node.Next = next;
                node = next;
            }

            m_last = node;
            m_last.Next = m_first;
            m_first.Prev = m_last;

            m_tos = m_first;
        }

        public void Push(T item, List<T> purgeList = null)
        {
            if(item == null)
            {
                throw new ArgumentNullException("item");
            }

            T[] purgeItems = m_empty;
            if(m_tos == m_last)
            {
                m_last = m_last.Next;
                m_first = m_first.Next;

                if(purgeList != null)
                {
                    if(m_tos.Next.Data != null)
                    {
                        purgeList.Add(m_tos.Next.Data);
                    }
                }
            }
            else
            {
                if(purgeList != null)
                {
                    Node node = m_tos;
                    for(int i = 0; i < (m_count - m_tosIndex); ++i)
                    {
                        if(node.Data != null)
                        {
                            purgeList.Add(node.Data);
                        }
                        
                        node = node.Next;
                    }
                }

                m_tosIndex++;
                m_count = m_tosIndex;
            }

            m_tos.Data = item;
            m_tos = m_tos.Next;
            m_tos.Data = null;
        }

        public T Pop()
        {
            if (!CanPop)
            {
                throw new InvalidOperationException("Stack is empty");
            }

            m_tos = m_tos.Prev;
            m_tosIndex--;
            return m_tos.Data;
        }

        public T Peek()
        {
            if (!CanPop)
            {
                throw new InvalidOperationException("Stack is empty");
            }

            return m_tos.Prev.Data;
        }

        public T Restore()
        {
            if (!CanRestore)
            {
                throw new InvalidOperationException("Nothing to restore");
            }

            T restored = m_tos.Data;
            m_tos = m_tos.Next;
            m_tosIndex++;
            return restored;
        }

        public void Clear()
        {
            Node node = m_first;
            do
            {
                node.Data = null;
                node = node.Next;
            }
            while (node.Prev != m_last);

            m_tosIndex = 0;
            m_count = 0;
            m_tos = m_first;
        }

        public Node Find(T data)
        {
            Node node = m_first;
            do
            {
                if (node.Data == data)
                {
                    return node;
                }
                node = node.Next;
            }
            while (node != m_first);
            return null;
        }

        public T Purge(Node node)
        {
            if(node.Stack != this)
            {
                throw new ArgumentException("node does not belong to this stack");
            }

            if(m_count == 0)
            {
                throw new InvalidOperationException("stack is empty");
            }

            if(node.Data == null)
            {
                return null;
            }

            if(node != m_last) 
            {
                if (node == m_first)
                {
                    m_first = m_first.Next;
                }

                if (m_tos == node)
                {
                    m_tos = m_tos.Next;
                }

                Node prev = node.Prev;
                Node next = node.Next;
                if(prev != next)
                {
                    prev.Next = next;
                    next.Prev = prev;
                }

                m_last.Next = node;
                node.Prev = m_last;

                m_last = node;
                m_last.Next = m_first;
                m_first.Prev = m_last;
            }

            m_count--;
            if (m_count < m_tosIndex)
            {
                m_tosIndex = m_count;
            }

            T data = m_last.Data;
            m_last.Data = null;
            return data;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _GetEnumerator();
        }

        IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
        {
            return _GetEnumerator();
        }

        private IEnumerator<Node> _GetEnumerator()
        {
            int index = 0;
            Node node = m_first;
            do
            {
                if(index == m_count)
                {
                    yield break;
                }
                index++;

                yield return node;
                node = node.Next;
            }
            while (node != m_first);
        }
    }

    public delegate bool UndoRedoCallback(Record record);
    public delegate void PurgeCallback(Record record);
    public delegate bool EraseReferenceCallback(Record record, object oldRef, object newRef);

    public class Record
    {
        private object m_oldState;
        private object m_newState;
        private object m_target;

        /// <summary>
        /// Apply changes to object. Return true if object state has been changed
        /// </summary>
        private UndoRedoCallback m_redoCallback;

        /// <summary>
        /// Revert object state changes. Return true if object state has been changed
        /// </summary>
        private UndoRedoCallback m_undoCallback;

        /// <summary>
        /// Cleanup. Record is removed from stack and object state could not be reverted anymore.
        /// </summary>
        private PurgeCallback m_purgeCallback;

        /// <summary>
        /// Erase/Repalce reference to object. Return false is record is still valid and can change object state, otherwise return true.
        /// </summary>
        private EraseReferenceCallback m_eraseCallback;

        public object Target
        {
            get { return m_target; }
            set { m_target = value; }
        }

        public object OldState
        {
            get { return m_oldState; }
            set { m_oldState = value; }
        }

        public object NewState
        {
            get { return m_newState; }
            set { m_newState = value; }
        }

        public Record(object target, object newState, object oldState, UndoRedoCallback redoCallback, UndoRedoCallback undoCallback, PurgeCallback purgeCallback, EraseReferenceCallback eraseCallback)
        {
            if (redoCallback == null)
            {
                throw new ArgumentNullException("redoCallback");
            }

            if(undoCallback == null)
            {
                throw new ArgumentNullException("undoCallback");
            }

            m_target = target;
            m_redoCallback = redoCallback;
            m_undoCallback = undoCallback;
            m_purgeCallback = purgeCallback;
            m_eraseCallback = eraseCallback;
            m_newState = newState;
            m_oldState = oldState;
        }

        public bool Undo()
        {
            return m_undoCallback(this);
        }

        public bool Redo()
        {
            return m_redoCallback(this);
        }

        public void Purge()
        {
            if(m_purgeCallback != null)
            {
                m_purgeCallback(this);
            }
            
        }

        public bool Erase(object oldRef, object newRef)
        {
            bool result = false;
            if(m_eraseCallback != null)
            {
               result = m_eraseCallback(this, oldRef, newRef);
            }
            return result;
        }
    }


    public delegate void RuntimeUndoEventHandler();
    public interface IRuntimeUndo
    {
        event RuntimeUndoEventHandler BeforeUndo;
        event RuntimeUndoEventHandler UndoCompleted;
        event RuntimeUndoEventHandler BeforeRedo;
        event RuntimeUndoEventHandler RedoCompleted;
        event RuntimeUndoEventHandler StateChanged;

        bool Enabled
        {
            get;
            set;
        }

    
        bool CanUndo
        {
            get;
        }

        bool CanRedo
        {
            get;
        }

        bool IsRecording
        {
            get;
        }

        void BeginRecord();
        void EndRecord();
        void Redo();
        void Undo();
        void Purge();
        void Erase(object oldRef, object newRef, bool ignoreLock = false);

        void Store();
        void Restore();

        Record CreateRecord(UndoRedoCallback redoCallback, UndoRedoCallback undoCallback, PurgeCallback purgeCallback = null, EraseReferenceCallback eraseCallback = null);
        Record CreateRecord(object target, object newState, object oldState, UndoRedoCallback redoCallback, UndoRedoCallback undoCallback, PurgeCallback purgeCallback = null, EraseReferenceCallback eraseCallback = null);
        void Select(IRuntimeSelection selection, UnityObject[] objects, UnityObject activeObject);

        void RegisterCreatedObjects(ExposeToEditor[] createdObjects, Action afterRedo = null, Action afterUndo = null);
        void DestroyObjects(ExposeToEditor[] destoryedObjects, Action afterRedo = null, Action afterUndo = null);

        void RecordValue(object target, MemberInfo memberInfo, Action afterRedo = null, Action afterUndo = null);
        void RecordValue(object target, object accessor, MemberInfo memberInfo, Action afterRedo = null, Action afterUndo = null);
        void BeginRecordValue(object target, MemberInfo memberInfo);
        void BeginRecordValue(object target, object accessor, MemberInfo memberInfo);
        void EndRecordValue(object target, MemberInfo memberInfo, Action afterRedo = null, Action afterUndo = null);
        void EndRecordValue(object target, object accessor, MemberInfo memberInfo, Action<object, object> targetErased = null, Action afterRedo = null, Action afterUndo = null);

        void BeginRecordTransform(Transform target, Transform parent = null, int siblingIndex = -1);
        void EndRecordTransform(Transform target, Transform parent = null, int siblingIndex = -1);

        void AddComponent(ExposeToEditor obj, Type type);
        void DestroyComponent(Component destroy, MemberInfo[] memberInfo);

        #region Obsolete
        [Obsolete("Use void Select(IRuntimeSelection selection, UnityObject[] objects, UnityObject activeObject) instead")]
        void Select(UnityObject[] objects, UnityObject activeObject);
        #endregion
    }


    /// <summary>
    /// Class for handling undo and redo operations
    /// </summary>
    public class RuntimeUndo : IRuntimeUndo
    {
        private class SelectionState
        {
            public UnityObject ActiveObject;
            public UnityObject[] Objects;

            public SelectionState(UnityObject[] objects, UnityObject activeObject)
            {
                ActiveObject = activeObject;
                if (objects != null)
                {
                    Objects = objects.ToArray();
                }
                else
                {
                    Objects = null;
                }
            }

            public SelectionState(IRuntimeSelection selection)
            {
                ActiveObject = selection.activeObject;
                if(selection.objects != null)
                {
                    Objects = selection.objects.ToArray();
                }
                else
                {
                    Objects = null;
                }
            }
        }

        public class SetValuesState
        {
            public object Accessor;
            public MemberInfo[] MemberInfo;
            public object[] Values;

            public SetValuesState(object accessor, MemberInfo[] memberInfo, object[] values)
            {
                Accessor = accessor;
                MemberInfo = memberInfo;
                Values = values;
            }
        }

        private class TransformState
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;
            public Transform parent;
            public int siblingIndex = -1;
            public bool applyOnRedo;
        }

        public bool Enabled
        {
            get;
            set;
        }

        protected bool Locked
        {
            get;
            private set;
        }

        public bool CanUndo
        {
            get { return m_stack.CanPop; }
        }

        public bool CanRedo
        {
            get { return m_stack.CanRestore; }
        }

        public bool IsRecording
        {
            get { return m_group != null; }
        }

        public event RuntimeUndoEventHandler BeforeUndo;
        public event RuntimeUndoEventHandler UndoCompleted;
        public event RuntimeUndoEventHandler BeforeRedo;
        public event RuntimeUndoEventHandler RedoCompleted;
        public event RuntimeUndoEventHandler StateChanged;

        public const int Limit = 8192;
        
        private Dictionary<object, Dictionary<MemberInfo, object>> m_objToValue;

        private List<Record> m_group;
        private UndoStack<Record[]> m_stack;
        private Stack<UndoStack<Record[]>> m_stacks;
        private List<Record[]> m_purgeRecords;
        private List<UndoStack<Record[]>.Node> m_purgeNodes;


        private IRTE m_rte;
        public RuntimeUndo(IRTE rte)
        {
            m_rte = rte;
            Reset();
        }

        public void Reset()
        {
            Enabled = true;
            m_group = null;
            m_stack = new UndoStack<Record[]>(Limit);
            m_stacks = new Stack<UndoStack<Record[]>>();
            m_purgeRecords = new List<Record[]>();
            m_purgeNodes = new List<UndoStack<Record[]>.Node>();
            m_objToValue = new Dictionary<object, Dictionary<MemberInfo, object>>();
        }

        public void BeginRecord()
        {
            if (!Enabled)
            {
                return;
            }
            if(Locked)
            {
                return;
            }

            m_group = new List<Record>();
        }

        public void EndRecord()
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }

            if (m_group != null)
            {
                m_stack.Push(m_group.ToArray(), m_purgeRecords);

                for(int i = 0; i < m_purgeRecords.Count; ++i)
                {
                    Record[] purgeRecords = m_purgeRecords[i];
                    if (purgeRecords != null)
                    {
                        for (int j = 0; j < purgeRecords.Length; ++j)
                        {
                            Record record = purgeRecords[j];
                            record.Purge();
                        }
                    }
                }
                m_purgeRecords.Clear();
                m_markAsDestroyedDuringLastOperation.Clear();

                if (StateChanged != null)
                {
                    StateChanged();
                }
            }
            m_group = null;
        }

        public void Redo()
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }

            if (!m_stack.CanRestore)
            {
                return;
            }

            try
            {
                Locked = true;
                DoRedo();
            }
            finally
            {
                Locked = false;
            } 
        }

        private void DoRedo()
        {
            if (BeforeRedo != null)
            {
                BeforeRedo();
            }

            bool somethingHasChanged;
            do
            {
                somethingHasChanged = false;
                Record[] records = m_stack.Restore();
                for (int i = 0; i < records.Length; ++i)
                {
                    Record record = records[i];
                    somethingHasChanged |= record.Redo();
                }
            }
            while (!somethingHasChanged && m_stack.CanRestore);

            if (RedoCompleted != null)
            {
                RedoCompleted();
            }
        }

        public void Undo()
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }

            if (!m_stack.CanPop)
            {
                return;
            }

            try
            {
                Locked = true;
                DoUndo();
            }
            finally
            {
                Locked = false;
            }
        }

        private void DoUndo()
        {
            if (BeforeUndo != null)
            {
                BeforeUndo();
            }

            bool somethingHasChanged;
            do
            {
                somethingHasChanged = false;
                Record[] records = m_stack.Pop();

                for (int i = records.Length - 1; i >= 0; --i)
                {
                    Record record = records[i];
                    somethingHasChanged |= record.Undo();
                }
            }
            while (!somethingHasChanged && m_stack.CanPop);

            if (UndoCompleted != null)
            {
                UndoCompleted();
            }
        }

        public void Purge()
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }

            _Purge();

            if (StateChanged != null)
            {
                StateChanged();
            }
        }

        private void _Purge()
        {
            foreach (UndoStack<Record[]>.Node node in m_stack)
            {
                if (node.Data != null)
                {
                    for (int i = 0; i < node.Data.Length; ++i)
                    {
                        Record record = node.Data[i];
                        record.Purge();
                    }
                }
            }
            m_stack.Clear();
            m_group = null;
            if (m_objToValue.Count > 0)
            {
                Debug.LogWarning("Unifished RecordValue operations exists.");
                m_objToValue = new Dictionary<object, Dictionary<MemberInfo, object>>();
            }
        }

        public void Erase(object oldRef, object newRef, bool ignoreLock = false)
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked && !ignoreLock)
            {
                return;
            }

            if (m_objToValue.Count > 0)
            {
                Debug.LogWarning("Unifished RecordValue operations exists.");
                m_objToValue = new Dictionary<object, Dictionary<MemberInfo, object>>();
            }

            foreach (UndoStack<Record[]>.Node node in m_stack)
            {
                if (node.Data != null)
                {
                    int erased = 0;
                    for (int i = 0; i < node.Data.Length; ++i)
                    {
                        Record record = node.Data[i];
                        if(record.Erase(oldRef, newRef))
                        {
                            erased++;
                        }
                    }

                    if(erased > 0 && node.Data.Length == erased)
                    {
                        m_purgeNodes.Add(node);
                    }
                }
            }

            for(int i = 0; i < m_purgeNodes.Count; ++i)
            {
                UndoStack<Record[]>.Node purgeNode = m_purgeNodes[i];
                if (purgeNode != null)
                {
                    for (int j = 0; j < purgeNode.Data.Length; ++j)
                    {
                        purgeNode.Data[j].Purge();
                    }
                }

                m_stack.Purge(purgeNode);
            }

            m_purgeNodes.Clear();

            if (StateChanged != null)
            {
                StateChanged();
            }
        }

        public void Store()
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }
            m_stacks.Push(m_stack);
            m_stack = new UndoStack<Record[]>(Limit);
            if (StateChanged != null)
            {
                StateChanged();
            }
        }

        public void Restore()
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }

            if (m_stacks.Count > 0)
            {
                _Purge();

                m_stack = m_stacks.Pop();
                if (StateChanged != null)
                {
                    StateChanged();
                }
            }
        }

        public Record CreateRecord(UndoRedoCallback redoCallback, UndoRedoCallback undoCallback, PurgeCallback purgeCallback = null, EraseReferenceCallback eraseCallback = null)
        {
            return CreateRecord(null, null, null, redoCallback, undoCallback, purgeCallback, eraseCallback);
        }

        public Record CreateRecord(object target, object newState, object oldState, UndoRedoCallback redoCallback, UndoRedoCallback undoCallback, PurgeCallback purgeCallback = null, EraseReferenceCallback eraseCallback = null)
        {
            if (!Enabled)
            {
                return null;
            }
            if (Locked)
            {
                return null;
            }

            if (purgeCallback == null)
            {
                purgeCallback = rec => { };
            }

            Record record = new Record(target, newState, oldState, redoCallback, undoCallback, purgeCallback, eraseCallback); 
            if (m_group != null)
            {
                m_group.Add(record);
            }
            else
            {
                m_stack.Push(new[] { record }, m_purgeRecords);

                //these lines causes wrong behavior.Remove->undo->remove->undo again and objects are not recovered
                //reason: the same action from different records undone.
                for (int i = 0; i < m_purgeRecords.Count; ++i)
                {
                    Record[] purgeItems = m_purgeRecords[i];
                    if (purgeItems != null)
                    {
                        for (int j = 0; j < purgeItems.Length; ++j)
                        {
                            purgeItems[j].Purge();
                        }
                    }
                }


                m_purgeRecords.Clear();

                if (StateChanged != null)
                {
                    StateChanged();
                }
            }
            return record;
        }

        private static bool HasSelectionChanged(UnityObject[] newObjects, UnityObject newActiveObject, IRuntimeSelection selection)
        {
            return HasSelectionChanged(newObjects, newActiveObject, selection.objects, selection.activeObject);
        }

        private static bool HasSelectionChanged(UnityObject[] newObjects, UnityObject newActiveObject, UnityObject[] objects, UnityObject activeObject)
        {
            if(activeObject != newActiveObject)
            {
                return true;
            }

            if(objects == newObjects)
            {
                return false;
            }

            if(objects == null || newObjects == null)
            {
                return true;
            }

            if(objects.Length != newObjects.Length)
            {
                return true;
            }

            for(int i = 0; i < objects.Length; ++i)
            {
                if(objects[i] != newObjects[i])
                {
                    return true;
                }
            }

            return false;
        }

        private static void EraseFromSelection(SelectionState state, object newReference, object oldReference)
        {
            if((object)state.ActiveObject == oldReference)
            {
                state.ActiveObject = newReference as UnityObject;
            }

            bool hasNulls = false;
            if(state.Objects != null)
            {
                for(int i = 0; i < state.Objects.Length; ++i)
                {
                    object reference = state.Objects[i];
                    if(reference == oldReference)
                    {
                        state.Objects[i] = newReference as UnityObject;
                        if(state.Objects[i] == null)
                        {
                            hasNulls = true;
                        }
                    }
                }
            }

            if(hasNulls)
            {
                state.Objects = state.Objects.Where(o => o != null).ToArray();
                if(state.Objects.Length == 0)
                {
                    state.Objects = null;
                }
            }
        }

        private bool ApplySelection(SelectionState state, IRuntimeSelection selection)
        {
            bool hasChanged = HasSelectionChanged(state.Objects, state.ActiveObject, selection);

            if (hasChanged)
            {
                selection.Select(state.ActiveObject, state.Objects);
            }

            return hasChanged;
        }

        public void Select(IRuntimeSelection selection, UnityObject[] objects, UnityObject activeObject)
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }
            if (!HasSelectionChanged(objects, activeObject, selection))
            {
                return;
            }

            Record newRecord = CreateRecord(selection,
                new SelectionState(objects, activeObject),
                new SelectionState(selection),
                record => ApplySelection((SelectionState)record.NewState, (IRuntimeSelection)record.Target),
                record => ApplySelection((SelectionState)record.OldState, (IRuntimeSelection)record.Target),
                record => { /*do nothing*/ },
                (record, oldReference, newReference) =>
                {
                    SelectionState newState = (SelectionState)record.NewState;
                    SelectionState oldState = (SelectionState)record.OldState;
                    EraseFromSelection(oldState, newReference, oldReference);
                    EraseFromSelection(newState, newReference, oldReference);

                    bool purge = false;
                    if (!HasSelectionChanged(newState.Objects, newState.ActiveObject, oldState.Objects, oldState.ActiveObject))
                    {
                        purge = true;
                    }
                    return purge;
                });

            if(newRecord != null)
            {
                newRecord.Redo();
            }
        }


        
        private bool MarkAsDestroyed(Record record, bool destroyed)
        {
            ExposeToEditor[] objects = (ExposeToEditor[])record.Target;
            for (int i = 0; i < objects.Length; ++i)
            {
                ExposeToEditor obj = objects[i];
                if(obj != null)
                {
                    obj.MarkAsDestroyed = destroyed;
                }
            }
            return true;
        }

        private void PurgeMarkedAsDestoryed(Record record)
        {
            ExposeToEditor[] objects = (ExposeToEditor[])record.Target;
            for (int i = 0; i < objects.Length; ++i)
            {
                ExposeToEditor obj = objects[i];

                if (obj != null && obj.MarkAsDestroyed)
                {
                    if(!m_markAsDestroyedDuringLastOperation.Contains(obj))
                    {
                        UnityObject.DestroyImmediate(obj.gameObject);
                    }
                }
            }
        }

        private static bool EraseMarkedAsDestroyed(Record record, object newReference, object oldReference)
        {
            ExposeToEditor[] objects = (ExposeToEditor[])record.Target;
            bool hasNulls = false;
            for (int i = 0; i < objects.Length; ++i)
            {
                ExposeToEditor obj = objects[i];
                if (obj == null)
                {
                    continue;
                }
                if (oldReference is GameObject)
                {
                    if ((object)obj.gameObject == oldReference)
                    {
                        objects[i] = null;
                        GameObject newRef = newReference as GameObject;
                        if (newRef != null)
                        {
                            objects[i] = newRef.GetComponent<ExposeToEditor>();
                        }
                    }
                }

                if (objects[i] == null)
                {
                    hasNulls = true;
                }
            }

            if (hasNulls)
            {
                objects = objects.Where(obj => obj != null).ToArray();
                record.Target = objects;
            }

            return objects.Length == 0;
        }

        public void RegisterCreatedObjects(ExposeToEditor[] createdObjects, Action afterRedo = null, Action afterUndo = null)
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }
            Record newRecord = CreateRecord(createdObjects, false, true,
                record => { bool result = MarkAsDestroyed(record, (bool)record.NewState); afterRedo?.Invoke(); return result; },
                record => { bool result = MarkAsDestroyed(record, (bool)record.OldState); afterUndo?.Invoke(); return result; },
                record => PurgeMarkedAsDestoryed(record),
                (record, oldReference, newReference) => EraseMarkedAsDestroyed(record, newReference, oldReference));

            if(newRecord != null)
            {
                newRecord.Redo();
            }
        }

        //To prevent gameobject from being destroyed during purge operation (in case if they are referenced somewhere in the stack)
        private HashSet<ExposeToEditor> m_markAsDestroyedDuringLastOperation = new HashSet<ExposeToEditor>();
        public void DestroyObjects(ExposeToEditor[] destoryedObjects, Action afterRedo = null, Action afterUndo = null)
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }

            for(int i = 0; i < destoryedObjects.Length; ++i)
            {
                if (!m_markAsDestroyedDuringLastOperation.Contains(destoryedObjects[i]))
                {
                    m_markAsDestroyedDuringLastOperation.Add(destoryedObjects[i]);
                }
            }

            Record newRecord = CreateRecord(destoryedObjects, true, false,
               record => { bool result = MarkAsDestroyed(record, (bool)record.NewState); afterRedo?.Invoke(); return result; },
               record => { bool result = MarkAsDestroyed(record, (bool)record.OldState); afterUndo?.Invoke(); return result; },
               record => PurgeMarkedAsDestoryed(record),
               (record, oldReference, newReference) => EraseMarkedAsDestroyed(record, newReference, oldReference));

            if (newRecord != null)
            {
                newRecord.Redo();
            }

            if(!IsRecording)
            {
                m_markAsDestroyedDuringLastOperation.Clear();
            }    
        }

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }


        private static Array DuplicateArray(Array array)
        {
            Array newArray = (Array)Activator.CreateInstance(array.GetType(), array.Length);
            if (array != null)
            {
                for (int i = 0; i < newArray.Length; ++i)
                {
                    newArray.SetValue(array.GetValue(i), i);
                }
            }

            return array;
        }

        private object GetValue(object accessor, MemberInfo m)
        {
            PropertyInfo p = m as PropertyInfo;
            if (p != null)
            {
                if (accessor == null || (accessor is UnityObject) && null == (UnityObject)accessor)
                {
                    return GetDefault(p.PropertyType);
                }

                object val = p.GetValue(accessor, null);
                if (val is Array)
                {
                    val = DuplicateArray((Array)val);
                }
                return val;
            }

            FieldInfo f = m as FieldInfo;
            if (f != null)
            {
                if (accessor == null || (accessor is UnityObject) && null == (UnityObject)accessor)
                {
                    return GetDefault(f.FieldType);
                }
                object val = f.GetValue(accessor);
                if (val is Array)
                {
                    val = DuplicateArray((Array)val);
                }
                return val;
            }

            if(m is MethodInfo)
            {
                return null;
            }

            throw new ArgumentException("member is not FieldInfo and is not PropertyInfo", "m");
        }

        private object[] GetValues(object accessor, MemberInfo[] memberInfo)
        {
            object[] values = new object[memberInfo.Length];
            for(int i = 0; i < memberInfo.Length; ++i)
            {
                values[i] = GetValue(accessor, memberInfo[i]);
            }
            return values;
        }

        private void AssignValue(object accessor, MemberInfo m, object value)
        {
            if(accessor == null || (accessor is UnityObject) && null == (UnityObject)accessor)
            {
                return;
            }
            PropertyInfo p = m as PropertyInfo;
            if (p != null)
            {
                p.SetValue(accessor, value, null);
                return;
            }

            FieldInfo f = m as FieldInfo;
            if (f != null)
            {
                f.SetValue(accessor, value);
                return;
            }

            if(m is MethodInfo)
            {
                return;
            }

            throw new ArgumentException("member is not FieldInfo and is not PropertyInfo", "m");
        }

        private void AssingValues(object accessor, MemberInfo[] memberInfo, object[] values)
        {
            for (int i = 0; i < memberInfo.Length; ++i)
            {
                AssignValue(accessor, memberInfo[i], values[i]);
            }
        }

        private bool AssignValues(SetValuesState state, Action callback)
        {
            if(state.Accessor == null || (state.Accessor is UnityObject) && null == (UnityObject)state.Accessor)
            {
                return false;
            }

            bool isValueChanged = false;
            for(int i = 0; i < state.Values.Length; ++i)
            {
                object oldValue = GetValue(state.Accessor, state.MemberInfo[i]);
                object newValue = state.Values[i];
                if(IsValueChanged(oldValue, newValue))
                {
                    isValueChanged = true;
                }
                AssignValue(state.Accessor, state.MemberInfo[i], newValue);
            }

            if(callback != null)
            {
                callback();
            }
            
            return isValueChanged;
        }

        private static bool IsValueChanged(object a, object b)
        {
            if(a == null && b == null)
            {
                return false;
            }

            if(a != null && b != null)
            {
                if(a is Vector3 && b is Vector3)
                {
                    return (Vector3)a != (Vector3)b;
                }
                else if(a is Vector2 && b is Vector2)
                {
                    return (Vector2)a != (Vector2)b;
                }
                else if(a is Vector4 && b is Vector4)
                {
                    return (Vector4)a != (Vector4)b;
                }

                return !a.Equals(b);
            }

            return true;
        }

        

        private void EraseFromSetValuesState(SetValuesState state, object newReference, object oldReference)
        {
            if(state.Accessor == oldReference)
            {
                state.Accessor = newReference;
            }

            bool hasNulls = false;
            if(state.Values != null)
            {
                for(int i = 0; i< state.Values.Length; ++i)
                {
                    object reference = state.Values[i];
                    if(reference == oldReference)
                    {
                        state.Values[i] = newReference;
                        if (newReference == null)
                        {
                            state.MemberInfo[i] = null;
                            hasNulls = true;
                        }
                    }
                    else if(reference is IList)
                    {
                        IList list = (IList)reference;
                        for(int j = 0; j < list.Count; ++j)
                        {
                            if(list[j] == oldReference)
                            {
                                list[j] = newReference;
                            }
                        }
                    }
                }
            }

            if(hasNulls)
            {
                state.Values = state.Values.Where(o => o != null).ToArray();
                state.MemberInfo = state.MemberInfo.Where(o => o != null).ToArray();
            }
        }

        private void RecordValues(object target, object accessor, MemberInfo[] memberInfo, object[] oldValues, Action<object, object> targetErased, Action afterRedo, Action afterUndo)
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }
            Record newRecord = CreateRecord(target,
                new SetValuesState(accessor, memberInfo, GetValues(accessor, memberInfo)),
                new SetValuesState(accessor, memberInfo, oldValues),
                record => AssignValues((SetValuesState)record.NewState, afterRedo),
                record => AssignValues((SetValuesState)record.OldState, afterUndo),
                record => { },
                (record, oldReference, newReference) =>
                {
                    if (record.Target == oldReference)
                    {
                        record.Target = newReference;
                        if (targetErased != null)
                        {
                            targetErased(accessor, record.Target);
                        }
                        if (record.Target == null)
                        {
                            return true;
                        }
                    }

                    SetValuesState newState = (SetValuesState)record.NewState;
                    SetValuesState oldState = (SetValuesState)record.OldState;

                    EraseFromSetValuesState(newState, newReference, oldReference);
                    EraseFromSetValuesState(oldState, newReference, oldReference);

                    if (newState.Values.Length == 0 && oldState.Values.Length == 0)
                    {
                        return true;
                    }

                    if(newState.Accessor == null)
                    {
                        return true;
                    }

                    return false;
                });
        }

        private void RecordValues(object target, MemberInfo[] memberInfo, object[] oldValues, Action afterRedo, Action afterUndo)
        {
            RecordValues(target, target, memberInfo, oldValues, null, afterRedo, afterUndo);
        }

        private void RecordValue(object target, object accessor, MemberInfo memberInfo, object oldValue, Action<object, object> targetErased, Action afterRedo, Action afterUndo)
        {
            RecordValues(target, accessor, new[] { memberInfo }, new[] { oldValue }, targetErased, afterRedo, afterUndo);
        }

        private bool ApplyRecordedValue(Record record, MemberInfo memberInfo)
        {
            object obj = record.Target;
            if (obj == null)
            {
                return false;
            }

            if (obj is UnityObject)
            {
                if (((UnityObject)obj) == null)
                {
                    return false;
                }
            }

            object state = record.NewState;
            object value = GetValue(obj, memberInfo);

            bool hasChanged = true;
            if (state == null && value == null)
            {
                hasChanged = false;
            }
            else if (state != null && value != null)
            {
                if (state is IEnumerable<object>)
                {
                    IEnumerable<object> eState = (IEnumerable<object>)state;
                    IEnumerable<object> eValue = (IEnumerable<object>)value;
                    hasChanged = !eState.SequenceEqual(eValue);
                }
                else
                {
                    hasChanged = !state.Equals(value);
                }

            }

            if (hasChanged)
            {
                AssignValue(obj, memberInfo, state);
            }
            return hasChanged;
        }

        public void RecordValue(object target, MemberInfo memberInfo, Action afterRedo, Action afterUndo)
        {
            RecordValue(target, target, memberInfo, GetValue(target, memberInfo), null, afterRedo, afterUndo);
        }

        public void RecordValue(object target, object accessor, MemberInfo memberInfo, Action afterRedo, Action afterUndo)
        {
            RecordValue(target, accessor, memberInfo, GetValue(accessor, memberInfo), null, afterRedo, afterUndo);
        }

        public void BeginRecordValue(object target, MemberInfo memberInfo)
        {
            BeginRecordValue(target, target, memberInfo);
        }

        public void BeginRecordValue(object target, object accessor, MemberInfo memberInfo)
        {
            if(!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }
            Dictionary<MemberInfo, object> memberInfoToValue;
            if(!m_objToValue.TryGetValue(target, out memberInfoToValue))
            {
                memberInfoToValue = new Dictionary<MemberInfo, object>();
                m_objToValue.Add(target, memberInfoToValue);
            }

            if(memberInfoToValue.ContainsKey(memberInfo))
            {
                Debug.LogWarning("Unfinished record value operation for " + memberInfo.Name + " exist");
            }

            memberInfoToValue[memberInfo] = GetValue(accessor, memberInfo);
        }

        public void EndRecordValue(object target, MemberInfo memberInfo, Action afterRedo, Action afterUndo)
        {
            EndRecordValue(target, target, memberInfo, null, afterRedo, afterUndo);
        }

        public void EndRecordValue(object target, object accessor, MemberInfo memberInfo, Action<object, object> targetErased, Action afterRedo, Action afterUndo)
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }
            Dictionary<MemberInfo, object> memberInfoToValue;
            if (!m_objToValue.TryGetValue(target, out memberInfoToValue))
            {
                return;
            }

            object oldValue;
            if(!memberInfoToValue.TryGetValue(memberInfo, out oldValue))
            {
                return;
            }

            memberInfoToValue.Remove(memberInfo);
            if(memberInfoToValue.Count == 0)
            {
                m_objToValue.Remove(target);
            }

            RecordValue(target, accessor, memberInfo, oldValue, targetErased, afterRedo, afterUndo);
        }

        public void BeginRecordTransform(Transform target, Transform parent = null, int siblingIndex = -1)
        {
            RecordTransform(false, target, parent, siblingIndex);
        }

        public void EndRecordTransform(Transform target, Transform parent = null, int siblingIndex = -1)
        {
            RecordTransform(true, target, parent, siblingIndex);
        }
  
        private void RecordTransform(bool applyOnRedo, Transform target, Transform parent = null, int siblingIndex = -1)
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }
            TransformState newState = new TransformState { position = target.position, rotation = target.rotation, scale = target.localScale };
            newState.parent = parent;
            newState.siblingIndex = siblingIndex;
            newState.applyOnRedo = applyOnRedo;

            CreateRecord(target, newState, null,
                record => ApplyTransform(record, true),
                record => ApplyTransform(record, false),
                record => { },
                (record, oldReference, newReference) =>
                {
                    return false;
                });
        }

        private static bool ApplyTransform(Record record, bool isRedo)
        {
            Transform transform = (Transform)record.Target;
            if (!transform)
            {
                return false;
            }

            TransformState state = (TransformState)record.NewState;
            if(state.applyOnRedo != isRedo)
            {
                return false;
            }
            bool hasChanged = transform.position != state.position ||
                              transform.rotation != state.rotation ||
                              transform.localScale != state.scale;

            bool trsOnly = state.siblingIndex == -1;
            if (!trsOnly)
            {
                int siblingIndex = transform.GetSiblingIndex();
                hasChanged = hasChanged || transform.parent != state.parent || siblingIndex != state.siblingIndex;
            }

            if (hasChanged)
            {
                //Transform prevParent = transform.parent;
                if (!trsOnly)
                {
                    transform.SetParent(state.parent, true);
                    transform.SetSiblingIndex(state.siblingIndex);
                }

                transform.position = state.position;
                transform.rotation = state.rotation;
                transform.localScale = state.scale;
            }
            return hasChanged;
        }

        private static Component AddComponent(GameObject go, Type type)
        {
            ExposeToEditor exposeToEditor = go.GetComponent<ExposeToEditor>();

            if (type == typeof(Rigidbody))
            {
                ExposeToEditor[] children = go.GetComponentsInChildren<ExposeToEditor>(true);
                for (int i = 0; i < children.Length; ++i)
                {
                    Collider[] colliders = children[i].Colliders;
                    if (colliders == null)
                    {
                        continue;
                    }

                    for (int j = 0; j < colliders.Length; ++j)
                    {
                        Collider collider = colliders[j];
                        if (collider is MeshCollider)
                        {
                            MeshCollider mc = (MeshCollider)collider;
                            mc.convex = true;
                        }
                    }
                }
            }

            Component component = exposeToEditor.AddComponent(type);
            if (component is Rigidbody)
            {
                Rigidbody rb = (Rigidbody)component;
                rb.isKinematic = true;
            }
            return component;
        }

        public void AddComponent(ExposeToEditor obj, Type type)
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }
            Record newRecord = CreateRecord(obj, type, null,
            record =>
            {
                ExposeToEditor target = (ExposeToEditor)record.Target;
                if (target == null)
                {
                    return false;
                }
                Type componentType = (Type)record.NewState;
                Component component = AddComponent(target.gameObject, componentType);
                if (record.OldState != null)
                {
                    Erase(record.OldState, component, true);
                }
                record.OldState = component;
                return true;
            },
            record =>
            {
                Component component = (Component)record.OldState;
                object replacement = new object();
                Erase(component, replacement, true);
                if (component != null)
                {
                    UnityObject.Destroy(component);
                }
                record.OldState = replacement;
                return true;
            },
            record =>
            {
                //if(record.OldState != null && (record.OldState is Component))
                //{
                //    Erase(record.OldState, null);
                //}
            },
            (record, oldReference, newReference) =>
            {
                ExposeToEditor target = record.Target as ExposeToEditor;
                if (target != null)
                {
                    if (target.gameObject == (object)oldReference)
                    {
                        GameObject newRef = newReference as GameObject;
                        if (newRef == null)
                        {
                            record.Target = null;
                        }
                        else
                        {
                            record.Target = newRef.GetComponent<ExposeToEditor>();
                        }
                    }
                }

                if(record.OldState == oldReference)
                {
                    record.OldState = newReference;

                    //Handling runtime script reload;
                    if(record.NewState != null && newReference != null)
                    {
                        if(record.NewState is Type)
                        {
                            Type t = (Type)record.NewState;
                            Type refType = newReference.GetType();
                            if(t.FullName == refType.FullName)
                            {
                                record.NewState = refType;
                            }
                        }
                    }
                }

                if ((record.Target as ExposeToEditor) == null)
                {
                    return true;
                }

                return false;
            });

            if(newRecord != null)
            {
                newRecord.Redo();
            }
        }

        public void DestroyComponent(Component destroy, MemberInfo[] memberInfo)
        {
            if (!Enabled)
            {
                return;
            }
            if (Locked)
            {
                return;
            }
            Type componentType = destroy.GetType();
            Record newRecord = CreateRecord(destroy.gameObject, null, destroy,
            record =>
            {
                GameObject go = record.Target as GameObject;
                Component component = record.OldState as Component;
                object replacement = new object();
                if (component)
                {
                    Erase(component, replacement, true);
                }
                record.OldState = replacement;
                record.NewState = GetValues(component, memberInfo);
                UnityObject.Destroy(component);
                return true;
            },
            record =>
            {
                GameObject go = record.Target as GameObject;
                Component component = AddComponent(go, componentType);
                AssingValues(component, memberInfo, (object[])record.NewState);
                
                object repacement = record.OldState;
                Erase(repacement, component, true);
                record.OldState = component;

                return true;
            },
            record =>
            {
                //if (record.OldState != null && !(record.OldState is Component))
                //{
                //    Erase(record.OldState, null);
                //}
            },
            (record, oldReference, newReference) =>
            {
                GameObject target = record.Target as GameObject;
                if (target != null)
                {
                    if (target.gameObject == (object)oldReference)
                    {
                        GameObject newRef = newReference as GameObject;
                        if (newRef == null)
                        {
                            record.Target = null;
                        }
                        else
                        {
                            record.Target = newRef;
                        }
                    }
                }

                if (record.OldState == oldReference)
                {
                    record.OldState = newReference;
                }

                if(record.NewState is object[])
                {
                    object[] values = (object[])record.NewState;
                    for(int i = 0; i < values.Length; ++i)
                    {
                        if(values[i] == oldReference)
                        {
                            values[i] = newReference;
                        }
                    }
                }

                if ((record.Target as GameObject) == null)
                {
                    return true;
                }

                return false;
            });

            if (newRecord != null)
            {
                newRecord.Redo();
            }
        }

        #region Obsolete

        [Obsolete("Use void Select(IRuntimeSelection selection, UnityObject[] objects, UnityObject activeObject) instead")]
        public void Select(UnityObject[] objects, UnityObject activeObject)
        {
            Select(m_rte.Selection, objects, activeObject);
        }

        #endregion
    }

    public class DisabledUndo : IRuntimeUndo
    {
        public bool Enabled { get { return false; } set { } }

        public bool CanUndo { get { return false; } }

        public bool CanRedo { get { return false; } }

        public bool IsRecording { get { return false; } }

        private void GetRidOfWarnings()
        {
            BeforeUndo();
            UndoCompleted();
            BeforeRedo();
            RedoCompleted();
            StateChanged();
        }

        public void BeginRecord()
        {
            
        }

        public void EndRecord()
        {
         
        }

        public void Redo()
        {
         
        }

        public void Undo()
        {
         
        }

        public void Purge()
        {
            
        }

        public void Erase(object oldRef, object newRef, bool ignoreLock)
        {
            
        }

        public void Store()
        {
         
        }

        public void Restore()
        {
         
        }

        public Record CreateRecord(UndoRedoCallback redoCallback, UndoRedoCallback undoCallback, PurgeCallback purgeCallback = null, EraseReferenceCallback eraseCallback = null)
        {
            return CreateRecord(null, null, null, redoCallback, undoCallback, purgeCallback, eraseCallback);
        }

        public Record CreateRecord(object target, object newState, object oldState, UndoRedoCallback redoCallback, UndoRedoCallback undoCallback, PurgeCallback purgeCallback = null, EraseReferenceCallback eraseCallback = null)
        {
            return null;
        }

        public void Select(IRuntimeSelection selection, UnityObject[] objects, UnityObject activeObject)
        {

        }

        [Obsolete]
        public void Select(UnityObject[] objects, UnityObject activeObject)
        {
         
        }

        public void RegisterCreatedObjects(ExposeToEditor[] createdObjects, Action afterRedo = null, Action afterUndo = null)
        {
         
        }

        public void DestroyObjects(ExposeToEditor[] destoryedObjects, Action afterRedo = null, Action afterUndo = null)
        {
            
        }

        public void RecordValue(object target, MemberInfo memberInfo, Action afterRedo, Action afterUndo)
        {

        }

        public void RecordValue(object target, object accessor, MemberInfo memberInfo, Action afterRedo, Action afterUndo)
        {

        }

        public void BeginRecordValue(object target, MemberInfo memberInfo)
        {
           
        }

        public void BeginRecordValue(object target, object accessor, MemberInfo memberInfo)
        {
            
        }

        public void EndRecordValue(object target, MemberInfo memberInfo, Action afterRedo, Action afterUndo)
        {
            
        }

        public void EndRecordValue(object target, object accessor, MemberInfo memberInfo, Action<object, object> targetErased, Action afterRedo, Action afterUndo)
        {
            
        }

        public void BeginRecordTransform(Transform target, Transform parent = null, int siblingIndex = -1)
        {

        }
        public void EndRecordTransform(Transform target, Transform parent = null, int siblingIndex = -1)
        {

        }

        public void AddComponent(ExposeToEditor obj, Type type)
        {

        }

        public void DestroyComponent(Component destroy, MemberInfo[] memberInfo)
        {

        }


        public event RuntimeUndoEventHandler BeforeUndo;
        public event RuntimeUndoEventHandler UndoCompleted;
        public event RuntimeUndoEventHandler BeforeRedo;
        public event RuntimeUndoEventHandler RedoCompleted;
        public event RuntimeUndoEventHandler StateChanged;

  
    }
}
