using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PKHL.ProjectSweeper.Controls
{
    /// <summary>
    /// Simple filter control for filtering list views
    /// </summary>
    public partial class FilterControl : UserControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(FilterControl), new PropertyMetadata("Filter:"));

        public static readonly DependencyProperty TargetControlProperty =
            DependencyProperty.Register("TargetControl", typeof(ItemsControl), typeof(FilterControl), new PropertyMetadata(null));

        public static readonly DependencyProperty FilterTextBindingPathProperty =
            DependencyProperty.Register("FilterTextBindingPath", typeof(string), typeof(FilterControl), new PropertyMetadata(""));

        public FilterControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Header text displayed before the filter textbox
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// The ItemsControl (ListView, ListBox, etc.) to filter
        /// </summary>
        public ItemsControl TargetControl
        {
            get { return (ItemsControl)GetValue(TargetControlProperty); }
            set { SetValue(TargetControlProperty, value); }
        }

        /// <summary>
        /// Property path to use for filtering (e.g., "StyleName")
        /// </summary>
        public string FilterTextBindingPath
        {
            get { return (string)GetValue(FilterTextBindingPathProperty); }
            set { SetValue(FilterTextBindingPathProperty, value); }
        }

        /// <summary>
        /// Gets the current filter text
        /// </summary>
        public string FilterText
        {
            get { return FilterTextBox.Text; }
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TargetControl != null && TargetControl.ItemsSource != null)
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(TargetControl.ItemsSource);
                if (view != null)
                {
                    view.Refresh();
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            FilterTextBox.Text = string.Empty;
        }
    }
}
