using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    [PublicAPI]
    public class NotifyPropertyChanged: Criteria
    {
        public static readonly Criteria HasNoSubscriber = new NotifyPropertyChanged();
        public static readonly Criteria HasSubscriberSameValue = new NotifyPropertyChanged();
        public static readonly Criteria HasSubscriberOtherValue = new NotifyPropertyChanged();
    }
}