using FlaxEngine;
using FlaxEngine.GUI;

namespace Game
{
    public class HostConnectScreenScript : Script
    {
        public UIControl UsernameTextBox;
        public UIControl AddressTextBox;
        public UIControl ConnectButton;
        public UIControl PortTextBox;
        public UIControl HostButton;

        public SceneReference GameScene;

        private void OnConnect()
        {
            NetworkSession.Instance.Connect(((TextBox)UsernameTextBox.Control).Text, ((TextBox)AddressTextBox.Control).Text, ushort.Parse(((TextBox)PortTextBox.Control).Text));
        }

        private void OnHost()
        {
            if (!NetworkSession.Instance.Host(((TextBox)UsernameTextBox.Control).Text,
                ushort.Parse(((TextBox)PortTextBox.Control).Text)))
            {
                Level.ChangeSceneAsync(GameScene);
            }
        }

        /// <inheritdoc/>
        public override void OnEnable()
        {
            ((Button)ConnectButton.Control).Clicked += OnConnect;
            ((Button)HostButton.Control).Clicked += OnHost;
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            ((Button)ConnectButton.Control).Clicked -= OnConnect;
            ((Button)HostButton.Control).Clicked -= OnHost;
        }
    }
}
