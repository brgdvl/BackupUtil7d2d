using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace BackupUtil7d2d
{
    public static class SFTP
    {
        private const string configpath = @"configs\config.json";//hardcoded - buuuuuhhhh
        private static ConfigDetails configDetails = null;

        private class ConfigDetails
        {
            internal string Host { set; get; }
            internal string Port { set; get; }
            internal string Username { set; get; }
            internal string Password { set; get; }
            internal string Path { set; get; }
            internal string SaveDir { set; get; }
            internal string LocalBackupDirectory { set; get; }
        }
    
        private static void ConfFile()
        {

            try
            {
                //File.OpenRead(configpath).Dispose();
                using (StreamReader r = new StreamReader(File.OpenRead(configpath)))
                {
                    var json = r.ReadToEnd();
                    var jobj = JObject.Parse(json).SelectToken("sftp");

                    configDetails = new ConfigDetails
                    {
                        Host = jobj.SelectToken("host").ToString(),
                        Port = jobj.SelectToken("port").ToString(),
                        Username = jobj.SelectToken("username").ToString(),
                        Password = jobj.SelectToken("password").ToString(),
                        Path = jobj.SelectToken("remotedir").ToString(),
                        SaveDir = jobj.SelectToken("savedir").ToString(),
                        LocalBackupDirectory = jobj.SelectToken("localbackupdir").ToString().Replace(@"\\", @"\")
                    };
                }
            }
            catch (FileNotFoundException fex)
            {
                Console.WriteLine(fex.ToString());
            }

            
        }

        public static void EstablishConnection()
        {
            ConfFile();//collect connection details

            //connect to sftp server
            using (SftpClient sftp = new SftpClient(configDetails.Host, 
                int.Parse(configDetails.Port), 
                configDetails.Username, 
                configDetails.Password))
            {
                var saveDir = configDetails.SaveDir; 
                try
                {
                    sftp.Connect();

                    var r = sftp.ListDirectory("/i237/Saved");

                    BackupContent(sftp, string.Empty, string.Empty);//empty string as this is method entrance
                }
                catch (Exception x)
                {
                    //some message (in log) that connection couldn't be established
                    throw;
                }

            }
        }

        private static void BackupContent(SftpClient sftp, string source, string destination)
        {
            //if this method is non-recursive
            if(source == string.Empty)
            {
                source = string.Concat(configDetails.Path, configDetails.SaveDir);
            }

            //fetch files and directories
            var remote = sftp.ListDirectory(source);

            //Console.WriteLine(source.Length);
            
            foreach(SftpFile x in remote)
            {
                if(!x.IsDirectory && !x.IsSymbolicLink)
                {
                    DownloadAFile(sftp, x);
                }
                else if(x.Name!="." && x.Name != "..")
                {
                    var dir = Directory.CreateDirectory(Path.Combine(
                        Path.Combine(configDetails.LocalBackupDirectory,
                        configDetails.SaveDir.Replace("/", @"\"),
                        x.FullName)));
                    BackupContent(sftp, x.FullName, dir.FullName);
                }
            }

            //nested method as the parent method is the only one that must use it
            void DownloadAFile(SftpClient nestSftp, SftpFile remoteFile)
            {
                string location = string.Concat(configDetails.LocalBackupDirectory,
                    remoteFile.FullName.Replace("/", @"\")
                    .Remove(remoteFile.FullName.LastIndexOf("/") + 1));

                Directory.CreateDirectory(location);
                using (Stream fs = File.OpenWrite(location + remoteFile.Name))
                {
                    nestSftp.DownloadFile(remoteFile.FullName, fs);
                }
            }
        }
    }
}