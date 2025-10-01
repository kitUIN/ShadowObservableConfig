# ✨ ShadowObservableConfig

[![NuGet Version](https://img.shields.io/nuget/v/ShadowObservableConfig.svg)](https://www.nuget.org/packages/ShadowObservableConfig)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%208.0-blue.svg)](https://dotnet.microsoft.com/)

一个专为 WinUI 3 应用程序设计的响应式配置文件管理库，通过源代码生成器自动生成配置类，支持 YAML 格式配置文件，提供完整的 MVVM 数据绑定支持。

## 🌟 特性

- **🚀 源代码生成器**：自动生成配置类，减少样板代码
- **📱 WinUI 3 支持**：专为 WinUI 3 应用程序优化
- **🔄 响应式配置**：支持 `INotifyPropertyChanged` 和 `INotifyCollectionChanged`
- **📄 YAML 支持**：内置 YAML 配置文件支持
- **🎯 类型安全**：编译时类型检查，避免运行时错误
- **🔧 自动保存**：配置更改时自动保存到文件
- **📦 嵌套配置**：支持复杂的嵌套配置结构
- **🎨 数据绑定**：完美支持 WinUI 3 数据绑定

## 📦 安装

### YAML 支持
```xml
<PackageReference Include="ShadowObservableConfig.Yaml" Version="0.5.3" />
```

## 🚀 快速开始

### 1. 创建配置类

```csharp
using ShadowObservableConfig.Attributes;
using System.Collections.ObjectModel;

[ObservableConfig(FileName = "app_config", DirPath = "config", Description = "应用程序配置", Version = "1.0.0")]
public partial class AppConfig
{
    [ObservableConfigProperty(Name = "AppName", Description = "应用程序名称")]
    private string _appName = "My App";

    [ObservableConfigProperty(Name = "Version", Description = "应用程序版本")]
    private string _version = "1.0.0";

    [ObservableConfigProperty(Name = "IsEnabled", Description = "是否启用")]
    private bool _isEnabled = true;

    [ObservableConfigProperty(Name = "MaxRetryCount", Description = "最大重试次数")]
    private int _maxRetryCount = 3;

    [ObservableConfigProperty(Name = "Settings", Description = "应用设置")]
    private AppSettings _settings = new();

    [ObservableConfigProperty(Name = "Features", Description = "功能列表")]
    private ObservableCollection<string> _features = new();
}

[ObservableConfig(Description = "应用设置", Version = "1.0.0")]
public partial class AppSettings
{
    [ObservableConfigProperty(Name = "Theme", Description = "主题")]
    private string _theme = "Light";

    [ObservableConfigProperty(Name = "Language", Description = "语言")]
    private string _language = "zh-CN";
}
```

### 2. 在 WinUI 3 中使用

```csharp
// App.xaml.cs
public App()
{
    ShadowConfigGlobalSetting.Init(new ShadowYamlConfigSetting());
    InitializeComponent();
}
```


```csharp
public sealed partial class MainPage : Page
{
    public AppConfig ViewModel { get; } = AppConfig.Load();

    public MainPage()
    {
        this.InitializeComponent();
        ViewModel.ConfigChanged += OnConfigChanged;
    }

    private void OnConfigChanged(object? sender, ConfigChangedEventArgs e)
    {
        Debug.WriteLine($"配置项 '{e.FullPropertyPath}' 已更改: {e.OldValue} -> {e.NewValue}");
    }
}
```

### 3. XAML 数据绑定

```xml
<Page x:Class="MyApp.MainPage">
    <StackPanel>
        <TextBox Header="应用程序名称" 
                 Text="{x:Bind ViewModel.AppName, Mode=TwoWay}" />
        
        <CheckBox Content="启用应用程序" 
                  IsChecked="{x:Bind ViewModel.IsEnabled, Mode=TwoWay}" />
        
        <NumberBox Header="最大重试次数" 
                   Value="{x:Bind ViewModel.MaxRetryCount, Mode=TwoWay}" />
        
        <ComboBox Header="主题" 
                  SelectedItem="{x:Bind ViewModel.Settings.Theme, Mode=TwoWay}">
            <ComboBoxItem Content="Light" />
            <ComboBoxItem Content="Dark" />
        </ComboBox>
    </StackPanel>
</Page>
```

## 📚 详细文档

### 属性说明

#### ObservableConfigAttribute
- `FileName`: 配置文件名（不含扩展名）不填该项说明当前类是内部类
- `DirPath`: 配置文件目录（默认为 "config"）
- `Description`: 配置描述
- `Version`: 配置版本

#### ObservableConfigPropertyAttribute
- `Name`: 属性在配置文件中的名称
- `Description`: 属性描述

### 支持的数据类型

- 基本类型：`string`, `int`, `double`, `bool`, `DateTime`
- 枚举类型：任何 `enum` 类型
- 集合类型：`ObservableCollection<T>`, `List<T>`
- 嵌套对象：其他标记了 `[ObservableConfig]` 的类

### 自动生成的方法

源代码生成器会自动为每个配置类生成：

- 公共属性访问器
- `Load()` 静态方法
- `Save()` 方法
- `AfterConfigInit()` 部分方法（可重写）

## 🔧 高级用法

### 自定义配置加载器

```csharp
public class CustomConfigLoader : IConfigLoader
{
    public T Load<T>(string filePath) where T : class
    {
        // 自定义加载逻辑
        return JsonSerializer.Deserialize<T>(File.ReadAllText(filePath));
    }

    public void Save<T>(T config, string filePath) where T : class
    {
        // 自定义保存逻辑
        File.WriteAllText(filePath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
    }
}
```

### 配置初始化回调

```csharp
[ObservableConfig(FileName = "my_config")]
public partial class MyConfig
{
    [ObservableConfigProperty(Name = "Value")]
    private string _value = "default";

    partial void AfterConfigInit()
    {
        // 配置加载完成后的初始化逻辑
        Console.WriteLine($"配置已加载: {Value}");
    }
}
```

## 🏗️ 项目结构

```
ShadowObservableConfig/
├── ShadowObservableConfig/              # 核心库
│   ├── BaseConfig.cs                    # 基础配置类
│   ├── Attributes/                      # 属性定义
│   └── Args/                           # 事件参数
├── ShadowObservableConfig.SourceGenerator/  # 源代码生成器
│   └── Generators/                     # 生成器实现
├── ShadowObservableConfig.Yaml/        # YAML 支持扩展
└── Config.WinUI/                       # WinUI 3 示例应用
```

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🙏 致谢

- [YamlDotNet](https://github.com/aaubry/YamlDotNet) - YAML 序列化库
- [Microsoft.CodeAnalysis](https://github.com/dotnet/roslyn) - 源代码分析 API
- [WinUI 3](https://github.com/microsoft/microsoft-ui-xaml) - 现代 Windows 应用框架

---

**Made with ❤️ by kitUIN**
