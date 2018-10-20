using CommandLineSyntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace UnitTests
{
    [TestClass]
    public class SmokeTests
    {
        [TestMethod]
        public void SmokeTest_ArgumentParser()
        {
            var testObj = new DynamicTypeBuilder()
                .WithProperty<int>("MyProperty", "=", "--alias1")
                .WithProperty<double>("MyDoubleProp", "=", "anotherAlias")
                .WithProperty<string>("MyStringProp", "=", "--Some-Other-Option")
                .Build();


        }

    }

    // https://stackoverflow.com/questions/3862226/how-to-dynamically-create-a-class-in-c
    public class DynamicTypeBuilder
    {
        private class PropDef
        {
            public string Name { get; set; }
            public Type PropType { get; set; }
            public string[] Aliases { get; set; }
            public string Splitter { get; set; }
        }

        private List<PropDef> allProps = new List<PropDef>();

        public DynamicTypeBuilder WithProperty<T>(string name, string splitter, params string[] aliases)
        {
            allProps.Add(new PropDef
            {
                Name = name,
                PropType = typeof(T),
                Splitter = splitter,
                Aliases = aliases
            });
            return this;
        }

        public object Build()
        {
            var myType = CompileResultType();
            var myObject = Activator.CreateInstance(myType);



            return myObject;
        }

        public Type CompileResultType()
        {
            TypeBuilder tb = GetTypeBuilder();
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            // NOTE: assuming your list contains Field objects with fields FieldName(string) and FieldType(Type)
            foreach (var field in allProps)
                CreateProperty(tb, field);

            Type objectType = tb.CreateType();            

            return objectType;
        }

        private static TypeBuilder GetTypeBuilder()
        {
            var typeSignature = "MyDynamicType";
            var an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);
            return tb;
        }

        private static void CreateProperty(TypeBuilder tb, PropDef definition)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + definition.Name, definition.PropType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = tb.DefineProperty(definition.Name, PropertyAttributes.HasDefault, definition.PropType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + definition.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, definition.PropType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + definition.Name,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { definition.PropType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);

            foreach (var alias in definition.Aliases)
            {
                var attrCtorParams = new Type[] { typeof(string), typeof(string) };
                var attrCtorInfo = typeof(OptionAliasAttribute).GetConstructor(attrCtorParams);
                var attrBuilder = new CustomAttributeBuilder(attrCtorInfo, new object[] { alias, definition.Splitter });
                propertyBuilder.SetCustomAttribute(attrBuilder);
            }
        }
    }
}
