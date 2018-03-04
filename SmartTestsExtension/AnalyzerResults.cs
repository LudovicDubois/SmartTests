using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using EnvDTE;

using JetBrains.Annotations;

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


        public void AddOrUpdate( Project project, string testsPath )
        {
            var tests = JsonConvert.DeserializeObject<Tests>( File.ReadAllText( testsPath ) );

            var projectTests = TestedProjects.SingleOrDefault( tested => tested.Project == project );
            if( projectTests != null )
                projectTests.Tests = tests;
            else
                TestedProjects.Add( new ProjectTests( project, tests ) );
        }
    }
}