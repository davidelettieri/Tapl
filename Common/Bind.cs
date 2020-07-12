using Common;

namespace Common
{
    public class Bind : ICommand
    {
        public IInfo Info { get; }
        public string Name { get; }
        public IBinding Binding { get; }

        public Bind(IInfo info, string name, IBinding binding)
        {
            Name = name;
            Info = info;
            Binding = binding;
        }
    }
}
