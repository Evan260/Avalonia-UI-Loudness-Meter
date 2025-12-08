using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

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
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        // Get relative position of button in relation to main grid
        var position = mChannelConfigButton.TranslatePoint(new Point(), mMainGrid) ?? throw new Exception("Cannot get TranslatePoint from Configuration Button");

        // Set margin of popup so it appears bottom left of button
        mChannelConfigPopup.Margin = new Thickness(
            position.X, 
            0, 
            0, 
            mMainGrid.Bounds.Height - position.Y - mChannelConfigButton.Bounds.Height);
    }
}