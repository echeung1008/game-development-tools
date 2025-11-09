using UnityEngine;

namespace BlueMuffinGames.Tools
{
    public abstract class State : MonoBehaviour
    {
        /// <summary>
        /// Called when this state is registered to the StateMachine.
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// Called when entering this state, right before Enter().
        /// </summary>
        public virtual void Entering() { }

        /// <summary>
        /// Called when this state is entered.
        /// </summary>
        public virtual void Enter() { }

        /// <summary>
        /// Called when exiting this state, right before Exit().
        /// </summary>
        public virtual void Exiting() { }

        /// <summary>
        /// Called when this state is exited.
        /// </summary>
        public virtual void Exit() { }
    }
}
