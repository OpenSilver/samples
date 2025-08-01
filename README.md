# OpenSilver Samples
A collection of samples and templates for OpenSilver application development.
Provides modularized examples of **XAML, C#, data binding, MVVM** and other XAML-based elements. Each sample is independently structured for easy learning and reuse.

## 🌐 Live Demo
Check out all examples instantly at **[OpenSilver Hub](https://opensilverhub.azurewebsites.net)** (temporary).
- **Real-time execution** - See samples running directly in your browser
- **Interactive testing** - Manipulate controls and learn how they work  
- **View source code** - Instantly examine XAML and C# code
- **Implementation ideas** - Explore various UI patterns and implementation methods

## 🚀 Development Setup

### 1. Install OpenSilver
If OpenSilver is not installed, choose one of the following methods:

**Method 1: Using .NET CLI**
```bash
dotnet new install OpenSilver.Templates
```

**Method 2: Visual Studio Extension**
Download and install the VS extension from https://opensilver.net/download-sdk within Visual Studio

### 2. Clone Repository
```bash
git clone https://github.com/opensilver/samples
cd samples
```

### 3. Install Visual Studio Templates
```bash
cd templates
install_templates.bat
```
Installed templates:
- **Showcase Template (OpenSilver)** - Project template
- **Showcase Content (OpenSilver)** - Item template
- **Showcase Item (OpenSilver)** - Item template

### 4. Open Solution
```bash
cd ../src
# Open OpenSilverSample.sln in Visual Studio
```

## 🏗️ Project Structure
```
samples/
├── templates/
│   ├── install_templates.bat
│   └── vs-templates/
└── src/
    ├── OpenSilverSample.sln     # Main solution
    ├── OpenSilverSample/        # Sample project
    │   └── MainPage.xaml        # Main showcase page
    └── OpenSilverSample.Browser/ # Execution project
```

## 🔧 Creating New Samples
### Add New Project
Right-click solution → Add → New Project → Select "Showcase Template (OpenSilver)"
![Add New Project](https://github.com/user-attachments/assets/08c6eaf8-be4d-4c25-84b2-7368be21f7ed)

### Add New Item
Right-click project → Add → New Item → Select "Showcase Content" or "Showcase Item"
![Add New Item](https://github.com/user-attachments/assets/7f818fcb-9bdd-44e3-8a4d-b28604711fd8)

### Integration and Testing
1. Add new TabItem to `MainPage.xaml`:
```xml
<TabItem Header="MySample">
    <mysample:MySampleContent IsMenuPanelVisible="True" DefaultSelectedItemName="Basic"/>
</TabItem>
```
2. Run `OpenSilverSample.Browser` to test

## 📋 Existing Samples
- **Basic Controls**: Button, ToggleButton, Slider, CheckBox, RadioButton
- **Data Controls**: ComboBox, ListBox, TextBox, ProgressBar
- **Layout**: Grid, StackPanel, WrapPanel, Border
- **Advanced Components**: AnimatedNavigationBar, AdaptiveColumnsPanel, StaggeredPanel
- **Patterns**: MVVM, Binding, DataContext, Resource

## 🤝 Contributing
1. Fork the repository
2. Clone locally and install templates
3. Develop new samples
4. Test thoroughly and submit Pull Request

## 🔗 Links
- [OpenSilver Official Site](https://opensilver.net/)
- [OpenSilver GitHub](https://github.com/opensilver/opensilver)
- [Community Forums](https://forums.opensilver.net/)
