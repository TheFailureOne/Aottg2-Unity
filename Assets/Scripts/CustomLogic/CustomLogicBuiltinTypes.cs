﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CustomLogic
{
    public class CustomLogicBuiltinTypes
    {
        private static CustomLogicBuiltinTypes instance;

        private static CustomLogicBuiltinTypes Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = new CustomLogicBuiltinTypes();
                instance.Init();
                return instance;
            }
        }

        private Dictionary<string, Type> _types;
        private Dictionary<string, string> _baseTypeNames;
        private Dictionary<string, HashSet<string>> _typeMemberNames;
        
        /// <summary>
        /// Map of all the builtin type names to their types.
        /// Only types that are marked with <see cref="CLTypeAttribute"/> are included
        /// </summary>
        public static Dictionary<string, Type> Types => Instance._types;
        
        /// <summary>
        /// Map of all the builtin type names to their base (builtin) type names.
        /// </summary>
        public static Dictionary<string, string> BaseTypeNames => Instance._baseTypeNames;
        
        /// <summary>
        /// Map of all the builtin type names to their member names.
        /// Only members that are marked with <see cref="CLPropertyAttribute"/> or <see cref="CLMethodAttribute"/>
        /// are included
        /// </summary>
        public static Dictionary<string, HashSet<string>> TypeMemberNames => Instance._typeMemberNames;
        
        public static bool IsBuiltinType(string typeName)
        {
            return Types.ContainsKey(typeName);
        }
        
        private void Init()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.HasAttribute<CLTypeAttribute>())
                .ToArray();

            _types = new Dictionary<string, Type>(types.Length);
            _baseTypeNames = new Dictionary<string, string>(types.Length);
            _typeMemberNames = new Dictionary<string, HashSet<string>>(types.Length);
            
            foreach (var type in types)
            {
                var name = GetBuiltinTypeName(type);
                
                var baseType = type.BaseType;
                var isBaseTypeBuiltin = baseType != null && baseType.HasAttribute<CLTypeAttribute>();
                var baseTypeName = isBaseTypeBuiltin ? GetBuiltinTypeName(baseType) : null;
                
                if (isBaseTypeBuiltin)
                    _baseTypeNames[name] = baseTypeName;
                
                _types[name] = type;
                
                if (!_typeMemberNames.ContainsKey(name))
                    _typeMemberNames[name] = new HashSet<string>();

                const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                           BindingFlags.Static;
                
                foreach (var member in type.GetMembers(flags).Where(x => 
                             x.HasAttribute<CLPropertyAttribute>() 
                             || x.HasAttribute<CLMethodAttribute>()))
                {
                    if (isBaseTypeBuiltin)
                    {
                        if (_typeMemberNames[baseTypeName].Contains(member.Name))
                            continue;
                    }
                    
                    _typeMemberNames[name].Add(member.Name);
                }
            }
        }

        private static string GetBuiltinTypeName(Type type)
        {
            return type.Name.Replace("CustomLogic", "").Replace("Builtin", "");
        }
    }
}