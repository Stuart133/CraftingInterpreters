namespace LoxDotNet.Interpreting
{
    class LoxClass
    {
        internal string Name { get; }

        internal LoxClass(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
