using System.Collections.Generic;
using Ject.Contracts;
using Ject.Dependencies;
using Ject.Dependencies.Extensions;
using Ject.Injection;
using Ject.Toolkit;
using NUnit.Framework;

namespace Ject.RuntimeTests
{
    public class InjectorTests
    {
        [Test]
        public void Construct()
        {
            ISignableContract contract = new SignableContract();
            
            contract.Describe(15);
            contract.Describe("hello");
            
            var testClass = Constructor.Construct<TestClassWithConstructor>(contract.Sign());
            
            Assert.AreEqual(15, testClass.Field1);
            Assert.AreEqual("hello", testClass.Field2);
        }

        [Test]
        public void InjectField()
        {
            ISignableContract contract = new SignableContract();
            var id = new Identifier("number");
            
            contract.Describe(15, id);
            contract.Describe("hello");

            var testClass = new TestClassWithFields();
            var injector = new Injector(testClass, contract.Sign());

            injector.InjectField("Field1", id);
            injector.InjectField("Field2");
            
            Assert.AreEqual(15, testClass.Field1);
            Assert.AreEqual("hello", testClass.Field2);
        }
        
        [Test]
        public void InjectProperties()
        {
            ISignableContract contract = new SignableContract();
            var id = new Identifier("number");
            
            contract.Describe(15, id);
            contract.Describe("hello");

            var testClass = new TestClassWithProperties();
            var injector = new Injector(testClass, contract.Sign());
            
            injector.InjectProperty("Property1", id);
            injector.InjectProperty("Property2");
            
            Assert.AreEqual(15, testClass.Property1);
            Assert.AreEqual("hello", testClass.Property2);
        }
        
        [Test]
        public void InjectMethods()
        {
            ISignableContract contract = new SignableContract();
            var id = new Identifier("number");
            
            contract.Describe(15, id);
            contract.Describe("hello");
            
            var testClass = new TestClassWithMethods();
            var injector = new Injector(testClass, contract.Sign());

            var dependencyIds = new Dictionary<string, Identifier>
            {
                ["field1"] = id
            };
            
            injector.InjectMethod("Method", dependencyIds);
            
            Assert.AreEqual(15, testClass.Field1);
            Assert.AreEqual("hello", testClass.Field2);
        }

     
        private class TestClassWithConstructor
        {
            public readonly int Field1;
            public readonly string Field2;

            public TestClassWithConstructor(int field1, string field2)
            {
                Field1 = field1;
                Field2 = field2;
            }
        }
        
        private class TestClassWithFields
        {
            #pragma warning disable 649
            public int Field1;
            public string Field2;
            #pragma warning restore 649
        }
        
        private class TestClassWithProperties
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Property1 { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Property2 { get; set; }
        }
        
        private class TestClassWithMethods
        {
            public int Field1;
            public string Field2;

            // ReSharper disable once UnusedMember.Local
            public void Method(int field1, string field2)
            {
                Field1 = field1;
                Field2 = field2;
            }
        }
    }
}