using System;

namespace Statistics.Data
{
    public class NamedDatapoint<T> : Datapoint where T : struct 
    {
        internal NamedDatapoint(string name, T value)
        {
            Name = name;
            Value = value;
        }

        internal NamedDatapoint(Enum nameEnum, T value) : this(nameEnum.ToString(), value)
        {
        }

        public string Name { get; }

        public T Value { get; }
    }
}
