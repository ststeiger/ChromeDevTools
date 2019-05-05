
#define DOTNET4


#if DOTNET4 

namespace System.Runtime.Serialization
{


    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumMemberAttribute 
        : Attribute
    {
        internal bool isValueSetExplicitly;
        internal string value;
        

        public EnumMemberAttribute()
        { }


        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                this.isValueSetExplicitly = true;
            }
        }


        public bool IsValueSetExplicitly
        {
            get
            {
                return this.isValueSetExplicitly;
            }
        }


    }


}

#endif 
