using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TB_mu2e
{
    public class SpillData
    {
        public DateTime RecTime { get; set; }
        public String Source { get; set; }
        public UInt32 SpillWordCount { get; set; }
        public UInt32 SpillTrigCount { get; set; }
        public UInt16 SpillCounter { get; set; }
        public UInt16 Mask { get; set; }
        public UInt16 ExpectNumCh { get; set; }
        public UInt16 BoardID { get; set; }
        public UInt16 SpillStatus { get; set; }
        public bool SpillParsed { get; set; }
        public bool NoError { get; set; }
        public bool IsDisplayed { get; set; }
        public LinkedList<mu2e_Event> SpillEvents;
        public int MaxTrigPerSpill =4000;

        public SpillData()
        {
            SpillParsed = false;
            NoError = false;
            SpillEvents = new LinkedList<mu2e_Event>();
        }

        public bool ParseInput(byte[] pack, int err = 0)
        {
            if (pack.Length < 0x810)
            { //empty event
                return false;
            }
            //{
            //    int i = 0;
            //    for (int ii = 0; ii < 480; ii++)
            //    {
            //        if (PP.glbDebug){ Console.Write(Convert.ToString((i) + (32 * ii), 16) + "(" + (ii).ToString() + ") => ");
            //        for (int jj = 0; jj < 16; jj++)
            //        {
            //            int this_index = (i) + (32 * ii) + jj * 2;
            //            if (PP.glbDebug){ Console.Write(Convert.ToString(pack[this_index] * 256 + pack[this_index + 1], 16) + " ");}
            //        }
            //        if (PP.glbDebug){ Console.WriteLine();}
            //    }
            //}
            try
            {
                //i is the byte offset counter
                this.RecTime = DateTime.Now;
                #region SpillHeader
                //spill word count 0123
                int i = 0;
                UInt32 t32 = 0;
                t32 = (UInt32)(pack[i] * 256 * 256 * 256 + pack[i + 1] * 256 * 256 + pack[i + 2] * 256 + pack[i + 3]);
                this.SpillWordCount = t32;

                //spill trig count 4567
                i = 4; t32 = 0;
                t32 = (UInt32)(pack[i] * 256 * 256 * 256 + pack[i + 1] * 256 * 256 + pack[i + 2] * 256 + pack[i + 3]);
                this.SpillTrigCount = t32;



                //spill counter 89
                i = 8;
                UInt16 t16 = 0;
                t16 = (UInt16)(pack[i] * 256 + pack[i + 1]);
                this.SpillCounter = t16;

                //mask 10,11
                i = 10; t16 = 0;
                t16 = (UInt16)(pack[i] * 256 + pack[i + 1]);
                this.Mask = t16;
                //from this, we can figure out the num of ch
                this.ExpectNumCh = 0;
                for (int p = 0; p < 16; p++)
                {
                    if ((Convert.ToUInt16(Math.Pow(2, p)) & this.Mask) == Convert.ToUInt16(Math.Pow(2, p))) { this.ExpectNumCh = (UInt16)(this.ExpectNumCh + 4); }
                }
                //board id 12,13
                i = 12; t16 = 0;
                t16 = (UInt16)(pack[i] * 256 + pack[i + 1]);
                this.BoardID = t16;

                //spill status 14,15
                i = 14; t16 = 0;
                t16 = (UInt16)(pack[i] * 256 + pack[i + 1]);
                this.SpillStatus = t16;

                #endregion SpillHeader
                UInt32 event_num_left = this.SpillTrigCount;
                bool first_event_of_spill = true;
                UInt16 ExpectedEventWordCount = 0;
                UInt32 StartPointer = 0x10;
                UInt32 myTriggerCount = 0;
                if (this.SpillTrigCount > MaxTrigPerSpill)
                { if (PP.glbDebug) { Console.WriteLine("too many trig, I quit"); return (false); } }

                i = 16;
                int ch_num = 0;
                DateTime start_parsing = DateTime.Now;

                mu2e_Event this_event = null;
                if (event_num_left > MaxTrigPerSpill) { err = 10000; System.Windows.Forms.MessageBox.Show("Possible garbage read from WC Controller"); return false; }
                while (event_num_left > 0)
                {
                    if (this_event == null) { this_event = new mu2e_Event(); }
                    #region EventHeader
                    //event word count
                    //i is 16 at this point for the first event
                    t16 = 0;


                    t16 = (UInt16)(pack[i] * 256 + pack[i + 1]);
                    this_event.EventWordCount = t16;
                    if (first_event_of_spill)
                    {
                        first_event_of_spill = false; ExpectedEventWordCount = t16;

                    }
                    else
                    {
                        UInt32 myPointer = 0;
                        if (this_event.ChNum > 0xf)
                        {
                            myPointer = StartPointer + (UInt32)(2 * ExpectedEventWordCount) * this.SpillTrigCount + (UInt32)(2 * ExpectedEventWordCount) * myTriggerCount;
                            i = (int)myPointer;
                        }
                        else
                        {
                            myPointer = StartPointer + (UInt32)(2 * ExpectedEventWordCount) * myTriggerCount;
                            i = (int)myPointer;

                            //for (int ii = 0; ii < 20; ii++)
                            //{
                            //    if (PP.glbDebug){ Console.Write(Convert.ToString((myPointer) + (32 * ii), 16) + "(" + (ii).ToString() + ") => ");
                            //    for (int jj = 0; jj < 16; jj++)
                            //    {
                            //        int this_index = (Int32)(myPointer) + (32 * ii) + jj * 2;
                            //        if (PP.glbDebug){ Console.Write(Convert.ToString(pack[this_index] * 256 + pack[this_index + 1], 16) + " ");
                            //    }
                            //    if (PP.glbDebug){ Console.WriteLine();
                            //}

                        }
                        if (PP.glbDebug) { Console.WriteLine("My pointer is =0x{0}", Convert.ToString(myPointer, 16)); }
                        if (PP.glbDebug) { Console.WriteLine("i={0},ThisEventWordCount={1}", Convert.ToString(i, 16), Convert.ToString(this_event.EventWordCount, 16)); }
                    }

                    i = i + 2;
                    //event time stamp
                    t32 = 0;
                    t32 = (UInt32)(pack[i++] * 256 * 256 * 256 + pack[i++] * 256 * 256 + pack[i++] * 256 + pack[i++]);
                    this_event.EventTimeStamp = t32;
                    //if (PP.glbDebug){ Console.WriteLine("time_stamp="+t32);}
                    //trigger counter
                    t32 = 0;
                    t32 = (UInt32)(pack[i++] * 256 * 256 * 256 + pack[i++] * 256 * 256 + pack[i++] * 256 + pack[i++]);
                    this_event.TrigCounter = t32;
                    if (PP.glbDebug) { Console.WriteLine("trig=" + t32); }
                    //if (this_event.TrigCounter > 2000)
                    //{
                    //    System.IO.StreamWriter tw_bad_Evt = new System.IO.StreamWriter("c:\\data\\bad_evt" + PP.myRun.num + "_" + PP.Random_Number);
                    //    PP.Random_Number++;
                    //    tw_bad_Evt.WriteLine("Found bad event at"+ Convert.ToString((i-4), 16)+" with TrigCount="+ Convert.ToString(this_event.TrigCounter,16));
                    //    {
                    //        int bi = 0;
                    //        if (i > 128)
                    //        {
                    //            bi = i - 128;
                    //            string szbi = Convert.ToString(bi, 16);
                    //            char last_bi = szbi.Last<char>();
                    //            while (last_bi != '0')
                    //            {
                    //                bi++;
                    //                szbi = Convert.ToString(bi, 16);
                    //                last_bi = szbi.Last<char>();
                    //            }
                    //        }
                    //        else { bi = 0; }
                    //        for (int ii = 0; ii < 480; ii++)
                    //        {
                    //            tw_bad_Evt.Write(Convert.ToString((bi) + (32 * ii), 16) + "(" + (ii).ToString() + ") => ");
                    //            for (int jj = 0; jj < 16; jj++)
                    //            {
                    //                int this_index = (bi) + (32 * ii) + jj * 2;
                    //                tw_bad_Evt.Write(Convert.ToString(pack[this_index] * 256 + pack[this_index + 1], 16) + " ");
                    //            }
                    //            tw_bad_Evt.WriteLine();
                    //        }
                    //    }
                    //    tw_bad_Evt.Close();
                    //}
                    //event num samples
                    t16 = 0;
                    t16 = (UInt16)(pack[i++] * 256 + pack[i++]);
                    this_event.NumSamples = t16;

                    //event trig type
                    t16 = (UInt16)(pack[i++] * 256 + pack[i++]);
                    this_event.TrigType = t16;

                    //event stsus
                    t16 = (UInt16)(pack[i++] * 256 + pack[i++]);
                    this_event.EventStatus = t16;

                    #endregion EventHeader

                    #region EventData
                    if (this_event.NumSamples == 0)
                    { if (PP.glbDebug) { Console.WriteLine("stop!"); } }

                    int num_ch = Convert.ToInt32(this_event.EventWordCount / this_event.NumSamples);
                    UInt16 ChNum = 0;
                    if (this_event.ChNum > 0xf) { ChNum = 16; }
                    for (int k = 0; k < num_ch; k++)   // num_chan == 0 (Corrado)
                    {
                        int[] ch_data = new int[this_event.NumSamples];
                        for (int j = 0; j < this_event.NumSamples; j++)
                        {
                            t16 = (UInt16)(pack[i++] * 256 + pack[i++]);
                            if ((t16 & 0x8000) == 0x8000)//ch_num
                            {
                                if (PP.glbDebug) { Console.WriteLine(t16.ToString("x")); }
                                t16 = (UInt16)(t16 & 0xfff);
                                ChNum = t16;
                                if (j != 0)
                                {
                                    if (PP.glbDebug)
                                    {
                                        Console.WriteLine("Chan in wrong spot!");
                                        Console.WriteLine("num_ch=" + num_ch.ToString());
                                    }
                                }
                            }
                            ch_data[j] = t16;
                            if (ch_data[j] > 0x7ff) { ch_data[j] = ch_data[j] - 0xfff; }
                        }

                        mu2e_Ch this_ch = new mu2e_Ch();
                        this_event.ChNum++;
                        this_ch.data = ch_data;
                        this_ch.num_ch = ChNum;
                        this_event.ChanData.Add(this_ch);
                    }


                    if (PP.glbDebug) { Console.WriteLine("end of event {0} at time {1}-----------------", this_event.TrigCounter, this_event.EventTimeStamp); }

                    #endregion EventData
                    if (this_event.ChNum >= ExpectNumCh)
                    {
                        myTriggerCount++;
                        event_num_left--;
                        this.SpillEvents.AddLast(this_event);
                        this_event = null;
                    }
                }
                DateTime end_parsing = DateTime.Now;
                TimeSpan time_to_parse = end_parsing.Subtract(start_parsing);
                if (PP.glbDebug) { Console.WriteLine("Time to parse {0} events was {1} ms", this.SpillTrigCount, time_to_parse.TotalMilliseconds); }
                return true;
                            }
            catch (Exception e)
            {
                if (PP.glbDebug) { Console.WriteLine("Caught exception " + e.Message + " in spill data"); }
                return false;
            }
        }
    }

    public class mu2e_Event
    {
        public UInt16 EventWordCount { get; set; }
        public UInt32 EventTimeStamp { get; set; }
        public UInt32 TrigCounter { get; set; }
        public UInt16 NumSamples { get; set; }
        public UInt16 TrigType { get; set; }
        public UInt16 EventStatus { get; set; }
        public UInt16 ChNum { get; set; }

        public List<mu2e_Ch> ChanData;

        public mu2e_Event()
        {
            EventWordCount = 0;
            EventTimeStamp = 0;
            ChanData = new List<mu2e_Ch>(16);
        }
    }

    public class mu2e_Ch
    {
        public int num_ch;
        public int[] data;
    }


}
