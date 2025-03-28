namespace LatteMod.Configs
{
    using System.Collections.Generic;
    using LatteMod.Customs.Constructions;
    using LatteMod.Customs.Consumables;
    using LatteMod.Customs.Firearms;
    using LatteMod.Customs.Grenades;

    public class Items
    {
        public bool IsEnabled { get; set; } = true;
        public List<Stabilizer> Stabilizers { get; private set; } = new List<Stabilizer>
        {
            new Stabilizer()
        };

        public List<InfraredMine> InfraredMines { get; private set; } = new List<InfraredMine>
        {
            new InfraredMine()
        };

        public List<CombatAdrenaline> CombatAdrenalines { get; private set; } = new List<CombatAdrenaline>
        {
            new CombatAdrenaline()
        };

        public List<NanoEnforcer> NanoEnforcers { get; private set; } = new List<NanoEnforcer>
        {
            new NanoEnforcer()
        };
        public List<Infiltrator> Infiltrators { get; private set; } = new List<Infiltrator>
        {
            new Infiltrator()
        };
        public List<Expendable> Expendables { get; private set; } = new List<Expendable>
        {
            new Expendable()
        };
    }
}
