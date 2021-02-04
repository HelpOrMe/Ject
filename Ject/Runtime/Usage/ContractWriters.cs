using System;
using System.Collections.Generic;
using System.IO;
using Ject.Contracts;
using Ject.Preferences;
using Toolkit;
using UnityEditor;
using UnityEngine;

namespace Ject.Usage
{
    [InitializeOnLoad]
    public static class ContractWriters
    {
        public static ContractWritersRawData RawData => DataAsset.rawData;
        public static ContractWritersSettings Settings => DataAsset.settings;
        
        public static readonly Dictionary<Identifier, ISignedContract> CachedContracts;
        private static readonly ContractWritersDataAsset DataAsset;
        
        public static IEnumerable<Identifier> ContractIds => RawData.contractWriterTypeNames.Keys;

        static ContractWriters()
        {
            DataAsset = LoadWritersData(PreferencesManager.Preferences.resourcesPath + "/ContractWritersData.asset");
            CachedContracts = new Dictionary<Identifier, ISignedContract>();
        }

        private static ContractWritersDataAsset LoadWritersData(string path)
        {
            var data = AssetDatabase.LoadAssetAtPath<ContractWritersDataAsset>(path);
            if (data == null) 
                throw new FileLoadException("Invalid contract writers data path " + path);
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