using System;
using UnityEngine;

namespace Toolkit
{
    [Serializable]
    public struct Identifier
    {
        public static readonly Identifier None = default;
        
        [SerializeField] private string sign;
        [SerializeField] private int hashCode;

        public Identifier(string sign) : this()
        {
            this.sign = sign;
            Merge(sign?.GetHashCode() ?? 0);
        }
        
        public Identifier(string sign, params int[] hashCodes) : this()
        {
            this.sign = sign;
            Merge(hashCodes);
        }

        public Identifier Merged(params int[] hashCodes)
        {
            Merge(hashCodes);
            return this;
        }
        
        public Identifier Merged(int subHashCode)
        {
            Merge(subHashCode);
            return this;
        }
        
        public void Merge(params int[] hashCodes)
        {
            foreach (int subHashCode in hashCodes)
            {
                Merge(subHashCode);
            }
        }

        public void Merge(int subHashCode)
        {
            hashCode = hashCode * 31 + subHashCode;
        }
        
        public override bool Equals(object obj) => obj is Identifier id && id.hashCode == hashCode;
        
        public override string ToString() => sign;
        
        public override int GetHashCode() => hashCode;

        public static bool operator ==(Identifier first, Identifier second) => first.hashCode == second.hashCode;

        public static bool operator !=(Identifier first, Identifier second) => !(first == second);
        
        public static implicit operator int(Identifier identifier) => identifier.hashCode;
    }
}
