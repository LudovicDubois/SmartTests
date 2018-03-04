﻿using System.ComponentModel;
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


        #region Tests Property

        private Tests _Tests;

        public Tests Tests
        {
            get => _Tests;
            set
            {
                _Tests = value;
                _TestsResult = new TestsResult( _Tests );
                RaisePropertyChanged();
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