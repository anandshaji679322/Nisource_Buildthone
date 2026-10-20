namespace SmartBillAi.Api.Models;

public class WeatherMetricDto
{
    public string Period { get; set; } = string.Empty;
    public double AvgTemperatureF { get; set; }
    public double MaxTemperatureF { get; set; }
    public double MinTemperatureF { get; set; }
    public double CoolingDegreeDays { get; set; }
    public double HeatingDegreeDays { get; set; }
    public double AvgHumidityPercent { get; set; }
    public string WeatherConditionSummary { get; set; } = "Typical Seasonality";
    public string WeatherSource { get; set; } = "Live Open-Meteo Reanalysis"; // Live Open-Meteo, Historical Reanalysis, Cached, Simulated
}

public class MeterMonthlyReading
{
    public string PeriodId { get; set; } = string.Empty; // e.g. "2026-04"
    public string MonthLabel { get; set; } = string.Empty; // e.g. "Apr 2026"
    public int DaysInPeriod { get; set; } = 30;
    public double? ActualKwh { get; set; } // Null if missing
    public bool IsActual { get; set; } = true;
    public double AvgTempF { get; set; }
    public double CoolingDegreeDays { get; set; }
    public double HeatingDegreeDays { get; set; }
}

public class MeterExceptionRecord
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string MeterId { get; set; } = string.Empty;
    public string ServiceAddress { get; set; } = string.Empty;
    public string Region { get; set; } = "Central Ohio (Columbus)";
    public string CustomerClass { get; set; } = "Residential Electric";
    public string LastActualReadingDate { get; set; } = "2026-06-30";
    public string LastActualPeriod { get; set; } = "June 2026 (385 kWh)";
    public string MissingPeriod { get; set; } = "July 2026";
    public int MissingPeriodDays { get; set; } = 31;
    public double Baseline3MonthAvgKwh { get; set; }
    public double SmartBillAiEstimateKwh { get; set; }
    public double VarianceKwh { get; set; }
    public double VariancePercent { get; set; }
    public double EstimatedDollarBaseline { get; set; }
    public double EstimatedDollarSmartBill { get; set; }
    public double FinancialVarianceAmount { get; set; }
    public string WeatherIndicator { get; set; } = "Severe Cooling Demand (+8.4°F vs Normal)";
    public string RiskLevel { get; set; } = "High"; // "High", "Medium", "Low"
    public string PotentialErrorDirection { get; set; } = "Underestimation Risk"; // Underestimation Risk, Overestimation Risk, Neutral
    public string ConfidenceRange { get; set; } = "± 4.8% (522 – 574 kWh)";
    public string ReviewStatus { get; set; } = "Pending Review"; // "Pending Review", "Accepted", "Flagged for Inspection"
    public double BaseCoolingSensitivityKwhPerCdd { get; set; } = 1.25;
    public double BaseLoadDailyKwh { get; set; } = 10.5;
    public List<MeterMonthlyReading> HistoricalReadings { get; set; } = new();
}

public class EstimatorDashboardDto
{
    public int TotalMissingReadings { get; set; } = 16;
    public int RequiringEstimatedBill { get; set; } = 12;
    public int AwaitingReview { get; set; } = 8;
    public int HighRiskCases { get; set; } = 4;
    public int UnderestimationFlags { get; set; } = 9;
    public int OverestimationFlags { get; set; } = 3;
    public List<MeterExceptionRecord> Records { get; set; } = new();
}

public class EstimationFactorExplanation
{
    public string Factor { get; set; } = string.Empty;
    public string ContributionKwh { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EvidenceLevel { get; set; } = "Strong";
}

public class MeterEstimateDetailDto
{
    public MeterExceptionRecord Record { get; set; } = new();
    public WeatherMetricDto WeatherDetails { get; set; } = new();
    public string BaselineMethodDescription { get; set; } = "3-Month Rolling Average Normalized by Billing Days: (P1 + P2 + P3) / TotalDays * 31";
    public string SmartBillAiFormula { get; set; } = "SmartBill(t) = BaseLoad(daily) * Days + (Beta_CDD * CDD_target * Days) + YoY_Seasonal_Delta";
    public List<EstimationFactorExplanation> ExplanationFactors { get; set; } = new();
    public string OperationalRecommendation { get; set; } = "Accept SmartBill AI Estimate: Prevents severe underestimation during heatwave.";
    public double ConfidenceLowerKwh { get; set; }
    public double ConfidenceUpperKwh { get; set; }
}

public class ReviewActionRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public string Action { get; set; } = "Accept"; // "Accept", "Flag", "Reset"
    public string? Notes { get; set; }
}

public class ReviewActionResponse
{
    public string CustomerId { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class BacktestMonthResult
{
    public string MonthLabel { get; set; } = string.Empty;
    public double ActualKwh { get; set; }
    public double BaselineEstimateKwh { get; set; }
    public double SmartBillAiEstimateKwh { get; set; }
    public double BaselineErrorKwh { get; set; }
    public double SmartBillErrorKwh { get; set; }
    public double BaselineAbsPercentError { get; set; }
    public double SmartBillAbsPercentError { get; set; }
}

public class BacktestEvaluationResultDto
{
    public string EvaluationScope { get; set; } = "Simulated 6-Month Chronological Out-of-Sample Backtest";
    public double BaselineMaeKwh { get; set; }
    public double SmartBillMaeKwh { get; set; }
    public double BaselineMapePercent { get; set; }
    public double SmartBillMapePercent { get; set; }
    public double TotalBaselineAbsErrorKwh { get; set; }
    public double TotalSmartBillAbsErrorKwh { get; set; }
    public double AccuracyImprovementPercent { get; set; }
    public List<BacktestMonthResult> MonthlyBreakdown { get; set; } = new();
    public string ExecutiveSummary { get; set; } = string.Empty;
}
