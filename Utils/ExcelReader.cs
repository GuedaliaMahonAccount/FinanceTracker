using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using OfficeOpenXml;

namespace Finance.Utils
{
    public static class ExcelReader
    {
        public static Dictionary<(string fromCurrency, string toCurrency, DateTime date), decimal> ReadExchangeRates(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Excel file not found at path: {filePath}");
            }

            var exchangeRates = new Dictionary<(string fromCurrency, string toCurrency, DateTime date), decimal>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var date = DateTime.Parse(worksheet.Cells[row, 1].Text);
                        var fromCurrency = worksheet.Cells[row, 2].Text;
                        var toCurrency = worksheet.Cells[row, 3].Text;

                        // Lire le taux de change en supportant ? la fois virgule et point
                        string rateText = worksheet.Cells[row, 4].Text.Trim();
                        
                        // Remplacer la virgule par un point pour le parsing
                        rateText = rateText.Replace(",", ".");
                        
                        if (!decimal.TryParse(rateText, NumberStyles.Any, CultureInfo.InvariantCulture, out var rate))
                        {
                            throw new FormatException($"Invalid rate format in row {row}: '{worksheet.Cells[row, 4].Text}'");
                        }

                        exchangeRates[(fromCurrency, toCurrency, date)] = rate;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidDataException($"Error processing row {row}: {ex.Message}", ex);
                    }
                }
            }

            return exchangeRates;
        }
    }
}