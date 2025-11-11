using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlueMuffinGames.Tools.StateMachine
{
    public abstract class State : MonoBehaviour
    {
        private List<StateTransition> _transitions = new();

        public event Action<State> OnShouldTransition = delegate { };
        public event Action OnEntered = delegate { };
        public event Action OnExited = delegate { };

        /// <summary>
        /// Called when this state is registered to the StateMachine.
        /// </summary>
        public virtual void Initialize(StateMachine stateMachine)
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out StateTransition transition))
                {
                    _transitions.Add(transition);
                    transition.Initialize(stateMachine, this);
                }
            }
        }

        /// <summary>
        /// Called when entering this state, right before Enter().
        /// </summary>
        public virtual void Entering()
        {
            foreach (StateTransition transition in _transitions)
            {
                transition.OnShouldTransition += HandleShouldTransition;
            }
        }

        /// <summary>
        /// Called when this state is entered.
        /// </summary>
        public virtual void Enter() { OnEntered?.Invoke(); }

        /// <summary>
        /// Called when exiting this state, right before Exit().
        /// </summary>
        public virtual void Exiting()
        {
            foreach (StateTransition transition in _transitions)
            {
                transition.OnShouldTransition -= HandleShouldTransition;
            }
        }

        /// <summary>
        /// Called when this state is exited.
        /// </summary>
        public virtual void Exit() { OnExited?.Invoke(); }

        private void HandleShouldTransition(State targetState)
        {
            OnShouldTransition?.Invoke(targetState);
        }
    }
}
