using System;
using System.Collections.Generic;
using Automatonymous;

namespace BasketTracking
{
    public class BasketStateMachineState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }

        public string UserName { get; set; }


        public Dictionary<string, int> Items { get; set; }
    }
}