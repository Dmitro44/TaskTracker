namespace TaskTracker.Application.Interfaces.Mapping;

public interface IGenericMapper<TFirst, TSecond>
{
    TSecond ToEntity(TFirst source);
    TFirst ToDto(TSecond source);
    void MapPartial(TFirst source, TSecond destination);
}