using System;
using Cysharp.Threading.Tasks;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using Katas.UniMod;

public class ModStartupAdapter : CrossBindingAdaptor
{
    //定义访问方法的方法信息
    static CrossBindingMethodInfo<Katas.UniMod.IMod> m_AbMethod1_0 = new CrossBindingMethodInfo<Katas.UniMod.IMod>("StartAsync");
    public override Type BaseCLRType
    {
        get
        {
            return typeof(Katas.UniMod.ModStartup);//这里是你想继承的类型
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adapter);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adapter(appdomain, instance);
    }

    public class Adapter : Katas.UniMod.ModStartup, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adapter()
        {

        }

        public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } }

        public override async UniTaskVoid StartAsync(IMod mod)
        {
            m_AbMethod1_0.Invoke(this.instance, mod);
        }

        public override string ToString()
        {
            IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
            m = instance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod)
            {
                return instance.ToString();
            }
            else
                return instance.Type.FullName;
        }
    }
}