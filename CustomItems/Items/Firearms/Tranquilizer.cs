namespace CustomItems.Items.Firearms
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using CustomPlayerEffects;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.API.Features.Roles;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.EventArgs.Player;
    using InventorySystem.Items.Firearms.Attachments;
    using MEC;
    using Mirror;
    using PlayerRoles;
    using UnityEngine;
    using YamlDotNet.Serialization;
    using Handlers = Exiled.Events.Handlers;
    [CustomItem(ItemType.GunCOM15)]
    public class Tranquilizer : CustomWeapon
    {
        [YamlIgnore]
        private List<Player> _reloading = new List<Player>();
        [YamlIgnore]
        private List<Player> _effected = new List<Player>();
        [YamlIgnore]
        private Dictionary<Player, float> _immune = new Dictionary<Player, float>();
        //private List<CoroutineHandle> coroutines = new List<CoroutineHandle>();
        public override uint Id { get; set; } = 2104;
        public override string Name { get; set; } = "<color=#00ff00ff>INVIL</color> Tactical Tranquilizer";
        public override string Description { get; set; }
        public override float Damage { get; set; } = 0.5f;
        public override float Weight { get; set; }
        public override byte ClipSize { get; set; } = 10;
        public override AttachmentName[] Attachments { get; set; } = new AttachmentName[3]
        {
            AttachmentName.SoundSuppressor,
            AttachmentName.DotSight,
            AttachmentName.Laser
        };
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            RoomSpawnPoints =
            {
                new RoomSpawnPoint
                {
                    Chance = 50,
                    Room = Exiled.API.Enums.RoomType.Lcz173
                },
                new RoomSpawnPoint
                {
                    Chance = 50,
                    Room = Exiled.API.Enums.RoomType.LczGlassBox
                }
            },
            RoleSpawnPoints =
            {
                new RoleSpawnPoint
                {
                    Chance = 15,
                    Role = PlayerRoles.RoleTypeId.NtfSergeant,
                },
                new RoleSpawnPoint
                {
                    Chance = 25,
                    Role = PlayerRoles.RoleTypeId.NtfCaptain
                }
            }
        };

        public Dictionary<RoleTypeId, float> Immunity = new Dictionary<RoleTypeId, float>
        {
            [RoleTypeId.NtfPrivate] = 2,
            [RoleTypeId.NtfSergeant] = 2,
            [RoleTypeId.NtfCaptain] = 5,
            [RoleTypeId.NtfSpecialist] = 5,
            [RoleTypeId.ChaosRifleman] = 2,
            [RoleTypeId.ChaosMarauder] = 2,
            [RoleTypeId.ChaosRepressor] = 5,
            [RoleTypeId.ChaosConscript] = 5,

            [RoleTypeId.Scp173] = 100,
            [RoleTypeId.Scp106] = 100,
            [RoleTypeId.Scp096] = 15,
            [RoleTypeId.Scp049] = 20,
            [RoleTypeId.Scp0492] = 10,
        };

        [Description("The duration for how long will tranquilizer effects will last")]
        public float Duration = 5f;

        [Description("Modifies the player resistence every time they get tranquilized")]
        public float ResistanceModifier = 1.5f;

        [Description("The maximum amount of resistence they can get")]
        public float MaximumResistance = 100f;

        [Description("Should player drop items when they get tranquilized")]
        public bool DropItems = false;

        [Description("The default resistance for roles that don't have immunities")]
        public float DefaultResistance = 1f;

        protected override void SubscribeEvents()
        {
            Handlers.Player.PickingUpItem += OnDeniableEvent;
            Handlers.Player.ChangingItem += OnDeniableEvent;
            Handlers.Scp049.StartingRecall += OnDeniableEvent;
            Handlers.Scp106.Teleporting += OnDeniableEvent;
            Handlers.Scp173.BlinkingRequest += OnDeniableEvent;
            Handlers.Scp173.PlacingTantrum += OnDeniableEvent;
            Handlers.Scp096.Charging += OnDeniableEvent;
            Handlers.Scp096.Enraging += OnDeniableEvent;
            Handlers.Scp096.AddingTarget += OnDeniableEvent;
            Handlers.Scp939.PlacingAmnesticCloud += OnDeniableEvent;
            Handlers.Player.VoiceChatting += OnDeniableEvent;

            Handlers.Player.Hurting += OnSedatedHurting;
            Handlers.Player.Spawned += OnImmunityResetEvent;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Handlers.Player.PickingUpItem -= OnDeniableEvent;
            Handlers.Player.ChangingItem -= OnDeniableEvent;
            Handlers.Scp049.StartingRecall -= OnDeniableEvent;
            Handlers.Scp106.Teleporting -= OnDeniableEvent;
            Handlers.Scp173.BlinkingRequest -= OnDeniableEvent;
            Handlers.Scp173.PlacingTantrum -= OnDeniableEvent;
            Handlers.Scp096.Charging -= OnDeniableEvent;
            Handlers.Scp096.Enraging -= OnDeniableEvent;
            Handlers.Scp096.AddingTarget -= OnDeniableEvent;
            Handlers.Scp939.PlacingAmnesticCloud -= OnDeniableEvent;
            Handlers.Player.VoiceChatting -= OnDeniableEvent;

            Handlers.Player.Hurting -= OnSedatedHurting;
            Handlers.Player.Spawned -= OnImmunityResetEvent;

            //foreach (CoroutineHandle coroutine in coroutines)
            //{
            //    Timing.KillCoroutines(coroutine);
            //}

            //coroutines.Clear();
            base.UnsubscribeEvents();
        }

        protected override void OnWaitingForPlayers()
        {
            _effected.Clear();
            _immune.Clear();
            _reloading.Clear();
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            // For now
        }

        protected override void OnShot(ShotEventArgs ev)
        {
            if (ev.Target is Player player)
            {
                ev.Player.ShowHitMarker();
                Sedate(ev.Target);
            }
        }

        public void Sedate(Player player, bool ignoreResistence = false)
        {
            float duration = Duration;
            if (! player.IsAlive)
            {
                Log.Error($"{nameof(Tranquilizer)}: Tried to invoke {nameof(Sedate)} for a dead player.");
                return;
            }

            Log.Info($"{nameof(Tranquilizer)}: Invoked {nameof(Sedate)} for player {player.DisplayNickname}({player.UserId})");
            if (! ignoreResistence)
            {
                float num;
                float random = Random.Range(0, MaximumResistance + 1);

                if (! (_immune.TryGetValue(player, out num) && Immunity.TryGetValue(player.Role.Type, out num)))
                {
                    num = DefaultResistance;
                }

                if (num > random)
                {
                    Log.Info($"{nameof(Tranquilizer)}: Sedate resisted with odds ({num} > {random})");
                    return;
                }

                duration -= Duration * (num / MaximumResistance);
                num *= ResistanceModifier;
                _immune[player] = num;

                Log.Info($"{nameof(Tranquilizer)}: Sedated player, resistance changed to {num} with prolonged duration {duration}s");
            }

            _effected.Add(player);
            var previousScale = player.Scale;
            var previousItem = player.CurrentItem;
            var ragdoll = Ragdoll.CreateAndSpawn(player.Role.Type, player.DisplayNickname, "Sedated", player.Position, rotation: player.Rotation);

            player.Scale = new Vector3(0.1f, 0.1f, 0.1f);
            player.CurrentItem = null;

            if (DropItems)
            {
                player.DropItems();
            }

            if (player.Role is Scp096Role role)
            {
                role.Calm();
                role.ClearTargets();
            }

            player.EnableEffect<AmnesiaItems>(duration);
            player.EnableEffect<AmnesiaVision>(duration);
            player.EnableEffect<Ensnared>(duration);
            player.EnableEffect<Invisible>(duration);
            player.EnableEffect<DamageReduction>(255, duration);
            
            Timing.CallDelayed(duration, () =>
            {
                _effected.Remove(player);
                player.Scale = previousScale;
                player.Position += Vector3.up * 1.5f;
                player.CurrentItem = !DropItems ? previousItem : null;

                ragdoll.Destroy();
            });
        }

        private void OnDeniableEvent(IDeniableEvent e)
        {
            if (e is IPlayerEvent ev)
            {
                if (_effected.Contains(ev.Player))
                {
                    e.IsAllowed = false;
                }
            }
        }

        private void OnSedatedHurting(HurtingEventArgs ev)
        {
            if (_effected.Contains(ev.Attacker))
            {
                ev.IsAllowed = false;
            }
        }

        private void OnImmunityResetEvent(IPlayerEvent ev)
        {
            _immune.Remove(ev.Player);
        }
    }
}
