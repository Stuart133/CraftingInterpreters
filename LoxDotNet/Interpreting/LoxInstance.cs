namespace LoxDotNet.Interpreting
{
    class LoxInstance
    {
        private readonly LoxClass _loxClass;

        public LoxInstance(LoxClass loxClass)
        {
            _loxClass = loxClass;
        }

        public override string ToString()
        {
            return $"{_loxClass.Name} instance";
        }
    }
}
