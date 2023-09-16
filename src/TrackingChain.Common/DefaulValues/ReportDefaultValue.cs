namespace TrackingChain.Common.DefaulValues
{
    public static class ReportDefaultValue
    {
        public const string TransactionErrorTemplate = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Transaction Error Report</title>
</head>
<body>
    <h2>Transaction Error Report</h2>
    <table style=""width: 80%; border-collapse: collapse;"">
        <thead style=""background-color: #f2f2f2;"">
            <tr>
                <th style=""padding: 10px; border: 1px solid #ddd;"">Tracking Id</th>
                <th style=""padding: 10px; border: 1px solid #ddd;"">Date</th>
                <th style=""padding: 10px; border: 1px solid #ddd;"">Description</th>
            </tr>
        </thead>
        <tbody>
            [TR_PLACEHOLDER]
        </tbody>
    </table>
</body>
</html>";

        public const string TransactionCancelledTemplate = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Transaction Cancelled Report</title>
</head>
<body>
    <h2>Transaction Cancelled Report</h2>
    <table style=""width: 80%; border-collapse: collapse;"">
        <thead style=""background-color: #f2f2f2;"">
            <tr>
                <th style=""padding: 10px; border: 1px solid #ddd;"">Tracking Id</th>
                <th style=""padding: 10px; border: 1px solid #ddd;"">Date</th>
                <th style=""padding: 10px; border: 1px solid #ddd;"">Description</th>
            </tr>
        </thead>
        <tbody>
            [TR_PLACEHOLDER]
        </tbody>
    </table>
</body>
</html>
";

    }
}
