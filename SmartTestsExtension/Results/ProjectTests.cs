using System.ComponentModel;
using System.Runtime.CompilerServices;

using EnvDTE;

using JetBrains.Annotations;

using SmartTestsAnalyzer;



namespace SmartTestsExtension.Results
{
    public class ProjectTests: INotifyPropertyChanged
    {
        public ProjectTests( Project project, Tests tests )
        {
            Project = project;
            Tests = tests;
        }


        public Project Project { get; }

        public string ProjectName => Project.Name;
        public string ProjectFullName => Project.FullName;


        internal void ProjectRenamed()
        {
            RaisePropertyChanged( nameof(ProjectName) );
            RaisePropertyChanged( nameof(ProjectFullName) );
        }


        #region Tests Property

        private Tests _Tests;

        public Tests Tests
        {
            get => _Tests;
            set
            {
                _Tests = value;
                RaisePropertyChanged();
                if( _TestsResult == null )
                    TestsResult = new TestsResult( _Tests );
                else
                    TestsResult.Synchronize( _Tests );
            }
        }

        #endregion


        #region TestResults Property

        private TestsResult _TestsResult;

        public TestsResult TestsResult
        {
            get => _TestsResult;
            set
            {
                if( value == _TestsResult )
                    return;
                _TestsResult = value;
                RaisePropertyChanged();
            }
        }

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged( [CallerMemberName] string propertyName = null ) => PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
}