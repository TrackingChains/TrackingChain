using System;

namespace TrackingChain.TransactionTriageCore.ModelViews
{
    public class TrackingStatusStatisticModelView
    {
        // Properties
        public int Triage { get; set; }
        public int Pool { get; set; }
        public int Pending { get; set; }
        public int Error { get; set; }
        public int Successful { get; set; }
        
        public int TriagePercentage => CalculatePercentage(Triage);
        public int PoolPercentage => CalculatePercentage(Pool);
        public int PendingPercentage => CalculatePercentage(Pending);
        public int ErrorPercentage => CalculatePercentage(Error);

        public int ErrorPercentageOnSuccessful => CalculatePercentageOnSuccessful(Error);
        public int SuccessfulPercentage => CalculatePercentageOnSuccessful(Successful);

        //Helpers.

        private int CalculatePercentage(int value)
        {
            int total = Triage + Pool + Pending + Error;
            if (total == 0)
                return 0;

            return (int)Math.Round(value / (decimal)total * 100);
        }

        private int CalculatePercentageOnSuccessful(int value)
        {
            int total = Successful + Error;
            if (total == 0)
                return 0;

            return (value / total) * 100;
        }
    }   
}
