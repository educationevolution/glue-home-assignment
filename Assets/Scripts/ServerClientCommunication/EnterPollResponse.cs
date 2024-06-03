using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerClientCommunication
{
    public class PollOptionData
    {
        public string Title;
        public string ImageUrl;
    }

    public class EnterPollResponseData
    {
        public int Id;
        public string Question;
        public List<PollOptionData> OptionsData;
        public float SecondsLeft;
    }

    public class EnterPollResponse : BaseServerResponse
    {
        public EnterPollResponseData Data;
        private static int _lastOptionsCount = 3;

        public EnterPollResponse(bool isSuccess) : base(isSuccess)
        {
            // Simulating between showing a poll with 2 and 3 options.
            _lastOptionsCount = _lastOptionsCount == 2 ?
                3 : 2;

            var optionsData = new List<PollOptionData>()
                {
                    new PollOptionData()
                    {
                        Title = "Beyonce",
                        ImageUrl = "Images/Beyoncé",
                    },
                    new PollOptionData()
                    {
                        Title = "Ariana",
                        ImageUrl = "Images/Ariana",
                    },
                    
                };

            if (_lastOptionsCount == 3)
            {
                optionsData.Add(new PollOptionData()
                {
                    Title = "Taylor",
                    ImageUrl = "Images/Taylor",
                });
            }

            Data = new EnterPollResponseData()
            {
                Id = Random.Range(0, 9999),
                Question = "WHO IS THE<BR>BEST SINGER?",
                SecondsLeft = 30f,
                OptionsData = optionsData
            };
        }
    }
}