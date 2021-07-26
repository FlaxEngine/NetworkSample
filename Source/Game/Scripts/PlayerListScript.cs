using FlaxEngine;
using FlaxEngine.GUI;

namespace Game
{
    public class PlayerListScript : Script
    {
        public UIControl VerticalPanel;
        private VerticalPanel _vpanel;

        public FontReference ListFont;

        /// <inheritdoc/>
        public override void OnEnable()
        {
            _vpanel = (VerticalPanel)VerticalPanel.Control;
            RebuildList();
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            var show = Input.GetKey(KeyboardKeys.Tab);
            ((UIControl)Actor).Control.Visible = show;
            if (show)
                RebuildList();
        }

        public void RebuildList()
        {
            _vpanel.DisposeChildren();

            // Network players
            for (var i = 0; i < GameSession.Instance.Players.Count; i++)
            {
                var p = GameSession.Instance.Players[i];
                var l = _vpanel.AddChild<Label>();
                l.Font = ListFont;
                l.Text = p.Name;
                l.Font.Size = 12;
                l.AnchorPreset = AnchorPresets.TopCenter;
            }

            // Local player
            {
                var p = GameSession.Instance.LocalPlayer;
                var l = _vpanel.AddChild<Label>();
                l.Font = ListFont;
                l.Text = p.Name;
                l.Font.Size = 12;
                l.AnchorPreset = AnchorPresets.TopCenter;
                l.TextColor = Color.Green;
            }
        }
    }
}
