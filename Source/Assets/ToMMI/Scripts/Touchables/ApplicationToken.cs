/*
 * @author Francesco Strada
 */

using System;
using Touchables.TokenEngine;
using UnityEngine;

namespace Touchables
{
    [AddComponentMenu("Touchable/ApplicationToken")]
    public class ApplicationToken : MonoBehaviour
    {
        public int TokenClass = -22;
        public UnityEngine.Object Target = null;

        public MonoBehaviour[] targetComponents = null;
        public int selectedComponent = 0;

        private ITokenEvents TokenFunction = null;

        #region Event Handlers
        public virtual void OnTokenPlacedOnScreen(object sender, ApplicationTokenEventArgs e)
        {
            if (TokenFunction != null)
                TokenFunction.OnTokenPlacedOnScreen(sender, e);
        }

        public virtual void OnTokenRemovedFromScreen(object sender, ApplicationTokenEventArgs e)
        {
            if (TokenFunction != null)
                TokenFunction.OnTokenRemovedFromScreen(sender, e);
        }

        public virtual void OnTokenUpdated(object sender, ApplicationTokenEventArgs e)
        {
            if (TokenFunction != null)
                TokenFunction.OnTokenUpdated(sender, e);
        }
        #endregion

        #region Unity Methods
        protected void OnEnable()
        {

            TokenManager.Instance.TokenPlacedOnScreen += OnTokenPlacedOnScreen;
            TokenManager.Instance.ScreenTokenUpdated += OnTokenUpdated;
            TokenManager.Instance.TokenRemovedFromScreen += OnTokenRemovedFromScreen;

            if (targetComponents != null)
                TokenFunction = targetComponents[selectedComponent] as ITokenEvents;

        }

        protected void OnDisable()
        {
                TokenManager.Instance.TokenPlacedOnScreen -= OnTokenPlacedOnScreen;
                TokenManager.Instance.ScreenTokenUpdated -= OnTokenUpdated;
                TokenManager.Instance.TokenRemovedFromScreen -= OnTokenRemovedFromScreen;                
        }

        #endregion


    }
}
