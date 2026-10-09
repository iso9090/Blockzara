namespace Blockzara.Runtime
{
    public sealed class RoundAdPolicy
    {
        bool roundOpen = true;
        bool continueUsed;

        public bool ContinueAvailable => roundOpen && !continueUsed;

        public void BeginRound()
        {
            roundOpen = true;
            continueUsed = false;
        }

        public bool ClaimContinue()
        {
            if (!ContinueAvailable) return false;
            continueUsed = true;
            return true;
        }

        public void RefundContinue()
        {
            if (roundOpen) continueUsed = false;
        }

        public bool ClaimEndOfRound()
        {
            if (!roundOpen) return false;
            roundOpen = false;
            return true;
        }
    }
}
