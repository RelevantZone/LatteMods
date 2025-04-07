namespace LatteMods.CustomItems.Configs
{
    using System;
    using System.Collections.Generic;

    using Consumables = CustomItems.Items.Consumables;
    using Firearms = CustomItems.Items.Firearms;
    using Grenades = CustomItems.Items.Grenades;
    using Usables = CustomItems.Items.Usables;
    public class Items
    {
        public Version Version { get; private set; } = new Version(1, 0, 1);
        #region Consumables
        public List<Consumables.Armament> Armaments { get; private set; } = [new Consumables.Armament()];
        public List<Consumables.LethalInjection> LethalInjections { get; private set; } = [new Consumables.LethalInjection()];
        #endregion

        #region Firearms
        public List<Firearms.Axiom> Axioms { get; private set; } = [new Firearms.Axiom()];
        public List<Firearms.Injector> Injectors { get; private set; } = [new Firearms.Injector()];
        public List<Firearms.Sniper> Snipers { get; private set; } = [new Firearms.Sniper()];
        public List<Firearms.Tranquilizer> Tranquilizers { get; private set; } = [new Firearms.Tranquilizer()];
        #endregion

        #region Grenades
        public List<Grenades.DetonatedCharges> DetonatedCharges { get; private set; } = [new Grenades.DetonatedCharges()];
        public List<Grenades.EmpGrenade> EmpGrenades { get; private set; } = [new Grenades.EmpGrenade()];
        #endregion

        #region Usables
        public List<Usables.Scrambler> Scramblers { get; private set; } = [new Usables.Scrambler()];
        public List<Usables.FortuneCoin> FortuneCoins { get; private set; } = [new Usables.FortuneCoin()];
        #endregion
    }
}
