using Ject.Contracts;
using Ject.Contracts.Extensions;
using Ject.Dependencies.Extensions;
using Ject.Dependencies.Generic.Extensions;
using NUnit.Framework;

namespace Ject.RuntimeTests
{
    public class ContractsTests
    {
        [Test]
        public void InstanceDependency()
        {
            ISignableContract contract = new SignableContract();
            contract.Describe(typeof(int)).AsInstance(10);
            contract.Describe(typeof(int)).AsInstance(15).WithIdentifier("testId");

            ISignedContract signedContract = contract.Sign();
            Assert.AreEqual(10, signedContract.Resolve(typeof(int)));
            Assert.AreEqual(15, signedContract.Resolve(typeof(int), "testId"));
        }
        
        [Test]
        public void InstanceDependencyGeneric()
        {
            ISignableContract contract = new SignableContract();
            contract.Describe(10);

            ISignedContract signedContract = contract.Sign();
            Assert.AreEqual(signedContract.Resolve<int>(), 10);
        }
        
        [Test]
        public void LazyDependency()
        {
            ISignableContract contract = new SignableContract();
            contract.Describe(typeof(TestClass)).AsLazy(_ => new TestClass());

            ISignedContract signedContract = contract.Sign();
            Assert.AreEqual(signedContract.Resolve(typeof(TestClass)), signedContract.Resolve(typeof(TestClass)));
        }
        
        [Test]
        public void LazyDependencyGeneric()
        {
            ISignableContract contract = new SignableContract();
            contract.Describe<TestClass>().AsLazy(_ => new TestClass());

            ISignedContract signedContract = contract.Sign();
            Assert.AreEqual(signedContract.Resolve<TestClass>(), signedContract.Resolve<TestClass>());
        }
        
        [Test]
        public void FactoryDependency()
        {
            ISignableContract contract = new SignableContract();
            contract.Describe(typeof(TestClass)).AsFactory(_ => new TestClass());

            ISignedContract signedContract = contract.Sign();
            Assert.AreNotEqual(signedContract.Resolve(typeof(TestClass)), signedContract.Resolve(typeof(TestClass)));
        }
        
        [Test]
        public void FactoryDependencyGeneric()
        {
            ISignableContract contract = new SignableContract();
            contract.Describe<TestClass>().AsFactory(_ => new TestClass());

            ISignedContract signedContract = contract.Sign();
            Assert.AreNotEqual(signedContract.Resolve<TestClass>(), signedContract.Resolve<TestClass>());
        }
        
        private class TestClass { }
    }
}