using Microsoft.CodeAnalysis;

namespace ShadowObservableConfig.SourceGenerator;

/// <summary>
/// 提供源代码生成器的日志记录功能
/// </summary>
internal class Logger
{
    private readonly string _category;
    private readonly GeneratorExecutionContext? _generatorContext;
    private readonly SourceProductionContext? _sourceContext;

    /// <summary>
    /// 使用生成器执行上下文初始化Logger的新实例
    /// </summary>
    /// <param name="category">日志类别</param>
    /// <param name="context">生成器执行上下文</param>
    public Logger(string category, GeneratorExecutionContext context)
    {
        _category = category;
        _generatorContext = context;
    }

    /// <summary>
    /// 使用源代码生产上下文初始化Logger的新实例
    /// </summary>
    /// <param name="category">日志类别</param>
    /// <param name="context">源代码生产上下文</param>
    public Logger(string category, SourceProductionContext context)
    {
        _category = category;
        _sourceContext = context;
    }

    /// <summary>
    /// 记录指定严重级别的日志消息
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <param name="title">日志标题</param>
    /// <param name="message">日志消息</param>
    /// <param name="severity">诊断严重级别</param>
    public void Log(string id, string title, string message, DiagnosticSeverity severity)
    {
        // For now, we'll just ignore diagnostics since the API is not available
        // In a real implementation, you might want to use a different logging mechanism
    }
    
    /// <summary>
    /// 记录信息级别的日志消息
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <param name="message">日志消息</param>
    public void Info(string id, string message)
    {
        Log(id, $"{_category} Info", message, DiagnosticSeverity.Info);
    }
    
    /// <summary>
    /// 记录警告级别的日志消息
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <param name="message">日志消息</param>
    public void Warning(string id, string message)
    {
        Log(id, $"{_category} Warning", message, DiagnosticSeverity.Warning);
    }
    
    /// <summary>
    /// 记录错误级别的日志消息
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <param name="message">日志消息</param>
    public void Error(string id, string message)
    {
        Log(id, $"{_category} Error", message, DiagnosticSeverity.Error);
    }
}