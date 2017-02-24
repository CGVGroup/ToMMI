/*
 * @author Francesco Strada
 */

using Touchables.MultiTouchManager;
using Touchables.TokenEngine;
using Touchables.TokenEngine.TokenTypes;
using UnityEngine;

//Prof has done editing on this file

namespace Touchables
{
    [AddComponentMenu("Touchable/TokensEngine")]
    public class TokensEngine : MonoBehaviour
    {
        private static TokensEngineProperties _pars = new TokensEngineProperties();

        public static TokensEngineProperties Pars { get {  return _pars; } }
        #region Unity Methods

        void Awake()
        {
            // initializes TOken and Cluster managers
            TokenManager.Instance.Initialize();

            // forcing FPS to better deal with marker & touches on mobile devices
            if (_pars.Target60FPS)
                Application.targetFrameRate = 60;

        }

        void Update()
        {            
            // chek touches and clusters them
            ClusterManager.Instance.Update();
        }

        void OnDestroy()
        {
            TokenManager.Instance.Disable();
        }

        #endregion

        #region Public Methods

        public TokensEngineProperties GetPars()
        {
            return _pars;
        }
        public static float PixelsToCm(float pxValue)
        {
            return (pxValue / Screen.dpi) * 2.54f;
        }


        public static float CmToPixels(float cmValue)
        {
            return ((cmValue * Screen.dpi) / 2.54f);
        }
        #endregion

    }
}
