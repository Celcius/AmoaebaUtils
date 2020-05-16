 using UnityEngine;
 using System.Collections;
 using System.Collections.Generic;
 
 namespace AmoaebaUtils
 {
 public static class ReflectionHelpers
 {
     public static System.Type[] GetAllDerivedTypes(System.Type aType)
     {
        return GetAllDerivedTypes(System.AppDomain.CurrentDomain, aType);
     }

     public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
     {
         var result = new List<System.Type>();
         var assemblies = aAppDomain.GetAssemblies();
         foreach (var assembly in assemblies)
         {
             var types = assembly.GetTypes();
             foreach (var type in types)
             {
                 if (type.IsSubclassOf(aType))
                     result.Add(type);
             }
         }
         return result.ToArray();
     }

     public static System.Type GetTypeFromName(string aType)
     {
        return GetTypeFromName(System.AppDomain.CurrentDomain, aType);
     }

     public static System.Type GetTypeFromName(this System.AppDomain aAppDomain, string aType)
     {
         var assemblies = aAppDomain.GetAssemblies();
         
         foreach (var assembly in assemblies)
         {
            System.Type type = assembly.GetType(aType);
            if(type != null)
            {
                return type;
            }
         }
         return null;
     }
 }
}