namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides criterions for notifying clients that a property value has changed
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class NotifyPropertyChanged: Criteria
    {
        /// <summary>
        ///     When there are no subscriber to <c>PropertyChanged</c> event
        /// </summary>
        public static readonly Criteria HasNoSubscriber = new NotifyPropertyChanged();
        /// <summary>
        ///     When there are subscribers to <c>PropertyChanged</c> event, but the new property value is the same as the original
        ///     property value
        /// </summary>
        public static readonly Criteria HasSubscriberSameValue = new NotifyPropertyChanged();
        /// <summary>
        ///     When there are subscribers to <c>PropertyChanged</c> event and the new property value is different than the
        ///     original property value
        /// </summary>
        public static readonly Criteria HasSubscriberOtherValue = new NotifyPropertyChanged();
    }
}