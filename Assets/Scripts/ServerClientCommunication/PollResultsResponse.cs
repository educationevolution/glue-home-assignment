using Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerClientCommunication
{
    public class PollResultsResponseData
    {
        public List<float> Results01;
        public string CallToActionTitle;
        public string CallToActionDescription;
    }

    public class PollResultsResponse : BaseServerResponse
    {
        public PollResultsResponseData Data;
        private EnterPollResponseData PollProperties => ClientServices.Instance.PollStore.CurrentPollProperties;

        public PollResultsResponse(bool isSuccess) : base(isSuccess)
        {
            var totalOptions = PollProperties.OptionsData.Count;
            var results01 = new List<float>();
            var highestResultIndex = 0;
            var highestResult01 = 0f;
            for (var i = 0; i < totalOptions; i++) 
            {
                var newResult01 = Random.Range(0, 1f);
                results01.Add(newResult01);
                if (highestResult01 > newResult01)
                {
                    highestResult01 = newResult01;
                    highestResultIndex = i;
                }
            }
            var winningOptionData = PollProperties.OptionsData[highestResultIndex];
            Data = new PollResultsResponseData()
            {
                Results01 = results01,
                CallToActionTitle = $"Team {winningOptionData.Title},",
                CallToActionDescription = "Let's see how much you know about her"
            };
        }
    }
}