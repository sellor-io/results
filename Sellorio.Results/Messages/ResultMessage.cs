using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sellorio.Results.Messages;

public class ResultMessage
{
    public string Text { get; }
    public ResultMessageSeverity Severity { get; }
    public IReadOnlyList<ResultMessagePathItem>? Path { get; }

    internal ResultMessage(string text, ResultMessageSeverity severity, ImmutableArray<ResultMessagePathItem>? path)
    {
        Text = text;
        Severity = severity;
        Path = path;
    }

    /// <remarks>
    /// With Path: Name at row 1 of Organisations is required.
    /// Without Path: "Is required."
    /// The latter would be used when we use field-specific validation display.
    /// </remarks>
    internal string ToDisplay()
    {
        if (Path == null || Path.Count == 0)
        {
            return Text;
        }

        var result = new StringBuilder(Text.Length + Path.Count * 10);
        var firstPathItem = true;

        foreach (var pathItem in Path.Reverse())
        {
            if (pathItem.Type is ResultMessagePathItemType.Indexer && pathItem.Value is int index)
            {
                result.Append(firstPathItem ? $"Row {index} " : $"at row {index} ");
            }
            else
            {
                result.Append(firstPathItem ? $"{pathItem.Value} " : $"of {pathItem.Value} ");
            }
        }

        // message should start with a capital to handle cases where there is no path.
        result.Append(char.ToLower(Text[0]));
        result.Append(Text[1..]);

        return result.ToString();
    }

    public static ResultMessage Critical(string message)
    {
        return new ResultMessage(message, ResultMessageSeverity.Critical, null);
    }

    public static ResultMessage Error(string message)
    {
        return new ResultMessage(message, ResultMessageSeverity.Error, null);
    }

    public static ResultMessage NotFound(string objectName)
    {
        return new ResultMessage($"{objectName} not found.", ResultMessageSeverity.NotFound, null);
    }

    public static ResultMessage Warning(string message)
    {
        return new ResultMessage(message, ResultMessageSeverity.Warning, null);
    }

    public static ResultMessage Information(string message)
    {
        return new ResultMessage(message, ResultMessageSeverity.Information, null);
    }

    public static ResultMessage Critical<TContext>(Expression<Func<TContext, object>> path, string message)
    {
        var messagePath = ExpressionToPathHelper.GetPath(path);
        return new ResultMessage(message, ResultMessageSeverity.Critical, messagePath);
    }

    public static ResultMessage Error<TContext>(Expression<Func<TContext, object>> path, string message)
    {
        var messagePath = ExpressionToPathHelper.GetPath(path);
        return new ResultMessage(message, ResultMessageSeverity.Error, messagePath);
    }

    public static ResultMessage NotFound<TContext>(Expression<Func<TContext, object>> path, string objectName)
    {
        var messagePath = ExpressionToPathHelper.GetPath(path);
        return new ResultMessage($"{objectName} not found.", ResultMessageSeverity.NotFound, messagePath);
    }

    public static ResultMessage Warning<TContext>(Expression<Func<TContext, object>> path, string message)
    {
        var messagePath = ExpressionToPathHelper.GetPath(path);
        return new ResultMessage(message, ResultMessageSeverity.Warning, messagePath);
    }

    public static ResultMessage Information<TContext>(Expression<Func<TContext, object>> path, string message)
    {
        var messagePath = ExpressionToPathHelper.GetPath(path);
        return new ResultMessage(message, ResultMessageSeverity.Information, messagePath);
    }

    public static ResultMessage Critical(IList<ResultMessagePathItem> path, string message)
    {
        return new ResultMessage(message, ResultMessageSeverity.Critical, path.ToImmutableArray());
    }

    public static ResultMessage Error(IList<ResultMessagePathItem> path, string message)
    {
        return new ResultMessage(message, ResultMessageSeverity.Error, path.ToImmutableArray());
    }

    public static ResultMessage NotFound(IList<ResultMessagePathItem> path, string objectName)
    {
        return new ResultMessage($"{objectName} not found.", ResultMessageSeverity.NotFound, path.ToImmutableArray());
    }

    public static ResultMessage Warning(IList<ResultMessagePathItem> path, string message)
    {
        return new ResultMessage(message, ResultMessageSeverity.Warning, path.ToImmutableArray());
    }

    public static ResultMessage Information(IList<ResultMessagePathItem> path, string message)
    {
        return new ResultMessage(message, ResultMessageSeverity.Information, path.ToImmutableArray());
    }
}