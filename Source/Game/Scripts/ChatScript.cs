using FlaxEditor;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEngine.Networking;

public class ChatScript : Script
{
    public UIControl MessageBox;
    public UIControl Panel;
    public UIControl VertPanel;

    public FontReference ChatFont;

    private bool _isWriting;
    private int _chatIndex;

    public bool IsWriting
    {
        get => _isWriting;
    }

    /// <inheritdoc/>
    public override void OnEnable()
    {
        _isWriting = false;
        ((TextBox)MessageBox.Control).Clear();
        ((VerticalPanel)VertPanel.Control).DisposeChildren();
        MessageBox.IsActive = false;
        _chatIndex = 0;
    }

    public override void OnDisable()
    {
        ((VerticalPanel)VertPanel.Control).DisposeChildren();
    }

    /// <inheritdoc/>
    public override void OnUpdate()
    {
        if (GameSession.Instance.ChatMessages.Count - 1 >= _chatIndex)
        {
            ((VerticalPanel)VertPanel.Control).BackgroundColor = new Color(0, 0, 0, 0.28f);
            while (_chatIndex < GameSession.Instance.ChatMessages.Count)
            {
                var l = ((VerticalPanel)VertPanel.Control).AddChild<Label>();
                var player = GameSession.Instance.GetPlayer(GameSession.Instance.ChatMessages[_chatIndex].Sender);
                var name = string.Empty;
                if (player != null)
                {
                    name = player.Name;
                }

                l.Font = ChatFont;
                l.Text = name + " : " + GameSession.Instance.ChatMessages[_chatIndex].Message;
                l.HorizontalAlignment = TextAlignment.Near;
                _chatIndex++;
            }
        }

        if (!_isWriting && Input.GetKeyUp(KeyboardKeys.Return))
        {
            _isWriting = true;
            MessageBox.IsActive = true;
            MessageBox.Control.Focus();
            ((TextBox)MessageBox.Control).Clear();
        }
        else if (_isWriting && Input.GetKeyUp(KeyboardKeys.Return))
        {
            _isWriting = false;
            MessageBox.IsActive = false;
            if (((TextBox)MessageBox.Control).Text != string.Empty)
            {
                GameSession.Instance.AddChatMessage(GameSession.Instance.LocalPlayer.ID, ((TextBox)MessageBox.Control).Text);
                ChatMessagePacket p = new ChatMessagePacket();
                p.Message = ((TextBox)MessageBox.Control).Text;
                p.SenderID = GameSession.Instance.LocalPlayer.ID;
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
