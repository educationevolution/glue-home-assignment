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

    public class FakeServerLink : MonoBehaviour
    {
        public static FakeServerLink Instance { get; private set; }
        public Action<ServerChatMessageData> OnChatMessageDataReceived;
        public Action OnPollResultsDataReceived;
        private Dictionary<int, Coroutine> _fakeRequestsById;
        private int _nextRequestId;

        private List<string> _fakeChatMessage = new List<string>()
        {
            "Which Beyoncé album do you think showcases her talent the most?",
            "What's your favorite Taylor Swift song and why does it resonate with you?",
            "Mariana's voice is so powerful! What's your favorite performance of hers?",
            "Do you think Beyoncé's influence goes beyond music?",
            "Taylor Swift's songwriting is so relatable. Which lyrics speak to you the most?",
            "Have you seen Mariana live in concert? Share your favorite memory!",
            "Beyoncé's performances are legendary! Which one stands out the most to you?",
            "Taylor Swift's evolution as an artist is fascinating. How do you feel about her changing styles?",
            "Mariana's music has a way of touching the soul. Which song resonates with you deeply?",
            "Beyoncé, Taylor Swift, and Mariana each have their unique style. Which one do you relate to the most?"
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
            var fakeServerResponseDelay = 0.2f;
            yield return new WaitForSeconds(fakeServerResponseDelay);

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
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 3f));
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