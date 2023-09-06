namespace TrackingChain.Common.DefaulValues
{
    public static class ReportDefaultValue
    {
        public const string TransactionErrorTemplate = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Email Content</title>
</head>
<body>
    <table style=""width: 100%; border-collapse: collapse;"">
        <thead style=""background-color: #f2f2f2;"">
            <tr>
                <th style=""padding: 10px; border: 1px solid #ddd;"">ID</th>
                <th style=""padding: 10px; border: 1px solid #ddd;"">Date</th>
                <th style=""padding: 10px; border: 1px solid #ddd;"">Field 1</th>
                <th style=""padding: 10px; border: 1px solid #ddd;"">Field 2</th>
            </tr>
        </thead>
        <tbody>
            [TR_PLACEHOLDER]
        </tbody>
    </table>
</body>
</html>";

        public const string TransactionErrorTitle = @"Transaction Error Report";

        public const string TransactionFailedTemplate = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Email Content</title>
</head>
<body>
    <table style=""width: 100%; border-collapse: collapse;"">
        <thead style=""background-color: #f2f2f2;"">
            <tr>
                <th style=""padding: 10px; border: 1px solid #ddd;"">ID</th>
                <th style=""padding: 10px; border: 1px solid #ddd;"">Date</th>
                <th style=""padding: 10px; border: 1px solid #ddd;"">Field 1</th>
                <th style=""padding: 10px; border: 1px solid #ddd;"">Field 2</th>
            </tr>
        </thead>
        <tbody>
            [TR_PLACEHOLDER]
        </tbody>
    </table>
</body>
</html>
";

        public const string TransactionFailedTitle = @"Transaction Failed Report";

    }
}
