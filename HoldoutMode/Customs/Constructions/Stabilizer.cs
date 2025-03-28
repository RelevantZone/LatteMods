namespace LatteMod.Customs.Constructions
{
    using System.Collections;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Spawn;
    using Exiled.API.Features.Toys;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using Mirror;
    using UnityEngine;

    using PlayerEvent = Exiled.Events.Handlers.Player;

    [CustomItem(ItemType.Medkit)]
    public class Stabilizer : CustomItem
    {
        public override uint Id { get; set; } = 150001;
        public override string Name { get; set; } = "INVIL PMS-Stabilizer";
        public override string Description { get; set; } = "A portable Medical Station developed by INVIL with the latest in dispersion Nanite technology, " +
            "capable of smartly repairing wounds and damages to nearby MTF instead of potential threats or decaying in the air. While not cheap to develop, " +
            "definitely very effective in the field for keeping MTF in combat while also patched up at the same time.";
        public override float Weight { get; set; } = 35f;
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties { };

        // Settings
        public float HealingRange { get; set; } = 8f;
        public float Duration { get; set; } = 60f;
        public float PointsPerFrame = 0.05f;
        public Vector3 PrefabGeneratorScale = new Vector3(3, 3, 3);
        // Misc
        public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();
        protected override void SubscribeEvents()
        {
            PlayerEvent.UsingItemCompleted += CreateRegenerationField;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            PlayerEvent.UsingItemCompleted -= CreateRegenerationField;

            foreach (var coroutine in Coroutines)
            {
                Timing.KillCoroutines(coroutine);
            }
            Coroutines.Clear();

            base.UnsubscribeEvents();
        }

        public void CreateRegenerationField(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Item)) return;

            ev.IsAllowed = false;
            ev.Item.Destroy();

            var pole = Primitive.Create(PrimitiveType.Cube, scale: new Vector3(1f, 3f, 1f));
            var flag = Primitive.Create(PrimitiveType.Cube, scale: new Vector3(1f, 1.2f, 1.2f));
            var light = Exiled.API.Features.Toys.Light.Create();

            pole.Position = ev.Player.Position + new Vector3(0, -ev.Player.Scale.y, 0);
            flag.Position = ev.Player.Position;
            flag.Color = Color.green;

            Coroutines.Add(Timing.RunCoroutine(RegenerateHealth(light.GameObject, ev.Player.LeadingTeam), light.GameObject));

            Timing.CallDelayed(Duration, () =>
            {
                pole.Destroy();
                flag.Destroy();
                light.Destroy();
            });
            
        }

        public IEnumerator<float> RegenerateHealth(GameObject center, LeadingTeam faction)
        {
            while (NetworkServer.active)
            {
                foreach (Player player in Player.List)
                {
                    if ((player.LeadingTeam == faction) &&
                        (Vector3.Distance(player.Position, center.transform.position) < HealingRange - 3))
                    {
                        player.Heal(PointsPerFrame);
                    }
                }

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
