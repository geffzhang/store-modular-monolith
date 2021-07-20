using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Quartz.Util;

namespace Common.Core.Utils.Reflection
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetAllInterfacesImplementingOpenGenericInterface(this Type type,
            Type openGenericType)
        {
            var interfaces = type.GetInterfaces();
            return interfaces.Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == openGenericType);
        }

        //https://stackoverflow.com/questions/42245011/get-all-implementations-types-of-a-generic-interface
        public static IEnumerable<Type> GetAllTypesImplementingOpenGenericInterface(this Type openGenericType,
            params Assembly[] assemblies)
        {
            var inputAssemblies = assemblies.Any() ? assemblies : AppDomain.CurrentDomain.GetAssemblies();
            return inputAssemblies.SelectMany(assembly =>
                GetAllTypesImplementingOpenGenericInterface(openGenericType, assembly));
        }

        public static IEnumerable<Type> GetAllTypesImplementingOpenGenericInterface(this Type openGenericType,
            Assembly assembly)
        {
            try
            {
                return GetAllTypesImplementingOpenGenericInterface(openGenericType, assembly.GetTypes());
            }
            catch (ReflectionTypeLoadException)
            {
                //It's expected to not being able to load all assemblies
                return new List<Type>();
            }
        }

        public static IEnumerable<Type> GetAllTypesImplementingOpenGenericInterface(this Type openGenericType,
            IEnumerable<Type> types)
        {
            return from type in types
                from interfaceType in type.GetInterfaces()
                where
                    interfaceType.IsGenericType &&
                    openGenericType.IsAssignableFrom(interfaceType.GetGenericTypeDefinition()) &&
                    type.IsClass && !type.IsAbstract
                select type;
        }

        //https://stackoverflow.com/questions/26733/getting-all-types-that-implement-an-interface
        public static IEnumerable<Type> GetAllTypesImplementingInterface(this Type interfaceType,
            params Assembly[] assemblies)
        {
            var inputAssemblies = assemblies.Any() ? assemblies : AppDomain.CurrentDomain.GetAssemblies();
            return inputAssemblies.SelectMany(assembly => GetAllTypesImplementingInterface(interfaceType, assembly));
        }

        public static IEnumerable<Type> GetAllTypesImplementingInterface<TInterface>(params Assembly[] assemblies)
        {
            var inputAssemblies = assemblies.Any() ? assemblies : AppDomain.CurrentDomain.GetAssemblies();
            return inputAssemblies.SelectMany(GetAllTypesImplementingInterface<TInterface>);
        }

        private static IEnumerable<Type> GetAllTypesImplementingInterface<TInterface>(Assembly assembly = null)
        {
            var inputAssembly = assembly ?? Assembly.GetExecutingAssembly();
            return inputAssembly.GetTypes()
                .Where(type => typeof(TInterface).IsAssignableFrom(type) && !type.IsInterface);
        }

        private static IEnumerable<Type> GetAllTypesImplementingInterface(this Type interfaceType, Assembly assembly)
        {
            return assembly.GetTypes().Where(type => interfaceType.IsAssignableFrom(type) && !type.IsInterface);
        }

        public static IEnumerable<string> GetPropertyNames<T>(params Expression<Func<T, object>>[] propertyExpressions)
        {
            var retVal = new List<string>();
            foreach (var propertyExpression in propertyExpressions)
            {
                retVal.Add(GetPropertyName(propertyExpression));
            }

            return retVal;
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> propertyExpression)
        {
            string retVal = null;
            if (propertyExpression != null)
            {
                var lambda = (LambdaExpression) propertyExpression;
                MemberExpression memberExpression;
                if (lambda.Body is UnaryExpression unaryExpression)
                {
                    memberExpression = (MemberExpression) unaryExpression.Operand;
                }
                else
                {
                    memberExpression = (MemberExpression) lambda.Body;
                }

                retVal = memberExpression.Member.Name;
            }

            return retVal;
        }

        public static PropertyInfo[] FindPropertiesWithAttribute(this Type type, Type attribute)
        {
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties.Where(x => x.GetCustomAttributes(attribute, true).Any()).ToArray();
        }

        //https://riptutorial.com/csharp/example/15938/creating-an-instance-of-a-type
        public static bool IsHaveAttribute(this PropertyInfo propertyInfo, Type attribute)
        {
            return propertyInfo.GetCustomAttributes(attribute, true).Any();
        }

        public static Type[] GetTypeInheritanceChainTo(this Type type, Type toBaseType)
        {
            var retVal = new List<Type>();

            retVal.Add(type);
            var baseType = type.BaseType;
            while (baseType != toBaseType && baseType != typeof(object))
            {
                retVal.Add(baseType);
                baseType = baseType.BaseType;
            }

            return retVal.ToArray();
        }

        public static bool IsDerivativeOf(this Type type, Type typeToCompare)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var retVal = type.BaseType != null;
            if (retVal)
            {
                retVal = type.BaseType == typeToCompare;
            }

            if (!retVal && type.BaseType != null)
            {
                retVal = type.BaseType.IsDerivativeOf(typeToCompare);
            }

            return retVal;
        }

        public static T[] GetFlatObjectsListWithInterface<T>(this object obj, List<T> resultList = null)
        {
            var retVal = new List<T>();

            if (resultList == null)
            {
                resultList = new List<T>();
            }

            //Ignore cycling references
            if (!resultList.Any(x => Object.ReferenceEquals(x, obj)))
            {
                var objectType = obj.GetType();

                if (objectType.GetInterface(typeof(T).Name) != null)
                {
                    retVal.Add((T) obj);
                    resultList.Add((T) obj);
                }

                var properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                var objects = properties.Where(x => x.PropertyType.GetInterface(typeof(T).Name) != null)
                    .Select(x => (T) x.GetValue(obj)).ToList();

                //Recursive call for single properties
                retVal.AddRange(objects.Where(x => x != null)
                    .SelectMany(x => x.GetFlatObjectsListWithInterface<T>(resultList)));

                //Handle collection and arrays
                var collections = properties.Where(p => p.GetIndexParameters().Length == 0)
                    .Select(x => x.GetValue(obj, null))
                    .Where(x => x is IEnumerable && !(x is string))
                    .Cast<IEnumerable>();

                foreach (var collection in collections)
                {
                    foreach (var collectionObject in collection)
                    {
                        if (collectionObject is T)
                        {
                            retVal.AddRange(collectionObject.GetFlatObjectsListWithInterface<T>(resultList));
                        }
                    }
                }
            }

            return retVal.ToArray();
        }

        public static T CreateInstanceFromType<T>(this Type type, params object[] parameters)
        {
            var instance = (T) Activator.CreateInstance(type, parameters);

            return instance;
        }

        public static bool IsDictionary(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var retVal = typeof(IDictionary).IsAssignableFrom(type);
            if (!retVal)
            {
                retVal = type.IsGenericType && typeof(IDictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition());
            }

            return retVal;
        }

        //https://stackoverflow.com/a/39679855/581476
        public static async Task<object> InvokeAsync(this MethodInfo methodInfo, object obj, params object[] parameters)
        {
            dynamic awaitable = methodInfo.Invoke(obj, parameters);
            return await awaitable;
        }

        public static T CastTo<T>(this object o) => (T)o;
        //https://stackoverflow.com/a/55852845/581476
        public static dynamic CastToReflected(this object o, Type type)
        {
            var methodInfo = typeof(ReflectionHelper).GetMethod(nameof(CastTo), BindingFlags.Static | BindingFlags.Public);
            var genericArguments = new[] { type };
            var genericMethodInfo = methodInfo?.MakeGenericMethod(genericArguments);
            return genericMethodInfo?.Invoke(null, new[] { o });
        }

        public static bool IsAssignableFromGenericList(this Type type)
        {
            foreach (var intType in type.GetInterfaces())
            {
                if (intType.IsGenericType
                    && intType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    return true;
                }
            }

            return false;
        }
    }
}