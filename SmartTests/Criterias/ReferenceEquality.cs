namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides criterions for two references comparison
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class ReferenceEquality: Criteria
    {
        /// <summary>
        ///     When the left side is <c>null</c> while the right side is not <c>null</c>
        /// </summary>
        // ReSharper disable UnusedMember.Global
        public static readonly Criteria HasNullLeftOperand = new ReferenceEquality();
        /// <summary>
        ///     When the left side is not <c>null</c> while the right side is <c>null</c>
        /// </summary>
        public static readonly Criteria HasNullRightOperand = new ReferenceEquality();
        /// <summary>
        ///     When the left side and the right side are <c>null</c>
        /// </summary>
        public static readonly Criteria HasBothNullOperands = new ReferenceEquality();
        /// <summary>
        ///     When the left side and the right side are not <c>null</c> and are equal (<c>object.Equals</c> return <c>true</c>)
        ///     but not same (<c>object.ReferenceEquals</c> returns <c>false</c>)
        /// </summary>
        public static readonly Criteria HasEqualNotNullOperands = new ReferenceEquality();
        /// <summary>
        ///     When the left side and the right side are not <c>null</c> and are identical (<c>object.ReferenceEquals</c> returns
        ///     <c>true</c>)
        /// </summary>
        public static readonly Criteria HasSameNotNullOperands = new ReferenceEquality();
        // ReSharper restore UnusedMember.Global
    }
}