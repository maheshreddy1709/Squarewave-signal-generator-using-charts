using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrlQiControllerLib;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace SquareWaveGenerator
{
    public partial class SignalWaveGenerator : Form
    {
        public static double dTimeperiod = 0;

        public string Channel1 = "P0";
        public string Channel2 = "N0";
        public string Channel3 = "P1";
        public string Channel4 = "N1";
        Qi_Controller grlC3Controller = null;
        Debuglogger debuglogger = null;
        public SignalWaveGenerator()
        {
            InitializeComponent();
            grlC3Controller = new Qi_Controller();
            Properties.Resources.Background_1.Save(System.Reflection.Assembly.GetExecutingAssembly().Location + "Tempfile");
            chartsignal1.ChartAreas[0].BackImage = System.Reflection.Assembly.GetExecutingAssembly().Location + "Tempfile";

            chartsignal1.Titles.Add("Siganl");
            chartsignal1.Series[Channel1].Points.Clear();

            chartsignal1.Series[Channel1].Points.AddXY(1, 1);
            chartsignal1.Series[Channel1].Points.AddXY(2, 1);
            chartsignal1.Series[Channel1].Points.AddXY(2, 3);
            chartsignal1.Series[Channel1].Points.AddXY(3, 3);
            chartsignal1.Series[Channel1].Points.AddXY(3, 1);
            chartsignal1.Series[Channel1].Points.AddXY(4, 1);
            chartsignal1.Series[Channel1].Points.AddXY(4, 3);
            chartsignal1.Series[Channel1].Points.AddXY(5, 3);


            chartsignal2.Series[Channel2].Points.Clear();
            chartsignal2.Series[Channel2].Points.AddXY(1, 3);
            chartsignal2.Series[Channel2].Points.AddXY(2, 3);
            chartsignal2.Series[Channel2].Points.AddXY(2, 1);
            chartsignal2.Series[Channel2].Points.AddXY(3, 1);
            chartsignal2.Series[Channel2].Points.AddXY(3, 3);
            chartsignal2.Series[Channel2].Points.AddXY(4, 3);
            chartsignal2.Series[Channel2].Points.AddXY(4, 1);
            chartsignal2.Series[Channel2].Points.AddXY(5, 1);

            chartsignal3.Series[Channel3].Points.Clear();
            chartsignal3.Series[Channel3].Points.AddXY(1, 1);
            chartsignal3.Series[Channel3].Points.AddXY(2, 1);
            chartsignal3.Series[Channel3].Points.AddXY(2, 3);
            chartsignal3.Series[Channel3].Points.AddXY(3, 3);
            chartsignal3.Series[Channel3].Points.AddXY(3, 1);
            chartsignal3.Series[Channel3].Points.AddXY(4, 1);
            chartsignal3.Series[Channel3].Points.AddXY(4, 3);
            chartsignal3.Series[Channel3].Points.AddXY(5, 3);

            chartsignal4.Series[Channel4].Points.Clear();
            chartsignal4.Series[Channel4].Points.AddXY(1, 3);
            chartsignal4.Series[Channel4].Points.AddXY(2, 3);
            chartsignal4.Series[Channel4].Points.AddXY(2, 1);
            chartsignal4.Series[Channel4].Points.AddXY(3, 1);
            chartsignal4.Series[Channel4].Points.AddXY(3, 3);
            chartsignal4.Series[Channel4].Points.AddXY(4, 3);
            chartsignal4.Series[Channel4].Points.AddXY(4, 1);
            chartsignal4.Series[Channel4].Points.AddXY(5, 1);
            debuglogger = new Debuglogger();
            debuglogger.CreateFile();
        }
        private void btnConnectController_Click(object sender, EventArgs e)
        {
            Qi_Controller.UpdateGRLControllerConfig(0, "192.168.255.1");
            Qi_Controller.EthernetCommunication.Dispose();
            Thread.Sleep(10);
            Qi_Controller.EthernetCommunication.InitilizePort();
            string strcont = "Disconnected";
            if (Qi_Controller.EthernetCommunication.IsPortOpen)
            {
                strcont = "Connected";
                grlC3Controller.ReadControllerVersion();
            }
            lblCtrlStatus.Text += strcont + ", " + Qi_Controller.FwVersion + ". ";
        }
        private void txtfreq_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(txtfreq.Text, out double dfreq);
            Txttimeperiod.Text = (1000 / dfreq).ToString("0.###");
        }
        private void btnSettimePeriod_Click(object sender, EventArgs e)
        {
            double.TryParse(Txttimeperiod.Text, out dTimeperiod);
            PWMHelper.TimePeriod_ns = (uint)((dTimeperiod * 1000) / 10);
            PWMHelper.UpdateONtimeandOffTime();

            txtbxRaisingEdge.Text = (0).ToString();
            TxtbxFallingEdge.Text = (dTimeperiod / 2).ToString();

            TxtbxRaisetimeChannel2.Text = (dTimeperiod / 2).ToString();
            TxtBxFalltimeChanl2.Text = (dTimeperiod).ToString();

            TxtbxRaisetimeChannel3.Text = (0).ToString();
            TxtBxFalltimeChanl3.Text = (dTimeperiod / 2).ToString();

            TxtbxRaisetimeChannel4.Text = (dTimeperiod / 2).ToString();
            TxtBxFalltimeChanl4.Text = (dTimeperiod).ToString();

            //Chartsignal.ChartAreas["ChartArea1"].AxisX.Interval = (dTimeperiod / 2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dTimeperiod == 0)
                double.TryParse(Txttimeperiod.Text, out dTimeperiod);
            double halfedge = (dTimeperiod / 2);
            double.TryParse(txtbxRaisingEdge.Text, out double itextval);
            PWMHelper.RisetimeP0_ns = (uint)((itextval * 1000) / 10);
            double dratioR = 0;
            if (itextval > 0)
                dratioR = (itextval / halfedge);

            double.TryParse(TxtbxFallingEdge.Text, out double itextvalfall);
            double dratioF = 1 - ((itextvalfall) / halfedge);

            PWMHelper.FalltimeP0_ns = (uint)((itextvalfall * 1000) / 10);

            if (itextval <= itextvalfall)
            {
                double dtempxraise = dratioR;
                double dtempxfall = 1 - dratioF;
                chartsignal1.Series[Channel1].Points.Clear();
                chartsignal1.Series[Channel1].Points.AddXY(0, 1);
                chartsignal1.Series[Channel1].Points.AddXY(dtempxraise, 3);
                chartsignal1.Series[Channel1].Points.AddXY(dtempxfall, 1);

                dtempxraise = 2 + dratioR;
                dtempxfall = 3 - dratioF;
                chartsignal1.Series[Channel1].Points.AddXY(dtempxraise, 1);
                chartsignal1.Series[Channel1].Points.AddXY(dtempxraise, 3);
                chartsignal1.Series[Channel1].Points.AddXY(dtempxfall, 1);
                chartsignal1.Series[Channel1].Points.AddXY(4, 1);
                //chartsignal1.Series[Channel1].Points.AddXY(4, 3);
                dtempxraise += 2;
                dtempxfall += 2;
                chartsignal1.Series[Channel1].Points.AddXY(dtempxraise, 1);
                chartsignal1.Series[Channel1].Points.AddXY(dtempxraise, 3);
                chartsignal1.Series[Channel1].Points.AddXY(dtempxfall, 1);
                chartsignal1.Series[Channel1].Points.AddXY(5, 1);
            }
            else
            {
                //dratioR = 1 - (itextval / halfedge);
                //dratioF = 1 - (itextvalfall / (halfedge));
                //double dtempxraise = 3 - dratioR;
                //double dtempxfall = 4 - dratioF;
                //chartsignal1.Series[Channel1].Points.Clear();
                //chartsignal1.Series[Channel1].Points.AddXY(2, 3);
                //chartsignal1.Series[Channel1].Points.AddXY(2, 1);
                //chartsignal1.Series[Channel1].Points.AddXY(dtempxraise, 1);
                //chartsignal1.Series[Channel1].Points.AddXY(dtempxfall, 3);
                //chartsignal1.Series[Channel1].Points.AddXY(dtempxfall, 1);
                //chartsignal1.Series[Channel1].Points.AddXY(4, 1);
            }



        }
        private void btnREChnl3_Click(object sender, EventArgs e)
        {
            if (dTimeperiod == 0)
                double.TryParse(Txttimeperiod.Text, out dTimeperiod);
            double halfedge = (dTimeperiod / 2);
            double.TryParse(TxtbxRaisetimeChannel3.Text, out double itextval);
            PWMHelper.RisetimeP1_ns = (uint)((itextval * 1000) / 10);
            double dratio = 0;
            if (itextval > 0)
                dratio = (itextval / halfedge);
            double dtempxraise = 2 + dratio;

            double.TryParse(TxtBxFalltimeChanl3.Text, out double itextvalfall);
            dratio = 1 - ((itextvalfall) / halfedge);
            double dtempxfall = 3 - dratio;
            PWMHelper.FalltimeP1_ns = (uint)((itextvalfall * 1000) / 10);

            chartsignal3.Series[Channel3].Points.Clear();
            chartsignal3.Series[Channel3].Points.AddXY(2, 1);
            chartsignal3.Series[Channel3].Points.AddXY(dtempxraise, 1);
            chartsignal3.Series[Channel3].Points.AddXY(dtempxraise, 3);
            chartsignal3.Series[Channel3].Points.AddXY(dtempxfall, 1);
            chartsignal3.Series[Channel3].Points.AddXY(4, 1);
            chartsignal3.Series[Channel3].Points.AddXY(4, 3);

        }
        private void btnREChnl2_Click(object sender, EventArgs e)
        {
            if (dTimeperiod == 0)
                double.TryParse(Txttimeperiod.Text, out dTimeperiod);
            double halfedge = (dTimeperiod / 2);
            double.TryParse(TxtbxRaisetimeChannel2.Text, out double itextval);
            PWMHelper.RisetimeN0_ns = (uint)((itextval * 1000) / 10);
            double dratioR = 1 - (itextval / halfedge);

            double.TryParse(TxtBxFalltimeChanl2.Text, out double itextvalfall);
            double dratioF = 2 - (itextvalfall / (halfedge));
            PWMHelper.FalltimeN0_ns = (uint)((itextvalfall * 1000) / 10);

            double dtempxraise = 3 - dratioR;
            double dtempxfall = 4 - dratioF;

            chartsignal2.Series[Channel2].Points.Clear();
            chartsignal2.Series[Channel2].Points.AddXY(2, 3);
            chartsignal2.Series[Channel2].Points.AddXY(2, 1);
            chartsignal2.Series[Channel2].Points.AddXY(dtempxraise, 3);
            chartsignal2.Series[Channel2].Points.AddXY(dtempxfall, 3);
            chartsignal2.Series[Channel2].Points.AddXY(dtempxfall, 1);
            chartsignal2.Series[Channel2].Points.AddXY(4, 1);


        }
        private void btnREChnl4_Click(object sender, EventArgs e)
        {
            if (dTimeperiod == 0)
                double.TryParse(Txttimeperiod.Text, out dTimeperiod);
            double halfedge = (dTimeperiod / 2);
            double.TryParse(TxtbxRaisetimeChannel4.Text, out double itextval);
            PWMHelper.RisetimeN1_ns = (uint)((itextval * 1000) / 10);
            double dratio = 1 - (itextval / halfedge);
            double dtempxraise = 3 - dratio;


            double.TryParse(TxtBxFalltimeChanl4.Text, out double itextvalfall);
            dratio = 2 - (itextvalfall / (halfedge));
            double dtempxfall = 4 - dratio;
            PWMHelper.FalltimeN1_ns = (uint)((itextvalfall * 1000) / 10);

            chartsignal4.Series[Channel4].Points.Clear();
            chartsignal4.Series[Channel4].Points.AddXY(2, 3);
            chartsignal4.Series[Channel4].Points.AddXY(2, 1);
            chartsignal4.Series[Channel4].Points.AddXY(dtempxraise, 3);
            chartsignal4.Series[Channel4].Points.AddXY(dtempxfall, 3);
            chartsignal4.Series[Channel4].Points.AddXY(dtempxfall, 1);
            chartsignal4.Series[Channel4].Points.AddXY(4, 1);

        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            double.TryParse(txtboxDeadTimeChnl1.Text, out double ideadtime1);
            PWMHelper.DeadTime_P = (int)ideadtime1;

            double.TryParse(txtBoxDeadTimeChnl2.Text, out double ideadtime2);
            PWMHelper.DeadTime_N = (int)ideadtime2;
            //grlC3Controller.ReferenceCalibratorConfig(PWMHelper.DeadTime_P, PWMHelper.DeadTime_N, PWMHelper.IsHalfBridge, false, false);



            SetP0(PWMHelper.RisetimeP0_ns, PWMHelper.FalltimeP0_ns);
            SetN0(PWMHelper.RisetimeN0_ns, PWMHelper.FalltimeN0_ns);
            SetP1(PWMHelper.RisetimeP1_ns, PWMHelper.FalltimeP1_ns);
            SetN1(PWMHelper.RisetimeN1_ns, PWMHelper.FalltimeN1_ns);

            List<byte> databuffer = new List<byte>();
            databuffer.Add(0x81);
            databuffer.Add(0x03);
            databuffer.Add(0x01);
            databuffer.Add(0x00);
            databuffer.Add(0x02);
            databuffer.Add(0x00);
            databuffer.Add(0x00);
            databuffer.Add((byte)(PWMHelper.TimePeriod_ns & 0xFF));
            databuffer.Add((byte)((PWMHelper.TimePeriod_ns >> 8) & 0xFF));
            databuffer.Add((byte)((PWMHelper.TimePeriod_ns >> 16) & 0xFF));
            byte blastbyte = (byte)((PWMHelper.TimePeriod_ns >> 24) & 0x7F);
            blastbyte = (byte)(blastbyte | 0xA0);
            databuffer.Add(blastbyte);
            debuglogger.UdpateDebugLogger(databuffer.ToArray());
            grlC3Controller.WriteReg(databuffer.ToArray());

        }
        private void RadbtnFirstFalingEdge_CheckedChanged(object sender, EventArgs e)
        {
            PWMHelper.IsHalfBridge = true;
            /* if (RadbtnFirstFalingEdge.Checked)
             {
                 chartsignal1.Series[Channel1].Points.Clear();
                 chartsignal1.Series[Channel1].Points.AddXY(2, 3);
                 chartsignal1.Series[Channel1].Points.AddXY(2, 1);
                 chartsignal1.Series[Channel1].Points.AddXY(3, 1);
                 chartsignal1.Series[Channel1].Points.AddXY(3, 3);
                 chartsignal1.Series[Channel1].Points.AddXY(4, 3);
                 chartsignal1.Series[Channel1].Points.AddXY(4, 1);

                 chartsignal2.Series[Channel2].Points.Clear();
                 chartsignal2.Series[Channel2].Points.AddXY(2, 1);
                 chartsignal2.Series[Channel2].Points.AddXY(2, 3);
                 chartsignal2.Series[Channel2].Points.AddXY(3, 3);
                 chartsignal2.Series[Channel2].Points.AddXY(3, 1);
                 chartsignal2.Series[Channel2].Points.AddXY(4, 1);
                 chartsignal2.Series[Channel2].Points.AddXY(4, 3);

                 chartsignal3.Series[Channel3].Points.Clear();
                 chartsignal3.Series[Channel3].Points.AddXY(2, 3);
                 chartsignal3.Series[Channel3].Points.AddXY(2, 1);
                 chartsignal3.Series[Channel3].Points.AddXY(3, 1);
                 chartsignal3.Series[Channel3].Points.AddXY(3, 3);
                 chartsignal3.Series[Channel3].Points.AddXY(4, 3);
                 chartsignal3.Series[Channel3].Points.AddXY(4, 1);

                 chartsignal4.Series[Channel4].Points.Clear();
                 chartsignal4.Series[Channel4].Points.AddXY(2, 1);
                 chartsignal4.Series[Channel4].Points.AddXY(2, 3);
                 chartsignal4.Series[Channel4].Points.AddXY(3, 3);
                 chartsignal4.Series[Channel4].Points.AddXY(3, 1);
                 chartsignal4.Series[Channel4].Points.AddXY(4, 1);
                 chartsignal4.Series[Channel4].Points.AddXY(4, 3);
             } */
        }
        private void radbtnStartraisingEdge_CheckedChanged(object sender, EventArgs e)
        {
            PWMHelper.IsHalfBridge = false;
            /* if (radbtnStartraisingEdge.Checked)
             {
                 chartsignal1.Series[Channel1].Points.Clear();
                 chartsignal1.Series[Channel1].Points.AddXY(2, 1);
                 chartsignal1.Series[Channel1].Points.AddXY(2, 3);
                 chartsignal1.Series[Channel1].Points.AddXY(3, 3);
                 chartsignal1.Series[Channel1].Points.AddXY(3, 1);
                 chartsignal1.Series[Channel1].Points.AddXY(4, 1);
                 chartsignal1.Series[Channel1].Points.AddXY(4, 3);

                 chartsignal2.Series[Channel2].Points.Clear();
                 chartsignal2.Series[Channel2].Points.AddXY(2, 3);
                 chartsignal2.Series[Channel2].Points.AddXY(2, 1);
                 chartsignal2.Series[Channel2].Points.AddXY(3, 1);
                 chartsignal2.Series[Channel2].Points.AddXY(3, 3);
                 chartsignal2.Series[Channel2].Points.AddXY(4, 3);
                 chartsignal2.Series[Channel2].Points.AddXY(4, 1);

                 chartsignal3.Series[Channel3].Points.Clear();
                 chartsignal3.Series[Channel3].Points.AddXY(2, 1);
                 chartsignal3.Series[Channel3].Points.AddXY(2, 3);
                 chartsignal3.Series[Channel3].Points.AddXY(3, 3);
                 chartsignal3.Series[Channel3].Points.AddXY(3, 1);
                 chartsignal3.Series[Channel3].Points.AddXY(4, 1);
                 chartsignal3.Series[Channel3].Points.AddXY(4, 3);

                 chartsignal4.Series[Channel4].Points.Clear();
                 chartsignal4.Series[Channel4].Points.AddXY(2, 3);
                 chartsignal4.Series[Channel4].Points.AddXY(2, 1);
                 chartsignal4.Series[Channel4].Points.AddXY(3, 1);
                 chartsignal4.Series[Channel4].Points.AddXY(3, 3);
                 chartsignal4.Series[Channel4].Points.AddXY(4, 3);
                 chartsignal4.Series[Channel4].Points.AddXY(4, 1);
             } */
        }

        private void SetP0(uint raiseedge, uint falledge)
        {
            List<byte> databuffer = new List<byte>();
            databuffer.Add(0x81);
            databuffer.Add(0x03);
            databuffer.Add(0x01);
            databuffer.Add(0x04);
            databuffer.Add(0x02);
            databuffer.Add(0x00);
            databuffer.Add(0x00);
            databuffer.Add((byte)(raiseedge & 0xFF));
            databuffer.Add((byte)((raiseedge >> 8) & 0xFF));
            databuffer.Add((byte)(falledge & 0xFF));
            databuffer.Add((byte)((falledge >> 8) & 0xFF));
            grlC3Controller.WriteReg(databuffer.ToArray());
            debuglogger.UdpateDebugLogger(databuffer.ToArray());
        }
        private void SetN0(uint raiseedge, uint falledge)
        {
            List<byte> databuffer = new List<byte>();
            databuffer.Add(0x81);
            databuffer.Add(0x03);
            databuffer.Add(0x01);
            databuffer.Add(0x08);
            databuffer.Add(0x02);
            databuffer.Add(0x00);
            databuffer.Add(0x00);
            databuffer.Add((byte)(raiseedge & 0xFF));
            databuffer.Add((byte)((raiseedge >> 8) & 0xFF));
            databuffer.Add((byte)(falledge & 0xFF));
            databuffer.Add((byte)((falledge >> 8) & 0xFF));
            debuglogger.UdpateDebugLogger(databuffer.ToArray());
            grlC3Controller.WriteReg(databuffer.ToArray());
        }
        private void SetP1(uint raiseedge, uint falledge)
        {
            List<byte> databuffer = new List<byte>();
            databuffer.Add(0x81);
            databuffer.Add(0x03);
            databuffer.Add(0x01);
            databuffer.Add(0x0C);
            databuffer.Add(0x02);
            databuffer.Add(0x00);
            databuffer.Add(0x00);
            databuffer.Add((byte)(raiseedge & 0xFF));
            databuffer.Add((byte)((raiseedge >> 8) & 0xFF));
            databuffer.Add((byte)(falledge & 0xFF));
            databuffer.Add((byte)((falledge >> 8) & 0xFF));
            debuglogger.UdpateDebugLogger(databuffer.ToArray());
            grlC3Controller.WriteReg(databuffer.ToArray());
        }
        private void SetN1(uint raiseedge, uint falledge)
        {
            List<byte> databuffer = new List<byte>();
            databuffer.Add(0x81);
            databuffer.Add(0x03);
            databuffer.Add(0x01);
            databuffer.Add(0x10);
            databuffer.Add(0x02);
            databuffer.Add(0x00);
            databuffer.Add(0x00);
            databuffer.Add((byte)(raiseedge & 0xFF));
            databuffer.Add((byte)((raiseedge >> 8) & 0xFF));
            databuffer.Add((byte)(falledge & 0xFF));
            databuffer.Add((byte)((falledge >> 8) & 0xFF));
            debuglogger.UdpateDebugLogger(databuffer.ToArray());
            grlC3Controller.WriteReg(databuffer.ToArray());
        }


    }

    public class PWMHelper
    {
        public static uint TimePeriod_ns = 0;

        public static uint RisetimeP0_ns = 0;
        public static uint FalltimeP0_ns = 0;

        public static uint RisetimeN0_ns = 0;
        public static uint FalltimeN0_ns = 0;

        public static uint RisetimeP1_ns = 0;
        public static uint FalltimeP1_ns = 0;

        public static uint RisetimeN1_ns = 0;
        public static uint FalltimeN1_ns = 0;

        public static bool IsHalfBridge = false;

        public static int DeadTime_P = 0;
        public static int DeadTime_N = 0;
        public PWMHelper()
        {
            TimePeriod_ns = 0;
            RisetimeP0_ns = 0;
            FalltimeP0_ns = 0;
            RisetimeN0_ns = 0;
            FalltimeN0_ns = 0;
            RisetimeP1_ns = 0;
            FalltimeP1_ns = 0;
            RisetimeN1_ns = 0;
            FalltimeN1_ns = 0;

            IsHalfBridge = false;

            DeadTime_P = 0;
            DeadTime_N = 0;
        }

        public static void UpdateONtimeandOffTime()
        {
            RisetimeP0_ns = 0;
            FalltimeP0_ns = (TimePeriod_ns / 2);

            RisetimeN0_ns = (TimePeriod_ns / 2);
            FalltimeN0_ns = (TimePeriod_ns);

            RisetimeP1_ns = 0;
            FalltimeP1_ns = (TimePeriod_ns / 2);

            RisetimeN1_ns = (TimePeriod_ns / 2);
            FalltimeN1_ns = (TimePeriod_ns);
        }
    }
}
