using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

using EnvDTE;

using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;

using NuGet.VisualStudio;

using VSLangProj;

using VSLangProj140;



namespace SmartTestsExtension
{
    /// <summary>
    ///     This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The minimum requirement for a class to be considered a valid package for Visual Studio
    ///         is to implement the IVsPackage interface and register itself with the shell.
    ///         This package uses the helper classes defined inside the Managed Package Framework (MPF)
    ///         to do it: it derives from the Package class that provides the implementation of the
    ///         IVsPackage interface and uses the registration attributes defined in the framework to
    ///         register itself and its components with the shell. These attributes tell the pkgdef creation
    ///         utility what data to put into .pkgdef file.
    ///     </para>
    ///     <para>
    ///         To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...
    ///         &gt; in .vsixmanifest file.
    ///     </para>
    /// </remarks>
    [PackageRegistration( UseManagedResourcesOnly = true )]
    [InstalledProductRegistration( "#110", "#112", "1.0", IconResourceID = 400 )] // Info on this package for Help/About
    [ProvideMenuResource( "Menus.ctmenu", 1 )]
    [ProvideToolWindow( typeof(SmartTestsWindow) )]
    [Guid( PackageGuidString )]
    [SuppressMessage( "StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms" )]
    public sealed class SmartTestsWindowPackage: Package
    {
        /// <summary>
        ///     SmartTestsWindowPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "7a04f852-4ba8-4df4-88a7-0fd2975edc58";


        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestsWindow" /> class.
        /// </summary>
        public SmartTestsWindowPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
            if( (int?)Registry.GetValue( @"HKEY_CURRENT_USER\Software\Pretty Objects\SmartTests", "Trace", 0 ) == 1 )
            {
                Trace.Listeners.Add( new TextWriterTraceListener( Path.Combine( Path.GetTempPath(), $"SmartTests{DateTime.Now:yyyy-MM-dd-hh-mm-ss}.log" ) ) );
                Trace.TraceInformation( "Trace Started" );
            }
        }


        #region Package Members

        /// <summary>
        ///     Initialization of the package; this method is called right after the package is sited, so this is the place
        ///     where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            try
            {
                SmartTestsWindowCommand.Initialize( this );
                base.Initialize();

                ThreadHelper.ThrowIfNotOnUIThread();
                var dte = (DTE)GetService( typeof(DTE) );

                var compositionService = (IComponentModel)ServiceProvider.GlobalProvider.GetService( typeof(SComponentModel) );
                compositionService.DefaultCompositionService.SatisfyImportsOnce( this );


                var solutionEvents = dte.Events.SolutionEvents;
                solutionEvents.Opened += () => SolutionEventsOnOpened( dte.Solution );
            }
            catch( Exception e )
            {
                Trace.TraceError( e.Message + Environment.NewLine + e.StackTrace );
                throw;
            }
        }


        private void SolutionEventsOnOpened( Solution solution )
        {
            foreach( Project project in solution.Projects )
            {
                if( project.Globals.VariableExists[ "UseSmartTests" ] )
                    continue;

                if( !IsTestProject( project ) )
                    continue;

                if( !IsSmartProject( project ) )
                    // Do not want to use SmartTests
                    if( !InstallSmartTests( project ) )
                    {
                        project.Globals[ "UseSmartTests" ] = "false";
                        project.Globals.VariablePersists[ "UseSmartTests" ] = true;
                        continue;
                    }

                if( !HasSmartAnalyzer( project ) )
                    InstallSmartAnalyzer( project );

                project.Globals[ "UseSmartTests" ] = "true";
                project.Globals.VariablePersists[ "UseSmartTests" ] = true;
            }
        }


        private bool IsTestProject( Project project )
        {
            if( PackageInstallerServices.IsPackageInstalled( project, "NUnit" ) )
                return true;
            if( PackageInstallerServices.IsPackageInstalled( project, "xunit" ) )
                return true;
            if( PackageInstallerServices.IsPackageInstalled( project, "MSTest.TestFramework" ) )
                return true;

            // MSTests or referenced directly?
            if( !( project.Object is VSProject3 vsProject ) )
                return false;

            foreach( Reference reference in vsProject.References )
            {
                switch( reference.Name )
                {
                    case "Microsoft.VisualStudio.TestPlatform.TestFramework":
                    case "nunit.framework":
                    case "xunit.core":
                        return true;
                }
            }

            return false;
        }


        private bool IsSmartProject( Project project ) => PackageInstallerServices.IsPackageInstalled( project, "SmartTests" );


        private bool HasSmartAnalyzer( Project project ) => PackageInstallerServices.IsPackageInstalled( project, "SmartTests.Analyzer" );


        [Import]
        public IVsPackageInstaller2 PackageInstaller;
        [Import]
        public IVsPackageInstallerServices PackageInstallerServices;


        private bool InstallSmartTests( Project project )
        {
            if( MessageBox.Show( $"SmartTests Extension is installed, but SmartTest package is not installed in project '{project.Name}'.\nDo you want to install it now?",
                                 "SmartTests",
                                 MessageBoxButton.YesNo ) == MessageBoxResult.No )
            {
                project.Globals[ "UseSmartTests" ] = "false";
                project.Globals.VariablePersists[ "UseSmartTests" ] = true;
                return false;
            }

            PackageInstaller.InstallLatestPackage( null, project, "SmartTests", false, false );
            return true;
        }


        private void InstallSmartAnalyzer( Project project )
        {
            PackageInstaller.InstallLatestPackage( null, project, "SmartTests.Analyzer", false, false );
        }

        #endregion
    }
}