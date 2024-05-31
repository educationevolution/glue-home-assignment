using Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerClientCommunication
{
    public abstract class BaseServerRequest
    {
        protected abstract string RequestCode();
        public int UserId { get; protected set; }
        public void SetRequestMetadata(int userId)
        {
            UserId = userId;
        }
    }
}