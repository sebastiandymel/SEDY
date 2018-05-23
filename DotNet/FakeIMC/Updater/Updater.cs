using System.IO;
using System.IO.Compression;
using System;
using System.Diagnostics;

namespace Updater
{
    public class Updater
    {
        private readonly Action closeApplicationImplementation;

        public Updater(Action closeApplicationImplementation)
        {
            this.closeApplicationImplementation = closeApplicationImplementation;
        }

        public void Update(FileInfo file)
        {
            if (file == null)
            {
                return;
            }
            try
            {
                var currentDir = Directory.GetCurrentDirectory();
                var dirName = "__Update";
                var newDirPath = Path.Combine(currentDir, dirName);

                //
                // REMOVE ALL FILES FROM UPDATE DIR......
                //
                RemoveAllFilesFrom(newDirPath);

                //
                // COPY ZIP FILE AND UNZIP TO TEMP FOLDER
                // 
                Directory.CreateDirectory(newDirPath);
                var newFile = Path.Combine(newDirPath, file.Name);
                File.Copy(file.FullName, newFile);
                if (file.Extension == ".zip")
                {
                    ZipFile.ExtractToDirectory(newFile, newDirPath);
                    File.Delete(newFile);
                }
                if (file.Extension == ".exe")
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(newFile);
                    startInfo.Verb = "runas";
                    Process.Start(startInfo);                    
                }

                //
                // CLOSE APP AND UPDATE
                //
                this.closeApplicationImplementation?.Invoke();
                
            }
            catch (Exception ex)
            {
                UpdateFailed(this, EventArgs.Empty);
            }
        }

        private static void RemoveAllFilesFrom(string newDirPath)
        {
            try
            {
                if (Directory.Exists(newDirPath))
                {
                    var di = new DirectoryInfo(newDirPath);
                    foreach (FileInfo f in di.EnumerateFiles())
                    {
                        f.Delete();
                    }
                    foreach (DirectoryInfo dir in di.EnumerateDirectories())
                    {
                        dir.Delete(true);
                    }
                    Directory.Delete(newDirPath);
                }
            }
            catch { }
        }

        public event EventHandler UpdateFailed = delegate { };
    }
}
