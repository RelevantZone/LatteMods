namespace LatteMods.CustomItems.Items.Consumables
{
    using CustomPlayerEffects;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Roles;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using YamlDotNet.Serialization;
    using Events = Exiled.Events.Handlers;
    [CustomItem(ItemType.Adrenaline)]
    public class LethalInjection : CustomItem
    {
        public override uint Id { get; set; } = 1102;
        public override string Name { get; set; } = "<b><color=#00ff00ff>INVIL</color> Lethal S-02</b>";
        public override string Description { get; set; } = "Distribution of technology developed by <b><color=#00ff00ff>INVIL</color></b>, " +
            "capable of returning Scp096 into impassive state." +
            "\n<color=yellow>Warning</color>: <color=red>This variant of nanites is <b>dangerous</b> for your body</color>";
        public override float Weight { get; set; }
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            RoomSpawnPoints =
            {
                new RoomSpawnPoint
                {
                    Chance = 100,
                    Room = Exiled.API.Enums.RoomType.Hcz096
                },
                new RoomSpawnPoint
                {
                    Chance = 100,
                    Room = Exiled.API.Enums.RoomType.HczTestRoom
                }
            },
            Limit = 2
        };

        protected override void SubscribeEvents()
        {
            Events.Player.UsedItem += OnInjection;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Events.Player.UsedItem -= OnInjection;

            base.UnsubscribeEvents();
        }

        public void OnInjection(UsedItemEventArgs ev)
        {
            if (!Check(ev.Item))
            {
                return;
            }

            ev.Player.Kill(Exiled.API.Enums.DamageType.Poison);
            
            foreach (Player player in Player.List)
            {
                if (player.Role.Is(out Scp096Role role))
                {
                    role.ClearTargets();
                    role.Calm();

                    player.ShowHint(new Hint($"The soothing flow of blood calms you down.."));
                }
            }
        }
    }
}
