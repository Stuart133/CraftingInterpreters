namespace LoxDotNet.Interpreting
{
    internal class Return : RuntimeException
    {
        internal object Value { get; }

        public Return(object value)
        {            
            Value = value;
        }
    }
}
