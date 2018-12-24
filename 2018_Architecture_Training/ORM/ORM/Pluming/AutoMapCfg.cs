using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using ORM.Inheritence._02;
using ORM.Pluming.Attributes;

namespace ORM.Pluming
{
    /// <summary>
    ///     The main auto-mapping class for FNH.
    /// </summary>
    public class AppAutomappingCfg : DefaultAutomappingConfiguration
    {

        public override bool ShouldMap(Type type)
        {
            if (type.ContainsGenericParameters)
                return false;

            if (type.IsEnum || type.IsInterface )
                return false;

            if (!IsEntity(type) && !IsValueType(type))
                return false;

            Debug.WriteLine("Mapped type " + type.Name);
            return true;
        }

        public override bool ShouldMap(Member member)
        {
            if (base.ShouldMap(member) == false)
            {
                Debug.WriteLine("False Map type:'{0}'->'{1}'", member.DeclaringType.Name, member.Name);
                return false;
            }

            if (member.IsProperty)
            {
                var property = (PropertyInfo) member.MemberInfo;

                // no setter, no mapping
                if (property.GetSetMethod(true) == null)
                {
                    Debug.WriteLine("False Map type:'{0}'->'{1}'", member.DeclaringType.Name, member.Name);
                    return false;
                }
            }

            Debug.WriteLine("Map type:'{0}'->'{1}'", member.DeclaringType.Name, member.Name);
            return true;
        }

        public override bool IsId(Member member)
        {
            return member.Name == "Id";
        }


        public override bool IsComponent(Type type)
        {
            if (IsValueType(type))
            {
                if (IsEntity(type))
                    throw new InvalidOperationException("The type " + type.FullName +
                                                        " is mapped as an entity and marked as tuple. Choose one.");
                return true;
            }

            return false;
        }

        private static bool IsValueType(Type type)
        {
            return type.Has<ValueTypeAttribute>();
        }

        public static bool IsEntity(Type type)
        {
            return GetAllInterfaces(type)
                    .Any(t => t == typeof(IEntity))
                ;
        }

        internal static IEnumerable<Type> GetAllInterfaces(Type t)
        {
            var result = new List<Type>(t.GetInterfaces());

            for (var i = 0; i < result.Count; i++)
                result.AddRange(result[i].GetInterfaces().Except(result));

            return result;
        }

        public override bool IsDiscriminated(Type type)
        {
            var ret = type.GetCustomAttributes(typeof(DiscriminatedWithAttribute), true).Any();
            return ret;
        }

        public override string GetComponentColumnPrefix(Member member)
        {
            return member.Name + "_";
        }
    }
}