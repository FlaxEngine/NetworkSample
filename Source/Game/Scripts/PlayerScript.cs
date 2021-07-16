using System;
using System.Collections.Generic;
using System.ComponentModel;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEngine.Networking;

namespace Game
{
    /// <summary>
    /// NetworkPlayerScript Script.
    /// </summary>
    public class PlayerScript : Script
    {
        public Player Player;

        /// <inheritdoc/>
        public override void OnStart()
        {
            // Here you can add code that needs to be called when script is created, just before the first game update
        }
        
        /// <inheritdoc/>
        public override void OnEnable()
        {
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            // Here you can add code that needs to be called when script is disabled (eg. unregister from events)
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            var trans = Actor.Transform;
            trans.Translation = Vector3.Lerp(trans.Translation, Player.Position, 0.4f);
            trans.Orientation = Quaternion.Lerp(trans.Orientation, Player.Rotation, 0.4f);
            Actor.Transform = trans; 

            SetNameLabel();
            Actor.Name = "Player_" + Player.Name;
        }

        public void SetNameLabel()
        {
            Actor albl = Actor.FindActor(typeof(UIControl));
            Label lbl = (Label)((UIControl) albl).Control;
            lbl.Text = Player.Name;
        }
    }
}
