namespace Example
{
    public class ClassWithMembersToBeObsoletedWithErrorInNextVersion
    {
        public string StringField;

        public string StringProperty { get; set; }

        public void Method()
        {
        }

        void nonPublicMethodShouldNotBeIncluded()
        {
            
        }

        internal void InternalMethodWithNoChangeShouldNotBeIncluded()
        {
            
        }

#pragma warning disable 169
        string nonPublicFieldShouldNotBeIncluded;
#pragma warning restore 169
    }
}