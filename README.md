# OpenSilver Button Controls Samples

This repository contains practical example code for OpenSilver Button controls, demonstrating various implementations and functionalities. The project aims to help developers better understand and use Button controls in OpenSilver.

## Project Background

This project serves as supplementary material to the official OpenSilver technical documentation. The official documentation provides detailed explanations and usage instructions for Button controls, including interactive demonstrations.

To help developers gain a deeper understanding of how OpenSilver Button controls are implemented, we've created this dedicated code repository containing complete source code for all examples, allowing developers to directly view, learn from, and use these samples.

## Content Overview

The repository includes the following Button control sample implementations:

- **OSFSample.ButtonContent**: Demonstrates Button content configuration and binding
- **OSFSample.ButtonClick**: Shows Button click event handling
- **OSFSample.ButtonCommand**: Demonstrates basic Command binding usage
- **OSFSample.ButtonCommandCanExecute**: Shows Command usage with CanExecute
- **OSFSample.ButtonDisabled**: Demonstrates Button disabled state control
- **OSFSample.Support**: Utility functions and helper classes

Each sample showcases different uses and features of OpenSilver Button controls, helping developers quickly understand how to implement similar functionality in their own applications.

## Sample Code

Below is the complete code for the Button Click Event sample, demonstrating how to handle Button click events and update UI elements:

### XAML Part:

```xml
<units:ExampleBase x:Class="OSFSample.ButtonClick.ButtonClickExample"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:units="clr-namespace:OSFSample.Support.UI.Units;assembly=OSFSample.Support"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             Foreground="{DynamicResource Theme_TextBrush}"
             Height="200"
             MaxWidth="780"
             mc:Ignorable="d">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="30"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>
        <Border Style="{StaticResource ExampleHeaderBorder}">
            <TextBlock Style="{StaticResource ExampleHeaderItemAssemblyName}"/>
        </Border>
        <Border Style="{StaticResource ExampleHeaderItemBorder}">
            <TextBlock Style="{StaticResource ExampleHeaderItemText}"/>
        </Border>
        <Border Style="{StaticResource ExampleContentBorder}"/>
        <StackPanel Style="{StaticResource ExampleContentStackPanel}">
            <Button Content="XAML Button Click" Click="Button_Click" Margin="10" Padding="10 5 10 7" />
            <TextBlock x:Name="btnCount" Text="Click Count: 0" Margin="10"/>
        </StackPanel>
    </Grid>
</units:ExampleBase>
```

### C# Part:

```csharp
using OSFSample.Support.UI.Units;
using System.Windows;
namespace OSFSample.ButtonClick;

public partial class ButtonClickExample : ExampleBase
{
    int count = 0;
    
    public ButtonClickExample()
    {
        this.InitializeComponent();
    }
    
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        count++;
        btnCount.Text = $"Click Count: {count}";
        System.Windows.MessageBox.Show("Button Clicked!");
    }
}
```

This example demonstrates how to update a counter text and display a message box when a button is clicked. The repository also includes other types of Button control samples such as content configuration, Command binding, CanExecute functionality, and disabled state control.

## How to Use

1. Clone this repository
2. Open the solution using an IDE that supports OpenSilver (such as Visual Studio)
3. Explore the code implementation in each sample folder
4. Run the project to see the actual effects

## Related Resources

- [OpenSilver Official Website](https://opensilver.net/)
- [OpenSilver Documentation](https://opensilverdev.azurewebsites.net/docs/9/4168)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Issues and pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.
