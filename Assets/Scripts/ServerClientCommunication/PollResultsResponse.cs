using Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerClientCommunication
{
    public class PollResultsResponseData
    {
        public List<float> Results01;
    }

    public class PollResultsResponse : BaseServerResponse
    {
        public PollResultsResponseData Data;

        public PollResultsResponse(bool isSuccess) : base(isSuccess)
        {
            var totalOptions = ClientServices.Instance.PollStore.CurrentPollProperties.OptionsData.Count;
            var results01 = new List<float>();
            for (var i = 0; i < totalOptions; i++) 
            {
                results01.Add(Random.Range(0, 1f));
            }
            Data = new PollResultsResponseData()
            {
                Results01 = results01
            };
        }
    }
}