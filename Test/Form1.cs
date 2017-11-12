using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public const UInt32 SPI_SETMOUSESPEED = 0x0071;
        public const UInt32 SPI_GETMOUSESPEED = 0x0070;
        public const UInt32 SPI_GETMOUSE = 0x0003;
        public const UInt32 SPI_SETMOUSE = 0x0004;
        public static UInt32 MOUSESPEED = 10;
        public static Boolean DRAWING = false;

        public Keys currentKey = Keys.A;
        public KeyModifiers currentModifiers = KeyModifiers.Alt;
        public int id;
        public Dictionary<CheckBox, KeyModifiers> checkBoxModifier;
        public bool canChange;
        public Dictionary<int, double> multipliers;
        public double val1 = 0;
        public double val2 = 0;

        [DllImport("User32.dll")]
        static extern Boolean SystemParametersInfo(
            UInt32 uiAction,
            UInt32 uiParam,
            UInt32 pvParam,
            UInt32 fWinIni);
        [DllImport("User32.dll")]
        static extern Boolean SystemParametersInfo(
            UInt32 uiAction,
            UInt32 uiParam,
            IntPtr pvParam,
            UInt32 fWinIni);

        public Form1()
        {
            InitializeComponent();
            checkBoxModifier = new Dictionary<CheckBox, KeyModifiers>();
            checkBoxModifier.Add(checkBox1, KeyModifiers.Alt);
            checkBoxModifier.Add(checkBox2, KeyModifiers.Control);
            checkBoxModifier.Add(checkBox3, KeyModifiers.Shift);
            checkBoxModifier.Add(checkBox4, KeyModifiers.Windows);

            multipliers = new Dictionary<int, double>();
            multipliers.Add(1, 0.03125);
            multipliers.Add(2, 0.0625);
            multipliers.Add(3, 0.125);
            multipliers.Add(4, 0.25);
            multipliers.Add(5, 0.375);
            multipliers.Add(6, 0.5);
            multipliers.Add(7, 0.625);
            multipliers.Add(8, 0.75);
            multipliers.Add(9, 0.875);
            multipliers.Add(10, 1);
            multipliers.Add(11, 1.25);
            multipliers.Add(12, 1.5);
            multipliers.Add(13, 1.75);
            multipliers.Add(14, 2.0);
            multipliers.Add(15, 2.75);
            multipliers.Add(16, 2.5);
            multipliers.Add(17, 2.75);
            multipliers.Add(18, 3.0);
            multipliers.Add(19, 3.25);
            multipliers.Add(20, 3.5);


            
            trackBar1.Value = GetMouseSpeed();
            trackBar2.Value = 3;
            trackBar3.Value = GetAcceleration();

            
            ReadSettings();
            label4.Text = currentKey.ToString();

            foreach (CheckBox i in checkBoxModifier.Keys)
            {
                if (currentModifiers.HasFlag(checkBoxModifier[i]))
                {
                    i.Checked = true;
                }
            }

            id = HotKeyManager.RegisterHotKey(currentKey, currentModifiers);
            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);
            

            

        }

        private void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            //Console.WriteLine("Hit me!");
            DRAWING = !DRAWING;
            MOUSESPEED = DRAWING ? (uint)trackBar2.Value : (uint)trackBar1.Value;
            SystemParametersInfo(
                SPI_SETMOUSESPEED,
                0,
                MOUSESPEED,
                0);
            checkBox5.Checked = DRAWING ? true : false;

        }

        private void HotKeyManager_HotKeyPressed()
        {
            //Console.WriteLine("Hit me!");
            DRAWING = !DRAWING;
            MOUSESPEED = DRAWING ? (uint)trackBar2.Value : (uint)trackBar1.Value;
            SystemParametersInfo(
                SPI_SETMOUSESPEED,
                0,
                MOUSESPEED,
                0);


        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            MOUSESPEED = DRAWING ? (uint)trackBar2.Value : (uint)trackBar1.Value;
            SystemParametersInfo(
                SPI_SETMOUSESPEED,
                0,
                MOUSESPEED,
                0);
            label6.Text = multipliers[trackBar1.Value].ToString();
            val1 = (double)(numericUpDown1.Value) * multipliers[trackBar1.Value];
            label9.Text = val1.ToString();
            
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            MOUSESPEED = DRAWING ? (uint)trackBar2.Value : (uint)trackBar1.Value;
            SystemParametersInfo(
                SPI_SETMOUSESPEED,
                0,
                MOUSESPEED,
                0);
            label7.Text = multipliers[trackBar2.Value].ToString();
            val2 = (double)(numericUpDown1.Value) * multipliers[trackBar2.Value];
            label10.Text = val2.ToString();
        }

        private static unsafe int GetMouseSpeed()
        {
            int speed;
            SystemParametersInfo(
                SPI_GETMOUSESPEED,
                0,
                new IntPtr(&speed),
                0);
            return speed;
        }
        public static int GetAcceleration() {
            int[] mouseParams = new int[3];
            SystemParametersInfo(SPI_GETMOUSE, 0, GCHandle.Alloc(mouseParams, GCHandleType.Pinned).AddrOfPinnedObject(), 0);
            return mouseParams[2];
        }
        public static void SetAcceleration(int i)
        {
            int[] mouseParams = new int[3];
            SystemParametersInfo(SPI_GETMOUSE, 0, GCHandle.Alloc(mouseParams, GCHandleType.Pinned).AddrOfPinnedObject(), 0);
            mouseParams[2] = i;
            SystemParametersInfo(SPI_SETMOUSE, 0, GCHandle.Alloc(mouseParams, GCHandleType.Pinned).AddrOfPinnedObject(), 0);
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            SetAcceleration(trackBar3.Value);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DRAWING = false;
            MOUSESPEED = DRAWING ? (uint)trackBar2.Value : (uint)trackBar1.Value;
            SystemParametersInfo(
                SPI_SETMOUSESPEED,
                0,
                MOUSESPEED,
                0);
            SaveSettings(trackBar2.Value, ref currentKey, currentModifiers, numericUpDown1.Value);
        }

        private void checkBox_Click(object sender, EventArgs e)
        {
            HotKeyManager.UnregisterHotKey(id);
            currentModifiers = 0;
            foreach (CheckBox i in checkBoxModifier.Keys) {
                if (i.Checked) {
                    currentModifiers = currentModifiers | checkBoxModifier[i];
                }
            }

            id = HotKeyManager.RegisterHotKey(currentKey, currentModifiers);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            canChange = true;
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (canChange && e.KeyValue > 47)
            {
                currentKey = e.KeyCode;
                HotKeyManager.UnregisterHotKey(id);
                id = HotKeyManager.RegisterHotKey(currentKey, currentModifiers);
                MessageBox.Show("Key Changed, The key is now " + currentKey.ToString());
                label4.Text = currentKey.ToString();

            }
            canChange = false;
        }
        private void ReadSettings() {
            try
            {
                using (StreamReader sr = new StreamReader("MouseDraw.cfg"))
                {
                    string line;
                    int count = 1;
                    while ((line = sr.ReadLine()) != null)
                    {
                    
                        if (!line.Equals("[Settings]")) {
                            line = SettingValue(line);

                            switch (count) {
                                case 1:
                                    trackBar2.Value = Convert.ToInt32(line);
                                    break;
                                case 2:
                                    currentKey = (Keys)Convert.ToInt32(line);
                                    break;
                                case 3:
                                    currentModifiers = (KeyModifiers)Convert.ToUInt32(line);
                                    break;
                                case 4:
                                    numericUpDown1.Value = Convert.ToDecimal(line);
                                    break;
                                default:
                                    break;
                            }
                            count++;
                        }
                        //Console.WriteLine(line);
                    }
                    HotKeyManager.UnregisterHotKey(id);
                    id = HotKeyManager.RegisterHotKey(currentKey, currentModifiers);
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("The file could not be read:");
               // Console.WriteLine(e.Message);
                SaveSettings(trackBar2.Value, ref currentKey, currentModifiers, numericUpDown1.Value);
            }
        }
        private void SaveSettings(int i, ref Keys k, KeyModifiers km, decimal dpi) {
            using (StreamWriter sw = new StreamWriter("MouseDraw.cfg"))
            {                
                sw.WriteLine("[Settings]");
                sw.WriteLine("DrawingSpeed = " + i);
                sw.WriteLine("Keys = " + (int)k);
                sw.WriteLine("KeyModifiers = " + (int)km);
                sw.WriteLine("DPI = " + dpi);   
            }
            
           
        }
        public static string SettingValue(string line) {
            int position = line.IndexOf("=");
            if (position > 0) {
                line = line.Substring(position + 1).TrimStart(' ');
            }
            return line;
        }

        public static string[] Settings(string[] lines)
        {
            List<string> lineList = new List<string>();
            foreach (string line in lines)
            {
                int position = line.IndexOf("=");
                if (position < 0)
                {
                    continue;
                }
                else {
                    lineList.Add(line.Substring(position + 1).TrimStart(' '));
                }
            }
            return lineList.ToArray();
        }

        private void checkBox5_Click(object sender, EventArgs e)
        {
            HotKeyManager_HotKeyPressed();
            checkBox5.Checked = DRAWING ? true : false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            val1 = (double)(numericUpDown1.Value) * multipliers[trackBar1.Value];
            val2 = (double)(numericUpDown1.Value) * multipliers[trackBar2.Value];
            label9.Text = val1.ToString();
            label10.Text = val2.ToString();
        }
    }
    


   
}
