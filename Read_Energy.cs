using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO.Ports;
using System.Threading;
using System.IO;
using ReadEnergy.Class_Display;

namespace ReadEnergy
{
    
    public partial class MainForm : Form
    {
        #region Create Port
        public Thread ReadSerialDataThead;
        SerialPort serialportIn = new SerialPort();
        #endregion

        public MainForm()
        {
            InitializeComponent();
            LoadMainFrom();
        }

        #region LoadMainForm
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadConfigurationSetting();
            
        }
        #endregion

        #region Get Comport DropDown
        private void txtComport_DropDown(object sender, EventArgs e)
        {
            Get_comport();
        }
        #endregion

        #region Edit button
        private void btnEdit_Click(object sender, EventArgs e)
        {
            LoadEdit();
        }
        #endregion

        #region Save button
        private void btnSave_Click(object sender, EventArgs e)
        {
            Save_btn();  
        }
        #endregion

        #region Connect button
        private void btnConnect_Click(object sender, EventArgs e)
        {
            Get_port_from_GUI();
        }
        #endregion

        #region Disconnect Button
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            LoadDisconnect();
        }

        #endregion

        #region Read button
        private void btnRead_Click(object sender, EventArgs e)
        {
            btnRead.Enabled = false;
            Read_Write_SerialPort();
            
        }
        #endregion

        #region Method Function

        #region MainForm
        private void LoadMainFrom() 
        {
            gbConnect.Enabled = false;
            btnEdit.Enabled = true;
            btnSave.Enabled = false;
            btnConnect.Enabled = false;
            btnRead.Enabled = false;
            btnDisconnect.Enabled = false;
        }
        #endregion

        #region Get Comport
        private void Get_comport()
        {
            txtComport.Items.Clear();
            string[] comport = SerialPort.GetPortNames();
            txtComport.Sorted = true;
            foreach (string com in comport)
            {
                txtComport.Items.Add(com);
            }
        }
        #endregion

        #region LoadEdit
        private void LoadEdit() 
        {
            gbConnect.Enabled = true;
            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnConnect.Enabled = false;
            btnRead.Enabled = false;
            btnDisconnect.Enabled = false;
            txtSlaveID.Enabled = true;
        }
        #endregion

        #region Config FIle
        private void LoadConfigurationSetting()
        {
            txtSlaveID.Text = ConfigurationManager.AppSettings["cfgSlaveID"];
            txtProtocol.Text = ConfigurationManager.AppSettings["cfgProtocol"];
            txtComport.Text = ConfigurationManager.AppSettings["cfgComport"];
            txtBaudrate.Text = ConfigurationManager.AppSettings["cfgBaudrate"];
            txtParity.Text = ConfigurationManager.AppSettings["cfgParity"];
            txtDataBit.Text = ConfigurationManager.AppSettings["cfgDataBit"];
            txtStopBit.Text = ConfigurationManager.AppSettings["cfgStopBit"];
        }
        #endregion

        #region Save Button
        private void Save_btn() 
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings.Remove("cfgSlaveID");
                config.AppSettings.Settings.Remove("cfgProtocol");
                config.AppSettings.Settings.Remove("cfgComport");
                config.AppSettings.Settings.Remove("cfgBaudrate");
                config.AppSettings.Settings.Remove("cfgParity");
                config.AppSettings.Settings.Remove("cfgDataBit");
                config.AppSettings.Settings.Remove("cfgStopBit");

                config.AppSettings.Settings.Add("cfgSlaveID", txtSlaveID.Text);
                config.AppSettings.Settings.Add("cfgProtocol", txtProtocol.Text);
                config.AppSettings.Settings.Add("cfgComport", txtComport.Text);
                config.AppSettings.Settings.Add("cfgBaudrate", txtBaudrate.Text);
                config.AppSettings.Settings.Add("cfgParity", txtParity.Text);
                config.AppSettings.Settings.Add("cfgDataBit", txtDataBit.Text);
                config.AppSettings.Settings.Add("cfgStopBit", txtStopBit.Text);

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                gbConnect.Enabled = false;
                btnEdit.Enabled = true;
                btnSave.Enabled = false;
                btnConnect.Enabled = true;
                btnRead.Enabled = false;
                btnDisconnect.Enabled = false;
                txtSlaveID.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Saving Error" + ex.Message);
            }
        }
        #endregion

        #region Get port from GUI
        private void Get_port_from_GUI() 
        {
            //Get port from GUI
            serialportIn.PortName = txtComport.Text;
            serialportIn.BaudRate = int.Parse(txtBaudrate.Text); //convert to int32
            serialportIn.Parity = (Parity)Enum.Parse(typeof(Parity), txtParity.Text);
            serialportIn.DataBits = int.Parse(txtDataBit.Text);
            serialportIn.StopBits = (StopBits)Enum.Parse(typeof(StopBits), txtStopBit.Text);
            serialportIn.Encoding = Encoding.GetEncoding("Windows-1252");
            serialportIn.Open();

            if (serialportIn.IsOpen)
            {
                btnEdit.Enabled = false;
                btnSave.Enabled = false;
                btnConnect.Enabled = false;
                btnRead.Enabled = true;
                btnDisconnect.Enabled = true;
                txtSlaveID.Enabled = false;
            }
        }
        #endregion

        #region Disconnect
        private void LoadDisconnect()
        {
            gbConnect.Enabled = false;
            btnEdit.Enabled = true;
            btnSave.Enabled = false;
            btnConnect.Enabled = true;
            btnRead.Enabled = false;
            btnDisconnect.Enabled = false;
            txtSlaveID.Enabled = false;
            serialportIn.Close();
            SendPackagebox.Clear();
            ReceivePackage.Clear();
            CRCSend.Clear();
            CRCReceive.Clear();
            txtVph1.Clear();
            txtVph2.Clear();
            txtVph3.Clear();
            txtIph1.Clear();
            txtIph2.Clear();
            txtIph3.Clear();
            EnergyReceive.ResetText();
        }
        #endregion

        #region Read_Write_SerialPort
        private void Read_Write_SerialPort()
        {
            #region RTU
            if (txtProtocol.Text == "RTU")
            {
                if (serialportIn.IsOpen)
                {
                    //input
                    Communication rtu = new Communication(Convert.ToByte(txtSlaveID.Text), 3, 0xFFFF, 4); //0xFFFF repleas by register data
                    string logDirectory = "F:/VS_Code/ReadEnergy_SMW/ReadEnergy_SMW/log/";
                    if (!Directory.Exists(logDirectory)) { Directory.CreateDirectory(logDirectory); }
                    string DateTime_Send = DateTime.Now.ToString("yyyyMMdd");
                    //Write 
                    serialportIn.Write(rtu._frame_send_RTU, 0, rtu._frame_send_RTU.Length); 
                    using (StreamWriter writer = new StreamWriter($"{logDirectory}\\log_{DateTime_Send}.txt",true)) 
                    {
                        DateTime_Send = DateTime.Now.ToString("yyyyMMdd");
                        string Bit2String_S = BitConverter.ToString(rtu._frame_send_RTU);
                        writer.WriteLine($"[{DateTime_Send}]-> {Bit2String_S}");

                    }
                    Thread.Sleep(500);
                    Display_spWrite spWrite = new Display_spWrite(rtu._frame_send_RTU, txtProtocol.Text);
                    SendPackagebox.Text = spWrite.SendPackagebox;
                    CRCSend.Text = spWrite.CRCSend;
                    if (serialportIn.BytesToRead >= 5)
                    {
                        byte[] frame_receive_RTU = new byte[serialportIn.BytesToRead];
                        string DateTime_receive = DateTime.Now.ToString("yyyyMMdd");
                        //Read
                        serialportIn.Read(frame_receive_RTU, 0, frame_receive_RTU.Length); 
                        using (StreamWriter writer = new StreamWriter($"{logDirectory}\\log_{DateTime_receive}.txt",true))
                        {
                            DateTime_Send = DateTime.Now.ToString("yyyyMMdd");
                            string Bit2String_S = BitConverter.ToString(frame_receive_RTU);
                            writer.WriteLine($"[{DateTime_receive}]-> {Bit2String_S}");

                        }
                        Display_spRead spRead = new Display_spRead(frame_receive_RTU, txtProtocol.Text);
                        EnergyReceive.Text = spRead.EnergyReceive;
                        ReceivePackage.Text = spRead.ReceivePackage;
                        CRCReceive.Text = spRead.CRCReceive;
                        Thread.Sleep(500);
                        serialportIn.Close();
                    }
                    serialportIn.Close();
                }
                serialportIn.Open();
                if (serialportIn.IsOpen)
                {
                    Communication rtu = new Communication(Convert.ToByte(txtSlaveID.Text), 3, 0xFFFF, 6); //replace by register data
                    //Write 
                    serialportIn.Write(rtu._frame_send_RTU, 0, rtu._frame_send_RTU.Length); 
                    Thread.Sleep(300);
                    if (serialportIn.BytesToRead >= 5)
                    {
                        byte[] frame_receive_RTU = new byte[serialportIn.BytesToRead];
                        //Read
                        serialportIn.Read(frame_receive_RTU, 0, frame_receive_RTU.Length); 
                        Display_Vollt_Amp spRead = new Display_Vollt_Amp(frame_receive_RTU, txtProtocol.Text);
                        txtVph1.Text = spRead.Voltph1;
                        txtVph2.Text = spRead.Voltph2;
                        txtVph3.Text = spRead.Voltph3;
                        txtIph1.Text = spRead.Ampph1;
                        txtIph2.Text = spRead.Ampph2;
                        txtIph3.Text = spRead.Ampph3;
                        serialportIn.Close();
                    }

                }
                /*else
                { MessageBox.Show("Comport isn't open!"); serialportIn.Close(); }*/
                
            }
            #endregion
        }
        #endregion
          
    #endregion
        
    }
}

