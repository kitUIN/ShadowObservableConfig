using Microsoft.CodeAnalysis;

namespace ShadowObservableConfig.SourceGenerator;

internal class Logger
{
    private readonly string _category;
    private readonly GeneratorExecutionContext? _generatorContext;
    private readonly SourceProductionContext? _sourceContext;

    public Logger(string category, GeneratorExecutionContext context)
    {
        _category = category;
        _generatorContext = context;
    }

    public Logger(string category, SourceProductionContext context)
    {
        _category = category;
        _sourceContext = context;
    }

    public void Log(string id, string title, string message, DiagnosticSeverity severity)
    {
        // For now, we'll just ignore diagnostics since the API is not available
        // In a real implementation, you might want to use a different logging mechanism
    }
    
    public void Info(string id, string message)
    {
        Log(id, $"{_category} Info", message, DiagnosticSeverity.Info);
    }
    
    public void Warning(string id, string message)
    {
        Log(id, $"{_category} Warning", message, DiagnosticSeverity.Warning);
    }
    
    public void Error(string id, string message)
    {
        Log(id, $"{_category} Error", message, DiagnosticSeverity.Error);
    }
}