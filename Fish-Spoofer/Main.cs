using Fish_Tools.core.Utils;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace Fish_Spoofer
{
    public class Program
    {
        public const string KDMapperURL = "https://raw.githubusercontent.com/official-notfishvr/notfishvr.dev-cdn/main/kdmapper.exe";
        public const string drvURL = "https://raw.githubusercontent.com/official-notfishvr/notfishvr.dev-cdn/main/HelloWorld.sys";
        public const string MapExePath = @"C:\Windows\map.exe";
        public const string MapdrvPath = @"C:\Windows\mapdrv.sys";

        static void Main(string[] args) { Logger logger = new Logger(); if (!Utils.IsAdministrator()) { Utils.RunAsAdministrator(); return; } MainMenu(logger); }
        public static void MainMenu(Logger logger)
        {
            Console.Clear();
            Console.Title = "Fish Spoofer";
            logger.PrintArt();
            logger.WriteBarrierLine("1", "Temp Spoof");
            logger.WriteBarrierLine("2", "PC Spoof");
            logger.WriteBarrierLine("3", "Network");
            Console.Write("-> ");
            ConsoleKey choice = Console.ReadKey().Key;
            if (choice == ConsoleKey.D1) { TempSpoof(logger); }
            if (choice == ConsoleKey.D2) { PCSpoof(logger); }
            if (choice == ConsoleKey.D3) { NetworkSpoof(logger); }
            MainMenu(logger);
        }
        public static void TempSpoof(Logger logger)
        {
            Console.Clear();
            Console.Title = "Fish Spoofer";
            logger.PrintArt();
            logger.WriteBarrierLine("0", "Back");
            logger.WriteBarrierLine("1", "EasyAntiCheat");
            logger.WriteBarrierLine("2", "BattleEye");
            Console.Write("-> ");
            ConsoleKey choice = Console.ReadKey().Key;
            if (choice == ConsoleKey.D0) { MainMenu(logger); }
            if (choice == ConsoleKey.D1) { Spoof.SpoofEAC(logger); }
            else if (choice == ConsoleKey.D2) { Spoof.SpoofBE(logger); }
            TempSpoof(logger);
        }
        public static void PCSpoof(Logger logger)
        {
            Console.Clear();
            Console.Title = "Fish Spoofer";
            logger.PrintArt();
            logger.WriteBarrierLine("0", "Back");
            logger.WriteBarrierLine("1", "Spoof Disks");
            logger.WriteBarrierLine("2", "Spoof GUID");
            logger.WriteBarrierLine("3", "Chang MAC");
            Console.Write("-> ");
            ConsoleKey choice = Console.ReadKey().Key;
            if (choice == ConsoleKey.D0) { MainMenu(logger); }
            if (choice == ConsoleKey.D1) { Spoof.SpoofDisks(); }
            if (choice == ConsoleKey.D2) { Spoof.SpoofGUID(); }
            if (choice == ConsoleKey.D3) { MACSpoof(logger); }
            PCSpoof(logger);
        }
        public static void MACSpoof(Logger logger)
        {
            Console.Clear();
            Console.Title = "Fish Spoofer";
            logger.PrintArt();
            logger.WriteBarrierLine("0", "Back");
            logger.WriteBarrierLine("1", "Custom MAC");
            logger.WriteBarrierLine("2", "Random MAC");
            Console.Write("-> ");
            ConsoleKey choice = Console.ReadKey().Key;
            if (choice == ConsoleKey.D0) { MainMenu(logger); }
            if (choice == ConsoleKey.D1)
            {
                logger.Write("Put Your MAC You Want");
                Console.Write("-> ");
                string mac = Console.ReadLine();
                Spoof.SpoofMAC(false, mac);
            }
            else if (choice == ConsoleKey.D2)
            {
                Spoof.SpoofMAC(true, null);
            }
            MACSpoof(logger);
        }
        public static void NetworkSpoof(Logger logger)
        {
            Console.Clear();
            Console.Title = "Fish Spoofer";
            logger.PrintArt();
            logger.WriteBarrierLine("0", "Back");
            logger.WriteBarrierLine("1", "Fix Network");
            logger.WriteBarrierLine("2", "Flush DNS");
            Console.Write("-> ");
            ConsoleKey choice = Console.ReadKey().Key;
            if (choice == ConsoleKey.D0) { MainMenu(logger); }
            if (choice == ConsoleKey.D1)
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
            if (choice == ConsoleKey.D2) { Spoof.FlushDNS(); }
            NetworkSpoof(logger);
        }
    }

    public class Spoof
    {
        public static void SpoofEAC(Logger logger)
        {
            try
            {
                Utils.DownloadAndStartDriver(Program.KDMapperURL, Program.drvURL, Program.MapExePath, Program.MapdrvPath, logger);
                logger.Debug($"New disk serial : {GetHardDiskSerialNo()}");
            }
            catch (Exception ex)
            {
                logger.Error("Error: " + ex.Message);
            }
        }
        public static void SpoofBE(Logger logger)
        {
            try
            {
                Utils.DownloadAndStartDriver(Program.KDMapperURL, Program.drvURL, Program.MapExePath, Program.MapdrvPath, logger);
                logger.Debug($"New disk serial : {GetHardDiskSerialNo()}");
            }
            catch (Exception ex)
            {
                logger.Error("Error: " + ex.Message);
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
                                if (registryKey3 != null && registryKey3.GetValue("DeviceType").ToString() == "DiskPeripheral")
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

    public class Utils
    {
        public static void DownloadAndStartDriver(string kdmapperUrl, string drvUrl, string mapExePath, string mapDrvPath, Logger logger)
        {
            using (WebClient webClient = new WebClient())
            {
                logger.Debug("Downloading Driver...");
                webClient.DownloadFile(kdmapperUrl, mapExePath);
                webClient.DownloadFile(drvUrl, mapDrvPath);
                Thread.Sleep(500);
                logger.Debug("Downloaded Driver.");

                logger.Debug("Attempting to start driver...");
                Start(mapExePath, mapDrvPath);
                logger.Debug("Driver started.");

                logger.Debug("Restarting Host...");
                foreach (var process in Process.GetProcessesByName("WmiPrvSE")) { process.Kill(); }
                logger.Debug("WMI Host Restarted.");

                File.Delete(mapExePath);
                File.Delete(mapDrvPath);
                logger.Debug("Driver Deleted.");
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
