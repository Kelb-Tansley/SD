namespace SD.Data.Interfaces;
public interface IEntityMapper<T, F>
{
    T Map(F fromModel);
    IEnumerable<T> MapAll(IEnumerable<F> fromModel);
}