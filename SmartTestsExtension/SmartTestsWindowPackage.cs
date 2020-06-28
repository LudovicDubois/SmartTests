using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

using EnvDTE;

using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;

using NuGet.VisualStudio;

using VSLangProj;

using VSLangProj140;

using Task = System.Threading.Tasks.Task;



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
    [PackageRegistration( UseManagedResourcesOnly = true, AllowsBackgroundLoading = true )]
    [InstalledProductRegistration( "#110", "#112", "1.0", IconResourceID = 400 )] // Info on this package for Help/About
    [ProvideMenuResource( "Menus.ctmenu", 1 )]
    [ProvideToolWindow( typeof(SmartTestsWindow) )]
    [Guid( PackageGuidString )]
    [SuppressMessage( "StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms" )]
    public sealed class SmartTestsWindowPackage: AsyncPackage
    {
        /// <summary>
        ///     SmartTestsWindowPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "7a04f852-4ba8-4df4-88a7-0fd2975edc58";


        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestsWindowPackage" /> class.
        /// </summary>
        public SmartTestsWindowPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.

            try
            {
                if( (int?)Registry.GetValue( @"HKEY_CURRENT_USER\Software\Pretty Objects\SmartTests", "Trace", 0 ) == 1 )
                {
                    Trace.Listeners.Add( new TextWriterTraceListener( Path.Combine( Path.GetTempPath(), $"SmartTests{DateTime.Now:yyyy-MM-dd-hh-mm-ss}.log" ) ) );
                    Trace.TraceInformation( "Trace Started" );
                }

                var componentModel = (IComponentModel)GetGlobalService( typeof(SComponentModel) );
                _PackageInstaller = componentModel.GetService<IVsPackageInstaller2>();
                _PackageInstallerServices = componentModel.GetService<IVsPackageInstallerServices>();
            }
            catch( Exception e )
            {
                Trace.TraceError( e.Message );
            }
        }


        #region Package Members

        /// <summary>
        ///     The async initialization portion of the package initialization process. This method is invoked from a background
        ///     thread.
        /// </summary>
        /// <param name="cancellationToken">
        ///     A cancellation token to monitor for initialization cancellation, which can occur when
        ///     VS is shutting down.
        /// </param>
        /// <param name="progress">The callback to notify for progress.</param>
        /// <returns>
        ///     A task representing the async work of package initialization, or an already completed task if there is none.
        ///     Do not return null from this method.
        /// </returns>
        protected override async Task InitializeAsync( CancellationToken cancellationToken, IProgress<ServiceProgressData> progress )
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync( cancellationToken );
            await SmartTestsWindowCommand.InitializeAsync( this );

            //var compositionService = (IComponentModel)await GetServiceAsync(typeof(SComponentModel));
            //compositionService.DefaultCompositionService.SatisfyImportsOnce(this);

            var dte = (DTE)await GetServiceAsync( typeof(DTE) );
            var solutionEvents = dte.Events.SolutionEvents;
            solutionEvents.Opened += () => SolutionEventsOnOpened( dte.Solution );
            if( dte.Solution != null )
                SolutionEventsOnOpened( dte.Solution ); // Force detection of current solution
        }


        private void SolutionEventsOnOpened( Solution solution )
        {
            try
            {
                foreach( Project project in solution.Projects )
                {
                    if( project.Globals == null )
                        // ?!?
                        continue;
                    if( project.Globals.VariableExists[ "UseSmartTests" ] )
                        continue;

                    if( !TestProject( project ) )
                        continue;

                    if( !SmartProject( project ) )
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
            catch( Exception e )
            {
                Trace.TraceError( e.Message + e.StackTrace );
            }
        }


        private bool TestProject( Project project )
        {
            if( _PackageInstallerServices.IsPackageInstalled( project, "NUnit" ) )
                return true;
            if( _PackageInstallerServices.IsPackageInstalled( project, "xunit" ) )
                return true;
            if( _PackageInstallerServices.IsPackageInstalled( project, "MSTest.TestFramework" ) )
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


        private bool SmartProject( Project project ) => _PackageInstallerServices.IsPackageInstalled( project, "SmartTests" );


        private bool HasSmartAnalyzer( Project project ) => _PackageInstallerServices.IsPackageInstalled( project, "SmartTests.Analyzer" );


        private readonly IVsPackageInstaller2 _PackageInstaller;
        private readonly IVsPackageInstallerServices _PackageInstallerServices;


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

            _PackageInstaller.InstallLatestPackage( null, project, "SmartTests", false, false );
            return true;
        }


        private void InstallSmartAnalyzer( Project project )
        {
            _PackageInstaller.InstallLatestPackage( null, project, "SmartTests.Analyzer", false, false );
        }

        #endregion
    }
}