﻿namespace Maxisoft.Utils.Collection.UpdateGuard
{
    public interface IUpdateGuarded
    {
        ref int GetInternalVersionCounter();
    }
}