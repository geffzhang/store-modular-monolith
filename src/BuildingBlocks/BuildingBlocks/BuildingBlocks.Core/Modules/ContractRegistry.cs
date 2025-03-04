using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.Modules
{
    internal class ContractRegistry : IContractRegistry
    {
        private readonly ISet<Type> _contracts = new HashSet<Type>();
        private readonly IDictionary<string, (Type, Type)> _paths = new Dictionary<string, (Type, Type)>();
        private readonly IModuleRegistry _moduleRegistry;
        private readonly ILogger<ContractRegistry> _logger;
        private List<Assembly> _assemblies;
        private List<Type> _types;

        public ContractRegistry(IModuleRegistry moduleRegistry, ILogger<ContractRegistry> logger)
        {
            _moduleRegistry = moduleRegistry;
            _logger = logger;
            LoadTypes();
        }

        public IContractRegistry Register<T>() where T : class
        {
            var contract = GetContractType<T>();
            _contracts.Add(contract);
            return this;
        }

        public IContractRegistry RegisterPath(string path)
            => RegisterPath<Empty, Empty>(path);

        public IContractRegistry RegisterPathWithRequest<TRequest>(string path) where TRequest : class
            => RegisterPath<TRequest, Empty>(path);

        public IContractRegistry RegisterPathWithResponse<TResponse>(string path) where TResponse : class
            => RegisterPath<Empty, TResponse>(path);
        
        public IContractRegistry RegisterPath<TRequest, TResponse>(string path)
            where TRequest : class where TResponse : class
        {
            if (path == null)
            {
                throw new ContractException("Path cannot be null.");
            }

            if (_paths.ContainsKey(path))
            {
                throw new ContractException($"Path: '{path}' is already registered.");
            }
            
            var requestContract = GetContractType<TRequest>();
            var responseContract = GetContractType<TResponse>();
            _paths.Add(path, (requestContract, responseContract));
            return this;
        }

        private Type GetContractType<T>() where T : class
        {
            var contractType = typeof(Contract<T>);
            var requestContract = _types.SingleOrDefault(x => x.BaseType == contractType && !x.IsAbstract);
            if (requestContract is {})
            {
                return requestContract;
            }
            
            var type = typeof(T);
            var module = type.GetModuleName();
            throw new ContractException($"Contract was not found for: '{type.Name}' in module: '{module}'.");
        }

        public void Validate()
        {
            if (_assemblies.Count == 0 || _types.Count == 0)
            {
                LoadTypes();
            }
            
            ValidateContracts();
            ValidatePaths();
            ClearTypes();
        }

        private void ValidatePaths()
        {
            foreach (var (path, (requestType, responseType)) in _paths)
            {
                var registration = _moduleRegistry.GetRequestRegistration(path);
                if (registration is null)
                {
                    throw new ContractException($"Request registration was not found for path: '{path}'.");
                }

                _logger.LogTrace($"Validating the contracts for path: '{path}'...");
                if (requestType != typeof(Empty))
                {
                    ValidateContract(requestType, path);
                }

                if (responseType != typeof(Empty))
                {
                    ValidateContract(responseType, path);
                }

                _logger.LogTrace($"Validated the contracts for path: '{path}'.");
            }
        }

        private void ValidateContracts()
        {
            foreach (var contractType in _contracts)
            {
                ValidateContract(contractType);
            }
        }

        private void ValidateContract(Type contractType, string path = null)
        {
            var contract = Activator.CreateInstance(contractType) as IContract;
            var contractModule = contract.GetModuleName();
            var messageAttribute = contract?.Type.GetCustomAttribute<MessageAttribute>();
            if (messageAttribute is null || !messageAttribute.Enabled)
            {
                return;
            }

            var contractName = contract.Type.Name;
            var module = messageAttribute.Module;
            var originalType = _types
                .Where(x => x.FullName is {} &&
                            x.FullName.Contains($"Trill.Modules.{module}", StringComparison.InvariantCultureIgnoreCase))
                .SingleOrDefault(x => x.Name == contractName);

            if (originalType is null)
            {
                throw new ContractException($"Contract: '{contractName}' was not found in module: '{module}'.");
            }

            _logger.LogTrace($"Validating the contract for: '{contractName}', " +
                             $"from module: '{contractModule}', original module: '{module}'...");

            var originalContract = FormatterServices.GetUninitializedObject(originalType);
            var originalContractType = originalContract.GetType();
            foreach (var propertyName in contract.Required)
            {
                var localProperty = GetProperty(contract.Type, propertyName, contractName, module,
                    contractModule, path);
                var originalProperty = GetProperty(originalContractType, propertyName, contractName, module,
                    contractModule, path);
                ValidateProperty(localProperty, originalProperty, propertyName, contractName, module,
                    contractModule, path);
            }

            _logger.LogTrace($"Successfully validated the contract for: '{contractName}', " +
                             $"from module: '{contractModule}', original module: '{module}'.");
        }

        private static void ValidateProperty(PropertyInfo localProperty, PropertyInfo originalProperty,
            string propertyName, string contractName, string module, string localModule, string path = null)
        {
            if (localProperty.PropertyType == typeof(string) && originalProperty.PropertyType == typeof(string))
            {
                return;
            }

            if (localProperty.PropertyType.IsClass && localProperty.PropertyType != typeof(string) &&
                originalProperty.PropertyType.IsClass &&
                originalProperty.PropertyType != typeof(string))
            {
                return;
            }

            if (localProperty.PropertyType == originalProperty.PropertyType)
            {
                return;
            }

            throw new ContractException($"Property: '{propertyName}' in contract: '{contractName}' (module: '{localModule}') " +
                                        $"from module: '{module}'{(path is null ? "" : $", path: '{path}'")}, has a different type " +
                                        $"(actual: '{originalProperty.PropertyType}', " +
                                        $"expected: '{localProperty.PropertyType}').");
        }

        private static PropertyInfo GetProperty(Type type, string name, string contractName, string module,
            string localModule, string path = null)
        {
            var originalName = name;
            while (true)
            {
                var nameParts = name.Split(".");
                var property = type.GetProperty(nameParts[0]);
                if (property is null)
                {
                    throw new ContractException($"Property: '{originalName}' was not found in " +
                                                $"contract: '{contractName}' (module: '{localModule}') from module: '{module}'" +
                                                $"{(path is null ? "." : $", path: '{path}'.")}");
                }

                if (property.PropertyType == typeof(string))
                {
                    return property;
                }

                if (nameParts.Length == 1)
                {
                    return property;
                }

                if (property.PropertyType.IsClass)
                {
                    type = property.PropertyType;
                    name = string.Join(".", nameParts.Skip(1));
                    continue;
                }
                
                type = property.PropertyType;
                name = string.Join(".", nameParts.Skip(1));
            }
        }

        private void LoadTypes()
        {
            _assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            _types = _assemblies.SelectMany(x => x.GetTypes()).ToList();
        }
        
        private void ClearTypes()
        {
            _assemblies.Clear();
            _types.Clear();
;        }
        
        private class Empty
        {
        }
        
        private class EmptyContract : Contract<Empty>
        {
        }

        private class ContractException : Exception
        {
            public ContractException(string message) : base(message)
            {
            }
        }
    }
}