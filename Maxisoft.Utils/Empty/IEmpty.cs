namespace Maxisoft.Utils.Empty
{
    /// <summary>
    ///     The implementation should try to suit the following behaviors:
    ///     <list type="bullet">
    ///         <item>its methods doing nothing or acting as passively as possible</item>
    ///         <item>it is default constructible eg <c>new()</c></item>
    ///         <item>it don't store any <c>object</c></item>
    ///         <item>its <c>sizeof</c>() should be close to <c>0</c></item>
    ///     </list>
    ///     tl;dr the implementation should be as useless as possible (without wasting resources)
    /// </summary>
    public interface IEmpty
    {
    }
}