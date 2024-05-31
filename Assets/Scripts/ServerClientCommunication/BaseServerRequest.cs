using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerClientCommunication
{
    public abstract class BaseServerRequest
    {
        protected abstract string RequestCode();
    }
}