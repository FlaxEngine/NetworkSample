using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;

namespace Game
{
    /// <summary>
    /// PlayerListScript Script.
    /// </summary>
    public class PlayerListScript : Script
    {
        public UIControl VerticalPanel;
        private VerticalPanel _vpanel;
        
        /// <inheritdoc/>
        public override void OnStart()
        {
            // Here you can add code that needs to be called when script is created, just before the first game update
        }
        
        /// <inheritdoc/>
        public override void OnEnable()
        {
            _vpanel = (VerticalPanel) VerticalPanel.Control;
            RebuildList();
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyboardKeys.Tab))
            {
                ((UIControl) Actor).Control.Visible = true;
            }
            else if (Input.GetKeyUp(KeyboardKeys.Tab))
            {
                ((UIControl) Actor).Control.Visible = false;
            }
            
            if (((UIControl) Actor).Control.Visible)
                RebuildList();
        }

        public void RebuildList()
        {
            _vpanel.DisposeChildren();
            for (var i = 0; i < GameSession.Instance.Players.Count; i++)
            {
                var p = GameSession.Instance.Players[i];
                var lbl = _vpanel.AddChild<Label>();
                lbl.Text = p.Name;
                lbl.Font.Size = 12;
                lbl.AnchorPreset = AnchorPresets.TopCenter;
            }
            
            Player lp = GameSession.Instance.LocalPlayer;
            var llbl = _vpanel.AddChild<Label>();
            llbl.Text = lp.Name;
            llbl.Font.Size = 12;
            llbl.AnchorPreset = AnchorPresets.TopCenter;
            llbl.TextColor = Color.Green;
        }
    }
}
