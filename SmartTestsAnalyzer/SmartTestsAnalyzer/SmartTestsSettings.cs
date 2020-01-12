// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace SmartTestsAnalyzer
{
    internal class SmartTestsSettings
    {
        // ReSharper disable once InconsistentNaming
        public bool IsEnabled { get; set; } = true;
        public string File { get; set; } = @"obj\SmartTestsData.json";

        // ReSharper disable once UnusedMember.Global
        internal string FullPath { get; set; }
    }
}