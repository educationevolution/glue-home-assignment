using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infrastructure
{
    public struct UserStickerData
    {
        public string ImageUrl;
    }

    public class StickersStore
    {
        private Dictionary<int, UserStickerData> _userStickersDataById = new()
        {
            { 
                0, new UserStickerData()
                {
                    ImageUrl = "Stickers/Sticker1"
                } },
            { 
                1, new UserStickerData()
                {
                    ImageUrl = "Stickers/Sticker2"
                } },
            { 
                2, new UserStickerData()
                {
                    ImageUrl = "Stickers/Sticker3"
                } }};

        public Dictionary<int, UserStickerData> GetAllUserStickersData => new Dictionary<int, UserStickerData>(_userStickersDataById);

        public UserStickerData GetUserStickerData(int id)
        {
            if (_userStickersDataById.ContainsKey(id) == false)
            {
                throw new Exception($"No {nameof(UserStickerData)} found for id {id}!");
            }
            return _userStickersDataById[id];
        }
    }
}