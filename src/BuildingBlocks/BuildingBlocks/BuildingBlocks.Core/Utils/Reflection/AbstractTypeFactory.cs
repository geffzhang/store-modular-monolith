using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Core.Extensions;

namespace BuildingBlocks.Core.Utils.Reflection
{
    //https://cephas.net/blog/2004/01/14/net-reflection-and-multiple-assemblies/
    /// <summary>
    /// Abstract static type factory. With supports of type overriding and sets special factories.
    /// </summary>
    /// <typeparam name="BaseType"></typeparam>
    public static class TypeFactory<BaseType>
    {
        private static readonly List<TypeInfo<BaseType>> _typeInfos = new();

        /// <summary>
        /// All registered type mapping informations within current factory instance
        /// </summary>
        public static IEnumerable<TypeInfo<BaseType>> AllTypeInfos => _typeInfos;

        // False-positive SLint warning disabled.
        // This field really need for every class applied by the template
        public static bool HasOverrides => _typeInfos.Count > 0;

        /// <summary>
        /// Register new type (fluent method)
        /// </summary>
        /// <returns>TypeInfo instance to continue configuration through fluent syntax</returns>
        public static TypeInfo<BaseType> RegisterType<T>() where T : BaseType
        {
            return RegisterType(typeof(T));
        }

        public static TypeInfo<BaseType> RegisterType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var result = _typeInfos.FirstOrDefault(x => x.AllSubclasses.Contains(type));

            if (result == null)
            {
                result = new TypeInfo<BaseType>(type);
                _typeInfos.Add(result);
            }

            return result;
        }

        /// <summary>
        /// Override already registered  type to new
        /// </summary>
        /// <returns>TypeInfo instance to continue configuration through fluent syntax</returns>
        public static TypeInfo<BaseType> OverrideType<OldType, NewType>() where NewType : BaseType
        {
            var oldType = typeof(OldType);
            var newType = typeof(NewType);
            var existTypeInfo = _typeInfos.FirstOrDefault(x => x.Type == oldType);
            var newTypeInfo = new TypeInfo<BaseType>(newType);
            if (existTypeInfo != null)
            {
                _typeInfos.Remove(existTypeInfo);
            }

            _typeInfos.Add(newTypeInfo);
            return newTypeInfo;
        }

        /// <summary>
        /// Create BaseType instance considering type mapping information
        /// </summary>
        /// <returns></returns>
        public static BaseType TryCreateInstance()
        {
            return TryCreateInstance(typeof(BaseType).Name);
        }

        /// <summary>
        /// Create derived from BaseType  specified type instance considering type mapping information
        /// </summary>
        /// <returns></returns>
        public static T TryCreateInstance<T>() where T : BaseType
        {
            return (T)TryCreateInstance(typeof(T).Name);
        }

        public static BaseType TryCreateInstance(string typeName, BaseType defaultObj)
        {
            var result = defaultObj;
            var typeInfo = FindTypeInfoByName(typeName);
            if (typeInfo != null)
            {
                result = TryCreateInstance(typeName);
            }

            return result;
        }


        public static BaseType TryCreateInstance(string typeName)
        {
            BaseType result;
            var typeInfo = FindTypeInfoByName(typeName);
            if (typeInfo != null)
            {
                if (typeInfo.Factory != null)
                {
                    result = typeInfo.Factory();
                }
                else
                {
                    result = (BaseType)Activator.CreateInstance(typeInfo.Type);
                }

                typeInfo.SetupAction?.Invoke(result);
            }
            else
            {
                var baseType = typeof(BaseType);
                if (baseType.IsAbstract)
                {
                    throw new OperationCanceledException(
                        $"A type with {typeName} name is not registered in the AbstractFactory, you cannot create an instance of an abstract class {baseType.Name} because it does not have a complete implementation");
                }

                result = (BaseType)Activator.CreateInstance(typeof(BaseType));
            }


            return result;
        }

        public static TypeInfo<BaseType> FindTypeInfoByName(string typeName)
        {
            //Try find first direct type match from registered types
            var result = _typeInfos.FirstOrDefault(x => x.TypeName.EqualsInvariant(typeName));
            //Then need to find in inheritance chain from registered types
            if (result == null)
            {
                result = _typeInfos.FirstOrDefault(x => x.IsAssignableTo(typeName));
            }

            return result;
        }
    }

    /// <summary>
    /// Helper class contains  type mapping information
    /// </summary>
    public class TypeInfo<BaseType>
    {
        public TypeInfo(Type type)
        {
            Services = new List<object>();
            Type = type;
            TypeName = type.Name;
        }

        public string TypeName { get; private set; }
        public Func<BaseType> Factory { get; private set; }
        public Action<BaseType> SetupAction { get; private set; }
        public Type Type { get; private set; }
        public Type MappedType { get; set; }
        public ICollection<object> Services { get; set; }

        public T GetService<T>()
        {
            return Services.OfType<T>().FirstOrDefault();
        }

        public TypeInfo<BaseType> WithService<T>(T service)
        {
            if (!Services.Contains(service))
            {
                Services.Add(service);
            }

            return this;
        }

        public TypeInfo<BaseType> MapToType<T>()
        {
            MappedType = typeof(T);
            return this;
        }

        public TypeInfo<BaseType> WithFactory(Func<BaseType> factory)
        {
            Factory = factory;
            return this;
        }

        public TypeInfo<BaseType> WithSetupAction(Action<BaseType> setupAction)
        {
            SetupAction = setupAction;
            return this;
        }

        public TypeInfo<BaseType> WithTypeName(string name)
        {
            TypeName = name;
            return this;
        }


        public bool IsAssignableTo(string typeName)
        {
            return TypeExtensions.GetTypeInheritanceChainTo(Type, typeof(BaseType)).Concat(new[] {typeof(BaseType)})
                .Any(t => typeName.EqualsInvariant(t.Name));
        }

        public IEnumerable<Type> AllSubclasses
        {
            get
            {
                return TypeExtensions.GetTypeInheritanceChainTo(Type, typeof(BaseType)).ToArray();
            }
        }
    }
}