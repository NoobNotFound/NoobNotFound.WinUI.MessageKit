# NoobNotFound.WinUI.MessageKit

A modern, feature-rich message dialog framework for WinUI 3 applications. This library provides an elegant alternative to traditional message boxes with enhanced customization, theming, and user experience features.

## Features

- 🎨 **Modern WinUI 3 Design** - Seamlessly integrates with your app's theme
- 🔊 **System Sounds** - Built-in sound effects for different message types
- ⏱️ **Timeout Support** - Auto-dismiss dialogs after specified time
- 🌐 **Localization Ready** - Easy text localization support  
- 🎭 **Multiple Icons** - Information, Warning, Error, Question, Success, and Custom icons
- 🔘 **Flexible Buttons** - Predefined button sets or fully custom buttons
- 📱 **Queue Management** - Prevents dialog conflicts with built-in queueing
- 🎯 **Response Metadata** - Track response times and additional context
- 🎨 **Theme Aware** - Automatic light/dark theme support
- ⚡ **Async/Await** - Modern async API throughout

## Installation

Install via NuGet Package Manager:

```
Install-Package NoobNotFound.WinUI.MessageKit
```

Or via .NET CLI:

```
dotnet add package NoobNotFound.WinUI.MessageKit
```

## Quick Start

### Basic Setup

First, configure the library in your `App.xaml.cs` or main window:

```csharp
using NoobNotFound.WinUI.MessageKit;

// Set the XamlRoot provider (usually in your MainWindow)
Core.XamlRootProvider = () => MainWindow.Content.XamlRoot;

// Optional: Configure theme provider
Core.ThemeProvider = () => ActualTheme;

// Optional: Configure localization
Core.LocalizationProvider = text => ResourceManager.GetString(text);
```

### Simple Usage

```csharp
// Simple information message
await MessageBox.ShowAsync("Hello, World!");

// Message with title
await MessageBox.ShowAsync("Success", "Operation completed successfully!");

// Confirmation dialog
var response = await MessageBox.ShowConfirmationAsync(
    "Delete File", 
    "Are you sure you want to delete this file?"
);

if (response.Result == MessageBoxResult.Yes)
{
    // User confirmed
}
```

## Advanced Usage

### Custom Options

```csharp
var options = new MessageBoxOptions
{
    Title = "Custom Dialog",
    Message = "This is a custom message with advanced options.",
    Icon = MessageBoxIcon.Warning,
    Buttons = MessageBoxButtons.YesNoCancel,
    Sound = MessageBoxSound.Warning,
    DefaultResult = MessageBoxResult.No,
    Timeout = TimeSpan.FromSeconds(30),
    Theme = ElementTheme.Dark,
    MaxWidth = 500
};

var response = await MessageBox.ShowAsync(options);
```

### Custom Buttons

```csharp
var options = new MessageBoxOptions
{
    Title = "Custom Actions",
    Message = "Choose your action:",
    Buttons = MessageBoxButtons.Custom,
    CustomButtons = new List<CustomButton>
    {
        new CustomButton 
        { 
            Text = "Save", 
            Result = MessageBoxResult.Custom1,
            IsDefault = true 
        },
        new CustomButton 
        { 
            Text = "Save As...", 
            Result = MessageBoxResult.Custom2 
        },
        new CustomButton 
        { 
            Text = "Cancel", 
            Result = MessageBoxResult.Cancel,
            IsCancel = true 
        }
    }
};

var response = await MessageBox.ShowAsync(options);

switch (response.Result)
{
    case MessageBoxResult.Custom1:
        // Handle Save
        break;
    case MessageBoxResult.Custom2:
        // Handle Save As
        break;
    case MessageBoxResult.Cancel:
        // Handle Cancel
        break;
}
```

### With Timeout and Metadata

```csharp
var options = new MessageBoxOptions
{
    Title = "Timed Message",
    Message = "This dialog will auto-close in 10 seconds.",
    Icon = MessageBoxIcon.Information,
    Timeout = TimeSpan.FromSeconds(10),
    DefaultResult = MessageBoxResult.OK
};

options.WithMetadata("UsageContext", "AutoSave")
       .WithMetadata("Cache", true);

var response = await MessageBox.ShowAsync(options);

if (response.TimedOut)
{
    // Handle timeout
    Console.WriteLine($"Dialog timed out after {response.ResponseTime}");
}
```

### Custom Icons

```csharp
var options = new MessageBoxOptions
{
    Title = "Custom Icon",
    Message = "This dialog uses a custom icon.",
    Icon = MessageBoxIcon.Custom,
    CustomIconPath = "ms-appx:///Assets/my-icon.png"
};

await MessageBox.ShowAsync(options);
```

## Predefined Methods

The library includes several convenience methods for common scenarios:

```csharp
// Information message
await MessageBox.ShowAsync("Info message");

// Error message
await MessageBox.ShowErrorAsync("Error", "Something went wrong!");

// Warning message  
await MessageBox.ShowWarningAsync("Warning", "Please review your input.");

// Confirmation dialog
var confirmed = await MessageBox.ShowConfirmationAsync(
    "Confirm Action", 
    "Do you want to continue?"
);
```

## Configuration Options

### MessageBoxOptions Properties

| Property | Type | Description |
|----------|------|-------------|
| `Title` | string | Dialog title text |
| `Message` | string | Main message content |
| `Buttons` | MessageBoxButtons | Button configuration |
| `Icon` | MessageBoxIcon | Icon type to display |
| `Sound` | MessageBoxSound | Sound to play |
| `DefaultResult` | MessageBoxResult | Default button/result |
| `Timeout` | TimeSpan? | Auto-close timeout |
| `Theme` | ElementTheme? | Light/Dark theme override |
| `MaxWidth` | double | Maximum dialog width |
| `MaxHeight` | double | Maximum dialog height |
| `CustomIconPath` | string | Path for custom icons |
| `CustomButtons` | List<CustomButton> | Custom button definitions |
| `Metadata` | Dictionary<string, object> | Additional data |

### Enums

**MessageBoxButtons**
- `OK` - Single OK button
- `OKCancel` - OK and Cancel buttons  
- `YesNo` - Yes and No buttons
- `YesNoCancel` - Yes, No, and Cancel buttons
- `RetryCancel` - Retry and Cancel buttons
- `AbortRetryIgnore` - Abort, Retry, and Ignore buttons
- `Custom` - Use CustomButtons collection

**MessageBoxIcon**
- `None` - No icon
- `Information` - Information icon (blue)
- `Warning` - Warning icon (orange)
- `Error` - Error icon (red)
- `Question` - Question icon (blue)
- `Success` - Success icon (green)
- `Custom` - Custom icon from path

**MessageBoxResult**
- `None`, `OK`, `Cancel`, `Yes`, `No`, `Retry`, `Ignore`, `Abort`
- `Custom1`, `Custom2`, `Custom3` - For custom buttons
- `Timeout` - Dialog timed out
- `Error` - An error occurred

## Response Information

The `MessageBoxResponse` object provides detailed information about user interaction:

```csharp
var response = await MessageBox.ShowAsync(options);

Console.WriteLine($"Result: {response.Result}");
Console.WriteLine($"Response Time: {response.ResponseTime}");
Console.WriteLine($"Timed Out: {response.TimedOut}");

// Access custom metadata
if (response.Metadata.ContainsKey("CustomData"))
{
    var data = response.Metadata["CustomData"];
}
```

## Queue Management

The library automatically queues dialogs to prevent conflicts. For immediate display without queueing:

```csharp
// Queued (default behavior)
var response1 = await MessageBox.ShowAsync(options);

// Immediate (no queue)
var response2 = await MessageBox.ShowImmediateAsync(options);
```

## Extension Methods

Fluent API for easier configuration:

```csharp
var options = new MessageBoxOptions
{
    Title = "Example",
    Message = "Fluent configuration example"
}
.WithTimeout(TimeSpan.FromSeconds(15))
.WithIcon(MessageBoxIcon.Question)
.WithSound(MessageBoxSound.Question)
.WithMetadata("Source", "FluentExample");
```

## Error Handling

```csharp
try 
{
    var response = await MessageBox.ShowAsync(options);
    // Handle response
}
catch (Exception ex)
{
    // Handle any errors
    Console.WriteLine($"MessageBox error: {ex.Message}");
}
```

## Best Practices

1. **Always set XamlRootProvider** in your app initialization
2. **Use appropriate icons** for different message types
3. **Set reasonable timeouts** for auto-dismissing dialogs
4. **Handle timeout scenarios** in your application logic
5. **Use confirmation dialogs** for destructive actions
6. **Leverage metadata** for tracking and analytics
7. **Configure localization** for multi-language apps

## Requirements

- .NET 8.0 or later
- Windows 10 version 1809 (build 17763) or later
- WinUI 3 / Windows App SDK

## Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests.

## License

This project is licensed under the GNU Public License - see the LICENSE file for details.
