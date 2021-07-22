using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sets
{
    public class Set : ISetElement, IEnumerable<ISetElement>, ICollection<ISetElement>
    {
        private List<ISetElement> elements;

        public Set(bool isOrdered, int capacity)
        {
            this.elements = new List<ISetElement>(capacity);
            this.IsOrdered = isOrdered;
        }

        public Set(bool isOrdered)
        {
            this.elements = new List<ISetElement>();
            this.IsOrdered = isOrdered;
        }

        public bool IsOrdered { get; internal set; }

        public int Count => this.elements.Count;

        public int Capacity => this.elements.Capacity;

        public bool IsReadOnly => false;

        public ISetElement this[int index] { get => this.elements[index]; }

        // basic operations
        public static Set operator *(Set a, Set b) // симметрическая разность
        {
            Set c = new Set(false);
            c = (a - b) + (b - a);
            return c;
        }

        public static Set operator -(Set a, Set b) // пересечение
        {
            Set c = new Set(false);
            for (int i = 0; i < a.Count; i++)
            {
                if (b.Contains(a[i].Dupplicate()))
                {
                    int resultCount = a.CountOf(a[i].Dupplicate()) - b.CountOf(a[i].Dupplicate());
                    if (resultCount > 0 && c.CountOf(a[i].Dupplicate()) < resultCount)
                    {
                        for (int j = 0; j < resultCount; j++)
                        {
                            c.Add(a[i].Dupplicate());
                        }
                    }
                }
                else
                {
                    c.Add(a[i].Dupplicate());
                }
            }

            return c;
        }

        public static Set operator +(Set a, Set b) // объединение
        {
            for (int i = 0; i < b.Count; i++)
            {
                if (a.Contains(b[i].Dupplicate()))
                {
                    // check countof
                    int resultCount = b.CountOf(b[i].Dupplicate()) - a.CountOf(b[i].Dupplicate()); // if c already contain neeeded number, else..
                    if (resultCount > 0)
                    {
                        for (int j = 0; j < b.CountOf(b[i].Dupplicate()); j++)
                        {
                            a.Add(b[i].Dupplicate());
                        }
                    }
                }
                else
                {
                    a.Add(b[i].Dupplicate());
                }
            }

            return a;
        }

        // methods
        public bool Equals(ISetElement setElement) // check equality between elem and "this"
        {
            if (setElement is Set)
            {
                Set set = (Set)setElement;

                if (this.Count != set.Count)
                {
                    return false; // Element count mismatch
                }

                if (this.IsOrdered)
                {
                    if (!set.IsOrdered)
                    {
                        return false; // Order propery mismatch
                    }

                    // 2 ordered sets
                    for (int i = 0; i < this.Count; i++)
                    {
                        if (this[i].Equals(set[i]))
                        {
                            continue;
                        }

                        return false;
                    }

                    return true;
                }
                else
                {
                    if (set.IsOrdered)
                    {
                        return false; // Order propery mismatch
                    }

                    for (int i = 0; i < this.Count; i++)
                    {
                        if (this.CountOf(this[i]) == set.CountOf(this[i]))
                        {
                            continue;
                        }

                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            string ret;

            if (this.IsOrdered)
            {
                if (this.Count == 0)
                {
                    return Consts.StrSetOrderedEmpty;
                }

                ret = this.elements[0].ToString();
                for (int i = 1; i < this.Count; i++)
                {
                    ret += Consts.StrSetOrderedSep + this.elements[i].ToString();
                }

                return $"{Consts.StrSetOrderedBegin}{ret}{Consts.StrSetOrderedEnd}";
            }
            else
            {
                if (this.Count == 0)
                {
                    return Consts.StrSetUnorderedEmpty;
                }

                ret = this.elements[0].ToString();
                for (int i = 1; i < this.Count; i++)
                {
                    ret += Consts.StrSetUnorderedSep + this.elements[i].ToString();
                }

                return $"{Consts.StrSetUnorderedBegin}{ret}{Consts.StrSetUnorderedEnd}";
            }
        }

        public ISetElement Dupplicate()
        {
            Set ret = new Set(this.IsOrdered, this.Capacity);
            foreach (ISetElement item in this.elements)
            {
                ret.Add(item.Dupplicate());
            }

            return ret;
        }

        public void Add(ISetElement item) => this.elements.Add(item);

        public void Add(string item) => this.Add(new SetValueElem(item));

        public void Add(params ISetElement[] items)
        {
            foreach (ISetElement item in items)
            {
                this.elements.Add(item);
            }
        }

        public void Clear() => this.elements.Clear();

        public bool Contains(ISetElement item) => this.elements.Contains(item, new SetElemComparer());

        public bool Contains(string value) => this.Contains(new SetValueElem(value));

        public void CopyTo(ISetElement[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int CountOf(ISetElement setElem)
        {
            int ret = 0;
            foreach (var item in this.elements)
            {
                if (setElem.Equals(item))
                {
                    ret++;
                }
            }

            return ret;
        }

        public int CountOf(string value) => this.CountOf(new SetValueElem(value));

        public IEnumerator<ISetElement> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }

        public bool Remove(ISetElement item)
        {
            this.elements.Remove(item);
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }
    }
}
