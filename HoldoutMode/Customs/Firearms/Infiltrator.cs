namespace LatteMod.Customs.Firearms
{
    using System;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.EventArgs;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using InventorySystem.Items.Usables;
    using PlayerStatsSystem;
    using UnityEngine;
    using KnobSetting = Scp914.Scp914KnobSetting;
    [CustomItem(ItemType.GunCOM18)]
    public class Infiltrator : CustomWeapon
    {
        public override uint Id { get; set; } = 153001;
        public override string Name { get; set; } = "AXIOM-5 \"HF\" INFILTRATOR";
        public override string Description { get; set; } = "The latest technology prototype developed by TANUKI company, " +
            "contracted by site-12 as one of their gateaway weapon to prevent multiple scp containment break." +
            "This weaponry has been tested to withstand several anomalies and capable of destroying heavily armored infantries.";
        public override float Damage { get; set; } = 80f;
        public override float Weight { get; set; } = 10f;
        public override byte ClipSize { get; set; } = 5;
        public override SpawnProperties SpawnProperties { get; set; }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            var max = Math.Min(ev.Firearm.MagazineAmmo + 1, ClipSize);

            ev.Firearm.MaxMagazineAmmo = max;

            base.OnReloading(ev);
        }

        protected override void OnReloaded(ReloadedWeaponEventArgs ev)
        {
            ev.Firearm.MagazineAmmo = ev.Firearm.MaxMagazineAmmo;

            base.OnReloaded(ev);
        }

        protected override void OnUpgrading(UpgradingItemEventArgs ev)
        {
            if (ev.KnobSetting == KnobSetting.Rough || ev.KnobSetting == KnobSetting.Coarse)
            {
                ev.Item.Destroy();
            } else
            {
                Firearm infiltrator = ev.Item as Firearm;
                infiltrator.MagazineAmmo = ClipSize;
            }

            base.OnUpgrading(ev);
        }

        protected override void OnShot(ShotEventArgs ev)
        {
            Map.ExplodeEffect(ev.Position, Exiled.API.Enums.ProjectileType.Flashbang);

            var colliders = Physics.OverlapSphere(ev.Position, 3f);
            foreach (var collider in colliders)
            {
                if (Player.TryGet(collider.gameObject, out Player player))
                {
                    if (ev.Player == player || ev.Target == player || !(Server.FriendlyFire && ev.Player.LeadingTeam == player.LeadingTeam))
                    {
                        continue;
                    }

                    player.Hurt(new CustomDamageHandler(ev.Target, ev.Player, Damage));
                }
            }

            base.OnShot(ev);
        }

        protected override void OnHurting(HurtingEventArgs ev)
        {
            ev.DamageHandler = new CustomDamageHandler(ev.Player, ev.Attacker, Damage, Exiled.API.Enums.DamageType.ParticleDisruptor);

            if (ev.Player.LeadingTeam == Exiled.API.Enums.LeadingTeam.Anomalies)
            {
                ev.Amount = ev.Player.HumeShield > 0 ? Math.Max((ev.Player.MaxHumeShield / 5) * Damage, ev.Player.HumeShield) : Damage;
            }
            base.OnHurting(ev);
        }
    }
}
