using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
using AutoList = System.Collections.Generic.List<object>;
#else
using AutoList = ILRuntime.Other.UncheckedList<object>;
#endif
namespace ILRuntime.Runtime.Generated
{
    unsafe class UnityEngine_AddressableAssets_Addressables_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityEngine.AddressableAssets.Addressables);
            args = new Type[]{typeof(UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator), typeof(System.String), typeof(UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation)};
            method = type.GetMethod("AddResourceLocator", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddResourceLocator_0);
            Dictionary<string, List<MethodInfo>> genericMethods = new Dictionary<string, List<MethodInfo>>();
            List<MethodInfo> lst = null;                    
            foreach(var m in type.GetMethods())
            {
                if(m.IsGenericMethodDefinition)
                {
                    if (!genericMethods.TryGetValue(m.Name, out lst))
                    {
                        lst = new List<MethodInfo>();
                        genericMethods[m.Name] = lst;
                    }
                    lst.Add(m);
                }
            }
            args = new Type[]{typeof(UnityEngine.Material)};
            if (genericMethods.TryGetValue("LoadAssetsAsync", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<UnityEngine.Material>>), typeof(System.Collections.IEnumerable), typeof(System.Action<UnityEngine.Material>), typeof(UnityEngine.AddressableAssets.Addressables.MergeMode), typeof(System.Boolean)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, LoadAssetsAsync_1);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(UnityEngine.GameObject)};
            if (genericMethods.TryGetValue("LoadAssetAsync", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.GameObject>), typeof(System.Object)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, LoadAssetAsync_2);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(System.Object), typeof(UnityEngine.SceneManagement.LoadSceneMode), typeof(System.Boolean), typeof(System.Int32)};
            method = type.GetMethod("LoadSceneAsync", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LoadSceneAsync_3);


        }


        static StackObject* AddResourceLocator_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation @remoteCatalogLocation = (UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation)typeof(UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @localCatalogHash = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator @locator = (UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator)typeof(UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            UnityEngine.AddressableAssets.Addressables.AddResourceLocator(@locator, @localCatalogHash, @remoteCatalogLocation);

            return __ret;
        }

        static StackObject* LoadAssetsAsync_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @releaseDependenciesOnFailure = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.AddressableAssets.Addressables.MergeMode @mode = (UnityEngine.AddressableAssets.Addressables.MergeMode)typeof(UnityEngine.AddressableAssets.Addressables.MergeMode).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Action<UnityEngine.Material> @callback = (System.Action<UnityEngine.Material>)typeof(System.Action<UnityEngine.Material>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Collections.IEnumerable @keys = (System.Collections.IEnumerable)typeof(System.Collections.IEnumerable).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync<UnityEngine.Material>(@keys, @callback, @mode, @releaseDependenciesOnFailure);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* LoadAssetAsync_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @key = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<UnityEngine.GameObject>(@key);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* LoadSceneAsync_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @priority = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Boolean @activateOnLoad = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.SceneManagement.LoadSceneMode @loadMode = (UnityEngine.SceneManagement.LoadSceneMode)typeof(UnityEngine.SceneManagement.LoadSceneMode).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Object @key = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(@key, @loadMode, @activateOnLoad, @priority);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
