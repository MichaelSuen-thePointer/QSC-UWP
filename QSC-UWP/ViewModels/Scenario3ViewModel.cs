using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using QSC_UWP.Annotations;

namespace QSC_UWP.ViewModels
{
    public class Scenario3ViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> ItemNameList { get; } = new ObservableCollection<string>() {
            "Item 1", "Item 2", "Item 3"
        };

        public async void OnListItemClicked(object sender, ItemClickEventArgs e)
        {
            var messageDialog = new ContentDialog();
            messageDialog.IsPrimaryButtonEnabled = true;
            messageDialog.PrimaryButtonText = "是";
            messageDialog.SecondaryButtonText = "否";
            messageDialog.Content = $"{e.ClickedItem}";
            await messageDialog.ShowAsync();
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class ListBoxHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return $"{(value as ObservableCollection<string>)?.Count} item(s) exists";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }
}