﻿using System;

using Orion.Shared.Absorb.Clients;

namespace Orion.Shared.Absorb.DataSources
{
    internal class GnuSocialDataSource : BaseDataSource
    {
        private readonly GnuSocialClientWrapper _gnuSocialClient;

        public GnuSocialDataSource(GnuSocialClientWrapper gnuSocialClient)
        {
            _gnuSocialClient = gnuSocialClient;
        }

        protected override void Connect(Source source)
        {
            throw new NotImplementedException();
        }

        protected override string NormalizedSource(string source)
        {
            throw new NotImplementedException();
        }
    }
}