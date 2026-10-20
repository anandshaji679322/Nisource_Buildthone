using SmartBillAi.Api.Models;

namespace SmartBillAi.Api.Services;

public class BillAdvisorService : IBillAdvisorService
{
    private readonly CustomerProfileDto _demoCustomer;

    public BillAdvisorService()
    {
        _demoCustomer = InitializeDemoCustomer();
    }

    public CustomerProfileDto GetCustomerProfile(string accountNumber)
    {
        return _demoCustomer;
    }

    public CustomerDashboardSummaryDto GetDashboardSummary(string accountNumber, string? selectedPeriodId = null)
    {
        var history = _demoCustomer.BillingHistory.OrderByDescending(x => x.PeriodId).ToList();
        var selected = string.IsNullOrEmpty(selectedPeriodId)
            ? history.FirstOrDefault() ?? new BillPeriodRecord()
            : history.FirstOrDefault(x => x.PeriodId == selectedPeriodId) ?? history.First();

        var selectedIndex = history.IndexOf(selected);
        var previous = (selectedIndex + 1 < history.Count)
            ? history[selectedIndex + 1]
            : (history.Count > 1 ? history[1] : selected);

        var billDelta = Math.Round(selected.TotalBillAmount - previous.TotalBillAmount, 2);
        var billPercent = previous.TotalBillAmount > 0
            ? Math.Round((billDelta / previous.TotalBillAmount) * 100.0, 1)
            : 0;

        var kwhDelta = Math.Round(selected.ConsumptionKwh - previous.ConsumptionKwh, 1);
        var kwhPercent = previous.ConsumptionKwh > 0
            ? Math.Round((kwhDelta / previous.ConsumptionKwh) * 100.0, 1)
            : 0;

        var dailyDelta = Math.Round(selected.AvgDailyKwh - previous.AvgDailyKwh, 2);
        var daysDelta = selected.BillingDays - previous.BillingDays;

        return new CustomerDashboardSummaryDto
        {
            Profile = _demoCustomer,
            SelectedBill = selected,
            PreviousBill = previous,
            BillAmountDelta = billDelta,
            BillAmountPercentChange = billPercent,
            ConsumptionDeltaKwh = kwhDelta,
            ConsumptionPercentChange = kwhPercent,
            AvgDailyDeltaKwh = dailyDelta,
            BillingDaysDelta = daysDelta,
            ExplanationStatus = "Ready"
        };
    }

    public BillComparisonResponse CompareBills(BillComparisonRequest request)
    {
        var periodA = _demoCustomer.BillingHistory.FirstOrDefault(x => x.PeriodId == request.PeriodIdA)
            ?? _demoCustomer.BillingHistory.Last();
        var periodB = _demoCustomer.BillingHistory.FirstOrDefault(x => x.PeriodId == request.PeriodIdB)
            ?? _demoCustomer.BillingHistory.First();

        var billDelta = Math.Round(periodA.TotalBillAmount - periodB.TotalBillAmount, 2);
        var billPercent = periodB.TotalBillAmount > 0 ? Math.Round((billDelta / periodB.TotalBillAmount) * 100.0, 1) : 0;
        var kwhDelta = Math.Round(periodA.ConsumptionKwh - periodB.ConsumptionKwh, 1);
        var kwhPercent = periodB.ConsumptionKwh > 0 ? Math.Round((kwhDelta / periodB.ConsumptionKwh) * 100.0, 1) : 0;
        var daysDelta = periodA.BillingDays - periodB.BillingDays;
        var dailyDelta = Math.Round(periodA.AvgDailyKwh - periodB.AvgDailyKwh, 2);
        var cddDelta = Math.Round(periodA.CoolingDegreeDays - periodB.CoolingDegreeDays, 1);
        var tariffDelta = Math.Round((periodA.RatePerKwh - periodB.RatePerKwh) * periodA.ConsumptionKwh, 2);

        var summary = $"Comparing {periodA.MonthName} with {periodB.MonthName}: Total bill is {(billDelta >= 0 ? "$" + billDelta.ToString("F2") + " higher" : "$" + Math.Abs(billDelta).ToString("F2") + " lower")} ({(billPercent >= 0 ? "+" : "")}{billPercent}%). Consumption varied by {(kwhDelta >= 0 ? "+" : "")}{kwhDelta:F1} kWh with an average daily difference of {(dailyDelta >= 0 ? "+" : "")}{dailyDelta:F2} kWh/day across {Math.Abs(daysDelta)} {(daysDelta >= 0 ? "more" : "fewer")} billing days.";

        return new BillComparisonResponse
        {
            PeriodA = periodA,
            PeriodB = periodB,
            BillDeltaAmount = billDelta,
            BillDeltaPercent = billPercent,
            ConsumptionDeltaKwh = kwhDelta,
            ConsumptionDeltaPercent = kwhPercent,
            DaysDelta = daysDelta,
            AvgDailyDeltaKwh = dailyDelta,
            CoolingDegreeDaysDelta = cddDelta,
            TariffDifferenceAmount = tariffDelta,
            ComparisonSummary = summary
        };
    }

    public List<EnergyRecommendationItem> GetPersonalizedRecommendations(string accountNumber, string periodId)
    {
        var bill = _demoCustomer.BillingHistory.FirstOrDefault(x => x.PeriodId == periodId)
            ?? _demoCustomer.BillingHistory.First();

        var list = new List<EnergyRecommendationItem>();

        if (bill.CoolingDegreeDays > 100)
        {
            list.Add(new EnergyRecommendationItem
            {
                Category = "Cooling",
                Title = "Optimize AC Smart Thermostat Scheduling",
                Action = "Program your thermostat to 75°F during 1:00 PM – 7:00 PM peak periods and 72°F overnight.",
                Rationale = $"Cooling degree days were {bill.CoolingDegreeDays:F0} this period. Each 1°F temperature adjustment reduces air conditioning compressor electrical consumption by 3–5%.",
                EstimatedMonthlySavings = 22.40,
                Feasibility = "Easy"
            });
        }
        else if (bill.HeatingDegreeDays > 400)
        {
            list.Add(new EnergyRecommendationItem
            {
                Category = "Heating",
                Title = "Lower Heating Setpoint to 68°F During Sleeping Hours",
                Action = "Set heating system to 68°F during waking hours and 62°F while sleeping or away.",
                Rationale = $"Heating degree days reached {bill.HeatingDegreeDays:F0}. Lowering your thermostat 7°–10°F for 8 hours a day can save up to 10% a year on heating.",
                EstimatedMonthlySavings = 28.00,
                Feasibility = "Easy"
            });
        }

        list.Add(new EnergyRecommendationItem
        {
            Category = "Appliance",
            Title = "Switch Laundry & Dishwashing to Off-Peak Windows",
            Action = "Run high-wattage electric resistance appliances (clothes dryer, dishwasher) after 8:00 PM.",
            Rationale = "Operating heat-generating appliances during hot afternoons increases indoor cooling loads, forcing your A/C to cycle 18% longer.",
            EstimatedMonthlySavings = 11.50,
            Feasibility = "Easy"
        });

        list.Add(new EnergyRecommendationItem
        {
            Category = "Baseline",
            Title = "Eliminate Phantom Electronics Standby Draw",
            Action = "Use smart power strips for entertainment centers and home office computer equipment.",
            Rationale = "Continuous standby loads (vampire draw) account for approximately 45 kWh/month of your baseline electricity usage.",
            EstimatedMonthlySavings = 8.10,
            Feasibility = "Moderate"
        });

        return list;
    }

    private static CustomerProfileDto InitializeDemoCustomer()
    {
        var profile = new CustomerProfileDto
        {
            AccountNumber = "ACT-7492-EAST",
            CustomerName = "Eleanor Vance",
            ServiceAddress = "4420 Sycamore Crest Way, Columbus, OH 43215",
            MeterId = "MTR-OH-9021",
            RatePlan = "Residential Standard Electric (RS-1)",
            IsSimulatedData = true
        };

        // 12 Months of realistic billing data (Aug 2025 to Jul 2026)
        var monthsConfig = new[]
        {
            // Id, Month, Days, kWh, Rate, Temp, CDD, HDD, Status
            ("2026-07", "July 2026", 31, 742.0, 0.158, 79.2, 440.0, 0.0, "Current"),
            ("2026-06", "June 2026", 30, 485.0, 0.154, 73.5, 255.0, 0.0, "Paid"),
            ("2026-05", "May 2026", 31, 332.0, 0.149, 64.2, 85.0, 110.0, "Paid"),
            ("2026-04", "April 2026", 30, 310.0, 0.149, 53.8, 25.0, 360.0, "Paid"),
            ("2026-03", "March 2026", 31, 385.0, 0.149, 43.1, 5.0, 680.0, "Paid"),
            ("2026-02", "February 2026", 28, 490.0, 0.152, 32.5, 0.0, 910.0, "Paid"),
            ("2026-01", "January 2026", 31, 565.0, 0.152, 28.8, 0.0, 1120.0, "Paid"),
            ("2025-12", "December 2025", 31, 520.0, 0.150, 33.2, 0.0, 985.0, "Paid"),
            ("2025-11", "November 2025", 30, 395.0, 0.148, 44.5, 0.0, 615.0, "Paid"),
            ("2025-10", "October 2025", 31, 325.0, 0.148, 55.4, 20.0, 318.0, "Paid"),
            ("2025-09", "September 2025", 30, 430.0, 0.152, 68.8, 140.0, 45.0, "Paid"),
            ("2025-08", "August 2025", 31, 685.0, 0.156, 76.8, 365.0, 0.0, "Paid")
        };

        var history = new List<BillPeriodRecord>();
        var random = new Random(42);

        foreach (var m in monthsConfig)
        {
            var fixedFee = 14.50;
            var energyCharge = Math.Round(m.Item4 * m.Item5, 2);
            var taxes = Math.Round((energyCharge + fixedFee) * 0.0725, 2); // 7.25% Ohio sales & municipal excise tax
            var totalBill = Math.Round(energyCharge + fixedFee + taxes, 2);
            var avgDaily = Math.Round(m.Item4 / m.Item3, 2);

            // Generate realistic daily usage points for selected month
            var dailyPoints = new List<DailyUsagePoint>();
            var baseDaily = m.Item4 / m.Item3;
            for (int d = 1; d <= m.Item3; d++)
            {
                var isWeekend = (d % 7 == 0 || d % 7 == 6);
                var weekendFactor = isWeekend ? 1.15 : 0.95;
                var randomNoise = 0.90 + (random.NextDouble() * 0.20);
                var dayKwh = Math.Round(baseDaily * weekendFactor * randomNoise, 1);
                var dayTemp = Math.Round(m.Item6 + (random.NextDouble() * 8.0 - 4.0), 1);

                dailyPoints.Add(new DailyUsagePoint
                {
                    Date = $"{m.Item1}-{d:D2}",
                    DayNumber = d,
                    Kwh = dayKwh,
                    AvgTempF = dayTemp,
                    IsWeekend = isWeekend
                });
            }

            history.Add(new BillPeriodRecord
            {
                PeriodId = m.Item1,
                MonthName = m.Item2,
                StartDate = $"{m.Item1}-01",
                EndDate = $"{m.Item1}-{m.Item3:D2}",
                BillingDays = m.Item3,
                ConsumptionKwh = m.Item4,
                TotalBillAmount = totalBill,
                EnergyCharges = energyCharge,
                FixedCharges = fixedFee,
                TaxesAndFees = taxes,
                RatePerKwh = m.Item5,
                AvgDailyKwh = avgDaily,
                AvgTemperatureF = m.Item6,
                CoolingDegreeDays = m.Item7,
                HeatingDegreeDays = m.Item8,
                Status = m.Item9,
                DailyUsage = dailyPoints
            });
        }

        profile.BillingHistory = history;
        return profile;
    }
}
