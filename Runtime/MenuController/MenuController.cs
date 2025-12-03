using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlueMuffinGames.Tools.MenuController
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private MenuPage _firstPage;
        [SerializeField] private bool _enableOnStart;

        private Stack<MenuPage> _navigationStack = new();

        public event Action onStackEmptied = delegate { };

        /// <summary>
        /// Push (show) a new page. Starts after hiding the current page, if any.
        /// </summary>
        /// <param name="page">The page to push.</param>
        /// <param name="onHideComplete">Invoked when the current page is hidden.</param>
        /// <param name="onShowComplete">Invoked when the new page is shown.</param>
        public virtual void PushPage(MenuPage page, Action onHideComplete = null, Action onShowComplete = null, params object[] args)
        {
            if (_navigationStack.TryPeek(out MenuPage currentPage)) currentPage.Hide(() =>
            {
                Push(); // push after the current page finishes hiding
            }, args);
            else Push(); // push immediately

            void Push()
            {
                _navigationStack.Push(page);
                onHideComplete?.Invoke();
                page.Show(onShowComplete, args);
            }
        }

        /// <summary>
        /// Pop (hide) the current page. Starts showing the previous page after hiding, if any.
        /// </summary>
        /// <param name="onHideComplete">Invoked when the current page is hidden.</param>
        /// <param name="onShowComplete">Invoked when the previous page is shown.</param>
        /// <returns>The popped page.</returns>
        public virtual MenuPage PopPage(Action onHideComplete = null, Action onShowComplete = null, params object[] args)
        {
            MenuPage result = null;

            if (_navigationStack.TryPeek(out result)) result.Hide(() =>
            {
                _navigationStack.Pop();
                onHideComplete?.Invoke();
                if (_navigationStack.TryPeek(out MenuPage previousPage)) previousPage.Show(onShowComplete, args);
                else onStackEmptied?.Invoke();
            }, args);
            
            return result;
        }
    }
}
