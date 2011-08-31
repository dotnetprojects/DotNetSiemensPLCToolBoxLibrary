using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace DotNetSimaticDatabaseProtokollerConfigurationTool.WPF
{
    public static class HyperlinkUtility
    {
        public static readonly DependencyProperty LaunchDefaultBrowserProperty = DependencyProperty.RegisterAttached("LaunchDefaultBrowser", typeof(bool), typeof(HyperlinkUtility), new PropertyMetadata(false, HyperlinkUtility_LaunchDefaultBrowserChanged));

        public static bool GetLaunchDefaultBrowser(DependencyObject d)
        {
            return (bool)d.GetValue(LaunchDefaultBrowserProperty);
        }

        public static void SetLaunchDefaultBrowser(DependencyObject d, bool value)
        {
            d.SetValue(LaunchDefaultBrowserProperty, value);
        }

        private static void HyperlinkUtility_LaunchDefaultBrowserChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DependencyObject d = (DependencyObject)sender;
            if ((bool)e.NewValue)
                ((ContentElement)d).AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(Hyperlink_RequestNavigateEvent));
            else
                ((ContentElement)d).RemoveHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(Hyperlink_RequestNavigateEvent));
        }

        private static void Hyperlink_RequestNavigateEvent(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
