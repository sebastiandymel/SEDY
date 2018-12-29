#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Steps;
using FluentNHibernate.Conventions;

#endregion

namespace MvcMusicStore.NH
{
    /// <summary>
    ///   The main auto-mapping class for FNH.
    /// </summary>
    public class AppAutomappingCfg : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            if (type.ContainsGenericParameters)
                return false;

            if (type.IsEnum || type.IsNested || type.IsInterface /*|| type.IsAbstract*/)
                return false;

            if (!IsEntity(type) && !IsTuple(type))
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
                //if (property.GetSetMethod(true) == null || property.Has<NotMappedAttribute>(false))
                //{
                //    Debug.WriteLine("False Map type:'{0}'->'{1}'", member.DeclaringType.Name, member.Name);
                //    return false;
                //}
            }

            Debug.WriteLine("Map type:'{0}'->'{1}'", member.DeclaringType.Name, member.Name);
            return true;
        }

        public override bool IsId(Member member)
        {
            //if (member.MemberInfo.Has<IdAttribute>())
            //    return true;
            return member.Name == "Id";
        }


        public override bool IsComponent(Type type)
        {
            if (IsTuple(type))
            {
                if (IsEntity(type))
                    throw new InvalidOperationException("The type " + type.FullName +
                                                        " is mapped as an entity and marked as tuple. Choose one.");
                return true;
            }

            return false;
        }

        private static bool IsTuple(Type type)
        {
            return false;
        }

        public static bool IsEntity(Type type)
        {
            return type.FullName.StartsWith("MvcMusicStore.Models.Entities");
        }

        internal static IEnumerable<Type> GetAllInterfaces(Type t)
        {
            var result = new List<Type>(t.GetInterfaces());

            for (var i = 0; i < result.Count; i++)
            {
                // add all not already contained at the end of the list
                result.AddRange(result[i].GetInterfaces().Except(result));
            }

            return result;
        }

        public override Type GetParentSideForManyToMany(Type left, Type right)
        {
            return base.GetParentSideForManyToMany(left, right);
        }

        public override string SimpleTypeCollectionValueColumn(Member member)
        {
            return base.SimpleTypeCollectionValueColumn(member);
        }

        public override bool IsConcreteBaseType(Type type)
        {
            return true;
        }

        public override bool AbstractClassIsLayerSupertype(Type type)
        {
            return !IsEntity(type)
                //|| type == typeof(ImageDTOBase)
                   || type.IsAbstract
                ;
        }

        public override bool IsDiscriminated(Type type)
        {
            return false;
            //return type.GetCustomAttributes(typeof(DiscriminatedWithAttribute), true).Any();
        }


        public override string GetComponentColumnPrefix(Member member)
        {
            return member.Name + "_";
        }


    }
}