using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.ServerClientCommunication
{
    public abstract class BaseServerRequest
    {
        protected abstract string RequestCode();
    }
}