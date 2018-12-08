using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

using EnvDTE;

using Microsoft.VisualStudio;

using Newtonsoft.Json;

using SmartTestsAnalyzer;

using SmartTestsExtension.Results;



namespace SmartTestsExtension
{
    public class AnalyzerResults
    {
        public static readonly AnalyzerResults Instance = new AnalyzerResults();


        private AnalyzerResults()
        {
            Pause = true;
        }


        private bool _Pause;
        public bool Pause
        {
            get => _Pause;
            set
            {
                if( value == _Pause )
                    return;
                _Pause = value;
                foreach( var testedProject in TestedProjects )
                {
                    if( testedProject.IsDirty )
                        Load( testedProject.Project, GetTestsFile( testedProject.Project ) );
                }
            }
        }

        public ObservableCollection<ProjectTests> TestedProjects { get; } = new ObservableCollection<ProjectTests>();


        private ProjectTests GetTests( Project project ) => TestedProjects.SingleOrDefault( tested => tested.Project == project );


        public void Clear()
        {
            try
            {
                TestedProjects.Clear();
            }
            catch( Exception e )
            {
                Trace.TraceError( e.Message + Environment.NewLine + e.StackTrace );
            }
        }


        public void SetSolution( Solution solution )
        {
            try
            {
                Trace.TraceInformation( $"Loading solution '{solution.FullName}'" );
                foreach( Project project in solution.Projects )
                    AddProject( project );
                Trace.TraceInformation( $"Loaded solution '{solution.FullName}'" );
            }
            catch( Exception e )
            {
                Trace.TraceError( e.Message + Environment.NewLine + e.StackTrace );
            }
        }


        private SmartTestsSettings GetSettings( Project project )
        {
            foreach( ProjectItem projectItem in project.ProjectItems )
                if( projectItem.Name == "SmartTests.json" &&
                    new Guid( projectItem.Kind ) == VSConstants.ItemTypeGuid.PhysicalFile_guid )
                    return JsonConvert.DeserializeObject<SmartTestsSettings>( File.ReadAllText( projectItem.FileNames[ 0 ] ) );
            return new SmartTestsSettings();
        }


        public void AddProject( Project project )
        {
            try
            {
                Trace.TraceInformation( $"Adding project '{project.FullName}'" );
                var path = GetTestsFile( project );
                if( path != null )
                    AddOrUpdate( project, path );
                Trace.TraceInformation( $"Added project '{project.FullName}'" );
            }
            catch( Exception e )
            {
                Trace.TraceError( e.Message + Environment.NewLine + e.StackTrace );
            }
        }


        private string GetTestsFile( Project project )
        {
            var settings = GetSettings( project );
            if( !settings.IsEnabled )
                return null;

            // We have SmartTests to show
            return Path.Combine( Path.GetDirectoryName( project.FullName ), settings.File );
        }


        private void AddOrUpdate( Project project, string testsPath )
        {
            Watch( project, testsPath );
            Load( project, testsPath );
        }


        private string _LastText; // Very bad hack so that we do not update it multiple times (as FileSystemWatcher.Change event is raised multiple times)


        private void Load( Project project, string testsPath )
        {
            var projectTests = GetTests( project );
            if ( Pause )
            {
                _LastText = null;
                SafeUpdateProject( projectTests, project, null );
                return;
            }

            Trace.TraceInformation( $"Loading tests '{testsPath}' for project '{project.FullName}'" );
            if( !File.Exists( testsPath ) )
            {
                Trace.TraceInformation( $"No tests file '{testsPath}' for project '{project.FullName}'" );
                return;
            }


            try
            {
                var text = File.ReadAllText( testsPath );
                if( text == _LastText )
                {
                    // Already processed!
                    Trace.TraceInformation( $"Same tests '{testsPath}' for project '{project.FullName}'" );
                    return;
                }

                _LastText = text;
                var tests = JsonConvert.DeserializeObject<Tests>( _LastText );
                SafeUpdateProject( projectTests, project, tests );

                Trace.TraceInformation( $"Loaded tests '{testsPath}' for project '{project.FullName}'" );
            }
            catch( Exception e )
            {
                // Not completely written yet!
                Trace.TraceError( e.Message + Environment.NewLine + e.StackTrace );
            }
        }


        private void SafeUpdateProject( ProjectTests projectTests, Project project, Tests tests )
        {
            if( projectTests == null )
                Application.Current.Dispatcher.Invoke( () => TestedProjects.Add( new ProjectTests( project, tests ) ) );
            else
                Application.Current.Dispatcher.Invoke( () => projectTests.Tests = tests );
        }


        private void Watch( Project project, string testsPath )
        {
            var watcher = new FileSystemWatcher( Path.GetDirectoryName( testsPath ), Path.GetFileName( testsPath ) )
                          {
                              NotifyFilter = NotifyFilters.LastWrite
                          };
            watcher.Changed += ( sender, args ) => Load( project, testsPath );
            watcher.EnableRaisingEvents = true;
        }


        public void RenameProject( Project project, string _ )
        {
            try
            {
                GetTests( project )?.ProjectRenamed();
            }
            catch( Exception e )
            {
                Trace.TraceError( e.Message + Environment.NewLine + e.StackTrace );
            }
        }


        public void RemoveProject( Project project )
        {
            try
            {
                TestedProjects.Remove( GetTests( project ) );
            }
            catch( Exception e )
            {
                Trace.TraceError( e.Message + Environment.NewLine + e.StackTrace );
            }
        }
    }
}