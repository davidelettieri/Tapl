using Common;

namespace Common
{
    public class Bind : ICommand
    {
        public string Name { get; }

        public Bind(string name)
        {
            Name = name;
        }
    }
}
