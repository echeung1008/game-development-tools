using System.Collections;
using UnityEngine;

namespace BlueMuffinGames.Tools.StateMachine
{
    public class DurationTransition : StateTransition
    {
        public float duration = 1f;

        private Coroutine _currentTimer;

        public override void Initialize(StateMachine stateMachine, State state)
        {
            base.Initialize(stateMachine, state);

            state.OnEntered += StartTimer;
            state.OnExited += StopTimer;
        }

        private void StartTimer()
        {
            _currentTimer = StartCoroutine(Timer());
        }

        private void StopTimer()
        {
            if (_currentTimer != null) StopCoroutine(_currentTimer);

            _currentTimer = null;
        }

        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(duration);
            Transition();
        }
    }
}
