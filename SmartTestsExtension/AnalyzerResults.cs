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
            return null;
        }


        public void AddProject( Project project )
        {
            var settings = GetSettings( project );
            if( settings == null ||
                !settings.IsEnabled )
                return;

            // We have SmartTests to show
            var path = Path.Combine( Path.GetDirectoryName( project.FullName ), settings.File );
            AddOrUpdate( project, path );
        }


        private void AddOrUpdate( Project project, string testsPath )
        {
            var tests = JsonConvert.DeserializeObject<Tests>( File.ReadAllText( testsPath ) );

            var projectTests = GetTests( project );
            if( projectTests != null )
                projectTests.Tests = tests;
            else
                TestedProjects.Add( new ProjectTests( project, tests ) );
        }


        public void RenameProject( Project project, string oldname ) => GetTests( project )?.ProjectRenamed();


        public void RemoveProject( Project project ) => TestedProjects.Remove( GetTests( project ) );
    }
}