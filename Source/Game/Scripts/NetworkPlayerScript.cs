using FlaxEngine;
using FlaxEngine.GUI;

namespace Game
{
    public class NetworkPlayerScript : Script
    {
        public Player Player;

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            // Sync actor transform
            var trans = Actor.Transform;
            trans.Translation = Vector3.Lerp(trans.Translation, Player.Position, 0.4f);
            trans.Orientation = Quaternion.Lerp(trans.Orientation, Player.Rotation, 0.4f);
            Actor.Transform = trans;

            // Sync actor name
            var label = Actor.FindActor<UIControl>();
            label.Get<Label>().Text = Player.Name;
            Actor.Name = "Player_" + Player.Name;
        }
    }
}
