using System.Runtime.InteropServices;

using Microsoft.VisualStudio.Shell;



namespace SmartTestsExtension
{
    /// <summary>
    ///     This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    ///     In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    ///     usually implemented by the package implementer.
    ///     <para>
    ///         This class derives from the ToolWindowPane class provided from the MPF in order to use its
    ///         implementation of the IVsUIElementPane interface.
    ///     </para>
    /// </remarks>
    [Guid( "1a453ed3-cd6c-4731-86d9-685ac32f60a0" )]
    public class SmartTestsWindow: ToolWindowPane
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestsWindow" /> class.
        /// </summary>
        public SmartTestsWindow()
            : base( null )
        {
            Caption = "Smart Tests Window";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            // ReSharper disable once VirtualMemberCallInConstructor
            Content = new SmartTestsWindowControl();
        }
    }
}