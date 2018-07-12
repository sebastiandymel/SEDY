using System.IO;
using System.IO.Compression;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace REMedy.Updater
{
    public class Updater
    {
        private readonly Action closeApplicationImplementation;

        public Updater(Action closeApplicationImplementation)
        {
            this.closeApplicationImplementation = closeApplicationImplementation;
        }

        public async Task<UpdateResult> Update(FileInfo file, Version newVersion, IUpdateConfirmation confirmation)
        {
            if (file == null)
            {
                return UpdateResult.WrongFile;
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
                    return UpdateResult.UserStopped;
                }

                Directory.CreateDirectory(newDirPath);
                var newFile = Path.Combine(newDirPath, file.Name);
                try
                {
                    File.Copy(file.FullName, newFile, overwrite: true);
                }
                catch(Exception ex)
                {
                    throw new UpdaterException("Update failed. It was not possible to copy installer to local drive.", ex);
                }
                string installerFilePath = string.Empty;

                if (file.Extension == ".zip")
                {
                    try
                    {
                        ZipFile.ExtractToDirectory(newFile, newDirPath);
                        File.Delete(newFile);
                    }
                    catch (Exception ex)
                    {
                        throw new UpdaterException("Update failed. It was not possible to extract zip file", ex);
                    }
                }
                if (file.Extension == ".exe")
                {
                    installerFilePath = newFile;
                }

                var startInstallation = await confirmation.ShouldPerformUpdate(newVersion);
                if (!startInstallation)
                {
                    return UpdateResult.UserStopped;
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
                await confirmation.NotifyUpdateFailed(ex.Message, ex.InnerException);
                throw new UpdaterException("Update process failed.", ex);
            }

            return UpdateResult.Success;
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
            catch(Exception ex)
            {
                throw new UpdaterException("Update failed. It was not possible to remove temporary directory", ex);
            }
        }
    }
}
