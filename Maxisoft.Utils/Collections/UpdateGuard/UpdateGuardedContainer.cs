﻿using System;

 namespace Maxisoft.Utils.Collections.UpdateGuard
{
    [Serializable]
    public class UpdateGuardedContainer : IUpdateGuarded
    {
        private int _version;

        public int Version
        {
            get => _version;
            set => _version = value;
        }


        public ref int GetInternalVersionCounter()
        {
            return ref _version;
        }

        public UpdateGuard<UpdateGuardedContainer> CreateGuard(bool increment = false)
        {
            return new UpdateGuard<UpdateGuardedContainer>(this) {PostIncrementVersionCounter = increment};
        }
    }
}