using System;
using System.IO;
using System.Windows.Controls;

using EnvDTE;

using Microsoft.VisualStudio;

using Newtonsoft.Json;

using SmartTestsAnalyzer;



namespace SmartTestsExtension
{
    /// <summary>
    ///     Interaction logic for SmartTestsWindowControl.
    /// </summary>
    public partial class SmartTestsWindowControl: UserControl
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestsWindowControl" /> class.
        /// </summary>
        public SmartTestsWindowControl()
        {
            this.InitializeComponent();
        }


        public void SetSolution( Solution solution )
        {
            foreach( Project project in solution.Projects )
            {
                var settings = GetSettings( project );
                if( settings == null ||
                    !settings.IsEnabled )
                    continue;

                // We have SmartTests to show
                var path = Path.Combine( Path.GetDirectoryName( project.FullName ), settings.File );
                AnalyzerResults.Instance.AddOrUpdate( project, path );
            }
        }


        private SmartTestsSettings GetSettings( Project project )
        {
            foreach( ProjectItem projectItem in project.ProjectItems )
                if( projectItem.Name == "SmartTests.json" &&
                    new Guid( projectItem.Kind ) == VSConstants.ItemTypeGuid.PhysicalFile_guid )
                    return JsonConvert.DeserializeObject<SmartTestsSettings>( File.ReadAllText( projectItem.FileNames[ 0 ] ) );
            return null;
        }
    }
}