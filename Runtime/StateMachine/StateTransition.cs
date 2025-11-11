using System;
using UnityEngine;

namespace BlueMuffinGames.Tools.StateMachine
{
    public class StateTransition : MonoBehaviour
    {
        [SerializeField] private State _targetState;

        public event Action<State> OnShouldTransition = delegate { };

        /// <summary>
        /// Called when this transition is registered to a State.
        /// </summary>
        public virtual void Initialize(StateMachine stateMachine, State state) { }

        /// <summary>
        /// Tells the StateMachine to transition to the target state.
        /// </summary>
        protected void Transition()
        {
            if (_targetState == null) return;

            OnShouldTransition?.Invoke(_targetState);
        }
    }
}
