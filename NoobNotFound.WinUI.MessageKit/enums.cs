using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NoobNotFound.WinUI.MessageKit;


/// <summary>
/// Results that can be returned from a MessageBox
/// </summary>
public enum MessageBoxResult
{
    None,
    OK,
    Cancel,
    Yes,
    No,
    Retry,
    Ignore,
    Abort,
    Custom1,
    Custom2,
    Custom3,
    Timeout,
    Error
}

/// <summary>
/// Predefined button configurations for MessageBox
/// </summary>
public enum MessageBoxButtons
{
    OK,
    OKCancel,
    YesNo,
    YesNoCancel,
    RetryCancel,
    AbortRetryIgnore,
    Custom
}

/// <summary>
/// Icon types for MessageBox
/// </summary>
public enum MessageBoxIcon
{
    None,
    Information,
    Warning,
    Error,
    Question,
    Success,
    Custom
}

/// <summary>
/// Sound effects for MessageBox
/// </summary>
public enum MessageBoxSound
{
    None,
    Default,
    Information,
    Warning,
    Error,
    Question
}