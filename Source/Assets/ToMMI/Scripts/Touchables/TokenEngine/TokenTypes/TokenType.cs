/*
 * @author Francesco Strada
 */

using System.Collections.Generic;

namespace Touchables.TokenEngine.TokenTypes
{
    /// <summary>
    /// Class reppresenting different token types measures
    /// </summary>
    internal static class TokenAttributes
    {
        //All distances are expressed in cm

        //internal const float TOKEN_3X3_ORIGIN_TO_CENTER_DST = 1.434f;
        //internal const float TOKEN_3X3_ORIGIN_TO_AXIS_MARKERS_DST = 1.75f;
        //internal const float TOKEN_3X3_DATA_MARKER_ORIGIN = 1.15f;

        internal const float TOKEN_3X3_ORIGIN_TO_CENTER_DST = 1.45f;
        internal const float TOKEN_3X3_ORIGIN_TO_AXIS_MARKERS_DST = 2.05f;
        internal const float TOKEN_3X3_DATA_MARKER_ORIGIN = 1.25f;
        internal const float TOKEN_3X3_DIAGONAL = 3.5f;//4.4 2.2


        internal const float TOKEN_4X4_ORIGIN_TO_CENTER_DST = 1.8f;
        internal const float TOKEN_4X4_ORIGIN_TO_AXIS_MARKERS_DST = 2.0f;
        internal const float TOKEN_4X4_DATA_MARKER_ORIGIN = 1.15f;
        internal const float TOKEN_4X4_DIAGONAL = 0.0f; //TO BE SET

        internal const float TOKEN_5X5_ORIGIN_TO_CENTER_DST = 1.8f;
        internal const float TOKEN_5X5_ORIGIN_TO_AXIS_MARKERS_DST = 2.0f;
        internal const float TOKEN_5X5_DATA_MARKER_ORIGIN = 1.15f;
        internal const float TOKEN_5X5_DIAGONAL = 0.0f; //TO BE SET

        //internal const float TOKEN_DATA_MARKERS_STEP = 0.3f;
        internal const float TOKEN_DATA_MARKERS_STEP = 0.4f;

    }


    abstract internal class TokenType
    {
        internal readonly float DistanceOriginCenterPX;
        internal readonly float DistanceOriginAxisMarkersPX;
        internal readonly float DataMarkerOriginPositionPX;
        internal readonly float DataGridMarkersStepPX;
        internal readonly float TokenDiagonalPX;

        internal readonly Dictionary<TokenDataGridCoord, int> TokenClassLUT;

        internal readonly float DataMarkerOriginPositionCM;

        protected TokenType()
        {
            DistanceOriginCenterPX = SetOriginToCenterDistance();
            DistanceOriginAxisMarkersPX = SetOriginToAxisDistance();
            DataMarkerOriginPositionPX = SetDataMarkerOriginPosition();
            DataMarkerOriginPositionCM = SetDataMarkerOriginPositionCM();
            TokenDiagonalPX = SetTokenDiagonal();

            TokenClassLUT = InitiliazeTokenClassLUT();

            DataGridMarkersStepPX = TokensEngine.CmToPixels(TokenAttributes.TOKEN_DATA_MARKERS_STEP);
        }

        internal int? GetTokenClass(int xIndex, int yIndex)
        {
            int result;
            if (TokenClassLUT.TryGetValue(new TokenDataGridCoord(xIndex, yIndex), out result))
                return result;
            else
                return null;
        }

        internal abstract float SetOriginToCenterDistance();
        internal abstract float SetOriginToAxisDistance();
        internal abstract float SetDataMarkerOriginPosition();
        internal abstract float SetDataMarkerOriginPositionCM();
        internal abstract float SetTokenDiagonal();
        internal abstract Dictionary<TokenDataGridCoord, int> InitiliazeTokenClassLUT();

    }

    internal struct TokenDataGridCoord
    {
        int x;
        int y;

        internal TokenDataGridCoord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
