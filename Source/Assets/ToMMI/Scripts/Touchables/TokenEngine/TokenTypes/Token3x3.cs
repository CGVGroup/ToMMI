/*
 * @author Francesco Strada
 */

using System.Collections.Generic;

namespace Touchables.TokenEngine.TokenTypes
{
    internal sealed class Token3x3 : TokenType
    {
        internal override float SetOriginToAxisDistance()
        {
            return TokensEngine.CmToPixels(TokenAttributes.TOKEN_3X3_ORIGIN_TO_AXIS_MARKERS_DST);
        }

        internal override float SetOriginToCenterDistance()
        {
            return TokensEngine.CmToPixels(TokenAttributes.TOKEN_3X3_ORIGIN_TO_CENTER_DST);
        }

        internal override float SetDataMarkerOriginPosition()
        {
            return TokensEngine.CmToPixels(TokenAttributes.TOKEN_3X3_DATA_MARKER_ORIGIN);
        }

        internal override float SetDataMarkerOriginPositionCM()
        {
            return TokenAttributes.TOKEN_3X3_DATA_MARKER_ORIGIN;
        }

        internal override float SetTokenDiagonal()
        {
            return TokensEngine.CmToPixels(TokenAttributes.TOKEN_3X3_DIAGONAL);
        }

        internal override Dictionary<TokenDataGridCoord, int> InitiliazeTokenClassLUT()
        {
            Dictionary<TokenDataGridCoord, int> classLUT = new Dictionary<TokenDataGridCoord, int>();

            classLUT.Add(new TokenDataGridCoord(0, 0), 0);
            classLUT.Add(new TokenDataGridCoord(0, 1), 1);
            classLUT.Add(new TokenDataGridCoord(0, 2), 2);
            classLUT.Add(new TokenDataGridCoord(1, 0), 3);
            classLUT.Add(new TokenDataGridCoord(1, 1), 4);
            classLUT.Add(new TokenDataGridCoord(1, 2), 5);
            classLUT.Add(new TokenDataGridCoord(2, 0), 6);
            classLUT.Add(new TokenDataGridCoord(2, 1), 7);
            classLUT.Add(new TokenDataGridCoord(-1, -1), 8);

            return classLUT;
        }
    }
}
