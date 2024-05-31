using Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ServerClientCommunication
{
    public class FakeServerLink : MonoBehaviour
    {
        public static FakeServerLink Instance { get; private set; }
        private Dictionary<int, Coroutine> _fakeRequestsById;
        private int _nextRequestId;

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
        }

        public void RequestToEnterPoll(BaseServerRequest request, Action<BaseServerResponse> responseHandler)
        {
            GenericCanvas.Instance.ShowGenericLoadingMessage();
            var requestId = _nextRequestId;
            _nextRequestId += 1;
            var newFakeRequestCoroutine = StartCoroutine(FakeRequestCoroutine(requestId, request, responseHandler));
            _fakeRequestsById[requestId] = newFakeRequestCoroutine;
        }

        private IEnumerator FakeRequestCoroutine(int id, BaseServerRequest request, Action<BaseServerResponse> responseHandler)
        {
            yield return new WaitForSeconds(0.4f);
            // decide response based on request
            var reponse = new EnterPollResponse(isSuccess: true);
            _fakeRequestsById.Remove(id);
            GenericCanvas.Instance.HideGenericLoadingMessage();
            responseHandler?.Invoke(reponse);
        }
    }
}