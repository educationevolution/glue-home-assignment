using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.ServerClientCommunication
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
    }

    public class EnterPollResponse : BaseServerResponse
    {
        public EnterPollResponseData Data;

        public EnterPollResponse(bool isSuccess) : base(isSuccess)
        {
            Data = new EnterPollResponseData()
            {
                Id = Random.Range(0, 9999),
                Question = "Who is the best singer?",
                OptionsData = new List<PollOptionData>()
                {
                    new PollOptionData()
                    {
                        Title = "BEYONCE",
                        ImageUrl = "",
                    },
                    new PollOptionData()
                    {
                        Title = "ARIANA",
                        ImageUrl = "",
                    },
                    new PollOptionData()
                    {
                        Title = "TAYLOR",
                        ImageUrl = "",
                    }
                }
            };
        }
    }
}