using ServerClientCommunication;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polls
{
    public class PollStore
    {
        public EnterPollResponseData CurrentPollProperties { get; private set; }
        public PollResultsResponseData CurrentPollResults { get; private set; }

        public void SetCurrentPollProperties(EnterPollResponseData data)
        {
            CurrentPollProperties = data;
        }

        public void SetCurrentPollResults(PollResultsResponseData data)
        {
            CurrentPollResults = data;
        }
    }
}