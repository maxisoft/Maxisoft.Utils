﻿namespace Maxisoft.Utils.Collections.UpdateGuard
{
    public interface IUpdateGuarded
    {
        ref int GetInternalVersionCounter();
    }
}