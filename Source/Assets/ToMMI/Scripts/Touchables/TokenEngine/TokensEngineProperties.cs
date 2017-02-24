/*
 * @author Francesco Strada
 */

namespace Touchables.TokenEngine
{
    public class TokensEngineProperties
    {
        public const int PIXELS = 0;
        public const int CENTIMETERS = 1;

        public const int TOKEN3x3 = 0;
        public const int TOKEN4x4 = 1;
        public const int TOKEN5x5 = 2;

        public bool ContinuousMeanSquare = false;
        public bool Target60FPS = false;
        public float RotationThr = 0.1f;
        public float TranslationThr = 0.1f;
        public int ComputePixels = PIXELS;
        public bool MeanSquare = true;
        public int Type;

    }
}
