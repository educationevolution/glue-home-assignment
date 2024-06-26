using Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ServerClientCommunication
{
    public class ServerChatMessageData
    {
        public int UserId;
        public string Text;
        public string AvatarImageUrl;
    }

    /// <summary>
    /// This component simuulates the communication between the client and the server.
    /// The client can send pre defined requests to the server, await for the server to send a response 
    /// and handle the response accordingly.
    /// The component also simulates server to client alerts for:
    /// - Ending a poll
    /// - Chat messages during a poll.
    /// </summary>
    public class FakeServerLink : MonoBehaviour
    {
        private const float FAKE_SERVER_RESPONSE_DELAY = 0.3f;
        private const float CHAT_MESSAGE_MIN_DELAY = 0.3f;
        private const float CHAT_MESSAGE_MAX_DELAY_ADDON = 1.5f;
        public static FakeServerLink Instance { get; private set; }
        public event Action<ServerChatMessageData> OnChatMessageDataReceived;
        public event Action OnPollResultsDataReceived;
        private Dictionary<int, Coroutine> _fakeRequestsById;
        private int _nextRequestId;

        private List<string> _fakeChatMessage = new List<string>()
        {
            "Which Beyonc� album do you think showcases her talent the most?",
            "What's your favorite Taylor Swift song and why does it resonate with you?",
            "Mariana's voice is so powerful! What's your favorite performance of hers?",
            "Do you think Beyonc�'s influence goes beyond music?",
            "Taylor Swift's songwriting is so relatable. Which lyrics speak to you the most?",
            "Have you seen Mariana live in concert? Share your favorite memory!",
            "Beyonc�'s performances are legendary! Which one stands out the most to you?",
            "Taylor Swift's evolution as an artist is fascinating. How do you feel about her changing styles?",
            "Mariana's music has a way of touching the soul. Which song resonates with you deeply?",
            "Beyonc�, Taylor Swift, and Mariana each have their unique style. Which one do you relate to the most?"
        };

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;
            _fakeRequestsById = new();
            StartCoroutine(FakeChatMessagesCoroutine());
        }

        /// <summary>
        /// Send a request to the server.
        /// </summary>
        /// <param name="request">Uniqe client to server request</param>
        /// <param name="responseHandler">Optional response handler for the server response</param>
        public void SendRequestToServer(BaseServerRequest request, Action<BaseServerResponse> responseHandler)
        {
            if (responseHandler != null)
            {
                GenericCanvas.Instance.ShowGenericLoadingMessage();
            }
            request.SetRequestMetadata(ClientServices.Instance.FakeUserId);
            var requestId = _nextRequestId;
            _nextRequestId += 1;
            var newFakeRequestCoroutine = StartCoroutine(FakeRequestCoroutine(requestId, request, responseHandler));
            _fakeRequestsById[requestId] = newFakeRequestCoroutine;
        }

        private IEnumerator FakeRequestCoroutine(int id, BaseServerRequest request, Action<BaseServerResponse> responseHandler)
        {
            yield return new WaitForSeconds(FAKE_SERVER_RESPONSE_DELAY);

            BaseServerResponse reponse = new EnterPollResponse(isSuccess: true);

            _fakeRequestsById.Remove(id);

            if (request is SendChatMessageRequest)
            {
                var chatMessageRequest = request as SendChatMessageRequest;
                CreateChatMessage(new ServerChatMessageData()
                {
                    UserId = chatMessageRequest.UserId,
                    Text = chatMessageRequest.Data.Message,
                    // User's avatar should be sent from the server to the clients
                    AvatarImageUrl = ClientServices.Instance.FakeUserAvatarImageUrl
                });
            }
            else if (request is EnterPollRequest)
            {
                var enterPollReponse = new EnterPollResponse(isSuccess: true);
                ClientServices.Instance.PollStore.SetCurrentPollProperties(enterPollReponse.Data);
                responseHandler?.Invoke(reponse);
                StartCoroutine(FakePollResultsResponseCoroutine(enterPollReponse.Data.SecondsLeft));
            }
            else if (request is ExitPollRequest)
            {
                var exitPollResponse = new ExitPollResponse(isSuccess: true);
                responseHandler?.Invoke(reponse);
            }

            GenericCanvas.Instance.HideGenericLoadingMessage();
        }

        private IEnumerator FakePollResultsResponseCoroutine(float secondsLeftToEndPoll)
        {
            yield return new WaitForSeconds(secondsLeftToEndPoll);
            var pollResultsReponse = new PollResultsResponse(isSuccess: true);
            ClientServices.Instance.PollStore.SetCurrentPollResults(pollResultsReponse.Data);
            OnPollResultsDataReceived?.Invoke();
        }
        
        private IEnumerator FakeChatMessagesCoroutine()
        {
            while (true)
            {
                var delayToNextMessage = UnityEngine.Random.Range(CHAT_MESSAGE_MIN_DELAY, CHAT_MESSAGE_MAX_DELAY_ADDON);
                yield return new WaitForSeconds(delayToNextMessage);
                var randomUserId = ClientServices.Instance.FakeUserId + UnityEngine.Random.Range(1, 100);
                CreateChatMessage(new ServerChatMessageData()
                {
                    UserId = randomUserId,
                    Text = GetFakeChatText(),
                    AvatarImageUrl = $"Images/userAvatar{UnityEngine.Random.Range(1, 5)}"
                });
            }
        }

        private void CreateChatMessage(ServerChatMessageData chatMessageData)
        {
            OnChatMessageDataReceived?.Invoke(chatMessageData);
        }

        private string GetFakeChatText()
        {
            var randomIndex = UnityEngine.Random.Range(0, _fakeChatMessage.Count);
            return _fakeChatMessage[randomIndex];
        }
    }
}