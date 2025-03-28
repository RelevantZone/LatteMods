namespace LatteMod.Components
{
    using System.Collections.Generic;
    using CommandSystem.Commands.RemoteAdmin.Dummies;
    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Mirror;
    using PlayerRoles.FirstPersonControl;
    using UnityEngine;

    using Handlers = Exiled.Events.Handlers;
    public enum ExpendableBehaviour
    {
        Idle,
        Cautious,
        Panic,
    }

    public class Expendable : MonoBehaviour
    {
        public ReferenceHub _hub;
        public Npc _npc;

        public Dictionary<PlayerMovementState, float> MovementSpeed = new Dictionary<PlayerMovementState, float>
        {
            [PlayerMovementState.Sneaking] = 12f,
            [PlayerMovementState.Crouching] = 11f,
            [PlayerMovementState.Walking] = 24f,
            [PlayerMovementState.Sprinting] = 30f
        };

        public bool IsArmed
        {
            get;
            private set;
        }

        public void Init(ReferenceHub owner)
        {
            _hub = GetComponent<ReferenceHub>();
            _npc = Npc.Get(_hub);
        }

        public void Update()
        {
            if (!NetworkServer.active || _hub == null || _npc == null )
            {
                Object.Destroy(this);
                return;
            }
        }

        public void SubscribeEvents()
        {
            
        }

        public void UnsubscribeEvents()
        {

        }
    }
}
