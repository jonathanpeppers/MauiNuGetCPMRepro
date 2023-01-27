using Microsoft.Extensions.Logging;

namespace Repro.Lib;
public class Class1
{
  private readonly ILogger<Class1> logger;

  public Class1(ILogger<Class1> logger) => this.logger = logger;
}
