namespace Blorc.OpenIdConnect.Tests
{
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using ApprovalTests;
    using ApprovalTests.Namers;
    using NUnit.Framework;
    using PublicApiGenerator;

    [TestFixture]
    public class PublicApiFacts
    {
        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public void Blorc_OpenIdConnect_HasNoBreakingChanges()
        {
            var assembly = typeof(UserManager).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }
    }

    internal static class PublicApiApprover
    {
        public static void ApprovePublicApi(Assembly assembly)
        {
            var publicApi = assembly.GeneratePublicApi();
            var writer = new ApprovalTextWriter(publicApi, "cs");
            var approvalNamer = new AssemblyPathNamer(assembly.Location);
            Approvals.Verify(writer, approvalNamer, Approvals.GetReporter());
        }
    }

    internal class AssemblyPathNamer : UnitTestFrameworkNamer
    {
        private readonly string _name;

        public AssemblyPathNamer(string assemblyPath)
        {
            _name = Path.GetFileNameWithoutExtension(assemblyPath);
        }

        public override string Name
        {
            get { return _name; }
        }
    }
}
