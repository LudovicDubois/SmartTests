using System.ComponentModel;
using System.Runtime.CompilerServices;

using EnvDTE;

using SmartTestsAnalyzer;



namespace SmartTestsExtension.Results
{
    public class ProjectTests: INotifyPropertyChanged
    {
        public ProjectTests( Project project, Tests tests )
        {
            Project = project;
            Tests = tests;
            IsDirty = tests == null;
        }


        public Project Project { get; }

        public string ProjectName => Project.Name;
        public string ProjectFullName => Project.FullName;


        internal void ProjectRenamed()
        {
            RaisePropertyChanged( nameof(ProjectName) );
            RaisePropertyChanged( nameof(ProjectFullName) );
        }


        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsDirty { get; set; }


        #region Tests Property

        private Tests _Tests;

        public Tests Tests
        {
            // ReSharper disable once UnusedMember.Global
            get => _Tests;
            set
            {
                if( value == null )
                {
                    // Do not update tests, make them dirty only
                    IsDirty = true;
                    return;
                }

                IsDirty = false;
                _Tests = value;
                RaisePropertyChanged();
                if( TestsResult == null )
                    TestsResult = new TestsResult();
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


        private void RaisePropertyChanged( [CallerMemberName] string propertyName = null ) => PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
}