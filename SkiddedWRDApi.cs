using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WeAreDevs_API
{
    public class ExploitAPI
    {
        private WebClient client = new WebClient();

        private ExploitAPI.BasicInject injector = new ExploitAPI.BasicInject();

        private string luapipe = "WeAreDevsPublicAPI_Lua";

        private string luacpipe = "WeAreDevsPublicAPI_LuaC";

        public ExploitAPI()
        {
        }

        public void AddFire(string username = "me")
        {
            this.SendLuaScript("Instance.new(\"Fire\", game:GetService(\"Players\").LocalPlayer.Character.HumanoidRootPart)");
        }

        public void AddForcefield(string username = "me")
        {
            this.SendLuaScript("Instance.new(\"ForceField\", game:GetService(\"Players\").LocalPlayer.Character)");
        }

        public void AddSmoke(string username = "me")
        {
            this.SendLuaScript("Instance.new(\"Smoke\", game:GetService(\"Players\").LocalPlayer.Character.HumanoidRootPart)");
        }

        public void AddSparkles(string username = "me")
        {
            this.SendLuaScript("Instance.new(\"Sparkles\", game:GetService(\"Players\").LocalPlayer.Character.HumanoidRootPart)");
        }

        public void ConsoleError(string text = "")
        {
            this.SendLuaScript(string.Concat("rconsoleerr ", text));
        }

        public void ConsolePrint(string text = "")
        {
            this.SendLuaScript(string.Concat("rconsoleprint ", text));
        }

        public void ConsoleWarn(string text = "")
        {
            this.SendLuaScript(string.Concat("rconsolewarn ", text));
        }

        public void DoBlockHead(string username = "me")
        {
            this.SendLuaScript("game:GetService(\"Players\").LocalPlayer.Character.Head.Mesh:Destroy()");
        }

        public void DoBTools(string username = "me")
        {
            this.SendLuaScript("loadstring(game:HttpGet(\"https://cdn.wearedevs.net/scripts/BTools.txt\"))()");
        }

        private bool DownloadLatestVersion()
        {
            if (File.Exists("exploit-main.dll"))
            {
                File.Delete("exploit-main.dll");
            }
            string latestData = this.GetLatestData();
            if (latestData.Length > 0)
            {
                this.client.DownloadFile(latestData.Split(new char[] { ' ' })[1], "exploit-main.dll");
            }
            if (File.Exists("exploit-main.dll"))
            {
                return true;
            }
            return false;
        }

        private string GetLatestData()
        {
            string str = this.ReadURL("https://cdn.wearedevs.net/software/exploitapi/latestdata.txt");
            if (str.Length > 0)
            {
                return str;
            }
            string str1 = this.ReadURL("https://pastebin.com/raw/Ly9mJwH7");
            if (str1.Length > 0)
            {
                return str1;
            }
            return "";
        }

        public bool isAPIAttached()
        {
            if (ExploitAPI.NamedPipeExist(this.luapipe))
            {
                return true;
            }
            return false;
        }

        public bool IsUpdated()
        {
            bool flag = false;
            string latestData = this.GetLatestData();
            if (latestData.Length <= 0)
            {
                MessageBox.Show("Could not check for the latest version. Did your fireall block us?", "Error");
            }
            else
            {
                flag = Convert.ToBoolean(latestData.Split(new char[] { ' ' })[0]);
            }
            return flag;
        }

        public bool LaunchExploit()
        {
            if (ExploitAPI.NamedPipeExist(this.luapipe))
            {
                MessageBox.Show("Dll already injected", "No problems");
            }
            else if (!this.IsUpdated())
            {
                MessageBox.Show("Exploit is currently patched... Please wait for the developers to fix it! Meanwhile, check wearedevs.net for updates/info.", "Error");
            }
            else if (!this.DownloadLatestVersion())
            {
                MessageBox.Show("Could not download the latest version! Did your firewall block us?", "Error");
            }
            else
            {
                if (this.injector.InjectDLL())
                {
                    return true;
                }
                MessageBox.Show("DLL failed to inject", "Error");
            }
            return false;
        }

        public void LuaC_getfield(int index, string instance)
        {
            this.SendLuaCScript(string.Concat("getglobal ", index.ToString(), " ", instance));
        }

        public void LuaC_getglobal(string service)
        {
            this.SendLuaCScript(string.Concat("getglobal ", service));
        }

        public void LuaC_gettop()
        {
            this.SendLuaCScript("gettop");
        }

        public void LuaC_next(int index)
        {
            this.SendLuaCScript("next");
        }

        public void LuaC_pcall(int numberOfArguments, int numberOfResults, int ErrorFunction)
        {
            this.SendLuaCScript(string.Concat(new string[] { "pushnumber ", numberOfArguments.ToString(), " ", numberOfResults.ToString(), " ", ErrorFunction.ToString() }));
        }

        public void LuaC_pop(int quantity)
        {
            this.SendLuaCScript(string.Concat("pop ", quantity.ToString()));
        }

        public void LuaC_pushboolean(string value = "false")
        {
            this.SendLuaCScript(string.Concat("pushboolean ", value));
        }

        public void LuaC_pushnil()
        {
            this.SendLuaCScript("pushnil");
        }

        public void LuaC_pushnumber(int number)
        {
            this.SendLuaCScript(string.Concat("pushnumber ", number.ToString()));
        }

        public void LuaC_pushstring(string text)
        {
            this.SendLuaCScript(string.Concat("pushstring ", text));
        }

        public void LuaC_pushvalue(int index)
        {
            this.SendLuaCScript(string.Concat("pushvalue ", index.ToString()));
        }

        public void LuaC_setfield(int index, string property)
        {
            this.SendLuaCScript(string.Concat("setfield ", index.ToString(), " ", property));
        }

        public void LuaC_settop(int index)
        {
            this.SendLuaCScript(string.Concat("settop ", index.ToString()));
        }

        public static bool NamedPipeExist(string pipeName)
        {
            bool flag;
            try
            {
                if (!ExploitAPI.WaitNamedPipe(Path.GetFullPath(string.Format("\\\\.\\pipe\\{0}", pipeName)), 0))
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    if (lastWin32Error == 0)
                    {
                        flag = false;
                        return flag;
                    }
                    else if (lastWin32Error == 2)
                    {
                        flag = false;
                        return flag;
                    }
                }
                flag = true;
            }
            catch (Exception exception)
            {
                flag = false;
            }
            return flag;
        }

        private string ReadURL(string url)
        {
            return this.client.DownloadString(url);
        }

        public void RemoveArms(string username = "me")
        {
            this.SendLuaScript("loadstring(game:HttpGet(\"https://cdn.wearedevs.net/scripts/Remove Arms.txt\"))()");
        }

        public void RemoveFire(string username = "me")
        {
            this.SendLuaScript("game:GetService(\"Players\").LocalPlayer.Character.HumanoidRootPart.Fire:Destroy()");
        }

        public void RemoveForceField(string username = "me")
        {
            this.SendLuaScript("game:GetService(\"Players\").LocalPlayer.Character.ForceField:Destroy()");
        }

        public void RemoveLegs(string username = "me")
        {
            this.SendLuaScript("loadstring(game:HttpGet(\"https://cdn.wearedevs.net/scripts/Remove Legs.txt\"))()");
        }

        public void RemoveLimbs(string username = "me")
        {
            this.SendLuaScript("loadstring(game:HttpGet(\"https://cdn.wearedevs.net/scripts/Remove Arms.txt\"))()");
            this.SendLuaScript("loadstring(game:HttpGet(\"https://cdn.wearedevs.net/scripts/Remove Legs.txt\"))()");
        }

        public void RemoveSmoke(string username = "me")
        {
            this.SendLuaScript("game:GetService(\"Players\").LocalPlayer.Character.HumanoidRootPart.Smoke:Destroy()");
        }

        public void RemoveSparkles(string username = "me")
        {
            this.SendLuaScript("game:GetService(\"Players\").LocalPlayer.Character.HumanoidRootPart.Sparkles:Destroy()");
        }

        [Obsolete("SendLimitedLuaScript is deprecated, please use SendLuaScript instead.")]
        public void SendLimitedLuaScript(string script)
        {
            this.SendLuaScript(script);
        }

        public void SendLuaCScript(string Script)
        {
            string[] strArrays = Script.Split("\r\n".ToCharArray());
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];
                try
                {
                    this.SMTP(this.luacpipe, str);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message.ToString());
                }
            }
        }

        public void SendLuaScript(string Script)
        {
            this.SMTP(this.luapipe, Script);
        }

        [Obsolete("SendScript is deprecated, please use SendLuaCScript instead.")]
        public void SendScript(string script)
        {
            this.SendLuaCScript(script);
        }

        public void SetFogEnd(int value = 0)
        {
            this.SendLuaScript(string.Concat("game:GetService(\"Lighting\").FogEnd = ", value.ToString()));
        }

        public void SetFogStart(int value = 0)
        {
            this.SendLuaScript(string.Concat("game:GetService(\"Lighting\").FogStart = ", value.ToString()));
        }

        public void SetJumpPower(int value = 100)
        {
            this.SendLuaScript(string.Concat("game:GetService(\"Players\").LocalPlayer.Character.Humanoid.JumpPower = ", value.ToString()));
        }

        public void SetWalkSpeed(string username = "me", int value = 100)
        {
            this.SendLuaScript(string.Concat("game:GetService(\"Players\").LocalPlayer.Character.Humanoid.WalkSpeed = ", value.ToString()));
        }

        private void SMTP(string pipe, string input)
        {
            if (!ExploitAPI.NamedPipeExist(pipe))
            {
                MessageBox.Show("Error occured. Did the dll properly inject?", "Oops", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", pipe, PipeDirection.Out))
                    {
                        namedPipeClientStream.Connect();
                        using (StreamWriter streamWriter = new StreamWriter(namedPipeClientStream))
                        {
                            streamWriter.Write(input);
                            streamWriter.Dispose();
                        }
                        namedPipeClientStream.Dispose();
                    }
                }
                catch (IOException oException)
                {
                    MessageBox.Show("Error occured sending message to the game!", "Connection Failed!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message.ToString());
                }
            }
        }

        public void Suicide(string username = "me")
        {
            this.SendLuaScript("game:GetService(\"Players\").LocalPlayer.Character:BreakJoints()");
        }

        public void TeleportToPlayer(string targetUsername = "me")
        {
            this.SendLuaScript(string.Concat("game:GetService(\"Players\").LocalPlayer.Character:MoveTo(game:GetService(\"Players\"):FindFirstChild(", targetUsername, ").Character.HumanoidRootPart.Position)"));
        }

        public void ToggleClickTeleport()
        {
            this.SendLuaScript("loadstring(game:HttpGet(\"https://cdn.wearedevs.net/scripts/Click Teleport.txt\"))()");
        }

        public void ToggleFloat(string username = "me")
        {
            this.SendLuaScript("loadstring(game:HttpGet(\"https://cdn.wearedevs.net/scripts/Float Character.txt\"))()");
        }

        [DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
        private static extern bool WaitNamedPipe(string name, int timeout);

        private class BasicInject
        {
            public BasicInject()
            {
            }

            [DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
            internal static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, UIntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

            [DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
            internal static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32", CharSet=CharSet.Ansi, ExactSpelling=true, SetLastError=true)]
            internal static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

            public bool InjectDLL()
            {
                UIntPtr uIntPtr;
                IntPtr intPtr;
                if (Process.GetProcessesByName("RobloxPlayerBeta").Length == 0)
                {
                    return false;
                }
                Process processesByName = Process.GetProcessesByName("RobloxPlayerBeta")[0];
                byte[] bytes = (new ASCIIEncoding()).GetBytes(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "exploit-main.dll"));
                IntPtr intPtr1 = ExploitAPI.BasicInject.LoadLibraryA("kernel32.dll");
                UIntPtr procAddress = ExploitAPI.BasicInject.GetProcAddress(intPtr1, "LoadLibraryA");
                ExploitAPI.BasicInject.FreeLibrary(intPtr1);
                if (procAddress == UIntPtr.Zero)
                {
                    return false;
                }
                IntPtr intPtr2 = ExploitAPI.BasicInject.OpenProcess(ExploitAPI.BasicInject.ProcessAccess.AllAccess, false, processesByName.Id);
                if (intPtr2 == IntPtr.Zero)
                {
                    return false;
                }
                IntPtr intPtr3 = ExploitAPI.BasicInject.VirtualAllocEx(intPtr2, (IntPtr)0, (uint)bytes.Length, 12288, 4);
                if (intPtr3 == IntPtr.Zero)
                {
                    return false;
                }
                if (!ExploitAPI.BasicInject.WriteProcessMemory(intPtr2, intPtr3, bytes, (uint)bytes.Length, out uIntPtr))
                {
                    return false;
                }
                if (ExploitAPI.BasicInject.CreateRemoteThread(intPtr2, (IntPtr)0, 0, procAddress, intPtr3, 0, out intPtr) == IntPtr.Zero)
                {
                    return false;
                }
                return true;
            }

            [DllImport("kernel32", CharSet=CharSet.Ansi, ExactSpelling=false, SetLastError=true)]
            internal static extern IntPtr LoadLibraryA(string lpFileName);

            [DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
            internal static extern IntPtr OpenProcess(ExploitAPI.BasicInject.ProcessAccess dwDesiredAccess, bool bInheritHandle, int dwProcessId);

            [DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
            internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

            [DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=true, SetLastError=true)]
            internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

            [DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
            internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

            [Flags]
            public enum ProcessAccess
            {
                Terminate = 1,
                CreateThread = 2,
                VMOperation = 8,
                VMRead = 16,
                VMWrite = 32,
                DuplicateHandle = 64,
                SetInformation = 512,
                QueryInformation = 1024,
                Synchronize = 1048576,
                AllAccess = 1050235
            }
        }
    }
}
