using System;

namespace TrackingChain.TransactionTriageCore.ModelViews
{
    public class TrackingStatusStatisticModelView
    {
        // Properties
        public int Triage { get; set; }
        public int Pool { get; set; }
        public int Pending { get; set; }
        public int ErrorToManage { get; set; }
        public int Error { get; set; }
        public int Successful { get; set; }
        
        public int TriagePercentage => CalculatePercentage(Triage);
        public int PoolPercentage => CalculatePercentage(Pool);
        public int PendingPercentage => CalculatePercentage(Pending);
        public int ErrorToManagePercentage => CalculatePercentage(ErrorToManage);
        
        public int ErrorPercentage => CalculatePercentage(Error);

        public int ErrorPercentageOnSuccessful => CalculatePercentageOnSuccessful(Error);
        public int SuccessfulPercentage => CalculatePercentageOnSuccessful(Successful);


        public int TimingAvgTriageToPool { get; set; }
        public int TimingAvgPoolToPending { get; set; }
        public int TimingAvgPendingToCompleted { get; set; }
        public int TimingAvgTriageToCompleted { get; set; }
        public int TimingMinAvgLastMonthTriageToCompleted { get; set; }
        public int TimingMaxAvgLastMonthTriageToCompleted { get; set; }

        //Helpers.

        private int CalculatePercentage(int value)
        {
            int total = Triage + Pool + Pending;
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
