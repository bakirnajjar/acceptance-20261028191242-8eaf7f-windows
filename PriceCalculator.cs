using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Acceptance.Pricing
{
    /// <summary>One order line; prices are integer cents. Synthetic acceptance sample.</summary>
    public class LineItem
    {
        public LineItem(string sku, int quantity, int unitCents)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException("quantity");
            if (unitCents < 0) throw new ArgumentOutOfRangeException("unitCents");
            Sku = sku;
            Quantity = quantity;
            UnitCents = unitCents;
        }

        public string Sku { get; private set; }
        public int Quantity { get; private set; }
        public int UnitCents { get; private set; }
    }

    public static class PriceCalculator
    {
        private static readonly Dictionary<string, int> Discounts = new Dictionary<string, int>
        {
            { "SPRING10", 10 },
            { "LOYAL5", 5 }
        };

        public static int SubtotalCents(IEnumerable<LineItem> items)
        {
            int total = 0;
            foreach (LineItem item in items)
            {
                total += item.Quantity * item.UnitCents;
            }
            return total;
        }

        public static int DiscountPercent(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return 0;
            int percent;
            return Discounts.TryGetValue(code.Trim().ToUpperInvariant(), out percent) ? percent : 0;
        }

        public static Tuple<int, int> Total(IList<LineItem> items, string code)
        {
            int subtotal = SubtotalCents(items);
            int discount = (subtotal * DiscountPercent(code) + 50) / 100;
            return Tuple.Create(discount, subtotal - discount);
        }

        public static string Receipt(IList<LineItem> items, string code)
        {
            Tuple<int, int> result = Total(items, code);
            int count = items.Sum(item => item.Quantity);
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} item(s), discount {1}.{2:00}, total {3}.{4:00}",
                count,
                result.Item1 / 100,
                result.Item1 % 100,
                result.Item2 / 100,
                result.Item2 % 100);
        }
    }
}
