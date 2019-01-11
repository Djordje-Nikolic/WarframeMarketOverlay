using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace George_Application_Simulation
{
    public partial class Form1 : Form
    {
        private const int WM_COPYDATA = 74; // c# doesn't have a predefined macro for the WM_COPYDATA message, so we create one here
        private IntPtr ScreenShot; // ScreenShot in unmanaged memory, it is c#'s responsibility to release this before cloasing

        private IntPtr ScreenShot_Resolution_X; // int representing the screenshot's width in unmanaged memory
        private IntPtr ScreenShot_Resolution_Y; // int representing the screenshot's height in unmanaged memory

        private IntPtr ExitCode_Pointer; // the WarFrame_OCR API will write to this unmanaged int pointer what went wrong
        private IntPtr DirectX_Thread_Handle; // Pointer to an unmanaged part of the memory, a handle to the background DirectX Thread returned by WarFrame_OCR_Initialize
        private bool WarFrame_OCR_ShutDown_Called; // used in order to know if we already called WarFrame_OCR_ShutDown since it needs to be called once and there are two places where we can call it, we need to call it if the user presses the button or if he closes out of the application while the background thread is running
        private IntPtr CSharp_Window_Name; // passed to WarFrame_OCR so it knows where to send over results
        private int Csharp_Window_Name_Length; // passed to WarFrame_OCR so it can read CSharp_Window_Name;

        private List<string> Detected_Text;

        // ExitCode Meanings
        // 1 - Could not create GraphicClass Class
        // 2 - Could now create MD3D Class
        // 3 - M3D Could not CoInitialize
        // 4 - MD3D Could not D3D11CreateDevice
        // 5 - Device Does not support DX feature level 11
        // 6  -Failed to Compile the Direct Compute Shader STAGE ONE PART 01
        // 7 - Cannot create ComputeShader STAGE ONE PART 01
        // 8 - Failed to Compile the Direct Compute Shader STAGE ONE PART 02
        // 9 - Cannot create ComputeShader STAGE ONE PART 02
        // 10 - Items.txt does not exist
        // 11 - Cannot create Texture2D for UAV Initialization
        // 12 - Cannot create ComputeShader UnOrderedAccessView
        // 13 - Cannot Create Texture2D Out of received ScreenShot
        // 14 - Cannot Create ShaderResourceView out of the Received ScreenShot
        // 15 - Cannot Create UINT32 Texture2D for the GPU to write to 
        // 16 - Cannot Create UAV out of the UINT32 Texture2D
        // 17 - Cannot Create CPU READ Texture2D for the UINT32 Texture
        // 18 - Cannot Create CPU READ Texture for the final result output to tesseract
        // 19 - Cannot Create UINT32 Texture2D for multithread algorythm output
        // 20 - Cannot Create ShaderResourceView for the UINT32 Texture2D for the multithreading algorythm output
        // 21 - Could not initialize the English Language for Tesseract
        // 22 - Cannot send cleaned tesseract output to George's Window
        // 23 - Cannot Find George's Form window in order to send a message to him


        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        struct COPYDATASTRUCT
        {
            public uint dwData;
            public int cbData;
            public IntPtr lpData;
        }

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        [DllImport("WarFrame_OCR.dll", EntryPoint = "WarFrame_OCR_Initialize", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WarFrame_OCR_Initialize(IntPtr ExitCode_Pointer, IntPtr CSharp_Window_Name_C_String, int CSharp_Window_Name_C_String_Length, bool is_warframe_x86);

        [DllImport("WarFrame_OCR.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WarFrame_OCR_Detect_Text(IntPtr Thread_Parameters, IntPtr ScreenShot, int ScreenShot_Resolution_X, int ScreenShot_Resolution_Y);

        [DllImport("WarFrame_OCR.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WarFrame_OCR_ShutDown(IntPtr Thread_Parameters, bool Is_Warframe_X86);

        [DllImport("WarFrame_OCR.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Warframe_OCR_Inject_Hook();

        [DllImport("WarFrame_OCR.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Warframe_OCR_Remove_Hook();


        IntPtr Convert_To_Unmanaged_Char_Array(string str, out int array_length)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            IntPtr result = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, result, bytes.Length);
            array_length = bytes.Length;
            return result;
        }



        public Form1()
        {
            InitializeComponent();

            /////////////////
            Detected_Text = new List<string>();
            button3.BackColor = Color.Red;
            button1.Enabled = false;
            button2.Enabled = false;
            button4.Enabled = false;
            WarFrame_OCR_ShutDown_Called = false;
            /////////////////

            unsafe
            {
                ExitCode_Pointer = Marshal.AllocHGlobal(sizeof(int));
                CSharp_Window_Name = Convert_To_Unmanaged_Char_Array("Form1", out Csharp_Window_Name_Length);
                DirectX_Thread_Handle = WarFrame_OCR_Initialize(ExitCode_Pointer, CSharp_Window_Name, Csharp_Window_Name_Length, false);

                int temp_exit_code = *((int*)(ExitCode_Pointer));
                textBox3.Text = temp_exit_code.ToString();

                if (temp_exit_code == 0)
                {
                    button1.Enabled = true;
                    button4.Enabled = true;
                }
                else
                {
                    DirectX_Thread_Handle = IntPtr.Zero;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            // Listen for operating system messages.
            switch (m.Msg)
            {
                case WM_COPYDATA:
                {
                        unsafe
                        {
                            COPYDATASTRUCT* pcds = (COPYDATASTRUCT*)m.LParam;
                            if (pcds->dwData == 666)
                            {
                                if (ScreenShot != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(ScreenShot);
                                }
                                ScreenShot = Marshal.AllocHGlobal(pcds->cbData);
                                CopyMemory(ScreenShot, pcds->lpData, (uint)pcds->cbData);

                                /////////////////
                                button3.BackColor = Color.Green;
                                button2.Enabled = true;
                                textBox4.Text = pcds->cbData.ToString();
                                /////////////////
                            }
                            if (pcds->dwData == 667) // receiving ScreenShot_Resolution_X
                            {
                                if (ScreenShot_Resolution_X != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(ScreenShot_Resolution_X);
                                }
                                ScreenShot_Resolution_X = Marshal.AllocHGlobal(pcds->cbData);
                                CopyMemory(ScreenShot_Resolution_X, pcds->lpData, (uint)(pcds->cbData));
                                textBox1.Text = (*((int*)(ScreenShot_Resolution_X))).ToString();
                            }
                            if (pcds->dwData == 668) // receiving ScreenShot_Resolution_Y
                            {
                                if (ScreenShot_Resolution_Y != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(ScreenShot_Resolution_Y);
                                }
                                ScreenShot_Resolution_Y = Marshal.AllocHGlobal(pcds->cbData);
                                CopyMemory(ScreenShot_Resolution_Y, pcds->lpData, (uint)(pcds->cbData));
                                textBox2.Text = (*((int*)(ScreenShot_Resolution_Y))).ToString();
                            }
                            if (pcds->dwData == 564) // Receiving Detected Text
                            {
                                var result = new StringBuilder();
                                for (var i = 0; i < pcds->cbData; i++)
                                {
                                    result.Append((char)Marshal.ReadByte(pcds->lpData, i));
                                }
                             
                                Detected_Text.Add(result.ToString());
                               
                                listBox1.Items.Add(Detected_Text.Last());
                            }
                            // This message means an error occurred on the DirectX thread
                            // !!!!!!! WHEN AN ERROR OCCURS THE DIRECTX THEAD CLEARS IT'S MEMORY AND SHUTSDOWN
                            // WE NEED TO UPDATE THE POINTERS ON OUR SIDE TO REFLECT THAT AS WELL !!!!!!!!!
                            // should also update our ErrorCode Text Box display since an error accured
                            if (pcds->dwData == 777) // Update ErrorCode Display
                            {
                                int temp_exit_code = *((int*)(ExitCode_Pointer));
                                textBox3.Text = temp_exit_code.ToString();
                                DirectX_Thread_Handle = IntPtr.Zero;
                                WarFrame_OCR_ShutDown_Called = true;
                            }
                        }
                        return;
                }
            }
            base.WndProc(ref m);
            return;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            unsafe
            {
                WarFrame_OCR_Detect_Text(DirectX_Thread_Handle, ScreenShot, *((int*)(ScreenShot_Resolution_X)), *((int*)(ScreenShot_Resolution_Y)));
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WarFrame_OCR_ShutDown_Called == false)
            {
                WarFrame_OCR_ShutDown(DirectX_Thread_Handle, false);
                DirectX_Thread_Handle = IntPtr.Zero;
                WarFrame_OCR_ShutDown_Called = true;
            }
            if (ScreenShot != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ScreenShot);
            }
            if (ScreenShot_Resolution_X != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ScreenShot_Resolution_X);
            }
            if (ScreenShot_Resolution_Y != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ScreenShot_Resolution_Y);
            }
            if (ExitCode_Pointer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ExitCode_Pointer);
            }
            if (CSharp_Window_Name != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(CSharp_Window_Name);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Warframe_OCR_Inject_Hook();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (WarFrame_OCR_ShutDown_Called == false)
            {
                WarFrame_OCR_ShutDown(DirectX_Thread_Handle, false);
                DirectX_Thread_Handle = IntPtr.Zero;
                WarFrame_OCR_ShutDown_Called = true;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        
       
    }
}
