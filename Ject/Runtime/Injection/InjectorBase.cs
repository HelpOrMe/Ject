using System;
using System.Reflection;

namespace Ject.Injection
{
    public class InjectorBase
    {
        public const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        protected readonly object Patient;
        protected readonly Type PatientType;

        protected InjectorBase(object patient)
        {
            Patient = patient;
            PatientType = patient.GetType();
        }
        
        protected FieldInfo GetField(string name)
        {
            FieldInfo field = PatientType.GetField(name, Flags);
            if (field == null)
                throw new MissingMemberException(MemberNullMessage("field", name));

            return field;
        }
            
        protected PropertyInfo GetProperty(string name)
        {
            PropertyInfo property = PatientType.GetProperty(name, Flags);
            if (property == null) 
                throw new MissingMemberException(MemberNullMessage("property", name));

            return property;
        }
        
        protected MethodInfo GetMethod(string name)
        {
            MethodInfo method = PatientType.GetMethod(name, Flags);
            if (method == null)
                throw new MissingMemberException(MemberNullMessage("method", name));

            return method;
        }
        
        protected string MemberNullMessage(string member, string memberName) 
            => $"Invalid {member} name {PatientType.Name}:{memberName}";
    }
}