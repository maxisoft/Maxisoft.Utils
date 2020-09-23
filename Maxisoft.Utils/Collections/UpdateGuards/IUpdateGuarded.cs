namespace Maxisoft.Utils.Collections.UpdateGuards
{
    public interface IUpdateGuarded
    {
        ref int GetInternalVersionCounter();
    }
}