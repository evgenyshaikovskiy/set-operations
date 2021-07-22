using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sets
{
    // Comparer for Contains
    internal class SetElemComparer : EqualityComparer<ISetElement>
    {
        public override bool Equals(ISetElement x, ISetElement y) => x.Equals(y);

        public override int GetHashCode([DisallowNull] ISetElement obj) => throw new NotImplementedException();
    }
}
