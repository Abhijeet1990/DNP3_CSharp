using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3Link
{
    public class LinkLayer
    {
    }

    public class CRC
    {
        //public string buffer;
        public byte[] buffer_bytes;
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
        public UInt16 CalcCrc(byte[] input, UInt32 length )
        {
            UInt16 CRC = 0;

            for (UInt32 i = 0; i < length; ++i)
            {
                byte index = (byte)((CRC ^ input[i]) & 0xFF);
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

        public byte[] AddCrc(byte[] input, UInt32 length)
        {
            UInt16 crc = CalcCrc(input, length);
            // add the crc to the back of input
            input[length] = (byte)(crc>>8);
            input[length + 1] = (byte) (crc & 0xFF);
            return input;
        }

        public bool IsCorrectCrc(byte[] input, UInt32 length)
        {
            return CalcCrc(input, length) == input[length];
        }
    }

    public enum LinkFunction: byte
    {
        PRI_RESET_LINK_STATES = 0x40,
        PRI_TEST_LINK_STATES = 0x42,
        PRI_CONFIRMED_USER_DATA = 0x43,
        PRI_UNCONFIRMED_USER_DATA = 0x44,
        PRI_REQUEST_LINK_STATUS = 0x49,
        SEC_ACK = 0x0,
        SEC_NACK = 0x1,
        SEC_LINK_STATUS = 0xB,
        SEC_NOT_SUPPORTED = 0xF,
        INVALID = 0xFF
    }

    public class LinkFunctionClass
    {
        public byte LinkFunctionToType(LinkFunction arg)
        {
            return (byte)arg;
        }

        public LinkFunction LinkFunctionFromType(byte arg)
        {
            switch (arg)
            {
                case (0x40):
                    return LinkFunction.PRI_RESET_LINK_STATES;
                case (0x42):
                    return LinkFunction.PRI_TEST_LINK_STATES;
                case (0x43):
                    return LinkFunction.PRI_CONFIRMED_USER_DATA;
                case (0x44):
                    return LinkFunction.PRI_UNCONFIRMED_USER_DATA;
                case (0x49):
                    return LinkFunction.PRI_REQUEST_LINK_STATUS;
                case (0x0):
                    return LinkFunction.SEC_ACK;
                case (0x1):
                    return LinkFunction.SEC_NACK;
                case (0xB):
                    return LinkFunction.SEC_LINK_STATUS;
                case (0xF):
                    return LinkFunction.SEC_NOT_SUPPORTED;
                default:
                    return LinkFunction.INVALID;
            }
        }

        public string LinkFunctionToString(LinkFunction arg)
        {
            switch (arg)
            {
                case (LinkFunction.PRI_RESET_LINK_STATES):
                    return "PRI_RESET_LINK_STATES";
                case (LinkFunction.PRI_TEST_LINK_STATES):
                    return "PRI_TEST_LINK_STATES";
                case (LinkFunction.PRI_CONFIRMED_USER_DATA):
                    return "PRI_CONFIRMED_USER_DATA";
                case (LinkFunction.PRI_UNCONFIRMED_USER_DATA):
                    return "PRI_UNCONFIRMED_USER_DATA";
                case (LinkFunction.PRI_REQUEST_LINK_STATUS):
                    return "PRI_REQUEST_LINK_STATUS";
                case (LinkFunction.SEC_ACK):
                    return "SEC_ACK";
                case (LinkFunction.SEC_NACK):
                    return "SEC_NACK";
                case (LinkFunction.SEC_LINK_STATUS):
                    return "SEC_LINK_STATUS";
                case (LinkFunction.SEC_NOT_SUPPORTED):
                    return "SEC_NOT_SUPPORTED";
                default:
                    return "INVALID";
            }
        }
    }

    public enum LinkHeaderIndex : byte
    {
        LI_START_05 = 0,
        LI_START_64 = 1,
        LI_LENGTH = 2,
        LI_CONTROL = 3,
        LI_DESTINATION = 4,
        LI_SOURCE = 6,
        LI_CRC = 8
    }

    public enum ControlMask : byte
    {
        MASK_DIR = 0x80,
        MASK_PRM = 0x40,
        MASK_FCB = 0x20,
        MASK_FCV = 0x10,
        MASK_FUNC = 0x0F,
        MASK_FUNC_OR_PRM = MASK_PRM | MASK_FUNC
    }

    public class LinkLayerConstants
    {
        public const byte LPDU_MIN_LENGTH = 5;
        public const byte LPDU_MAX_LENGTH = 255;
        public const byte LPDU_HEADER_SIZE = 10;
        public const byte LPDU_DATA_BLOCK_SIZE = 16;
        public const byte LPDU_CRC_SIZE = 2;
        public const byte LPDU_DATA_PLUS_CRC_SIZE = 18;
        public const byte LPDU_MAX_USER_DATA_SIZE = 250;
        public const UInt16 LPDU_MAX_FRAME_SIZE = 292;	//10(header) + 250 (user data) + 32 (block CRC's) = 292 frame bytes
    }

    public class LinkHeader
    {
        // Fields read directly from the header
        private byte length; // Length of field, range [5,255] valid
        private UInt16 src;   // Where the frame originated
        private UInt16 dest;  // Where the frame is going
        private byte ctrl;	// Control octet, individual fields accessed using accessors below
        //public byte[] buffer_bytes;
        public MemoryStream ms;

        public LinkHeader()
        {

        }
        public LinkHeader(byte aLen, UInt16 aSrc, UInt16 aDest, bool aFromMaster, bool aFcvDfc, bool aFcb, LinkFunction aCode)
        {
            length = aLen;
            src = aSrc;
            dest = aDest;
            ctrl = ControlByte(aFromMaster, aFcb, aFcvDfc, aCode);
        }
        public byte GetControl()
        {
            return ctrl;
        }
        public byte GetLength()
        {
            return length;
        }
        public UInt16 GetDest()
        {
            return dest;
        }
        public UInt16 GetSrc()
        {
            return src;
        }
        public bool IsFromMaster()
        {
            return ((ctrl & (byte)ControlMask.MASK_DIR) != 0);
        }
        public bool IsPriToSec()
        {
            return ((ctrl & (byte)ControlMask.MASK_PRM) != 0);
        }
        public bool IsFcbSet()
        {
            return ((ctrl & (byte)ControlMask.MASK_FCB) != 0);
        }
        public bool IsFcvDfcSet()
        {
            return ((ctrl & (byte)ControlMask.MASK_FCV) != 0);
        }
        public byte GetFuncByte()
        {
            return (byte)(ctrl & (byte)ControlMask.MASK_FUNC);
        }
        public LinkFunction GetFuncEnum()
        {
            var f = new LinkFunctionClass();
            return f.LinkFunctionFromType((byte)(ctrl & (byte)ControlMask.MASK_FUNC_OR_PRM));
        }
        public bool ValidLength()
        {
            return length > 4;
        }
        public void ChangeFCB(bool aFCB)
        {
            if (aFCB)
            {
                ctrl |= (byte)ControlMask.MASK_FCB;
            }
            else
            {
                // bit inversion
                byte rev = 0;
                byte orig = (byte)ControlMask.MASK_FCB;
                for (int i = 0; i < 16; ++i)
                {
                    rev <<= 1;
                    rev |= (byte)(orig & 1);
                    orig >>= 1;
                }
                ctrl &= rev;
            }
        }
        public byte ControlByte(bool aIsMaster, bool aFcb, bool aFcvDfc, LinkFunction aFunc)
        {
            var field = new LinkFunctionClass();
            byte ret = field.LinkFunctionToType(aFunc);

            if (aIsMaster) ret |= (byte)ControlMask.MASK_DIR;
            if (aFcb) ret |= (byte)ControlMask.MASK_FCB;
            if (aFcvDfc) ret |= (byte)ControlMask.MASK_FCV;

            return ret;
        }
        public void Read(byte[] input)
        {
            length = input[(int)LinkHeaderIndex.LI_LENGTH];
            dest = (UInt16)(input[(int)LinkHeaderIndex.LI_DESTINATION] + (input[(int)LinkHeaderIndex.LI_DESTINATION + 1] << 8)); //same output
            src = (UInt16)(input[(int)LinkHeaderIndex.LI_SOURCE] + (input[(int)LinkHeaderIndex.LI_SOURCE + 1] << 8)); //same output
            ctrl = input[(int)LinkHeaderIndex.LI_CONTROL];
        }
        public void Write(byte[] input)
        {
            input[(int)LinkHeaderIndex.LI_START_05] = 0x05;
            input[(int)LinkHeaderIndex.LI_START_64] = 0x64;
            input[(int)LinkHeaderIndex.LI_LENGTH] = length;
            input[(int)LinkHeaderIndex.LI_DESTINATION] = (byte)(dest >> 8);
            input[(int)LinkHeaderIndex.LI_DESTINATION + 1] = (byte)(dest & 0xFF);
            input[(int)LinkHeaderIndex.LI_SOURCE] = (byte)(src >> 8);
            input[(int)LinkHeaderIndex.LI_SOURCE + 1] = (byte)(src & 0xFF);
            input[(int)LinkHeaderIndex.LI_CONTROL] = ctrl;

            var crc = new CRC();
            input = crc.AddCrc(input, (int)LinkHeaderIndex.LI_CRC);
            // this is just written .. not so significant
            ms.Write(input, 0, input.Length);

        }
    }

    public class LinkHeaderFields
    {
        public LinkFunction func;
        public bool isFromMaster;
        public bool fcb;
        public bool fcvdfc;
        public UInt16 dest;
        public UInt16 src;

        public Addresses ToAddresses()
        {
            return new Addresses(src, dest);
        }

        public LinkHeaderFields(LinkFunction func_, bool isMaster_, bool fcb_, bool fcvdfc_, UInt16 dest_, UInt16 source_)
        {
            func = func_;
            isFromMaster = isMaster_;
            fcb = fcb_;
            fcvdfc = fcvdfc_;
            dest = dest_;
            src = source_;
        }
    }

    public class LinkFrame
    {
        public static void ReadUserData(byte[] pSrc, byte[] pDest, UInt32 length_)
        {
            var length = length_;
            var pRead = pSrc;
            var pWrite = pDest;

            while (length > 0)
            {
                UInt32 max = LinkLayerConstants.LPDU_DATA_BLOCK_SIZE;
                UInt32 num = (length <= max) ? length : max;
                UInt32 num_with_crc = num + 2;
                Array.Copy(pRead, pWrite,  num);
                pRead = ResizeByteArray(pRead,num_with_crc);
                pWrite = ResizeByteArray(pWrite,num);
                length -= num;
            }
        }
        public static void WriteUserData(byte[] pSrc, byte[] pDest, byte length)
        {
            while (length > 0)
            {
                byte max = LinkLayerConstants.LPDU_DATA_BLOCK_SIZE;
                byte num = length > max ? max : length;
                Array.Copy(pSrc, pDest, num);
                var f = new CRC();
                f.AddCrc(pDest, num);
                pSrc = ResizeByteArray(pSrc, num);
                pDest = ResizeByteArray(pDest, (byte)(num+2));
                length -= num;
            }
        }
        public static Int32 CalcFrameSize(byte dataLength)
        {
            return LinkLayerConstants.LPDU_HEADER_SIZE + CalcUserDataSize(dataLength);
        }
        private static Int32 CalcUserDataSize(byte dataLength)
        {
            if (dataLength > 0)
            {
                Int32 mod16 = dataLength % LinkLayerConstants.LPDU_DATA_BLOCK_SIZE;
                Int32 size = (dataLength / LinkLayerConstants.LPDU_DATA_BLOCK_SIZE) * LinkLayerConstants.LPDU_DATA_PLUS_CRC_SIZE; //complete blocks
                return (mod16 > 0) ? (size + mod16 + LinkLayerConstants.LPDU_CRC_SIZE) : size; //possible partial block
            }
            else
            {
                return 0;
            }
        }
        public static bool ValidateBodyCRC(byte[] pBody, UInt32 length)
        {
            var temp = pBody;
            while (length > 0)
	        {
		        UInt32 max = LinkLayerConstants.LPDU_DATA_BLOCK_SIZE;
                UInt32 num = (length <= max) ? length : max;

                var crc = new CRC();               
		        if (crc.IsCorrectCrc(temp, num))
                {
                    //Array.Resize<byte>(ref temp, (int)num + 2);
                    temp = ResizeByteArray(temp, num);
                    length -= num;
                }
                else
		        {
			        return false;
		        }
	        }
	        return true;
        }
        public static byte[] ResizeByteArray(byte[] temp, uint num)
        {
            var sourceStartIndex = 1;
            var destinationLength = temp.Length - ((int)num + 2);
            var destinationStartIndex = 0;
            var destination = new byte[destinationLength];
            Array.Copy(temp, sourceStartIndex, destination, destinationStartIndex, destinationLength);
            temp = destination;
            return temp;
        }

        private static byte[] FormatHeader(ref Byte[] buffer, byte aDataLength, bool aIsMaster, bool aFcb, bool aFcvDfc, LinkFunction aFuncCode, UInt16 aDest, UInt16 aSrc)
        {
            if (buffer.Length < (int)LinkLayerConstants.LPDU_HEADER_SIZE)
                {
                return null;
                }
            LinkHeader header = new LinkHeader((byte)(aDataLength + LinkLayerConstants.LPDU_MIN_LENGTH), aSrc, aDest, aIsMaster, aFcvDfc, aFcb, aFuncCode);
            header.Write(buffer);
            var temp = buffer;
            var ret = temp.Take(10).ToArray();           
            temp = ResizeByteArray(temp, 10);
            buffer = temp;
            return  ret;
        }

        public static byte[] FormatAck(ref Byte[] buffer, bool aIsMaster, bool aIsRcvBuffFull, UInt16 aDest, UInt16 aSrc)
        {
            return FormatHeader(ref buffer, 0, aIsMaster, false, aIsRcvBuffFull, LinkFunction.SEC_ACK, aDest, aSrc);
        }
        public static byte[] FormatNack(ref Byte[] buffer, bool aIsMaster, bool aIsRcvBuffFull, UInt16 aDest, UInt16 aSrc)
        {
            return FormatHeader(ref buffer, 0, aIsMaster, false, aIsRcvBuffFull, LinkFunction.SEC_NACK, aDest, aSrc);
        }
        public static byte[] FormatLinkStatus(ref Byte[] buffer, bool aIsMaster, bool aIsRcvBuffFull, UInt16 aDest, UInt16 aSrc)
        {
            return FormatHeader(ref buffer, 0, aIsMaster, false, aIsRcvBuffFull, LinkFunction.SEC_LINK_STATUS, aDest, aSrc);
        }
        public static byte[] FormatNotSupported(ref Byte[] buffer, bool aIsMaster, bool aIsRcvBuffFull, UInt16 aDest, UInt16 aSrc)
        {
            return FormatHeader(ref buffer, 0, aIsMaster, false, aIsRcvBuffFull, LinkFunction.SEC_NOT_SUPPORTED, aDest, aSrc);
        }

        public static byte[] FormatResetLinkStates(ref Byte[] buffer, bool aIsMaster, UInt16 aDest, UInt16 aSrc)
        {
            return FormatHeader(ref buffer, 0, aIsMaster, false, false, LinkFunction.PRI_RESET_LINK_STATES, aDest, aSrc);
        }
        public static byte[] FormatRequestLinkStatus(ref Byte[] buffer, bool aIsMaster, UInt16 aDest, UInt16 aSrc)
        {
            return FormatHeader(ref buffer, 0, aIsMaster, false, false, LinkFunction.PRI_REQUEST_LINK_STATUS, aDest, aSrc);
        }
        public static byte[] FormatTestLinkStatus(ref Byte[] buffer, bool aIsMaster,bool aFcb, UInt16 aDest, UInt16 aSrc)
        {
            return FormatHeader(ref buffer, 0, aIsMaster, aFcb, true, LinkFunction.PRI_TEST_LINK_STATES, aDest, aSrc);
        }

        public static byte[] FormatConfirmedUserData(ref byte[] buffer, bool aIsMaster, bool aFcb, UInt16 aDest, UInt16 aSrc, byte[] apData, byte dataLength)
        {
            if (dataLength > 0) return null;
            if (dataLength > LinkLayerConstants.LPDU_MAX_USER_DATA_SIZE) return null;
            var userDataSize = CalcUserDataSize(dataLength);
            var temp = buffer;
            var ret = temp.Take(userDataSize + LinkLayerConstants.LPDU_HEADER_SIZE).ToArray();
            FormatHeader(ref buffer, dataLength, aIsMaster, aFcb, true, LinkFunction.PRI_CONFIRMED_USER_DATA, aDest, aSrc);
            WriteUserData(apData, buffer, dataLength);
            buffer = ResizeByteArray(buffer, (byte)userDataSize);
            return ret;
        }

        public static byte[] FormatUnconfirmedUserData(ref byte[] buffer, bool aIsMaster, bool aFcb, UInt16 aDest, UInt16 aSrc, byte[] apData, byte dataLength)
        {
            if (dataLength > 0) return null;
            if (dataLength > LinkLayerConstants.LPDU_MAX_USER_DATA_SIZE) return null;
            var userDataSize = CalcUserDataSize(dataLength);
            var temp = buffer;
            var ret = temp.Take(userDataSize + LinkLayerConstants.LPDU_HEADER_SIZE).ToArray();
            FormatHeader(ref buffer, dataLength, aIsMaster, aFcb, true, LinkFunction.PRI_UNCONFIRMED_USER_DATA, aDest, aSrc);
            WriteUserData(apData, buffer, dataLength);
            buffer = ResizeByteArray(buffer, (byte)userDataSize);
            return ret;
        }
    }

    public class Addresses
    {
        public UInt16 source = 0;
        public UInt16 destination = 0;

        public Addresses(UInt16 s, UInt16 d)
        {
            source = s;
            destination = d;
        }

        public Addresses Reverse()
        {
            return new Addresses(this.destination, this.source);
        }

        public bool IsEqual(Addresses a)
        {
            if((this.destination == a.destination) && (this.source == a.source)) return true;
            else return false;
        }
    }

    public class LinkConfig
    {
        /// The master/outstation bit set on all messages
        public bool IsMaster;

        /// If true, the link layer will send data requesting confirmation
        public bool UseConfirms;

        /// The number of retry attempts the link will attempt after the initial try
        public UInt16 NumRetry;

        /// dnp3 address of the local device
        public UInt16 LocalAddr;

        /// dnp3 address of the remote device
        public UInt16 RemoteAddr;

        /// the response timeout in milliseconds for confirmed requests
        TimeSpan TimeOut;

        /// the interval for keep-alive messages (link status requests)
        TimeSpan KeepAliveTimeout;

        public LinkConfig() { }
        public LinkConfig(bool isMaster,
        bool useConfirms,
        UInt16 numRetry,
        UInt16 localAddr,
        UInt16 remoteAddr,
        TimeSpan timeout,
        TimeSpan keepAliveTimeout)
        {
            IsMaster = isMaster;
            UseConfirms = useConfirms;
            NumRetry = numRetry;
            LocalAddr = localAddr;
            RemoteAddr = remoteAddr;
            TimeOut = timeout;
            KeepAliveTimeout = keepAliveTimeout;
        }

        public LinkConfig(bool isMaster,
        bool useConfirms)
        {
            IsMaster = isMaster;
            UseConfirms = useConfirms;
            NumRetry = 0;
            LocalAddr = (UInt16)(isMaster ? 1 : 1024);
            RemoteAddr = (UInt16)(isMaster ? 1024 : 1);
            TimeOut = new TimeSpan(0,0,1); // 1 second
            KeepAliveTimeout = new TimeSpan(0, 1, 0); // 1 minute
        }

        public Addresses GetAddresses()
        {
            return new Addresses(this.LocalAddr, this.RemoteAddr);
        }
    }

    public class LinkLayerConfig : LinkConfig
    {
        /**
	* If true, the the link-layer will respond to any source address
	* user data frames will be passed up to transport reassembly for these frames
	*/
        public bool respondToAnySource;
        public LinkConfig linkConfig;

        public LinkLayerConfig(LinkConfig lc, bool respondToAnySource_)
        {
            linkConfig = lc;
            respondToAnySource = respondToAnySource_;
        }
    }

    public class ShiftableBuffer
    {
        private byte[] pBuffer;
        private UInt32 M_SIZE = 16;
        private UInt32 writePos;
        private UInt32 readPos;

        public ShiftableBuffer(byte[] pBuffer_, UInt32 size)
        {
            pBuffer = pBuffer_;
            M_SIZE = size;
            writePos = 0;
            readPos = 0;

        }

        public UInt32 NumBytesRead()
        {
            return writePos - readPos;
        }

        /// @return Bytes of available for writing
        public UInt32 NumWriteBytes()
    	{
		     return M_SIZE - writePos;
	    }

        /// @return Pointer to the position in the buffer available for writing
        public Byte[] WriteBuff()
	    {
            var temp = pBuffer;
            temp = temp.Where((v, i) => i > writePos).ToArray();
            return temp;
	     }


        public void Shift()
        {
            var numRead_ = this.NumBytesRead();

            var temp = pBuffer;
            temp = temp.Where((v, i) => i > readPos).ToArray();
            Array.Copy(temp, pBuffer, numRead_);

            readPos = 0;
            writePos = numRead_;
        }

        public void Reset()
        {
            writePos = 0; readPos = 0;
        }

        public void AdvanceRead(UInt32 numBytes)
        {
            readPos += numBytes;
        }

        public void AdvanceWrite(UInt32 numBytes)
        {
            writePos += numBytes;
        }

        public bool Sync(UInt32 skipCount)
        {
            var byteCount = this.NumBytesRead();
            while (byteCount > 1) // at least 2 bytes
            {
                if (pBuffer[readPos] == 0x05 && pBuffer[readPos+1] == 0x64)
                {
                    return true;
                }
                else
                {
                    this.AdvanceRead(1); // skip the first byte
                    ++skipCount;
                }
            }
            return false;
        }
    }

   
}
