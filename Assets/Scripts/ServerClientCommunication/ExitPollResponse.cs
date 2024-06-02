using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerClientCommunication
{
    public class ExitPollResponse : BaseServerResponse
    {
        public EnterPollResponseData Data;

        public ExitPollResponse(bool isSuccess) : base(isSuccess)
        {
            
        }
    }
}