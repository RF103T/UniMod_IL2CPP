using System;
using System.Collections.Generic;
using System.Reflection;
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
using AutoList = System.Collections.Generic.List<object>;
#else
using AutoList = ILRuntime.Other.UncheckedList<object>;
#endif
namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

//will auto register in unity
#if UNITY_5_3_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static private void RegisterBindingAction()
        {
            ILRuntime.Runtime.CLRBinding.CLRBindingUtils.RegisterBindingAction(Initialize);
        }


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            System_Byte_Binding.Register(app);
            Katas_UniMod_IMod_Binding.Register(app);
            System_String_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            Cysharp_Threading_Tasks_UniTaskVoid_Binding.Register(app);
            Cysharp_Threading_Tasks_CompilerServices_AsyncUniTaskVoidMethodBuilder_Binding.Register(app);
            UnityEngine_AddressableAssets_Addressables_Binding.Register(app);
            System_Collections_Generic_List_1_String_Binding.Register(app);
            Cysharp_Threading_Tasks_AddressablesAsyncExtensions_Binding.Register(app);
            Cysharp_Threading_Tasks_UniTask_1_IList_1_Material_Binding_Awaiter_Binding.Register(app);
            Cysharp_Threading_Tasks_UniTask_1_GameObject_Binding_Awaiter_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            System_Collections_Generic_List_1_Material_Binding.Register(app);
            Cysharp_Threading_Tasks_UniTask_1_SceneInstance_Binding_Awaiter_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            UnityEngine_Input_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            UnityEngine_Mathf_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            UnityEngine_Renderer_Binding.Register(app);
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
