using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Microsoft.UI;
using System.Threading.Tasks;
using System.Threading;

using System;
using System.Media;
namespace NoobNotFound.WinUI.MessageKit;

/// <summary>
/// Modern, feature-rich MessageBox implementation for WinUI 3
/// </summary>
public sealed class MessageBox : ContentDialog
{
    private readonly MessageBoxOptions _options;
    private readonly TaskCompletionSource<MessageBoxResponse> _completionSource;
    private readonly CancellationTokenSource _timeoutCancellation;
    private readonly DateTime _startTime;
    private Timer _timeoutTimer;

    public MessageBoxResponse Response { get; private set; }

    private MessageBox(MessageBoxOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _completionSource = new TaskCompletionSource<MessageBoxResponse>();
        _timeoutCancellation = new CancellationTokenSource();
        _startTime = DateTime.UtcNow;

        InitializeDialog();
        SetupButtons();
        SetupTimeout();
        PlaySound();

        this.Closed += OnClosed;
    }

    private void InitializeDialog()
    {
        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        Title = _options.Title;
        RequestedTheme = _options.Theme ?? GetCurrentTheme();

        // Create content with icon support
        var content = CreateContentWithIcon();
        Content = content;

        // Set XamlRoot safely
        SetXamlRoot();
    }

    private FrameworkElement CreateContentWithIcon()
    {
        var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16 };

        // Add icon if specified
        if (_options.Icon != MessageBoxIcon.None)
        {
            var iconElement = CreateIconElement();
            if (iconElement != null)
            {
                panel.Children.Add(iconElement);
            }
        }

        // Add message text
        var textBlock = new TextBlock
        {
            Text = _options.Message,
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = _options.MaxWidth - 100, // Account for icon space
            VerticalAlignment = VerticalAlignment.Center
        };

        panel.Children.Add(textBlock);
        return panel;
    }

    private FrameworkElement CreateIconElement()
    {
        if (_options.Icon == MessageBoxIcon.Custom && !string.IsNullOrEmpty(_options.CustomIconPath))
        {
            return new Image
            {
                Source = new BitmapImage(new Uri(_options.CustomIconPath)),
                Width = 32,
                Height = 32,
                VerticalAlignment = VerticalAlignment.Top
            };
        }

        // Use system icons
        var iconGlyph = GetIconGlyph(_options.Icon);
        if (string.IsNullOrEmpty(iconGlyph)) return null;

        return new FontIcon
        {
            Glyph = iconGlyph,
            FontSize = 32,
            Foreground = GetIconBrush(_options.Icon),
            VerticalAlignment = VerticalAlignment.Top
        };
    }

    private string GetIconGlyph(MessageBoxIcon icon) => icon switch
    {
        MessageBoxIcon.Information => "\uE946",
        MessageBoxIcon.Warning => "\uE7BA",
        MessageBoxIcon.Error => "\uE783",
        MessageBoxIcon.Question => "\uE9CE",
        MessageBoxIcon.Success => "\uE73E",
        _ => ""
    };

    private Brush GetIconBrush(MessageBoxIcon icon) => icon switch
    {
        MessageBoxIcon.Information => new SolidColorBrush(Colors.DeepSkyBlue),
        MessageBoxIcon.Warning => new SolidColorBrush(Colors.Orange),
        MessageBoxIcon.Error => new SolidColorBrush(Colors.Red),
        MessageBoxIcon.Question => new SolidColorBrush(Colors.Blue),
        MessageBoxIcon.Success => new SolidColorBrush(Colors.Green),
        _ => new SolidColorBrush(Colors.Gray)
    };

    private void SetXamlRoot()
    {
        try
        {
            XamlRoot = Core.GetXamlRoot();
        }
        catch
        {
            // XamlRoot setting failed, dialog may still work
        }
    }

    private void SetupButtons()
    {
        if (_options.Buttons == MessageBoxButtons.Custom && _options.CustomButtons.Count > 0)
        {
            SetupCustomButtons();
        }
        else
        {
            SetupPredefinedButtons();
        }

        // Set default button
        SetDefaultButton();
    }

    private void SetupCustomButtons()
    {
        var buttons = _options.CustomButtons;

        if (buttons.Count > 0)
        {
            PrimaryButtonText = buttons[0].Text;
        }
        if (buttons.Count > 1)
        {
            SecondaryButtonText = buttons[1].Text;
        }
        if (buttons.Count > 2)
        {
            CloseButtonText = buttons[2].Text;
        }
    }

    private void SetupPredefinedButtons()
    {
        var buttonTexts = GetButtonTexts(_options.Buttons);

        PrimaryButtonText = buttonTexts.Primary;
        SecondaryButtonText = buttonTexts.Secondary;
        CloseButtonText = buttonTexts.Close;
    }

    private (string Primary, string Secondary, string Close) GetButtonTexts(MessageBoxButtons buttons) => buttons switch
    {
        MessageBoxButtons.OK => ("", "OK".Localize(), ""),
        MessageBoxButtons.OKCancel => ("OK".Localize(), "Cancel".Localize(), ""),
        MessageBoxButtons.YesNo => ("Yes".Localize(), "No".Localize(), ""),
        MessageBoxButtons.YesNoCancel => ("Yes".Localize(), "No".Localize(), "Cancel".Localize()),
        MessageBoxButtons.RetryCancel => ("Retry".Localize(), "Cancel".Localize(), ""),
        MessageBoxButtons.AbortRetryIgnore => ("Abort".Localize(), "Retry".Localize(), "Ignore".Localize()),
        _ => ("", "", "")
    };

    private void SetDefaultButton()
    {
        DefaultButton = _options.DefaultResult switch
        {
            MessageBoxResult.OK or MessageBoxResult.Yes or MessageBoxResult.Retry or MessageBoxResult.Abort => ContentDialogButton.Primary,
            MessageBoxResult.No or MessageBoxResult.Cancel or MessageBoxResult.Ignore => ContentDialogButton.Secondary,
            _ => ContentDialogButton.Primary
        };
    }

    private void SetupTimeout()
    {
        if (_options.Timeout.HasValue)
        {
            _timeoutTimer = new Timer(OnTimeout, null, _options.Timeout.Value, Timeout.InfiniteTimeSpan);
        }
    }

    private void OnTimeout(object state)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            CompleteDialog(new MessageBoxResponse
            {
                Result = _options.DefaultResult != MessageBoxResult.None ? _options.DefaultResult : MessageBoxResult.Timeout,
                TimedOut = true,
                ResponseTime = DateTime.UtcNow - _startTime
            });
            Hide();
        });
    }

    private void PlaySound()
    {
        if (_options.Sound == MessageBoxSound.None) return;

        switch (_options.Sound)
        {
            case MessageBoxSound.Default:
            case MessageBoxSound.Information:
            case MessageBoxSound.Question:
                SystemSounds.Asterisk.Play();
                break;

            case MessageBoxSound.Warning:
                SystemSounds.Exclamation.Play();
                break;

            case MessageBoxSound.Error:
                SystemSounds.Hand.Play();
                break;

            default:
                SystemSounds.Beep.Play();
                break;
        }
    }

    private void CompleteDialog(MessageBoxResponse response)
    {
        Response = response;
        _timeoutTimer?.Dispose();
        _timeoutCancellation?.Cancel();

        if (!_completionSource.Task.IsCompleted)
        {
            _completionSource.SetResult(response);
        }
    }

    private void OnClosed(ContentDialog c,ContentDialogClosedEventArgs args)
    {
        var result = MapDialogResult(args.Result);
        var response = new MessageBoxResponse
        {
            Result = result,
            ResponseTime = DateTime.UtcNow - _startTime,
            Metadata = _options.Metadata
        };

        CompleteDialog(response);
    }

    private MessageBoxResult MapDialogResult(ContentDialogResult dialogResult)
    {
        if (_options.Buttons == MessageBoxButtons.Custom && _options.CustomButtons.Count > 0)
        {
            return dialogResult switch
            {
                ContentDialogResult.Primary => _options.CustomButtons.Count > 0 ? _options.CustomButtons[0].Result : MessageBoxResult.Custom1,
                ContentDialogResult.Secondary => _options.CustomButtons.Count > 1 ? _options.CustomButtons[1].Result : MessageBoxResult.Custom2,
                ContentDialogResult.None => _options.CustomButtons.Count > 2 ? _options.CustomButtons[2].Result : MessageBoxResult.Cancel,
                _ => MessageBoxResult.None
            };
        }

        return _options.Buttons switch
        {
            MessageBoxButtons.OK => MessageBoxResult.OK,
            MessageBoxButtons.OKCancel => dialogResult == ContentDialogResult.Primary ? MessageBoxResult.OK : MessageBoxResult.Cancel,
            MessageBoxButtons.YesNo => dialogResult == ContentDialogResult.Primary ? MessageBoxResult.Yes : MessageBoxResult.No,
            MessageBoxButtons.YesNoCancel => dialogResult switch
            {
                ContentDialogResult.Primary => MessageBoxResult.Yes,
                ContentDialogResult.Secondary => MessageBoxResult.No,
                _ => MessageBoxResult.Cancel
            },
            MessageBoxButtons.RetryCancel => dialogResult == ContentDialogResult.Primary ? MessageBoxResult.Retry : MessageBoxResult.Cancel,
            MessageBoxButtons.AbortRetryIgnore => dialogResult switch
            {
                ContentDialogResult.Primary => MessageBoxResult.Abort,
                ContentDialogResult.Secondary => MessageBoxResult.Retry,
                _ => MessageBoxResult.Ignore
            },
            _ => MessageBoxResult.None
        };
    }

    private static ElementTheme GetCurrentTheme()
    {
        try
        {
            return Core.GetCurrentTheme();
        }
        catch
        {
            // Fallback
        }

        return ElementTheme.Default;
    }

    #region Static Factory Methods

    /// <summary>
    /// Shows a MessageBox with comprehensive options (uses queue by default)
    /// </summary>
    public static Task<MessageBoxResponse> ShowAsync(MessageBoxOptions options) =>
        MessageBoxManager.ShowQueuedAsync(options);

    /// <summary>
    /// Shows a MessageBox with comprehensive options without queueing
    /// </summary>
    public static async Task<MessageBoxResponse> ShowImmediateAsync(MessageBoxOptions options)
    {
        var messageBox = new MessageBox(options);

        try
        {
            await messageBox.ShowAsync();
            return await messageBox._completionSource.Task;
        }
        catch (Exception ex)
        {
            return new MessageBoxResponse
            {
                Result = MessageBoxResult.Error,
                Error = ex,
                ResponseTime = DateTime.UtcNow - messageBox._startTime
            };
        }
    }

    /// <summary>
    /// Shows a simple information message (queued)
    /// </summary>
    public static Task<MessageBoxResponse> ShowAsync(string message) =>
        ShowAsync(new MessageBoxOptions
        {
            Title = "Information".Localize(),
            Message = message,
            Icon = MessageBoxIcon.Information
        });

    /// <summary>
    /// Shows a message with title (queued)
    /// </summary>
    public static Task<MessageBoxResponse> ShowAsync(string title, string message) =>
        ShowAsync(new MessageBoxOptions
        {
            Title = title,
            Message = message,
            Icon = MessageBoxIcon.Information
        });

    /// <summary>
    /// Shows a confirmation dialog (queued)
    /// </summary>
    public static Task<MessageBoxResponse> ShowConfirmationAsync(string title, string message) =>
        ShowAsync(new MessageBoxOptions
        {
            Title = title,
            Message = message,
            Buttons = MessageBoxButtons.YesNo,
            Icon = MessageBoxIcon.Question,
            DefaultResult = MessageBoxResult.No
        });

    /// <summary>
    /// Shows an error message (queued)
    /// </summary>
    public static Task<MessageBoxResponse> ShowErrorAsync(string title, string message) =>
        ShowAsync(new MessageBoxOptions
        {
            Title = title,
            Message = message,
            Buttons = MessageBoxButtons.OK,
            Icon = MessageBoxIcon.Error,
            Sound = MessageBoxSound.Error
        });

    /// <summary>
    /// Shows a warning message (queued)
    /// </summary>
    public static Task<MessageBoxResponse> ShowWarningAsync(string title, string message) =>
        ShowAsync(new MessageBoxOptions
        {
            Title = title,
            Message = message,
            Buttons = MessageBoxButtons.OK,
            Icon = MessageBoxIcon.Warning,
            Sound = MessageBoxSound.Warning
        });

    #endregion
}