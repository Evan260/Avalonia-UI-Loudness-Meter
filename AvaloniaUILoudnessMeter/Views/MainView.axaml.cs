using System;
using Avalonia;
using Avalonia.Controls;

namespace AvaloniaUILoudnessMeter.Views;

public partial class MainView : UserControl
{
    #region  Private Members
    
    private Control mChannelConfigButton;
    private Control mChannelConfigPopup;
    private Control mMainGrid;

    #endregion
    
    public MainView()
    {
        InitializeComponent();

        mChannelConfigButton = this.FindControl<Control>("ChannelConfigurationButton") ?? throw new Exception("Cannot find Channel Configuration Button Binding");
        mChannelConfigPopup = this.FindControl<Control>("ChannelConfigurationPopup") ?? throw new Exception("Cannot find Channel Configuration Popup Binding");
        mMainGrid = this.FindControl<Control>("MainGrid") ?? throw new Exception("Cannot find Main Grid Binding");

        // Subscribe to LayoutUpdated to position the popup after layout is complete
        LayoutUpdated += OnLayoutUpdated;
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        // Get relative position of button in relation to main grid
        Point? position = mChannelConfigButton.TranslatePoint(new Point(), mMainGrid)
            ?? throw new Exception("Cannot get TranslatePoint from Configuration Button");

        // Set margin of popup so it appears bottom left of button
        Thickness newMargin = new(
            position.Value.X,
            0,
            0,
            mMainGrid.Bounds.Height - position.Value.Y - mChannelConfigButton.Bounds.Height);

        // Only update if margin actually changed to avoid unnecessary layout cycles
        if (mChannelConfigPopup.Margin != newMargin)
        {
            mChannelConfigPopup.Margin = newMargin;
        }
    }
}