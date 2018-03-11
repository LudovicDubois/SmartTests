using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using EnvDTE;

using Microsoft.VisualStudio.Shell;

using TextSelection = EnvDTE.TextSelection;



namespace SmartTestsExtension
{
    /// <summary>
    ///     Interaction logic for SmartTestsWindowControl.
    /// </summary>
    public partial class SmartTestsWindowControl: UserControl
    {
        static SmartTestsWindowControl()
        {
            _Dte = (DTE)ServiceProvider.GlobalProvider.GetService( typeof(DTE) );

            _SolutionEvents = _Dte.Events.SolutionEvents;
            _SolutionEvents.Opened += () => AnalyzerResults.Instance.SetSolution( _Dte.Solution );
            _SolutionEvents.AfterClosing += AnalyzerResults.Instance.Clear;
            _SolutionEvents.ProjectAdded += AnalyzerResults.Instance.AddProject;
            _SolutionEvents.ProjectRenamed += AnalyzerResults.Instance.RenameProject;
            _SolutionEvents.ProjectRemoved += AnalyzerResults.Instance.RemoveProject;
        }


        private static readonly DTE _Dte;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private static readonly SolutionEvents _SolutionEvents;


        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestsWindowControl" /> class.
        /// </summary>
        public SmartTestsWindowControl()
        {
            this.InitializeComponent();
        }


        private void Hyperlink_OnClick( object sender, RoutedEventArgs e )
        {
            var row = (DataRowView)( (Hyperlink)e.OriginalSource ).DataContext;

            var window = _Dte.ItemOperations.OpenFile( row[ "TestFileName" ].ToString() );
            window.Activate();
            ( (TextSelection)window.Document.Selection ).GotoLine( int.Parse( row[ "TestLine" ].ToString() ) );
        }
    }
}