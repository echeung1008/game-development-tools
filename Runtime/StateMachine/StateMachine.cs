using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlueMuffinGames.Tools.StateMachine
{
    public class StateMachine : MonoBehaviour
    {
        [SerializeField] private bool _allowExternalStates = false;
        [SerializeField] private int _initialStateIndex = 0;
        [SerializeField] private bool _printWarningsAndErrors = false;

        public State CurrentState { get; private set; }

        private Dictionary<string, State> _states = new();

        public event Action<State> OnStateExited = delegate { };
        public event Action<State> OnStateEntered = delegate { };

        public virtual void ChangeState(string stateName)
        {
            if (_states.TryGetValue(stateName, out State state))
            {
                ChangeState(state);
            }
            else LogWarning($"State {stateName} is not registered in StateMachine {name}");
        }

        public virtual void ChangeState(State state)
        {
            if (state == null) return;
            if (!_allowExternalStates && !_states.Values.Contains(state)) { LogError($"Attempted to transition to {state.name} when external states aren't allowed in StateMachine {name}."); return; }
            if (state == CurrentState) return;

            if (CurrentState != null)
            {
                CurrentState.Exiting();
                CurrentState.Exit();
                CurrentState.OnShouldTransition -= ChangeState;
                OnStateExited?.Invoke(CurrentState);
            }

            CurrentState = state;

            CurrentState.Entering();
            CurrentState.Enter();
            CurrentState.OnShouldTransition += ChangeState;
            OnStateEntered?.Invoke(CurrentState);
        }

        protected virtual void Awake()
        {
            int numStates = 0;

            // initialize and register states
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out State state))
                {
                    if (_states.ContainsKey(state.name)) { LogWarning($"State name {state.name} is conflicting with another state."); continue; }

                    _states[state.name] = state;
                    state.Initialize();
                    numStates++;
                }
            }

            Log($"Registered {numStates} states in StateMachine {name}");

            ChangeState(_states.Values.ToList()[Mathf.Min(numStates-1, _initialStateIndex)]);
        }

        #region Debugging
        private void Log(string msg)
        {
            if (!_printWarningsAndErrors) return;

            Debug.Log(msg);
        }

        private void LogWarning(string msg)
        {
            if (!_printWarningsAndErrors) return;

            Debug.LogWarning(msg);
        }

        private void LogError(string msg)
        {
            if (!_printWarningsAndErrors) return;

            Debug.LogError(msg);
        }
        #endregion
    }
}
