using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhoenixStyleBrowser
{
    public class StyleLibrary : NotifyPropertyChanged, IStyleLibrary
    {
        protected static string AssemblyNameFormat = "Phoenix.Application.Resources.StyleLibrary.{0}.dll";
        private string errorMessage;
        private bool isValid;
        private bool isSelected;
        private string name;
        private string directory;
        private string paths;
        private readonly IViewController viewController;
        private readonly IResourceDictionaryLoader resourceLoader;
        private readonly FileInfo[] dlls;

        public event EventHandler IsSelectedChanged = delegate { };

        protected string Dir { get; }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        public string Directory
        {
            get { return directory; }
            set
            {
                directory = value;
                OnPropertyChanged();
            }
        }

        public string Paths
        {
            get { return paths; }
            set
            {
                paths = value;
                OnPropertyChanged();
            }
        }

        public string Icon { get; }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsValid
        {
            get { return isValid; }
            set
            {
                isValid = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged();
                IsSelectedChanged(this, EventArgs.Empty);
            }
        }

        public StyleLibrary(IViewController viewController, IResourceDictionaryLoader resourceLoader, FileInfo[] dlls)
        {
            LoadLibrary = new AsyncCommand(LoadThisLibrary, () => IsValid);
            this.viewController = viewController;
            this.resourceLoader = resourceLoader;
            this.dlls = dlls;
        }

        public virtual Task Initialize()
        {
            return Task.Factory.StartNew(() =>
            {
                IsValid = this.dlls?.Length > 0;
                if (IsValid)
                {
                    Name = dlls[0].DirectoryName;
                    Directory = dlls[0].DirectoryName;
                    Paths = string.Join(Environment.NewLine, dlls.Select(x => x.Name.Replace(x.Extension, String.Empty)));
                    LoadLibrary.Refresh();
                }
            });
        }

        public AsyncCommand LoadLibrary { get; }

        private async Task LoadThisLibrary()
        {
            try
            {
                ErrorMessage = string.Empty;
                IsSelected = true;
                var dictionaries = await this.resourceLoader.LoadFrom(this.dlls.Select(x => x.FullName).ToArray());
                this.viewController.Show("ResourcesPresenterView", dictionaries);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                IsValid = false;
                IsSelected = false;
                LoadLibrary.Refresh();
            }
        }
    }
}
