using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShinyBleApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceListPage : ContentPage
    {
        public DeviceListPage()
        {
            InitializeComponent();
            BindingContext = ViewModels.DeviceListViewModel.Instance;

            ViewModels.DeviceListViewModel.Instance.LogList.CollectionChanged += LogList_CollectionChanged;
        }

        private void LogList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        try
                        {
                            //var v = _listView.ItemsSource.Cast<object>().LastOrDefault();
                            //_listView.ScrollTo(v, ScrollToPosition.End, true);
                            foreach(var item in e.NewItems)
                            {
                                _label.Text += "\n" + item.ToString() ;
                            }


                            _scrollView.ScrollToAsync(_label, ScrollToPosition.End, false);
                        }
                        catch (Exception) { }
                        break;
                    }
            }
        }
    }
}