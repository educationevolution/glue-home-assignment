using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerClientCommunication
{
    public class EnterPollRequestData
    {

    }

    public class EnterPollRequest : BaseServerRequest
    {
        protected override string RequestCode() => "enter_poll";
    }
}