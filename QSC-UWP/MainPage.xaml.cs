using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using QSC_UWP.Controls;
using QSC_UWP.Views;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace QSC_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current = null;
        public Frame AppFrame => this.SplitFrame;
        public event TypedEventHandler<MainPage, Rect> TogglePaneButtonRectChanged;
        public Rect TogglePaneButtonRect { get; private set; }
        public MainPageViewModel ViewModel { get; } = new MainPageViewModel();

        private readonly List<NavigationMenuItem> _navigationList = new List<NavigationMenuItem>
        {
            new NavigationMenuItem()
            {
                Symbol = Symbol.Contact,
                Label="Basic Page",
                DestPageType = typeof(Scenario1)
            },
            new NavigationMenuItem()
            {
                Symbol= Symbol.Admin,
                Label="AppBar Page",
                DestPageType = typeof(Scenario2)
            },
            new NavigationMenuItem()
            {
                Symbol= Symbol.Admin,
                Label="Item Page",
                DestPageType = typeof(Scenario3)
            }
        };

        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += (sender, args) =>
            {
                Current = this;
                this.TogglePaneButton.Focus(FocusState.Programmatic);
            };

            this.RootSplitView.RegisterPropertyChangedCallback(
                SplitView.DisplayModeProperty,
                (sender, args) =>
                {
                    this.CheckTogglePaneButtonSizeChanged();
                });

            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            NavigationMenuList.ItemsSource = _navigationList;

        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (this.AppFrame == null)
                return;

            if (this.AppFrame.CanGoBack && !e.Handled)
            {
                e.Handled = true;
                this.AppFrame.GoBack();
            }
        }

        private void OnNavigatingToPage(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                var item = (from p in this._navigationList where p.DestPageType == e.SourcePageType select p).SingleOrDefault();
                if (item == null && this.AppFrame.BackStackDepth > 0)
                {
                    foreach (var entry in this.AppFrame.BackStack.Reverse())
                    {
                        item = (from p in this._navigationList where p.DestPageType == entry.SourcePageType select p).SingleOrDefault();
                        if (item != null)
                            break;
                    }
                }

                var container = (ListViewItem)NavigationMenuList.ContainerFromItem(item);

                if (container != null)
                {
                    container.IsTabStop = false;
                }
                NavigationMenuList.SetSelectedItem(container);
                if (container != null)
                {
                    container.IsTabStop = true;
                }
            }
        }

        private void OnNavigatedToPage(object sender, NavigationEventArgs e)
        {
            if (e.Content is Page && e.Content != null)
            {
                var control = (Page)e.Content;
                control.Loaded += Page_Loaded;
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppFrame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((Page)sender).Focus(FocusState.Programmatic);
            ((Page)sender).Loaded -= Page_Loaded;
            this.CheckTogglePaneButtonSizeChanged();
        }

        private void CheckTogglePaneButtonSizeChanged()
        {
            if (this.RootSplitView.DisplayMode == SplitViewDisplayMode.Inline ||
                this.RootSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                var transform = this.TogglePaneButton.TransformToVisual(this);
                var rect =
                    transform.TransformBounds(new Rect(0, 0, this.TogglePaneButton.ActualWidth,
                        this.TogglePaneButton.ActualHeight));
                this.TogglePaneButtonRect = rect;
            }
            else
            {
                this.TogglePaneButtonRect = new Rect();
            }

            this.TogglePaneButtonRectChanged?.DynamicInvoke(this, this.TogglePaneButtonRect);

        }

        private void NavigationMenuList_OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (!args.InRecycleQueue && args.Item != null && args.Item is NavigationMenuItem)
            {
                args.ItemContainer.SetValue(AutomationProperties.NameProperty, ((NavigationMenuItem)args.Item).Label);
            }
            else
            {
                args.ItemContainer.ClearValue(AutomationProperties.NameProperty);
            }
        }

        private void NavigationMenuList_OnItemInvoked(object sender, ListViewItem listViewItem)
        {
            var item = (NavigationMenuItem)((NavigationMenuListView)sender).ItemFromContainer(listViewItem);

            if (item?.DestPageType != null &&
                item.DestPageType != this.AppFrame.CurrentSourcePageType)
            {
                this.AppFrame.Navigate(item.DestPageType, item.Arguments);
            }
        }

        private void TogglePaneButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            this.CheckTogglePaneButtonSizeChanged();
        }
    }

    public class MainPageViewModel
    {
        public List<string> PageNameList { get; } = new List<string> { "Page1", "Page2", "Page3" };
        public string PageListHeader { get; } = "Header";
    }

    public class PageValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string content = value as string;
            return content ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }

}
