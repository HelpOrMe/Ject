using System;
using System.Reflection;

namespace Ject.Injection
{
    public class InjectorBase
    {
        public const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        protected readonly object patient;
        protected readonly Type patientType;

        protected InjectorBase(object patient)
        {
            this.patient = patient;
            patientType = patient.GetType();
        }
        
        protected FieldInfo GetField(string name)
        {
            FieldInfo field = patientType.GetField(name, Flags);
            if (field == null)
                throw new MissingMemberException(MemberNullMessage("field", name));

            return field;
        }
            
        protected PropertyInfo GetProperty(string name)
        {
            PropertyInfo property = patientType.GetProperty(name, Flags);
            if (property == null) 
                throw new MissingMemberException(MemberNullMessage("property", name));

            return property;
        }
        
        protected MethodInfo GetMethod(string name)
        {
            MethodInfo method = patientType.GetMethod(name, Flags);
            if (method == null)
                throw new MissingMemberException(MemberNullMessage("method", name));

            return method;
        }
        
        protected string MemberNullMessage(string member, string memberName) 
            => $"Invalid {member} name {patientType.Name}:{memberName}";
    }
}