using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode2021Day16 {
    public abstract class BITSPacket {
        public const int VersionLength = 3;
        public const int TypeIDLength = 3;
        public enum TypeID {
            Sum = 0,
            Product = 1,
            Minimum = 2,
            Maximum = 3,
            LiteralNumber = 4,
            GreaterThan = 5,
            LessThan = 6,
            EqualTo = 7
        }

        public long versionNumber;
        public TypeID typeID;

        public BITSPacket(long thisVersionNumber) {
            versionNumber = thisVersionNumber;
        }

        public abstract string DecodePacketContents(string binary);

        public static string DecodePacketHeader(string binary, out long thisVersionNumber, out TypeID thisTypeID) {
            int placeIndex = 0;

            thisVersionNumber = BinaryToDecimal(binary.Substring(placeIndex, VersionLength));
            placeIndex += VersionLength;

            thisTypeID = (TypeID)BinaryToDecimal(binary.Substring(placeIndex, TypeIDLength));
            placeIndex += TypeIDLength;

            return placeIndex < binary.Length ? binary.Substring(placeIndex) : "";
        }

        public static long BinaryToDecimal (string binaryNumber) {
            long exponent = 0;
            long decimalNumber = 0;

            for (int i = binaryNumber.Length - 1; i >= 0; i--) {
                decimalNumber = Int32.Parse(binaryNumber.Substring(i, 1)) == 1 ? decimalNumber + (long)Math.Pow(2, exponent) : decimalNumber;
                exponent++;
            }

            return decimalNumber;
        }

        public virtual long TotalVersionNumber() {
            return versionNumber;
        }

        public abstract long CalculateValue();
    }

    public class LiteralValuePacket : BITSPacket {
        public const int ContinueOrStopLength = 1;
        public const int Continue = 1;
        public const int Stop = 0;
        public const int SectionLength = 4;

        public long LiteralValue {
            get { return literalValue; }
            set { literalValue = value; }
        }
        private long literalValue;

        public LiteralValuePacket(long thisVersionNumber) : base(thisVersionNumber) {

        }

        public override string DecodePacketContents(string binary) {
            int placeIndex = 0;
            string literalValueBinary = "";
            bool continueReading;

            do {
                continueReading = Int32.Parse(binary.Substring(placeIndex, ContinueOrStopLength)) == Continue;
                placeIndex += ContinueOrStopLength;

                literalValueBinary += binary.Substring(placeIndex, SectionLength);
                placeIndex += SectionLength;
            }
            while (continueReading);

            literalValue = BinaryToDecimal(literalValueBinary);
            return placeIndex < binary.Length ? binary.Substring(placeIndex) : "";
        }

        public override long TotalVersionNumber() {
            return base.TotalVersionNumber();
        }

        public override long CalculateValue() {
            Console.Write(literalValue);
            return literalValue;
        }
    }

    public class OperatorPacket : BITSPacket {
        public const int LengthTypeIDLength = 1;
        public const int TotalLengthInBitsLength = 15;
        public const int NumberOfSubPacketsLength = 11;
        public enum LengthTypeID {
            TotalLengthInBits15 = 0,
            NumberOfSubPackets11 = 1
        }

        public LengthTypeID lengthTypeID;
        public long totalLengthInBits;
        public long numSubPackets;
        List<BITSPacket> subPackets;

        public OperatorPacket(long thisVersionNumber) : base(thisVersionNumber) {
            subPackets = new List<BITSPacket>();
        }

        public override string DecodePacketContents(string binary) {
            binary = DetermineLengthTypeID(binary);

            switch (lengthTypeID) {
                case LengthTypeID.TotalLengthInBits15:
                    binary = DecodeSubPacketsByTotalLength(binary, totalLengthInBits);
                    break;
                case LengthTypeID.NumberOfSubPackets11:
                    binary = DecodeSubPacketsByNumPackets(binary, numSubPackets);
                    break;
            }

            return binary;
        }

        private string DetermineLengthTypeID(string binary) {
            int placeIndex = 0;
            lengthTypeID = (LengthTypeID)Int32.Parse(binary.Substring(placeIndex, LengthTypeIDLength));
            placeIndex += LengthTypeIDLength;

            switch (lengthTypeID) {
                case LengthTypeID.TotalLengthInBits15:
                    totalLengthInBits = BinaryToDecimal(binary.Substring(placeIndex, TotalLengthInBitsLength));
                    placeIndex += TotalLengthInBitsLength;
                    break;
                case LengthTypeID.NumberOfSubPackets11:
                    numSubPackets = BinaryToDecimal(binary.Substring(placeIndex, NumberOfSubPacketsLength));
                    placeIndex += NumberOfSubPacketsLength;
                    break;
            }

            return placeIndex < binary.Length ? binary.Substring(placeIndex) : "";
        }

        private string DecodeSubPacketsByTotalLength(string binary, long totalLengthInBits) {
            long totalBitsUsed = 0;

            while (totalBitsUsed < totalLengthInBits) {
                int preSubPacketBinaryLength = binary.Length;
                binary = DecodeSubPacket(binary);
                totalBitsUsed += preSubPacketBinaryLength - binary.Length;
            }

            return binary;
        }

        private string DecodeSubPacketsByNumPackets(string binary, long totalSubPackets) {
            long numSubPacketsDecoded = 0;

            while (numSubPacketsDecoded < totalSubPackets) {
                binary = DecodeSubPacket(binary);
                numSubPacketsDecoded++;
            }

            return binary;
        }

        private string DecodeSubPacket(string binary) {
            long subPacketVersionNumber;
            TypeID subPacketTypeID;
            binary = DecodePacketHeader(binary, out subPacketVersionNumber, out subPacketTypeID);

            BITSPacket newSubPacket;
            switch (subPacketTypeID) {
                case TypeID.LiteralNumber:
                    newSubPacket = new LiteralValuePacket(subPacketVersionNumber);
                    break;
                default:
                    newSubPacket = new OperatorPacket(subPacketVersionNumber);
                    break;
            }

            newSubPacket.typeID = subPacketTypeID;

            binary = newSubPacket.DecodePacketContents(binary);
            subPackets.Add(newSubPacket);

            return binary;
        }

        public override long TotalVersionNumber() {
            long sum = 0;

            foreach (BITSPacket subPacket in subPackets) {
                sum += subPacket.TotalVersionNumber();
            }

            return versionNumber + sum;
        }

        public override long CalculateValue() {
            switch (typeID) {
                case TypeID.Sum:
                    return Sum();
                case TypeID.Product:
                    return Product();
                case TypeID.Minimum:
                    return Minimum();
                case TypeID.Maximum:
                    return Maximum();
                case TypeID.GreaterThan:
                    return GreaterThan();
                case TypeID.LessThan:
                    return LessThan();
                case TypeID.EqualTo:
                    return EqualTo();
            }

            return -1;
        }

        private long Sum() {
            long sum = 0;

            Console.Write("(");
            foreach (BITSPacket subPacket in subPackets) {
                sum += subPacket.CalculateValue();
                Console.Write(" + ");
            }
            Console.Write(")");

            return sum;
        }

        private long Product() {
            long product = 1;

            Console.Write("(");
            foreach (BITSPacket subPacket in subPackets) {
                product *= subPacket.CalculateValue();
                Console.Write(" * ");
            }
            Console.Write(")");

            return product;
        }

        private long Minimum() {
            long minimum = long.MaxValue;

            Console.Write("Min(");
            foreach (BITSPacket subPacket in subPackets) {
                long subPacketValue = subPacket.CalculateValue();
                Console.Write(", ");
                minimum = subPacketValue < minimum ? subPacketValue : minimum;
            }
            Console.Write(")");

            return minimum;
        }

        private long Maximum() {
            long maximum = long.MinValue;

            Console.Write("Max(");
            foreach (BITSPacket subPacket in subPackets) {
                long subPacketValue = subPacket.CalculateValue();
                Console.Write(", ");
                maximum = subPacketValue > maximum ? subPacketValue : maximum;
            }
            Console.Write(")");

            return maximum;
        }

        private long GreaterThan() {
            Console.Write("Greater Than: ");
            return subPackets[0].CalculateValue() > subPackets[1].CalculateValue() ? 1 : 0;
        }

        private long LessThan() {
            Console.Write("Less Than: ");
            return subPackets[0].CalculateValue() < subPackets[1].CalculateValue() ? 1 : 0;
        }

        private long EqualTo() {
            Console.Write("Equal To: ");
            return subPackets[0].CalculateValue() == subPackets[1].CalculateValue() ? 1 : 0;
        }
    }

    class Program {
        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 16\Part1Input.txt";
            string input = LoadInput(filePath);

            BITSPacket packet = DecodeBITSTransmission(input);
            long totalVersionNumber = packet.TotalVersionNumber();
            long packetValue = packet.CalculateValue();
            Console.WriteLine("Sum of version numbers: {0}", totalVersionNumber);
            Console.WriteLine("Value of outermost packet: {0}", packetValue);
        }

        public static string LoadInput(string filePath) {
            StreamReader streamReader = new StreamReader(filePath);

            return streamReader.ReadLine();
        }

        public static BITSPacket DecodeBITSTransmission(string hexadecimalInput) {
            string binaryInput = HexadecimalToBinary(hexadecimalInput);

            long packetVersionNumber;
            BITSPacket.TypeID packetTypeID;
            binaryInput = BITSPacket.DecodePacketHeader(binaryInput, out packetVersionNumber, out packetTypeID);

            BITSPacket newPacket;
            switch (packetTypeID) {
                case BITSPacket.TypeID.LiteralNumber:
                    newPacket = new LiteralValuePacket(packetVersionNumber);
                    break;
                default:
                    newPacket = new OperatorPacket(packetVersionNumber);
                    break;
            }

            newPacket.typeID = packetTypeID;

            newPacket.DecodePacketContents(binaryInput);

            return newPacket;
        }

        public static string HexadecimalToBinary(string hexadecimalInput) {
            Dictionary<char, string> hexadecimalToBinaryConversions = new Dictionary<char, string>() {
                { '0', "0000" },
                { '1', "0001" },
                { '2', "0010" },
                { '3', "0011" },
                { '4', "0100" },
                { '5', "0101" },
                { '6', "0110" },
                { '7', "0111" },
                { '8', "1000" },
                { '9', "1001" },
                { 'A', "1010" },
                { 'B', "1011" },
                { 'C', "1100" },
                { 'D', "1101" },
                { 'E', "1110" },
                { 'F', "1111" }
            };

            string binaryInput = "";
            foreach (char hexChar in hexadecimalInput) {
                binaryInput += hexadecimalToBinaryConversions[hexChar];
            }

            return binaryInput;
        }
    }
}
