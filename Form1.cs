using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace serialPortTrans
{
    public partial class Form1 : Form
    {
        //全局变量声明
        GraphPane myPane;
        GraphPane myPaneTamb;

        int index = 0;
        int indexTamb = 0;
        PointPairList list_Td1 = new PointPairList();
        PointPairList list_Tu1 = new PointPairList();
        PointPairList list_Td2 = new PointPairList();
        PointPairList list_Tu2 = new PointPairList();
        PointPairList list_Te = new PointPairList();
        PointPairList list_Teout = new PointPairList();
        PointPairList list_Tc1 = new PointPairList();
        PointPairList list_Tc2 = new PointPairList();
        

        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;//设置该属性 为false,否则后期容易出现异常
            myPane = zedGraphControl1.GraphPane;
            myPaneTamb = zedGraphControl2.GraphPane;
            /***************Td1/Td2/Tu1/Tu2*************/
            LineItem myCurve_Td1 = myPane.AddCurve("Td1", list_Td1, Color.Red, SymbolType.Star);//新建并初始化曲线
            LineItem myCurve_Tu1 = myPane.AddCurve("Tu1", list_Tu1, Color.Green, SymbolType.Circle);//新建并初始化曲线
            LineItem myCurve_Td2 = myPane.AddCurve("Td2", list_Td2, Color.Blue, SymbolType.Diamond);//新建并初始化曲线
            LineItem myCurve_Tu2 = myPane.AddCurve("Tu2", list_Tu2, Color.Black, SymbolType.Square);//新建并初始化曲线
            /*******环境温度板子********/
            LineItem myCurve_Te = myPaneTamb.AddCurve("Te", list_Td1, Color.Red, SymbolType.Star);//新建并初始化曲线
            LineItem myCurve_Teout = myPaneTamb.AddCurve("Teout", list_Tu1, Color.Green, SymbolType.Circle);//新建并初始化曲线
            LineItem myCurve_Tc1 = myPaneTamb.AddCurve("Tc", list_Td2, Color.Blue, SymbolType.Diamond);//新建并初始化曲线
            LineItem myCurve_Tc2 = myPaneTamb.AddCurve("Tctest", list_Tu2, Color.Black, SymbolType.Square);//新建并初始化曲线


            PortBox.Text = "COM6";
            PortBoxTamb.Text = "COM8";
        }
        public class CommonData
        {
            public static double Td1, Tu1, Td2, Tu2;
            public static double Te, Teout, Tc1, Tc2;
            public static double Kchange=2.3;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;//隐藏最大化
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            this.zedGraphControl1.GraphPane.Title = "探头内各传感器的温度曲线";
            this.zedGraphControl1.GraphPane.XAxis.Title = "时间(s)";
            this.zedGraphControl1.GraphPane.YAxis.Title = "温度(℃)";



            this.zedGraphControl2.GraphPane.Title = "环境温度、实际核心温度监测";
            this.zedGraphControl2.GraphPane.XAxis.Title = "时间(s)";
            this.zedGraphControl2.GraphPane.YAxis.Title = "温度(℃)";

            PortBox.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            PortBoxTamb.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());

        }

        private void PortBox_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        bool isopen = false;
        private void Btn_open_Click(object sender, EventArgs e)
        {
            if (!isopen)
            {
                serialPort1.PortName = PortBox.Text;
                serialPort2.PortName = PortBoxTamb.Text;
                
                try
                {
                    serialPort1.Open();     //打开串口
                    serialPort2.Open();
                    PortBox.Enabled = false;
                    PortBoxTamb.Enabled = false;
                    btn_open.Text = "关闭串口";
                    btn_open.BackColor = Color.Red;
                    isopen = true;

                }
                catch
                {
                    MessageBox.Show("串口打开失败！");
                }
            }
            else
            {
                try
                {
                    isopen = false;
                    serialPort1.Close();
                    serialPort2.Close();
                    btn_open.Text = "打开串口";
                    PortBox.Enabled = true;
                    PortBoxTamb.Enabled = true;
                    btn_open.BackColor = Color.Green;
                }
                catch
                {
                    MessageBox.Show("串口关闭失败！");
                }

            }

        }


        private static string ByteToHexStr(byte[] bytes)
        {
            string returnstr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnstr += bytes[i].ToString("X2");
                    returnstr += " ";
                }
            }
            return returnstr;
        }
        
        private static double VolAdcToTemp(int sensor_No,int vol_adc)
        {
            // 实现将AD值转化为温度值
            double[] TEMP_TABLE =
            {
                21.43,
                22.05,
                22.45,
                22.88,
                23.38,
                23.85,
                24.35,
                24.69,
                24.8,
                25.26,
                25.7,
                26.2,
                26.79,
                27.31,
                27.87,
                28.35,
                28.86,
                29.33,
                29.89,
                30.3,
                30.64,
                30.88,
                31.32,
                31.89,
                32.39,
                32.84,
                33.37,
                33.83,
                34.4,
                34.87,
                35.39,
                35.89,
                36.41,
                36.76,
                37.10,
                37.60,
                38.03,
                38.57,
                38.96,
                39.33,
                39.82,
                40.22,
                40.61,
                41.00,
            };
            int[,] VOL_ADC_TABLE = {
                {   4017    ,   4013    ,   4027    ,   4020    }   ,
                {   3894    ,   3888    ,   3904    ,   3896    }   ,
                {   3817    ,   3817    ,   3828    ,   3818    }   ,
                {   3736    ,   3733    ,   3744    ,   3736    }   ,
                {   3641    ,   3631    ,   3650    ,   3639    }   ,
                {   3541    ,   3543    ,   3551    ,   3552    }   ,
                {   3441    ,   3441    ,   3453    ,   3455    }   ,
                {   3377    ,   3377    ,   3387    ,   3388    }   ,
                {   3352    ,   3353    ,   3368    ,   3367    }   ,
                {   3265    ,   3266    ,   3274    ,   3275    }   ,
                {   3180    ,   3179    ,   3192    ,   3189    }   ,
                {   3082    ,   3082    ,   3096    ,   3096    }   ,
                {   2970    ,   2970    ,   2986    ,   2981    }   ,
                {   2868    ,   2868    ,   2881    ,   2882    }   ,
                {   2766    ,   2763    ,   2777    ,   2770    }   ,
                {   2670    ,   2665    ,   2676    ,   2674    }   ,
                {   2574    ,   2570    ,   2582    ,   2577    }   ,
                {   2482    ,   2477    ,   2482    ,   2481    }   ,
                {   2386    ,   2383    ,   2392    ,   2389    }   ,
                {   2311    ,   2306    ,   2315    ,   2314    }   ,
                {   2252    ,   2258    ,   2266    ,   2263    }   ,
                {   2212    ,   2204    ,   2219    ,   2216    }   ,
                {   2138    ,   2135    ,   2147    ,   2143    }   ,
                {   2045    ,   2044    ,   2052    ,   2051    }   ,
                {   1965    ,   1964    ,   1976    ,   1971    }   ,
                {   1894    ,   1892    ,   1905    ,   1899    }   ,
                {   1808    ,   1809    ,   1814    ,   1816    }   ,
                {   1738    ,   1737    ,   1745    ,   1749    }   ,
                {   1655    ,   1654    ,   1663    ,   1660    }   ,
                {   1588    ,   1586    ,   1594    ,   1593    }   ,
                {   1516    ,   1515    ,   1520    ,   1521    }   ,
                {   1446    ,   1446    ,   1452    ,   1454    }   ,
                {   1380    ,   1381    ,   1386    ,   1387    }   ,
                {   1334    ,   1333    ,   1340    ,   1341    }   ,
                {   1291    ,   1291    ,   1295    ,   1297    }   ,
                {   1227    ,   1226    ,   1232    ,   1236    }   ,
                {   1173    ,   1174    ,   1183    ,   1182    }   ,
                {   1107    ,   1108    ,   1112    ,   1114    }   ,
                {   1063    ,   1063    ,   1070    ,   1069    }   ,
                {   1020    ,   1016    ,   1025    ,   1027    }   ,
                {   966 ,   966 ,   968 ,   966 }   ,
                {   919 ,   918 ,   926 ,   925 }   ,
                {   874 ,   875 ,   883 ,   881 }   ,
                {   834 ,   836 ,   839 ,   840 }

            };
            int NUM_OF_CAL_PT = VOL_ADC_TABLE.GetLength(0);
            double temp1;
            double temp2;
            double temp = 0;
            double k_AdcToVol;
            int cTop = 0;
            int cBottom = NUM_OF_CAL_PT - 1;
            int cMid = (cTop + cBottom) >> 1;
            if (vol_adc > VOL_ADC_TABLE[0,sensor_No - 1])
            {
                temp = 0;
            }
            else if (vol_adc < 600)//(vol_adc < VOL_ADC_TABLE[NUM_OF_CAL_PT-1][sensor_No-1])              
            {
                temp = 100;
            }
            else
            {
                //大于41小于42
                if (vol_adc < VOL_ADC_TABLE[NUM_OF_CAL_PT - 1,sensor_No - 1])
                {
                    temp = (VOL_ADC_TABLE[NUM_OF_CAL_PT - 1,sensor_No - 1] - vol_adc) / (VOL_ADC_TABLE[NUM_OF_CAL_PT - 2,sensor_No - 1] - VOL_ADC_TABLE[NUM_OF_CAL_PT - 1,sensor_No - 1]) * (TEMP_TABLE[NUM_OF_CAL_PT - 1] - TEMP_TABLE[NUM_OF_CAL_PT - 2]) + TEMP_TABLE[NUM_OF_CAL_PT - 1];
                }
                else
                {
                    while ((cBottom - cTop) > 1)
                    {
                        if (vol_adc > VOL_ADC_TABLE[cMid,sensor_No - 1])
                        {
                            cBottom = cMid;
                            cMid = (cTop + cBottom)/2;
                        }
                        else if (vol_adc < VOL_ADC_TABLE[cMid,sensor_No - 1])
                        {
                            cTop = cMid;
                            cMid = (cTop + cBottom) /2;
                        }
                        else                 //说明当前电阻值刚好是表中的某个数，温度值就是对应的那个数
                        {
                            temp = TEMP_TABLE[cMid];
                            break;
                        }
                    }
                    if (temp != TEMP_TABLE[cMid]) 
                    {
                        k_AdcToVol = (TEMP_TABLE[cBottom] - TEMP_TABLE[cTop]) / (VOL_ADC_TABLE[cTop,sensor_No - 1] - VOL_ADC_TABLE[cBottom,sensor_No - 1]);
                        temp1 = TEMP_TABLE[cTop] + k_AdcToVol * (VOL_ADC_TABLE[cTop,sensor_No - 1] - vol_adc);
                        temp2 = TEMP_TABLE[cBottom] - k_AdcToVol * (vol_adc - VOL_ADC_TABLE[cBottom,sensor_No - 1]);
                        temp = (temp1 + temp2) / 2;
                    }
                }
            }
            return temp;
        }
        private static double VolAdcToTamb(int sensor_No, int vol_adc)
        {
            // 实现将AD值转化为温度值
            double[] TEMP_TABLE =
            {
                21.43,
                22.05,
                22.45,
                22.88,
                23.38,
                23.85,
                24.35,
                24.69,
                24.8,
                25.26,
                25.7,
                26.2,
                26.79,
                27.31,
                27.87,
                28.35,
                28.86,
                29.33,
                29.89,
                30.3,
                30.64,
                30.88,
                31.32,
                31.89,
                32.39,
                32.84,
                33.37,
                33.83,
                34.4,
                34.87,
                35.39,
                35.89,
                36.41,
                36.76,
                37.10,
                37.60,
                38.03,
                38.57,
                38.96,
                39.33,
                39.82,
                40.22,
                40.61,
                41.00,
            };
            int[,] VOL_ADC_TABLE = {
                {   4017    ,   4013    ,   4027    ,   4020    }   ,
                {   3894    ,   3888    ,   3904    ,   3896    }   ,
                {   3817    ,   3817    ,   3828    ,   3818    }   ,
                {   3736    ,   3733    ,   3744    ,   3736    }   ,
                {   3641    ,   3631    ,   3650    ,   3639    }   ,
                {   3541    ,   3543    ,   3551    ,   3552    }   ,
                {   3441    ,   3441    ,   3453    ,   3455    }   ,
                {   3377    ,   3377    ,   3387    ,   3388    }   ,
                {   3352    ,   3353    ,   3368    ,   3367    }   ,
                {   3265    ,   3266    ,   3274    ,   3275    }   ,
                {   3180    ,   3179    ,   3192    ,   3189    }   ,
                {   3082    ,   3082    ,   3096    ,   3096    }   ,
                {   2970    ,   2970    ,   2986    ,   2981    }   ,
                {   2868    ,   2868    ,   2881    ,   2882    }   ,
                {   2766    ,   2763    ,   2777    ,   2770    }   ,
                {   2670    ,   2665    ,   2676    ,   2674    }   ,
                {   2574    ,   2570    ,   2582    ,   2577    }   ,
                {   2482    ,   2477    ,   2482    ,   2481    }   ,
                {   2386    ,   2383    ,   2392    ,   2389    }   ,
                {   2311    ,   2306    ,   2315    ,   2314    }   ,
                {   2252    ,   2258    ,   2266    ,   2263    }   ,
                {   2212    ,   2204    ,   2219    ,   2216    }   ,
                {   2138    ,   2135    ,   2147    ,   2143    }   ,
                {   2045    ,   2044    ,   2052    ,   2051    }   ,
                {   1965    ,   1964    ,   1976    ,   1971    }   ,
                {   1894    ,   1892    ,   1905    ,   1899    }   ,
                {   1808    ,   1809    ,   1814    ,   1816    }   ,
                {   1738    ,   1737    ,   1745    ,   1749    }   ,
                {   1655    ,   1654    ,   1663    ,   1660    }   ,
                {   1588    ,   1586    ,   1594    ,   1593    }   ,
                {   1516    ,   1515    ,   1520    ,   1521    }   ,
                {   1446    ,   1446    ,   1452    ,   1454    }   ,
                {   1380    ,   1381    ,   1386    ,   1387    }   ,
                {   1334    ,   1333    ,   1340    ,   1341    }   ,
                {   1291    ,   1291    ,   1295    ,   1297    }   ,
                {   1227    ,   1226    ,   1232    ,   1236    }   ,
                {   1173    ,   1174    ,   1183    ,   1182    }   ,
                {   1107    ,   1108    ,   1112    ,   1114    }   ,
                {   1063    ,   1063    ,   1070    ,   1069    }   ,
                {   1020    ,   1016    ,   1025    ,   1027    }   ,
                {   966 ,   966 ,   968 ,   966 }   ,
                {   919 ,   918 ,   926 ,   925 }   ,
                {   874 ,   875 ,   883 ,   881 }   ,
                {   834 ,   836 ,   839 ,   840 }

            };
            int NUM_OF_CAL_PT = VOL_ADC_TABLE.GetLength(0);
            double temp1;
            double temp2;
            double temp = 0;
            double k_AdcToVol;
            int cTop = 0;
            int cBottom = NUM_OF_CAL_PT - 1;
            int cMid = (cTop + cBottom) >> 1;
            if (vol_adc > VOL_ADC_TABLE[0, sensor_No - 1])
            {
                temp = 0;
            }
            else if (vol_adc < 600)//(vol_adc < VOL_ADC_TABLE[NUM_OF_CAL_PT-1][sensor_No-1])              
            {
                temp = 100;
            }
            else
            {
                //大于41小于42
                if (vol_adc < VOL_ADC_TABLE[NUM_OF_CAL_PT - 1, sensor_No - 1])
                {
                    temp = (VOL_ADC_TABLE[NUM_OF_CAL_PT - 1, sensor_No - 1] - vol_adc) / (VOL_ADC_TABLE[NUM_OF_CAL_PT - 2, sensor_No - 1] - VOL_ADC_TABLE[NUM_OF_CAL_PT - 1, sensor_No - 1]) * (TEMP_TABLE[NUM_OF_CAL_PT - 1] - TEMP_TABLE[NUM_OF_CAL_PT - 2]) + TEMP_TABLE[NUM_OF_CAL_PT - 1];
                }
                else
                {
                    while ((cBottom - cTop) > 1)
                    {
                        if (vol_adc > VOL_ADC_TABLE[cMid, sensor_No - 1])
                        {
                            cBottom = cMid;
                            cMid = (cTop + cBottom) / 2;
                        }
                        else if (vol_adc < VOL_ADC_TABLE[cMid, sensor_No - 1])
                        {
                            cTop = cMid;
                            cMid = (cTop + cBottom) / 2;
                        }
                        else                 //说明当前电阻值刚好是表中的某个数，温度值就是对应的那个数
                        {
                            temp = TEMP_TABLE[cMid];
                            break;
                        }
                    }
                    if (temp != TEMP_TABLE[cMid])
                    {
                        k_AdcToVol = (TEMP_TABLE[cBottom] - TEMP_TABLE[cTop]) / (VOL_ADC_TABLE[cTop, sensor_No - 1] - VOL_ADC_TABLE[cBottom, sensor_No - 1]);
                        temp1 = TEMP_TABLE[cTop] + k_AdcToVol * (VOL_ADC_TABLE[cTop, sensor_No - 1] - vol_adc);
                        temp2 = TEMP_TABLE[cBottom] - k_AdcToVol * (vol_adc - VOL_ADC_TABLE[cBottom, sensor_No - 1]);
                        temp = (temp1 + temp2) / 2;
                    }
                }
            }
            return temp;
        }
        private void serial_receive(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

            int num = serialPort1.BytesToRead;//获取缓冲区字节数
            byte[] ReceiveBuf = new byte[num];
            serialPort1.Read(ReceiveBuf, 0, num);
            Invoke(new MethodInvoker(delegate ()
            {
                showBox.Text += ByteToHexStr(ReceiveBuf);
                if (num == 24)
                {
                    string str = ByteToHexStr(ReceiveBuf);
                    string str_Td1 = str.Substring(39, 2) + str.Substring(42, 2);
                    string str_Tu1 = str.Substring(48, 2) + str.Substring(51, 2);
                    string str_Td2 = str.Substring(57, 2) + str.Substring(60, 2);
                    string str_Tu2 = str.Substring(66, 2) + str.Substring(69, 2);

                    /*******实现将AD值转化为温度值********/
                    CommonData.Td1 = VolAdcToTemp(1, Convert.ToInt32(str_Td1, 16));
                    CommonData.Tu1 = VolAdcToTemp(2, Convert.ToInt32(str_Tu1, 16));
                    CommonData.Td2 = VolAdcToTemp(3, Convert.ToInt32(str_Td2, 16));
                    CommonData.Tu2 = VolAdcToTemp(4, Convert.ToInt32(str_Tu2, 16));
                                        index += 2;
                    
                    // 造一些数据，PointPairList里有数据对x，y的数
                    list_Td1.Add(index, CommonData.Td1); //添加一组数据
                    list_Tu1.Add(index, CommonData.Td1);
                    list_Td2.Add(index, CommonData.Td2);
                    list_Tu2.Add(index, CommonData.Tu2);

                    LineItem myCurve_Td1 = myPane.AddCurve(null, list_Td1, Color.Red, SymbolType.Star);//新建并初始化曲线
                    LineItem myCurve_Tu1 = myPane.AddCurve(null, list_Tu1, Color.Green, SymbolType.Circle);//新建并初始化曲线
                    LineItem myCurve_Td2 = myPane.AddCurve(null, list_Td2, Color.Blue, SymbolType.Diamond);//新建并初始化曲线
                    LineItem myCurve_Tu2 = myPane.AddCurve(null, list_Tu2, Color.Black, SymbolType.Square);//新建并初始化曲线

                    //填充图表颜色
                    myPane.PaneFill = new Fill(Color.White, Color.FromArgb(200, 200, 255), 45.0f);
                    //画到zedGraphControl1控件中，此句必加
                    zedGraphControl1.AxisChange();//调整x轴
                    //重绘控件
                    Refresh();

                }
            }
                ));
        }
        private void serialTamb_receive(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int numTamb = serialPort2.BytesToRead;//获取缓冲区字节数
            byte[] ReceiveBuf = new byte[numTamb];
            serialPort2.Read(ReceiveBuf, 0, numTamb);
            Invoke(new MethodInvoker(delegate ()
            {
                showBoxTamb.Text += ByteToHexStr(ReceiveBuf);
                if (numTamb == 24)
                {
                    string str = ByteToHexStr(ReceiveBuf);
                    string str_Te = str.Substring(39, 2) + str.Substring(42, 2);
                    string str_Teout = str.Substring(48, 2) + str.Substring(51, 2);
                    string str_Tc1 = str.Substring(57, 2) + str.Substring(60, 2);
                    string str_Tc2 = str.Substring(66, 2) + str.Substring(69, 2);

                    /*******实现将AD值转化为温度值********/
                    CommonData.Te = VolAdcToTamb(1, Convert.ToInt32(str_Te, 16));
                    CommonData.Teout = VolAdcToTamb(2, Convert.ToInt32(str_Teout, 16));
                    CommonData.Tc1 = VolAdcToTamb(3, Convert.ToInt32(str_Tc1, 16));
                    CommonData.Tc2 = VolAdcToTamb(4, Convert.ToInt32(str_Tc2, 16));

                    double Tc = (CommonData.Tc1 + CommonData.Tc2) / 2;
                    TcBox.Text = Tc.ToString();
                    double K = 0;
                    double k1 = Tc - CommonData.Td2;
                    double k2 = CommonData.Td1 - CommonData.Tu1;
                    double k3 = Tc - CommonData.Td1;
                    double k4 = CommonData.Td2 - CommonData.Tu2;
                    K = (k1 * k2) / (k3 * k4);
                    KBox.Text = K.ToString();
                    double k5 = CommonData.Td1 - CommonData.Td2;
                    double Tctest = CommonData.Td1 + ((k2 * k5) / (CommonData.Kchange * k4 - k2));
                    TctestBox.Text = Tctest.ToString();
                    double error = System.Math.Abs(Tc-Tctest);
                    errorBox.Text = error.ToString();

                    indexTamb += 2;
              
                    // 造一些数据，PointPairList里有数据对x，y的数
                    list_Te.Add(index, CommonData.Te); //添加一组数据
                    list_Teout.Add(index, CommonData.Teout);
                    list_Tc1.Add(index, Tc);
                    list_Tc2.Add(index, Tctest);

                    LineItem myCurve_Te = myPaneTamb.AddCurve(null, list_Te, Color.Red, SymbolType.Star);//新建并初始化曲线
                    LineItem myCurve_Teout = myPaneTamb.AddCurve(null, list_Teout, Color.Green, SymbolType.Circle);//新建并初始化曲线
                    LineItem myCurve_Tc1 = myPaneTamb.AddCurve(null, list_Tc1, Color.Blue, SymbolType.Diamond);//新建并初始化曲线
                    LineItem myCurve_Tc2 = myPaneTamb.AddCurve(null, list_Tc2, Color.Black, SymbolType.Square);//新建并初始化曲线

                    //填充图表颜色
                    myPaneTamb.PaneFill = new Fill(Color.White, Color.FromArgb(100, 100, 255), 35.0f);
                    //画到zedGraphControl1控件中，此句必加
                    zedGraphControl2.AxisChange();//调整x轴
                    //重绘控件
                    Refresh();

                }
            }
                ));
        }

        private void Clear_data_Click(object sender, EventArgs e)
        {
            showBox.Text = "";           
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void TcBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void save_Data(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt文件(*.txt)|*.txt";//设置文件后缀的过滤
            
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            //saveFileDialog1.FileName = "111";//设置默认文件名为111
            saveFileDialog1.InitialDirectory = System.Environment.CurrentDirectory.ToString();//设置默认目录为本程序目录

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, true);
                sw.Write(showBox.Text);
                sw.Close();
                MessageBox.Show("保存成功！");
            }          
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {                   
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            showBoxTamb.Text = "";
        }

        private void GroupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            //changeTbox.Text.
            CommonData.Kchange=Convert.ToDouble(changeTbox.Text);
        }
    }
}
