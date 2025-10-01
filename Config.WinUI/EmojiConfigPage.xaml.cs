using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ShadowObservableConfig.Args;

namespace Config.WinUI;

/// <summary>
/// Emoji 插件配置页面
/// </summary>
public sealed partial class EmojiConfigPage : Page
{
    /// <summary>
    /// 配置视图模型
    /// </summary>
    public EmojiConfig ViewModel { get; } = EmojiConfig.Load();

    /// <summary>
    /// 渲染模式枚举集合（供下拉框绑定）
    /// </summary>
    public Array RenderModes { get; } = Enum.GetValues(typeof(EmojiRenderMode));

    public EmojiConfigPage()
    {
        this.InitializeComponent();
        ViewModel.ConfigChanged += ViewModel_ConfigChanged;
    }

    private void ViewModel_ConfigChanged(object? sender, ConfigChangedEventArgs e)
    {
        Debug.WriteLine($"配置项 '{e.FullPropertyPath}' 已更改");
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await LoadConfigurationAsync();
    }

    /// <summary>
    /// 加载配置
    /// </summary>
    private async Task LoadConfigurationAsync()
    {
        try
        {
            // 这里应该调用配置加载逻辑
            // 由于 EmojiConfig 是 partial 类，实际的加载逻辑应该在生成的代码中
            await Task.Delay(100); // 模拟异步加载
            UpdateStatus("配置已加载");
        }
        catch (Exception ex)
        {
            UpdateStatus($"加载配置失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 保存配置按钮点击事件
    /// </summary>
    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // 这里应该调用配置保存逻辑
            // 由于 EmojiConfig 是 partial 类，实际的保存逻辑应该在生成的代码中
            await Task.Delay(100); // 模拟异步保存
            UpdateStatus("配置已保存");
        }
        catch (Exception ex)
        {
            UpdateStatus($"保存配置失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 重置为默认按钮点击事件
    /// </summary>
    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // 重置为默认值
            ViewModel.DefaultEmojiSize = 32;
            ViewModel.EnableAutoComplete = true;
            ViewModel.MaxEmojiHistory = 50;
            ViewModel.DefaultSkinTone = "default";
            ViewModel.AnimationSpeed = 1.0;
            ViewModel.RenderMode = EmojiRenderMode.Native;
            
            // 重置嵌套设置
            ViewModel.Settings.NestedValue = "";
            ViewModel.Settings.NestedNumber = 0;
            ViewModel.Settings.NestedBoolean = false;
            
            // 清空收藏表情列表
            ViewModel.FavoriteEmojis.Clear();
            
            // 清空自定义设置列表
            ViewModel.CustomSettings.Clear();
            
            UpdateStatus("已重置为默认值");
        }
        catch (Exception ex)
        {
            UpdateStatus($"重置失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 重新加载按钮点击事件
    /// </summary>
    private async void LoadButton_Click(object sender, RoutedEventArgs e)
    {
        await LoadConfigurationAsync();
    }

    /// <summary>
    /// 更新状态信息
    /// </summary>
    private void UpdateStatus(string message)
    {
        StatusTextBlock.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
    }

    /// <summary>
    /// 默认肤色选择改变事件
    /// </summary>
    private void DefaultSkinToneComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DefaultSkinToneComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            ViewModel.DefaultSkinTone = selectedItem.Tag?.ToString() ?? "default";
        }
    }

    /// <summary>
    /// 添加表情按钮点击事件
    /// </summary>
    private void AddEmojiButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var emojiText = NewEmojiTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(emojiText))
            {
                UpdateStatus("请输入要添加的表情");
                return;
            }

            if (ViewModel.FavoriteEmojis.Contains(emojiText))
            {
                UpdateStatus("该表情已存在于收藏列表中");
                return;
            }

            ViewModel.FavoriteEmojis.Add(emojiText);
            NewEmojiTextBox.Text = "";
            UpdateStatus($"已添加表情: {emojiText}");
        }
        catch (Exception ex)
        {
            UpdateStatus($"添加表情失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 删除表情按钮点击事件
    /// </summary>
    private void RemoveEmojiButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.Tag is string emoji)
            {
                ViewModel.FavoriteEmojis.Remove(emoji);
                UpdateStatus($"已删除表情: {emoji}");
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"删除表情失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 添加自定义设置按钮点击事件
    /// </summary>
    private void AddCustomSettingButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var newSetting = new NestedSettings
            {
                NestedValue = "新设置",
                NestedNumber = 0,
                NestedBoolean = false
            };

            ViewModel.CustomSettings.Add(newSetting);
            UpdateStatus("已添加新的自定义设置");
        }
        catch (Exception ex)
        {
            UpdateStatus($"添加自定义设置失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 删除自定义设置按钮点击事件
    /// </summary>
    private void RemoveCustomSettingButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.Tag is NestedSettings setting)
            {
                ViewModel.CustomSettings.Remove(setting);
                UpdateStatus("已删除自定义设置");
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"删除自定义设置失败: {ex.Message}");
        }
    }
}
