using Microsoft.Phone.Controls;
using SettingsPageAnimation.Framework;
using SettingsPageAnimation.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace SettingsPageAnimation
{
    public partial class MainPage : PhoneApplicationPage
    {
        private double _dragDistanceToOpen = 75.0;
        private double _dragDistanceToClose = 305.0;
        private double _dragDistanceNegative = -75.0;

        private FrameworkElement _feRoot;
        private FrameworkElement _feSettings;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the LongListSelector control to the sample data
            DataContext = App.ViewModel;

            _feRoot = this.LayoutRoot as FrameworkElement;
            _feSettings = this.SettingsPane as FrameworkElement;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        // Handle selection changed on LongListSelector
        private void MainLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (MainLongListSelector.SelectedItem == null)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + (MainLongListSelector.SelectedItem as ItemViewModel).ID, UriKind.Relative));

            // Reset selected item to null (no selection)
            MainLongListSelector.SelectedItem = null;
        }

        private bool _isSettingsOpen = false;
        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            if (_isSettingsOpen)
            {
                CloseSettings();
            }
            else
            {
                OpenSettings();
            }
        }

        private void CloseSettings()
        {
            //VisualStateManager.GoToState(this, "SettingsClosedState", true);

            var trans = _feRoot.GetHorizontalOffset().Transform;
            trans.Animate(trans.X, 0, TranslateTransform.XProperty, 300, 0, new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            });

            trans = _feSettings.GetHorizontalOffset().Transform;
            trans.Animate(trans.X, -480, TranslateTransform.XProperty, 300, 0, new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            });

            _isSettingsOpen = false;
        }

        private void OpenSettings()
        {
            //VisualStateManager.GoToState(this, "SettingsOpenState", true);

            var trans = _feRoot.GetHorizontalOffset().Transform;
            trans.Animate(trans.X, 380, TranslateTransform.XProperty, 300, 0, new CubicEase
                {
                    EasingMode = EasingMode.EaseOut
                });

            trans = _feSettings.GetHorizontalOffset().Transform;
            trans.Animate(trans.X, 480, TranslateTransform.XProperty, 300, 0, new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            });

            _isSettingsOpen = true;
        }

        private void GestureListener_OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal && e.HorizontalChange > 0 && !_isSettingsOpen)
            {
                double offset = _feRoot.GetHorizontalOffset().Value + e.HorizontalChange;
                if (offset > _dragDistanceToOpen)
                    this.OpenSettings();
                else
                {
                    _feRoot.SetHorizontalOffset(offset);
                    _feSettings.SetHorizontalOffset(offset);
                }
            }

            if (e.Direction == System.Windows.Controls.Orientation.Horizontal && e.HorizontalChange < 0 && _isSettingsOpen)
            {
                double offsetRoot = _feRoot.GetHorizontalOffset().Value + e.HorizontalChange;
                double offsetSettings = _feSettings.GetHorizontalOffset().Value + e.HorizontalChange;
                if (offsetRoot < _dragDistanceToClose)
                    this.CloseSettings();
                else
                {
                    _feRoot.SetHorizontalOffset(offsetRoot);
                    _feSettings.SetHorizontalOffset(offsetSettings);
                }
            }
        }

        private void GestureListener_OnDragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal && e.HorizontalChange > 0 && !_isSettingsOpen)
            {
                if (e.HorizontalChange < _dragDistanceToOpen)
                    this.ResetLayoutRoot();
                else
                    this.OpenSettings();
            }

            if (e.Direction == System.Windows.Controls.Orientation.Horizontal && e.HorizontalChange < 0 && _isSettingsOpen)
            {
                if (e.HorizontalChange > _dragDistanceNegative)
                    this.ResetLayoutRoot();
                else
                    this.CloseSettings();
            }
        }

        private void SettingsStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            ResetLayoutRoot();
        }

        private void ResetLayoutRoot()
        {
            if (!_isSettingsOpen)
            {
                _feRoot.SetHorizontalOffset(0.0);
                _feSettings.SetHorizontalOffset(-480.0);
            }
            else
            {
                _feRoot.SetHorizontalOffset(380.0);
                _feSettings.SetHorizontalOffset(480.0);
            }
        }
    }
}