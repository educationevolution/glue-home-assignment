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
    }

    public class EnterPollResponse : BaseServerResponse
    {
        public EnterPollResponseData Data;

        public EnterPollResponse(bool isSuccess) : base(isSuccess)
        {
            Data = new EnterPollResponseData()
            {
                Id = Random.Range(0, 9999),
                Question = "WHO IS THE<BR>BEST SINGER?",
                OptionsData = new List<PollOptionData>()
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
                    new PollOptionData()
                    {
                        Title = "Taylor",
                        ImageUrl = "Images/Taylor",
                    }
                }
            };
        }
    }
}