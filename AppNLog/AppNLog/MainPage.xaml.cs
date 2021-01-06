using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AppNLog
{
    public partial class MainPage : ContentPage
    {
        Logger logger;
        string logFolder;

        public MainPage()
        {
            InitializeComponent();

            IFileHelper fileHelper = Xamarin.Forms.DependencyService.Get<IFileHelper>();
            if (fileHelper != null)
            {
                logFolder = fileHelper.GetDocumentPath();
                Debug.WriteLine($"Log folder : {logFolder}");
            }

            logger = LogManager.GetCurrentClassLogger();
            logger.Info("Logging successful");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void ExportLog(object sender, EventArgs e)
        {
            logger.Info("Logging successful");
            logger.Error("Logging successful");
            logger.Warn("Logging successful");
            logger.Trace("Logging successful");
            logger.Debug("Logging successful");

            string[] filePaths = Directory.GetFiles(logFolder+"/logs", "*",SearchOption.TopDirectoryOnly);
            foreach(var path in filePaths)
            {
                Debug.WriteLine("Arquivo: " + path);
            }

            await CompressAndExportFolder(logFolder + "/logs");
        }

        async Task CompressAndExportFolder(string folderPath)
        {
            // Get a temporary cache directory
            var exportZipTempDirectory = Path.Combine(FileSystem.CacheDirectory, "Export");

            // Delete folder incase anything from previous exports, it will be recreated later anyway
            try
            {
                if (Directory.Exists(exportZipTempDirectory))
                {
                    Directory.Delete(exportZipTempDirectory, true);
                }
            }
            catch (Exception ex)
            {
                // Log things and move on, don't want to fail just because of a left over lock or something
                Debug.WriteLine(ex);
            }

            // Get a timestamped filename
            var exportZipFilename = $"MyLogs_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.zip";
            Directory.CreateDirectory(exportZipTempDirectory);

            var exportZipFilePath = Path.Combine(exportZipTempDirectory, exportZipFilename);
            if (File.Exists(exportZipFilePath))
            {
                File.Delete(exportZipFilePath);
            }

            // Zip everything up
            ZipFile.CreateFromDirectory(folderPath, exportZipFilePath, CompressionLevel.Fastest, true);

            // Give the user the option to share this using whatever medium they like
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "My Logs",
                File = new ShareFile(exportZipFilePath),
            });
        }
    }
}
