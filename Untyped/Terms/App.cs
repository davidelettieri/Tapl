using Common;

namespace Untyped.Terms;

/// <summary>
/// Application term (xy)
/// </summary>
/// <param name="Left">The first term in the application</param>
/// <param name="Right">The second term in the application</param>
public record App(ITerm Left, ITerm Right) : ITerm;