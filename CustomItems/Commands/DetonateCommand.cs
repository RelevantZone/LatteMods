namespace LatteMods.CustomItems.Commands
{
    using System;
    using System.Linq;
    using CommandSystem;
    using CustomItems.Items.Grenades;
    using Exiled.API.Features;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DetonateCommand : ICommand
    {
        public string Command { get; } = "detonate";
        public string[] Aliases { get; } = ["det"];
        public string Description { get; } = "Emits signal to detonate remote charges";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);

            if (! DetonatedCharges.Instance.HasActiveCharges(player))
            {
                response = "\n<color=red>You haven't placed any charges to be detonated!</color>";
                return false;
            }

            if (DetonatedCharges.Instance.RequireDetonator && player.CurrentItem.Type != DetonatedCharges.Instance.DetonatorTool)
            {
                response = $"\n<color=red>You need to hold the detonator tool {Enum.GetName(typeof(ItemType), DetonatedCharges.Instance.DetonatorTool)}!</color>";
                return false;
            }

            DetonatedCharges.Instance.DetonateAllCharges(player);

            response = $"\n<color=green>Successfully detonated remote charges</color>";
            return true;
        }
    }
}
