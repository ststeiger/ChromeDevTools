
#define DOTNET4


namespace System
{


    public static class TypeExtensions
    {


#if DOTNET4

        public static System.Type GetTypeInfo(this System.Type t)
        {
            return t;
        }


        public static object[] GetCustomAttributes(this System.Type t, System.Type tt)
        {
            return t.GetCustomAttributes(tt, true);
        }


#endif


        // https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Type.cs#L61
        // public virtual Type[] GenericTypeArguments => (IsGenericType && !IsGenericTypeDefinition) ? GetGenericArguments() : Array.Empty<Type>();
        public static System.Type[] GetGenericTypeArguments(this System.Type t)
        {
#if DOTNET4
            
            if (t.IsGenericType && !t.IsGenericTypeDefinition)
                return t.GetGenericArguments();

            return new System.Type[0];
#else
            return t.GenericTypeArguments;
#endif
        }


    }


}
