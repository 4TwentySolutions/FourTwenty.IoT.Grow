namespace GrowIoT.ViewModels
{
    public class EntityViewModel<T> where T : new()
    {

        public T DbEntity { get; private set; } = new T();
        public void SetEntity(T entity) => DbEntity = entity;
    }
}
