using JetBrains.Annotations;



namespace SmartTestsAnalyzer
{
    [UsedImplicitly( ImplicitUseTargetFlags.WithMembers )]
    class SmartTestsSettings
    {
        public bool IsEnabled { get; set; } = true;
        public string File { get; set; } = @"obj\SmartTestsData.json";

        internal string FullPath { get; set; }
    }
}