using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
    public partial class SmartTestsWindowControl
    {
        static SmartTestsWindowControl()
        {
            try
            {
                _Dte = (DTE)ServiceProvider.GlobalProvider.GetService( typeof(DTE) );

                _SolutionEvents = _Dte.Events.SolutionEvents;
                _SolutionEvents.Opened += () => AnalyzerResults.Instance.SetSolution( _Dte.Solution );
                _SolutionEvents.AfterClosing += AnalyzerResults.Instance.Clear;
                _SolutionEvents.ProjectAdded += AnalyzerResults.Instance.AddProject;
                _SolutionEvents.ProjectRenamed += AnalyzerResults.Instance.RenameProject;
                _SolutionEvents.ProjectRemoved += AnalyzerResults.Instance.RemoveProject;
                AnalyzerResults.Instance.SetSolution( _Dte.Solution ); // Force detection of current solution
            }
            catch( Exception e )
            {
                Trace.TraceError( e.Message + Environment.NewLine + e.StackTrace );
                throw;
            }
        }


        private static readonly DTE _Dte;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private static readonly SolutionEvents _SolutionEvents;


        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestsWindowControl" /> class.
        /// </summary>
        public SmartTestsWindowControl()
        {
            InitializeComponent();
        }


        private void Hyperlink_OnClick( object sender, RoutedEventArgs e )
        {
            var row = (DataRowView)( (Hyperlink)e.OriginalSource ).DataContext;

            var window = _Dte.ItemOperations.OpenFile( row[ "TestFileName" ].ToString() );
            window.Activate();
            ( (TextSelection)window.Document.Selection ).GotoLine( int.Parse( row[ "TestLine" ].ToString() ) );
        }


        private void SmartTestsWindowControl_OnLoaded( object sender, RoutedEventArgs e )
        {
            AnalyzerResults.Instance.Pause = false;
        }


        private void SmartTestsWindowControl_OnUnloaded( object sender, RoutedEventArgs e )
        {
            AnalyzerResults.Instance.Pause = true;
        }


        private static readonly HashSet<string> _HiddenColumns = new HashSet<string>( new[] { "Test", "TestFileName", "TestLine", "TestLocation", "HasError", "IsMissing" } );


        private void DataGrid_OnAutoGeneratingColumn( object sender, DataGridAutoGeneratingColumnEventArgs e )
        {
            if( _HiddenColumns.Contains( e.Column.Header.ToString() ) )
                e.Cancel = true;
        }
    }
}