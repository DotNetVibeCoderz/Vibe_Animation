using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace MathArt;

public partial class MainWindow : Window
{
    private MathCanvas? _mathCanvas;
    private ListBox? _animationList;
    private Button? _fullScreenToggle;
    private Button? _exitButton;

    public MainWindow()
    {
        InitializeComponent();
        
        _mathCanvas = this.FindControl<MathCanvas>("MathCanvasControl");
        _animationList = this.FindControl<ListBox>("AnimationList");
        _fullScreenToggle = this.FindControl<Button>("FullScreenToggle");
        _exitButton = this.FindControl<Button>("ExitButton");

        if (_animationList != null)
        {
            _animationList.SelectionChanged += AnimationList_SelectionChanged;
        }

        if (_fullScreenToggle != null)
        {
            _fullScreenToggle.Click += FullScreenToggle_Click;
        }

        if (_exitButton != null)
        {
            _exitButton.Click += ExitButton_Click;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void AnimationList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_mathCanvas != null && _animationList != null)
        {
            _mathCanvas.AnimationType = _animationList.SelectedIndex;
        }
    }

    private void FullScreenToggle_Click(object? sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.FullScreen)
        {
            WindowState = WindowState.Normal;
        }
        else
        {
            WindowState = WindowState.FullScreen;
        }
    }

    private void ExitButton_Click(object? sender, RoutedEventArgs e)
    {
        // Menutup aplikasi ketika tombol Exit diklik
        this.Close();
    }
}