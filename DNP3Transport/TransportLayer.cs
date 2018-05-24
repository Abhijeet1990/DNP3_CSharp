using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3Transport
{
    public class TransportLayer
    {
    }

    public class TransportHeader
    {
        private const byte FIN_MASK = 0x80;
        private const byte FIR_MASK = 0x40;
        private const byte SEQ_MASK = 0x3F;
        public bool fir;
        public bool fin;
        public byte seq; // 0 to 63

        public TransportHeader(byte data)
        {
            fir = (data & FIR_MASK) != 0;
            fin = (data & FIN_MASK) != 0;
            seq = (byte)(data & SEQ_MASK);
        }

        public byte ToByte(bool fir_, bool fin_, byte seq_)
        {
            byte hdr = 0;
            if (fir_)
            {
                hdr |= FIR_MASK;
            }
            if (fin_)
            {
                hdr |= FIN_MASK;
            }
            // Only the lower 6 bits of the sequence number
            hdr |= (byte)(SEQ_MASK & seq_);
            return hdr;
        }
    }

    public class TransportConstants
    {
        /// Maximum TPDU length
        public const byte MAX_TPDU_LENGTH = 250;
        /// Maximum TPDU payload size
        public const byte MAX_TPDU_PAYLOAD = 249;
    }



}
