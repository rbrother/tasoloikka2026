using Godot;

namespace Tasoloikka2026.UI;

public partial class PauseMenu : CanvasLayer
{
    private Control? _panel;
    private Button? _restartButton;
    private Button? _quitButton;
    private bool _isOpen;

    public override void _Ready()
    {
        _panel = GetNodeOrNull<Control>("Panel");
        _restartButton = GetNodeOrNull<Button>("Panel/VBox/RestartButton");
        _quitButton = GetNodeOrNull<Button>("Panel/VBox/QuitButton");

        if (_restartButton != null)
        {
            _restartButton.Pressed += OnRestartPressed;
        }

        if (_quitButton != null)
        {
            _quitButton.Pressed += OnQuitPressed;
        }

        SetMenuVisible(false);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            ToggleMenu();
            GetViewport().SetInputAsHandled();
        }
    }

    private void ToggleMenu()
    {
        _isOpen = !_isOpen;
        SetMenuVisible(_isOpen);
        GetTree().Paused = _isOpen;

        if (_isOpen)
        {
            _restartButton?.GrabFocus();
        }
    }

    private void SetMenuVisible(bool visible)
    {
        if (_panel != null)
        {
            _panel.Visible = visible;
        }
    }

    private void OnRestartPressed()
    {
        GetTree().Paused = false;
        GetTree().ReloadCurrentScene();
    }

    private void OnQuitPressed()
    {
        GetTree().Paused = false;
        GetTree().Quit();
    }
}
