using System;
using UnityEngine;

namespace BlueMuffinGames.Tools.MenuController
{
    public class MenuPage : MonoBehaviour
    {
        /// <summary>
        /// Show this page.
        /// </summary>
        /// <param name="onComplete">Invoked when finished showing.</param>
        /// <param name="args"></param>
        public virtual void Show(Action onComplete = null, params object[] args)
        {
            gameObject.SetActive(true);
            onComplete?.Invoke();
        }

        /// <summary>
        /// Hide this page.
        /// </summary>
        /// <param name="onComplete">Invoked when finished hiding.</param>
        /// <param name="args"></param>
        public virtual void Hide(Action onComplete = null, params object[] args)
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }

        /// <summary>
        /// Invoked by the parent MenuController.
        /// </summary>
        public virtual void Initialize() { }
    }
}
