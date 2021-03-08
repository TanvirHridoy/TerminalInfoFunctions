using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace ConsoleApp6
{
   public  class TerminalFunction
    {
        private  TerminalDetails terminalDetails = new TerminalDetails();

        public bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
        public List<ProcessorInfo> GetProcessorInfos() {
            //processor info
            ProcessorInfo processor = new ProcessorInfo();
            List<ProcessorInfo> processors = new List<ProcessorInfo>();
            SelectQuery Sq = new SelectQuery("Win32_Processor");
            ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(Sq);
            ManagementObjectCollection osDetailsCollection = objOSDetails.Get();
            foreach (ManagementObject mo in osDetailsCollection)
            {
                processor.PrcssrName = mo["Name"].ToString();
                processor.PrcssrId = mo["ProcessorId"].ToString();
                processors.Add(processor);
            }
            return processors;
        }
        public List<HardDriveInfo> GetHardDriveInfos()
        {
            List<HardDriveInfo> hardDriveInfos = new List<HardDriveInfo>();

            ManagementObjectSearcher moSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (ManagementObject wmi_HD in moSearcher.Get())
            {
                HardDriveInfo hardDriveInfo = new HardDriveInfo();
                hardDriveInfo.Model = wmi_HD["Model"].ToString();  //Model Number
                hardDriveInfo.Type = wmi_HD["InterfaceType"].ToString();  //Interface Type
                hardDriveInfo.SerialNo = wmi_HD["SerialNumber"].ToString(); //Serial Number
                hardDriveInfos.Add(hardDriveInfo);
            }
            return hardDriveInfos;
        } 
        public string GetWindowsVersion()
        {
            string winvar = "";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            foreach (ManagementObject managementObject in mos.Get())
            {
               
                if (managementObject["Caption"] != null)
                {
                    winvar = managementObject["Caption"].ToString() + " build" + managementObject["Version"].ToString();   //Display operating system caption
                }
            }
            return winvar;
        }
        public string GetmotherboardId()
        {
            ManagementObjectSearcher moss = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectCollection moc = moss.Get();
            string motherBoard = "";
            foreach (ManagementObject mo in moc)
            {
                motherBoard = (string)mo["SerialNumber"];
            }
            return motherBoard;
        }
        public List<volumeDetails> GetVolumeDetails()
        {
            List<volumeDetails> dlist = new List<volumeDetails>();

            ManagementClass mangnmt = new ManagementClass("Win32_LogicalDisk");
            ManagementObjectCollection mcol = mangnmt.GetInstances();
            int sl = 1;
            foreach (ManagementObject strt in mcol)
            {
                dlist.Add(new volumeDetails()
                {
                    Slno = sl.ToString(),
                    Name = Convert.ToString(strt["Name"]),
                    VolumeName = Convert.ToString(strt["VolumeName"]),
                    FileSystem = Convert.ToString(strt["FileSystem"]),
                    Size = Convert.ToString(strt["Size"]),
                    Totalsize = Convert.ToString(strt["Size"]),
                    Description = Convert.ToString(strt["Description"]),
                    VolumeSerialNumber = Convert.ToString(strt["VolumeSerialNumber"])
                });
                sl++;
            }
            return dlist;
        }
        public List<NetInterfaceDetails> GetNetworkInfo()
        {
            List<NetInterfaceDetails> listNetInterface = new List<NetInterfaceDetails>();
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            List<NetworkInterface> nics = NetworkInterface.GetAllNetworkInterfaces().ToList();
            int sl2 = 1;
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                PhysicalAddress address = adapter.GetPhysicalAddress();
                string tempMac = "";
                byte[] bytes = address.GetAddressBytes();
                for (int i = 0; i < bytes.Length; i++)
                {
                    tempMac = tempMac + bytes[i].ToString("X2");
                    // Insert a hyphen after each byte, unless we are at the end of the
                    // address.
                    if (i != bytes.Length - 1)
                    {
                        tempMac = tempMac + "-";
                    }
                }
                //terminalDetails.WifiMac = tempMac;
                var ip = properties.UnicastAddresses.Count() > 1 ? properties.UnicastAddresses[1].Address.ToString() : "";
                listNetInterface.Add(new NetInterfaceDetails { Slno = sl2.ToString(), Name = adapter.Name, NetworkInterfaceType = adapter.NetworkInterfaceType.ToString(), OperationalStatus = adapter.OperationalStatus.ToString(), PhysicalMac = tempMac, Ipaddress = ip.ToString() });
                sl2++;
            }
            return listNetInterface;
        }
        public  TerminalDetails GetFullTerminalInfo()
        {
            terminalDetails.PcName = Environment.MachineName.ToString();
            terminalDetails.PcUserName = Environment.UserName.ToString();
            string[] dr = Environment.GetLogicalDrives();

            terminalDetails.lstProcessors = GetProcessorInfos();
            //hard drive info Win32_OperatingSystem
            terminalDetails.hardDriveInfos = GetHardDriveInfos();
            //windows info
            terminalDetails.PcWinVar = this.GetWindowsVersion();
            terminalDetails.MotherboardId = this.GetmotherboardId();
            terminalDetails.driveDetails = this.GetVolumeDetails();
            terminalDetails.listNetInterface = this.GetNetworkInfo();
            terminalDetails.IsInternetConnected = CheckForInternetConnection();
            return terminalDetails;
        }
    }
    public class volumeDetails
    {
        public string Slno { get; set; }
        public string Name { get; set; }
        public string VolumeName { get; set; }
        public string FileSystem { get; set; }
        public string Size { get; set; }
        public string Totalsize { get; set; }
        public string Description { get; set; }
        public string VolumeSerialNumber { get; set; }

    }
    public class NetInterfaceDetails
    {
        public string Slno { get; set; }
        public string Name { get; set; }
        public string NetworkInterfaceType { get; set; }
        public string OperationalStatus { get; set; }
        public string PhysicalMac { get; set; }
        public string Ipaddress { get; set; }

    }
    public class deviceItem
    {
        public string ItemId { get; set; }
        public string Title { get; set; }
        public string Device_Group { get; set; }
        public string Sub_SL { get; set; }
        public string Value { get; set; }

    }
    public class HardDriveInfo
    {
        public string Model = null;
        public string Type = null;
        public string SerialNo = null;
    }
    public class ProcessorInfo
    {
        public string PrcssrName { get; set; }
        public string PrcssrId { get; set; }
    }
    public class TerminalDetails
    {

        public List<HardDriveInfo> hardDriveInfos = new List<HardDriveInfo>();
        public string PcName { get; set; }
        public string PcUserName { get; set; }
        public string PcWinVar { get; set; }
        public string PcWinVarsion { get; set; }

        //
        public string MotherboardId { get; set; }
        //Processor

       public List<ProcessorInfo> lstProcessors = new List<ProcessorInfo>();
        public bool IsInternetConnected { get; set; }
        //Installed partition Details
        public string WorkDir { get; set; }
        public List<volumeDetails> driveDetails { get; set; }
        public List<NetInterfaceDetails> listNetInterface { get; set; }
    }
}
