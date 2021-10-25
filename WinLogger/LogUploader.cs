using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System;

namespace WinLogger
{
    public class LogUploader
    {
        private static TextBlock _logger;
        private static ScrollViewer _scroll;


        public LogUploader(TextBlock logger, ScrollViewer scroll)
        {
            _logger = logger;
            _scroll = scroll;
        }

        public static void LogInformation(string text)
        {
            _logger.Inlines.Add(new Run(text + Environment.NewLine)
            {
                Foreground = Brushes.White
            });
            _scroll.ScrollToBottom();
        }

        public static void LogWarning(string text)
        {
            _logger.Inlines.Add(new Run(text + Environment.NewLine)
            {
                Foreground = Brushes.Yellow
            });
            _scroll.ScrollToBottom();
        }

        public static void LogError(string text)
        {
            _logger.Inlines.Add(new Run(text + Environment.NewLine)
            {
                Foreground = Brushes.Red
            });
            _scroll.ScrollToBottom();
        }

        public static void LogSuccess(string text)
        {
            _logger.Inlines.Add(new Run(text + Environment.NewLine)
            {
                Foreground = Brushes.Green
            });
            _scroll.ScrollToBottom();
        }
    }
}