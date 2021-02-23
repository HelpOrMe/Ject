using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Ject.Dependencies.Extensions;
using Ject.Injection;
using Toolkit;
using Ject.Usage;
using Ject.Usage.Scene;
using NUnit.Framework;
using Toolkit.Collections;
using UnityEngine;

namespace Ject.RuntimeTests
{
    public class SceneWriterTests
    {
        private const string GoodSongName = "Somebody to love";
        private const string AnotherGoodSongName = "Highway to Hell";

        private static readonly Identifier GoodSongId = new Identifier("Good song");
        private static readonly Identifier AnotherGoodSongId = new Identifier("Another good song");

        private GameObject _holder;
            
        private SceneWriter _sceneWriter;
        private TestMusicComponent _testComponent;
        
        private Dictionary<Component, List<Context>> _componentContexts;
        
        [SetUp]
        public void Setup()
        {
            ContractWriters.RawData.contractWriterTypeNames[GoodSongId] =
                typeof(GoodSongWriter).AssemblyQualifiedName;
            
            ContractWriters.RawData.contractWriterTypeNames[AnotherGoodSongId] =
                typeof(AnotherGoodSongWriter).AssemblyQualifiedName;

            _holder =new GameObject("TestObject");
            _sceneWriter = _holder.AddComponent<SceneWriter>();
            _testComponent = _holder.AddComponent<TestMusicComponent>();

            SetupComponentContexts();
        }

        private void SetupComponentContexts()
        {
            var goodSongWriterContext = new Context
            {
                injectionInfo = new InjectionInfo
                {
                    fieldNames = new [] {nameof(TestMusicComponent.goodSongName)},
                },
                usedContractWriterIds = new[] {GoodSongId}
            };
            
            var anotherGoodSongWriterContext = new Context
            {
                injectionInfo = new InjectionInfo
                {
                    fieldNames = new [] {nameof(TestMusicComponent.anotherGoodSong)},
                    fieldDependencyIds = new SerializableSupportedDictionary<string, Identifier>
                    {
                        [nameof(TestMusicComponent.anotherGoodSong)] = AnotherGoodSongId
                    }
                },
                usedContractWriterIds = new[] {AnotherGoodSongId}
            };
            
            _componentContexts = new Dictionary<Component, List<Context>>
            {
                [_testComponent] = new List<Context> { goodSongWriterContext, anotherGoodSongWriterContext }
            };
        }

        [TearDown]
        public void Teardown()
        {
            ContractWriters.RawData.contractWriterTypeNames.Remove(GoodSongId);
            ContractWriters.RawData.contractWriterTypeNames.Remove(AnotherGoodSongId);
            Object.Destroy(_holder);
        }
        
        [Test]
        public void Sync()
        {
            _sceneWriter.Write(_componentContexts);
            
            Assert.AreEqual(GoodSongName, _testComponent.goodSongName);
            Assert.AreEqual(AnotherGoodSongName, _testComponent.anotherGoodSong);
            
            _testComponent.goodSongName = null;
            _testComponent.anotherGoodSong = null;
        }

        [Test(ExpectedResult = null)]
        public IEnumerator PartialAsync()
        {
            Task task = _sceneWriter.WriteAsync(_componentContexts);
            
            while (!task.IsCompleted)
            {
                yield return null;
            }
 
            if (task.IsFaulted)
            {
                ExceptionDispatchInfo.Capture(task.Exception!).Throw();
            }
            
            Assert.AreEqual(GoodSongName, _testComponent.goodSongName);
            Assert.AreEqual(AnotherGoodSongName, _testComponent.anotherGoodSong);
            
            _testComponent.goodSongName = null;
            _testComponent.anotherGoodSong = null;
        }
        
        private class GoodSongWriter : ContractWriter
        {
            protected override void Write()
            {
                Contract.Describe(GoodSongName);
            }
        }
        
        private class AnotherGoodSongWriter : ContractWriter
        {
            protected override void Write()
            {
                Contract.Describe(AnotherGoodSongName, AnotherGoodSongId);
            }
        }
        
        private class TestMusicComponent : MonoBehaviour
        {
            public string goodSongName;
            public string anotherGoodSong;
        }
    }
}