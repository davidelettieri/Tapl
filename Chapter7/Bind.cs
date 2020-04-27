using Common;

namespace Chapter7
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
