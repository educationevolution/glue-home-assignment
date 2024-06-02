using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerClientCommunication
{
    public class ExitPollRequestData
    {

    }

    public class ExitPollRequest : BaseServerRequest
    {
        protected override string RequestCode() => "exit_poll";
    }
}