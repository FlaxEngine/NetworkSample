using FlaxEngine;
using FlaxEngine.Networking;

namespace Game
{
    public class DemoSceneScript : Script
    {
        private GameSession _game;
        public Prefab PlayerPrefab;
        private float _lastTransformSent;

        /// <inheritdoc/>
        public override void OnEnable()
        {
            _game = GameSession.Instance;
            _game.OnPlayerAdded += OnPlayerAdded;
            _game.OnPlayerRemoved += OnPlayerRemoved;
            for (var i = 0; i < _game.Players.Count; i++)
            {
                OnPlayerAdded(_game.Players[i]);
            }

            _lastTransformSent = 0;
        }

        public void OnPlayerAdded(Player player)
        {
            player.Actor = PrefabManager.SpawnPrefab(PlayerPrefab, Actor);
            var script = player.Actor.GetScript<NetworkPlayerScript>();
            script.Player = player;
            player.Actor.Name = "Player_" + player.Name;
        }

        public void OnPlayerRemoved(Player player)
        {
            Destroy(player.Actor);
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            _game.OnPlayerAdded -= OnPlayerAdded;
            _game.OnPlayerRemoved -= OnPlayerRemoved;
            for (var i = 0; i < _game.Players.Count; i++)
            {
                _game.Players[i].Actor = null;
            }

            NetworkSession.Instance.Disconnect();
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            if (NetworkSession.Instance.IsServer && Time.UnscaledGameTime - _lastTransformSent > 0.05f)
            {
                var te = new PlayersTransformPacket.TransformEntry();
                var ptp = new PlayersTransformPacket();
                for (var i = 0; i < GameSession.Instance.Players.Count; i++)
                {
                    te.Guid = GameSession.Instance.Players[i].ID;
                    te.Position = GameSession.Instance.Players[i].Position;
                    te.Rotation = GameSession.Instance.Players[i].Rotation;
                    ptp.Transforms.Add(te);
                }

                te.Guid = GameSession.Instance.LocalPlayer.ID;
                te.Position = GameSession.Instance.LocalPlayer.Position;
                te.Rotation = GameSession.Instance.LocalPlayer.Rotation;
                ptp.Transforms.Add(te);
                NetworkSession.Instance.SendAll(ptp, NetworkChannelType.UnreliableOrdered);
                _lastTransformSent = Time.UnscaledGameTime;
            }
        }
    }
}
