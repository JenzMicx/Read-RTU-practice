using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadEnergy.Class_Display
{
    class Display_spRead
    {
        #region Field
        private byte[] _SerialPortRead;
        private string _ReceivePackage;
        private string _CRCReceive;
        private string _EnergyReceive;
        private string _protocol;

        #endregion

        #region Property
        public byte[] SerialPortRead { get => _SerialPortRead; set => _SerialPortRead = value; }
        public string ReceivePackage { get { return _ReceivePackage; } }
        public string CRCReceive { get { return _CRCReceive; } }
        public string EnergyReceive { get { return _EnergyReceive; } }
        public string Protocol { get => _protocol; set => _protocol = value; }

        #endregion

        #region Process
        public Display_spRead(byte[] serialPortRead,string protocol) 
        { 
            _SerialPortRead = serialPortRead;
            Protocol = protocol;
            if(Protocol == "RTU")
            {
                _EnergyReceive = this.Energy_DisplayRTU(_SerialPortRead);
                _ReceivePackage = this.Display_ReceivePackageRTU(_SerialPortRead) + this.CRC(_SerialPortRead); 
                _CRCReceive = this.CRC(_SerialPortRead);
            }
        }
        #endregion

        #region Display Function
    
        //ReceivePackagebox Display
        private string Display_ReceivePackageRTU(byte[] frame_receive)
        {
            string regis = string.Empty;
            foreach (byte data in frame_receive)
            {
                regis = string.Format("{0:X2} {1:X2} {2:X2} {3:X2} {4:X2} {5:X2} {6:X2} ", 
                    frame_receive[0], 
                    frame_receive[1], 
                    frame_receive[2], 
                    frame_receive[3], 
                    frame_receive[4], 
                    frame_receive[5], 
                    frame_receive[6]);
            }
            return regis;
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


        //Energy Value(HEX2DEC)
        private string Energy_DisplayRTU(byte[] frame)
        {
            string Display_Energy_ModbusRTU = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", frame[3], frame[4], frame[5], frame[6]);
            int dec = Int32.Parse(Display_Energy_ModbusRTU, System.Globalization.NumberStyles.HexNumber);
            Display_Energy_ModbusRTU = Convert.ToString(dec);
            return Display_Energy_ModbusRTU + " wh";
        }
        #endregion
    }
}
