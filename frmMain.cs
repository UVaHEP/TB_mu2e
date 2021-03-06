﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace TB_mu2e
{

    public partial class frmMain : Form
    {
        private int _ActiveFEB = 0;
        private int reg_set = 0;
        private bool flgBreak = false;

        private string msg1Conn = "";
        private string msg2Conn = "";
        private uConsole console_label;
        private List<Button> sel_list;

        private static int num_reg = 15;
        private TextBox[] txtREGISTERS = new TextBox[num_reg];
        private static int num_spill_reg = 5;
        private TextBox[] txtSPILL = new TextBox[num_spill_reg];
        CheckBox[] chkChEnable = new CheckBox[64];
        private string[] rnames = new string[50];
        private System.Windows.Forms.Label[] lblREG = new System.Windows.Forms.Label[num_reg + num_spill_reg];
        private bool _IntegralScan = false;

        private SpillData DispSpill;
        private mu2e_Event DispEvent;
        private bool DebugLogging;
        private int[] spill_trig_num;

        public void AddConsoleMessage(string mess)
        {
            console_label.add_messg(mess);
        }

        public frmMain()
        {
            InitializeComponent();
            btnFEB1.BackColor = SystemColors.Control;
            lblMessage.Text = msg1Conn + "\n" + msg2Conn;

            btnFEB1.Click += new System.EventHandler(this.button1_Click);
            btnFEB1.Tag = PP.FEB1; btnFEB1.Text = PP.FEB1.host_name_prop;
            btnFEB2.Click += new System.EventHandler(this.button1_Click);
            btnFEB2.Tag = PP.FEB2; btnFEB2.Text = PP.FEB2.host_name_prop;

            btnWC.Click += new System.EventHandler(this.button1_Click);
            btnWC.Tag = PP.WC; btnWC.Text = PP.WC.host_name_prop;

            console_label = new uConsole();


            spill_trig_num = new int[3];
            for (int i = 0; i < 3; i++)
            {
                spill_trig_num[i] = 0;
            }


            #region Registers

            rnames[0] = "CSR";
            rnames[1] = "SDRAM_WritePointer";
            rnames[2] = "SDRAM_ReadPointer";
            rnames[3] = "TEST_PULSE_FREQ";//pipe_del
            rnames[4] = "TEST_PULSE_DURATION";//num_samp
            rnames[5] = "TEST_PULSE_INTERSPILL";//mux_ctrl
            rnames[6] = "CHAN_MASK";//trig_ctrl
            rnames[7] = "MUX_SEL";//afe_fifo
            rnames[8] = "TRIG_CONTROL";//mig_stat
            rnames[9] = "SPILL_TRIG_COUNT";//mig_fifo
            ///
            rnames[10] = "SPILL_NUMBER";
            rnames[11] = "SPILL_STATE";
            rnames[12] = "SPILL_TRIG_COUNT";
            rnames[13] = "SPILL_WORD_COUNT";
            rnames[14] = "UPTIME";

            for (int i = 0; i < num_reg; i++)
            {
                txtREGISTERS[i] = new TextBox();
                txtREGISTERS[i].Location = new System.Drawing.Point(240, 16 + 24 * i);
                txtREGISTERS[i].Size = new System.Drawing.Size(100, 15);
                txtREGISTERS[i].TextAlign = HorizontalAlignment.Right;
                txtREGISTERS[i].Tag = i;

                lblREG[i] = new System.Windows.Forms.Label();
                lblREG[i].Location = new System.Drawing.Point(1, 20 + 24 * i);
                lblREG[i].Size = new System.Drawing.Size(230, 20);
                lblREG[i].TextAlign = ContentAlignment.MiddleRight;
                lblREG[i].Text = rnames[i];

            }

            for (int k = 0; k < 2; k++)
            {
                foreach (Control c in tabControl.TabPages[2 + k].Controls)
                {
                    for (int i = 0; i < num_reg; i++)
                    {
                        if (c.Name.Contains("groupBoxREG"))
                        {
                            c.Controls.Add(txtREGISTERS[i]);
                            c.Controls.Add(lblREG[i]);
                        }
                    }
                }

            }
            for (int i = 0; i < num_spill_reg; i++)
            {
                txtSPILL[i] = new TextBox();
                txtSPILL[i].Location = new System.Drawing.Point(125, 20 * (i + 1));
                txtSPILL[i].Size = new System.Drawing.Size(120, 17);
                txtSPILL[i].TextAlign = HorizontalAlignment.Right;
                txtSPILL[i].Tag = i;
            }

            for (int k = 0; k < 2; k++)
            {
                foreach (Control c in tabControl.TabPages[2 + k].Controls)
                {
                    for (int i = 0; i < num_reg; i++)
                    {
                        if (c.Name == "groupBoxSPILL")
                        { c.Controls.Add(txtSPILL[i]); }
                    }
                }

            }
            #endregion Registers

            #region ch_select

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    chkChEnable[j * 4 + i] = new CheckBox();
                    chkChEnable[j * 4 + i].Location = new System.Drawing.Point(6 + 45 * j, 20 * i);
                    chkChEnable[j * 4 + i].Size = new System.Drawing.Size(45, 20);
                    chkChEnable[j * 4 + i].Text = (j * 4 + i + 1).ToString();
                    chkChEnable[j * 4 + i].Tag = j * 4 + i;

                }
            }
            for (int k = 0; k < 2; k++)
            {
                foreach (Control c in tabControl.TabPages[k].Controls)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        if (c.Name == "groupBoxEvDisplay")
                        { c.Controls.Add(chkChEnable[i]); }
                    }
                }

            }
            #endregion ch_select

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button mySender = (Button)sender;
            string myName = mySender.Text;
            IStenComms comm = (IStenComms)mySender.Tag;
            if (!comm.ClientOpen) { comm.DoOpen(); }
            if (!comm.ClientOpen)
            {
                //label1.Text = comm.m_prop;
                mySender.BackColor = Color.Red;
                if (comm.name.Contains("FEB1")) { btnFEB1.BackColor = Color.Red; }
                if (comm.name.Contains("FEB2")) { btnFEB2.BackColor = Color.Red; }
                if (comm.name.Contains("WC")) { btnWC.BackColor = Color.Red; }
                if (comm.name.Contains("FECC")) { lblFEB1.BackColor = Color.Red; }
            }
            else
            {
                mySender.BackColor = Color.Green;
                if (comm.name.Contains("FEB1"))
                {
                    lblFEB1.BackColor = Color.Green;
                    btnFEB1.BackColor = Color.LightSeaGreen;
                    btnFEB2.BackColor = Color.Green;
                    _ActiveFEB = 1;
                    lblActive.Text = " FEB1";
                    PP.active_Stream = PP.FEB1.stream;
                    PP.active_Socket = PP.FEB1.TNETSocket;
                    dbgFEB1.BackColor = Color.Green;
                    dbgFEB2.BackColor = Color.Gray;
                    dbgWC.BackColor = Color.Gray;
                    btnBiasREAD_Click(null, null);
                    btnRegREAD_Click(null, null);
                }
                if (comm.name.Contains("FEB2"))
                {
                    lblFEB2.BackColor = Color.Green;
                    btnFEB2.BackColor = Color.LightSeaGreen;
                    btnFEB1.BackColor = Color.Green;
                    _ActiveFEB = 2;
                    lblActive.Text = " FEB2";
                    PP.active_Stream = PP.FEB2.stream;
                    PP.active_Socket = PP.FEB2.TNETSocket;
                    dbgFEB2.BackColor = Color.Green;
                    dbgFEB1.BackColor = Color.Gray;
                    dbgWC.BackColor = Color.Gray;
                    btnBiasREAD_Click(null, null);
                    btnRegREAD_Click(null, null);
                }
                if (comm.name.Contains("WC"))
                {
                    lblWC.BackColor = Color.Green;
                    _ActiveFEB = 0;
                    lblActive.Text = "NONE";
                    PP.active_Stream = PP.WC.stream_prop;
                    PP.active_Socket = PP.WC.TNETSocket_prop;
                    dbgFEB2.BackColor = Color.Gray;
                    dbgFEB1.BackColor = Color.Gray;
                    dbgWC.BackColor = Color.Green;
                }
                if (comm.name.Contains("FECC")) { }
                lblMessage.Text = DateTime.Now + " -> " + comm.m_prop;
            }
        }

        public void select_module_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((Control.ModifierKeys & Keys.Control) == Keys.Control))
            {
                if (e.KeyChar.ToString() == " ")
                {
                    if (dbgFEB2.BackColor == Color.Green)
                    { dbgFEB_Click((object)dbgWC, null); textBox1.Text = ""; return; }
                    if (dbgFEB1.BackColor == Color.Green)
                    { dbgFEB_Click((object)dbgFEB2, null); textBox1.Text = ""; return; }
                    if (dbgWC.BackColor == Color.Green)
                    { dbgFEB_Click((object)dbgFEB1, null); textBox1.Text = ""; return; }
                }
                if (e.KeyChar.ToString() == "1")
                { dbgFEB_Click((object)dbgWC, null); }
                if (e.KeyChar.ToString() == "2")
                { dbgFEB_Click((object)dbgWC, null); }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string l = "";
            bool this_is_a_write = false;
            string tt = "";

            if (textBox1.Text.Contains("\n"))
            {
                try
                {
                    if (textBox1.Text.Contains("$")) //this is a comment
                    { }
                    else
                    {
                        byte[] buf = PP.GetBytes(textBox1.Text);
                        tt = textBox1.Text;
                        //textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 2);
                        if (textBox1.Text.ToLower().Contains("wr")) { this_is_a_write = true; }
                        textBox1.Text = "";
                        while (PP.active_Socket.Available > 0)
                        {
                            byte[] rbuf = new byte[PP.active_Socket.Available];
                            PP.active_Socket.Receive(rbuf);
                        }
                        PP.active_Socket.Send(buf);
                        System.Threading.Thread.Sleep(1);
                        int max_timeout = 50; int timeout = 0;
                        if (this_is_a_write) { max_timeout = 3; }
                        while ((PP.active_Socket.Available == 0) && (timeout < max_timeout))
                        {
                            timeout++; System.Threading.Thread.Sleep(2);
                        }
                        if (timeout < max_timeout)
                        {
                            byte[] rec_buf = new byte[PP.active_Socket.Available];
                            int ret_len = PP.active_Socket.Receive(rec_buf);
                            string t = PP.GetString(rec_buf, ret_len);
                            t = tt.Substring(0, tt.Length - 2) + ": " + t;
                            l = console_label.add_messg(t);
                        }
                        else
                        {
                            if (this_is_a_write)
                            { l = console_label.add_messg(tt); }//there is no response to a write
                            else
                            { l = console_label.add_messg("timeout!"); }
                        }
                        lblConsole_disp.Text = l;
                        Application.DoEvents();
                    }
                }
                catch
                {
                    string m = "Exception caught. Do yu have a module selected?";
                    lblConsole_disp.Text = m;
                    Application.DoEvents();
                }
            }
        }

        private void btnRegREAD_Click(object sender, EventArgs e)
        {
            Mu2e_FEB_client myFEB = null;
            int i = this.tabControl.SelectedIndex;
            if (tabControl.SelectedTab.Text.Contains("FEB"))
            {
                if (_ActiveFEB == 1)
                { myFEB = PP.FEB1; }
                else if (_ActiveFEB == 2)
                { myFEB = PP.FEB2; }
                else
                { MessageBox.Show("No FEB active"); return; }

                myFEB.checkFEB_connection();

                ushort fpga_num = Convert.ToUInt16(udFPGA.Value);
                for (int j = 0; j < num_reg; j++)
                {
                    Mu2e_Register r1;
                    Mu2e_Register.FindName(rnames[j], ref myFEB.arrReg, out r1);
                    r1.fpga_index = fpga_num;
                    Mu2e_Register.ReadReg(ref r1, ref myFEB.client);
                    if (!r1.pref_hex)
                    { txtREGISTERS[j].Text = r1.val.ToString(); }
                    else
                    { txtREGISTERS[j].Text = "0x" + Convert.ToString(r1.val, 16); }
                }
            }
        }

        private void btnRegMONITOR_Click(object sender, EventArgs e)
        {

        }

        private void btnRegWRITE_Click(object sender, EventArgs e)
        {
            Mu2e_FEB_client myFEB = null;
            int i = this.tabControl.SelectedIndex;
            if (tabControl.SelectedTab.Text.Contains("FEB"))
            {
                if (_ActiveFEB == 1)
                { myFEB = PP.FEB1; }
                if (_ActiveFEB == 2)
                { myFEB = PP.FEB2; }

                ushort fpga_num = Convert.ToUInt16(udFPGA.Value);
                for (int j = 0; j < num_reg; j++)
                {
                    Mu2e_Register r1;
                    Mu2e_Register.FindName(rnames[j], ref myFEB.arrReg, out r1);
                    r1.fpga_index = fpga_num;
                    if (txtREGISTERS[j].Text.Contains("x"))
                    {
                        try
                        {
                            UInt32 v = Convert.ToUInt32(txtREGISTERS[j].Text, 16);
                            Mu2e_Register.WriteReg(v, ref r1, ref myFEB.client);
                        }
                        catch
                        { txtREGISTERS[j].Text = "?"; }
                    }
                    else
                    {
                        try
                        {
                            UInt32 v = Convert.ToUInt32(txtREGISTERS[j].Text);
                            Mu2e_Register.WriteReg(v, ref r1, ref myFEB.client);
                        }
                        catch
                        { txtREGISTERS[j].Text = "?"; }
                    }
                }
            }
        }

        private void dbgFEB_Click(object sender, EventArgs e)
        {
            Button mySender = (Button)sender;
            string myName = mySender.Text;
            if (myName.Contains("FEB1"))
            {
                button1_Click((object)btnFEB1, e);
                lblConsole_disp.Text = console_label.add_messg("---- FEB1 ----\r\n");
            }
            if (myName.Contains("FEB2"))
            {
                button1_Click((object)btnFEB2, e);
                lblConsole_disp.Text = console_label.add_messg("---- FEB2 ----\r\n");
            }
            if (myName.Contains("WC"))
            {
                button1_Click((object)btnWC, e);
                lblConsole_disp.Text = console_label.add_messg("----  WC  ----\r\n");
            }
            if (myName.Contains("FECC")) { }
        }

        private void btnSpillREAD_Click(object sender, EventArgs e)
        {
            Mu2e_FEB_client myFEB = null;
            int i = this.tabControl.SelectedIndex;
            if (tabControl.SelectedTab.Text.Contains("FEB"))
            {
                if (_ActiveFEB == 1)
                { myFEB = PP.FEB1; }
                if (_ActiveFEB == 2)
                { myFEB = PP.FEB2; }


                ushort fpga_num = Convert.ToUInt16(udFPGA.Value);
                for (int j = 0; j < num_spill_reg; j++)
                {
                    Mu2e_Register r1;
                    Mu2e_Register.FindName(rnames[j], ref myFEB.arrReg, out r1);
                    r1.fpga_index = fpga_num;
                    Mu2e_Register.ReadReg(ref r1, ref myFEB.client);
                    if (!r1.pref_hex)
                    { txtSPILL[j].Text = r1.val.ToString(); }
                    else
                    { txtSPILL[j].Text = "0x" + Convert.ToString(r1.val, 16); }
                }
            }
        }

        private void btnSpillWRITE_Click(object sender, EventArgs e)
        {
            Mu2e_FEB_client myFEB = null;
            int i = this.tabControl.SelectedIndex;
            if (tabControl.SelectedTab.Text.Contains("FEB"))
            {
                if (_ActiveFEB == 1)
                { myFEB = PP.FEB1; }
                if (_ActiveFEB == 2)
                { myFEB = PP.FEB2; }


                ushort fpga_num = Convert.ToUInt16(udFPGA.Value);
                for (int j = 0; j < num_spill_reg; j++)
                {
                    Mu2e_Register r1;
                    Mu2e_Register.FindName(rnames[j], ref myFEB.arrReg, out r1);
                    r1.fpga_index = fpga_num;
                    if (txtSPILL[j].Text.Contains("x"))
                    {
                        try
                        {
                            UInt32 v = Convert.ToUInt32(txtREGISTERS[j].Text, 16);
                            Mu2e_Register.WriteReg(v, ref r1, ref myFEB.client);
                        }
                        catch
                        { txtSPILL[j].Text = "?"; }
                    }
                    else
                    {
                        try
                        {
                            UInt32 v = Convert.ToUInt32(txtSPILL[j].Text);
                            Mu2e_Register.WriteReg(v, ref r1, ref myFEB.client);
                        }
                        catch
                        { txtSPILL[j].Text = "?"; }
                    }
                }
            }
        }

        private void btnSpillMON_Click(object sender, EventArgs e)
        {

        }

        private void btnBiasREAD_Click(object sender, EventArgs e)
        {
            double[] cmb_temp = new double[5];
            string name = tabControl.SelectedTab.Text;
            if (name.Contains("FEB"))
            {
                //Console.WriteLine("NEW READING");
                //for (int t = 0; t < 300; t++)
                //{
                //    for (int i = 0; i < 8; i++)
                //    {
                //        double I1 = 0;
                //        int nGoodReads = 0;
                //        for (int k = 0; k < nreads.Value; k++)
                //        {
                //            double I1now = PP.FEB1.ReadA0((int)udFPGA.Value, i);
                //            if (I1now > 0)
                //            {
                //                I1 = I1 + I1now;
                //                nGoodReads++;
                //            }
                //        }
                //        Console.Write(" " + I1/nGoodReads);
                //    }
                //    Console.WriteLine("");
                //    System.Threading.Thread.Sleep(60000);
                //}

                switch (_ActiveFEB)
                {
                    case 1:
                        txtV.Text = PP.FEB1.ReadV((int)udFPGA.Value).ToString("0.000");
                        txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");

                        double I1 = 0;
                        int nGoodReads = 0;
                        for (int i = 0; i < nreads.Value; i++)
                        {
                            double I1now = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value);
                            if (I1now > 0)
                            {
                                I1 = I1 + I1now;
                                nGoodReads++;
                            }
//                            Console.WriteLine(PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value));
                        }
                        txtI.Text = (I1 / (double)(nGoodReads)).ToString("0.0000");

                        PP.FEB1.ReadTemp(out cmb_temp[1], out cmb_temp[2], out cmb_temp[3],out cmb_temp[4], (int)udFPGA.Value);
                        break;
                    case 2:
                        txtV.Text = PP.FEB2.ReadV((int)udFPGA.Value).ToString("0.000");
                        txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                        PP.FEB2.ReadTemp(out cmb_temp[1], out cmb_temp[2], out cmb_temp[3], out cmb_temp[4], (int)udFPGA.Value);
                        break;

                    default:
                        break;
                }
            }
            txtCMB_Temp1.Text = cmb_temp[1].ToString("0.0");
            txtCMB_Temp2.Text = cmb_temp[2].ToString("0.0");
            txtCMB_Temp3.Text = cmb_temp[3].ToString("0.0");
            txtCMB_Temp4.Text = cmb_temp[4].ToString("0.0");
        }

        private void btnBiasWRITE_Click(object sender, EventArgs e)
        {
            string name = tabControl.SelectedTab.Text;
            if (name.Contains("FEB"))
            {
                switch (_ActiveFEB)
                {
                    case 1:
                        PP.FEB1.SetV(Convert.ToDouble(txtV.Text), (int)udFPGA.Value);
                        txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                        break;
                    case 2:
                        PP.FEB2.SetV(Convert.ToDouble(txtV.Text));
                        txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                        break;

                    default:
                        break;
                }
            }
        }

        private void btnBIAS_MON_Click(object sender, EventArgs e)
        {

        }

        private void tabFEB2_Click(object sender, EventArgs e)
        {

        }

        private void btnScan_Click(object sender, EventArgs e)
        {


            //zedFEB1.GraphPane.CurveList.Clear();
            Mu2e_FEB_client FEB = new Mu2e_FEB_client();
            switch (_ActiveFEB)
            {
                case 1:
                    FEB = PP.FEB1;
                    break;
                case 2:
                    FEB = PP.FEB2;
                    break;
            }

            if (btnScan.Text != "SCAN")
            { flgBreak = true; return; }

            if (ShowSpect.Visible)
            {
                IV_curve myIV = new IV_curve();
                udChan_ValueChanged(null, null);
                myIV.min_v = (double)udStart.Value;
                myIV.max_v = (double)udStop.Value;
                myIV.chan = (Int32)udChan.Value;
                myIV.board = _ActiveFEB;
                decimal v = udStart.Value;
                while ((v < udStop.Value) & !flgBreak)
                {
                    txtV.Text = v.ToString("0.000");
                    btnBiasWRITE_Click(null, null);
                    Application.DoEvents();
                    double I = Convert.ToDouble(txtI.Text);
                    myIV.AddPoint((double)v, (double)I);


                    //plot
                    UpdateDisplay_IV(myIV);
                    double s = 0;

                    btnScan.Text = v.ToString("0.00") + "V";

                    v += udInterval.Value / 1000;

                    System.Threading.Thread.Sleep(10);
                    Application.DoEvents();
                }
                if (_ActiveFEB == 1) { PP.FEB1IVs.Add(myIV); }
                if (_ActiveFEB == 2) { PP.FEB2IVs.Add(myIV); }
                v = udStart.Value;
                txtV.Text = v.ToString("0.000");
                btnBiasWRITE_Click(null, null);
                if (myIV.saved == false) { myIV.Save(); }
                myIV.saved = true;
                myIV.saved_time = DateTime.Now;
            }

            if (ShowIV.Visible)
            {
                #region ShowSpect
                //create new histo
                HISTO_curve myHisto = new HISTO_curve();
                myHisto.list = new PointPairList();
                myHisto.loglist = new PointPairList();
                myHisto.created_time = DateTime.Now;
                myHisto.min_thresh = (int)udStart.Value;
                myHisto.max_thresh = (int)udStop.Value;
                myHisto.interval = (int)udInterval.Value;
                myHisto.chan = (int)udChan.Value;
                myHisto.integral = _IntegralScan;
                myHisto.board = _ActiveFEB;
                udChan_ValueChanged(null, null);
                System.Threading.Thread.Sleep(100);
                btnBIAS_MON_Click(null, null);
                Application.DoEvents();
                myHisto.V = Convert.ToDouble(txtV.Text);
                myHisto.I = Convert.ToDouble(txtI.Text);
                //Fetch registers
                int FPGA_index = (int)udFPGA.Value;
                Mu2e_Register r_mux;
                Mu2e_Register r_ch;
                Mu2e_Register r_interval;
                Mu2e_Register r_th0; //write this last- starts count : bit[12]=1 means still counting
                Mu2e_Register r_count0;
                Mu2e_Register r_th1; //write this last- starts count : bit[12]=1 means still counting
                Mu2e_Register r_count1;

                Mu2e_Register.FindAddr(0x20, ref FEB.arrReg, out r_mux);
                Mu2e_Register.FindName("HISTO_CHAN_SEL", ref FEB.arrReg, out r_ch);
                Mu2e_Register.FindName("HISTO_COUNT_INTERVAL", ref FEB.arrReg, out r_interval);
                Mu2e_Register.FindName("HISTO_THRESH_AFE0", ref FEB.arrReg, out r_th0);
                Mu2e_Register.FindName("HISTO_COUNT0", ref FEB.arrReg, out r_count0);
                Mu2e_Register.FindName("HISTO_THRESH_AFE1", ref FEB.arrReg, out r_th1);
                Mu2e_Register.FindName("HISTO_COUNT1", ref FEB.arrReg, out r_count1);

                r_ch.fpga_index = (ushort)FPGA_index;
                r_interval.fpga_index = (ushort)FPGA_index;
                r_th0.fpga_index = (ushort)FPGA_index;
                r_count0.fpga_index = (ushort)FPGA_index;
                r_th1.fpga_index = (ushort)FPGA_index;
                r_count1.fpga_index = (ushort)FPGA_index;

                //set interval & ch & stop
                UInt32 v = (UInt32)(myHisto.chan);
                if (v > 7) { v = v - 8; }
                if (_IntegralScan) { v = v + 8; }
                Mu2e_Register.WriteReg(0, ref r_mux, ref FEB.client);
                Mu2e_Register.WriteReg(v, ref r_ch, ref FEB.client);
                v = (UInt32)(myHisto.interval);
                Mu2e_Register.WriteReg(v, ref r_interval, ref FEB.client);

                myHisto.min_count = 0;

                //loop over th
                zedFEB1.GraphPane.XAxis.Scale.Max = myHisto.max_thresh;
                zedFEB1.GraphPane.XAxis.Scale.Min = myHisto.min_thresh;
                zedFEB1.GraphPane.YAxis.Scale.Max = 100;
                zedFEB1.GraphPane.YAxis.Scale.Min = 0;

                for (int i = myHisto.min_thresh; i < myHisto.max_thresh; i++)
                {
                    if (flgBreak) { i = myHisto.max_thresh; }
                    UInt32 th = (UInt32)i;
                    Mu2e_Register.WriteReg(th, ref r_th0, ref FEB.client);
                    System.Threading.Thread.Sleep(1);
                    Mu2e_Register.WriteReg(th, ref r_th1, ref FEB.client);
                    System.Threading.Thread.Sleep(2 * myHisto.interval);

                    Mu2e_Register.ReadReg(ref r_count0, ref FEB.client);
                    System.Threading.Thread.Sleep(1);
                    Mu2e_Register.ReadReg(ref r_count1, ref FEB.client);
                    UInt32 count = 0;
                    if (myHisto.chan > 7) { count = r_count1.val; }
                    else { count = r_count0.val; }

                    if (count > myHisto.max_count) { myHisto.max_count = count; }

                    myHisto.AddPoint((int)th, (int)count);
                    //myCurve.AddPoint(th, count);

                    btnScan.Text = i.ToString();
                    Application.DoEvents();

                }
                if (_ActiveFEB == 1) { PP.FEB1Histo.Add(myHisto); }
                if (_ActiveFEB == 2) { PP.FEB2Histo.Add(myHisto); }

                Application.DoEvents();
                UpdateDisplay();
                #endregion ShowSpect
            }
            btnScan.Text = "SCAN";
            flgBreak = false;
            Application.DoEvents();
        }

        private void UpdateDisplay_IV(IV_curve myIV)
        {
            zedFEB1.GraphPane.CurveList.Clear();
            if (chkLogY.Checked)
            {
                if (Math.Round((double)(Math.Log10(myIV.min_I))) < -2)
                { zedFEB1.GraphPane.YAxis.Scale.Min = -2; }
                else
                { zedFEB1.GraphPane.YAxis.Scale.Min = Math.Round((double)(Math.Log10(myIV.min_I))) - .1; }
                zedFEB1.GraphPane.YAxis.Scale.Max = Math.Round((double)(Math.Log10(myIV.max_I * 1000)), 0);
                if (zedFEB1.GraphPane.YAxis.Scale.Max < 0.1) { zedFEB1.GraphPane.YAxis.Scale.Max = 0.1; }
                zedFEB1.GraphPane.AddCurve(myIV.chan.ToString(), myIV.loglist, Color.DarkRed, SymbolType.None);
                zedFEB1.GraphPane.YAxis.Scale.MajorStep = .1 * (double)(Math.Log10(myIV.max_I * 1000));
            }
            else
            {
                zedFEB1.GraphPane.YAxis.Scale.Min = 0.0;
                zedFEB1.GraphPane.YAxis.Scale.Max = Math.Round((double)(myIV.max_I + 0.1 * (myIV.max_I - myIV.min_I)), 0);
                zedFEB1.GraphPane.AddCurve(myIV.chan.ToString(), myIV.list, Color.DarkRed, SymbolType.None);
            }
            double s = Math.Round((double)(myIV.max_v - myIV.min_v) / 10.0, 0);
            if (zedFEB1.GraphPane.XAxis.Scale.MajorStep < s) { zedFEB1.GraphPane.XAxis.Scale.MajorStep = s; }
            zedFEB1.GraphPane.XAxis.Scale.MinorStep = zedFEB1.GraphPane.XAxis.Scale.MajorStep / 4;
            zedFEB1.GraphPane.XAxis.Scale.Min = (double)udStart.Value - .2;
            zedFEB1.GraphPane.XAxis.Scale.Max = (double)udStop.Value + .2;

            s = Math.Round((myIV.max_I - myIV.min_I) / 10.0, 0);
            zedFEB1.GraphPane.YAxis.Scale.MinorStep = zedFEB1.GraphPane.YAxis.Scale.MajorStep / 4;

            zedFEB1.Invalidate(true);
            Application.DoEvents();
        }
        private void UpdateDisplay()
        {
            Color[] this_color = new Color[12];
            Histo_helper.InitColorList(ref this_color);

            List<HISTO_curve> myHistoList = null;
            zedFEB1.GraphPane.CurveList.Clear();

            if (_ActiveFEB == 1) { myHistoList = PP.FEB1Histo; }
            if (_ActiveFEB == 2) { myHistoList = PP.FEB2Histo; }

            foreach (HISTO_curve h1 in myHistoList)
            {
                if (chkLogY.Checked)
                {
                    zedFEB1.GraphPane.YAxis.Scale.Max = Math.Round((double)(Math.Log10(h1.max_count + 0.1 * (h1.max_count - h1.min_count))), 0);
                    zedFEB1.GraphPane.AddCurve(h1.chan.ToString(), h1.loglist, Color.DarkRed, SymbolType.None);
                }
                else
                {
                    zedFEB1.GraphPane.YAxis.Scale.Max = Math.Round((double)(h1.max_count + 0.1 * (h1.max_count - h1.min_count)), 0);
                    zedFEB1.GraphPane.AddCurve(h1.chan.ToString(), h1.list, this_color[h1.chan % 16], SymbolType.None);
                }
                double s = 0;
                s = Math.Round((double)(h1.max_thresh - h1.min_thresh) / 10.0, 0);
                if (zedFEB1.GraphPane.XAxis.Scale.MajorStep < s) { zedFEB1.GraphPane.XAxis.Scale.MajorStep = s; }
                zedFEB1.GraphPane.XAxis.Scale.MinorStep = zedFEB1.GraphPane.XAxis.Scale.MajorStep / 4;

                s = Math.Round((h1.max_count - h1.min_count) / 10.0, 0);
                if (zedFEB1.GraphPane.YAxis.Scale.MajorStep < s) { zedFEB1.GraphPane.YAxis.Scale.MajorStep = s; }
                zedFEB1.GraphPane.YAxis.Scale.MinorStep = zedFEB1.GraphPane.YAxis.Scale.MajorStep / 4;
            }
            zedFEB1.Invalidate(true);
            Application.DoEvents();
        }

        private void btnCHANGE_Click(object sender, EventArgs e)
        {
            string[,] s = new string[6, num_reg];
            int max_index = 5;
            int index = Convert.ToInt32(btnCHANGE.Tag);
            if (index + 1 > max_index) { btnCHANGE.Tag = Convert.ToString(0); }
            else { btnCHANGE.Tag = Convert.ToString(index + 1); }

            s[0, 0] = "CSR";
            s[0, 1] = "TRIG_CONTROL";
            s[0, 2] = "CHAN_MASK";
            s[0, 3] = "TEST_PULSE_FREQ";
            s[0, 4] = "TEST_PULSE_DURATION";
            s[0, 5] = "EVENT_WORD_COUNT";
            s[0, 6] = "CHAN_MASK";
            s[0, 7] = "MUX_SEL";
            s[0, 8] = "TRIG_CONTROL";
            s[0, 9] = "SPILL_TRIG_COUNT";
            s[0, 10] = "SPILL_NUMBER";
            s[0, 11] = "SPILL_STATE";
            s[0, 12] = "SPILL_ERROR";
            s[0, 13] = "SPILL_WORD_COUNT";
            s[0, 14] = "SPILL_TRIG_COUNT";

            s[1, 0] = "CSR";
            s[1, 1] = "HISTO_THRESH_AFE0";
            s[1, 2] = "HISTO_THRESH_AFE1";
            s[1, 3] = "HISTO_COUNT_INTERVAL";
            s[1, 4] = "HISTO_CHAN_SEL";
            s[1, 5] = "HISTO_COUNT0";
            s[1, 6] = "HISTO_COUNT1";
            s[1, 7] = "MUX_SEL";
            s[1, 8] = "TRIG_CONTROL";
            s[1, 9] = "SPILL_STATE";
            s[1, 10] = "CHAN_MASK";
            s[1, 11] = "AFE_VGA0";
            s[1, 12] = "AFE_VGA1";
            s[1, 13] = "TRIG_CONTROL";
            s[1, 14] = "UPTIME";

            s[2, 0] = "CSR";
            s[2, 1] = "MIG_Status";
            s[2, 2] = "MIG_fifo_count";
            s[2, 3] = "CHAN_MASK";
            s[2, 4] = "NUM_SAMPLE_REG";
            s[2, 5] = "TRIG_CONTROL";
            s[2, 6] = "AFE_INPUT_FIFO_EMPTY_FLAG";
            s[2, 7] = "MUX_SEL";
            s[2, 8] = "BIAS_BUS_DAC0";
            s[2, 9] = "BIAS_BUS_DAC1";
            s[2, 10] = "AFE_VGA0";
            s[2, 11] = "AFE_VGA1";
            s[2, 12] = "SPILL_NUMBER";
            s[2, 13] = "SPILL_WORD_COUNT";
            s[2, 14] = "SPILL_TRIG_COUNT";

            s[3, 0] = "TRIG_CONTROL";
            s[3, 1] = "NUM_SAMPLE_REG";
            s[3, 2] = "HIT_DEL_REG";
            s[3, 3] = "TEST_PULSE_DURATION";
            s[3, 4] = "TEST_PULSE_INTERSPILL";
            s[3, 5] = "TEST_PULSE_DELAY";
            s[3, 6] = "SPILL_NUMBER";
            s[3, 7] = "SPILL_TRIG_COUNT";
            s[3, 8] = "SPILL_WORD_COUNT";
            s[3, 9] = "CHAN_MASK";
            s[3, 10] = "SPILL_STATE";
            s[3, 11] = "CSR";
            s[3, 12] = "SDRAM_ReadPointer";
            s[3, 13] = "SDRAM_WritePointer";
            s[3, 14] = "UPTIME";

            s[4, 0] = "BIAS_DAC_CH0";
            s[4, 1] = "BIAS_DAC_CH1";
            s[4, 2] = "BIAS_DAC_CH2";
            s[4, 3] = "BIAS_DAC_CH3";
            s[4, 4] = "BIAS_DAC_CH4";
            s[4, 5] = "BIAS_DAC_CH5";
            s[4, 6] = "BIAS_DAC_CH6";
            s[4, 7] = "BIAS_DAC_CH7";
            s[4, 8] = "BIAS_DAC_CH8";
            s[4, 9] = "BIAS_DAC_CH9";
            s[4, 10] = "BIAS_DAC_CH10";
            s[4, 11] = "BIAS_DAC_CH11";
            s[4, 12] = "BIAS_DAC_CH12";
            s[4, 13] = "BIAS_DAC_CH13";
            s[4, 14] = "BIAS_DAC_CH14";

            s[5, 0] = "TEST_COUNTER";
            s[5, 1] = "ONE_WIRE_COMMAND";
            s[5, 2] = "ONE_WIRE_CONTROL";
            s[5, 3] = "ONE_WIRE_READ0";
            s[5, 4] = "ONE_WIRE_READ1";
            s[5, 5] = "ONE_WIRE_REA2";
            s[5, 6] = "ONE_WIRE_READ3";
            s[5, 7] = "ONE_WIRE_READ4";
            s[5, 8] = "BIAS_DAC_CH8";
            s[5, 9] = "BIAS_DAC_CH9";
            s[5, 10] = "BIAS_DAC_CH10";
            s[5, 11] = "BIAS_DAC_CH11";
            s[5, 12] = "BIAS_DAC_CH12";
            s[5, 13] = "BIAS_DAC_CH13";
            s[5, 14] = "BIAS_DAC_CH14";

            Mu2e_Register r1;
            for (int i = 0; i < num_reg; i++)
            {
                rnames[i] = s[index, i];
                txtREGISTERS[i].Tag = i;

                Mu2e_Register.FindName(rnames[i], ref PP.FEB1.arrReg, out r1);
                lblREG[i].Text = rnames[i] + "(x" + Convert.ToString(r1.addr, 16) + ")";
            }

            btnRegREAD_Click(null, null);
        }

        private void btnErase_Click(object sender, EventArgs e)
        {
            zedFEB1.GraphPane.CurveList.Clear();
            zedFEB1.Invalidate(true);
            List<HISTO_curve> myHistoList = null;
            List<HISTO_curve> EraseHistoList = null;
            if (_ActiveFEB == 1) { myHistoList = PP.FEB1Histo; }
            if (_ActiveFEB == 2) { myHistoList = PP.FEB2Histo; }


            //foreach (HISTO_curve h1 in myHistoList)
            //{
            //    if (h1.saved) { myHistoList.Remove(h1); }
            //}
            myHistoList.Clear();
        }

        private void btnTestSpill_Click(object sender, EventArgs e)
        {
            bool in_spill;
            string s_num_trig;
            UInt32 num_trig;
            string s_WC_time;
            //--->            mu2e_Event ev = new mu2e_Event();

            WC_client.check_status(out in_spill, out s_num_trig, out s_WC_time);
            try { num_trig = Convert.ToUInt32("0x0" + s_num_trig, 16); }
            catch { num_trig = 0; }


            Mu2e_Register r2;
            Mu2e_Register.FindName("TRIG_CONTROL", ref PP.FEB1.arrReg, out r2);
            // Mu2e_Register.WriteReg(0x0350, ref r2, ref PP.FEB1.client);
            Mu2e_Register r3;
            Mu2e_Register.FindName("SPILL_STATE", ref PP.FEB1.arrReg, out r3);
            System.Threading.Thread.Sleep(250);
            Mu2e_Register.ReadReg(ref r3, ref PP.FEB1.client);



            //if (_ActiveFEB == 1)
            //{
            //    while (r3.val < 6)
            //    {
            //        System.Threading.Thread.Sleep(250);
            //        Application.DoEvents();
            //        Mu2e_Register.ReadReg(ref r3, ref PP.FEB1.client);
            //        Console.WriteLine("waiting for spill to start");
            //    }

            //    while (r3.val == 6)
            //    {
            //        System.Threading.Thread.Sleep(250);
            //        Mu2e_Register.ReadReg(ref r3, ref PP.FEB1.client);
            //        if (PP.glbDebug){ Console.WriteLine("still in spill");
            //    }
            //    PP.FEB1.ReadAll(ref ev, ref PP.FEB1.client);
            //}
            //else if (_ActiveFEB == 2)
            {

                if (!in_spill) { WC_client.FakeSpill(); in_spill = true; }

                while (in_spill)
                {
                    System.Threading.Thread.Sleep(250);
                    WC_client.check_status(out in_spill, out s_num_trig, out s_WC_time);

                    if (PP.glbDebug) { Console.Write(in_spill.ToString() + " "); }
                    Mu2e_Register.ReadReg(ref r3, ref PP.FEB1.client);
                    if (PP.glbDebug) { Console.Write(r3.val.ToString() + " "); }
                    Mu2e_Register.ReadReg(ref r3, ref PP.FEB2.client);
                    if (PP.glbDebug) { Console.Write(r3.val.ToString() + " "); }
                }
                //PP.FEB2.ReadAll(ref ev, ref PP.FEB2.client);
                //PP.FEB1.ReadAll(ref ev, ref PP.FEB1.client);
                int len = 0;
                WC_client.read_TDC(out len);
                if (PP.glbDebug){ Console.WriteLine(" "); }
                if (PP.glbDebug){ Console.WriteLine(len); }
                if (PP.glbDebug){ Console.WriteLine("done "); }

            }

            //Mu2e_Register r2;
            //Mu2e_Register.FindName("SPILL_TRIG_COUNT", ref PP.FEB2.arrReg, out r2);
            //r2.fpga_index = 0;
            //Mu2e_Register.ReadReg(ref r2, ref PP.FEB2.client);
            //if (r2.val == 0)
            //{
            //    MessageBox.Show("0 trig found in  FEB2.FPGA0  ");
            //}
            ////else if (r2.val != num_trig)
            ////{
            ////    MessageBox.Show("trig count mismatch: WC=" + num_trig + " FEB2.FPGA0=  " + r2.val);
            ////}
            //else
            //{
            //    if (_ActiveFEB == 2)
            //    {
            //        PP.FEB2.ReadAll(ref ev);
            //    }
            //}
        }

        private void chkLogY_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowIV.Visible)
            { UpdateDisplay(); }
            if (ShowSpect.Visible)
            {
                if (_ActiveFEB == 1)
                {
                    if (PP.FEB1IVs.Count > 0) { UpdateDisplay_IV(PP.FEB1IVs.Last()); }
                }
                if (_ActiveFEB == 2)
                {
                    if (PP.FEB2IVs.Count > 0) { UpdateDisplay_IV(PP.FEB2IVs.Last()); }
                }
            }
        }

        private void chkIntegral_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIntegral.Checked) { _IntegralScan = true; }
            else { _IntegralScan = false; }
            UpdateDisplay();
        }

        private void btnSaveHistos_Click(object sender, EventArgs e)
        {
            if (ShowIV.Visible)
            {
                List<HISTO_curve> myHistoList = null;
                if (_ActiveFEB == 1) { myHistoList = PP.FEB1Histo; }
                if (_ActiveFEB == 2) { myHistoList = PP.FEB2Histo; }

                foreach (HISTO_curve h1 in myHistoList)
                {
                    if (h1.saved) { }
                    else { h1.Save(); }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (PP.FEB1.ClientOpen) { PP.FEB1.Close(); dbgFEB1.BackColor = Color.LightGray; btnFEB1.BackColor = Color.LightGray; }
            if (PP.FEB2.ClientOpen) { PP.FEB2.Close(); dbgFEB2.BackColor = Color.LightGray; btnFEB2.BackColor = Color.LightGray; }
        }

        private void ShowIV_Click(object sender, EventArgs e)
        {
            ShowIV.Visible = false;
            ShowSpect.Visible = true;
            chkIntegral.Visible = false;
            udStart.DecimalPlaces = 1;
            udStart.Minimum = 0;
            udStart.Maximum = 80;
            udStart.Value = 60;
            udStart.Increment = (decimal)0.1;
            udStop.DecimalPlaces = 1;
            udStop.Minimum = 1;
            udStop.Maximum = 80;
            udStop.Value = 70;
            udStop.Increment = (decimal)0.1;
            lblInc.Text = "Step (mv)";
            udInterval.Minimum = (decimal)50;
            udInterval.Maximum = 1000;
            udInterval.Value = (decimal)100;
            udInterval.Increment = (decimal)50;
            Application.DoEvents();
        }

        private void ShowSpect_Click(object sender, EventArgs e)
        {
            ShowIV.Visible = true;
            ShowSpect.Visible = false;
            udStart.DecimalPlaces = 0;
            udStop.DecimalPlaces = 0;
            chkIntegral.Visible = true;
            udStart.Minimum = 0;
            udStart.Maximum = 4000;
            udStart.Value = 2000;
            udStart.Increment = (decimal)100;
            udStop.Minimum = 0;
            udStop.Maximum = 4000;
            udStop.Value = 2200;
            udStop.Increment = (decimal)100;
            lblInc.Text = "Time (ms)";
            udInterval.Minimum = (decimal)1;
            udInterval.Maximum = 255;
            udInterval.Value = (decimal)25;
            udInterval.Increment = (decimal)1;
            Application.DoEvents();
        }

        private void btnConnectAll_Click(object sender, EventArgs e)
        {
            Application.DoEvents();
            if (chkWC.Checked)
            { button1_Click((object)btnWC, e); }
            Application.DoEvents();
            if (chkFEB1.Checked)
            { button1_Click((object)btnFEB1, e); }
            Application.DoEvents();
            if (chkFEB2.Checked)
            { button1_Click((object)btnFEB2, e); }
            Application.DoEvents();


            //-------------------------------------------------------
            chkFakeIt_CheckedChanged(null, null);
            chkFakeIt.Visible = false;
            //------DANGER: Fake always off--------------
            Application.DoEvents();
        }

        private void udChan_ValueChanged(object sender, EventArgs e)
        {
            int FPGA_index = (int)udFPGA.Value;
            uint chan = 0;
            Mu2e_Register mux_reg = new Mu2e_Register();
            Mu2e_FEB_client myFEB = new Mu2e_FEB_client();
            if (_ActiveFEB == 1) { myFEB = PP.FEB1; }
            if (_ActiveFEB == 2) { myFEB = PP.FEB2; }
            chan = (uint)udChan.Value;
            mux_reg.fpga_index = (ushort)FPGA_index;
            if (myFEB != null)
            {
                Mu2e_Register.FindAddr(0x020, ref myFEB.arrReg, out mux_reg);
                if (chan < 16)
                {
                    uint v = (uint)(0x10 + chan);
                    Mu2e_Register.WriteReg(v, ref mux_reg, ref myFEB.client);
                }
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool in_spill;
            if (PP.glbDebug) { Console.WriteLine("tick"); }
            if (tabControl.SelectedIndex == 0)
            {
                //if (PP.glbDebug){ Console.WriteLine("timer");}
                try
                {
                    TimeSpan since_last_spill = DateTime.Now.Subtract(PP.myRun.timeLastSpill);
                    lblSpillTime.Text = since_last_spill.TotalSeconds.ToString("0.0");
                    if ((since_last_spill.TotalSeconds > 2) && (PP.myRun.spill_complete))
                    {
                        Mu2e_Register r_spill;
                        Mu2e_Register.FindName("TRIG_CONTROL", ref PP.FEB1.arrReg, out r_spill);
                        if (!PP.myRun.OneSpill)
                        { }// Mu2e_Register.WriteReg(0x02, ref r_spill, ref PP.FEB1.client); }
                    }
                    if (PP.myRun != null)
                    {
                        lblFEB1_TotTrig.Text = PP.myRun.total_trig[0].ToString();
                        lblFEB2_TotTrig.Text = PP.myRun.total_trig[1].ToString();
                        lblWC_TotTrig.Text = PP.myRun.total_trig[2].ToString();
                        if (PP.myRun.ACTIVE)
                        {
                            lblRunTime.Text = DateTime.Now.Subtract(PP.myRun.created).Seconds.ToString("0.0");
                            if ((since_last_spill.Seconds > 2) & (since_last_spill.Seconds < 10))
                            {
                                if (PP.myRun.spill_complete == false)
                                {

                                    PP.myRun.RecordSpill();
                                    if (PP.myRun.OneSpill)
                                    {
                                        //display it
                                        PP.myRun.DeactivateRun();
                                    }
                                    else
                                    {


                                    }
                                    PP.myRun.total_trig[0] += spill_trig_num[0];
                                    PP.myRun.total_trig[1] += spill_trig_num[1];
                                    PP.myRun.total_trig[2] += spill_trig_num[2];


                                    DispSpill = PP.myRun.Spills.Last();
                                    for (LinkedListNode<mu2e_Event> it = DispSpill.SpillEvents.First; it != null; it = it.Next)
                                    {
                                        DispEvent = it.Value;

                                        mu2e_Ch[] cha = DispEvent.ChanData.ToArray();
                                        double[] y = new double[cha[0].data.Count() - 1];
                                        for (int i = 0; i < 4; i++)
                                        {
                                            double ped = cha[i].data.Skip(1).Take(50).Average();
                                            double maxADC = cha[i].data.Max() - ped;
                                            PP.myRun.max_adc[i] += maxADC;
                                        }

                                    }

                                    for (int i = 0; i < 4; i++)
                                    {
                                        double meanADC = PP.myRun.max_adc[i] / PP.myRun.total_trig[0];
                                        //Console.WriteLine(i.ToString() + " " + meanADC.ToString() + " " + PP.myRun.total_trig[0]);
                                    }
                                    string _lblmaxadc0 = string.Format("{0:N2}", PP.myRun.max_adc[0] / PP.myRun.total_trig[0]);
                                    lblMaxADC0.Text = _lblmaxadc0;

                                    string _lblmaxadc1 = string.Format("{0:N2}", PP.myRun.max_adc[1] / PP.myRun.total_trig[0]);
                                    lblMaxADC1.Text = _lblmaxadc1;

                                    string _lblmaxadc2 = string.Format("{0:N2}", PP.myRun.max_adc[2] / PP.myRun.total_trig[0]);
                                    lblMaxADC2.Text = _lblmaxadc2;

                                    string _lblmaxadc3 = string.Format("{0:N2}", PP.myRun.max_adc[3] / PP.myRun.total_trig[0]);
                                    lblMaxADC3.Text = _lblmaxadc3;

                                }
                                else
                                {
                                    if ((btnDisplaySpill.Text == "DONE")&&(since_last_spill.Seconds>8))
                                    {
                                        timer1.Enabled = false;
                                        this.btnDisplaySpill_Click(null, null);
                                        for (int i = 0; i < 10; i++)
                                        {
                                            Application.DoEvents();
                                            System.Threading.Thread.Sleep(50);
                                        }
                                        this.btnDisplaySpill_Click(null, null);
                                        timer1.Enabled = true;
                                    }
                                }
                            }

                        }
                    }
                }
                catch (Exception ex) { timer1.Enabled = false; if (PP.glbDebug) { Console.WriteLine("timer off " + ex.Message); } }

                if (PP.myRun != null) //update log
                {
                    lblRunName.Text = "Run_" + PP.myRun.num.ToString();
                    lblRunLog.Text = "";
                    string[] all_m = PP.myRun.RunStatus.ToArray<string>();
                    int l = all_m.Length;
                    if (PP.myRun.RunStatus.Count > 13)
                    {
                        for (int i = l - 13; i < l; i++)
                        { lblRunLog.Text += all_m[i] + "\r\n"; }
                    }
                    else
                    {
                        for (int i = 0; i < all_m.Length; i++)
                        { lblRunLog.Text += all_m[i] + "\r\n"; }
                    }


                    if (PP.myRun.fake)
                    {
                        if ((PP.FEB1.ClientOpen) && (chkFEB1.Checked))
                        {

                            uint spill_status = 0;
                            uint spill_num = 0;
                            uint trig_num = 0;
                            PP.FEB1.CheckStatus(out spill_status, out spill_num, out trig_num);
                            if (spill_status > 2) { in_spill = true; } else { in_spill = false; }
                            lblSpillFEB1.Text = spill_status.ToString();
                            lblFEB1TrigNum.Text = trig_num.ToString();
                            spill_trig_num[0] = (int)trig_num;
                            if (in_spill)
                            {
                                if (PP.myRun != null)
                                {
                                    PP.myRun.timeLastSpill = DateTime.Now;
                                    PP.myRun.UpdateStatus("Detected spill. Run file is " + PP.myRun.OutFileName);
                                    PP.myRun.spill_complete = false;
                                }
                            }

                        }
                    }

                    else //if (!PP.myRun.fake)
                    {
                        if (PP.FEB1.ClientOpen)
                        {
                            uint spill_status = 0;
                            uint spill_num = 0;
                            uint trig_num = 0;
                            PP.FEB1.CheckStatus(out spill_status, out spill_num, out trig_num);
                            lblSpillFEB1.Text = spill_status.ToString();
                            lblFEB1TrigNum.Text = trig_num.ToString();
                            spill_trig_num[0] = (int)trig_num;
                        }

                        if (PP.FEB2.ClientOpen)
                        {
                            uint spill_status = 0;
                            uint spill_num = 0;
                            uint trig_num = 0;
                            PP.FEB2.CheckStatus(out spill_status, out spill_num, out trig_num);
                            lblSpillFEB2.Text = spill_status.ToString();
                            lblFEB2TrigNum.Text = trig_num.ToString();
                            spill_trig_num[1] = (int)trig_num;
                        }

                        if (PP.WC.ClientOpen)
                        {

                            string num_trig;
                            string mytime;
                            WC_client.check_status(out in_spill, out num_trig, out mytime);
                            lblSpillWC.Text = in_spill.ToString();
                            //spill_trig_num[2] = Convert.ToInt32(num_trig);

                            if (in_spill)
                            {
                                if (PP.myRun != null)
                                {
                                    PP.myRun.timeLastSpill = DateTime.Now;
                                    PP.myRun.UpdateStatus("Detected spill. Run file is " + PP.myRun.OutFileName);
                                    PP.myRun.spill_complete = false;
                                }
                            }


                            lblWCTrigNum.Text = num_trig;
                        }
                    }
                }
            }
        }

        private void btnPrepare_Click(object sender, EventArgs e)
        {
            if ((!PP.FEB1.ClientOpen && chkFEB1.Checked) || (!PP.FEB2.ClientOpen && chkFEB2.Checked) || (!PP.WC.ClientOpen && chkWC.Checked))
            { MessageBox.Show("Are all clients open?"); }
            timer1.Enabled = false;
            PP.myRun = new Run();
            if (chkFakeIt.Checked) { PP.myRun.fake = true; }
            else { PP.myRun.fake = false; }

            if (PP.myRun.fake == false)
            #region notfake
            {
                WC_client.DisableTrig();
                PP.myRun.UpdateStatus("waiting for spill to disable WC");
                if (!PP.WC.in_spill) { WC_client.FakeSpill(); }
                int spill_timeout = 0;
                int big_count = 0;
                bool inspill = false;
                string X = "";
                string Y = "";
                lblRunTime.Text = "not running";
                PP.myRun.ACTIVE = false;
                while (!inspill)
                {
                    if (PP.glbDebug) { Console.WriteLine("waiting for spill"); }
                    System.Threading.Thread.Sleep(200);
                    Application.DoEvents();
                    WC_client.check_status(out inspill, out X, out Y);
                    spill_timeout++;
                    if (spill_timeout > 500) { WC_client.FakeSpill(); spill_timeout = 0; big_count++; }
                    if (big_count > 3) { MessageBox.Show("can't get a spill..."); return; }
                }
                PP.myRun.UpdateStatus("in spill....");
                while (PP.WC.in_spill)
                {
                        if (PP.glbDebug) { Console.WriteLine("waiting for spill to end"); }
                    System.Threading.Thread.Sleep(200);
                    WC_client.check_status(out inspill, out X, out Y);
                    Application.DoEvents();
                }
                PP.myRun.UpdateStatus("Prepairing FEB1 and FEB2");
                //            PP.FEB1.GetReady();
                //            PP.FEB2.GetReady();
                PP.myRun.UpdateStatus("Arming WC");
                if (!PP.WC.in_spill) { WC_client.EnableTrig(); }
                lblRunName.Text = PP.myRun.run_name;
                timer1.Enabled = true;
            }
            #endregion notfake
            else
            {
                PP.myRun.UpdateStatus("Fake Run- sending spills to FEB1");
                lblRunName.Text = PP.myRun.run_name;
                timer1.Enabled = true;
            }
        }

        private void btnStartRun_Click(object sender, EventArgs e)
        {
            if (PP.myRun != null)
            {
                PP.myRun.ActivateRun(); PP.myRun.UpdateStatus("Run STARTING");
                PP.myRun.OneSpill = false;
            }

        }

        private void btnOneSpill_Click(object sender, EventArgs e)
        {
            Mu2e_Register r_spill;
            Mu2e_Register.FindName("TRIG_CONTROL", ref PP.FEB1.arrReg, out r_spill);
            Mu2e_Register.WriteReg(0x300, ref r_spill, ref PP.FEB1.client);
            if (PP.myRun != null)
            {
                PP.myRun.ActivateRun(); PP.myRun.UpdateStatus("Run STARTING in ONE SPILL MODE");
                Mu2e_Register.WriteReg(0x300, ref r_spill, ref PP.FEB1.client);
                PP.myRun.OneSpill = true;
            }
        }

        private void btnStopRun_Click(object sender, EventArgs e)
        {
            if (PP.myRun != null)
            {
                PP.myRun.DeactivateRun();
                PP.myRun.UpdateStatus("Run STOPPING");
                timer1.Enabled = false;
            }
        }

        private void chkFakeIt_CheckedChanged(object sender, EventArgs e)
        {
            WC_client.DisableFake();
            //if (PP.WC.ClientOpen)
            //{
            //    if (chkFakeIt.Checked) { WC_client.ForeverFake(); }
            //    else { WC_client.DisableFake(); }
            //}
            //if (chkFakeIt.Checked) { btnOneSpill.Visible = true; } else { btnOneSpill.Visible = false; }
            btnOneSpill.Visible = false;
        }

        private void btnChangeName_Click(object sender, EventArgs e)
        {
            ClientNameChange frmChange = new ClientNameChange();
            frmChange.ShowDialog();
            btnFEB1.Text = PP.FEB1.host_name_prop;
            btnFEB2.Text = PP.FEB2.host_name_prop;
        }

        private void btnDisplaySpill_Click(object sender, EventArgs e)
        {
            if (btnDisplaySpill.Text == "DISPLAY")
            {
                if (PP.myRun != null)
                {
                    if (PP.myRun.Spills != null)
                    {
                        if (PP.myRun.Spills.Count > 0)
                        {

                            DispSpill = PP.myRun.Spills.Last();
                            for (LinkedListNode<mu2e_Event> it = DispSpill.SpillEvents.First; it != null; it = it.Next)
                            {
                                DispEvent = it.Value;

                                mu2e_Ch[] cha = DispEvent.ChanData.ToArray();
                                double[] y = new double[cha[0].data.Count() - 1];

                                for (int i = 0; i < 4; i++)
                                {
                                    PP.myRun.max_adc[i] += cha[i].data.Max();
                                    Console.WriteLine(PP.myRun.max_adc[i].ToString());
                                }

                            }

                            PP.myRun.Spills.Last().IsDisplayed = true;

                            //DispSpill = PP.myRun.Spills.Last();
                            btnDisplaySpill.Text = "DONE";

                            for (int i = 0; i < DispSpill.ExpectNumCh; i++)
                            {
                                this.chkChEnable[i].Visible = true;
                            }
                            lblEventCount.Text = "Spill " + DispSpill.SpillCounter + "," + DispSpill.SpillEvents.Count + " e";
                            txtEvent.Text = Convert.ToString(1);
                            DispEvent = DispSpill.SpillEvents.First();
                            txtEvent.Text = DispEvent.TrigCounter.ToString();
                            UpdateEventDisplay();

                        }
                    }
                }
            }
            else
            {
                btnDisplaySpill.Text = "DISPLAY";
                foreach (SpillData aSpill in PP.myRun.Spills)
                {
                    if (aSpill.IsDisplayed) { aSpill.IsDisplayed = false; }
                }
                for (int i = 0; i < 64; i++)
                {
                    this.chkChEnable[i].Visible = false;
                }
            }
        }

        private void btnNextDisp_Click(object sender, EventArgs e)
        {
            try
            {
                LinkedListNode<mu2e_Event> node = DispSpill.SpillEvents.Find(DispEvent).Next;
                DispEvent = node.Value;
                txtEvent.Text = DispEvent.TrigCounter.ToString();
                UpdateEventDisplay();
            }
            catch { }
        }

        private void btnPrevDisp_Click(object sender, EventArgs e)
        {
            try
            {
                LinkedListNode<mu2e_Event> node = DispSpill.SpillEvents.Find(DispEvent).Previous;
                DispEvent = node.Value;
                txtEvent.Text = DispEvent.TrigCounter.ToString();
                UpdateEventDisplay();
            }
            catch { }

        }

        private void UpdateEventDisplay()
        {//zg1
            Color[] this_color = new Color[12];
            mu2e_Ch[] cha = DispEvent.ChanData.ToArray();
            zg1.GraphPane.Legend.IsVisible = false;
            if ((!chkPersist.Checked) ||(zg1.GraphPane.CurveList.Count>500))
            { zg1.GraphPane.CurveList.Clear(); }
            try
            {
                if (cha.Count() > 0)
                {
                    double[] x = new double[cha[0].data.Count() - 1];
                    double[] y = new double[cha[0].data.Count() - 1];
                    zg1.GraphPane.YAxis.Scale.Max = Convert.ToDouble(ud_VertMax.Value);
                    zg1.GraphPane.YAxis.Scale.Min = Convert.ToDouble(ud_VertMin.Value);
                    zg1.GraphPane.XAxis.Scale.Max = Convert.ToDouble(x.Count());
                    zg1.GraphPane.XAxis.Scale.Min = Convert.ToDouble(1);
                    zg1.GraphPane.YAxis.Scale.MajorStep = 0.2 * (Convert.ToDouble(ud_VertMax.Value) - Convert.ToDouble(ud_VertMin.Value));
                    zg1.GraphPane.XAxis.Scale.MajorStep = 10;
                    zg1.GraphPane.YAxis.Scale.MinorStep = 0.2 * zg1.GraphPane.YAxis.Scale.MajorStep;
                    zg1.GraphPane.XAxis.Scale.MinorStep = 0.2 * zg1.GraphPane.XAxis.Scale.MajorStep;

                    for (int i = 0; i < x.Count(); i++)
                    { x[i] = i + 1; }

                    for (int i = 0; i < 32; i++)
                    {
                        if (chkChEnable[i].Checked)
                        {
                            for (int j = 0; j < x.Count(); j++)
                            {
                                y[j] = cha[i].data[j + 1];
                            }
                            zg1.GraphPane.AddCurve(cha[i].data[0].ToString(), x, y, Color.DarkRed, SymbolType.None);
                        }
                    }
                    zg1.Invalidate(true);

                }
            }
            catch
            { }
            Application.DoEvents();
        }

        private void btnDebugLogging_Click(object sender, EventArgs e)
        {
            string hName = "";
            if (btnDebugLogging.Text.Contains("START LOG"))
            {
                btnDebugLogging.Text = "STOP LOG";
                hName = "FEB"+_ActiveFEB+ "_commands_" ;
                hName += "_" + DateTime.Now.Year.ToString("0000");
                hName += DateTime.Now.Month.ToString("00");
                hName += DateTime.Now.Day.ToString("00");
                hName += "_" + DateTime.Now.Hour.ToString("00");
                hName += DateTime.Now.Minute.ToString("00");
                hName += DateTime.Now.Second.ToString("00");
            }
        }

        private void btnTimerFix_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            
            //PP.FEB1.client.Close();
            //Application.DoEvents();
            //PP.FEB1.Open();
            //Application.DoEvents();
            Mu2e_Register csr = new Mu2e_Register();
            Mu2e_Register.FindName("CSR", ref PP.FEB1.arrReg, out csr);
            for (int i = 0; i < 4; i++)
            {
                csr.fpga_index =(UInt16)( i);
                Mu2e_Register.WriteReg(0xfc, ref csr, ref PP.FEB1.client);
                System.Threading.Thread.Sleep(10);
            }
            if (timer1.Enabled) { }
            else { timer1.Enabled = true; }
        }

        private void chkLast_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void groupBoxEvDisplay_Enter(object sender, EventArgs e)
        {
           
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void numLabel_Click(object sender, EventArgs e)
        {

        }

        private void numButton_Click(object sender, EventArgs e)
        {
            //Begin Cole's Test Code
            numButton.Enabled = false;  //prevents multiple clicks on the button
            RadioButton[] statusButtons = { but0, but1, but2, but3, but4, but5, but6, but7 }; //holds the 8 buttons
            string[] chanOuts = new string[8];
            using (StreamWriter writer = File.AppendText("C:\\Users\\FEB-Laptop-1\\Desktop\\ScanningData\\ScanningData.txt"))
            {
                string dicountNumber = numTextBox.Text;
                writer.Write("{0}\t", dicountNumber);
                DateTime today = DateTime.Now;
                writer.Write("{0}\t", today.ToString("g"));

                string name = tabControl.SelectedTab.Text;
                if (name.Contains("FEB"))
                {
                    switch (_ActiveFEB)
                    {
                        case 1:
                            PP.FEB1.SetV(Convert.ToDouble(txtV.Text), (int)udFPGA.Value);
                            txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                            break;
                        case 2:
                            PP.FEB2.SetV(Convert.ToDouble(txtV.Text));
                            txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                            break;

                        default:
                            break;
                    }

                    //initially set to green & reset nocurrent flag
                    for (int i = 0; i < 8; i++)
                    {
                        statusButtons[i].BackColor = Color.Green;
                        statusButtons[i].Update();
                    }
                    //but6.BackColor = System.Drawing.Color.Green;
                    //but6.Update();


                    for (int i = 0; i < 8; i++)
                    {
                        double average = 0.0;
                        for (int j = 0; j < numAvg.Value; j++) //added averaging
                            average += PP.FEB1.ReadA0((int)udFPGA.Value, i);
                        average /= 10.0;
                        if (average < Convert.ToDouble(iWarningThresh.Text)) //if the current was less than warning thresh && we still have current, update the color of the lamp
                        {
                            statusButtons[i].BackColor = Color.Red;
                            //but6.BackColor = Color.Red; //set lamp color to red
                            if (average < 0) //if there was no current then
                                statusButtons[i].BackColor = Color.Blue; //set lamp color to blue "cold-no-current"
                            statusButtons[i].Update();
                        }
                        chanOuts[i] = average.ToString("0.0000");
                        Console.WriteLine("Channel {0}: {1}", i, chanOuts[i]);
                        writer.Write("{0}\t", chanOuts[i]);
                        autoDataProgress.Increment(1);
                    }
                    writer.WriteLine();

                    switch (_ActiveFEB)
                    {
                        case 1:
                            PP.FEB1.SetV(0.0, (int)udFPGA.Value);
                            txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                            break;
                        case 2:
                            PP.FEB2.SetV(0.0);
                            txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                            break;

                        default:
                            break;
                    }
                    autoDataProgress.Increment(-7);

                }

               
            }
            numButton.Enabled = true;
            //End Cole's Test Code
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }
    }






    public class ConnectAttemptEventArgs : EventArgs
    {
        public int ConnectAttempt { get; set; }
    }



}
