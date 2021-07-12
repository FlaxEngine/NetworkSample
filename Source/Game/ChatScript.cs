using System;
using FlaxEditor;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEngine.Networking;
using Game;

/// <summary>
/// OnlineScript Script.
/// </summary>
public class ChatScript : Script
{
    public UIControl MessageBox;
    public UIControl Panel;
    public UIControl VertPanel;

    private bool _isWritting;
    private int _chatIndex;
    
    /// <inheritdoc/>
    public override void OnEnable()
    {
        _isWritting = false;
        ((TextBox)MessageBox.Control).Clear();
        ((VerticalPanel) VertPanel.Control).DisposeChildren();
        MessageBox.IsActive = false;
        _chatIndex = 0;
    }

    /// <inheritdoc/>
    public override void OnUpdate()
    {
        if (GameSession.Instance.ChatMessages.Count - 1 >= _chatIndex)
        {
            while (_chatIndex < GameSession.Instance.ChatMessages.Count)
            {
                var l = ((VerticalPanel) VertPanel.Control).AddChild<Label>();
                var player = GameSession.Instance.GetPlayer(GameSession.Instance.ChatMessages[_chatIndex].Sender);
                string name = String.Empty;
                if (player != null)
                {
                    name = player.Name;
                }
                l.Text = name + " : " + GameSession.Instance.ChatMessages[_chatIndex].Message;
                l.HorizontalAlignment = TextAlignment.Near;
                _chatIndex++;
            }
        }
        
        if (!_isWritting && Input.GetKeyUp(KeyboardKeys.Return))
        {
            _isWritting = true;
            MessageBox.IsActive = true;
            MessageBox.Control.Focus();
            ((TextBox)MessageBox.Control).Clear();
        }
        else if (_isWritting && Input.GetKeyUp(KeyboardKeys.Return))
        {
            _isWritting = false;
            MessageBox.IsActive = false;
            if (((TextBox) MessageBox.Control).Text != string.Empty)
            {
                GameSession.Instance.AddChatMessage(GameSession.Instance.LocalPlayer.ID, ((TextBox)MessageBox.Control).Text);
                ChatMessagePacket p = new ChatMessagePacket();
                p.Message = ((TextBox) MessageBox.Control).Text;
                p.Sender = GameSession.Instance.LocalPlayer.ID;
                NetworkSession.Instance.Send(p, NetworkChannelType.Reliable);
                ((TextBox)MessageBox.Control).Clear();
            }
#if FLAX_EDITOR
            Editor.Instance.Windows.GameWin.Focus();
#endif
        }
        ((Panel)Panel.Control).ScrollViewTo(VertPanel.Control.Bounds.BottomLeft, true);
    }
}