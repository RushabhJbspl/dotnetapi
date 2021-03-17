using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;

namespace Worldex.Infrastructure.EFLocalizer
{
    public class EFStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly WorldexContext _context;

        public EFStringLocalizerFactory(WorldexContext context)
        {
            _context = context;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new EFStringLocalizer(_context);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new EFStringLocalizer(_context);
        }
    }
}
