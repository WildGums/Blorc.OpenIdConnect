namespace Blorc.OpenIdConnect.Tests
{
    using System.Reflection;

    public class Stub<T>
    {
        public T Instance { get; }

        public Stub(T instance)
        {
            Instance = instance;
        }

        public void SetField(string fieldName, object value)
        {
            var field = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field is not null)
            {
                field.SetValue(Instance, value);
            }
        }
    }
}
