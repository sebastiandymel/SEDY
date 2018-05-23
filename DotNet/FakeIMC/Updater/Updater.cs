using System.IO;
using System.IO.Compression;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Updater
{
    public class Updater
    {
        private readonly Action closeApplicationImplementation;

        public Updater(Action closeApplicationImplementation)
        {
            this.closeApplicationImplementation = closeApplicationImplementation;
        }

        public async Task Update(FileInfo file, Version newVersion, IUpdateConfirmation confirmation)
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
                var shouldDownloadFile = await confirmation.ShouldDownloadUpdate(newVersion);
                if (!shouldDownloadFile)
                {
                    return;
                }

                Directory.CreateDirectory(newDirPath);
                var newFile = Path.Combine(newDirPath, file.Name);
                File.Copy(file.FullName, newFile, overwrite: true);
                string installerFilePath = string.Empty;

                if (file.Extension == ".zip")
                {
                    ZipFile.ExtractToDirectory(newFile, newDirPath);
                    File.Delete(newFile);
                }
                if (file.Extension == ".exe")
                {
                    installerFilePath = newFile;
                }

                var startInstallation = await confirmation.ShouldPerformUpdate(newVersion);
                if (!startInstallation)
                {
                    return;
                }

                //
                // CLOSE APP AND UPDATE
                //
                ProcessStartInfo startInfo = new ProcessStartInfo(installerFilePath);
                startInfo.Verb = "runas";
                Process.Start(startInfo);
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
