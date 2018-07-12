using System.Windows;
using System.Windows.Controls;
using Unity;

namespace FakeIMC.UI
{
    public class ViewFactory : ContentControl
    {
        private static IUnityContainer container;

        public ViewFactory()
        {
            if (ViewFactory.container == null)
            {
                if (DesignHelper.IsInDesignMode)
                {
                    ViewFactory.container = new UnityContainer();
                    ViewFactory.container.AddNewExtension<Unity.Interception.ContainerIntegration.Interception>();
                    ViewFactory.container.AddNewExtension<CompositionModule>();
                }
                else
                {
                    ViewFactory.container = App.Container;
                }
            }
        }
        public static readonly DependencyProperty ViewNameProperty = DependencyProperty.Register(
            "ViewName", typeof(string), typeof(ViewFactory), new PropertyMetadata(default(FrameworkElement), OnNameChanged));

        private static void OnNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewFactory)d).ResolveView(e.NewValue as string);
        }

        public string ViewName
        {
            get { return (string) GetValue(ViewFactory.ViewNameProperty); }
            set { SetValue(ViewFactory.ViewNameProperty, value); }
        }

        private void ResolveView(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Content = ViewFactory.container.Resolve<UserControl>(name);
            }
        }
    }
}