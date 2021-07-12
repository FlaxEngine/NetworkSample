using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEngine.Json;

namespace Game
{
    /// <summary>
    /// HostConnectScreenScript Script.
    /// </summary>
    public class HostConnectScreenScript : Script
    {
        public UIControl UsernameTextBox;
        public UIControl AddressTextBox;
        public UIControl ConnectButton;
        public UIControl PortTextBox;
        public UIControl HostButton;

        private NetworkSession _netplug;
        
        /// <inheritdoc/>
        public override void OnStart()
        {

        }

        private void OnConnect()
        {
            _netplug.Connect(((TextBox)UsernameTextBox.Control).Text, ((TextBox)AddressTextBox.Control).Text, ushort.Parse(((TextBox)PortTextBox.Control).Text));
        }

        private void OnHost()
        {
            _netplug.Host(((TextBox)UsernameTextBox.Control).Text,ushort.Parse(((TextBox)PortTextBox.Control).Text));
            JsonSerializer.ParseID("74a68a984824b4510d12589f199ad68f", out var guid);
            Level.ChangeSceneAsync(guid);
        }
        
        /// <inheritdoc/>
        public override void OnEnable()
        {
            _netplug = PluginManager.GetPlugin<NetworkSession>();
            ((Button) ConnectButton.Control).Clicked += OnConnect;
            ((Button) HostButton.Control).Clicked += OnHost;
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            ((Button) ConnectButton.Control).Clicked -= OnConnect;
            ((Button) HostButton.Control).Clicked -= OnHost;
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {

        }
    }
}
