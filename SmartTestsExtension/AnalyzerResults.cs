using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using EnvDTE;

using JetBrains.Annotations;

using Microsoft.VisualStudio;

using Newtonsoft.Json;

using SmartTestsAnalyzer;

using SmartTestsExtension.Results;



namespace SmartTestsExtension
{
    [UsedImplicitly( ImplicitUseTargetFlags.WithMembers )]
    public class AnalyzerResults
    {
        public static readonly AnalyzerResults Instance = new AnalyzerResults();


        private AnalyzerResults()
        { }


        #region Callback Property

        private Action<Project, ProjectTests, Tests> _Callback;
        private readonly object _CallbackLocker = new object();

        public Action<Project, ProjectTests, Tests> Callback
        {
            get
            {
                lock( _CallbackLocker )
                    return _Callback;
            }
            set
            {
                lock( _CallbackLocker )
                    _Callback = value;
                if( value != null )
                    foreach( var testedProject in TestedProjects )
                        Load( testedProject.Project, GetTestsFile( testedProject.Project ) );
            }
        }

        #endregion


        public ObservableCollection<ProjectTests> TestedProjects { get; } = new ObservableCollection<ProjectTests>();


        private ProjectTests GetTests( Project project ) => TestedProjects.SingleOrDefault( tested => tested.Project == project );


        public void Clear() => TestedProjects.Clear();


        public void SetSolution( Solution solution )
        {
            foreach( Project project in solution.Projects )
                AddProject( project );
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
            var path = GetTestsFile( project );
            if( path != null )
                AddOrUpdate( project, path );
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
            //Load( project, testsPath );
        }


        private string _LastText; // Very bad hack so that we do not update it multiple times (as FileSystemWatcher.Change event is raised multiple times)


        private void Load( Project project, string testsPath )
        {
            if( Callback == null || // Smart Tool Window is not visible
                !File.Exists( testsPath ) )
                return;

            try
            {
                var text = File.ReadAllText( testsPath );
                if( text == _LastText )
                    // Already processed!
                    return;

                _LastText = text;
                Callback.Invoke( project,
                                 GetTests( project ),
                                 JsonConvert.DeserializeObject<Tests>( text ) );
            }
            catch( IOException )
            {
                // Not completely written yet!
            }
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


        public void RenameProject( Project project, string _ ) => GetTests( project )?.ProjectRenamed();


        public void RemoveProject( Project project ) => TestedProjects.Remove( GetTests( project ) );
    }
}