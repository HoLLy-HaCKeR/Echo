using System;
using Echo.Concrete.Values.ValueType;
using Xunit;

namespace Echo.Concrete.Tests.Values.ValueType
{
    public class Integer8Tests
    {
        [Fact]
        public void KnownValueSignedUnsignedSame()
        {
            var value = new Integer8Value(0x12);

            Assert.True(value.IsKnown);
            Assert.Equal(Integer8Value.FullyKnownMask, value.Mask);
            Assert.Equal(0x12, value.U8);
            Assert.Equal(0x12, value.I8);
        }
        
        [Fact]
        public void KnownValueSignedUnsignedDifferent()
        {
            var value = new Integer8Value(0x80);

            Assert.True(value.IsKnown);
            Assert.Equal(Integer8Value.FullyKnownMask, value.Mask);
            Assert.Equal(0x80, value.U8);
            Assert.Equal(-0x80, value.I8);
        }

        [Fact]
        public void PartiallyUnknownValue()
        {
            var value = new Integer8Value(
                0b00110011, 
                0b11101101);
            
            Assert.False(value.IsKnown);
            Assert.Equal(0b00100001, value.U8);
        }

        [Fact]
        public void LittleEndianBitOrder()
        {
            var value = new Integer8Value(0b0001_0010);

            var bits = new bool[8];
            value.GetBits().CopyTo(bits, 0);
            Assert.Equal(new[]
            {
                false, true, false, false, true, false, false, false
            }, bits);
        }

        [Fact]
        public void ParseFullyKnownBitString()
        {
            var value = new Integer8Value("00001100");
            Assert.Equal(0b00001100, value.U8);
        }

        [Fact]
        public void ParsePartiallyKnownBitString()
        {
            var value = new Integer8Value("000011??");
            Assert.Equal(0b11111100, value.Mask);
            Assert.Equal(0b00001100, value.U8 & value.Mask);
        }

        [Fact]
        public void ParseFewerBits()
        {
            var value = new Integer8Value("101");
            Assert.Equal(0b101, value.U8);
        }

        [Fact]
        public void ParseWithMoreZeroes()
        {
            var value = new Integer8Value("0000000000000101");
            Assert.Equal(0b101, value.U8);
        }

        [Fact]
        public void ParseWithOverflow()
        {
            Assert.Throws<OverflowException>(() => new Integer8Value("10000000000000101"));
        }

        [Theory]
        [InlineData("00000000", "11111111")]
        [InlineData("11111111", "00000000")]
        [InlineData("????????", "????????")]
        [InlineData("0011??00", "1100??00")]
        public void Not(string input, string expected)
        {
            var value1 = new Integer8Value(input);
            
            value1.Not();
            
            Assert.Equal(new Integer8Value(expected), value1);
        }

        [Theory]
        [InlineData("00110101", "11101111", "00110101")]
        [InlineData("00000000", "0000000?", "0000000?")]
        [InlineData("0000000?", "0000000?", "0000000?")]
        public void And(string a, string b, string expected)
        {
            var value1 = new Integer8Value(a);
            var value2 = new Integer8Value(b);
            
            value1.And(value2);
            
            Assert.Equal(new Integer8Value(expected), value1);
        }

        [Theory]
        [InlineData("00110101", "11101111", "11111111")]
        [InlineData("00000000", "0000000?", "0000000?")]
        [InlineData("0000000?", "0000000?", "0000000?")]
        [InlineData("0010000?", "0001000?", "0011000?")]
        public void Or(string a, string b, string expected)
        {
            var value1 = new Integer8Value(a);
            var value2 = new Integer8Value(b);
            
            value1.Or(value2);
            
            Assert.Equal(new Integer8Value(expected), value1);
        }

        [Theory]
        [InlineData("00110101", "11101111", "11111111")]
        [InlineData("00000000", "0000000?", "0000000?")]
        [InlineData("0000000?", "0000000?", "0000000?")]
        [InlineData("0010000?", "0011000?", "0001000?")]
        public void Xor(string a, string b, string expected)
        {
            var value1 = new Integer8Value(a);
            var value2 = new Integer8Value(b);
            
            value1.Or(value2);
            
            Assert.Equal(new Integer8Value(expected), value1);
        }

        [Theory]
        [InlineData("00010010", "00110100", "01000110")]
        [InlineData("00000000", "0000000?", "0000000?")]
        [InlineData("00000001", "0000000?", "000000??")]
        [InlineData("0000000?", "00000001", "000000??")]
        [InlineData("0000000?", "0000000?", "000000??")]
        [InlineData("0000??11", "00000001", "000?????")]
        [InlineData("000??0??", "00000101", "00??????")]
        public void Add(string a, string b, string expected)
        {
            var value1 = new Integer8Value(a);
            var value2 = new Integer8Value(b);

            value1.Add(value2);
            
            Assert.Equal(new Integer8Value(expected), value1);
        }
    }
}