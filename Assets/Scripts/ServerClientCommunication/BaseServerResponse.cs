using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerClientCommunication
{
    public abstract class BaseServerResponse
    {
        public bool IsSuccess { get; protected set; }

        public BaseServerResponse(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
    }
}