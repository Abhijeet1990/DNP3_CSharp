using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3Simple
{
    public class DNP3Protocol
    {
    }

    public class LinkLayer
    {
        // List of header fields and data
        public UInt16 startBytes;
        public UInt16 length;
        public byte controlByte;
        public UInt16 destination;
        public UInt16 source;
        public UInt16 crc;
        public byte[] LinkData;

        private static UInt16[] crcTable = new UInt16[256] {
    0x0000, 0x365E, 0x6CBC, 0x5AE2, 0xD978, 0xEF26, 0xB5C4, 0x839A,
    0xFF89, 0xC9D7, 0x9335, 0xA56B, 0x26F1, 0x10AF, 0x4A4D, 0x7C13,
    0xB26B, 0x8435, 0xDED7, 0xE889, 0x6B13, 0x5D4D, 0x07AF, 0x31F1,
    0x4DE2, 0x7BBC, 0x215E, 0x1700, 0x949A, 0xA2C4, 0xF826, 0xCE78,
    0x29AF, 0x1FF1, 0x4513, 0x734D, 0xF0D7, 0xC689, 0x9C6B, 0xAA35,
    0xD626, 0xE078, 0xBA9A, 0x8CC4, 0x0F5E, 0x3900, 0x63E2, 0x55BC,
    0x9BC4, 0xAD9A, 0xF778, 0xC126, 0x42BC, 0x74E2, 0x2E00, 0x185E,
    0x644D, 0x5213, 0x08F1, 0x3EAF, 0xBD35, 0x8B6B, 0xD189, 0xE7D7,
    0x535E, 0x6500, 0x3FE2, 0x09BC, 0x8A26, 0xBC78, 0xE69A, 0xD0C4,
    0xACD7, 0x9A89, 0xC06B, 0xF635, 0x75AF, 0x43F1, 0x1913, 0x2F4D,
    0xE135, 0xD76B, 0x8D89, 0xBBD7, 0x384D, 0x0E13, 0x54F1, 0x62AF,
    0x1EBC, 0x28E2, 0x7200, 0x445E, 0xC7C4, 0xF19A, 0xAB78, 0x9D26,
    0x7AF1, 0x4CAF, 0x164D, 0x2013, 0xA389, 0x95D7, 0xCF35, 0xF96B,
    0x8578, 0xB326, 0xE9C4, 0xDF9A, 0x5C00, 0x6A5E, 0x30BC, 0x06E2,
    0xC89A, 0xFEC4, 0xA426, 0x9278, 0x11E2, 0x27BC, 0x7D5E, 0x4B00,
    0x3713, 0x014D, 0x5BAF, 0x6DF1, 0xEE6B, 0xD835, 0x82D7, 0xB489,
    0xA6BC, 0x90E2, 0xCA00, 0xFC5E, 0x7FC4, 0x499A, 0x1378, 0x2526,
    0x5935, 0x6F6B, 0x3589, 0x03D7, 0x804D, 0xB613, 0xECF1, 0xDAAF,
    0x14D7, 0x2289, 0x786B, 0x4E35, 0xCDAF, 0xFBF1, 0xA113, 0x974D,
    0xEB5E, 0xDD00, 0x87E2, 0xB1BC, 0x3226, 0x0478, 0x5E9A, 0x68C4,
    0x8F13, 0xB94D, 0xE3AF, 0xD5F1, 0x566B, 0x6035, 0x3AD7, 0x0C89,
    0x709A, 0x46C4, 0x1C26, 0x2A78, 0xA9E2, 0x9FBC, 0xC55E, 0xF300,
    0x3D78, 0x0B26, 0x51C4, 0x679A, 0xE400, 0xD25E, 0x88BC, 0xBEE2,
    0xC2F1, 0xF4AF, 0xAE4D, 0x9813, 0x1B89, 0x2DD7, 0x7735, 0x416B,
    0xF5E2, 0xC3BC, 0x995E, 0xAF00, 0x2C9A, 0x1AC4, 0x4026, 0x7678,
    0x0A6B, 0x3C35, 0x66D7, 0x5089, 0xD313, 0xE54D, 0xBFAF, 0x89F1,
    0x4789, 0x71D7, 0x2B35, 0x1D6B, 0x9EF1, 0xA8AF, 0xF24D, 0xC413,
    0xB800, 0x8E5E, 0xD4BC, 0xE2E2, 0x6178, 0x5726, 0x0DC4, 0x3B9A,
    0xDC4D, 0xEA13, 0xB0F1, 0x86AF, 0x0535, 0x336B, 0x6989, 0x5FD7,
    0x23C4, 0x159A, 0x4F78, 0x7926, 0xFABC, 0xCCE2, 0x9600, 0xA05E,
    0x6E26, 0x5878, 0x029A, 0x34C4, 0xB75E, 0x8100, 0xDBE2, 0xEDBC,
    0x91AF, 0xA7F1, 0xFD13, 0xCB4D, 0x48D7, 0x7E89, 0x246B, 0x1235
};

        public void serialize(ref byte[] buffer)
        {
            // Add Header
            AddHeader(ref buffer);

        }

        public void AddHeader(ref byte[] buffer)
        {
            // Add Crc
            UInt16 Crc = GetCRC(ref buffer);
            buffer = InsertBytes(buffer, Crc);

            // Add src and dest
            buffer = InsertBytes(buffer, source);
            buffer = InsertBytes(buffer, destination);

            // Add Control byte
            UsefulMethods.InsertElementArray(ref buffer, 0, controlByte);

            // Add Length
            length = (UInt16)((UInt16)buffer.Count()/ (UInt16)2); // yet to add the start bytes
            buffer = InsertBytes(buffer, length);

            // Add Start Bytes
            startBytes = 0x0564;
            buffer = InsertBytes(buffer, startBytes);

        }

        private static byte[] InsertBytes(byte[] buffer, ushort Crc)
        {
            byte low_byte = (byte)(Crc & 0xFF);
            byte high_byte = (byte)((Crc >> 8) & 0xFF);
            UsefulMethods.InsertElementArray(ref buffer, 0, low_byte);
            UsefulMethods.InsertElementArray(ref buffer, 0, high_byte);
            return buffer;
        }

        public void deserialize(ref byte[] buffer)
        {
            // Extract Start bytes
            startBytes = GetInts(ref buffer);

            // Extract length
            length = GetInts(ref buffer);

            // Extract Control byte
            controlByte = UsefulMethods.RemoveHeader(ref buffer, 0);

            // Extract destination
            destination = GetInts(ref buffer);

            //Extract Source
            source = GetInts(ref buffer);

            // Extract crc
            crc = GetInts(ref buffer);

            // Verify crc
            bool verify = ValidateCRC(ref buffer, crc);

            if (verify) Console.WriteLine("verified data");

            else return;

        }

        private UInt16 GetInts(ref byte[] buffer)
        {
            byte high_byte = UsefulMethods.RemoveHeader(ref buffer, 0);
            byte low_byte = UsefulMethods.RemoveHeader(ref buffer, 0);
            var merged = (UInt16)((high_byte << 8) + low_byte);
            return merged;
        }

        public UInt16 GetCRC(ref byte[] buffer)
        {
            crc = ComputeCRC(ref buffer);
            return crc; 
        } 

        public UInt16 ComputeCRC(ref byte[] buffer)
        {
            UInt16 CRC = 0;
            int len = buffer.Count();

            for (UInt32 i = 0; i < len; ++i)
            {
                byte index = (byte)((CRC ^ buffer[i]) & 0xFF);
                CRC = (UInt16)(crcTable[index] ^ (CRC >> 8));
            }

            // Bit reverse
            UInt16 rev = 0;
            for (int i = 0; i < 16; ++i)
            {
                rev <<= 1;
                rev |= (UInt16)(CRC & 1);
                CRC >>= 1;
            }
            return rev;
        }

        public bool ValidateCRC(ref byte[] buffer, UInt16 crc_)
        {
            if (crc_ == ComputeCRC(ref buffer))
            {
                return true;
            }
            return false;
        }
    }

    public class TransportLayer
    {
        // List of header fields and data

        public byte FIR;
        public byte FIN;
        public byte seq;
        public byte[] TransportData;

        public void serialize(ref byte[] buffer)
        {
            // Add Header
            byte header = 0;
            header |= (byte)(FIR << 7);
            header |= (byte)(FIN << 6);
            header |= seq;
            UsefulMethods.InsertElementArray(ref buffer, 0, header);

        }

        public void deserialize(ref byte[] buffer)
        {
            // Extract Header
            var transHeader = UsefulMethods.RemoveHeader(ref buffer, 0);
            seq = (byte)(transHeader & 0x3F);
            FIR = (byte)((transHeader >> 6) & 0x01);
            FIN = (byte)((transHeader >> 7) & 0x01);

            TransportData = buffer;

        }
    }

    public class ApplicationLayer
    {
        // List of header fields and data
        public byte ApplicationControl; // take a default c2 FIR =1,FIN =2, Seq =2
        public byte FunctionCode; // take READ as example 0x01

        // for response
        public UInt16 InternalIndications;

        public byte[] ApplicationData; // READ Binary Input Change (02) Default Variation (00) Prefix Code(0) RangeCode(6)

        public void serialize(ref byte[] buffer)
        {
            //Add Header
            AddHeader(ref buffer);
        }

        public void deserialize(ref byte[] buffer)
        {
            // Extract Header
            ApplicationControl = UsefulMethods.RemoveHeader(ref buffer, 0);
            FunctionCode = UsefulMethods.RemoveHeader(ref buffer, 0);

            // Set the data
            ApplicationData = buffer;
        }

        public void AddHeader(ref byte[] buffer)
        {
            // Add Function Code
            UsefulMethods.InsertElementArray(ref buffer, 0, FunctionCode);

            // Add Application Control
            UsefulMethods.InsertElementArray(ref buffer, 0, ApplicationControl);

        }

    }

    public static class UsefulMethods
    {
        public static byte RemoveHeader(ref byte[] buffer, UInt16 position)
        {
            var x = buffer.ToList();
            var ret = buffer[0];
            x.RemoveAt(position);
            buffer = x.ToArray();
            return ret;
        }

        public static void InsertElementArray(ref byte[] buffer, UInt16 position, byte item)
        {
            var x = buffer.ToList();
            x.Insert(position, item);
            buffer = x.ToArray();
        }
    }
}
