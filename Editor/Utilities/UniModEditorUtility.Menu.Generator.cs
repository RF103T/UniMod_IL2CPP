using ILRuntime.Runtime.Enviorment;

namespace Katas.UniMod.Editor
{
    public static partial class UniModEditorUtility
    {
        private const string GeneratorMenu = "UniMod/Generator";

        [UnityEditor.MenuItem(GeneratorMenu + "/ILRuntime/Generate CLR Binding Code by Analysis")]
        static void GenerateCLRBindingByAnalysis()
        {
            //用新的分析热更dll调用引用来生成绑定代码
            AppDomain domain = new AppDomain();
            using (System.IO.FileStream fs = new System.IO.FileStream("Library/ScriptAssemblies/Katas.UniMod.ModExample.dll", System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                domain.LoadAssembly(fs);

                //Crossbind Adapter is needed to generate the correct binding code
                InitILRuntime(domain);
                ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, "Assets/UniMod/Runtime/ILRuntime/Generated");
            }

            UnityEditor.AssetDatabase.Refresh();
        }

        static void InitILRuntime(AppDomain appDomain)
        {
            appDomain.RegisterCrossBindingAdaptor(new ModStartupAdapter());
            appDomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
            appDomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdapter());
        }
    }
}