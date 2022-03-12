using System;

namespace Battlehub.RTCommon
{
    public interface IRTEState
    {
        bool IsCreated
        {
            get;
        }

        event Action<object> Created;
        event Action<object> Destroyed;
    }
}


