using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadEnergy.Class_Display
{
    class Display_spWrite
    {
        #region Field
        private byte[] _SerialPortWrite;
        private string _SendPackage;
        private string _CRCSend;
        private string _protocol;
        #endregion

        #region Property
        public string SendPackagebox { get { return _SendPackage; } }
        public string CRCSend { get { return _CRCSend; } }
        public byte[] SerialPortWrite { get => _SerialPortWrite; set => _SerialPortWrite = value; }
        public string Protocol { get => _protocol; set => _protocol = value; }
        #endregion

        #region Process
        public Display_spWrite(byte[] SerialPortWrite,string protocol)
        {
            Protocol = protocol;
            this.SerialPortWrite = SerialPortWrite;
            if (Protocol == "RTU")
            {
                _SendPackage = this.Display_SendPackageRTU(this.SerialPortWrite);
                _CRCSend = this.CRC(this.SerialPortWrite);
            }
        }
        #endregion

        #region Display Function

        //SendPackagebox Dispaly
        private string Display_SendPackageRTU(byte[] frame)
        {
            string regist = string.Empty;
            foreach (byte data in frame)
            {
                regist = string.Format("{0:X2} {1:X2} {2:X2} {3:X2} {4:X2} {5:X2} {6:X2} {7:X2} ", frame[0], frame[1], frame[2], frame[3], frame[4], frame[5], frame[6],frame[7]);
            }
            return regist;
        }



        //CRC Display
        private string CRC(byte[] frame_byte)
        {
            string crc_print = string.Empty; //set to Read-only
            foreach (byte data in frame_byte)
            {
                crc_print = String.Format("{0:X2} {1:X2}", frame_byte[frame_byte.Length - 2], frame_byte[frame_byte.Length - 1]);
            }
            return crc_print;
        }
        #endregion
    }
}
