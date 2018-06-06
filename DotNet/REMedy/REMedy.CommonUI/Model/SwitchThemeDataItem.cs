using System;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FakeVerifit.CommonUI
{
    public class SwitchThemeDataItem : ViewModelBase
    {
        private readonly ResourceDictionary skinResource;
        private readonly IAppConfigManager appConfigManager;
        private string name;
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

        public SwitchThemeDataItem(string skinName, IAppConfigManager appConfigManager)
        {
            Name = skinName;
            this.appConfigManager = appConfigManager;
            var uri = new Uri($"pack://application:,,,/FakeVerifit.CommonUI;component/Skins/{skinName}Skin.xaml", UriKind.RelativeOrAbsolute);
            this.skinResource = new ResourceDictionary() { Source = uri };

            var primaryColor = (Color) this.skinResource["Color.Primary"];
            var secondaryColor = (Color) this.skinResource["Color.Secondary"];

            Primary = new SolidColorBrush(primaryColor);
            Secondary = new SolidColorBrush(secondaryColor);

            SwitchThemeCommand = new RelayCommand(SwitchTheme);
        }

       

        public RelayCommand SwitchThemeCommand { get; }

        public void SwitchTheme()
        {
            foreach (var key in this.skinResource.Keys)
            {
                Application.Current.MainWindow.Resources[key] = this.skinResource[key];
            }

            this.appConfigManager.SaveStartUpTheme(this.name);
        }
    }
}