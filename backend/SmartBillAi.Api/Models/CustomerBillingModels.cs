namespace SmartBillAi.Api.Models;

public class DailyUsagePoint
{
    public string Date { get; set; } = string.Empty;
    public int DayNumber { get; set; }
    public double Kwh { get; set; }
    public double AvgTempF { get; set; }
    public bool IsWeekend { get; set; }
}

public class BillPeriodRecord
{
    public string PeriodId { get; set; } = string.Empty; // e.g. "2026-07"
    public string MonthName { get; set; } = string.Empty; // e.g. "July 2026"
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public int BillingDays { get; set; }
    public double ConsumptionKwh { get; set; }
    public double TotalBillAmount { get; set; }
    public double EnergyCharges { get; set; }
    public double FixedCharges { get; set; }
    public double TaxesAndFees { get; set; }
    public double RatePerKwh { get; set; }
    public double AvgDailyKwh { get; set; }
    public double AvgTemperatureF { get; set; }
    public double CoolingDegreeDays { get; set; }
    public double HeatingDegreeDays { get; set; }
    public string Status { get; set; } = "Current"; // "Paid", "Current", "Due"
    public List<DailyUsagePoint> DailyUsage { get; set; } = new();
}

public class CustomerProfileDto
{
    public string AccountNumber { get; set; } = "ACT-7492-EAST";
    public string CustomerName { get; set; } = "Eleanor Vance";
    public string ServiceAddress { get; set; } = "4420 Sycamore Crest Way, Columbus, OH 43215";
    public string MeterId { get; set; } = "MTR-OH-9021";
    public string RatePlan { get; set; } = "Residential Standard Electric (RS-1)";
    public bool IsSimulatedData { get; set; } = true;
    public List<BillPeriodRecord> BillingHistory { get; set; } = new();
}

public class CustomerDashboardSummaryDto
{
    public CustomerProfileDto Profile { get; set; } = new();
    public BillPeriodRecord SelectedBill { get; set; } = new();
    public BillPeriodRecord PreviousBill { get; set; } = new();
    public double BillAmountDelta { get; set; }
    public double BillAmountPercentChange { get; set; }
    public double ConsumptionDeltaKwh { get; set; }
    public double ConsumptionPercentChange { get; set; }
    public double AvgDailyDeltaKwh { get; set; }
    public int BillingDaysDelta { get; set; }
    public string ExplanationStatus { get; set; } = "Ready";
}

public class ExplainBillRequest
{
    public string AccountNumber { get; set; } = string.Empty;
    public string SelectedPeriodId { get; set; } = string.Empty;
    public string? BaselinePeriodId { get; set; }
}

public class ContributingFactorItem
{
    public string FactorName { get; set; } = string.Empty;
    public string ImpactLevel { get; set; } = "High"; // High, Medium, Low
    public double EstimatedDollarImpact { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsFactBased { get; set; } = true; // Fact vs Inference
}

public class EnergyRecommendationItem
{
    public string Category { get; set; } = "Cooling";
    public string Title { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Rationale { get; set; } = string.Empty;
    public double EstimatedMonthlySavings { get; set; }
    public string Feasibility { get; set; } = "Easy"; // Easy, Moderate, Investment
}

public class ExplainBillResponse
{
    public string PeriodId { get; set; } = string.Empty;
    public string ComparisonPeriodId { get; set; } = string.Empty;
    public string PlainLanguageSummary { get; set; } = string.Empty;
    public List<string> ObservedChanges { get; set; } = new();
    public Dictionary<string, string> SupportingFigures { get; set; } = new();
    public List<ContributingFactorItem> ContributingFactors { get; set; } = new();
    public List<EnergyRecommendationItem> PracticalRecommendations { get; set; } = new();
    public string ConfidenceScore { get; set; } = "95% (High Confidence - Verified Smart Meter & Weather Data)";
    public string SourceEngine { get; set; } = "GenAI Lab (gpt-4o-mini) + Deterministic Tariff Engine";
    public bool DistinguishesFactsFromInferences { get; set; } = true;
}

public class BillComparisonRequest
{
    public string AccountNumber { get; set; } = string.Empty;
    public string PeriodIdA { get; set; } = string.Empty;
    public string PeriodIdB { get; set; } = string.Empty;
}

public class BillComparisonResponse
{
    public BillPeriodRecord PeriodA { get; set; } = new();
    public BillPeriodRecord PeriodB { get; set; } = new();
    public double BillDeltaAmount { get; set; }
    public double BillDeltaPercent { get; set; }
    public double ConsumptionDeltaKwh { get; set; }
    public double ConsumptionDeltaPercent { get; set; }
    public int DaysDelta { get; set; }
    public double AvgDailyDeltaKwh { get; set; }
    public double CoolingDegreeDaysDelta { get; set; }
    public double TariffDifferenceAmount { get; set; }
    public string ComparisonSummary { get; set; } = string.Empty;
}
