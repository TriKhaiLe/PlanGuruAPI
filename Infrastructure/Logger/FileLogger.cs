using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Files
{
    public sealed class FileLogger
    {
        private static readonly Lazy<FileLogger> _instance = new Lazy<FileLogger>(() => new FileLogger());

        private readonly string _logDirectory;
        private readonly object _lock = new object();

        public static FileLogger Instance => _instance.Value;
        private FileLogger()
        {
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(_logDirectory);
        }
        private string GetLogFilePath()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            return Path.Combine(_logDirectory, $"log-{date}.txt");
        }
        public void Log(string message, string level = "INFO")
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            string filePath = GetLogFilePath();

            lock (_lock)
            {
                File.AppendAllText(filePath, logMessage + Environment.NewLine, Encoding.UTF8);
            }
        }
        public void Info(string message) => Log(message, "INFO");
        public void Warning(string message) => Log(message, "WARN");
        public void Error(string message) => Log(message, "ERROR");
    }
}
