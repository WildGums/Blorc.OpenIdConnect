namespace Blorc.OpenIdConnect.Tests
{
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using PublicApiGenerator;

    [TestFixture]
    public class PublicApiFacts
    {
        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Blorc_OpenIdConnect_HasNoBreakingChanges_Async()
        {
            var assembly = typeof(UserManager).Assembly;

            await PublicApiApprover.ApprovePublicApiAsync(assembly);
        }
    }

    internal static class PublicApiApprover
    {
        public static async Task ApprovePublicApiAsync(Assembly assembly)
        {
            var publicApi = assembly.GeneratePublicApi();
            await VerifyNUnit.Verifier.Verify(publicApi);
        }
    }
}
