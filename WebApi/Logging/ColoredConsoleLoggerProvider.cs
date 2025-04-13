using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace WebApi.Logging
{
    public sealed class ColoredConsoleLoggerProvider : ILoggerProvider
    {
        private readonly IDisposable? _onChangeToken;
        private ColoredConsoleLoggerConfiguration _currentConfig;
        private readonly ConcurrentDictionary<string, ColoredConsoleLogger> _loggers = new();

        public ColoredConsoleLoggerProvider(IOptionsMonitor<ColoredConsoleLoggerConfiguration> config)
        {
            _currentConfig = config.CurrentValue;
            _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new ColoredConsoleLogger(name, _currentConfig));
        }

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken?.Dispose();
        }
    }

    public class ColoredConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly ColoredConsoleLoggerConfiguration _config;

        public ColoredConsoleLogger(string name, ColoredConsoleLoggerConfiguration config)
        {
            _name = name;
            _config = config;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _config.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var originalColor = Console.ForegroundColor;
            
            try
            {
                Console.ForegroundColor = GetLogLevelColor(logLevel);

                string timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz");
                string message = formatter(state, exception);

                // For HTTP request logs, check if we can apply special formatting
                if (_name == "WebApi.Middlewares.RequestLoggingMiddleware" && message.Contains("[") && message.Contains("]"))
                {
                    WriteHttpRequestLog(message);
                }
                else
                {
                    // Regular log messages
                    Console.WriteLine($"{timestamp} [{logLevel}] {_name}: {message}");
                    
                    if (exception != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(exception.ToString());
                    }
                }
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }

        private void WriteHttpRequestLog(string message)
        {
            // Parse the components from the message
            try
            {
                // Example format: 2025-04-12T02:39:20.000+07:00 [GET] /api/users 200 1234 - 42.123 ms {} - IP: 127.0.0.1 - Mozilla/5.0...
                var originalColor = Console.ForegroundColor;
                
                // Extract timestamp (keep the first part as is)
                int timestampEnd = message.IndexOf("[");
                string timestamp = message.Substring(0, timestampEnd).Trim();
                Console.Write($"{timestamp} ");

                // Extract and colorize HTTP method
                int methodStart = message.IndexOf("[") + 1;
                int methodEnd = message.IndexOf("]", methodStart);
                string method = message.Substring(methodStart, methodEnd - methodStart);
                
                Console.ForegroundColor = GetMethodColor(method);
                Console.Write($"[{method}] ");
                
                // Extract URL (between method and status)
                int urlStart = methodEnd + 2; // Skip "] "
                string remaining = message.Substring(urlStart);
                int nextSpace = remaining.IndexOf(" ");
                if (nextSpace > 0)
                {
                    string url = remaining.Substring(0, nextSpace);
                    Console.ForegroundColor = originalColor;
                    Console.Write($"{url} ");
                    
                    // Extract status code
                    remaining = remaining.Substring(nextSpace + 1);
                    nextSpace = remaining.IndexOf(" ");
                    if (nextSpace > 0)
                    {
                        string statusCode = remaining.Substring(0, nextSpace);
                        if (int.TryParse(statusCode, out int status))
                        {
                            Console.ForegroundColor = GetStatusColor(status);
                            Console.Write($"{status} ");
                            
                            // Write the rest of the message
                            remaining = remaining.Substring(nextSpace + 1);
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine(remaining);
                        }
                        else
                        {
                            // Unable to parse status code, print the rest as is
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine(remaining);
                        }
                    }
                    else
                    {
                        // No space after URL, print the rest as is
                        Console.ForegroundColor = originalColor;
                        Console.WriteLine(remaining);
                    }
                }
                else
                {
                    // Unable to parse properly, print the original message
                    Console.ForegroundColor = originalColor;
                    Console.WriteLine(message);
                }
            }
            catch
            {
                // If there's any error in parsing, just write the original message
                Console.WriteLine(message);
            }
        }

        private static ConsoleColor GetLogLevelColor(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => ConsoleColor.Gray,
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Information => ConsoleColor.Green,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Critical => ConsoleColor.DarkRed,
                _ => ConsoleColor.Gray
            };
        }
        
        private static ConsoleColor GetMethodColor(string method)
        {
            return method.ToUpper() switch
            {
                "GET" => ConsoleColor.Green,
                "POST" => ConsoleColor.Blue,
                "PUT" => ConsoleColor.Yellow,
                "PATCH" => ConsoleColor.Magenta,
                "DELETE" => ConsoleColor.Red,
                _ => ConsoleColor.Gray
            };
        }
        
        private static ConsoleColor GetStatusColor(int statusCode)
        {
            return statusCode switch
            {
                >= 500 => ConsoleColor.Red,
                >= 400 => ConsoleColor.Yellow,
                >= 300 => ConsoleColor.Cyan,
                >= 200 => ConsoleColor.Green,
                _ => ConsoleColor.Gray
            };
        }
    }

    public class ColoredConsoleLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public int EventId { get; set; } = 0;
        public bool ColorEnabled { get; set; } = true;
    }

    // NullScope is used when no scope is needed
    internal class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();
        private NullScope() { }
        public void Dispose() { }
    }

    public static class ColoredConsoleLoggerExtensions
    {
        public static ILoggingBuilder AddColoredConsoleLogger(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, ColoredConsoleLoggerProvider>();
            
            return builder;
        }
        
        public static ILoggingBuilder AddColoredConsoleLogger(this ILoggingBuilder builder, Action<ColoredConsoleLoggerConfiguration> configure)
        {
            builder.AddColoredConsoleLogger();
            builder.Services.Configure(configure);
            
            return builder;
        }
    }
}