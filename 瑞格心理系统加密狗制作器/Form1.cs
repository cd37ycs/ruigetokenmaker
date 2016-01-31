using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace 瑞格心理系统加密狗制作器
{
    public struct GUID
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Data1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Data2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Data3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Data4;
    }

    public struct SP_INTERFACE_DEVICE_DATA
    {
        public int cbSize;
        public GUID InterfaceClassGuid;
        public int Flags;
        public int Reserved;
    }


    public struct SP_DEVINFO_DATA
    {
        public int cbSize;
        public GUID ClassGuid;
        public int DevInst;
        public int Reserved;
    }


    public struct SP_DEVICE_INTERFACE_DETAIL_DATA
    {
        public int cbSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
        public byte[] DevicePath;
    }


    public struct HIDD_ATTRIBUTES
    {
        public int Size;
        public ushort VendorID;
        public ushort ProductID;
        public ushort VersionNumber;
    }


    public struct HIDP_CAPS
    {
        public short Usage;
        public short UsagePage;
        public short InputReportByteLength;
        public short OutputReportByteLength;
        public short FeatureReportByteLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public short[] Reserved;
        public short NumberLinkCollectionNodes;
        public short NumberInputButtonCaps;
        public short NumberInputValueCaps;
        public short NumberInputDataIndices;
        public short NumberOutputButtonCaps;
        public short NumberOutputValueCaps;
        public short NumberOutputDataIndices;
        public short NumberFeatureButtonCaps;
        public short NumberFeatureValueCaps;
        public short NumberFeatureDataIndices;
    }

    enum Ry4Cmd : ushort
    {
        RY_FIND = 1,
        RY_FIND_NEXT,
        RY_OPEN,
        RY_CLOSE,
        RY_READ,
        RY_WRITE,
        RY_RANDOM,
        RY_SEED,
        RY_WRITE_USERID,
        RY_READ_USERID,
        RY_SET_MOUDLE,
        RY_CHECK_MOUDLE,
        RY_WRITE_ARITHMETIC,
        RY_CALCULATE1,
        RY_CALCULATE2,
        RY_CALCULATE3,
        RY_DECREASE
    };

    enum Ry4ErrCode : uint
    {
        ERR_SUCCESS = 0,
        ERR_NO_PARALLEL_PORT = 0x80300001,
        ERR_NO_DRIVER,
        ERR_NO_ROCKEY,
        ERR_INVALID_PASSWORD,
        ERR_INVALID_PASSWORD_OR_ID,
        ERR_SETID,
        ERR_INVALID_ADDR_OR_SIZE,
        ERR_UNKNOWN_COMMAND,
        ERR_NOTBELEVEL3,
        ERR_READ,
        ERR_WRITE,
        ERR_RANDOM,
        ERR_SEED,
        ERR_CALCULATE,
        ERR_NO_OPEN,
        ERR_OPEN_OVERFLOW,
        ERR_NOMORE,
        ERR_NEED_FIND,
        ERR_DECREASE,
        ERR_AR_BADCOMMAND,
        ERR_AR_UNKNOWN_OPCODE,
        ERR_AR_WRONGBEGIN,
        ERR_AR_WRONG_END,
        ERR_AR_VALUEOVERFLOW,

        ERR_UNKNOWN = 0x8030ffff,

        ERR_RECEIVE_NULL = 0x80300100,
        ERR_PRNPORT_BUSY = 0x80300101

    };

    public partial class Form1 : Form
    {
        private const ushort VID = 0x096E;
        private const ushort PID = 0x0006;
        private const short DIGCF_PRESENT = 0x2;
        private const short DIGCF_DEVICEINTERFACE = 0x10;
        private const short INVALID_HANDLE_VALUE = (-1);
        private const short ERROR_NO_MORE_ITEMS = 259;

        private const uint GENERIC_READ = 0x80000000;
        private const int GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x1;
        private const uint FILE_SHARE_WRITE = 0x2;
        private const uint OPEN_EXISTING = 3;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        private const uint INFINITE = 0xFFFF;

        [DllImport("HID.dll")]
        private static extern bool HidD_GetAttributes(int HidDeviceObject, ref HIDD_ATTRIBUTES Attributes);

        [DllImport("HID.dll")]
        private static extern int HidD_GetHidGuid(ref GUID HidGuid);

        [DllImport("HID.dll")]
        private static extern bool HidD_GetPreparsedData(int HidDeviceObject, ref int PreparsedData);

        [DllImport("HID.dll")]
        private static extern int HidP_GetCaps(int PreparsedData, ref HIDP_CAPS Capabilities);

        [DllImport("HID.dll")]
        private static extern bool HidD_FreePreparsedData(int PreparsedData);

        [DllImport("HID.dll")]
        private static extern bool HidD_SetFeature(int HidDeviceObject, byte[] ReportBuffer, int ReportBufferLength);

        [DllImport("HID.dll")]
        private static extern bool HidD_GetFeature(int HidDeviceObject, byte[] ReportBuffer, int ReportBufferLength);

        [DllImport("SetupApi.dll")]
        private static extern int SetupDiGetClassDevsA(ref GUID ClassGuid, int Enumerator, int hwndParent, int Flags);

        [DllImport("SetupApi.dll")]
        private static extern bool SetupDiDestroyDeviceInfoList(int DeviceInfoSet);

        [DllImport("SetupApi.dll")]
        private static extern bool SetupDiGetDeviceInterfaceDetailA(int DeviceInfoSet, ref  SP_INTERFACE_DEVICE_DATA DeviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, int DeviceInfoData);

        [DllImport("SetupApi.dll")]
        private static extern bool SetupDiEnumDeviceInterfaces(int DeviceInfoSet, int DeviceInfoData, ref GUID InterfaceClassGuid, int MemberIndex, ref SP_INTERFACE_DEVICE_DATA DeviceInterfaceData);

        [DllImport("kernel32.dll", EntryPoint = "CreateFileA")]
        private static extern int CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, uint lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, uint hTemplateFile);




        [DllImport("kernel32.dll")]
        private static extern int CloseHandle(int hObject);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        //以下函数用于将字节数组转化为宽字符串    
        private static string ByteConvertString(byte[] buffer)
        {
            char[] null_string = { '\0', '\0' };
            System.Text.Encoding encoding = System.Text.Encoding.Default;
            return encoding.GetString(buffer).TrimEnd(null_string);
        }
        //以下用于将16进制字符串转化为无符号长整型
        private uint HexToInt(string s)
        {
            string[] hexch = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F"};
            s = s.ToUpper();
            int i, j;
            int r, n, k;
            string ch;

            k = 1; r = 0;
            for (i = s.Length; i > 0; i--)
            {
                ch = s.Substring(i - 1, 1);
                n = 0;
                for (j = 0; j < 16; j++)
                    if (ch == hexch[j])
                        n = j;
                r += (n * k);
                k *= 16;
            }
            return unchecked((uint)r);
        }
        private int HexStringToByteArray(string InString, ref byte[] b)
        {
            int nlen;
            int retutn_len;
            int n, i;
            string temp;
            nlen = InString.Length;
            if (nlen < 16) retutn_len = 16;
            retutn_len = nlen / 2;
            b = new byte[retutn_len];
            i = 0;
            for (n = 0; n < nlen; n = n + 2)
            {
                temp = InString.Substring(n, 2);
                b[i] = (byte)HexToInt(temp);
                i = i + 1;
            }
            return retutn_len;
        }


        private bool FindRockey4ND(int pos, ref int count, ref string OutPath)
        {
            int hardwareDeviceInfo;
            SP_INTERFACE_DEVICE_DATA DeviceInfoData = new SP_INTERFACE_DEVICE_DATA();
            int i;
            GUID HidGuid = new GUID();
            SP_DEVICE_INTERFACE_DETAIL_DATA functionClassDeviceData = new SP_DEVICE_INTERFACE_DETAIL_DATA();
            int requiredLength;
            int d_handle;
            HIDD_ATTRIBUTES Attributes = new HIDD_ATTRIBUTES();

            i = 0;
            HidD_GetHidGuid(ref HidGuid);

            hardwareDeviceInfo = SetupDiGetClassDevsA(ref HidGuid, 0, 0, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

            if (hardwareDeviceInfo == INVALID_HANDLE_VALUE) return false;

            DeviceInfoData.cbSize = Marshal.SizeOf(DeviceInfoData);

            while (SetupDiEnumDeviceInterfaces(hardwareDeviceInfo, 0, ref HidGuid, i, ref DeviceInfoData))
            {
                if (GetLastError() == ERROR_NO_MORE_ITEMS) break;
                functionClassDeviceData.cbSize = Marshal.SizeOf(functionClassDeviceData) - 255;// 5;
                requiredLength = 0;
                if (!SetupDiGetDeviceInterfaceDetailA(hardwareDeviceInfo, ref DeviceInfoData, ref functionClassDeviceData, 300, ref requiredLength, 0))
                {
                    SetupDiDestroyDeviceInfoList(hardwareDeviceInfo);
                    return false;
                }
                OutPath = ByteConvertString(functionClassDeviceData.DevicePath);
                d_handle = CreateFile(OutPath, 0, FILE_SHARE_READ | FILE_SHARE_WRITE, 0, OPEN_EXISTING, 0, 0);
                if (INVALID_HANDLE_VALUE != d_handle)
                {
                    if (HidD_GetAttributes(d_handle, ref Attributes))
                    {
                        if ((Attributes.ProductID == PID) && (Attributes.VendorID == VID))
                        {
                            if (pos == count)
                            {
                                SetupDiDestroyDeviceInfoList(hardwareDeviceInfo);
                                CloseHandle(d_handle);
                                return true;
                            }
                            count = count + 1;
                        }
                    }
                    CloseHandle(d_handle);
                }
                i = i + 1;
            }
            SetupDiDestroyDeviceInfoList(hardwareDeviceInfo);
            return false;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void WriteDogdata()
        {
            byte[] buffer = new byte[1000];
            ushort handle = 0;
            ushort function = 0;
            ushort p1 = 0;
            ushort p2 = 0;
            ushort p3 = 0;
            ushort p4 = 0;
            uint lp1 = 0;
            uint lp2 = 0;

            int iMaxRockey = 0;
            uint[] uiarrRy4ID = new uint[32];
            uint iCurrID;


            p1 = 0xbcef; p2 = 0x43b0; p3 = 0xbcef; p4 = 0x43b0;
            Rockey4NDClass.Rockey4ND R4nd = new Rockey4NDClass.Rockey4ND();
            R4nd.Rockey(function, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);
            ushort ret = R4nd.Rockey((ushort)Ry4Cmd.RY_FIND, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);

            uiarrRy4ID[iMaxRockey] = lp1;
            iMaxRockey++;
            if (lp1 < 1)
                listBox.Items.Add("未发现加密锁硬件!");
            else
            {

                ushort flag = 0;
                while (flag == 0)
                {
                    flag = R4nd.Rockey((ushort)Ry4Cmd.RY_FIND_NEXT, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);

                    if (flag == 0)
                    {
                        uiarrRy4ID[iMaxRockey] = lp1;
                        iMaxRockey++;
                    }

                }

                string strRet = "找到 " + iMaxRockey + " 个加密锁,对应硬件id如下："+"\r\n";
                listBox.Items.Add(strRet);

                for (int i = 0; i < iMaxRockey; i++)
                {
                    strRet = string.Format("({0}): {1:X8}", i + 1, uiarrRy4ID[i]) + "\r\n";
                    listBox.Items.Add(strRet);

                }
                iCurrID = uiarrRy4ID[0];
                for (int n = 0; n < iMaxRockey; n++)
                {
                    lp1 = uiarrRy4ID[n];
                    p1 = 0xc44c; p2 = 0xc8f8; p3 = 0x0799; p4 = 0xc43b;

                    R4nd.Rockey((ushort)Ry4Cmd.RY_OPEN, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);

                    for (int i = 0; i < 500; i++) buffer[i] = (byte)0xFF;
                    p1 = 0; p2 = 500;//r4nd用户内存分两页，每页500字节，从0开始计数
                    R4nd.Rockey((ushort)Ry4Cmd.RY_WRITE, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);
                    p1 = 500; p2 = 500;//r4nd用户内存分两页，每页500字节，从0开始计数
                    R4nd.Rockey((ushort)Ry4Cmd.RY_WRITE, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);

                    //写入数据
                    buffer[0] = (byte)0x50;
                    buffer[1] = (byte)0x73;
                    buffer[2] = (byte)0x79;
                    buffer[3] = (byte)0x33;
                    for (int i = 4; i < 20; i++) buffer[i] = (byte)0x00;
                    listBox.Items.Add((string)("写入数据..." + "\r\n"));
                    p1 = 0; p2 = 20;//r4nd用户内存分两页，每页500字节，从0开始计数
                    R4nd.Rockey((ushort)Ry4Cmd.RY_WRITE, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);
                    p1 = 500; p2 = 20;//r4nd用户内存分两页，每页500字节，从0开始计数
                    R4nd.Rockey((ushort)Ry4Cmd.RY_WRITE, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);

                    strRet = string.Format("读取数据...") + "\r\n";
                    //listBox.Items.Add(strRet);
                    p1 = 500; p2 = 20;
                    R4nd.Rockey((ushort)Ry4Cmd.RY_READ, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);
                    strRet += System.Text.Encoding.Default.GetString(buffer);
                    listBox.Items.Add(strRet);

                    //Demo8:Write UID to 0x19303A78.
                    //写入用户ID
                    strRet = string.Format("写入数据ID...") + "\r\n";
                    listBox.Items.Add(strRet);
                    lp1 = 0x3001001;
                    R4nd.Rockey((ushort)Ry4Cmd.RY_WRITE_USERID, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);
                    lp1 = 0;

                    //Demo9: Read UID.
                    strRet = string.Format("读取数据ID...") + "\r\n";
                    R4nd.Rockey((ushort)Ry4Cmd.RY_READ_USERID, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);
                    strRet += string.Format("数据ID = {0:x7} ", lp1) + "\r\n";
                    listBox.Items.Add(strRet);

                    //Demo10:Set module 0x08, number is 3, can be decreased.
                    strRet = string.Format("设置模块及算法...") + "\r\n";
                    listBox.Items.Add(strRet);                    
                    for (int i = 0; i < 64; i++)
                    {
                        p1 = (ushort)i; p2 = 1; p3 = 0;
                        R4nd.Rockey((ushort)Ry4Cmd.RY_SET_MOUDLE, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);
                    }

                    //Demo11:Check module. p1=module index
                    strRet = string.Format("检查模块及算法...") + "\r\n";
                    listBox.Items.Add(strRet);
                    for (int i = 0; i < 64; i++)
                    {
                        p1 = (ushort)i;
                        R4nd.Rockey((ushort)Ry4Cmd.RY_CHECK_MOUDLE, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);
                        strRet = string.Format("模块号={0:X2}, 模块内容={1}, 是否可递减={2}", p1, p2, p3) + "\r\n";
                        listBox.Items.Add(strRet);
                    }                    

                    //Demo17,Close.
                    strRet = string.Format("关闭加密锁硬件 {0:X4}...", handle) + "\r\n";
                    listBox.Items.Add(strRet);
                    R4nd.Rockey((ushort)Ry4Cmd.RY_CLOSE, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);

                    strRet = string.Format("加密锁制作完成！") + "\r\n";
                    listBox.Items.Add(strRet);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //
            string OutPath = "";
            bool ret;
            int count = 0;
            int hUsbDevice;
            byte temp;
            //textBox1.Text = "";
            ret = FindRockey4ND(0, ref count, ref OutPath);
            if (!ret)
            {
                listBox.Items.Add("没有找到加密锁设备" + "\r\n");
                return;
            }
            else
            {
                listBox.Items.Add("找到加密锁设备..." + "\r\n");
                hUsbDevice = CreateFile(OutPath, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, 0, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, 0);
                if (hUsbDevice == INVALID_HANDLE_VALUE)
                {
                    listBox.Items.Add("加密锁设备打开错误！" + "\r\n");
                    return;
                }
                else
                {
                    listBox.Items.Add("开始准备所需数据..." + "\r\n");
                    Random rd = new Random();
                    temp = Convert.ToByte(rd.Next() % 256);
                    byte[] p1 = new byte[2];
                    byte[] p2 = new byte[2];
                    byte[] p3 = new byte[2];
                    byte[] p4 = new byte[2];
                    byte[] hid = new byte[4];
                    byte[] inData = new byte[25];
                    HexStringToByteArray("bcef", ref p1);
                    HexStringToByteArray("43b0", ref p2);
                    HexStringToByteArray("bcef", ref p3);
                    HexStringToByteArray("43b0", ref p4);
                    HexStringToByteArray("80818496", ref hid);

                    inData[0] = 0;
                    inData[1] = 0x81;//命令字
                    inData[2] = temp;//随机数
                    inData[3] = Convert.ToByte(temp ^ 0xAC);//密钥
                    inData[4] = Convert.ToByte(temp ^ 0x87);
                    inData[5] = Convert.ToByte(temp ^ 0x3D);
                    inData[6] = Convert.ToByte(temp ^ p1[0]);
                    inData[7] = Convert.ToByte(temp ^ p1[1]);
                    inData[8] = Convert.ToByte(temp ^ p2[0]);
                    inData[9] = Convert.ToByte(temp ^ p2[1]);
                    inData[10] = Convert.ToByte(temp ^ p3[0]);
                    inData[11] = Convert.ToByte(temp ^ p3[1]);
                    inData[12] = Convert.ToByte(temp ^ p4[0]);
                    inData[13] = Convert.ToByte(temp ^ p4[1]);
                    inData[14] = Convert.ToByte(temp ^ hid[0]);
                    inData[15] = Convert.ToByte(temp ^ hid[1]);
                    inData[16] = Convert.ToByte(temp ^ hid[2]);
                    inData[17] = Convert.ToByte(temp ^ hid[3]);
                    inData[18] = temp;
                    inData[19] = temp;
                    inData[20] = temp;
                    inData[21] = temp;
                    inData[22] = temp;
                    inData[23] = temp;
                    inData[24] = Convert.ToByte(inData[1] ^ inData[2] ^ inData[3] ^ inData[4] ^ inData[5] ^ inData[6] ^ inData[7] ^ inData[8] ^ inData[9] ^ inData[10] ^ inData[11] ^ inData[12] ^ inData[13] ^ inData[14] ^ inData[15] ^ inData[16] ^ inData[17]);

                    ret = HidD_SetFeature(hUsbDevice, inData, 25);
                    if (ret)
                        listBox.Items.Add("硬件信息写入成功！" + "\r\n");
                    else
                        listBox.Items.Add("硬件信息写入失败！" + "\r\n");
                }
                CloseHandle(hUsbDevice);
            }
            
            //以上修改r4nd的hid和密码
            //以下写入数据
            WriteDogdata();
            //数据写入完毕

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
