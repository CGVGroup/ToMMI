/*
 * @author Francesco Strada
 */

using System;

namespace Touchables.Utils
{
    internal sealed class TokenStatistics
    {
        private static readonly TokenStatistics _instance = new TokenStatistics();

        private float tokensRequestingIdentification = 0.0f;
        private int successfullTokenIdentification = 0;
        private int failedTokenIdentification = 0;
        private float succesfullTokenIdentificationPercentage = 0.0f;

        private int expectedTokenClass = 0;
        private int totalClassComputedTokens = 0;
        private int successfullClassRecon = 0;

        public int TokenIdentificationSuccessRate
        {
            get
            {
                return (int)Math.Round(succesfullTokenIdentificationPercentage, 0, MidpointRounding.AwayFromZero);
            }
        }

        public int TotalTokens { get { return (int)tokensRequestingIdentification; } }
        public int FailedIdentificationTokens { get { return failedTokenIdentification; } }

        public static TokenStatistics Instance
        {
            get
            {
                return _instance;
            }
        }

        public int ExpectedTokenClass
        {
            get
            {
                return expectedTokenClass;
            }
            set
            {
                expectedTokenClass = value;
                totalClassComputedTokens = 0;
                successfullClassRecon = 0;

            }
        }

        public int TotalTokenClassRequest { get { return totalClassComputedTokens; } }
        public int SuccessfullTokenClassRecon { get { return successfullClassRecon; } }

        private TokenStatistics() { }

        internal void ResetMetrics()
        {
            tokensRequestingIdentification = 0.0f;
            successfullTokenIdentification = 0;
            failedTokenIdentification = 0;
            succesfullTokenIdentificationPercentage = 0.0f;

        }

        internal void TokenIdentification(bool succesfull)
        {
            tokensRequestingIdentification++;

            if (succesfull)
            {
                successfullTokenIdentification++;
                succesfullTokenIdentificationPercentage = (successfullTokenIdentification / tokensRequestingIdentification) * 100;
            }
            else
                failedTokenIdentification++;
        }

        internal void TokenClassRecognition(int? tokenClass)
        {
            totalClassComputedTokens++;
            if (tokenClass == expectedTokenClass)
                successfullClassRecon++;
        }

    }
}
