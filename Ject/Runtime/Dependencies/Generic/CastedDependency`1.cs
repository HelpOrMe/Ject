namespace Ject.Dependencies.Generic
{
    public class CastedDependency<T> : Dependency
    {
        public CastedDependency(DependencyTranslateInfo info) : base(info)
        {
            Description.ImplementTypes.Add(typeof(T));
        }

        public CastedDependency<TI> ThatImplements<TI>() where TI : T
        {
            Description.ImplementTypes.Add(typeof(TI));
            return new CastedDependency<TI>(TranslateInfo);
        }
        
        public CastedDependency<T> ThatImplements<TI, TI1>() 
            where TI : T where TI1 : T
        {
            Description.ImplementTypes.AddRange(
                new[] { typeof(TI), typeof(TI1) });
            return this;
        }
        
        public CastedDependency<T> ThatImplements<TI, TI1, TI2>() 
            where TI : T where TI1 : T where TI2 : T
        {
            Description.ImplementTypes.AddRange(
                new[] { typeof(TI), typeof(TI1), typeof(TI2) });
            return this;
        }
        
        public CastedDependency<T> ThatImplements<TI, TI1, TI2, TI3>() 
            where TI : T where TI1 : T where TI2 : T where TI3 : T
        {
            Description.ImplementTypes.AddRange(
                new[] { typeof(TI), typeof(TI1), typeof(TI2), typeof(TI3) });
            return this;
        }
        
        public CastedDependency<T> ThatImplements<TI, TI1, TI2, TI3, TI4>()
            where TI : T where TI1 : T where TI2 : T where TI3 : T where TI4 : T
        {
            Description.ImplementTypes.AddRange(
                new[] { typeof(TI), typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4) });
            return this;
        }
    }
}