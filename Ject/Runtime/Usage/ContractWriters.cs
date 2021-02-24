using System;
using System.Collections.Generic;
using System.IO;
using Ject.Contracts;
using Toolkit;
using UnityEditor;
using UnityEngine;

namespace Ject.Usage
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class ContractWriters
    {
        public static ContractWritersRawData RawData => Data.rawData;
        public static ContractWritersSettings Settings => Data.settings;
        
        public static readonly Dictionary<Identifier, ISignedContract> CachedContracts;
        private static readonly ContractWritersData Data;
        
        public static IEnumerable<Identifier> ContractIds => RawData.contractWriterTypeNames.Keys;

        static ContractWriters()
        {
            Data = LoadWritersData();
            CachedContracts = new Dictionary<Identifier, ISignedContract>();
        }

        private static ContractWritersData LoadWritersData()
        {
            var data = Resources.Load<ContractWritersData>("Ject/ContractWritersData");
            
            if (data == null)
            {
#if UNITY_EDITOR
                Directory.CreateDirectory("Assets/Resources/Ject");
                data = ScriptableObject.CreateInstance<ContractWritersData>();
                AssetDatabase.CreateAsset(data, "Assets/Resources/Ject/ContractWritersData.asset");
#else
                throw new FileLoadException("Contract writers data doesn't exist");
#endif
            }
            
            return data;
        }

        public static ISignedContract WriteContract(Identifier contractId)
        {
            if (Settings.cachingEnabled && CachedContracts.ContainsKey(contractId))
            {
                return CachedContracts[contractId];
            }
            
            if (!RawData.contractWriterTypeNames.ContainsKey(contractId))
            {
                Debug.LogError("Invalid contract id " + contractId);
                return EmptySignedContract.instance;
            }
            
            ContractWriter contractWriter = NewWriter(RawData.contractWriterTypeNames[contractId]);
            ISignableContract contract = NewContract();
            contractWriter.Write(contract);
            ISignedContract signedContract = contract.Sign();
            
            if (Settings.cachingEnabled)
            {
                CachedContracts[contractId] = signedContract;
            }
            
            return signedContract;
        }

        public static ISignableContract NewContract() => new SignableContract();
        
        public static ContractWriter NewWriter(string writerTypeName)
        {
            var type = Type.GetType(writerTypeName);
            if (type == null)
            {
                throw new TypeLoadException("Invalid contract writer type name " + writerTypeName);
            }
            if (!typeof(ContractWriter).IsAssignableFrom(type))
            {
                throw new TypeLoadException("Invalid contract writer type " + type.FullName);
            }
            
            var contractWriter = Activator.CreateInstance(type) as ContractWriter;
            if (contractWriter == null)
            {
                throw new TypeLoadException(
                    $"Invalid constructor of {type.FullName}." + 
                    " Contract writer's constructor must have no parameters");
            }

            return contractWriter;
        }
    }
}