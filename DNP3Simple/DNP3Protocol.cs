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

    public enum DIR : byte
    {
        /// Indicates a frame from Outstation
        OUTSTATION = 0x0,
        /// Indicates a frame from Master
        MASTER = 0x1
    }

    public enum PRM : byte
    {
        /// Transaction terminated by Master or Outstation
        TERMINATED = 0x0,
        /// Transaction initiated by Master or Outstation
        INITIATED = 0x1
    }

    public enum FCB : byte
    {
        set = 0x1,
        unset = 0x0
    }

    public enum FCV : byte
    {
        set = 0x1,
        unset = 0x0
    }

    public enum DFC: byte
    {
        set = 0x1,
        unset = 0x0
    }

    public enum PrimaryFunctionCode : byte
    {
        PRI_RESET_LINK_STATES = 0x0,
        PRI_TEST_LINK_STATES = 0x2,
        PRI_CONFIRMED_USER_DATA = 0x3,
        PRI_UNCONFIRMED_USER_DATA = 0x4,
        PRI_REQUEST_LINK_STATUS = 0x9,
        INVALID = 0xFF
    }

    public enum SecondaryFunctionCode : byte
    {
        SEC_ACK = 0x0,
        SEC_NACK = 0x1,
        SEC_LINK_STATUS = 0xB,
        SEC_NOT_SUPPORTED = 0xF,
        INVALID = 0xFF
    }

    public class LinkLayer
    {
        // List of header fields and data
        public UInt16 startBytes;
        public byte length;
        public byte controlByte;

        public byte dir;
        public byte prm;
        // FCB is used to detect losses and duplication
        // FCB is valid only for request messages sent from Primary stations to secondary stations
        public byte fcb;
        // fcv is for request and dfc is for response
        // fcv = 1 indicates the state of FCB bit is valid and the state of the fcb bit in the received message must be checked
        // fcv = 0 indicates the state of FCB is ignored
        // dfc = 1 indicates receive buffer at sec station unavailable
        // dfc = 0 indicates the receive buffer is available 
        public byte fcv;
        public byte dfc;
        public byte functionCode;

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

        /* Build ControlByte
         * bit 0-3 :Function Code
         * bit 4: FCV/DFC
         * bit 5: FCB/0
         * bit 6: PRM
         * bit 7: DIR = 0 for Outstation and 1 for Master            
        */
        public byte GetControlByte(bool isReq)
        {
            byte ctrlByte = 0;
            ctrlByte = (byte)(ctrlByte + (dir << 7));
            ctrlByte = (byte)(ctrlByte + (prm << 6));
            ctrlByte = (byte)(ctrlByte + (fcb << 5));
            if (isReq) ctrlByte = (byte)(ctrlByte + (fcv << 4));
            else ctrlByte = (byte)(ctrlByte + (dfc << 4));
            ctrlByte = (byte)(ctrlByte + functionCode);
            return ctrlByte;

        }

        
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
            length = (byte)(buffer.Count()); // yet to add the start bytes but remove the CRC bytes 
            UsefulMethods.InsertElementArray(ref buffer, 0, length);

            // Add Start Bytes
            startBytes = 0x6405;
            buffer = InsertBytes(buffer, startBytes);

        }

        private static byte[] InsertBytes(byte[] buffer, ushort data)
        {
            byte lsb = (byte)(data & 0xFF);
            byte hsb = (byte)((data >> 8) & 0xFF);            
            UsefulMethods.InsertElementArray(ref buffer, 0, hsb);
            UsefulMethods.InsertElementArray(ref buffer, 0, lsb);
            return buffer;
        }

        public void deserialize(ref byte[] buffer)
        {
            // Extract Start bytes
            startBytes = GetInts(ref buffer);

            // Extract length
            length = UsefulMethods.RemoveHeader(ref buffer, 0);

            // Extract Control byte
            controlByte = UsefulMethods.RemoveHeader(ref buffer, 0);

            // Process Control byte
            ProcessControlByte(controlByte);

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

        public void ProcessControlByte(byte ctrlByte)
        {
            dir = (byte)((ctrlByte & 0x80) >> 7);
            prm = (byte)((ctrlByte & 0x40) >> 6);
            fcb = (byte)((ctrlByte & 0x20) >> 5);
            fcv = (byte)((ctrlByte & 0x10) >> 4);
            functionCode = (byte)(ctrlByte & 0x0F);
        }

        public static Byte ModifyControlByte(ref LinkLayer ll_)
        {
            //ll_.dir = (byte)((ll_.controlByte & 0x80) >> 7);
            if (ll_.dir == (byte)DIR.MASTER) { ll_.dir = (byte)DIR.OUTSTATION; }
            //ll_.prm = (byte)((ll_.controlByte & 0x40) >> 6);
            if (ll_.prm == (byte)PRM.INITIATED) { ll_.prm = (byte)PRM.TERMINATED; }
            //ll_.fcb = (byte)((ll_.controlByte & 0x20) >> 5);
            if(ll_.fcb == (byte)FCB.set) { ll_.fcv = (byte)((ll_.controlByte & 0x10) >> 4); }
            //ll_.functionCode = (byte)(ll_.controlByte & 0x0F);
            if(ll_.functionCode == (byte)PrimaryFunctionCode.PRI_CONFIRMED_USER_DATA)
            {
                ll_.functionCode = (byte)SecondaryFunctionCode.SEC_ACK;
            }
            return ll_.GetControlByte(false);
            
        }

        public static byte[] LinkResponse(LinkLayer ll_, TransportLayer tl_)
        {
            LinkLayer ll = new LinkLayer();
            ll.LinkData = tl_.TransportData;
            ll.source = ll_.destination;
            ll.destination = ll_.source;
            ll.controlByte = ModifyControlByte(ref ll_);
            ll.serialize(ref ll.LinkData);
            return ll.LinkData;
        }

        private UInt16 GetInts(ref byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return 0;             
            byte lsb = UsefulMethods.RemoveHeader(ref buffer, 0);
            byte hsb = UsefulMethods.RemoveHeader(ref buffer, 0);
            var merged = (UInt16)((hsb << 8) + lsb);
            return merged;
        }

        public UInt16 GetCRC(ref byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return 0;
            crc = ComputeCRC(ref buffer);
            return crc; 
        } 

        public UInt16 ComputeCRC(ref byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return 0;
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

    public enum FIRTrans : byte
    {
        SET = 0x1,
        UNSET = 0x0
    }
    public enum FINTrans : byte
    {
        SET = 0x1,
        UNSET = 0x0
    }


    public class TransportLayer
    {
        // List of header fields and data

        
        public byte FIN;
        public byte FIR;
        public byte seq;
        public byte[] TransportData;
        public bool stillMoreSegments;
        public bool isFirstSegment;
        public bool segSeriesStarted;
        public bool segSeriesEnded;
        public byte previousSegment;
        public byte expectedSegment;
        public bool isMultipleSegment;
        public bool recvdAllSegment;
        public bool discard;


        public void serialize(ref byte[] buffer)
        {
            // Add Header
            byte header = 0;
            header |= (byte)(FIN << 7);
            header |= (byte)(FIR << 6);
            header |= seq;
            UsefulMethods.InsertElementArray(ref buffer, 0, header);

        }
        /*
        public void ProcessTransport(byte transportByte)
        {
            FIN = (byte)((transportByte & 0x80) >> 7);
            FIR = (byte)((transportByte & 0x40) >> 6);
            seq = (byte)((transportByte & 0x20) & 0x3F);
        }*/

        public static void ProcessTransport(ref TransportLayer tl_)
        {
            if ((byte)(tl_.seq + 1) == 64)
                tl_.expectedSegment = 0;
            else
                tl_.expectedSegment = (byte)(tl_.seq + 1);

            //Rule 1: transport seg-series may only begin with FIR bit set.
            if (tl_.isMultipleSegment)
            {
                if (tl_.FIR == (byte)FIRTrans.SET && tl_.segSeriesStarted == false)
                {
                    // Transport segment series starts
                    tl_.segSeriesStarted = true;
                }
                // Rule 2: transport seg-series ends with FIN bit set.
                if (tl_.FIN == (byte)FINTrans.SET && tl_.segSeriesEnded == false)
                {
                    // Transport segment series ends
                    tl_.segSeriesEnded = true;
                }

                // Rule 3: no trans seg-series in progress and FIR not set will be discarded
                if (tl_.recvdAllSegment && tl_.FIR == (byte)FIRTrans.UNSET)
                {
                    // Do not consider the FIR bit

                }
            }

            // Rule 4: A trans seg with FIR bit set may have any seq no 0 to 63 without regard to history

            if (tl_.segSeriesStarted)
            {
                // Rule 5 (b): received trans seg with FIR set will discard in progress seg-series.
                if (tl_.isMultipleSegment && tl_.FIR == (byte)FIRTrans.SET)
                {
                    tl_.discard = true;
                }

                // Rule 5 (c): seg no. equivalent to previous is discarded
                if (tl_.seq == tl_.previousSegment)
                {
                    tl_.discard = true;
                }

                // Rule 5 (d): if FIR is UNSET and seq no. different then expected discard
                if (tl_.FIR == (byte)FIRTrans.UNSET && (tl_.seq != tl_.expectedSegment))
                {
                    // discard further segments and exit
                    tl_.discard = true;
                }
            }
            // Rule 6: there is only one segment if FIR and FIN are set
            if(tl_.FIR == (byte)FIRTrans.SET && tl_.FIN == (byte)FINTrans.SET)
            {
                tl_.isMultipleSegment = false;
                tl_.recvdAllSegment = true;
            }

            // Rule 7: if all the segments are assembled then only send the payload to Application Layer
            if(tl_.isMultipleSegment && tl_.FIN == (byte)FINTrans.SET && tl_.discard == false)
            {
                tl_.recvdAllSegment = true;
                tl_.segSeriesEnded = true;
            }

            tl_.previousSegment = tl_.seq;

        }

        public void deserialize(ref byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return;
            // Extract Header
            var transHeader = UsefulMethods.RemoveHeader(ref buffer, 0);
            seq = (byte)(transHeader & 0x3F);
            FIR = (byte)((transHeader >> 6) & 0x01);
            FIN = (byte)((transHeader >> 7) & 0x01);

            TransportData = buffer;

        }

        public static byte[] TransportResponse(TransportLayer tl_, ApplicationLayer al_)
        {
            TransportLayer tl = new TransportLayer();
            tl.TransportData = al_.ApplicationData;
            tl.seq = (byte)(tl_.seq + 1);
            tl.FIN = 1;
            tl.FIR = 1;
            tl.serialize(ref tl.TransportData);
            return tl.TransportData;
        }
    }

    public enum FunctionCode : byte
    {
        /// Master sends this to an outstation to confirm the receipt of an Application Layer fragment
        CONFIRM = 0x0,
        /// Outstation shall return the data specified by the objects in the request
        READ = 0x1,
        /// Outstation shall store the data specified by the objects in the request
        WRITE = 0x2,
        /// Outstation shall select (or arm) the output points specified by the objects in the request in preparation for a subsequent operate command
        SELECT = 0x3,
        /// Outstation shall activate the output points selected (or armed) by a previous select function code command
        OPERATE = 0x4,
        /// Outstation shall immediately actuate the output points specified by the objects in the request
        DIRECT_OPERATE = 0x5,
        /// Same as DIRECT_OPERATE but outstation shall not send a response
        DIRECT_OPERATE_NR = 0x6,
        /// Outstation shall copy the point data values specified by the objects in the request to a separate freeze buffer
        IMMED_FREEZE = 0x7,
        /// Same as IMMED_FREEZE but outstation shall not send a response
        IMMED_FREEZE_NR = 0x8,
        /// Outstation shall copy the point data values specified by the objects in the request into a separate freeze buffer and then clear the values
        FREEZE_CLEAR = 0x9,
        /// Same as FREEZE_CLEAR but outstation shall not send a response
        FREEZE_CLEAR_NR = 0xA,
        /// Outstation shall copy the point data values specified by the objects in the request to a separate freeze buffer at the time and/or time intervals specified in a special time data information object
        FREEZE_AT_TIME = 0xB,
        /// Same as FREEZE_AT_TIME but outstation shall not send a response
        FREEZE_AT_TIME_NR = 0xC,
        /// Outstation shall perform a complete reset of all hardware and software in the device
        COLD_RESTART = 0xD,
        /// Outstation shall reset only portions of the device
        WARM_RESTART = 0xE,
        /// Obsolete-Do not use for new designs
        INITIALIZE_DATA = 0xF,
        /// Outstation shall place the applications specified by the objects in the request into the ready to run state
        INITIALIZE_APPLICATION = 0x10,
        /// Outstation shall start running the applications specified by the objects in the request
        START_APPLICATION = 0x11,
        /// Outstation shall stop running the applications specified by the objects in the request
        STOP_APPLICATION = 0x12,
        /// This code is deprecated-Do not use for new designs
        SAVE_CONFIGURATION = 0x13,
        /// Enables outstation to initiate unsolicited responses from points specified by the objects in the request
        ENABLE_UNSOLICITED = 0x14,
        /// Prevents outstation from initiating unsolicited responses from points specified by the objects in the request
        DISABLE_UNSOLICITED = 0x15,
        /// Outstation shall assign the events generated by the points specified by the objects in the request to one of the classes
        ASSIGN_CLASS = 0x16,
        /// Outstation shall report the time it takes to process and initiate the transmission of its response
        DELAY_MEASURE = 0x17,
        /// Outstation shall save the time when the last octet of this message is received
        RECORD_CURRENT_TIME = 0x18,
        /// Outstation shall open a file
        OPEN_FILE = 0x19,
        /// Outstation shall close a file
        CLOSE_FILE = 0x1A,
        /// Outstation shall delete a file
        DELETE_FILE = 0x1B,
        /// Outstation shall retrieve information about a file
        GET_FILE_INFO = 0x1C,
        /// Outstation shall return a file authentication key
        AUTHENTICATE_FILE = 0x1D,
        /// Outstation shall abort a file transfer operation
        ABORT_FILE = 0x1E,
        /// The master uses this function code when sending authentication requests to the outstation
        AUTH_REQUEST = 0x20,
        /// The master uses this function code when sending authentication requests to the outstation that do no require acknowledgement
        AUTH_REQUEST_NO_ACK = 0x21,
        /// Master shall interpret this fragment as an Application Layer response to an ApplicationLayer request
        RESPONSE = 0x81,
        /// Master shall interpret this fragment as an unsolicited response that was not prompted by an explicit request
        UNSOLICITED_RESPONSE = 0x82,
        /// The outstation uses this function code to issue authentication messages to the master
        AUTH_RESPONSE = 0x83,
        /// Unknown function code. Used internally in opendnp3 to indicate the code didn't match anything known
        UNKNOWN = 0xFF
    }

    public class ApplicationLayer
    {
        // List of header fields and data
        public byte ApplicationControl; // take a default c2 FIR =1,FIN =2, Seq =2
        public byte FunctionCode; // take READ as example 0x01

        public byte FIR;
        public byte FIN;
        public byte CON;
        public byte UNS;
        public byte seq;

        // for response
        public UInt16 InternalIndications;

        public byte[] ApplicationData; // READ Binary Input Change (02) Default Variation (00) Prefix Code(0) RangeCode(6)

        public byte GetControlByte()
        {
            byte ctrlByte = 0;
            ctrlByte = (byte)(ctrlByte + (FIN << 7));
            ctrlByte = (byte)(ctrlByte + (FIR << 6));
            ctrlByte = (byte)(ctrlByte + (CON << 5));
            ctrlByte = (byte)(ctrlByte + (UNS << 4));
            ctrlByte = (byte)(ctrlByte + seq);
            return ctrlByte;

        }

        public void serialize(ref byte[] buffer)
        {
            //Add Header
            AddHeader(ref buffer);
        }

        public void deserialize(ref byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return;
            // Extract Header
            ApplicationControl = UsefulMethods.RemoveHeader(ref buffer, 0);
            FunctionCode = UsefulMethods.RemoveHeader(ref buffer, 0);

            // Set the data
            ApplicationData = buffer;
        }

        public static void ProcessApplicationHeader(ref ApplicationLayer al_)
        {

            // application code
            al_.FIR = (byte)((al_.ApplicationControl & 0x80) >> 7);
            al_.FIN = (byte)((al_.ApplicationControl & 0x40) >> 6);
            al_.CON = (byte)((al_.ApplicationControl & 0x20) >> 5);
            al_.UNS = (byte)((al_.ApplicationControl & 0x10) >> 4);
            al_.seq = (byte)(al_.ApplicationControl & 0x0F);
        }

        public static void ModifyApplicationHeader(ref ApplicationLayer al_)
        {
            // function code
            if (al_.FunctionCode == (byte)DNP3Simple.FunctionCode.READ)
                al_.FunctionCode = (byte)DNP3Simple.FunctionCode.RESPONSE;
            if (al_.FunctionCode == (byte)DNP3Simple.FunctionCode.WRITE)
                al_.FunctionCode = (byte)DNP3Simple.FunctionCode.RESPONSE;
            if (al_.FunctionCode == (byte)DNP3Simple.FunctionCode.OPERATE)
                al_.FunctionCode = (byte)DNP3Simple.FunctionCode.RESPONSE;
            if (al_.FunctionCode == (byte)DNP3Simple.FunctionCode.DIRECT_OPERATE)
                al_.FunctionCode = (byte)DNP3Simple.FunctionCode.RESPONSE;
            if (al_.FunctionCode == (byte)DNP3Simple.FunctionCode.DIRECT_OPERATE_NR)
                al_.FunctionCode = (byte)DNP3Simple.FunctionCode.UNKNOWN;

            // upgrade the control byte based on the rules, currently keep dummy
            // application code
            al_.FIR = 1;
            al_.FIN = 1;
            al_.CON = 1;
            al_.UNS = 0;
            al_.seq = 1;

            al_.ApplicationControl = al_.GetControlByte();

        }

        public static byte[] ApplicationResponse(ApplicationLayer al_)
        {
            ProcessApplicationHeader(ref al_);

            ModifyApplicationHeader(ref al_);

            // we will send the DNP3 request
            ApplicationLayer al = new ApplicationLayer();
            al.InternalIndications = 0x8000; // Device Restart for READ binary input change REQUEST
            byte[] data = new byte[] { 0x00, 0x00 };
            data[1] = (byte)(al.InternalIndications & 0xFF);
            data[0] = (byte)((al.InternalIndications >> 8) & 0xFF);
            al.ApplicationData = data;
            al.FunctionCode = al_.FunctionCode;
            al.ApplicationControl = al_.ApplicationControl;
            al.serialize(ref al.ApplicationData);
            return al.ApplicationData;
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
            if (buffer == null || buffer.Length == 0) return 0;
            var x = buffer.ToList();
            var ret = buffer[0];
            x.RemoveAt(position);
            buffer = x.ToArray();
            return ret;
        }

        public static void InsertElementArray(ref byte[] buffer, UInt16 position, byte item)
        {
            if (buffer == null || buffer.Length == 0) return;
            var x = buffer.ToList();
            x.Insert(position, item);
            buffer = x.ToArray();
        }
    }
}
