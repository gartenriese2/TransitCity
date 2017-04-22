using System;

namespace Statistics.Data
{
    public class NamedDatapoint<T> : IDatapoint where T : struct 
    {
        public NamedDatapoint(string name, T value)
        {
            Name = name;
            Value = value;
        }

        public NamedDatapoint(Enum nameEnum, T value) : this(nameEnum.ToString(), value)
        {
        }

        public string Name { get; }

        public T Value { get; }
    }
}
