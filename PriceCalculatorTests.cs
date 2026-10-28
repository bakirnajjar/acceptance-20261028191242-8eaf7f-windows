using System;
using System.Collections.Generic;
using Xunit;

namespace Acceptance.Pricing
{
    public class PriceCalculatorTests
    {
        [Fact]
        public void SubtotalMultipliesQuantityByUnitPrice()
        {
            var items = new List<LineItem> { new LineItem("A", 2, 150), new LineItem("B", 1, 999) };
            Assert.Equal(1299, PriceCalculator.SubtotalCents(items));
        }

        [Theory]
        [InlineData(" spring10 ", 10)]
        [InlineData("LOYAL5", 5)]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        [InlineData("UNKNOWN", 0)]
        public void DiscountCodesIgnoreCaseAndUnknownCodesGiveNothing(string code, int expected)
        {
            Assert.Equal(expected, PriceCalculator.DiscountPercent(code));
        }

        [Fact]
        public void DiscountRoundsHalfUp()
        {
            var result = PriceCalculator.Total(new List<LineItem> { new LineItem("A", 1, 105) }, "SPRING10");
            Assert.Equal(11, result.Item1);
            Assert.Equal(94, result.Item2);
        }

        [Fact]
        public void ReceiptReportsCountDiscountAndTotal()
        {
            var items = new List<LineItem> { new LineItem("A", 2, 1000), new LineItem("B", 3, 250) };
            Assert.Equal("5 item(s), discount 1.38, total 26.12", PriceCalculator.Receipt(items, "LOYAL5"));
        }

        [Fact]
        public void EmptyReceiptReportsZeroCountDiscountAndTotal()
        {
            Assert.Equal("0 item(s), discount 0.00, total 0.00", PriceCalculator.Receipt(new List<LineItem>(), "LOYAL5"));
        }

        [Fact]
        public void InvalidLinesAreRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LineItem("A", 0, 100));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LineItem("A", 1, -1));
        }
    }
}
