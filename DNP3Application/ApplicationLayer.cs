using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using DNP3Link;

namespace DNP3Application
{
    public class ApplicationLayer
    {
    }

    public class APDUBuilders
    {
        public void ReadIntegrity(APDUWrapper request, ClassField classes, byte seq)
        {
           ClassRequest(request, FunctionCode.READ, classes, seq);
        }

        public void ReadAllObjects(APDUWrapper request, GroupVariationID gvId, byte seq)
        {
            request.SetControl(new AppControlField(true, true, false, false, seq));
            request.SetFunction(FunctionCode.READ);
            //auto writer = request.GetWriter();
            //writer.WriteHeader(gvId, QualifierCode::ALL_OBJECTS);
        }

        public void ClassRequest(APDUWrapper request, FunctionCode fc, ClassField classes, byte seq)
        {

            request.SetControl(new AppControlField(true, true, false, false, seq));
	        request.SetFunction(fc);
	        //auto writer = request.GetWriter();
            //WriteClassHeaders(writer, classes);
        }

        public void DisableUnsolicited(APDUWrapper request, byte seq)
        {
            ClassRequest(request, FunctionCode.DISABLE_UNSOLICITED, ClassField.AllEventClasses(), seq);
        }

        public void EnableUnsolicited(APDUWrapper request, ClassField classes, byte seq)
        {

            ClassRequest(request, FunctionCode.ENABLE_UNSOLICITED, classes, seq);
        }

        public void ClearRestartIIN(APDUWrapper request, byte seq = 0)
        {
            request.SetFunction(FunctionCode.WRITE);
            request.SetControl(new AppControlField(true, true, false, false, seq));
            //auto writer = request.GetWriter();
            //auto iter = writer.IterateOverSingleBitfield<openpal::UInt8>(GroupVariationID(80, 1), QualifierCode::UINT8_START_STOP, static_cast<uint8_t>(IINBit::DEVICE_RESTART));
            //iter.Write(false);
        }

        public void MeasureDelay(APDUWrapper request, byte seq = 0)
        {
            request.SetFunction(FunctionCode.DELAY_MEASURE);
            var field = new AppControlField();
            request.SetControl(field.Request(seq));
        }

        public void RecordCurrentTime(APDUWrapper request, byte seq = 0)
        {
            request.SetFunction(FunctionCode.RECORD_CURRENT_TIME);
            var field = new AppControlField();
            request.SetControl(field.Request(seq));
        }

        // -------- responses -------------

        public void NullUnsolicited(APDUWrapper response, byte seq, IINField iin)
        {
            response.SetControl(new AppControlField(true, true, true, true, seq));
            response.SetFunction(FunctionCode.UNSOLICITED_RESPONSE);
            response.SetIIN(iin);
        }
    }

    // HeaderWriter class need to be written which will be consumed by the APDUBuilder class.
    public class HeaderWriter
    {

    }

    public class GroupVariationRecord
    {

    }

    public class IVariableLength
    {

    }

    public class RangeWriterIterator
    {

    }

    public class CountWriteIterator
    {

    }

    public class PrefixedWriteIterator
    {

    }

    public class BitfieldRangeWriterIterator
    {

    }

    public class DNP3Serializer
    {

    }

    public enum QualifierCode : byte
    {
        UINT8_START_STOP = 0x0,
        UINT16_START_STOP = 0x1,
        ALL_OBJECTS = 0x6,
        UINT8_CNT = 0x7,
        UINT16_CNT = 0x8,
        UINT8_CNT_UINT8_INDEX = 0x17,
        UINT16_CNT_UINT16_INDEX = 0x28,
        UINT16_FREE_FORMAT = 0x5B,
        UNDEFINED = 0xFF
    }

    public class QualifierCodeClass
    {
        public byte QualifierCodeToType(QualifierCode arg)
        {
            return (byte)(arg);
        }

        public QualifierCode QualifierCodeFromType(byte arg)
        {
            switch (arg)
            {
                case (0x0):
                    return QualifierCode.UINT8_START_STOP;
                case (0x1):
                    return QualifierCode.UINT16_START_STOP;
                case (0x6):
                    return QualifierCode.ALL_OBJECTS;
                case (0x7):
                    return QualifierCode.UINT8_CNT;
                case (0x8):
                    return QualifierCode.UINT16_CNT;
                case (0x17):
                    return QualifierCode.UINT8_CNT_UINT8_INDEX;
                case (0x28):
                    return QualifierCode.UINT16_CNT_UINT16_INDEX;
                case (0x5B):
                    return QualifierCode.UINT16_FREE_FORMAT;
                default:
                    return QualifierCode.UNDEFINED;
            }
        }

        public string QualifierCodeToString(QualifierCode arg)
        {
            switch(arg)
            {
                case(QualifierCode.UINT8_START_STOP):
                    return "8-bit start stop";
                case(QualifierCode.UINT16_START_STOP):
                    return "16-bit start stop";
                case(QualifierCode.ALL_OBJECTS):
                    return "all objects";
                case(QualifierCode.UINT8_CNT):
                    return "8-bit count";
                case(QualifierCode.UINT16_CNT):
                    return "16-bit count";
                case(QualifierCode.UINT8_CNT_UINT8_INDEX):
                    return "8-bit count and prefix";
                case(QualifierCode.UINT16_CNT_UINT16_INDEX):
                    return "16-bit count and prefix";
                case(QualifierCode.UINT16_FREE_FORMAT):
                    return "16-bit free format";
                default:
                    return "unknown";
  }
}
    }

    public class APDUHeader
    {
        public const UInt32 REQUEST_SIZE = 2;
        public const UInt32 RESPONSE_SIZE = 4;

        public AppControlField control;
        public FunctionCode function = FunctionCode.UNKNOWN;

        public APDUHeader()
        {

        }
        public APDUHeader(AppControlField field, FunctionCode code)
        {
            control = field;
            function = code;
        }

        public APDUHeader SolicitedConfirm(byte seq)
        {
            return Confirm(seq, false);
        }

        public APDUHeader UnsolicitedConfirm(byte seq)
        {
            return Confirm(seq, true);
        }

        public APDUHeader Confirm(byte seq, bool unsolicited)
        {
            APDUHeader header = new APDUHeader();
            header.function = FunctionCode.CONFIRM;
            AppControlField field = new AppControlField(true, true, false, unsolicited, seq);
            header.control = field;
            return header;
        }
    }

    public class AppResponseHeader : APDUHeader
    {
        public AppResponseHeader()
        {

        }
        public AppResponseHeader(AppControlField field, FunctionCode code, IINField iin_field)
        {
            control = field;
            function = code;
            iin = iin_field;
        }

        public IINField iin;
    }

    //public class APDUResponse : APDUWrapper
    //{

    //}

    //public class APDURequest : APDUWrapper
    //{

    //}

    public class APDUWrapper
    {
        public APDUWrapper() {
        }
        public enum APDUType
        {
            APDU_RESPONSE = 0,
            APDU_REQUEST = 1
        }
        public string buffer;
        public byte[] buffer_bytes;

        public void SetFunction(FunctionCode code)
        {
            if (buffer == null) return;
            var field = new FunctionCodeField();
            buffer_bytes[1] = field.FunctionCodeToType(code);
        }

        public FunctionCode GetFunction() 
        {
            if (buffer == null) return FunctionCode.UNKNOWN;
            var field = new FunctionCodeField();
            return field.FunctionCodeFromType(buffer_bytes[1]);
        }

        public AppControlField GetControl()
        {
            if (buffer == null) return new AppControlField();
	        return new AppControlField(buffer_bytes[0]);
        }

        public void SetControl(AppControlField control)
        {
            buffer_bytes[0] = control.ToByte();
        }

        // The IIN is only used for the APDU response from the Outstation
        public IINField GetIIN()
        {
            var field = new IINField(buffer_bytes[2], buffer_bytes[3]);
            return field;
        }

        public void SetIIN(IINField field)
        {
            buffer_bytes[2] = field.LSB;
            buffer_bytes[3] = field.MSB;
        }
    }

    public class ClassField
    {
        private byte bitfield;

        public const byte CLASS_0 = (byte)(PointClass.Class0);
        public const byte CLASS_1 = (byte)(PointClass.Class1);
        public const byte CLASS_2 = (byte)(PointClass.Class2);
        public const byte CLASS_3 = (byte)(PointClass.Class3);
        public const byte EVENT_CLASSES = CLASS_1 | CLASS_2 | CLASS_3;
        public const byte ALL_CLASSES = EVENT_CLASSES | CLASS_0;


        public ClassField()
        {
        }

        public ClassField(EventClass ec)
        {
            bitfield = (byte)(0);
            bitfield |= (byte)(ec == EventClass.EC1 ? 1 : 0);
            bitfield |= (byte)(ec == EventClass.EC2 ? 1 : 0);
            bitfield |= (byte)(ec == EventClass.EC3 ? 1 : 0);
        }

        public ClassField(bool class0, bool class1, bool class2, bool class3)
        {
            bitfield = (byte)(class0 ? ClassField.CLASS_0 : 0);
            bitfield |= (byte)(class1 ? ClassField.CLASS_1 : 0);
            bitfield |= (byte)(class2 ? ClassField.CLASS_2 : 0);
            bitfield |= (byte)(class3 ? ClassField.CLASS_3 : 0);
        }

        public ClassField(byte mask_)
        {
            bitfield = (byte)(mask_ & ALL_CLASSES);
        }

        public static ClassField AllEventClasses()
        {
            return new ClassField(EVENT_CLASSES);
        }

        public static ClassField AllClasses()
        {
            return new ClassField(ALL_CLASSES);
        }

        public static ClassField None()
        {
            return new ClassField();
        }

        public bool IsEmpty()
        {
            return bitfield == 0;
        }

        public bool Intersects(ClassField f)
        {
            return (bitfield & f.bitfield) > 0;
        }

        public ClassField OnlyEventClasses()
        {
            return new ClassField((byte)(bitfield & EVENT_CLASSES));
        }

        public void Set(PointClass pc)
        {
            bitfield |= (byte)pc;
        }

        public void Clear(ClassField cf)
        {
            bitfield &= (byte)~(cf.bitfield);
        }

        public void Set(ClassField cf)
        {
            bitfield |= (byte)cf.bitfield;
        }

        public bool HasEventType(EventClass ec)
        {
            switch (ec)
            {
                case (EventClass.EC1) :
                    return HasClass1();
                case (EventClass.EC2) :
                    return HasClass2();
                case (EventClass.EC3) :
                    return HasClass3();
                default:
                    return false;
            }
        }

        public bool HasClass0()
        {
            return (bitfield & CLASS_0) != 0;
        }
        public bool HasClass1()
        {
            return (bitfield & CLASS_1) != 0;
        }
        public bool HasClass2()
        {
            return (bitfield & CLASS_2) != 0;
        }
        public bool HasClass3()
        {
            return (bitfield & CLASS_3) != 0;
        }
        public bool HasEventClass()
        {
            return (bitfield & EVENT_CLASSES) != 0;
        } 
        public bool HasAnyClass()
        {
            return bitfield != 0;
        }
       
    }

    public enum EventType : UInt16
    {
        Binary = 0,
	    Analog = 1,
	    Counter = 2,
	    FrozenCounter = 3,
	    DoubleBitBinary = 4,
	    BinaryOutputStatus = 5,
	    AnalogOutputStatus = 6,
	    OctetString = 7
    }

    public enum EventClass : byte
    {
        EC1 = 0,
	    EC2 = 1,
	    EC3 = 2
    }

    public enum PointClass : byte
    {
        /// No event class assignment
        Class0 = 0x1,
        /// Assigned to event class 1
        Class1 = 0x2,
        /// Assigned to event class 2
        Class2 = 0x4,
        /// Assigned to event class 3
        Class3 = 0x8
    } 

    public class GroupVariationID
    {
        public byte group;
        public byte variation;
        public GroupVariationID()
        {
            group = 0xFF;
            variation = 0xFF;
        }
        public GroupVariationID(byte grp, byte varn)
        {
            group = grp;
            variation = varn;
        }
    }

    public class AnalogOutput<T>
    {
        public T value;
        public CommandStatus status;
        public AnalogOutput()
        {
        }
        public AnalogOutput(T val)
        {
            value = val;
            status = CommandStatus.SUCCESS;
        }
        public AnalogOutput(T val, CommandStatus stat)
        {
            value = val;
            status = stat;
        }
    }

    public class AnalogOutputInt16 : AnalogOutput<Int16>
   {
        public AnalogOutputInt16() { }

        public AnalogOutputInt16(Int16 val ) { value = val; }

        public AnalogOutputInt16(Int16 val, CommandStatus stat) { value = val; status = stat; }

        public bool checksEqual(AnalogOutputInt16 x)
        {
            return x.value == value && x.status == status;
        }

    }

    public class AnalogOutputInt32 : AnalogOutput<Int32>
    {
        public AnalogOutputInt32() { }

        public AnalogOutputInt32(Int32 val) { value = val; }

        public AnalogOutputInt32(Int32 val, CommandStatus stat) { value = val; status = stat; }

        public bool checksEqual(AnalogOutputInt32 x)
        {
            return x.value == value && x.status == status;
        }

    }

    public class AnalogOutputFloat32 : AnalogOutput<float>
    {
        public AnalogOutputFloat32() { }

        public AnalogOutputFloat32(float val) { value = val; }

        public AnalogOutputFloat32(float val, CommandStatus stat) { value = val; status = stat; }

        public bool checksEqual(AnalogOutputFloat32 x)
        {
            return x.value == value && x.status == status;
        }

    }

    public class AnalogOutputDouble64 : AnalogOutput<double>
    {
        public AnalogOutputDouble64() { }

        public AnalogOutputDouble64(double val) { value = val; }

        public AnalogOutputDouble64(double val, CommandStatus stat) { value = val; status = stat; }

        public bool checksEqual(AnalogOutputDouble64 x)
        {
            return x.value == value && x.status == status;
        }

    }

    public class AnalogCommandEvent
    {
        public AnalogCommandEvent() { }

        public AnalogCommandEvent(double val, CommandStatus stat)
        {
            value = val;
            status = stat;
        }
                
        //public AnalogCommandEvent(double value, CommandStatus status, DNPTime t)
        //{
        //    value = val;
        //    status = stat;
        //    time = t;
        //}
        //public DNPTime time;
        public double value;
        public CommandStatus status;
        
        public bool Check(AnalogCommandEvent ag_event)
        {
            return (value == ag_event.value) && (status == ag_event.status);
            //return (value == ag_event.value) && (status == ag_event.status) && (time == ag_event.time);
        }
    }

    public class BinaryCommandEvent
    {
        public bool value;
        public CommandStatus status;
        public Flags flags;
        public BinaryCommandEvent() { }

        public BinaryCommandEvent(Flags f)
        {
            flags = f;
        }
        public BinaryCommandEvent(bool val, CommandStatus stat)
        {
            value = val;
            status = stat;
        }

        private const byte ValueMask = 0x80;
        private const byte StatusMask = 0x7F;

        private static bool GetValueFromFlags(Flags flags)
        {
            return (flags.value & ValueMask) == ValueMask;
        }
        private static CommandStatus GetStatusFromFlags(Flags flags)
        {
            var status = new CommandStatusClass();
            return status.CommandStatusFromType((byte)(flags.value & StatusMask));
        }
    }

    public class Flags
    {
        public byte value;
        public Flags( byte f)
        {
            value = f;
        }
    }

    public enum CommandStatus : byte
    {
        /// command was accepted, initiated, or queued
        SUCCESS = 0,
        /// command timed out before completing
        TIMEOUT = 1,
        /// command requires being selected before operate, configuration issue
        NO_SELECT = 2,
        /// bad control code or timing values
        FORMAT_ERROR = 3,
        /// command is not implemented
        NOT_SUPPORTED = 4,
        /// command is all ready in progress or its all ready in that mode
        ALREADY_ACTIVE = 5,
        /// something is stopping the command, often a local/remote interlock
        HARDWARE_ERROR = 6,
        /// the function governed by the control is in local only control
        LOCAL = 7,
        /// the command has been done too often and has been throttled
        TOO_MANY_OPS = 8,
        /// the command was rejected because the device denied it or an RTU intercepted it
        NOT_AUTHORIZED = 9,
        /// command not accepted because it was prevented or inhibited by a local automation process, such as interlocking logic or synchrocheck
        AUTOMATION_INHIBIT = 10,
        /// command not accepted because the device cannot process any more activities than are presently in progress
        PROCESSING_LIMITED = 11,
        /// command not accepted because the value is outside the acceptable range permitted for this point
        OUT_OF_RANGE = 12,
        /// command not accepted because the outstation is forwarding the request to another downstream device which reported LOCAL
        DOWNSTREAM_LOCAL = 13,
        /// command not accepted because the outstation has already completed the requested operation
        ALREADY_COMPLETE = 14,
        /// command not accepted because the requested function is specifically blocked at the outstation
        BLOCKED = 15,
        /// command not accepted because the operation was cancelled
        CANCELLED = 16,
        /// command not accepted because another master is communicating with the outstation and has exclusive rights to operate this control point
        BLOCKED_OTHER_MASTER = 17,
        /// command not accepted because the outstation is forwarding the request to another downstream device which cannot be reached or is otherwise incapable of performing the request
        DOWNSTREAM_FAIL = 18,
        /// (deprecated) indicates the outstation shall not issue or perform the control operation
        NON_PARTICIPATING = 126,
        /// 10 to 126 are currently reserved
        UNDEFINED = 127
    }
    public class CommandStatusClass
    {
        public byte CommandStatusToType(CommandStatus arg)
        {
            return (byte)(arg);
        }
        public CommandStatus CommandStatusFromType(byte arg)
        {
            switch (arg)
            {
                case (0):
                    return CommandStatus.SUCCESS;
                case (1):
                    return CommandStatus.TIMEOUT;
                case (2):
                    return CommandStatus.NO_SELECT;
                case (3):
                    return CommandStatus.FORMAT_ERROR;
                case (4):
                    return CommandStatus.NOT_SUPPORTED;
                case (5):
                    return CommandStatus.ALREADY_ACTIVE;
                case (6):
                    return CommandStatus.HARDWARE_ERROR;
                case (7):
                    return CommandStatus.LOCAL;
                case (8):
                    return CommandStatus.TOO_MANY_OPS;
                case (9):
                    return CommandStatus.NOT_AUTHORIZED;
                case (10):
                    return CommandStatus.AUTOMATION_INHIBIT;
                case (11):
                    return CommandStatus.PROCESSING_LIMITED;
                case (12):
                    return CommandStatus.OUT_OF_RANGE;
                case (13):
                    return CommandStatus.DOWNSTREAM_LOCAL;
                case (14):
                    return CommandStatus.ALREADY_COMPLETE;
                case (15):
                    return CommandStatus.BLOCKED;
                case (16):
                    return CommandStatus.CANCELLED;
                case (17):
                    return CommandStatus.BLOCKED_OTHER_MASTER;
                case (18):
                    return CommandStatus.DOWNSTREAM_FAIL;
                case (126):
                    return CommandStatus.NON_PARTICIPATING;
                default:
                    return CommandStatus.UNDEFINED;
            }
        }

        public string CommandStatusToString(CommandStatus arg)
        {
            switch (arg)
            {
                case (CommandStatus.SUCCESS):
                    return "SUCCESS";
                case (CommandStatus.TIMEOUT):
                    return "TIMEOUT";
                case (CommandStatus.NO_SELECT):
                    return "NO_SELECT";
                case (CommandStatus.FORMAT_ERROR):
                    return "FORMAT_ERROR";
                case (CommandStatus.NOT_SUPPORTED):
                    return "NOT_SUPPORTED";
                case (CommandStatus.ALREADY_ACTIVE):
                    return "ALREADY_ACTIVE";
                case (CommandStatus.HARDWARE_ERROR):
                    return "HARDWARE_ERROR";
                case (CommandStatus.LOCAL):
                    return "LOCAL";
                case (CommandStatus.TOO_MANY_OPS):
                    return "TOO_MANY_OPS";
                case (CommandStatus.NOT_AUTHORIZED):
                    return "NOT_AUTHORIZED";
                case (CommandStatus.AUTOMATION_INHIBIT):
                    return "AUTOMATION_INHIBIT";
                case (CommandStatus.PROCESSING_LIMITED):
                    return "PROCESSING_LIMITED";
                case (CommandStatus.OUT_OF_RANGE):
                    return "OUT_OF_RANGE";
                case (CommandStatus.DOWNSTREAM_LOCAL):
                    return "DOWNSTREAM_LOCAL";
                case (CommandStatus.ALREADY_COMPLETE):
                    return "ALREADY_COMPLETE";
                case (CommandStatus.BLOCKED):
                    return "BLOCKED";
                case (CommandStatus.CANCELLED):
                    return "CANCELLED";
                case (CommandStatus.BLOCKED_OTHER_MASTER):
                    return "BLOCKED_OTHER_MASTER";
                case (CommandStatus.DOWNSTREAM_FAIL):
                    return "DOWNSTREAM_FAIL";
                case (CommandStatus.NON_PARTICIPATING):
                    return "NON_PARTICIPATING";
                default:
                    return "UNDEFINED";
            }
        }
    }

    public class AppControlField
    {
        public bool FIR = true;
        public bool FIN = true;
        public bool CON = false;
        public bool UNS = false;
        public byte SEQ = 0;

        private const byte FIR_MASK = 0x80;
        private const byte FIN_MASK = 0x40;
        private const byte CON_MASK = 0x20;
        private const byte UNS_MASK = 0x10;
        private const byte SEQ_MASK = 0x0F;

        public AppControlField(){}
        public AppControlField(bool fir, bool fin, bool con, bool uns, byte seq = 0)
        {
            FIR = fir;
            FIN = fin;
            CON = con;
            UNS = uns;
            SEQ = seq;
        }

        public AppControlField(byte seq)
        {
            FIR = (seq & FIR_MASK) != 0;
            FIN = (seq & FIN_MASK) != 0;
            CON = (seq & CON_MASK) != 0;
            UNS = (seq & UNS_MASK) != 0;
            SEQ = (byte)(seq & SEQ_MASK);
        }

        public bool IsFinAndFir()
        {
            return FIR && FIN;
        }

        public AppControlField Request(byte seq)
        {
            AppControlField field = new AppControlField(true, true, false, false, seq);
            return field;
        }

        public byte ToByte()
        {
            byte ret = 0;
            if (FIR) { ret |= FIR_MASK; }
            if (FIN) { ret |= FIN_MASK; }
            if (CON) { ret |= CON_MASK; }
            if (UNS) { ret |= UNS_MASK; }
            byte seq = (byte)(SEQ % 16);
            return (byte)(ret | seq);
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
    public class FunctionCodeField
    {
        public byte FunctionCodeToType(FunctionCode arg)
        {
            return (byte)(arg);
        }

        public FunctionCode FunctionCodeFromType(byte arg)
        {
            switch (arg)
            {
                case (0x0):
                    return FunctionCode.CONFIRM;
                case (0x1):
                    return FunctionCode.READ;
                case (0x2):
                    return FunctionCode.WRITE;
                case (0x3):
                    return FunctionCode.SELECT;
                case (0x4):
                    return FunctionCode.OPERATE;
                case (0x5):
                    return FunctionCode.DIRECT_OPERATE;
                case (0x6):
                    return FunctionCode.DIRECT_OPERATE_NR;
                case (0x7):
                    return FunctionCode.IMMED_FREEZE;
                case (0x8):
                    return FunctionCode.IMMED_FREEZE_NR;
                case (0x9):
                    return FunctionCode.FREEZE_CLEAR;
                case (0xA):
                    return FunctionCode.FREEZE_CLEAR_NR;
                case (0xB):
                    return FunctionCode.FREEZE_AT_TIME;
                case (0xC):
                    return FunctionCode.FREEZE_AT_TIME_NR;
                case (0xD):
                    return FunctionCode.COLD_RESTART;
                case (0xE):
                    return FunctionCode.WARM_RESTART;
                case (0xF):
                    return FunctionCode.INITIALIZE_DATA;
                case (0x10):
                    return FunctionCode.INITIALIZE_APPLICATION;
                case (0x11):
                    return FunctionCode.START_APPLICATION;
                case (0x12):
                    return FunctionCode.STOP_APPLICATION;
                case (0x13):
                    return FunctionCode.SAVE_CONFIGURATION;
                case (0x14):
                    return FunctionCode.ENABLE_UNSOLICITED;
                case (0x15):
                    return FunctionCode.DISABLE_UNSOLICITED;
                case (0x16):
                    return FunctionCode.ASSIGN_CLASS;
                case (0x17):
                    return FunctionCode.DELAY_MEASURE;
                case (0x18):
                    return FunctionCode.RECORD_CURRENT_TIME;
                case (0x19):
                    return FunctionCode.OPEN_FILE;
                case (0x1A):
                    return FunctionCode.CLOSE_FILE;
                case (0x1B):
                    return FunctionCode.DELETE_FILE;
                case (0x1C):
                    return FunctionCode.GET_FILE_INFO;
                case (0x1D):
                    return FunctionCode.AUTHENTICATE_FILE;
                case (0x1E):
                    return FunctionCode.ABORT_FILE;
                case (0x20):
                    return FunctionCode.AUTH_REQUEST;
                case (0x21):
                    return FunctionCode.AUTH_REQUEST_NO_ACK;
                case (0x81):
                    return FunctionCode.RESPONSE;
                case (0x82):
                    return FunctionCode.UNSOLICITED_RESPONSE;
                case (0x83):
                    return FunctionCode.AUTH_RESPONSE;
                default:
                    return FunctionCode.UNKNOWN;
            }
        }

        public string FunctionCodeToString(byte arg)
        {
            switch (arg)
            {
                case ((byte)FunctionCode.CONFIRM):
                    return "CONFIRM";
                case ((byte)FunctionCode.READ):
                    return "READ";
                case ((byte)FunctionCode.WRITE):
                    return "WRITE";
                case ((byte)FunctionCode.SELECT):
                    return "SELECT";
                case ((byte)FunctionCode.OPERATE):
                    return "OPERATE";
                case ((byte)FunctionCode.DIRECT_OPERATE):
                    return "DIRECT_OPERATE";
                case ((byte)FunctionCode.DIRECT_OPERATE_NR):
                    return "DIRECT_OPERATE_NR";
                case ((byte)FunctionCode.IMMED_FREEZE):
                    return "IMMED_FREEZE";
                case ((byte)FunctionCode.IMMED_FREEZE_NR):
                    return "IMMED_FREEZE_NR";
                case ((byte)FunctionCode.FREEZE_CLEAR):
                    return "FREEZE_CLEAR";
                case ((byte)FunctionCode.FREEZE_CLEAR_NR):
                    return "FREEZE_CLEAR_NR";
                case ((byte)FunctionCode.FREEZE_AT_TIME):
                    return "FREEZE_AT_TIME";
                case ((byte)FunctionCode.FREEZE_AT_TIME_NR):
                    return "FREEZE_AT_TIME_NR";
                case ((byte)FunctionCode.COLD_RESTART):
                    return "COLD_RESTART";
                case ((byte)FunctionCode.WARM_RESTART):
                    return "WARM_RESTART";
                case ((byte)FunctionCode.INITIALIZE_DATA):
                    return "INITIALIZE_DATA";
                case ((byte)FunctionCode.INITIALIZE_APPLICATION):
                    return "INITIALIZE_APPLICATION";
                case ((byte)FunctionCode.START_APPLICATION):
                    return "START_APPLICATION";
                case ((byte)FunctionCode.STOP_APPLICATION):
                    return "STOP_APPLICATION";
                case ((byte)FunctionCode.SAVE_CONFIGURATION):
                    return "SAVE_CONFIGURATION";
                case ((byte)FunctionCode.ENABLE_UNSOLICITED):
                    return "ENABLE_UNSOLICITED";
                case ((byte)FunctionCode.DISABLE_UNSOLICITED):
                    return "DISABLE_UNSOLICITED";
                case ((byte)FunctionCode.ASSIGN_CLASS):
                    return "ASSIGN_CLASS";
                case ((byte)FunctionCode.DELAY_MEASURE):
                    return "DELAY_MEASURE";
                case ((byte)FunctionCode.RECORD_CURRENT_TIME):
                    return "RECORD_CURRENT_TIME";
                case ((byte)FunctionCode.OPEN_FILE):
                    return "OPEN_FILE";
                case ((byte)FunctionCode.CLOSE_FILE):
                    return "CLOSE_FILE";
                case ((byte)FunctionCode.DELETE_FILE):
                    return "DELETE_FILE";
                case ((byte)FunctionCode.GET_FILE_INFO):
                    return "GET_FILE_INFO";
                case ((byte)FunctionCode.AUTHENTICATE_FILE):
                    return "AUTHENTICATE_FILE";
                case ((byte)FunctionCode.ABORT_FILE):
                    return "ABORT_FILE";
                case ((byte)FunctionCode.AUTH_REQUEST):
                    return "AUTH_REQUEST";
                case ((byte)FunctionCode.AUTH_REQUEST_NO_ACK):
                    return "AUTH_REQUEST_NO_ACK";
                case ((byte)FunctionCode.RESPONSE):
                    return "RESPONSE";
                case ((byte)FunctionCode.UNSOLICITED_RESPONSE):
                    return "UNSOLICITED_RESPONSE";
                case ((byte)FunctionCode.AUTH_RESPONSE):
                    return "AUTH_RESPONSE";
                default:
                    return "UNKNOWN";
            }
        }

        public bool IsNoAckFunctionCode(FunctionCode code)
        {
            switch (code)
            {
                case (FunctionCode.DIRECT_OPERATE_NR) :
                case (FunctionCode.IMMED_FREEZE_NR) :
                case (FunctionCode.FREEZE_AT_TIME_NR) :
                case (FunctionCode.FREEZE_CLEAR_NR) :
                    return true;
                default:
                    return false;
            }
        }
    }

    public enum IINBit
    {
        ALL_STATIONS = 0,
	    CLASS1_EVENTS,
	    CLASS2_EVENTS,
	    CLASS3_EVENTS,
	    NEED_TIME,
	    LOCAL_CONTROL,
	    DEVICE_TROUBLE,
	    DEVICE_RESTART,
	    FUNC_NOT_SUPPORTED,
	    OBJECT_UNKNOWN,
	    PARAM_ERROR,
	    EVENT_BUFFER_OVERFLOW,
	    ALREADY_EXECUTING,
	    CONFIG_CORRUPT,
	    RESERVED1,
	    RESERVED2 = 15
    }
    /** DNP3 two-byte IIN field.*/
    public class IINField
    {
        private enum LSBMask : byte
        {
            ALLSTATIONS = 0x01,
            CLASS1_EVENTS = 0x02,
            CLASS2_EVENTS = 0x04,
            CLASS3_EVENTS = 0x08,
            NEED_TIME = 0x10,
            LOCAL_CONTROL = 0x20,
            DEVICE_TROUBLE = 0x40,
            DEVICE_RESTART = 0x80,
        }
        private enum MSBMask: byte
        {
            FUNC_NOT_SUPPORTED = 0x01,
            OBJECT_UNKNOWN = 0x02,
            PARAM_ERROR = 0x04,
            EVENT_BUFFER_OVERFLOW = 0x08,
            ALREADY_EXECUTING = 0x10,
            CONFIG_CORRUPT = 0x20,
            RESERVED1 = 0x40,
            RESERVED2 = 0x80,

            //special mask that indicates if there was any error processing a request
            REQUEST_ERROR_MASK = FUNC_NOT_SUPPORTED | OBJECT_UNKNOWN | PARAM_ERROR
        }
        public byte LSB;
        public byte MSB;

        public IINField()
        {

        }
        public IINField(byte lsb, byte msb)
        {
            LSB = lsb;
            MSB = msb;
        }
        public bool IsSet(IINBit bit) 
        {
	    switch(bit)
	    {
	        case(IINBit.ALL_STATIONS):
		        return Get(LSBMask.ALLSTATIONS);
	        case(IINBit.CLASS1_EVENTS):
		        return Get(LSBMask.CLASS1_EVENTS);
	        case(IINBit.CLASS2_EVENTS):
		        return Get(LSBMask.CLASS2_EVENTS);
	        case(IINBit.CLASS3_EVENTS):
		        return Get(LSBMask.CLASS3_EVENTS);
	        case(IINBit.NEED_TIME):
		        return Get(LSBMask.NEED_TIME);
	        case(IINBit.LOCAL_CONTROL):
		        return Get(LSBMask.LOCAL_CONTROL);
	        case(IINBit.DEVICE_TROUBLE):
		        return Get(LSBMask.DEVICE_TROUBLE);
	        case(IINBit.DEVICE_RESTART):
		        return Get(LSBMask.DEVICE_RESTART);
	        case(IINBit.FUNC_NOT_SUPPORTED):
		        return Get(MSBMask.FUNC_NOT_SUPPORTED);
	        case(IINBit.OBJECT_UNKNOWN):
		        return Get(MSBMask.OBJECT_UNKNOWN);
	        case(IINBit.PARAM_ERROR):
		        return Get(MSBMask.PARAM_ERROR);
	        case(IINBit.EVENT_BUFFER_OVERFLOW):
		        return Get(MSBMask.EVENT_BUFFER_OVERFLOW);
	        case(IINBit.ALREADY_EXECUTING):
		        return Get(MSBMask.ALREADY_EXECUTING);
	        case(IINBit.CONFIG_CORRUPT):
		        return Get(MSBMask.CONFIG_CORRUPT);
	        case(IINBit.RESERVED1):
		        return Get(MSBMask.RESERVED1);
	        case(IINBit.RESERVED2):
		        return Get(MSBMask.RESERVED2);
	        default:
		        return false;
	    }
     }

        public void SetBit(IINBit bit)
        {
            switch (bit)
            {
                case (IINBit.ALL_STATIONS):
                    Set(LSBMask.ALLSTATIONS);
                    break;
                case (IINBit.CLASS1_EVENTS):
                    Set(LSBMask.CLASS1_EVENTS);
                    break;
                case (IINBit.CLASS2_EVENTS):
                    Set(LSBMask.CLASS2_EVENTS);
                    break;
                case (IINBit.CLASS3_EVENTS):
                    Set(LSBMask.CLASS3_EVENTS);
                    break;
                case (IINBit.NEED_TIME):
                    Set(LSBMask.NEED_TIME);
                    break;
                case (IINBit.LOCAL_CONTROL):
                    Set(LSBMask.LOCAL_CONTROL);
                    break;
                case (IINBit.DEVICE_TROUBLE):
                    Set(LSBMask.DEVICE_TROUBLE);
                    break;
                case (IINBit.DEVICE_RESTART):
                    Set(LSBMask.DEVICE_RESTART);
                    break;
                case (IINBit.FUNC_NOT_SUPPORTED):
                    Set(MSBMask.FUNC_NOT_SUPPORTED);
                    break;
                case (IINBit.OBJECT_UNKNOWN):
                    Set(MSBMask.OBJECT_UNKNOWN);
                    break;
                case (IINBit.PARAM_ERROR):
                    Set(MSBMask.PARAM_ERROR);
                    break;
                case (IINBit.EVENT_BUFFER_OVERFLOW):
                    Set(MSBMask.EVENT_BUFFER_OVERFLOW);
                    break;
                case (IINBit.ALREADY_EXECUTING):
                    Set(MSBMask.ALREADY_EXECUTING);
                    break;
                case (IINBit.CONFIG_CORRUPT):
                    Set(MSBMask.CONFIG_CORRUPT);
                    break;
                case (IINBit.RESERVED1):
                    Set(MSBMask.RESERVED1);
                    break;
                case (IINBit.RESERVED2):
                    Set(MSBMask.RESERVED2);
                    break;
                default:
                    break;
            }
        }

        public void SetBitToValue(IINBit bit, bool value)
        {
            if (value)
            {
                SetBit(bit);
            }
            else
            {
                ClearBit(bit);
            }
        }

        public bool CheckIINField(IINField field)
        {
            return (LSB == field.LSB) && (MSB == field.MSB);
        }
        
        public void ClearBit(IINBit bit)
        {
            switch (bit)
            {
                case (IINBit.ALL_STATIONS):
                    Clear(LSBMask.ALLSTATIONS);
                    break;
                case (IINBit.CLASS1_EVENTS):
                    Clear(LSBMask.CLASS1_EVENTS);
                    break;
                case (IINBit.CLASS2_EVENTS):
                    Clear(LSBMask.CLASS2_EVENTS);
                    break;
                case (IINBit.CLASS3_EVENTS):
                    Clear(LSBMask.CLASS3_EVENTS);
                    break;
                case (IINBit.NEED_TIME):
                    Clear(LSBMask.NEED_TIME);
                    break;
                case (IINBit.LOCAL_CONTROL):
                    Clear(LSBMask.LOCAL_CONTROL);
                    break;
                case (IINBit.DEVICE_TROUBLE):
                    Clear(LSBMask.DEVICE_TROUBLE);
                    break;
                case (IINBit.DEVICE_RESTART):
                    Clear(LSBMask.DEVICE_RESTART);
                    break;
                case (IINBit.FUNC_NOT_SUPPORTED):
                    Clear(MSBMask.FUNC_NOT_SUPPORTED);
                    break;
                case (IINBit.OBJECT_UNKNOWN):
                    Clear(MSBMask.OBJECT_UNKNOWN);
                    break;
                case (IINBit.PARAM_ERROR):
                    Clear(MSBMask.PARAM_ERROR);
                    break;
                case (IINBit.EVENT_BUFFER_OVERFLOW):
                    Clear(MSBMask.EVENT_BUFFER_OVERFLOW);
                    break;
                case (IINBit.ALREADY_EXECUTING):
                    Clear(MSBMask.ALREADY_EXECUTING);
                    break;
                case (IINBit.CONFIG_CORRUPT):
                    Clear(MSBMask.CONFIG_CORRUPT);
                    break;
                case (IINBit.RESERVED1):
                    Clear(MSBMask.RESERVED1);
                    break;
                case (IINBit.RESERVED2):
                    Clear(MSBMask.RESERVED2);
                    break;
                default:
                    break;
            }
        }

        private void Set(MSBMask bit)
        {
            MSB = (byte)(MSB | ~(byte)bit);
        }

        private void Set(LSBMask bit)
        {
            LSB = (byte)(LSB | ~(byte)bit);
        }

        private bool Get(MSBMask bit)
        {
            return (MSB & (byte)(bit)) != 0;
        }

        private bool Get(LSBMask bit)
        {
            return (LSB & (byte)(bit)) != 0;
        }

        public void Clear()
        {
            LSB = MSB = 0;
        }

        private void Clear(MSBMask bit)
        {
            MSB = (byte)(MSB & ~(byte)bit);
        }

        private void Clear(LSBMask bit)
        {
            LSB = (byte)(LSB & ~(byte)bit);
        }

    }

    public class Message
    {
        public Addresses addresses;
        public Byte[] payload;

        public Message(Addresses ad, Byte[] pl)
        {
            addresses = ad;
            payload = pl;
        }
    }
}
