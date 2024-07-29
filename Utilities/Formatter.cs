using System.Globalization;

namespace Plata.Utilities
{
    public static class Formatter
    {
        public static string FormatDecimalForInput(decimal value)
        {
            if (value == decimal.Truncate(value)) return value.ToString("F0", CultureInfo.InvariantCulture);
            else return value.ToString("F2", CultureInfo.InvariantCulture);
        }

        public static string FormatDecimalForText(decimal value)
        {
            if (value == decimal.Truncate(value)) return value.ToString("N0");
            else return value.ToString("N2");
        }

        public static string FormatRateForText(decimal value)
        {
            return $"{FormatDecimalForText(value * 100)}%";
        }

        public static string FormatMoneyForText(decimal value, string currencySymbol)
        {
            return $"{FormatDecimalForText(Math.Round(value, 2))} {currencySymbol}";
        }
    }
}
