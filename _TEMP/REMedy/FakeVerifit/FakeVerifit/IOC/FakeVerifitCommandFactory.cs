using System.Collections.Generic;
using System.Linq;

namespace FakeVerifit
{
    /// <summary>
    /// AudioscanAPI 3.0
    /// </summary>
    public class FakeVerifitCommandFactory
    {
        private readonly IEnumerable<IFakeVerifitCommandCreator> creators;

        /// <inheritdoc />
        public FakeVerifitCommandFactory(IEnumerable<IFakeVerifitCommandCreator> creators)
        {
            this.creators = creators;
        }

        public IFakeVerifitCommand TryFindCommand(string command)
        {
            return this.creators
                .Select(creator => creator.Create(command))
                .FirstOrDefault(cmd => cmd != null);
        }
    }
}