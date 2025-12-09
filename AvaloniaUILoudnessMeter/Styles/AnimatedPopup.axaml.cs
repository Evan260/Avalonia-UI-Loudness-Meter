using System;
using System.Threading;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaUILoudnessMeter;

public partial class AnimatedPopup : ContentControl
{
    #region Private Members

    /// <summary>
    /// The underlay control for closing this popup.
    /// </summary>
    private Control _underlayControl;

    /// <summary>
    /// Indicates if this is the first time we are animating.
    /// </summary>
    private bool _firstAnimation = true;

    /// <summary>
    /// Indicates if we have captured the opacity value yet.
    /// </summary>
    private bool _opacityCaptured;

    /// <summary>
    /// Store the controls original Opacity value at startup.
    /// </summary>
    private double _originalOpacity;

    /// <summary>
    /// The speed of the animation in FPS.
    /// </summary>
    private readonly TimeSpan _framerate = TimeSpan.FromSeconds(1 / 60.0);

    /// <summary>
    /// Calculate total ticks that make up the animation time.
    /// </summary>
    private int _totalTicks => (int)(_animationTime.TotalSeconds / _framerate.TotalSeconds);

    /// <summary>
    /// Store the controls desired size.
    /// </summary>
    private Size _desiredSize;

    /// <summary>
    /// A flag for when we are animating.
    /// </summary>
    private bool _animating;

    /// <summary>
    /// Keeps track of if we have found the desired 100% width/height auto size.
    /// </summary>
    private bool _sizeFound;

    /// <summary>
    /// The animation UI timer.
    /// </summary>
    private readonly DispatcherTimer _animationTimer;

    /// <summary>
    /// The timeout timer to detect when auto-sizing has finished firing.
    /// </summary>
    private Timer _sizingTimer;

    /// <summary>
    /// The current position in the animation.
    /// </summary>
    private int _currentAnimationTick;

    #endregion

    #region Public Properties

    /// <summary>
    /// Indicates if the control is currently fully opened.
    /// </summary>
    public bool IsOpened => _currentAnimationTick >= _totalTicks;

    #region Open

    private bool _open;

    public static readonly DirectProperty<AnimatedPopup, bool> OpenProperty =
        AvaloniaProperty.RegisterDirect<AnimatedPopup, bool>(
            nameof(Open), o => o.Open, (o, v) => o.Open = v);

    /// <summary>
    /// Property to set whether the control should be open or closed.
    /// </summary>
    public bool Open
    {
        get => _open;
        set
        {
            if (value == _open)
                return;

            if (value)
            {
                if (Parent is Grid grid)
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        if (grid.RowDefinitions?.Count > 0)
                            _underlayControl.SetValue(Grid.RowSpanProperty, grid.RowDefinitions.Count);

                        if (grid.ColumnDefinitions?.Count > 0)
                            _underlayControl.SetValue(Grid.ColumnSpanProperty, grid.ColumnDefinitions.Count);

                        if (!grid.Children.Contains(_underlayControl))
                            grid.Children.Insert(0, _underlayControl);
                    });
                }
            }
            else
            {
                if (IsOpened)
                    UpdateDesiredSize();
            }

            UpdateAnimation();
            SetAndRaise(OpenProperty, ref _open, value);
        }
    }

    #endregion

    #region Animation Time

    private TimeSpan _animationTime = TimeSpan.FromSeconds(3);

    public static readonly DirectProperty<AnimatedPopup, TimeSpan> AnimationTimeProperty =
        AvaloniaProperty.RegisterDirect<AnimatedPopup, TimeSpan>(
            nameof(AnimationTime), o => o.AnimationTime, (o, v) => o.AnimationTime = v);

    /// <summary>
    /// The duration of the animation.
    /// </summary>
    public TimeSpan AnimationTime
    {
        get => _animationTime;
        set => SetAndRaise(AnimationTimeProperty, ref _animationTime, value);
    }

    #endregion

    #region Animate Opacity

    private bool _animateOpacity = true;

    public static readonly DirectProperty<AnimatedPopup, bool> AnimateOpacityProperty =
        AvaloniaProperty.RegisterDirect<AnimatedPopup, bool>(
            nameof(AnimateOpacity), o => o.AnimateOpacity, (o, v) => o.AnimateOpacity = v);

    /// <summary>
    /// Whether to animate the popup's opacity.
    /// </summary>
    public bool AnimateOpacity
    {
        get => _animateOpacity;
        set => SetAndRaise(AnimateOpacityProperty, ref _animateOpacity, value);
    }

    #endregion

    #region Underlay Opacity

    private double _underlayOpacity = 0.2;

    public static readonly DirectProperty<AnimatedPopup, double> UnderlayOpacityProperty =
        AvaloniaProperty.RegisterDirect<AnimatedPopup, double>(
            nameof(UnderlayOpacity), o => o.UnderlayOpacity, (o, v) => o.UnderlayOpacity = v);

    /// <summary>
    /// The opacity of the underlay when fully open.
    /// </summary>
    public double UnderlayOpacity
    {
        get => _underlayOpacity;
        set => SetAndRaise(UnderlayOpacityProperty, ref _underlayOpacity, value);
    }

    #endregion

    #endregion

    #region Public Commands

    [RelayCommand]
    public void BeginOpen()
    {
        Open = true;
    }

    [RelayCommand]
    public void BeginClose()
    {
        Open = false;
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Default constructor.
    /// </summary>
    public AnimatedPopup()
    {
        _underlayControl = new Border
        {
            Background = Brushes.Black,
            Opacity = 0,
            ZIndex = 9
        };

        _underlayControl.PointerPressed += (sender, args) => BeginClose();

        _animationTimer = new DispatcherTimer
        {
            Interval = _framerate
        };

        _sizingTimer = new Timer(t =>
        {
            if (_sizeFound)
                return;

            _sizeFound = true;

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                UpdateDesiredSize();
                UpdateAnimation();
            });
        });

        _animationTimer.Tick += (s, e) => AnimationTick();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Updates the animation desired size based on the current visuals desired size.
    /// </summary>
    private void UpdateDesiredSize() => _desiredSize = DesiredSize - Margin;

    /// <summary>
    /// Calculate and start any new required animations.
    /// </summary>
    private void UpdateAnimation()
    {
        if (!_sizeFound)
            return;

        _animationTimer.Start();
    }

    /// <summary>
    /// Should be called when an open or close transition has completed.
    /// </summary>
    private void AnimationComplete()
    {
        if (_open)
        {
            Width = double.NaN;
            Height = double.NaN;
            Opacity = _originalOpacity;
        }
        else
        {
            Width = 0;
            Height = 0;

            if (Parent is Grid grid)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _underlayControl.Opacity = 0;

                    if (grid.Children.Contains(_underlayControl))
                        grid.Children.Remove(_underlayControl);
                });
            }
        }
    }

    /// <summary>
    /// Update controls sizes based on the next tick of an animation.
    /// </summary>
    private void AnimationTick()
    {
        if (_firstAnimation)
        {
            _firstAnimation = false;
            _animationTimer.Stop();
            Opacity = _originalOpacity;
            AnimationComplete();
            return;
        }

        if ((_open && _currentAnimationTick >= _totalTicks) ||
            (!_open && _currentAnimationTick == 0))
        {
            _animationTimer.Stop();
            AnimationComplete();
            _animating = false;
            return;
        }

        _animating = true;
        _currentAnimationTick += _open ? 1 : -1;

        var percentageAnimated = (float)_currentAnimationTick / _totalTicks;

        var quadraticEasing = new QuadraticEaseIn();
        var linearEasing = new LinearEasing();

        var finalWidth = _desiredSize.Width * quadraticEasing.Ease(percentageAnimated);
        var finalHeight = _desiredSize.Height * quadraticEasing.Ease(percentageAnimated);

        Width = finalWidth;
        Height = finalHeight;

        if (AnimateOpacity)
            Opacity = _originalOpacity * linearEasing.Ease(percentageAnimated);

        _underlayControl.Opacity = _underlayOpacity * quadraticEasing.Ease(percentageAnimated);
    }

    #endregion

    #region Overrides

    public override void Render(DrawingContext context)
    {
        if (!_sizeFound)
        {
            if (!_opacityCaptured)
            {
                _opacityCaptured = true;
                _originalOpacity = Opacity;
                Dispatcher.UIThread.InvokeAsync(() => Opacity = 0);
            }

            _sizingTimer.Change(100, int.MaxValue);
        }

        base.Render(context);
    }

    #endregion
}