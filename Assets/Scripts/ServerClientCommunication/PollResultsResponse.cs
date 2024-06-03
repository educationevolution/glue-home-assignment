using Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerClientCommunication
{
    public class PollSingleResultData
    {
        public float Result01;
        public List<string> VotersAvatarImageUrls;
    }
    public class PollResultsResponseData
    {
        public List<PollSingleResultData> ResultsData;
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
            var resultsData = new List<PollSingleResultData>();
            var resultsLeft01 = 1f;
            var highestResultIndex = 0;
            var highestResult01 = 0f;
            for (var i = 0; i < totalOptions; i++) 
            {
                var singleResultData = new PollSingleResultData();
                var newResult01 = i == totalOptions - 1?
                    resultsLeft01 : Random.Range(0, resultsLeft01);
                singleResultData.Result01 = newResult01;
                singleResultData.VotersAvatarImageUrls = GetVotersAvatarImageUrls();
                resultsLeft01 -= newResult01;
                resultsData.Add(singleResultData);
                if (highestResult01 > newResult01)
                {
                    highestResult01 = newResult01;
                    highestResultIndex = i;
                }
            }
            var winningOptionData = PollProperties.OptionsData[highestResultIndex];
            Data = new PollResultsResponseData()
            {
                ResultsData = resultsData,
                CallToActionTitle = $"Team {winningOptionData.Title},",
                CallToActionDescription = "Let's see how much you know about her"
            };
        }

        private List<string> GetVotersAvatarImageUrls()
        {
            var votersCount = Random.Range(3, 8);
            var returnList = new List<string>();
            for (var i = 0; i < votersCount; i++) 
            {
                returnList.Add($"Images/userAvatar{Random.Range(1, 5)}");
            }
            return returnList;
        }
    }
}