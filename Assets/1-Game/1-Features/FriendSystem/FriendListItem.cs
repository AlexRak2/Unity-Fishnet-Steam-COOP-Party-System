using Game.PopupSystem;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.FriendSystem
{
    public class FriendListItem : MonoBehaviour
    {
        private Friend friend;
        [SerializeField] private RawImage _pfpImage;
        [SerializeField] private TMP_Text _usernameText;

        public void Setup(Friend friend,Texture2D texture)
        {
            this.friend = friend;
            _pfpImage.texture = texture;
            _usernameText.text = friend.Name;
        }

        public void InviteFriend()
        {
            friend.InviteToGame("Join Me");
            PopupManager.Popup_Show(new PopupContent("FRIEND INVITED", "", true));
        }
    }
}