using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Repro.Wasm;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

Console.WriteLine("Hello, World!");
AbstractType rec = new ConcreteType(4);
var jsonSerializedRec = JsonSerializer.Serialize(rec);
var deserializedRec = JsonSerializer.Deserialize<AbstractType>(jsonSerializedRec);
Console.WriteLine("Done!");

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();

public interface IType
{
    int Value();
}

[JsonSerializable(typeof(AbstractType))]
[JsonDerivedType(typeof(ConcreteType), nameof(ConcreteType))]
[JsonDerivedType(typeof(OtherConcreteType), nameof(OtherConcreteType))]
public abstract record AbstractType : IType, IAdditionOperators<AbstractType, AbstractType, AbstractType>,
    ISubtractionOperators<AbstractType, AbstractType, AbstractType>,
    IMultiplyOperators<AbstractType, AbstractType, AbstractType>,
    IDivisionOperators<AbstractType, AbstractType, AbstractType>
{
    public static AbstractType operator +(AbstractType left, AbstractType right)
    {
        return (left, right) switch
        {
            (ConcreteType l, ConcreteType r) => new ConcreteType((l.Order + r.Order) % 4),
            _ => throw new NotImplementedException()
        };
    }

    public static AbstractType operator /(AbstractType left, AbstractType right)
    {
        return (left, right) switch
        {
            (ConcreteType l, ConcreteType r) => new ConcreteType((l.Order + r.Order) % 4),
            _ => throw new NotImplementedException()
        };
    }

    public static AbstractType operator *(AbstractType left, AbstractType right)
    {
        return (left, right) switch
        {
            (ConcreteType l, ConcreteType r) => new ConcreteType((l.Order - r.Order) % 4),
            _ => throw new NotImplementedException()
        };
    }

    public static AbstractType operator -(AbstractType left, AbstractType right)
    {
        return (left, right) switch
        {
            (ConcreteType l, ConcreteType r) => new ConcreteType((l.Order - r.Order) % 4),
            _ => throw new NotImplementedException()
        };
    }

    /// <inheritdoc />
    public abstract int Value();
}

[JsonSerializable(typeof(OtherConcreteType))]
public sealed record OtherConcreteType(int OtherOrder) : AbstractType
{
    public override int Value()
    {
        return OtherOrder;
    }
}

[JsonSerializable(typeof(ConcreteType))]
public sealed record ConcreteType : AbstractType
{
    [JsonIgnore] private readonly Lazy<int> somePreComputedValue;

    [JsonConstructor]
    public ConcreteType(int order)
    {
        Order = order;
        somePreComputedValue = new Lazy<int>(() => order);
    }

    public int Order { get; init; }

    public override int Value()
    {
        return somePreComputedValue.Value;
    }
}