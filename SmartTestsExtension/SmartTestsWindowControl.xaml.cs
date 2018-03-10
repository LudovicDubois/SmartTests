using System.Windows;
using System.Windows.Controls;

using EnvDTE;

using Microsoft.VisualStudio.Shell;



namespace SmartTestsExtension
{
    /// <summary>
    ///     Interaction logic for SmartTestsWindowControl.
    /// </summary>
    public partial class SmartTestsWindowControl : UserControl
    {
        static SmartTestsWindowControl()
        {
            var dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));

            _SolutionEvents = dte.Events.SolutionEvents;
            _SolutionEvents.Opened += () => AnalyzerResults.Instance.SetSolution(dte.Solution);
            _SolutionEvents.AfterClosing += AnalyzerResults.Instance.Clear;
            _SolutionEvents.ProjectAdded += AnalyzerResults.Instance.AddProject;
            _SolutionEvents.ProjectRenamed += AnalyzerResults.Instance.RenameProject;
            _SolutionEvents.ProjectRemoved += AnalyzerResults.Instance.RemoveProject;
        }


        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private static readonly SolutionEvents _SolutionEvents;


        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestsWindowControl" /> class.
        /// </summary>
        public SmartTestsWindowControl()
        {
            this.InitializeComponent();
        }
    }
}