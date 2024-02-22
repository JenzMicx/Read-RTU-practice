using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace ReadEnergy_SMW.Class_Display
{
    class Display_Vollt_Amp
    {
        #region Field
        private byte[] _SerialPortRead;
        private string _protocol;
        private string _Voltph1;
        private string _Voltph2;
        private string _Voltph3;
        private string _Ampph1;
        private string _Ampph2;
        private string _Ampph3;
        #endregion

        #region Property
        public string Voltph1 { get { return _Voltph1; } }
        public string Voltph2 { get { return _Voltph2; } }
        public string Voltph3 { get { return _Voltph3; } }
        public string Ampph1 { get { return _Ampph1; } }
        public string Ampph2 { get { return _Ampph2; } }
        public string Ampph3 { get { return _Ampph3; } }
        public byte[] SerialPortRead { get => _SerialPortRead; set => _SerialPortRead = value; }
        public string Protocol { get => _protocol; set => _protocol = value; }
        #endregion

        #region Process
        public Display_Vollt_Amp(byte[] serialPortRead, string protocol)
        {
            SerialPortRead = serialPortRead;
            Protocol = protocol;
            if (Protocol == "RTU")
            { 
                _Ampph1 = this.Display_RTUIph1(_SerialPortRead);
                _Ampph2 = this.Display_RTUIph2(_SerialPortRead);
                _Ampph3 = this.Display_RTUIph3(_SerialPortRead);
                _Voltph1 = this.Display_RTUVph1(_SerialPortRead);
                _Voltph2 = this.Display_RTUVph2(_SerialPortRead);
                _Voltph3 = this.Display_RTUVph3(_SerialPortRead);
            }
        }
        #endregion
        
        #region Display_Modbus RTU
        private string Display_RTUVph1(byte[] frame)
        {
            string Display_Vph1 = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", frame[3], frame[4], frame[5], frame[6]);
            int Decimal = Int32.Parse(Display_Vph1, NumberStyles.HexNumber);
            double Deci = (Convert.ToDouble(Decimal)) / 100;
            return Convert.ToString(Deci) + " V";
        }

        private string Display_RTUVph2(byte[] frame)
        {
            string Display_Vph2 = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", frame[7], frame[8], frame[9], frame[10]);
            int Decimal = Int32.Parse(Display_Vph2, NumberStyles.HexNumber);
            double Deci = (Convert.ToDouble(Decimal))/ 100;
            return Convert.ToString(Deci) + " V";
        }

        private string Display_RTUVph3(byte[] frame)
        {
            string Display_Vph3 = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", frame[11], frame[12], frame[13], frame[14]);
            int Decimal = Int32.Parse(Display_Vph3, NumberStyles.HexNumber);
            double Deci = (Convert.ToDouble(Decimal)) / 100;
            return Convert.ToString(Deci) + " V";
        }

        private string Display_RTUIph1(byte[] frame)
        {
            string Display_Iph1 = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", frame[15], frame[16], frame[17], frame[18]);
            int Decimal = Int32.Parse(Display_Iph1, NumberStyles.HexNumber);
            double Deci = (Convert.ToDouble(Decimal)) / 100;
            return Convert.ToString(Deci) + " A";
        }

        private string Display_RTUIph2(byte[] frame)
        {
            string Display_Iph2 = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", frame[19], frame[20], frame[21], frame[22]);
            int Decimal = Int32.Parse(Display_Iph2, NumberStyles.HexNumber);
            double Deci = (Convert.ToDouble(Decimal)) / 100;
            return Convert.ToString(Deci) + " A";
        }

        private string Display_RTUIph3(byte[] frame)
        {
            string Display_Iph3 = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", frame[23], frame[24], frame[25], frame[26]);
            int Decimal = Int32.Parse(Display_Iph3, NumberStyles.HexNumber);
            double Deci = (Convert.ToDouble(Decimal)) / 100;
            return Convert.ToString(Deci) + " A";
        }
        #endregion

    }
}
