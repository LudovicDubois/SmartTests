using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

using EnvDTE;

using Microsoft.VisualStudio.Shell;

using SmartTestsAnalyzer;

using SmartTestsExtension.Results;

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


        private void SmartTestsWindowControl_OnLoaded( object sender, RoutedEventArgs e )
        {
            AnalyzerResults.Instance.Callback = Reload;
        }


        private void SmartTestsWindowControl_OnUnloaded( object sender, RoutedEventArgs e )
        {
            AnalyzerResults.Instance.Callback = null;
        }


        private DispatcherOperation _CurrentLoad;


        private void Reload( Project project, ProjectTests projectTests, Tests tests )
        {
            _CurrentLoad?.Abort();
            _CurrentLoad = Dispatcher.BeginInvoke( new Action<Project, ProjectTests, Tests>( SafeReload ), project, projectTests, tests );
        }


        private void SafeReload( Project project, ProjectTests projectTests, Tests tests )
        {
            try
            {
                if( projectTests != null )
                    projectTests.Tests = tests;
                else
                    AnalyzerResults.Instance.TestedProjects.Add( new ProjectTests( project, tests ) );
            }
            catch( Exception e )
            {
                Trace.WriteLine( e );
            }
        }


        private static readonly HashSet<string> _HiddenColumns = new HashSet<string>( new[] { "Test", "TestFileName", "TestLine", "TestLocation", "HasError", "IsMissing" } );


        private void DataGrid_OnAutoGeneratingColumn( object sender, DataGridAutoGeneratingColumnEventArgs e )
        {
            if( _HiddenColumns.Contains( e.Column.Header.ToString() ) )
                e.Cancel = true;
        }
    }
}