using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadEnergy
{
    class Communication
    {
        #region Field
        private byte _MeterID;
        private byte _FunctionCode;
        private ushort _startRegister;
        private uint _Quantity;
        public byte[] _frame_send_RTU;
        #endregion

        #region Property
        public byte MeterID { get => _MeterID; set => _MeterID = value; }
        public byte FunctionCode { get => _FunctionCode; set => _FunctionCode = value; }
        public ushort StartRegister { get => _startRegister; set => _startRegister = value; }
        public uint Quantity { get => _Quantity; set => _Quantity = value; }

        #endregion

        #region Input Communication
        //Modbus-RTU
        public Communication(byte meterID, byte fuctioncode, ushort startregister, uint quantity)
        {
            MeterID = meterID;
            FunctionCode = fuctioncode;
            StartRegister = startregister;
            Quantity = quantity;
            _frame_send_RTU = this.ReadRegisterRTU(MeterID, FunctionCode, StartRegister, Quantity);
        }

        #endregion

        #region Frame format RTU
        private byte[] ReadRegisterRTU(byte MeterID, byte FunctionCode, ushort startRegister, uint Quantity)
        {
            byte[] frame = new byte[8];             //Total 8 byte
            frame[0] = MeterID;                     //SlaveID
            frame[1] = FunctionCode;                //Read 03
            frame[2] = (byte)(startRegister >> 8);  //Start Address High
            frame[3] = (byte)startRegister;         //Start Address low
            frame[4] = (byte)(Quantity >> 8);       //Qauntity of Register High          
            frame[5] = (byte)Quantity;              //Qauntity of Register Low
                                                    //CRC
            byte[] CRC = this.CalculateCRC(frame);
            frame[6] = CRC[0];
            frame[7] = CRC[1];
            return frame;
        }
        #endregion


        #region CRC
        private byte[] CalculateCRC(byte[] frame)
        {
            byte[] CRC = new byte[2];
            ushort crc2hex = 0xFFFF;
            char CRCLSB;
            for (int i = 0; i < frame.Length - 2; i++)
            {
                crc2hex = (ushort)(crc2hex ^ frame[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(crc2hex & 0x0001);
                    crc2hex = (ushort)((crc2hex >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                    {
                        crc2hex = (ushort)(crc2hex ^ 0xA001);
                    }
                }
            }
            CRC[1] = (byte)((crc2hex >> 8) & 0xFF);
            CRC[0] = (byte)(crc2hex & 0xFF);
            return CRC;
        }
        #endregion
    }
}
