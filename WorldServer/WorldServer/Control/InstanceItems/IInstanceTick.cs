﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Control
{
    public interface IInstanceTick
    {
        void Tick(float frameDiff);
    }
}
