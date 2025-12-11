using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaloniaUILoudnessMeter.ViewModels;

namespace AvaloniaUILoudnessMeter.Views;

public partial class MainView : UserControl
{
    #region  Private Members
    
    private Control _channelConfigButton;
    private Control _channelConfigPopup;
    private Control _mainGrid;
    private bool _isUpdatingLayout;

    #endregion
    
    public MainView()
    {
        InitializeComponent();

        _channelConfigButton = this.FindControl<Control>("ChannelConfigurationButton") ?? throw new Exception("Cannot find Channel Configuration Button Binding");
        _channelConfigPopup = this.FindControl<Control>("ChannelConfigurationPopup") ?? throw new Exception("Cannot find Channel Configuration Popup Binding");
        _mainGrid = this.FindControl<Control>("MainGrid") ?? throw new Exception("Cannot find Main Grid Binding");

        // Subscribe to OnLayoutUpdated to position the popup after layout is complete
        LayoutUpdated += OnLayoutUpdated;
    }

    protected override async void OnLoaded(RoutedEventArgs e)
    {
        await ((MainViewModel)DataContext).LoadSettingsCommand.ExecuteAsync(null);
        
        base.OnLoaded(e);
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        if (_isUpdatingLayout)
        {
            return;
        }

        // Get relative position of button in relation to main grid
        Point? position = _channelConfigButton.TranslatePoint(new Point(), _mainGrid)
            ?? throw new Exception("Cannot get TranslatePoint from Configuration Button");

        // Set margin of popup so it appears bottom left of button
        Thickness newMargin = new(
            position.Value.X,
            0,
            0,
            _mainGrid.Bounds.Height - position.Value.Y);

        // Only update if margin actually changed to avoid unnecessary layout cycles
        if (_channelConfigPopup.Margin != newMargin)
        {
            _isUpdatingLayout = true;
            _channelConfigPopup.Margin = newMargin;
            Dispatcher.UIThread.Post(() => _isUpdatingLayout = false, DispatcherPriority.Background);
        }
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        => ((MainViewModel)DataContext).ChannelConfigurationButtonPressedCommand.Execute(null);
}