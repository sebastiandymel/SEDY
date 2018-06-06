using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;
using Autofac;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
using log4net.Appender;
using Remedy.CommonUI;

namespace FakeVerifit.UI
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IThemeManager themeManager;

        public UICollection<LogEntry> Logs { get; } = new UICollection<LogEntry>();

        public SettingsViewModel Settings { get; } = App.Container.Resolve<SettingsViewModel>();
        public DeviceSetupViewModel DeviceSetup { get; } = App.Container.Resolve<DeviceSetupViewModel>();
        public AdditionalControlsViewModel AdditionalControls { get; } = App.Container.Resolve<AdditionalControlsViewModel>();

        public RelayCommand SaveResponseCommand { get; }
        public RelayCommand LoadResponseCommand { get; }
        public RelayCommand SetupThemesCommand { get; }

        public RelayCommand ClearLogCommand { get; }
        public RelayCommand ExportLogCommand { get; }

        public RelayCommand ToogleScrollLockCommand { get; }
        public RelayCommand CopyIPToClipboardCommand { get; }

        public RelayCommand ShowAboutCommand { get; }

        public RelayCommand ToggleLeftPanelCommand { get; }
        public RelayCommand ToggleRightPanelCommand { get; }

        private bool isScrollLockOn;
        public bool IsScrollLockOn
        {
            get => this.isScrollLockOn;
            set => Set(ref this.isScrollLockOn, value);
        }

        private bool wrapLines;
        private IExternalThemeManager externalThemeManager;


        public bool WrapLines
        {
            get => this.wrapLines;
            set => Set(ref this.wrapLines, value);
        }

        public ObservableCollection<ThemeDataItem> Themes { get; } = new ObservableCollection<ThemeDataItem>();

        public IAppConfigManager AppConfigManager { get; } = App.Container.Resolve<IAppConfigManager>();

        private bool isRightPanelExpanded;

        public bool IsRightPanelExpanded
        {
            get => this.isRightPanelExpanded;
            set
            {
                if (IsLeftPanelExpanded)
                {
                    IsLeftPanelExpanded = false;
                }

                if (this.isRightPanelExpanded != value)
                {
                    Set(ref this.isRightPanelExpanded, value);
                }
            }
        }

        private bool isLeftPanelExpanded;
        private Timer timer;

        public bool IsLeftPanelExpanded
        {
            get => this.isLeftPanelExpanded;
            set
            {
                if (IsRightPanelExpanded)
                {
                    IsRightPanelExpanded = false;
                }

                if (this.isLeftPanelExpanded != value)
                {
                    Set(ref this.isLeftPanelExpanded, value);
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IThemeManager themeManager, IExternalThemeManager externalThemeManager)
        {
            this.themeManager = themeManager;
            this.externalThemeManager = externalThemeManager;
            this.externalThemeManager.ThemesChanged += OnThemesChanged;
            this.timer = new Timer(UpdateLog, Application.Current.Dispatcher, 0, 100);

            SaveResponseCommand = new RelayCommand(SaveResponses);
            LoadResponseCommand = new RelayCommand(LoadResponses);
            SetupThemesCommand = new RelayCommand(SetupThemes);
            ClearLogCommand = new RelayCommand(ClearLog);
            ExportLogCommand = new RelayCommand(ExportLog);
            ToogleScrollLockCommand = new RelayCommand(ToogleScrollLock);
            CopyIPToClipboardCommand = new RelayCommand(CopyIPToClipboard);
            ShowAboutCommand = new RelayCommand(ShowAboutDialog);
            ToggleLeftPanelCommand = new RelayCommand(ToggleLeftPanel);
            ToggleRightPanelCommand = new RelayCommand(ToggleRightPanel);
        }

        private void ExportLog()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = ".txt",
                FileName = $"FakeVerifit_Log_{ DateTime.Now.ToString("yyyy_MM_dd__HH-mm", CultureInfo.InvariantCulture)}.txt",
                Filter = "FakeVerifit text log (*.txt)|*.txt"
            };
            bool? result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                var filename = dlg.FileName;
                using (StreamWriter sw = new StreamWriter(filename))
                {
                    foreach (var logEntry in Logs)
                    {
                        sw.WriteLine(logEntry.Message);
                    }
                }
            }
        }

        private void ToggleRightPanel()
        {
            var value = IsRightPanelExpanded;
            IsRightPanelExpanded = !value;
        }

        private void ToggleLeftPanel()
        {
            var value = IsLeftPanelExpanded;
            IsLeftPanelExpanded = !value;
        }

        private void OnThemesChanged(object sender, EventArgs e)
        {
            UpdateThemeList();
        }

        private void ShowAboutDialog()
        {
            AboutWindow aboutWindow = new AboutWindow
            {
                Owner = Window.GetWindow(Application.Current.MainWindow)
            };
            var keys = Application.Current.MainWindow.Resources.Keys;
            foreach (var key in keys)
            {
                aboutWindow.Resources[key] = Application.Current.MainWindow.Resources[key];
            }
            aboutWindow.ShowDialog();
        }

        private void CopyIPToClipboard()
        {
            Clipboard.SetText(App.IP);
        }

        private void ToogleScrollLock()
        {
            IsScrollLockOn = !IsScrollLockOn;
        }

        private void ClearLog()
        {
            Logs.Clear();
            RaisePropertyChanged(nameof(Logs));
        }

        private void SetupThemes()
        {
            UpdateThemeList();

            var skinName = AppConfigManager.GetStartUpTheme();
            if (skinName != null)
            {
                var theme = Themes.FirstOrDefault(t => t.Name == skinName);
                theme?.SwitchTheme();
            }

        }

        private void UpdateThemeList()
        {

            Themes.Clear();
            foreach (var themeName in this.themeManager.GetThemes())
            {
                Themes.Add(new InternalThemeDataItem(themeName, AppConfigManager));
            }

            foreach (var theme in this.externalThemeManager.ThemeData)
            {
                Themes.Add(new ExternalThemeDataItem(theme.Name, theme.ResourceDictionary, AppConfigManager));
            }
        }

        private readonly object updateLogLock = new object();
        private void UpdateLog(object state)
        {
            lock (this.updateLogLock)
            {
                var dispatcher = state as Dispatcher;
                var repository = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();

                var appenders = repository.GetAppenders();
                var memoryAppender = (MemoryAppender)appenders[0];
                foreach (var ev in memoryAppender.GetEvents())
                {
                    dispatcher.Invoke(() =>
                    {
                        Logs.Add(new LogEntry
                        {
                            Message = ev.RenderedMessage
                        });
                        
                    });
                }
                memoryAppender.Clear();
            }
        }

        private const string xmlRootElementName = "collections";
        private void LoadResponses()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = "XML Files (*.xml)|*.xml"
            };
            bool? result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                var filename = dlg.FileName;

                XDocument doc = XDocument.Load(filename);
                var serializableCollections = GetAllSerializableCollections().ToArray();

                foreach (var element in doc.Element(xmlRootElementName).Elements())
                {
                    var idAttribute = element.Attribute("id");

                    var collection = serializableCollections.FirstOrDefault(c => c.Id == idAttribute.Value);
                    if (collection != null)
                    {
                        collection.Deserialize(element);
                    }
                }
            }

            Settings.InitializeGrids();
        }

        private void SaveResponses()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = ".xml",
                Filter = "XML Files (*.xml)|*.xml"
            };
            bool? result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                var filename = dlg.FileName;

                XDocument doc = new XDocument();
                var root = new XElement(xmlRootElementName);
                foreach (var collection in GetAllSerializableCollections())
                {
                    root.Add(collection.Serialize());
                }
                doc.Add(root);
                doc.Save(filename);
            }
        }

        public IEnumerable<SerializableFreqObservableCollection> GetAllSerializableCollections()
        {
            var properties = Settings.GetType().GetProperties().Where(x => x.PropertyType == typeof(SerializableFreqObservableCollection));

            foreach (var propertyInfo in properties)
            {
                yield return (SerializableFreqObservableCollection)propertyInfo.GetValue(Settings);
            }
        }

        public byte[] WindowsScreenShot
        {
            get
            {
                byte[] image = null;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var window = Application.Current.MainWindow;
                    image = GetJpgImage(window, 1, 75);
                });
                return image;
            }
        }

        public string LogoPath
        {
            get
            {
                string path = "";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var oBitmapImage = Application.Current.MainWindow.Resources["Image.Logo"];
                    var bitImage = (BitmapImage)oBitmapImage;
                    path = "Skins/" + bitImage.UriSource;
                });

                return path;
            }
        }

        ///
        /// Gets a JPG "screenshot" of the current UIElement
        ///
        /// UIElement to screenshot
        /// Scale to render the screenshot
        /// JPG Quality
        /// Byte array of JPG data
        public static byte[] GetJpgImage(UIElement source, double scale, int quality)
        {
            double actualHeight = source.RenderSize.Height;
            double actualWidth = source.RenderSize.Width;

            double renderHeight = actualHeight * scale;
            double renderWidth = actualWidth * scale;

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
            VisualBrush sourceBrush = new VisualBrush(source);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(scale, scale));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }
            renderTarget.Render(drawingVisual);

            JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder
            {
                QualityLevel = quality
            };
            jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            Byte[] _imageArray;

            using (MemoryStream outputStream = new MemoryStream())
            {
                jpgEncoder.Save(outputStream);
                _imageArray = outputStream.ToArray();
            }

            return _imageArray;
        }

        public override void Cleanup()
        {
            this.timer.Dispose();
            base.Cleanup();
        }
    }
}