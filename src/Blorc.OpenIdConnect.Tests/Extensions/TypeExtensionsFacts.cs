namespace Blorc.OpenIdConnect.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class TypeExtensionsFacts
    {
        [TestFixture]
        public class IsPrimitiveEx_Method
        {
            [TestCase(typeof(string), true)]
            [TestCase(typeof(decimal), true)]
            [TestCase(typeof(bool), true)]
            [TestCase(typeof(float), true)]
            [TestCase(typeof(double), true)]
            [TestCase(typeof(Guid), true)]
            [TestCase(typeof(Uri), true)]
            [TestCase(typeof(TimeSpan), true)]
            [TestCase(typeof(ObjectExtensionsFacts.The_AsClaims_Method.ComplexType), false)]
            [TestCase(typeof(User<Profile>), false)]
            [TestCase(typeof(Profile), false)]
            public void Returns_The_Expected_Value(Type type, bool result)
            {
                Assert.That(type.IsPrimitiveEx(), Is.EqualTo(result));
            }
        }
    }
}
