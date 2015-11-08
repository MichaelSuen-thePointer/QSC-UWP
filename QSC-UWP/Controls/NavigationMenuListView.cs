using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace QSC_UWP.Controls
{
    public class NavigationMenuListView : ListView
    {
        private SplitView _parentSplitView;

        public NavigationMenuListView()
        {
            this.SelectionMode = ListViewSelectionMode.Single;
            this.IsItemClickEnabled = true;
            this.ItemClick += ItemClickedHandler;

            this.Loaded += (s, a) =>
            {
                var parent = VisualTreeHelper.GetParent(this);
                while (parent != null && !(parent is SplitView))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }
                if (parent != null)
                {
                    this._parentSplitView = parent as SplitView;

                    _parentSplitView.RegisterPropertyChangedCallback(SplitView.IsPaneOpenProperty, (sender, args) =>
                    {
                        this.OnPaneToggled();
                    });
                }

            };
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            for (int i = 0; i < this.ItemContainerTransitions.Count; i++)
            {
                if (this.ItemContainerTransitions[i] is EntranceThemeTransition)
                {
                    this.ItemContainerTransitions.RemoveAt(i);
                }
            }
        }

        public void SetSelectedItem(ListViewItem item)
        {
            int index = -1;
            if (item != null)
            {
                index = this.IndexFromContainer(item);
            }
            Debug.Assert(this.Items != null);
            // ReSharper disable once PossibleNullReferenceException
            for (int i = 0; i < this.Items.Count; i++)
            {
                var listViewItem = (ListViewItem)this.ContainerFromIndex(i);
                Debug.Assert(listViewItem != null);
                if (i != index)
                {
                    listViewItem.IsSelected = false;
                }
                else if (i == index)
                {
                    listViewItem.IsSelected = true;
                }
            }
        }

        public event EventHandler<ListViewItem> ItemInvoked;

        private void ItemClickedHandler(object sender, ItemClickEventArgs e)
        {
            var item = this.ContainerFromItem(e.ClickedItem);
            this.InvokeItem(item);
        }

        private void InvokeItem(object focusedItem)
        {
            this.SetSelectedItem(focusedItem as ListViewItem);
            Debug.Assert(this.ItemInvoked != null);
            this.ItemInvoked(this, focusedItem as ListViewItem);
            if (this._parentSplitView.IsPaneOpen &&
                (this._parentSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay ||
                 this._parentSplitView.DisplayMode == SplitViewDisplayMode.Overlay))
            {
                this._parentSplitView.IsPaneOpen = false;

                (focusedItem as ListViewItem)?.Focus(FocusState.Programmatic);
            }
        }

        private void OnPaneToggled()
        {
            if (this._parentSplitView.IsPaneOpen)
            {
                Debug.Assert(this.ItemsPanelRoot != null);
                this.ItemsPanelRoot.ClearValue(FrameworkElement.WidthProperty);
                this.ItemsPanelRoot.ClearValue(FrameworkElement.HorizontalAlignmentProperty);
            }
            else if (this._parentSplitView.DisplayMode == SplitViewDisplayMode.CompactInline
                     || this._parentSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                Debug.Assert(this.ItemsPanelRoot != null);
                this.ItemsPanelRoot.SetValue(FrameworkElement.WidthProperty, this._parentSplitView.CompactPaneLength);
                this.ItemsPanelRoot.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            }
        }
    }
}
