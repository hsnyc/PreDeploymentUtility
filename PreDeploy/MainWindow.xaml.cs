using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Printing;
using System.Management;


namespace BOTPre
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();

            // Automatically resize height relative to content
            this.SizeToContent = SizeToContent.Height;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
                
            
            try
            {
            
                //Create a list to hold the data
                List<string> uData = new List<string>();
                
                //Get user name
                string userN = Environment.UserName;
                uData.Add("User Name: " + userN);
                //System.Windows.Forms.MessageBox.Show("User Name: " + userN);

                //Get Domain name
                string domainN = Environment.UserDomainName;
                uData.Add("Domain: " + domainN);
                //System.Windows.Forms.MessageBox.Show("Domain: " + domainN);
            
                // Get the local computer host name.
                String hostName = Dns.GetHostName();
                uData.Add("Computer Name: " + hostName);
                //System.Windows.Forms.MessageBox.Show("Computer Name: " + hostName);

                //Get Full Hosts names
                IPHostEntry host;            
                host = Dns.GetHostEntry(Dns.GetHostName());
                string hostN = host.HostName.ToString();  //Full Host name
                uData.Add("Full Computer Name: " + hostN);
                
                //Console.WriteLine("GetHostAddresses({0}) returns:", hostname);

                //Gets IP Address
                string ipAddr = DoGetHostAddresses();
                uData.Add("IP Address: " + ipAddr);

                //Get screen resolution             
                var resolution = Screen.PrimaryScreen.Bounds.Size;
                uData.Add("Resolution: " + resolution);
                //System.Windows.Forms.MessageBox.Show("Resolution: " + resolution.ToString());

                //Get Network Drives 
                System.IO.DriveInfo[] drives = DriveInfo.GetDrives();

                foreach (var dr in drives)
                {
                    //System.Windows.Forms.MessageBox.Show("Drive: " + dr.Name);

                    string driveType = dr.DriveType.ToString();
                    string logicalD = dr.ToString();

                    if (driveType == "Network")
                    {
                        string UNCPath = Pathing.GetUNCPath(logicalD);
                        uData.Add("Drive \"" + logicalD + "\" is: " + UNCPath);
                        //System.Windows.Forms.MessageBox.Show("UNC Path for Drive " + logicalD + " is: " + UNCPath);
                    }

                    //Reference: dr.DriveType
                }

                //Get printers  
                int count = 0;

                foreach (PrintQueue prt in GetPrinters())
                {                
                    string printer = prt.Name;

                    //Get PortName and Driver
                    GetPrtProps(printer);

                    uData.Add("Printer " + count + ": " + printer + " | PortName: " + src.prtPortName + " | DriverName: " + src.prtDriveName);
                    //System.Windows.Forms.MessageBox.Show("Printer " + count + ": " + printer);

                    
                    //uData.Add("PortName: " + src.prtPortName);
                    //uData.Add("DriverName: " + src.prtDriveName);

                    //System.Type prtType = prt.GetType();
                    //System.Windows.Forms.MessageBox.Show("Printer Type" + prtType);
                    
                    count++;
                }
                
                //Write it to local txt file on desktop
                string path = "udata.txt";
                if (!File.Exists(path))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        //Write time to text file
                        uData_rTextBox.AppendText(DateTime.Today.ToString());

                        foreach(string uitem in uData)
                        {
                            sw.WriteLine(uitem);
                        }
                    }

                    //Write data to Text box
                    uData_rTextBox.AppendText(DateTime.Today.ToString());

                    foreach(string item in uData)
                    {
                        uData_rTextBox.AppendText(Environment.NewLine + item);
                    }
                }

                // Open the file to read from. 
                using (StreamReader sr = File.OpenText(path))
                {

                    var start = uData_rTextBox.Document.ContentStart;
                    var end = uData_rTextBox.Document.ContentEnd;
                    int difference = start.GetOffsetToPosition(end);

                    //System.Windows.Forms.MessageBox.Show("Dif equals: " + difference);

                    if (difference == 0)
                    {
                        //System.Windows.Forms.MessageBox.Show("Textbox is empty");

                        string s = "";

                        uData_rTextBox.AppendText(Environment.NewLine + DateTime.Today.ToString());
                        while ((s = sr.ReadLine()) != null)
                        {
                            //System.Windows.Forms.MessageBox.Show(s);
                            uData_rTextBox.AppendText(Environment.NewLine + s);
                            //REF: Environment.NewLine // + DateTime.Today
                        }

                        
                    }
 
                }


                //WriteToText(uName, dName, hName, fHName, ipA );
            }

            catch (SocketException sock)
            {
                System.Windows.Forms.MessageBox.Show("SocketException caught!!!");
                System.Windows.Forms.MessageBox.Show("Source : " + sock.Source);
                System.Windows.Forms.MessageBox.Show("Message : " + sock.Message);

            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("SocketException caught!!!");
                System.Windows.Forms.MessageBox.Show("Source : " + ex.Source);
                System.Windows.Forms.MessageBox.Show("Message : " + ex.Message);
            }

            
        }



        //Get IP address
        public static string DoGetHostAddresses()
        {

            //Get IPs
            IPHostEntry host;            
            host = Dns.GetHostEntry(Dns.GetHostName());

            //System.Windows.Forms.MessageBox.Show("Full Computer Name: " + host.HostName.ToString());

            string localIP = "?";

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    //System.Windows.Forms.MessageBox.Show("Local IP: " + localIP);
                }
            }

            return localIP;
        }

        List<PrintQueue> GetPrinters()
        {
            PrintServer localPrintServer = new PrintServer();

            PrintQueueCollection printQueues = localPrintServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });

            var printerList = (from printer in printQueues select printer).ToList();

            return printerList;

            //from --> http://www.mindfiresolutions.com/How-to-get-list-of-Local-and-Network-Printers-1495.php
        }

        //My little source class to capture some properties.
        sources src = new sources();

        private void getUserData_bttn_Click(object sender, RoutedEventArgs e)
        {
            //make sure U:\ drive exists
            string dir = @"U:\";

            if (Directory.Exists(dir))
            {
                //System.Windows.Forms.MessageBox.Show("U:\\ Exists!");

                var profilePath = Environment.ExpandEnvironmentVariables("%UserProfile%");
                

                //Get Desktop files               
                string dektop = profilePath + "\\Desktop\\";

                if (Directory.Exists(dektop))
                {
                    src.sourceP = "desktop";
                    src.sourceFolder = dektop;
                    ProcessDirectory(dektop, src.sourceP, src.sourceFolder);
                }


                //Get Favorites
                string favorites = profilePath + "\\Favorites\\";

                if (Directory.Exists(favorites))
                {
                    src.sourceP = "favorites";
                    src.sourceFolder = favorites;
                    ProcessDirectory(favorites, src.sourceP, src.sourceFolder);
                }

                //Get My Documents
                /*string myDocs = profilePath + "\\My Documents\\";

                if (Directory.Exists(myDocs))
                {
                    src.sourceP = "docs";
                    ProcessDirectory(myDocs, src.sourceP);
                }*/

                

                //DONE: To add to array--> "%UserProfile%\\Desktop", "%UserProfile%\\My Documents", "%UserProfile%\\Favorites", "%UserProfile%\\AppData\Local\\Microsoft\\Outlook\\Test.pst"

                //TODO: Get the PST file

                //Get PST in the default Outlook folder
                string pst = profilePath + "\\AppData\\Local\\Microsoft\\Outlook\\test.pst";

                //DONE Processing
                System.Windows.Forms.MessageBox.Show("All done!");

            }

            else
            {
                System.Windows.Forms.MessageBox.Show("U:\\Drive is not mapped. Make sure user can access their home drive.");
            }
        }       

        // Process all files in the directory passed in, recurse on any directories  
        // that are found, and process the files they contain. 
        public static void ProcessDirectory(string sourceDirectory, string s, string sourceF)
        {

            try
            {
                // Process the list of files found in the directory. 
                string[] fileEntries = Directory.GetFiles(sourceDirectory);

                string destPath = "";

                //Create destination folder
                switch (s)
                {
                    case "desktop":
                        destPath = @"U:\UserBackup\Desktop";
                        break;

                    case "favorites":
                        destPath = @"U:\UserBackup\Favorites";
                        break;
                }

                foreach (string fileName in fileEntries)
                {
                    copyFile(fileName, sourceF, destPath);
                }

                // Recurse into subdirectories of this directory. 
                string[] subdirectoryEntries = Directory.GetDirectories(sourceDirectory);

                foreach (string subdirectory in subdirectoryEntries)
                {
                    ProcessDirectory(subdirectory, s, sourceF);
                }
              
                //TODO: Maybe a progress bar               
            }

            catch (UnauthorizedAccessException unEx)
            {
                System.Windows.Forms.MessageBox.Show("Exception getting files: " + unEx);
            }
   
        }

        private static void copyFile(string fileName, string sourceDir, string destP)
        {
            try
            {
                //Remove path from the file name. 
                //string fileN = fileName.Substring(sourceDir.Length + 1);

                //System.Windows.Forms.MessageBox.Show("Filename: " + fileName);
                //System.Windows.Forms.MessageBox.Show("sourceDir: " + sourceDir);
                //System.Windows.Forms.MessageBox.Show("destP: " + destP);

                //Remove the source from the file name.
                string splitPath = fileName.Remove(0, sourceDir.Length);

                //System.Windows.Forms.MessageBox.Show("splitPath: " + splitPath);

                //Combine destination path with split path
                string fileP = System.IO.Path.Combine(destP, splitPath);

                //System.Windows.Forms.MessageBox.Show("fileP: " + fileP);

                //Get directory of the the new file
                string destDir = System.IO.Path.GetDirectoryName(fileP);

                //System.Windows.Forms.MessageBox.Show("destDir: " + destDir);

                //create the directory
                System.IO.Directory.CreateDirectory(destDir);

                //Create source path
                //string sourceFileName = System.IO.Path.Combine(dest, fileN);

                //Create dest path
                //string fileP = System.IO.Path.Combine(dest, fileN);

                //System.Windows.Forms.MessageBox.Show("Copy this file: " + fileP);

                //Copy the file
                File.Copy(fileName, fileP, true);
            }

            catch (Exception copy)
            {
                System.Windows.Forms.MessageBox.Show("I encountered a problem copying this file: " + copy);
            }
            
            //throw new NotImplementedException();
        }
        

        private void GetPrtProps(string prtName)
        {
            string printerName = prtName;
            string query = string.Format("SELECT * from Win32_Printer WHERE Name LIKE '%{0}'", printerName);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection coll = searcher.Get();

            foreach (ManagementObject printer in coll)
            {
                foreach (PropertyData property in printer.Properties)
                {
                    if (property.Name == "PortName")
                    {
                        src.prtPortName = property.Value.ToString();
                        //System.Windows.Forms.MessageBox.Show(property.Name + " : " + src.prtPortName);

                    }

                    if (property.Name == "DriverName")
                    {

                        src.prtDriveName = property.Value.ToString();
                        //System.Windows.Forms.MessageBox.Show(property.Name + " : " + src.prtDriveName);
                    }

                    //System.Windows.Forms.MessageBox.Show(property.Name + " : " + property.Value);
                    //Console.WriteLine(string.Format("{0}: {1}", property.Name, property.Value));
                }
            }
        }

        
    }

    
    public static class Pathing
    {
        //Dll import
        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]

        public static extern int WNetGetConnection(
            [MarshalAs(UnmanagedType.LPTStr)] string localName,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
            ref int length);
        /// <summary>
        /// Given a path, returns the UNC path or the original. (No exceptions
        /// are raised by this function directly). For example, "P:\2008-02-29"
        /// might return: "\\networkserver\Shares\Photos\2008-02-09"
        /// </summary>
        /// <param name="originalPath">The path to convert to a UNC Path</param>
        /// <returns>A UNC path. If a network drive letter is specified, the
        /// drive letter is converted to a UNC or network path. If the 
        /// originalPath cannot be converted, it is returned unchanged.</returns>
        public static string GetUNCPath(string originalPath)
        {
            StringBuilder sb = new StringBuilder(512);
            int size = sb.Capacity;

            // look for the {LETTER}: combination ...
            if (originalPath.Length > 2 && originalPath[1] == ':')
            {
                // don't use char.IsLetter here - as that can be misleading
                // the only valid drive letters are a-z && A-Z.
                char c = originalPath[0];
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    int error = WNetGetConnection(originalPath.Substring(0, 2),
                        sb, ref size);
                    if (error == 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(originalPath);

                        string path = System.IO.Path.GetFullPath(originalPath)
                            .Substring(System.IO.Path.GetPathRoot(originalPath).Length);
                        return System.IO.Path.Combine(sb.ToString().TrimEnd(), path);
                    }
                }
            }

            return originalPath;

            //From --> http://www.wiredprairie.us/blog/index.php/archives/22
        }
   
    }

}
