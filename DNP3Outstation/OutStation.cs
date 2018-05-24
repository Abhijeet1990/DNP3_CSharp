using DNP3Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3Outstation
{
    public class OutStation
    {
    }

    public class ApplicationIIN
    {
        // flags normally controlled by the application, not the stack
        public bool needTime = false;
        public bool localControl = false;
        public bool deviceTrouble = false;
        public bool configCorrupt = false;

        // this is only for appliactions that have an additional external event buffer that can overflow
        public bool eventBufferOverflow = false;

        public ApplicationIIN() { }

        public IINField ToIIN()
        {
            IINField ret = new IINField();
            ret.SetBitToValue(IINBit.NEED_TIME, needTime);
            ret.SetBitToValue(IINBit.LOCAL_CONTROL, localControl);
            ret.SetBitToValue(IINBit.CONFIG_CORRUPT, configCorrupt);
            ret.SetBitToValue(IINBit.DEVICE_TROUBLE, deviceTrouble);
            ret.SetBitToValue(IINBit.EVENT_BUFFER_OVERFLOW, eventBufferOverflow);
            return ret;
        }
    }
}
