using BGReportMetrics.API.Models;

namespace BGReportMetrics.API.Data;

public static class DbInitializer
{
    private static readonly string[] PgxBioCodes   = ["PGX", "BIO"];
    private static readonly string[] AllTestCodes  = ["WGS", "WES", "HC", "GA", "PRESEEK", "CYTO", "PGX", "BIO"];
    private static readonly string[] NumericCodes  = ["1900", "1897", "60140", "60145", "61200", "64000", "64005", "24001"];
    private static readonly string[] BillTypes     = ["STD", "INS", "REF", "CAN", "PNL"];
    private static readonly string[] UnlockReasons = ["Transcription Error", "New Clinical Info", "QC Failure", "Physician Request", "Lab Error", "Variant Reclassification", "Sample Mix-up"];
    private static readonly string[] UnlockTypes   = ["Amended", "Corrected", "Addendum"];

    private static readonly Dictionary<string, int> TatByCode = new()
    {
        ["WGS"] = 14, ["WES"] = 14, ["HC"] = 21, ["GA"] = 21,
        ["PRESEEK"] = 28, ["CYTO"] = 14, ["PGX"] = 7, ["BIO"] = 21,
        ["1900"] = 14, ["1897"] = 14, ["60140"] = 21, ["60145"] = 21,
        ["61200"] = 14, ["64000"] = 28, ["64005"] = 28, ["24001"] = 14,
    };

    // ── MGL database seed ────────────────────────────────────────────────
    public static async Task SeedMglAsync(MglDbContext ctx)
    {
        if (ctx.Samples.Any()) return;

        var rng = new Random(42);

        // AllTestCodes (TAT lookup)
        var allCodes = TatByCode.Select((kvp, i) =>
            new AllTestCode { Id = i + 1, TestCode = kvp.Key, TAT = kvp.Value }).ToList();
        ctx.AllTestCodes.AddRange(allCodes);

        // ClientTestCodes (client TAT overrides)
        var clientCodes = new List<ClientTestCode>
        {
            new() { Id = 1, ClientName = "Natera", TAT = 10 },
            new() { Id = 2, ClientName = "Quest",  TAT = 18 },
            new() { Id = 3, ClientName = "GH",     TAT = 25 },
        };
        ctx.ClientTestCodes.AddRange(clientCodes);

        int sampleId = 1;
        var samples         = new List<Sample>();
        var additionalInfos = new List<SamplesAdditionalInfo>();
        var archiveLogs     = new List<ArchiveReportLog>();
        var mglUnlocks      = new List<ReportUnlock>();

        foreach (var (start, end, count) in new[]
        {
            (new DateTime(2026, 3, 1), new DateTime(2026, 5, 31), 900),
            (new DateTime(2026, 1, 1), new DateTime(2026, 2, 28), 100)
        })
        {
            for (int i = 0; i < count; i++, sampleId++)
            {
                var testCode = rng.NextDouble() < 0.15
                    ? NumericCodes[rng.Next(NumericCodes.Length)]
                    : AllTestCodes[rng.Next(AllTestCodes.Length)];

                var billType = rng.NextDouble() < 0.08 ? "CAN"
                    : rng.NextDouble() < 0.05 ? "PNL"
                    : BillTypes[rng.Next(3)];

                var faxedDate       = RandomDate(rng, start, end);
                var hasOriginalFax  = rng.NextDouble() < 0.12;
                var originalFaxDate = hasOriginalFax ? faxedDate.AddDays(-rng.Next(1, 30)) : (DateTime?)null;
                var isCancelled     = billType == "CAN";

                var orderOffset   = rng.Next(5, 46);
                var testOrderDate = faxedDate.AddDays(-orderOffset);
                if (testOrderDate < new DateTime(2026, 1, 1))
                    testOrderDate = new DateTime(2026, 1, 1).AddDays(rng.Next(1, 10));

                int? clientTestCodeId = rng.NextDouble() < 0.25
                    ? clientCodes[rng.Next(clientCodes.Count)].Id
                    : null;

                samples.Add(new Sample
                {
                    Id               = sampleId,
                    LabNumber        = $"LAB{sampleId:D6}",
                    TestCode         = testCode,
                    FaxedDate        = faxedDate,
                    OriginalFaxDate  = originalFaxDate,
                    BillType         = billType,
                    ManualClose      = rng.NextDouble() < 0.05 ? 1 : 0,
                    Interpretation   = isCancelled && rng.NextDouble() < 0.4
                        ? "Sample failed quality threshold during processing" : null,
                    TestOrderDate    = testOrderDate,
                    ClientTestCodeId = clientTestCodeId
                });

                if (!isCancelled)
                    archiveLogs.Add(new ArchiveReportLog { Id = sampleId, SamplesId = sampleId });

                int effectiveTat = clientTestCodeId.HasValue
                    ? clientCodes.First(c => c.Id == clientTestCodeId).TAT
                    : (TatByCode.TryGetValue(testCode, out var t) ? t : 14);

                additionalInfos.Add(new SamplesAdditionalInfo
                {
                    Id               = sampleId,
                    SampleId         = sampleId,
                    WasAutoSignedOut = (!isCancelled && rng.NextDouble() < 0.35) ? 1 : 0,
                    UpgradedOrder    = rng.NextDouble() < 0.08 ? 1 : 0,
                    DueDate          = rng.NextDouble() < 0.2
                        ? testOrderDate.AddDays(effectiveTat + rng.Next(-2, 3))
                        : null
                });

                // Amended/corrected — non-PGx/BIO only
                if (!isCancelled && hasOriginalFax && !PgxBioCodes.Contains(testCode) && rng.NextDouble() < 0.10)
                    for (int u = 0; u < rng.Next(1, 3); u++)
                        mglUnlocks.Add(new ReportUnlock
                        {
                            SampleId         = sampleId,
                            Reason           = UnlockReasons[rng.Next(UnlockReasons.Length)],
                            ReportUnlockType = UnlockTypes[rng.Next(UnlockTypes.Length)]
                        });
            }
        }

        ctx.Samples.AddRange(samples);
        ctx.ArchiveReportLog.AddRange(archiveLogs);
        ctx.SamplesAdditionalInfo.AddRange(additionalInfos);
        ctx.ReportUnlocks.AddRange(mglUnlocks);

        await ctx.SaveChangesAsync();
    }

    // ── LIMS database seed ───────────────────────────────────────────────
    public static async Task SeedLimsAsync(LimsDbContext ctx)
    {
        if (ctx.LimsReports.Any()) return;

        var rng = new Random(42);

        var panelCodes = new List<PanelTestCode>
        {
            new() { Id = 1, Code = "PGX", Name = "Pharmacogenomics" },
            new() { Id = 2, Code = "BIO", Name = "Biochemical" }
        };
        ctx.PanelTestCodes.AddRange(panelCodes);

        var limsOrders   = new List<LimsOrder>();
        var limsReports  = new List<LimsReport>();
        var limsUnlocks  = new List<LimsReportUnlock>();
        int limsUnlockId = 1;

        for (int i = 1; i <= 150; i++)
        {
            var panelCode = panelCodes[rng.Next(panelCodes.Count)];
            limsOrders.Add(new LimsOrder
            {
                Id              = i,
                OldLabNumber    = $"PGX{i:D5}",
                PanelTestCodeId = panelCode.Id
            });

            var reportedDate = RandomDate(rng, new DateTime(2026, 3, 1), new DateTime(2026, 5, 31));
            var isAuto       = rng.NextDouble() < 0.4;

            limsReports.Add(new LimsReport
            {
                Id           = i,
                OrderId      = i,
                ReportedBy   = isAuto ? "PGxAutoSignout" : $"user{rng.Next(1, 20)}@lab.com",
                ReportedDate = reportedDate
            });

            if (!isAuto && rng.NextDouble() < 0.15)
                limsUnlocks.Add(new LimsReportUnlock
                {
                    Id               = limsUnlockId++,
                    ReportId         = i,
                    ReportUnlockType = UnlockTypes[rng.Next(UnlockTypes.Length)],
                    Reason           = UnlockReasons[rng.Next(UnlockReasons.Length)],
                    IsOnReport       = 1
                });
        }

        ctx.LimsOrders.AddRange(limsOrders);
        ctx.LimsReports.AddRange(limsReports);
        ctx.LimsReportUnlocks.AddRange(limsUnlocks);

        await ctx.SaveChangesAsync();
    }

    private static DateTime RandomDate(Random rng, DateTime start, DateTime end) =>
        start.AddDays(rng.Next((end - start).Days + 1));
}
