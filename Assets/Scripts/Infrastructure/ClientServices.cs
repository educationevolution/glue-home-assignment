using Polls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure
{
    public class ClientServices : MonoBehaviour
    {
        public static ClientServices Instance { get; private set; }
        public PollStore PollStore { get; private set; }
        public ImageStore ImageStore { get; private set; }
        public int FakeUserId => 1;
        public string FakeUserAvatarImageUrl => $"Images/mainUserAvatar";

        private void Awake()
        {
            if (Instance != null) 
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PollStore = new();
            ImageStore = new();
        }
    }
}