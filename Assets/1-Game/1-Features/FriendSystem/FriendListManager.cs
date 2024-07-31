using System;
using Game.Utils;
using Steamworks;
using UnityEngine;

namespace Game.FriendSystem
{
    public class FriendListManager : MonoBehaviour
    {
        [SerializeField] private GameObject _friendListItem;
        [SerializeField] private Transform _friendListContainer;
        private void Start()
        {
            ReloadFriendsListAsync();
        }

        public async void ReloadFriendsListAsync()
        { 
            foreach ( var friend in SteamFriends.GetFriends())
            {
                GameObject friendItem = Instantiate(_friendListItem, _friendListContainer);
                var image = await SteamFriends.GetLargeAvatarAsync(friend.Id);
                friendItem.GetComponent<FriendListItem>().Setup(friend, SteamExtension.GetTextureFromImage(image.Value));
            }
        }
    }
}