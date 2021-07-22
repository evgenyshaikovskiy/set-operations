using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sets
{
    public class SetValueElem : ISetElement
    {
        private string value;

        public SetValueElem(string value)
        {
            this.value = value;
        }

        public ISetElement this[int index] { get => throw new InvalidOperationException("Cannot index a set value element"); }

        public bool Equals(ISetElement elem)
        {
            if (elem is SetValueElem)
            {
                return this.value == ((SetValueElem)elem).value;
            }

            return false;
        }

        public override string ToString() => this.value;

        // Aliases for Duppl
        public ISetElement Dupplicate()
        {
            return new SetValueElem(this.value);
        }
    }
}
