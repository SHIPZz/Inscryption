using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Code.Editor
{
    [InitializeOnLoad]
    public static class JsonLogSaver
    {
        private const string LOGS_FOLDER = "Logs";
        private const string JSON_LOG_FILE_NAME = "UnityLogs.json";
        private static readonly List<LogEntry> _logEntries = new List<LogEntry>();
        private static bool _isLoggingEnabled = false;
        private static readonly object _sync = new object();
        private static string _sessionStartTimeString;
        private static bool _pendingFlush = false;
        private static double _nextAutoFlushTime = 0;

        static JsonLogSaver()
        {
            _sessionStartTimeString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            EnableLogging();
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.quitting += OnEditorQuitting;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        [Serializable]
        public class LogEntry
        {
            public string timestamp;
            public string level;
            public string message;
            public string stackTrace;
            public string category;

            public LogEntry(string timestamp, string level, string message, string stackTrace, string category = "")
            {
                this.timestamp = timestamp;
                this.level = level;
                this.message = message;
                this.stackTrace = stackTrace;
                this.category = category;
            }
        }

        [Serializable]
        public class LogData
        {
            public string sessionStartTime;
            public List<LogEntry> entries = new List<LogEntry>();

            public LogData(string sessionStartTime)
            {
                this.sessionStartTime = sessionStartTime;
            }
        }

        public static void EnableLogging()
        {
            if (_isLoggingEnabled)
            {
                Debug.Log("[JsonLogSaver] Logging already enabled");
                return;
            }

            _isLoggingEnabled = true;
            _logEntries.Clear();
            Application.logMessageReceived += OnLogMessageReceived;
            Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;

            Debug.Log("[JsonLogSaver] Logging enabled. All Unity console logs will be saved to JSON.");
        }

        public static void DisableLogging()
        {
            if (!_isLoggingEnabled)
            {
                Debug.Log("[JsonLogSaver] Logging already disabled");
                return;
            }

            _isLoggingEnabled = false;
            Application.logMessageReceived -= OnLogMessageReceived;
            Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;

            SaveLogsToFile();
            Debug.Log("[JsonLogSaver] Logging disabled. Logs saved to JSON file.");
        }

        public static void ClearLogs()
        {
            _logEntries.Clear();
            Debug.Log("[JsonLogSaver] In-memory logs cleared");
        }

        public static bool IsLoggingEnabled()
        {
            return _isLoggingEnabled;
        }

        public static void SaveLogsToFile()
        {
            WriteLogsToFile(true);
        }

        public static LogData LoadLogsFromFile()
        {
            try
            {
                string filePath = Path.Combine(Application.dataPath, "..", LOGS_FOLDER, JSON_LOG_FILE_NAME);

                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[JsonLogSaver] Log file not found: {filePath}");
                    return null;
                }

                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<LogData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonLogSaver] Failed to load logs: {e.Message}");
                return null;
            }
        }

        public static void AnalyzeDrawIssues()
        {
            var logData = LoadLogsFromFile();
            if (logData == null) return;

            Debug.Log("=== ANALYZING DRAW ISSUES ===");

            var drawLogs = logData.entries.Where(e =>
                e.message.Contains("Draw") ||
                e.message.Contains("CheckDraw") ||
                e.message.Contains("ProcessDraw") ||
                e.message.Contains("GameDraw") ||
                e.message.Contains("Both players") ||
                e.message.Contains("creating GameDraw") ||
                e.message.Contains("No cards could be drawn")
            ).ToList();

            Debug.Log($"Found {drawLogs.Count} draw-related log entries:");

            foreach (var log in drawLogs)
            {
                Debug.Log($"{log.timestamp} [{log.level}] {log.message}");
            }

            var gameDrawLogs = drawLogs.Where(e => e.message.Contains("creating GameDraw")).ToList();
            if (gameDrawLogs.Any())
            {
                Debug.Log("\n=== GAME DRAW EVENTS ===");
                foreach (var log in gameDrawLogs)
                {
                    Debug.Log($"DRAW TRIGGERED: {log.timestamp} - {log.message}");

                    var contextLogs = logData.entries.Where(e =>
                        DateTime.Parse(e.timestamp) <= DateTime.Parse(log.timestamp) &&
                        DateTime.Parse(e.timestamp) >= DateTime.Parse(log.timestamp).AddSeconds(-10)
                    ).ToList();

                    Debug.Log("Context (10 seconds before draw):");
                    foreach (var context in contextLogs.Take(10))
                    {
                        Debug.Log($"  {context.timestamp} [{context.level}] {context.message}");
                    }
                }
            }
        }

        public static void ShowLogStats()
        {
            var logData = LoadLogsFromFile();
            if (logData == null) return;

            var stats = new Dictionary<string, int>();
            foreach (var entry in logData.entries)
            {
                if (!stats.ContainsKey(entry.level))
                    stats[entry.level] = 0;
                stats[entry.level]++;
            }

            Debug.Log("=== LOG STATISTICS ===");
            Debug.Log($"Session started: {logData.sessionStartTime}");
            Debug.Log($"Total entries: {logData.entries.Count}");

            foreach (var stat in stats)
            {
                Debug.Log($"{stat.Key}: {stat.Value}");
            }
        }

        private static void OnLogMessageReceived(string message, string stackTrace, LogType type)
        {
            if (!_isLoggingEnabled) return;

            string level = GetLogLevelString(type);
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string category = ExtractCategory(message);

            lock (_sync)
            {
                _logEntries.Add(new LogEntry(timestamp, level, message, stackTrace, category));
            }

            _pendingFlush = true;
            _nextAutoFlushTime = EditorApplication.timeSinceStartup + 0.5f;
        }

        private static void OnLogMessageReceivedThreaded(string message, string stackTrace, LogType type)
        {
            OnLogMessageReceived(message, stackTrace, type);
        }

        private static string GetLogLevelString(LogType type)
        {
            return type switch
            {
                LogType.Log => "INFO",
                LogType.Warning => "WARN",
                LogType.Error => "ERROR",
                LogType.Assert => "ASSERT",
                LogType.Exception => "EXCEPTION",
                _ => "UNKNOWN"
            };
        }

        private static string ExtractCategory(string message)
        {
            if (message.StartsWith("[") && message.Contains("]"))
            {
                int endIndex = message.IndexOf("]");
                if (endIndex > 1)
                {
                    return message.Substring(1, endIndex - 1);
                }
            }
            return "";
        }

        public static string GetLogFilePath()
        {
            return Path.Combine(Application.dataPath, "..", LOGS_FOLDER, JSON_LOG_FILE_NAME);
        }

        private static void OnEditorUpdate()
        {
            if (!_pendingFlush) return;
            if (EditorApplication.timeSinceStartup < _nextAutoFlushTime) return;
            WriteLogsToFile(false);
            _pendingFlush = false;
        }

        private static void OnEditorQuitting()
        {
            WriteLogsToFile(false);
        }

        private static void OnBeforeAssemblyReload()
        {
            WriteLogsToFile(false);
        }

        private static void WriteLogsToFile(bool logToConsole)
        {
            try
            {
                string directoryPath = Path.Combine(Application.dataPath, "..", LOGS_FOLDER);
                Directory.CreateDirectory(directoryPath);

                string filePath = Path.Combine(directoryPath, JSON_LOG_FILE_NAME);

                var existing = LoadLogsFromFile();
                var mergedEntries = new List<LogEntry>();
                var seen = new HashSet<string>();

                if (existing != null && existing.entries != null)
                {
                    foreach (var e in existing.entries)
                    {
                        string key = $"{e.timestamp}|{e.level}|{e.message}|{e.stackTrace}|{e.category}";
                        if (seen.Add(key)) mergedEntries.Add(e);
                    }
                }

                List<LogEntry> snapshot;
                lock (_sync)
                {
                    snapshot = new List<LogEntry>(_logEntries);
                }

                foreach (var e in snapshot)
                {
                    string key = $"{e.timestamp}|{e.level}|{e.message}|{e.stackTrace}|{e.category}";
                    if (seen.Add(key)) mergedEntries.Add(e);
                }

                var logData = new LogData(existing?.sessionStartTime ?? _sessionStartTimeString);
                logData.entries = mergedEntries;

                string json = JsonUtility.ToJson(logData, true);
                File.WriteAllText(filePath, json);

                if (logToConsole)
                {
                    Debug.Log($"[JsonLogSaver] Logs saved to: {filePath} ({logData.entries.Count} entries)");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonLogSaver] Failed to save logs: {e.Message}");
            }
        }
    }

    public static class JsonLogMenu
    {
        [MenuItem("Tools/JSON Logs/Enable Logging")]
        public static void EnableLogging()
        {
            JsonLogSaver.EnableLogging();
        }

        [MenuItem("Tools/JSON Logs/Disable Logging & Save")]
        public static void DisableLogging()
        {
            JsonLogSaver.DisableLogging();
        }

        [MenuItem("Tools/JSON Logs/Clear Current Logs")]
        public static void ClearLogs()
        {
            JsonLogSaver.ClearLogs();
        }

        [MenuItem("Tools/JSON Logs/Save Logs Now")]
        public static void SaveLogsNow()
        {
            JsonLogSaver.SaveLogsToFile();
        }

        [MenuItem("Tools/JSON Logs/Analyze Draw Issues")]
        public static void AnalyzeDrawIssues()
        {
            JsonLogSaver.AnalyzeDrawIssues();
        }

        [MenuItem("Tools/JSON Logs/Show Statistics")]
        public static void ShowStatistics()
        {
            JsonLogSaver.ShowLogStats();
        }

        [MenuItem("Tools/JSON Logs/Open JSON Log File")]
        public static void OpenLogFile()
        {
            var logPath = JsonLogSaver.GetLogFilePath();
            if (File.Exists(logPath))
            {
                System.Diagnostics.Process.Start(logPath);
            }
            else
            {
                Debug.LogError($"JSON log file not found: {logPath}");
            }
        }

    }
}