using System;
using System.Reflection;



namespace SmartTests.Helpers
{
    public static class ExceptionHelper
    {
        public static Exception NoInvocation( this Exception @this ) => ( @this as TargetInvocationException )?.InnerException.NoInvocation() ?? @this;
    }
}
