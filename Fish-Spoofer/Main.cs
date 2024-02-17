using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Fish_Spoofer
{
    public class Program
    {
        public static string KDMapperURL = "https://cdn.discordapp.com/attachments/1181389681775104020/1207941305595076659/kdmapper.exe?ex=65e17a01&is=65cf0501&hm=d396fd009cf4d45a50165053001f8234ef0e50476b09379ce6e6e480aa12f017&";
        public static string drvURL = "https://cdn.discordapp.com/attachments/1181389681775104020/1207941334820716554/HelloWorld.sys?ex=65e17a08&is=65cf0508&hm=dc4e484831df412337970e4622e416fa6b7a2583354c8bdfb13197dd52e2677b&";
        public static string MapExePath = @"C:\Windows\map.exe";
        public static string MapdrvPath = @"C:\Windows\mapdrv.sys";

        static void Main(string[] args)
        {
            if (!Utils.IsAdministrator()) { Utils.RunAsAdministrator(); return; }

            Console.Clear();
            Console.Title = "Fish Spoofer";
            Console.WriteLine();
            UI.WriteLineAlt("Fish Spoofer");
            UI.WriteSpacing(true);
            UI.WriteBarrierLine("1", "Temp Spoof");
            UI.WriteBarrierLine("2", "Chang MAC");
            UI.WriteBarrierLine("3", "PC Spoof");
            UI.WriteBarrierLine("4", "Network");
            UI.WriteSpacing(true);
            UI.WriteSpacing(false);
            Console.Write("   -> ");
            string choice = Console.ReadLine();
            if (choice == "1") { TempSpoof(); }
            if (choice == "2") { MACSpoof(); }
            if (choice == "3") { PCSpoof(); }
            if (choice == "4") { NetworkSpoof(); }
        }
        public static void TempSpoof()
        {
            Console.Clear();
            Console.Title = "Fish Spoofer";
            Console.WriteLine();
            UI.WriteLineAlt("Fish Spoofer");
            UI.WriteSpacing(true);
            UI.WriteBarrierLine("0", "Back");
            UI.WriteBarrierLine("1", "EasyAntiCheat");
            UI.WriteBarrierLine("2", "BattleEye");
            UI.WriteSpacing(true);
            UI.WriteSpacing(false);
            Console.Write("   -> ");
            string choice = Console.ReadLine();
            if (choice == "0") { Main(null); }
            if (choice == "1") { Spoof.SpoofEAC(); }
            else if (choice == "2") { Spoof.SpoofBE(); }
            TempSpoof();
        }
        public static void MACSpoof()
        {
            Console.Clear();
            Console.Title = "Fish Spoofer";
            Console.WriteLine();
            UI.WriteLineAlt("Fish Spoofer");
            UI.WriteSpacing(true);
            UI.WriteBarrierLine("0", "Back");
            UI.WriteBarrierLine("1", "Custom MAC");
            UI.WriteBarrierLine("2", "Random MAC");
            UI.WriteSpacing(true);
            UI.WriteSpacing(false);
            Console.Write("   -> ");
            string choice = Console.ReadLine();
            if (choice == "0") { Main(null); }
            if (choice == "1")
            {
                UI.WriteLine("Put Your MAC You Want");
                UI.WriteSpacing(true);
                UI.WriteSpacing(false);
                Console.Write("   -> ");
                string mac = Console.ReadLine();
                Spoof.SpoofMAC(false, mac);
            }
            else if (choice == "2")
            {
                Spoof.SpoofMAC(true, null);
            }
            MACSpoof();
        }
        public static void PCSpoof()
        {
            Console.Clear();
            Console.Title = "Fish Spoofer";
            Console.WriteLine();
            UI.WriteLineAlt("Fish Spoofer");
            UI.WriteSpacing(true);
            UI.WriteBarrierLine("0", "Back");
            UI.WriteBarrierLine("1", "Spoof Disks");
            UI.WriteBarrierLine("2", "Spoof GUID");
            UI.WriteSpacing(true);
            UI.WriteSpacing(false);
            Console.Write("   -> ");
            string choice = Console.ReadLine();
            if (choice == "0") { Main(null); }
            if (choice == "1") { Spoof.SpoofDisks(); }
            if (choice == "2") { Spoof.SpoofGUID(); }
            PCSpoof();
        }
        public static void NetworkSpoof()
        {
            Console.Clear();
            Console.Title = "Fish Spoofer";
            Console.WriteLine();
            UI.WriteLineAlt("Fish Spoofer");
            UI.WriteSpacing(true);
            UI.WriteBarrierLine("0", "Back");
            UI.WriteBarrierLine("1", "Fix Network");
            UI.WriteBarrierLine("2", "Flush DNS");
            UI.WriteSpacing(true);
            UI.WriteSpacing(false);
            Console.Write("   -> ");
            string choice = Console.ReadLine();
            if (choice == "0") { Main(null); }
            if (choice == "1")
            {
                var NetworkAdapters = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Class\\{4d36e972-e325-11ce-bfc1-08002be10318}");
                foreach (string adapter in NetworkAdapters.GetSubKeyNames())
                {
                    if (adapter != "Properties")
                    {
                        var NetworkAdapter = Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Control\\Class\\{{4d36e972-e325-11ce-bfc1-08002be10318}}\\{adapter}", true);
                        NetworkAdapter.GetValue("NetworkAddress");
                        string adapterId = NetworkAdapter.GetValue("NetCfgInstanceId").ToString();
                        Utils.EnableLocalAreaConection(adapterId, true);
                    }
                }
            }
            if (choice == "2") { Spoof.FlushDNS(); }
            NetworkSpoof();
        }
    }

    public class Spoof
    {
        public static void SpoofEAC()
        {
            try
            {
                Utils.DownloadAndStartDriver(Program.KDMapperURL, Program.drvURL, Program.MapExePath, Program.MapdrvPath);
                UI.WriteLine("New disk serial : " + GetHardDiskSerialNo());
            }
            catch (Exception ex)
            {
                UI.WriteLine("Error: " + ex.Message);
            }
        }
        public static void SpoofBE()
        {
            try
            {
                Utils.DownloadAndStartDriver(Program.KDMapperURL, Program.drvURL, Program.MapExePath, Program.MapdrvPath);
                UI.WriteLine("New disk serial : [N/A]");
            }
            catch (Exception ex)
            {
                UI.WriteLine("Error: " + ex.Message);
            }
        }
        public static string GetHardDiskSerialNo()
        {
            ManagementClass mangnmt = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection mcol = mangnmt.GetInstances();
            string result = "";
            foreach (ManagementObject strt in mcol) { result += Convert.ToString(strt["SerialNumber"]); }
            return result;
        }
        public static bool SpoofMAC(bool random, string mac)
        {
            bool err = false;

            var NetworkAdapters = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Class\\{4d36e972-e325-11ce-bfc1-08002be10318}");
            foreach (string adapter in NetworkAdapters.GetSubKeyNames())
            {
                if (adapter != "Properties")
                {
                    try
                    {
                        var NetworkAdapter = Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Control\\Class\\{{4d36e972-e325-11ce-bfc1-08002be10318}}\\{adapter}", true);
                        if (NetworkAdapter.GetValue("BusType") != null)
                        {
                            if (random) { NetworkAdapter.SetValue("NetworkAddress", Utils.RandomMac()); }
                            else { NetworkAdapter.SetValue("NetworkAddress", mac); }
                            string adapterId = NetworkAdapter.GetValue("NetCfgInstanceId").ToString();
                            Utils.EnableLocalAreaConection(adapterId, true);
                            Utils.EnableLocalAreaConection(adapterId, true);
                        }
                    }
                    catch (System.Security.SecurityException ex) { err = true; break; }
                }
            }

            return err;
        }
        public static void SpoofDisks()
        {
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\Scsi"))
            {
                foreach (string text in registryKey.GetSubKeyNames())
                {
                    using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\Scsi\\" + text))
                    {
                        foreach (string text2 in registryKey2.GetSubKeyNames())
                        {
                            using (RegistryKey registryKey3 = Registry.LocalMachine.OpenSubKey(string.Concat(new string[] { "HARDWARE\\DEVICEMAP\\Scsi\\", text, "\\", text2, "\\Target Id 0\\Logical Unit Id 0" }), true))
                            {
                                if (registryKey3 != null)
                                {
                                    if (registryKey3.GetValue("DeviceType").ToString() == "DiskPeripheral")
                                    {
                                        string text3 = Utils.RandomId(14);
                                        string text4 = Utils.RandomId(14);
                                        registryKey3.SetValue("DeviceIdentifierPage", Encoding.UTF8.GetBytes(text4));
                                        registryKey3.SetValue("Identifier", text3);
                                        registryKey3.SetValue("InquiryData", Encoding.UTF8.GetBytes(text3));
                                        registryKey3.SetValue("SerialNumber", text4);
                                    }
                                }
                            }
                        }
                    }
                }
                using (RegistryKey registryKey4 = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\MultifunctionAdapter\\0\\DiskController\\0\\DiskPeripheral"))
                {
                    foreach (string text5 in registryKey4.GetSubKeyNames())
                    {
                        using (RegistryKey registryKey5 = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\MultifunctionAdapter\\0\\DiskController\\0\\DiskPeripheral\\" + text5, true))
                        {
                            registryKey5.SetValue("Identifier", Utils.RandomId(8) + "-" + Utils.RandomId(8) + "-A");
                        }
                    }
                }
            }
        }
        public static void SpoofGUID()
        {
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\IDConfigDB\\Hardware Profiles\\0001", true))
            {
                registryKey.SetValue("HwProfileGuid", string.Format("{{{0}}}", Guid.NewGuid()));
                using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography", true))
                {
                    registryKey2.SetValue("MachineGuid", Guid.NewGuid().ToString());
                    using (RegistryKey registryKey3 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\SQMClient", true))
                    {
                        registryKey3.SetValue("MachineId", string.Format("{{{0}}}", Guid.NewGuid()));
                        using (RegistryKey registryKey4 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", true))
                        {
                            Random random = new Random();
                            int num = random.Next(1, 31);
                            string text;
                            if (num < 10)
                            {
                                text = string.Format("0{0}", num);
                            }
                            else
                            {
                                text = num.ToString();
                            }
                            int num2 = random.Next(1, 13);
                            string text2;
                            if (num2 < 10)
                            {
                                text2 = string.Format("0{0}", num2);
                            }
                            else
                            {
                                text2 = num2.ToString();
                            }
                            registryKey4.SetValue("BIOSReleaseDate", string.Format("{0}/{1}/{2}", text, text2, random.Next(2000, 2023)));
                            registryKey4.SetValue("BIOSVersion", Utils.RandomId(10));
                            registryKey4.SetValue("ComputerHardwareId", string.Format("{{{0}}}", Guid.NewGuid()));
                            registryKey4.SetValue("ComputerHardwareIds", string.Format("{{{0}}}\n{{{1}}}\n{{{2}}}\n", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));
                            registryKey4.SetValue("SystemManufacturer", Utils.RandomId(15));
                            registryKey4.SetValue("SystemProductName", Utils.RandomId(6));
                            using (RegistryKey registryKey5 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate", true))
                            {
                                registryKey5.SetValue("SusClientId", Guid.NewGuid().ToString());
                                registryKey5.SetValue("SusClientIdValidation", Encoding.UTF8.GetBytes(Utils.RandomId(25)));
                            }
                        }
                    }
                }
            }
        }
        public static void FlushDNS()
        {
            Utils.RunAsProcess("ipconfig /release");
            Utils.RunAsProcess("ipconfig /flushdns");
            Utils.RunAsProcess("ipconfig /renew");
            Utils.RunAsProcess("ipconfig /flushdns");
            Utils.RunAsProcess("ping localhost -n 3 >nul");
        }
    }

    public class UI
    {
        public static ConsoleColor color = ConsoleColor.DarkBlue;
        public static void WriteLine(string line)
        {
            Console.Write("  ");
            Thread.Sleep(20);
            Console.BackgroundColor = color;
            Console.ForegroundColor = color;
            Console.Write(" ");
            Console.ResetColor();
            Thread.Sleep(20);
            Console.Write("  ");
            for (int i = 0; i < line.Length; i++)
            {
                Console.Write(line[i]);
                Thread.Sleep(20);
            }
            Thread.Sleep(20);
            Console.WriteLine();
        }
        public static void WriteLineAlt(string line)
        {
            Console.Write("  ");
            Console.BackgroundColor = color;
            Console.ForegroundColor = color;
            Console.Write(" ");
            Console.ResetColor();
            Console.Write("  ");
            Console.Write(line);
            Thread.Sleep(20);
            Console.WriteLine();
        }
        public static void WriteBarrierLine(string num, string line)
        {
            Console.Write("  ");
            Thread.Sleep(20);
            Console.BackgroundColor = color;
            Console.ForegroundColor = color;
            Console.Write(" ");
            Console.ResetColor();
            Console.Write("  [");
            Thread.Sleep(20);
            Console.ForegroundColor = color;
            Console.Write(num);
            Console.ResetColor();
            Thread.Sleep(20);
            Console.Write("] ");
            for (int i = 0; i < line.Length; i++)
            {
                Console.Write(line[i]);
                Thread.Sleep(20);
            }
            Thread.Sleep(20);
            Console.WriteLine();
        }
        public static void WriteSpacing(bool writeline)
        {
            if (!writeline)
            {
                Console.Write("  ");
                Thread.Sleep(20);
                Console.BackgroundColor = color;
                Console.ForegroundColor = color;
                Console.Write(" ");
                Console.ResetColor();
            }
            if (writeline)
            {
                Console.Write("  ");
                Thread.Sleep(20);
                Console.BackgroundColor = color;
                Console.ForegroundColor = color;
                Console.WriteLine(" ");
                Console.ResetColor();
            }
        }
    }

    public class Utils
    {
        public static void DownloadAndStartDriver(string kdmapperUrl, string drvUrl, string mapExePath, string mapDrvPath)
        {
            using (WebClient webClient = new WebClient())
            {
                UI.WriteLine("Downloading Driver...");
                webClient.DownloadFile(kdmapperUrl, mapExePath);
                webClient.DownloadFile(drvUrl, mapDrvPath);
                Thread.Sleep(500);
                UI.WriteLine("Downloaded Driver.");

                UI.WriteLine("Attempting to start driver...");
                Start(mapExePath, mapDrvPath);
                UI.WriteLine("Driver started.");

                UI.WriteLine("Restarting Host...");
                foreach (var process in Process.GetProcessesByName("WmiPrvSE")) { process.Kill(); }
                UI.WriteLine("WMI Host Restarted.");

                File.Delete(mapExePath);
                File.Delete(mapDrvPath);
                UI.WriteLine("Driver Deleted.");
            }
        }
        public static void Start(string path, string optional = "")
        {
            Process proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.FileName = path;
            proc.StartInfo.Arguments = optional;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";
            proc.Start();
            proc.WaitForExit();
        }
        public static void RunAsProcess(string Code)
        {
            Process process = Process.Start(new ProcessStartInfo("cmd.exe", "/c " + Code) { CreateNoWindow = true, UseShellExecute = false });
            process.WaitForExit();
            process.Close();
        }
        public static bool IsAdministrator()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
        public static void RunAsAdministrator()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
            startInfo.Verb = "runas";

            try { Process.Start(startInfo); }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }

            Environment.Exit(0);
        }
        public static string RandomMac()
        {
            string chars = "ABCDEF0123456789";
            string windows = "26AE";
            string result = "";
            Random random = new Random();

            result += chars[random.Next(chars.Length)];
            result += windows[random.Next(windows.Length)];

            for (int i = 0; i < 5; i++)
            {
                result += "-";
                result += chars[random.Next(chars.Length)];
                result += chars[random.Next(chars.Length)];

            }

            return result;
        }
        public static string RandomId(int length)
        {
            string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string text2 = "";
            Random random = new Random();
            for (int i = 0; i < length; i++) { text2 += text[random.Next(text.Length)].ToString(); }
            return text2;
        }
        public static void EnableLocalAreaConection(string adapterId, bool enable = true)
        {
            string interfaceName = "Ethernet";
            foreach (NetworkInterface i in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (i.Id == adapterId)
                {
                    interfaceName = i.Name;
                    break;
                }
            }

            string control;
            if (enable) { control = "enable"; }
            else { control = "disable"; }

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("netsh", $"interface set interface \"{interfaceName}\" {control}");
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();
        }
    }
}
