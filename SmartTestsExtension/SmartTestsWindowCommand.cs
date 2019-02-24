﻿using System;
using System.ComponentModel.Design;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
using Task = System.Threading.Tasks.Task;



namespace SmartTestsExtension
{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed class SmartTestsWindowCommand
    {
        /// <summary>
        ///     Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        ///     Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid( "475d557f-3b41-4561-9053-1978c5e942d5" );

        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;


        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestsWindowCommand" /> class.
        ///     Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private SmartTestsWindowCommand( AsyncPackage package, OleMenuCommandService commandService )
        {
            this.package = package ?? throw new ArgumentNullException( nameof(package) );
            commandService = commandService ?? throw new ArgumentNullException( nameof(commandService) );

            var menuCommandID = new CommandID( CommandSet, CommandId );
            var menuItem = new MenuCommand( ShowToolWindow, menuCommandID );
            commandService.AddCommand( menuItem );
        }


        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        public static SmartTestsWindowCommand Instance { get; private set; }

        /// <summary>
        ///     Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider ServiceProvider
        {
            get { return package; }
        }


        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync( AsyncPackage package )
        {
            // Switch to the main thread - the call to AddCommand in SmartTestsWindowCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync( package.DisposalToken );

            OleMenuCommandService commandService = await package.GetServiceAsync( ( typeof(IMenuCommandService) ) ) as OleMenuCommandService;
            Instance = new SmartTestsWindowCommand( package, commandService );
        }


        /// <summary>
        ///     Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void ShowToolWindow( object sender, EventArgs e )
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = package.FindToolWindow( typeof(SmartTestsWindow), 0, true );
            if( ( null == window ) || ( null == window.Frame ) )
            {
                throw new NotSupportedException( "Cannot create tool window" );
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            ErrorHandler.ThrowOnFailure( windowFrame.Show() );
        }
    }
}