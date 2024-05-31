using ServerClientCommunication;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polls
{
    public class PollStore
    {
        public EnterPollResponseData CurrentPollServerData { get; private set; }

        public void SetCurrentPollServerData(EnterPollResponseData data)
        {
            CurrentPollServerData = data;
        }
    }
}