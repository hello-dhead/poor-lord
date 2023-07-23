using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Compilation;
using poorlord;
using UnityEngine;

namespace Assets.Scripts.Commons
{
    public class TypeUtility
    {
        /// <summary>
        /// 해당 타입을 상속받는 모든 타입을 리턴
        /// </summary>
        public static List<Type> GetTypesWithBaseType(Type baseType)
        {
            var playerAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);
            List<Type> results = new List<Type>();
            foreach (var playerAssembly in playerAssemblies)
            {
                var assembly = System.Reflection.Assembly.Load(playerAssembly.name);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.BaseType != null && type.BaseType.Name == baseType.Name)
                    {
                        results.Add(type);
                    }
                }
            }
            return results;
        }
    }
}
