using System;
using Toolkit;

namespace Ject.Contracts.Extensions
{
    public static class SignedContractExtensions
    {
        public static T Resolve<T>(this ISignedContract contract) 
            => (T)contract.Resolve(typeof(T));

        public static T Resolve<T>(this ISignedContract contract, string identifier) 
            => (T)contract.Resolve(typeof(T), new Identifier(identifier, identifier.GetHashCode()));

        public static T Resolve<T>(this ISignedContract contract, Identifier identifier) 
            => (T)contract.Resolve(typeof(T), identifier);

        public static object Resolve(this ISignedContract contract, Type type)
            => contract.Resolve(type);
        
        public static object Resolve(this ISignedContract contract, Type type, string identifier)
            => contract.Resolve(type, new Identifier(identifier, identifier.GetHashCode()));
    }
}