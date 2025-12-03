using System;
using UnityEngine;

namespace BlueMuffinGames.Tools.MenuController
{
    public class MenuPage : MonoBehaviour
    {
        public virtual void Show(Action onComplete = null, params object[] args)
        {
            gameObject.SetActive(true);
            onComplete?.Invoke();
        }

        public virtual void Hide(Action onComplete = null, params object[] args)
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }
    }
}
