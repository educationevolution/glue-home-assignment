using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.ServerClientCommunication
{
    public class EnterPollRequestData
    {

    }

    public class EnterPollRequest : BaseServerRequest
    {
        protected override string RequestCode() => "enter_poll";
    }
}