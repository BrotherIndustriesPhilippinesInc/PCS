namespace PartsControlSystem.Models
{
    public static class Other4MProcessMappingSeed
    {
        public static Other4MProcessMapping[] GetSeedData()
        {
            return new[]
            {
                new Other4MProcessMapping { Id = 1,  ProcessStep = "Test Run meeting date",        StepOrder = 1,  LeadTimeDays = 5,  LeadTimeNote = "5 days after 4M form received" },
                new Other4MProcessMapping { Id = 2,  ProcessStep = "Kataken Request date",          StepOrder = 2,  LeadTimeDays = 1,  LeadTimeNote = "1 day after 4M form received" },
                new Other4MProcessMapping { Id = 3,  ProcessStep = "Kataken PH Sample Submission",  StepOrder = 3,  LeadTimeDays = null, LeadTimeNote = "Manual Input/0" },
                new Other4MProcessMapping { Id = 4,  ProcessStep = "Kataken Evaluation Approval",   StepOrder = 4,  LeadTimeDays = 7,  LeadTimeNote = "7 days" },
                new Other4MProcessMapping { Id = 5,  ProcessStep = "DE Evaluation",                 StepOrder = 5,  LeadTimeDays = 10, LeadTimeNote = "10 days" },
                new Other4MProcessMapping { Id = 6,  ProcessStep = "EE Evaluation",                 StepOrder = 6,  LeadTimeDays = 10, LeadTimeNote = "10 days" },
                new Other4MProcessMapping { Id = 7,  ProcessStep = "QA Evaluation",                 StepOrder = 7,  LeadTimeDays = 10, LeadTimeNote = "10 days" },
                new Other4MProcessMapping { Id = 8,  ProcessStep = "ITF Process",                   StepOrder = 8,  LeadTimeDays = 2,  LeadTimeNote = "2 days" },
                new Other4MProcessMapping { Id = 9,  ProcessStep = "Delivery PO Requisition",       StepOrder = 9,  LeadTimeDays = 1,  LeadTimeNote = "1 day after QA evaluation" },
                new Other4MProcessMapping { Id = 10, ProcessStep = "Test Run PO request",           StepOrder = 10, LeadTimeDays = 1,  LeadTimeNote = "1 day after delivery of Test Run" },
                new Other4MProcessMapping { Id = 11, ProcessStep = "TEST RUN",                      StepOrder = 11, LeadTimeDays = 2,  LeadTimeNote = "2 days" },
                new Other4MProcessMapping { Id = 12, ProcessStep = "IMPLEMENTATION DATE",           StepOrder = 12, LeadTimeDays = null, LeadTimeNote = "Depends on DCI implementation / current material stocks depletion" },
                new Other4MProcessMapping { Id = 13, ProcessStep = "FIRST DELIVERY DATE",           StepOrder = 13, LeadTimeDays = null, LeadTimeNote = "Depends on DCI implementation / current material stocks depletion" },
            };
        }
    }
}