using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerClientCommunication
{
    public class SendChatMessageRequestData
    {
        public string Message;
    }

    public class SendChatMessageRequest : BaseServerRequest
    {
        public SendChatMessageRequestData Data { get; private set; }
        protected override string RequestCode() => "send_chat_message";

        public SendChatMessageRequest(SendChatMessageRequestData data)
        {
            Data = data;
        }
    }
}