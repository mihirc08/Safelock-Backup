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
using System.IO;
using Microsoft.Win32;
using System.Security;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Windows.Forms;
using Ionic.Zip;
using Ionic.Zlib;
using System.Net;



namespace Backup_Update
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String username;
        String userDirectory;
        String FileName;
        String FilePath;
        String SavePath;
        String dcPath="";
        String folderPath = "D:\\backup\\";
        DirectoryInfo d;
        String ftpPath = "ftp://127.0.0.1/";
        String[] filesname;
        String userDrectory;
        public MainWindow(String label)
        {
            InitializeComponent();
            username = label;
            userDirectory = "C:\\backup\\" + username;
            welcomeLabel.Content = welcomeLabel.Content +" "+ label;
            fileBrowse.IsEnabled = false;
            fullBrowse.IsEnabled = false;
            fileText.Text = "";
            fullText.Text = "";
            ftpPath = ftpPath + label + "/";
            userDrectory = "ftp://127.0.0.1/" + username;
            
        }

        public MainWindow()
        {
            InitializeComponent();

        }


 //titlebar controls
        private void titlebar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();    
        }

        private void close_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
            this.Close();
        }

        private void min_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
 //========================================================================================
      
  
                                  //    Backup tab     //


    // Button for browsing files to backup
        private void fileBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog folderBrowserDialog1 = new Microsoft.Win32.OpenFileDialog();
            folderBrowserDialog1.Multiselect = true;
            
            if (folderBrowserDialog1.ShowDialog() == true)
            {
                filesname = folderBrowserDialog1.FileNames;

                foreach (String ss in filesname)
                {
                    fileText.Text = ss;
                    FilePath = ss.Replace(":", "_drive");
                    FileName = System.IO.Path.GetFileName(ss);

                String[] folder = FilePath.Split(System.IO.Path.DirectorySeparatorChar);
                for (int i = 0; i < folder.Length-1; i++)
                {
                    folderPath = folderPath + folder[i];
                    Directory.CreateDirectory(folderPath);
                    folderPath = folderPath + "\\";
                }
                SavePath = "D:\\backup\\" + FilePath;
                EncryptFile(ss, SavePath);
                folderPath = "D:\\backup\\";
                }
            }
            
            if (fileText.Text != "")
            {
                fileText.IsEnabled = false;
                fullBrowse.IsEnabled = false;


            }   
        }
   //======================================================================================


   //File encryption method for backing up
        private void EncryptFile(string inputFile, string outputFile)
        {

            try
            {
                string password = @"myKey123"; //Key Here
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();

            }
            catch(Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, "Error");
            }
        }
   //======================================================================================



   //button for browsing folder to back up
        private void fullBrowse_Click(object sender, RoutedEventArgs e)
        {
            fullText.Text = "";
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.ShowDialog();
            if (fbd.SelectedPath != "")
            {
                fullText.Text = fbd.SelectedPath;
                d = new DirectoryInfo(fbd.SelectedPath);
                folderPath = "D:\\backup\\";
                FolderEncrypt(d);
            }
        }
  //======================================================================================

  //Folder ecryotion method for encrypting folder
        void FolderEncrypt(DirectoryInfo dir)
        {
            var dirName = dir.Name;
            String fpath = dir.FullName.Replace(":", "_drive");
            String[] folder = fpath.Split(System.IO.Path.DirectorySeparatorChar);

            FileInfo[] Files = dir.GetFiles();
            DirectoryInfo[] directory = dir.GetDirectories();

            for (int i = 0; i < folder.Length; i++)
            {
                folderPath = folderPath + folder[i];
                Directory.CreateDirectory(folderPath);
                folderPath = folderPath + "\\";
            }


            if (Files.Length > 0)
            {
                for (int ff = 0; ff < Files.Length; ff++)
                {
                    String frd = Files[ff].FullName;
                    String FinalPath = folderPath + Files[ff].Name;
                    EncryptFile(frd, FinalPath);

                }
            }

            if (directory.Length > 0)
            {
                for (int j = 0; j < directory.Length; j++)
                {
                    folderPath = "D:\\backup\\";
                    FolderEncrypt(directory[j]);
                }

            }

        }
  //======================================================================================

  //Compress method to compress backedup folder
        void FCompress(DirectoryInfo di)
        {
            
            ZipFile zipfolder = new ZipFile();
            zipfolder.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
            zipfolder.AddDirectory(di.FullName);
            zipfolder.Save(di.FullName + ".zip");
            FileInfo zi = new FileInfo(di.FullName + ".zip");
            Upload(zi);
            System.Windows.MessageBox.Show("Folder Backed up successfully "); 
        }
  //======================================================================================

  //method for uploading files to ftp
        public void Upload(FileInfo toUpload)
        {
            var fileName = toUpload.Name;
            String fpath = toUpload.FullName.Remove(0, 10);
            String[] folder = fpath.Split(System.IO.Path.DirectorySeparatorChar);

            for (int i = 0; i < folder.Length - 1; i++)
            {
                ftpPath = ftpPath + folder[i];

                try
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    WebResponse response = request.GetResponse();

                    ftpPath = ftpPath + "\\";
                }
                catch (System.Net.WebException)
                {
                    ftpPath = ftpPath + "\\";
                }


            }
            try
            {

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath + toUpload.Name);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                Stream ftpStream = request.GetRequestStream();
                FileStream file = File.OpenRead(toUpload.FullName);
                int length = 1024;
                byte[] buffer = new byte[length];
                int bytesRead = 0;
                do
                {
                    bytesRead = file.Read(buffer, 0, length);
                    ftpStream.Write(buffer, 0, bytesRead);
                }
                while (bytesRead != 0);
                file.Close();
                ftpStream.Close();
            }
            catch
            {
                System.Windows.MessageBox.Show("error!!!!");

            }

        }
  //======================================================================================


  //backup button for backing up file
        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            if (fileText.Text != "" || fullText.Text != "")
            {
                if (rbFile.IsChecked == true)
                {
                    foreach (String ss in filesname)
                    {
                        FileInfo tp1 = new FileInfo(ss);
                        FileInfo fi = new FileInfo("D:\\backup\\" + tp1.FullName.Replace(":", "_drive"));
                        ftpPath = "ftp://127.0.0.1/" + username + "/";
                        Upload(fi);
                    }
                    System.Windows.MessageBox.Show("File Backed up successfully ");
                }
                else if (rbFull.IsChecked == true)
                {
                    DirectoryInfo tp = new DirectoryInfo(fullText.Text);
                    DirectoryInfo di = new DirectoryInfo("D:\\backup\\" + tp.FullName.Replace(":", "_drive"));
                    ftpPath = "ftp://127.0.0.1/" + username + "/";
                    FCompress(di);

                }
            }
            else
            {
                System.Windows.MessageBox.Show("Please select File or Folder to Backup");
            }
        }
  //======================================================================================
        


  //radio_button code
        private void rbFile_Checked(object sender, RoutedEventArgs e)
        {
            fullText.IsEnabled = false;
            fullBrowse.IsEnabled = false;
            fullText.Text = "";

            fileText.IsEnabled = true;
            fileBrowse.IsEnabled = true;
        }
        private void rbFull_Checked(object sender, RoutedEventArgs e)
        {
            fileText.IsEnabled = false;
            fileBrowse.IsEnabled = false;
            fileText.Text = "";

            fullText.IsEnabled = true;
            fullBrowse.IsEnabled = true;
        }
  //======================================================================================
       

                               // Restore tab  //
        
        
  //browse file from ftp
        private void restoreBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("ftp://127.0.0.1/"+username);
        }
  //======================================================================================

  //Find downloaded file
        private void restore_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.ShowDialog();
            restoreText.Text = ofd.FileName;
        }
  //======================================================================================

  //method for decrypting file
        private void DecryptFile(string inputFile, string outputFile)
        {

            {
                string password = @"myKey123"; // Key Here

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, key),
                    CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();
                

            }
        }
  //======================================================================================

  //method for decompressing file
        public void Decompress(FileInfo fi)
        {
            using (ZipFile zip = ZipFile.Read(fi.FullName))
            {
                zip.ExtractAll(System.IO.Path.GetDirectoryName(fi.FullName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(fi.FullName));
            }
            
            DirectoryInfo fc = new DirectoryInfo(System.IO.Path.GetDirectoryName(fi.FullName)+"\\"+System.IO.Path.GetFileNameWithoutExtension(fi.FullName));
            FolderDecrypt(fc);

     
            
        }
  //======================================================================================

        void FolderDecrypt(DirectoryInfo dir)
        {
            var dirName = dir.Name;

            dcPath = dcPath + dir.Name;
            Directory.CreateDirectory(dcPath);
            dcPath = dcPath + "\\";
            FileInfo[] Files = dir.GetFiles();
            DirectoryInfo[] directory = dir.GetDirectories();


            if (Files.Length > 0)
            {
                for (int ff = 0; ff < Files.Length; ff++)
                {
                    String frd = Files[ff].FullName;
                    String FinalPath = dcPath + Files[ff].Name;
                    DecryptFile(frd, FinalPath);
                }
            }

            if (directory.Length > 0)
            {
                for (int j = 0; j < directory.Length; j++)
                {

                    FolderDecrypt(directory[j]);
                }

            }

        }

  //restoring downloaded file
        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {

            if (restoreText.Text.Contains(".zip"))
            {
                FolderBrowserDialog sfbd = new FolderBrowserDialog();
                sfbd.ShowDialog();
                if (sfbd.SelectedPath != "")
                {
                    restoreSaveText.Text = sfbd.SelectedPath;
                    dcPath = restoreSaveText.Text + "\\";
                    Decompress(new FileInfo(restoreText.Text));   //calling folderdercypt method inside Decompress
                    System.Windows.Forms.MessageBox.Show("File got Restored Successfully!!");
                    File.Delete(restoreText.Text);
                    DeleteDirectory(System.IO.Path.GetDirectoryName(restoreText.Text) + "\\" + System.IO.Path.GetFileNameWithoutExtension(restoreText.Text));
                    System.Windows.Forms.MessageBox.Show("Your temporary downloaded file got deleted");
                }
            }
            else
            {
                FolderBrowserDialog sfbd = new FolderBrowserDialog();
                sfbd.ShowDialog();
                if (sfbd.SelectedPath != "")
                {
                    restoreSaveText.Text = sfbd.SelectedPath + "\\" + System.IO.Path.GetFileName(restoreText.Text);

                    DecryptFile(restoreText.Text, restoreSaveText.Text);
                    System.Windows.Forms.MessageBox.Show("File got Restored Successfully!!");

                    File.Delete(restoreText.Text);
                    System.Windows.Forms.MessageBox.Show("Your temporary downloaded file got deleted");
                }
            }
        }
  //======================================================================================


                              //    My Backup tab     //


 //Listing user directories
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ListDirectory(treeView1,userDirectory);
        }

        public void ListDirectory(System.Windows.Controls.TreeView treeview, String path)
        {
            treeView1.Items.Clear();
            
            var RootDirectoryInfo = new DirectoryInfo(path);
            treeview.Items.Add(CreateDirectoryNode(RootDirectoryInfo));
            
        }

        private static TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeViewItem();
            directoryNode.Header = directoryInfo.Name;
            directoryNode.Tag = directoryInfo.FullName;
            foreach (var directory in directoryInfo.GetDirectories())
            {
                directoryNode.Tag = directoryInfo.FullName;
                directoryNode.Items.Add(CreateDirectoryNode(directory));
            }
            foreach (var file in directoryInfo.GetFiles())
            {
                TreeViewItem newnode = new TreeViewItem();
                newnode.Header = file.Name;
                directoryNode.Items.Add(newnode);
                newnode.Tag = file.FullName;
                
            }
            
            return directoryNode;
        }
 //==========================================================================================


 //for deleting files and folder
        private void delete_Click_2(object sender, RoutedEventArgs e)
        {
            if (treeView1.SelectedItem != null)
            {
                TreeViewItem asd = new TreeViewItem();
                asd = (TreeViewItem)treeView1.SelectedItem;

                if (File.Exists((String)asd.Tag))
                {
                    File.Delete((String)asd.Tag);
                }
                else
                {
                    Directory.Delete((String)asd.Tag, true);

                }
                System.Windows.Forms.MessageBox.Show("Your Selected Item got Deleted");
                ListDirectory(treeView1, userDirectory);
            }
            else
                System.Windows.Forms.MessageBox.Show("Select File to delete");

        }
        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
 //==========================================================================================

       
 //logout button
        private void logout_Click_3(object sender, RoutedEventArgs e)
        {
            Window1 m = new Window1();
            this.Hide();
            m.Show();
        }
 //==========================================================================================

    }
}
