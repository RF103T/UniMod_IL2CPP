using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using Katas.UniMod;

public class IAsyncStateMachineAdapter : CrossBindingAdaptor
{
    //定义访问方法的方法信息
    static CrossBindingMethodInfo m_AbMethod1_0 = new CrossBindingMethodInfo("MoveNext");
    static CrossBindingMethodInfo<System.Runtime.CompilerServices.IAsyncStateMachine> m_AbMethod1_1 = new CrossBindingMethodInfo<System.Runtime.CompilerServices.IAsyncStateMachine>("SetStateMachine");
    public override Type BaseCLRType
    {
        get
        {
            return typeof(System.Runtime.CompilerServices.IAsyncStateMachine);//这里是你想继承的类型
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

    public class Adapter : System.Runtime.CompilerServices.IAsyncStateMachine, CrossBindingAdaptorType
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

        public void MoveNext()
        {
            m_AbMethod1_0.Invoke(this.instance);
        }

        public void SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine arg1)
        {
            m_AbMethod1_1.Invoke(this.instance, arg1);
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