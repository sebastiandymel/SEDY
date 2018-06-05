using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Remedy.CommonUI
{
    public abstract class ThemeDataItem : ViewModelBase
    {
        protected ResourceDictionary skinResource;
        protected IAppConfigManager appConfigManager;
        protected string name;
        public string Name
        {
            get => this.name;
            set => Set(ref this.name, value);
        }

        private Brush primary;
        public Brush Primary
        {
            get => this.primary;
            set => Set(ref this.primary, value);
        }

        private Brush secondary;
        public Brush Secondary
        {
            get => this.secondary;
            set => Set(ref this.secondary, value);
        }

        public ThemeDataItem()
        {
        }

        protected ThemeDataItem(string skinName, IAppConfigManager appConfigManager)
        {
            Name = skinName;
            this.appConfigManager = appConfigManager;
            Initialize();
        }

        protected void Initialize()
        {

            var primaryColor = (Color)this.skinResource["Color.Primary"];
            var secondaryColor = (Color)this.skinResource["Color.Secondary"];

            Primary = new SolidColorBrush(primaryColor);
            Secondary = new SolidColorBrush(secondaryColor);

            SwitchThemeCommand = new RelayCommand(SwitchTheme);
        }


        public RelayCommand SwitchThemeCommand { get; private set; }

        public void SwitchTheme()
        {
            var appResources = Application.Current.MainWindow.Resources;
            appResources.BeginInit();
            foreach (string key in this.skinResource.Keys)
            {
                var value = this.skinResource[key];
                if (value is BitmapImage)
                {
                    //forcing to load image
                    var bi = value as BitmapImage;
                    var pixelWidth = bi.PixelWidth;
                    bi.Freeze();
                }
                Application.Current.MainWindow.Resources[key] = value;
            }
            appResources.EndInit();
            
            this.appConfigManager.SaveStartUpTheme(this.name);
        }
    }
    public class InternalThemeDataItem : ThemeDataItem
    {
        public InternalThemeDataItem(string skinName, IAppConfigManager appConfigManager)
        {
            Name = skinName;
            this.appConfigManager = appConfigManager;
            var uri = new Uri($"pack://application:,,,/Remedy.CommonUI;component/Skins/{Name}Skin.xaml", UriKind.RelativeOrAbsolute);
            this.skinResource = new ResourceDictionary() { Source = uri };

            Initialize();
        }
    }

    public class ExternalThemeDataItem : ThemeDataItem
    {
        public ExternalThemeDataItem(string skinName, ResourceDictionary resourceDictionary, IAppConfigManager appConfigManager)
        {
            Name = skinName;
            this.appConfigManager = appConfigManager;
            this.skinResource = resourceDictionary;

            Initialize();
        }
    }
}