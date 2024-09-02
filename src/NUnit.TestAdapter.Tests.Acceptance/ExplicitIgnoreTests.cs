using NUnit.Framework;
using NUnit.VisualStudio.TestAdapter.Tests.Acceptance.WorkspaceTools;

namespace NUnit.VisualStudio.TestAdapter.Tests.Acceptance;

public class ExplicitIgnoreTests : CsProjAcceptanceTests
{
    /// <summary>
    /// Related to Issue1197.  Since we use a category filter, the test with Explicit only should be executed, but not the one with Ignore.
    /// https://github.com/nunit/nunit3-vs-adapter/issues/1197.
    /// </summary>
    protected override void AddTestsCs(IsolatedWorkspace workspace)
    {
        workspace.AddFile("Tests.cs", @"
                using NUnit.Framework;

                namespace TestProject1
                {
                    [TestFixture, Category(""MyCustomCategory"")]
                    [Explicit]
                    [Ignore(""Ignoring"")]
                    public class MyTestClass
                    {
                        [Test]
                        public void MyTest()
                        {
                            Assert.Pass();
                        }
                    }
                
                    [TestFixture, Category(""MyCustomCategory"")]
                    [Explicit]
                    public class AnotherTestClass
                    {
                        [Test]
                        public void AnotherTest()
                        {
                            Assert.Pass();
                        }
                    }
                }
            ");
    }

    protected override string Framework => Frameworks.Net80;

    [TestCase("TestCategory=MyCustomCategory", 1, 2)]
    public void Filter_DotNetTest(string filter, int executed, int total)
    {
        var workspace = Build();
        var results = workspace.DotNetTest(filter, true, true, true, TestContext.WriteLine);
        Verify(executed, total, results);
    }
}