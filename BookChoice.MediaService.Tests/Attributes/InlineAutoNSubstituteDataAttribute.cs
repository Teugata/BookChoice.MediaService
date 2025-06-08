using AutoFixture.Xunit2;

namespace BookChoice.MediaService.Tests.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class InlineAutoNSubstituteDataAttribute : InlineAutoDataAttribute
{
    public InlineAutoNSubstituteDataAttribute(params object[] objects)
        : base(new AutoNSubstituteDataAttribute(), objects)
    {
    }
}