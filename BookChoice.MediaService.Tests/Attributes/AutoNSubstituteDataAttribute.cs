using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace BookChoice.MediaService.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoNSubstituteDataAttribute : AutoDataAttribute
    {
        public AutoNSubstituteDataAttribute() : base(CreateFixture)
        {
        }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization { ConfigureMembers = true, GenerateDelegates = true });
            return fixture;
        }
    }
}
