using Game.PlayerSystem;
using Game.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class ReadyUpButton : BaseMonoBehaviour
    {
        [SerializeField] private Image _readyImage;
        [SerializeField] private Color _readyColor;
        [SerializeField] private Color _notReadyColor;
        [SerializeField] private TMP_Text _readyText;

        protected override void RegisterEvents()
        {
            MyClient.OnIsReady += ToggleReadyButton;
        }
        protected override void UnregisterEvents()
        {
            MyClient.OnIsReady -= ToggleReadyButton;
        }
        
        private void ToggleReadyButton(bool value)
        {
            _readyImage.color = value ? _readyColor : _notReadyColor;
            _readyText.text = value ? "Ready" : "Not Ready";
        }

    }
}